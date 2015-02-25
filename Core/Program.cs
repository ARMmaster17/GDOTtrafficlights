using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");
            //Insert startup args + variables
            string appDir;  //Working directory of application
            string appLang = "en-us"; //Language used by application, English by default
            string[] appLangStrings =  new string[20]; //Collection of strings for specified language
            Console.WriteLine("Loading settings...");
            //Load and parse the settings file
            appDir = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
#if (DEBUG)
            //Get to the dev folder root
            appDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
            Console.WriteLine(appDir);      
#endif
            using (StreamReader sr = new StreamReader(appDir + "/settings.ini")) //To allow for easy disposal of object when we are done.
            {
                while (true)
                {
                    String line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    if (line.StartsWith("lang="))
                    {
                        appLang = line.Replace("lang=", null);
                        Console.WriteLine("Language detected: " + appLang);
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Unknown setting: " + line);
                    }
                }
            }
            //Initialize the language file
            using (StreamReader sr = new StreamReader(appDir + "/lang/" + appLang + "/strings.ini")) //To allow for easy disposal of object when we are done.
            {
                for (int i = 0; i < 2; i++)
                {
                    string line = sr.ReadLine();
                    appLangStrings[i] = line.Replace("{" + i + "}=", "");
                }
            }
            Console.WriteLine(appLangStrings[0]);
            //Open the pipe server
            Console.WriteLine(appLangStrings[1]);
            //Insert function to run executables of other modules
            Console.ReadLine();
        }
    }
}
