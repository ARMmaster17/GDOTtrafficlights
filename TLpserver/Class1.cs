using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace TLpserver
{
    public class pipeManager
    {
        public static string sendMsg(string cmd)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
            {
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                writer.WriteLine(cmd);
                writer.Flush();
                string result = reader.ReadLine();
                client.Close();
                return result;
            }
        }
    }
}
