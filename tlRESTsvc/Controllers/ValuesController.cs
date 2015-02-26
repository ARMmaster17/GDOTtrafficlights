using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.IO.Pipes;

namespace tlRESTsvc.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(string cmd, string sender)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
            {
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                writer.WriteLine("tl_rest|" + cmd + "|tl_core|" + sender);
                writer.Flush();
                string result = reader.ReadLine();
                client.Close();
                return result;
            }
        }

        // POST api/values
        public string Post([FromBody]string cmd, string sender)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
            {
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                writer.WriteLine("tl_rest|" + cmd + "|tl_core|" + sender);
                writer.Flush();
                string result = reader.ReadLine();
                client.Close();
                return result;
            }
        }

        // PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}