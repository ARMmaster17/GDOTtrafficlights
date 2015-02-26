using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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
            string[] appModPaths = new string[20]; //Collection of executable paths for modules

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
                    else if (line.StartsWith("hostname="))
                    {
                        // Do nothing, this is a valid setting, but it is not used by the core
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
                for (int i = 0; i < appLangStrings.Length; i++)
                {
                    string line = sr.ReadLine();
                    try
                    {
                        appLangStrings[i] = line.Replace("{" + i + "}=", "");
                    }
                    catch
                    {
                        //we have hit the end of the file
                        break;
                    }
                }
            }
            Console.WriteLine(appLangStrings[0]); //Notify user we are starting the pipe server
            //Open the pipe server
            //NamedPipeServerStream pipeServer = createServer(); //this is done automatically in the pipe-listen loop
            Console.WriteLine(appLangStrings[1]); //Notify the user we are starting the modules

            //Insert function to run executables of other modules
            //Get list of modules to start
            using (StreamReader sr = new StreamReader(appDir + "/modules.ini"))
            {
                for (int i = 0; i < appModPaths.Length; i++)
                {
                    appModPaths[i] = sr.ReadLine();
                }
            }
            //Execute each module
            foreach (string i in appModPaths)
            {
                if(i == null)
                {
                    //break, we have hit the end of the list of modules
                    break;
                }
                else
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var startInfo = new ProcessStartInfo(appDir + i) { Verb = "runas" };
                            Process.Start(startInfo);
                            //Process.Start(appDir + i);
                            Console.WriteLine("CORE: Module " + i + " has been started");
                        }
                        catch (FileNotFoundException e)
                        {
                            Console.WriteLine("CORE: An error has occured launching module " + i);
                            Console.WriteLine("Details: " + e.ToString());
                        }
                        catch
                        {
                            Console.WriteLine("CORE: An unknown error has occured launching module " + i);
                        }
                    });
                }
            }

            /////////////////////////////
            // END OF LOADING SEQUENCE //
            /////////////////////////////

            //Start listening for commands
            Console.WriteLine("Ready...");
            while(true)
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("GDOTTL"))
                {
                    try
                    {
                        pipeServer.WaitForConnection();
                        StreamReader reader = new StreamReader(pipeServer);
                        StreamWriter writer = new StreamWriter(pipeServer);
                        string cmdraw = reader.ReadLine();
#if DEBUG
                        Console.WriteLine("CORE: Recieved message " + cmdraw);
#endif
                        string ln = interpretCommand(cmdraw);
                        if (ln.Contains("|EXIT|tl_core"))
                        {
                            string[] msg = ln.Split('|');
                            Console.WriteLine("CORE: " + msg[0] + "has ordered a shutdown because " + msg[3]);
                            writer.WriteLine("tl_core|SHUTDOWN|tl_all|f");
                            writer.Flush();
                            break;
                        }
                        writer.WriteLine(ln);
                        writer.Flush();
                    }
                    catch
                    {
                        //do nothing, there was a pipe error that will most likely be fixed next loop
                    }
                }
            }
            //terminate function
            Console.Write("Press any key to quit: ");
            Console.ReadKey();
        }
        private static string interpretCommand(string cmd)
        {
            string[] cmda = cmd.Split('|');
            switch (cmda[2])
            {
                case "tl_core":
                    if (cmda[1] == "EXIT")
                    {
                        return cmda[0] + "|EXIT|tl_core|" + cmda[3];
                    }
                    else if (cmda[1] == "ECHO")
                    {
                        Console.WriteLine(cmda[3]);
                        return "tl_core|STATUS|" + cmda[0] + "|0";
                    }
                    else if (cmda[1] == "LIGHTSTAT")
                    {
                        // Right now for debuging purposes, return green
                        return "tl_rest|STATUS|tl_core|0";
                    }
                    else
                    {
                        return "tl_core|ERROR|" + cmda[0] + "|Unrecognized command";
                    }
                default:
                    return "tl_core|ERROR|" + cmda[0] + "|Unrecognized command";
            }
        }
    }
}
