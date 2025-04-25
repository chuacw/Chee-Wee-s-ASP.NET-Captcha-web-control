using chuacw.TelligentCommunity;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

// by Chee Wee Chua,
// Singapore, Apr 2025

namespace chuacw.WebControls
{
    [ValidationProperty("Message")]
    public class Captcha : BaseValidator, INamingContainer
    {
        internal const string DefaultImageSrc = "~/captchaimage.aspx";

        private const bool DefaultDisplayErrorMessage = true;

        private const string DefaultErrorColorName = "Red";

        private const string DefaultErrorMessage = "The text you entered was incorrect, please try again.";

        private const string DefaultErrorMessageTimeout = "The time limit to enter the text has expired, please try again. ";

        private const string DefaultMessage = "Enter the text you see in the image: ";

        private const string DefaultMessageAlphabetic = "Enter the letters you see in the image: ";

        private const string DefaultMessageNumeric = "Enter the numbers you see in the image: ";

        private const string DefaultReloadText = "Get new image";

        private const bool DefaultTestAuthenticatedUsers = false;

        private const string ValidatedContextKey = "thwcCaptchaValidated";

        protected System.Web.UI.WebControls.Image imgCaptcha;

        protected Label lblMessage;

        protected HyperLink linkReload;

        protected TextBox txtCode;

        protected Label lblError;

        private readonly static Color DefaultErrorColor;

        private ITemplate _InnerTemplate;

