using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Pipes;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                    using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
                    {
                        client.Connect();
                        StreamReader reader = new StreamReader(client);
                        StreamWriter writer = new StreamWriter(client);
                        writer.WriteLine(this.Request["cmd"]);
                        writer.Flush();
                        Response.Write(reader.ReadLine());
                        client.Close();
                    }
                    Response.Write("<br />");
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