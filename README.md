# Introduction
This is a Captcha web control for ASP.NET by Chee Wee, Chua.
Developed primarily for Telligent Community Server.

# Build
Open the solution in Visual Studio and build it.

# Setup

Copy the chuacw.TelligentCommunity.dll to the bin folder.
Create a file named "captchaimage.aspx" with the following content and placed it in the Web folder:

```
<%@ Page Inherits="chuacw.TelligentCommunity.CaptchaImagePage" %>
```

# Adding required imports and registration
Add the following code to register and import any namespaces and assemblies:

```
<%@ Import Namespace="chuacw.TelligentCommunity" %>
<%@ Import Namespace="chuacw.WebControls" %>
<%@ Register TagPrefix="cw" Namespace="chuacw.TelligentCommunity" Assembly="chuacw.TelligentCommunity" %>
<%@ Register TagPrefix="cw" Namespace="chuacw.WebControls" Assembly="chuacw.TelligentCommunity" %>
```

# Usage
## Telligent Community / Community Server
Add the following code into any necessary file, ensuring you update the id and ValidationGroup attribute to the proper names:

```
<CSControl:ConditionalContent id="CaptchaContainer" runat="server">
    <ContentConditions><CSControl:UserPropertyValueComparison runat="server" UseAccessingUser="true" ComparisonProperty="IsAnonymous" Operator="IsSetOrTrue" /></ContentConditions>
    <TrueContentTemplate>
		<dt>Enter the following code to ensure that your comment reaches the intended party:</dt>
		<dd><cw:Captcha runat="server" id="captcha1" ValidationGroup="CreateCommentForm" /></dd>
    </TrueContentTemplate>
    <FalseContentTemplate>
    </FalseContentTemplate>
</CSControl:ConditionalContent>
```

## ASP.NET
Add the following code into any necessary file, ensuring you update the id and ValidationGroup attribute to the proper names:

```
<cw:Captcha runat="server" id="captcha1" ValidationGroup="CreateCommentForm" />
```

# Author
Chee Wee, Chua,  
Singapore,   
Apr 2025.    