        [Bindable(true)]
        [Category("Captcha")]
        [DefaultValue(CaptchaType.Numeric)]
        [Description("Type of characters that should appear in the code.")]
        public CaptchaType CaptchaType
        {
            get
            {
                return (CaptchaType)this.ViewState["CaptchaType"];
            }
            set
            {
                this.ViewState["CaptchaType"] = value;
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("True if the error message should be displayed.")]
        public bool DisplayErrorMessage
        {
            get
            {
                return (bool)this.ViewState["DisplayErrorMessage"];
            }
            set
            {
                this.ViewState["DisplayErrorMessage"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("The text you entered was incorrect, please try again.")]
        [Description("Error message to display when the wrong code is entered.")]
        public new string ErrorMessage
        {
            get
            {
                return (string)this.ViewState["ErrorMessage"];
            }
            set
            {
                this.ViewState["ErrorMessage"] = value;
                if (this.lblError != null)
                {
                    this.lblError.Text = value;
                }
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Red")]
        [Description("Font color of the error message.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ErrorMessageColor
        {
            get
            {
                return (Color)this.ViewState["ErrorMessageColor"];
            }
            set
            {
                this.ViewState["ErrorMessageColor"] = value;
                if (this.lblError != null)
                {
                    this.lblError.ForeColor = value;
                }
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("The time limit to enter the text has expired, please try again. ")]
        [Description("Error message to display when the time limit to enter the code has been exceeded.")]
        public string ErrorMessageTimeout
        {
            get
            {
                return (string)this.ViewState["ErrorMessageTimeout"];
            }
            set
            {
                this.ViewState["ErrorMessageTimeout"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        [Description("Color of the image background.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ImageBackColor
        {
            get
            {
                return (Color)this.ViewState["ImageBackColor"];
            }
            set
            {
                this.ViewState["ImageBackColor"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "LightGray")]
        [Description("Color of the image background noise.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ImageBackNoiseColor
        {
            get
            {
                return (Color)this.ViewState["ImageBackNoiseColor"];
            }
            set
            {
                this.ViewState["ImageBackNoiseColor"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Arial")]
        [Description("Name of the font used in the image.")]
        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(FontConverter.FontNameConverter))]
        public string ImageFontName
        {
            get
            {
                return (string)this.ViewState["ImageFontName"];
            }
            set
            {
                this.ViewState["ImageFontName"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DarkGray")]
        [Description("Color of the image foreground.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ImageForeColor
        {
            get
            {
                return (Color)this.ViewState["ImageForeColor"];
            }
            set
            {
                this.ViewState["ImageForeColor"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "LightGray")]
        [Description("Color of the image foreground noise.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ImageForeNoiseColor
        {
            get
            {
                return (Color)this.ViewState["ImageForeNoiseColor"];
            }
            set
            {
                this.ViewState["ImageForeNoiseColor"] = value;
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(50)]
        [Description("Height of the image.")]
        public int ImageHeight
        {
            get
            {
                return (int)this.ViewState["ImageHeight"];
            }
            set
            {
                this.ViewState["ImageHeight"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("~/captchaimage.aspx")]
        [Description("Url of the image.")]
        public string ImageSrc
        {
            get
            {
                return (string)this.ViewState["ImageSrc"];
            }
            set
            {
                this.ViewState["ImageSrc"] = value;
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(150)]
        [Description("Width of the image.")]
        public int ImageWidth
        {
            get
            {
                return (int)this.ViewState["ImageWidth"];
            }
            set
            {
                this.ViewState["ImageWidth"] = value;
            }
        }

        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate InnerTemplate
        {
            get
            {
                return this._InnerTemplate;
            }
            set
            {
                this._InnerTemplate = value;
                base.ChildControlsCreated = false;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Description("Instructional text displayed to user.")]
        public string Message
        {
            get
            {
                return (string)this.ViewState["Message"];
            }
            set
            {
                this.ViewState["Message"] = value;
                if (this.lblMessage != null)
                {
                    this.lblMessage.Text = value;
                }
            }
        }

        [Bindable(true)]
        [Category("Captcha")]
        [DefaultValue(3)]
        [Description("Number of characters in the code.")]
        public int NumberOfCharacters
        {
            get
            {
                return (int)this.ViewState["NumberOfCharacters"];
            }
            set
            {
                this.ViewState["NumberOfCharacters"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Description("The URL of the CAPTCHA reload icon. Set to the empty string to display no icon, set to null for the default icon.")]
        public string ReloadImageUrl
        {
            get
            {
                return (string)this.ViewState["ReloadImageUrl"];
            }
            set
            {
                this.ViewState["ReloadImageUrl"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Get new image")]
        [Description("The text of the CAPTCHA reload icon. If the ReloadImageUrl property is set to a value this proprety becomes the icon's tooltip, otherwise it is displayed as text.")]
        public string ReloadText
        {
            get
            {
                return (string)this.ViewState["ReloadText"];
            }
            set
            {
                this.ViewState["ReloadText"] = value;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicates if the control should be visible to authenticated users.")]
        public bool TestAuthenticatedUsers
        {
            get
            {
                return (bool)this.ViewState["TestAuthenticatedUsers"];
            }
            set
            {
                this.ViewState["TestAuthenticatedUsers"] = value;
            }
        }

        static Captcha()
        {
            Captcha.DefaultErrorColor = Color.Red;
        }

        public Captcha()
        {
            this.CaptchaType = CaptchaType.Numeric;
            this.DisplayErrorMessage = true;
            this.ErrorMessage = "The text you entered was incorrect, please try again.";
            this.ErrorMessageColor = Captcha.DefaultErrorColor;
            this.ErrorMessageTimeout = "The time limit to enter the text has expired, please try again. ";
            this.ImageBackColor = CaptchaImagePage.DefaultBackColor;
            this.ImageBackNoiseColor = CaptchaImagePage.DefaultBackNoiseColor;
            this.ImageFontName = "Arial";
            this.ImageForeColor = CaptchaImagePage.DefaultForeColor;
            this.ImageForeNoiseColor = CaptchaImagePage.DefaultForeNoiseColor;
            this.ImageHeight = 50;
            this.ImageSrc = "~/captchaimage.aspx";
            this.ImageWidth = 150;
            this.NumberOfCharacters = 3;
            this.ReloadText = "Get new image";
            this.TestAuthenticatedUsers = false;
            base.Init += new EventHandler(this.Captcha_Init);
            base.PreRender += new EventHandler(this.Captcha_PreRender);
        }

        private void Captcha_Init(object sender, EventArgs e)
        {
            base.ControlToValidate = this.ID;
        }

        private void Captcha_PreRender(object sender, EventArgs e)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            StringBuilder stringBuilder = new StringBuilder();
            if (this.Context.User.Identity.IsAuthenticated && !this.TestAuthenticatedUsers)
            {
                this.Visible = false;
                return;
            }
            if (this.CaptchaType != CaptchaType.Numeric)
            {
                nameValueCollection["ct"] = this.CaptchaType.ToString();
            }
            if (this.NumberOfCharacters != 3)
            {
                nameValueCollection["nc"] = this.NumberOfCharacters.ToString();
            }
            if (this.ImageWidth != 150)
            {
                nameValueCollection["w"] = this.ImageWidth.ToString();
            }
            if (this.ImageHeight != 50)
            {
                nameValueCollection["h"] = this.ImageHeight.ToString();
            }
            if (this.ImageFontName != "Arial")
            {
                nameValueCollection["fn"] = this.ImageFontName;
            }
            if (this.ImageForeColor != CaptchaImagePage.DefaultForeColor)
            {
                int argb = this.ImageForeColor.ToArgb();
                nameValueCollection["fc"] = argb.ToString("X8");
            }
            if (this.ImageForeNoiseColor != CaptchaImagePage.DefaultForeNoiseColor)
            {
                int num = this.ImageForeNoiseColor.ToArgb();
                nameValueCollection["fnc"] = num.ToString("X8");
            }
            if (this.ImageBackColor != CaptchaImagePage.DefaultBackColor)
            {
                int argb1 = this.ImageBackColor.ToArgb();
                nameValueCollection["bc"] = argb1.ToString("X8");
            }
            if (this.ImageBackNoiseColor != CaptchaImagePage.DefaultBackNoiseColor)
            {
                int num1 = this.ImageBackNoiseColor.ToArgb();
                nameValueCollection["bnc"] = num1.ToString("X8");
            }
            if (nameValueCollection.Count > 0)
            {
                stringBuilder.Append('?');
            }
            for (int i = 0; i < nameValueCollection.Count; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append('&');
                }
                stringBuilder.Append(string.Format("{0}={1}", nameValueCollection.Keys[i], nameValueCollection[i]));
            }
            this.imgCaptcha.ImageUrl = string.Concat(base.ResolveUrl(this.imgCaptcha.ImageUrl), stringBuilder.ToString());
            if (this.linkReload != null)
            {
                this.linkReload.Text = this.ReloadText;
                this.linkReload.ImageUrl = this.ReloadImageUrl;
                if (this.linkReload.ImageUrl != string.Empty)
                {
                    this.linkReload.ToolTip = this.linkReload.Text;
                }
            }
            if (this.lblMessage != null && this.Message == null)
            {
                switch (this.CaptchaType)
                {
                    case CaptchaType.Alphabetic:
                        {
                            this.lblMessage.Text = "Enter the letters you see in the image: ";
                            return;
                        }
                    case CaptchaType.Alphanumeric:
                        {
                            this.lblMessage.Text = "Enter the text you see in the image: ";
                            return;
                        }
                    case CaptchaType.Numeric:
                        {
                            this.lblMessage.Text = "Enter the numbers you see in the image: ";
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
        }

        protected override void CreateChildControls()
        {
            if (this._InnerTemplate == null)
            {
                HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
                this.imgCaptcha = new System.Web.UI.WebControls.Image()
                {
                    AlternateText = "CAPTCHA text",
                    CssClass = "captchaImage"
                };
                this.linkReload = new HyperLink()
                {
                    CssClass = "captchaReload"
                };
                this.lblMessage = new Label();
                this.txtCode = new TextBox()
                {
                    Columns = this.NumberOfCharacters
                };
                this.lblError = new Label();
                this.lblError.Style["display"] = "block";
                htmlGenericControl.Controls.Add(this.imgCaptcha);
                htmlGenericControl.Controls.Add(new LiteralControl(" "));
                htmlGenericControl.Controls.Add(this.linkReload);
                this.Controls.Add(htmlGenericControl);
                this.Controls.Add(this.lblError);
                this.Controls.Add(this.lblMessage);
                this.Controls.Add(this.txtCode);
            }
            else
            {
                this.InnerTemplate.InstantiateIn(this);
                this.imgCaptcha = this.FindControl("imgCaptcha") as System.Web.UI.WebControls.Image;
                this.linkReload = this.FindControl("linkReload") as HyperLink;
                this.lblMessage = this.FindControl("lblMessage") as Label;
                this.lblError = this.FindControl("lblError") as Label;
                this.txtCode = this.FindControl("txtCode") as TextBox;
                if (this.imgCaptcha == null || this.txtCode == null)
                {
                    throw new Exception("The InnerTemplate must contain both a System.Web.UI.WebControls.Image with an ID of \"imgCaptcha\" and a System.Web.UI.WebControls.TextBox with and ID of \"txtCode\".");
                }
            }
            this.imgCaptcha.ImageUrl = this.ImageSrc;
            this.txtCode.EnableViewState = false;
            if (this.linkReload != null)
            {
                this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "CaptchaReloadScript", "function thwc_CaptchaReload(imageID) {var image = document.getElementById(imageID); if(!image.srco) image.srco = image.src; image.src = image.srco + (image.srco.indexOf('?') == -1 ? '?' : '&') + (new Date()).getTime();}", true);
                this.linkReload.NavigateUrl = string.Format("javascript:thwc_CaptchaReload('{0}')", this.imgCaptcha.ClientID);
                this.linkReload.Attributes["onmouseover"] = "window.status = ''; return true;";
            }
            if (this.lblMessage != null)
            {
                this.lblMessage.Text = this.Message;
            }
            if (this.lblError != null)
            {
                this.lblError.Text = this.ErrorMessage;
                this.lblError.ForeColor = this.ErrorMessageColor;
                this.lblError.Visible = false;
                this.lblError.EnableViewState = false;
            }
        }

        protected override bool EvaluateIsValid()
        {
            bool flag;
            bool flag1;
            if (this.Context.Items["thwcCaptchaValidated"] != null)
            {
                return (bool)this.Context.Items["thwcCaptchaValidated"];
            }
            string code = CaptchaImagePage.Code;
            if (code == null || this.txtCode == null)
            {
                if (!this.Visible)
                {
                    flag1 = true;
                }
                else
                {
                    flag1 = (!this.Context.User.Identity.IsAuthenticated ? false : !this.TestAuthenticatedUsers);
                }
                flag = flag1;
            }
            else
            {
                flag = this.txtCode.Text.Equals(code, StringComparison.InvariantCultureIgnoreCase);
                if (flag)
                {
                    CaptchaImagePage.Code = null;
                }
            }
            if (!this.Visible || this.Context.User.Identity.IsAuthenticated && !this.TestAuthenticatedUsers)
            {
                flag = true;
            }
            else if (!flag && this.DisplayErrorMessage && this.lblError != null)
            {
                this.lblError.Visible = true;
                if (code == null)
                {
                    this.lblError.Text = this.ErrorMessageTimeout;
                }
            }
            this.Context.Items["thwcCaptchaValidated"] = flag;
            this.txtCode.Text = string.Empty;
            return flag;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }
    }
}