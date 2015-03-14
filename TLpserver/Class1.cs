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
        public static string sendMsg(string cmd) // Static to lower resource use
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
            {
                // Connect to server
                client.Connect();
                // Create IO interfaces
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                // Write the specified message to the stream
                writer.WriteLine(cmd);
                // Send the message
                writer.Flush();
                // Get the response
                string result = reader.ReadLine();
                // Notify the server we are done
                client.Close();
                // Return the response
                return result;
            }
        }
    }
}
