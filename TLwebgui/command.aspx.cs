using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Pipes;
using System.Web.UI;
using System.Web.UI.WebControls;
using TLpserver;

namespace TLwebgui
{
    public partial class command : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                if (this.Request["cmd"] != null)
                {
                    Response.Write(pipeManager.sendMsg(this.Request["cmd"]));
                    Response.Write("<br />");
                }
                else
                {
                    //This is the first time this page is being loaded, just show the textbox and button
                }
            }
            else
            {
                //User is not logged in, redirect to login page
                Response.Redirect("Default.aspx");
            }
        }
    }
}