using System;
using System.Net;
using System.IO;
using System.Text;
using TLpserver;

namespace tlRESTsvr
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListener server = new HttpListener();
            //server.Prefixes.Add("http://127.0.0.1/");
            //server.Prefixes.Add("http://localhost/");
            Console.WriteLine("Loading settings...");
            //Load and parse the settings file
            string appDir;  //Working directory of application
            appDir = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
#if (DEBUG)
            //Get to the dev folder root
            appDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
            Console.WriteLine(appDir);
#endif

#if (RELEASE)
            using (StreamReader sr = new StreamReader(appDir + "/settings.ini")) //To allow for easy disposal of object when we are done.
            {
                while (true)
                {
                    String line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    if (line.StartsWith("hostname="))
                    {
                        string hn = line.Replace("hostname=", null);
                        Console.WriteLine("Added hostname: " + hn);
                        server.Prefixes.Add(hn);
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Unknown setting: " + line);
                    }
                }
            }
#else
            server.Prefixes.Add("http://localhost:81/");
            server.Prefixes.Add("http://192.168.1.148:81/");
#endif


            server.Start();
            Console.WriteLine("Listening...");
 
            while (true)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerResponse response = context.Response;
                string page = Directory.GetCurrentDirectory() + context.Request.Url.LocalPath;
#if (DEBUG)
                Console.WriteLine("Recieved request: " + page);
#endif
                if (page == string.Empty)
                {
                    page = "index.html";
                    TextReader tr = new StreamReader(page);
                    string msg = tr.ReadToEnd();
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    response.ContentLength64 = buffer.Length;
                    Stream st = response.OutputStream;
                    st.Write(buffer, 0, buffer.Length);
                }
                else if (page.EndsWith("/lightstat"))
                {
                    page.Replace("/lightstat", "");
                    char[] ltr = page.ToCharArray(0, page.Length);
                    string id = "";
                    int index = ltr.Length - 1;
                    bool islightstat = true;
                    for (int i = index; i > 0; i--)
                    {
                        
                        if (ltr[i] == '/')
                        {
                            //end of id field, break
                            if (islightstat == true)
                            {
                                id = "";
                                islightstat = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            id = ltr[i] + id;
                        }
                    }
                    string resp = pipeManager.sendMsg("tl_rest|LIGHTSTAT|tl_core|" + id);
                    Stream st = response.OutputStream;
                    string res = resp.Replace("tl_rest|STATUS|tl_core|", null);
                    string msg = "<xml><trafficstatus><status id=\"" + id + "\">" + res + "</status></trafficstatus></xml>";
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    response.ContentLength64 = buffer.Length;
                    st.Write(buffer, 0, buffer.Length);
                }
                else if (page.EndsWith("/setlightstat"))
                {
                    page.Replace("/setlightstat", "");
                    char[] ltr = page.ToCharArray(0, page.Length);
                    string id = "";
                    int index = ltr.Length - 1;
                    int islightstat = 0; //0 for text, 1 for value, 2 for index
                    int ind = 0;
                    int val = 0;
                    for (int i = index; i > 0; i--)
                    {

                        if (ltr[i] == '/')
                        {
                            //end of id field, break
                            if (islightstat == 0)
                            {
                                id = "";
                                islightstat++;
                            }
                            else if(islightstat == 1)
                            {
                                val = Convert.ToInt32(id);
                                id = "";
                                islightstat++;
                            }
                            else
                            {
                                ind = Convert.ToInt32(id);
                                break;
                            }
                        }
                        else
                        {
                            id = ltr[i] + id;
                        }
                    }
#if DEBUG
                    Console.WriteLine("Writing value of " + val.ToString() + " to index " + ind.ToString());
#endif
                    string resp = pipeManager.sendMsg("tl_rest|LIGHTSTAT|tl_core|" + ind.ToString() + "|" + val.ToString());
                    Stream st = response.OutputStream;
                    string res = resp.Replace("tl_db|STATUS|tl_rest|", null);
                    string msg = "<xml><trafficstatus><status id=\"" + ind.ToString() + "\">" + res + "</status></trafficstatus></xml>";
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    response.ContentLength64 = buffer.Length;
                    st.Write(buffer, 0, buffer.Length);
                }
                context.Response.Close();
            }
        }
    }
}
