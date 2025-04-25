using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;

// by Chee Wee Chua,
// Singapore, Apr 2025

namespace chuacw.TelligentCommunity
{
    public class CaptchaImagePage : Page
    {
        private const string SessionID = "THCaptcha";

        internal const string DefaultBackColorName = "White";

        internal const string DefaultBackNoiseColorName = "LightGray";

        internal const string DefaultFontName = "Arial";

        internal const string DefaultForeColorName = "DarkGray";

        internal const string DefaultForeNoiseColorName = "LightGray";

        internal const int DefaultHeight = 50;

        internal const int DefaultNumberOfCharacters = 3;

        internal const CaptchaType DefaultType = CaptchaType.Numeric;

        internal const int DefaultWidth = 150;

        internal readonly static Color DefaultBackColor;

        internal readonly static Color DefaultBackNoiseColor;

        internal readonly static Color DefaultForeColor;

        internal readonly static Color DefaultForeNoiseColor;

        internal static string Code
        {
            get
            {
                HttpContext current = HttpContext.Current;
                if (current.Session != null)
                {
                    return current.Session["THCaptcha"] as string;
                }
                HttpCookie item = current.Request.Cookies["THCaptcha"];
                if (item == null)
                {
                    return null;
                }
                try
                {
                    FormsAuthenticationTicket formsAuthenticationTicket = FormsAuthentication.Decrypt(item.Value);
                    if (!formsAuthenticationTicket.Expired)
                    {
                        return formsAuthenticationTicket.UserData;
                    }
                }
                catch
                {
                }
                return null;
            }
            set
            {
                HttpContext current = HttpContext.Current;
                if (current.Session != null)
                {
                    current.Session["THCaptcha"] = value;
                    return;
                }
                SessionStateSection webApplicationSection = (SessionStateSection)WebConfigurationManager.GetWebApplicationSection("system.web/sessionState");
                FormsAuthenticationTicket formsAuthenticationTicket = new FormsAuthenticationTicket(1, string.Empty, DateTime.Now, DateTime.Now + webApplicationSection.Timeout, false, value);
                current.Response.Cookies["THCaptcha"].Value = FormsAuthentication.Encrypt(formsAuthenticationTicket);
            }
        }

        static CaptchaImagePage()
        {
            CaptchaImagePage.DefaultBackColor = Color.White;
            CaptchaImagePage.DefaultBackNoiseColor = Color.LightGray;
            CaptchaImagePage.DefaultForeColor = Color.DarkGray;
            CaptchaImagePage.DefaultForeNoiseColor = Color.LightGray;
        }

        public CaptchaImagePage()
        {
        }

        private string GenerateCode(CaptchaType type, int numberOfCharacters)
        {
            char chr;
            StringBuilder stringBuilder = new StringBuilder(numberOfCharacters);
            Random random = new Random();
            for (int i = 0; i < numberOfCharacters; i++)
            {
                if (type == CaptchaType.Numeric || type == CaptchaType.Alphanumeric && random.Next(2) == 0)
                {
                    stringBuilder.Append(random.Next((type == CaptchaType.Numeric ? 0 : 1), 10));
                }
                else
                {
                    do
                    {
                        chr = (char)random.Next(97, 123);
                    }
                    while (type == CaptchaType.Alphanumeric && chr == 'o');
                    stringBuilder.Append(chr);
                }
            }
            return stringBuilder.ToString();
        }

        protected override void OnLoad(EventArgs e)
        {
            Regex regex = new Regex("^[0-9A-Fa-f]{8}$");
            Regex regex1 = new Regex("^\\d+$");
            string item = base.Request.QueryString["ct"];
            CaptchaType captchaType = (item == null || !Enum.IsDefined(typeof(CaptchaType), item) ? CaptchaType.Numeric : (CaptchaType)Enum.Parse(typeof(CaptchaType), item));
            item = base.Request.QueryString["nc"];
            int num = (item == null || !regex1.IsMatch(item) ? 3 : Convert.ToInt32(item));
            if (num <= 0)
            {
                num = 3;
            }
            item = base.Request.QueryString["fc"];
            Color color = (item == null || !regex.IsMatch(item) ? CaptchaImagePage.DefaultForeColor : Color.FromArgb(Convert.ToInt32(item, 16)));
            item = base.Request.QueryString["fnc"];
            Color color1 = (item == null || !regex.IsMatch(item) ? CaptchaImagePage.DefaultForeNoiseColor : Color.FromArgb(Convert.ToInt32(item, 16)));
            item = base.Request.QueryString["bc"];
            Color color2 = (item == null || !regex.IsMatch(item) ? CaptchaImagePage.DefaultBackColor : Color.FromArgb(Convert.ToInt32(item, 16)));
            item = base.Request.QueryString["bnc"];
            Color color3 = (item == null || !regex.IsMatch(item) ? CaptchaImagePage.DefaultBackNoiseColor : Color.FromArgb(Convert.ToInt32(item, 16)));
            string str = (base.Request.QueryString["fn"] != null ? base.Request.QueryString["fn"] : "Arial");
            item = base.Request.QueryString["w"];
            int num1 = (item == null || !regex1.IsMatch(item) ? 150 : Convert.ToInt32(item));
            if (num1 <= 0)
            {
                num1 = 150;
            }
            item = base.Request.QueryString["h"];
            int num2 = (item == null || !regex1.IsMatch(item) ? 50 : Convert.ToInt32(item));
            if (num2 <= 0)
            {
                num2 = 50;
            }
            string str1 = this.GenerateCode(captchaType, num);
            CaptchaImagePage.Code = str1;
            using (CaptchaImage captchaImage = new CaptchaImage(str1, num1, num2, str, color, color1, color2, color3))
            {
                base.Response.Clear();
                base.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                base.Response.ContentType = "image/jpeg";
                captchaImage.Image.Save(base.Response.OutputStream, ImageFormat.Jpeg);
                base.Response.End();
            }
        }

        internal class Parameters
        {
            public const string BackColor = "bc";

            public const string BackNoiseColor = "bnc";

            public const string CaptchaType = "ct";

            public const string FontName = "fn";

            public const string ForeColor = "fc";

            public const string ForeNoiseColor = "fnc";

            public const string Height = "h";

            public const string NumberOfCharacters = "nc";

            public const string Width = "w";

            public Parameters()
            {
            }
        }
    }
}