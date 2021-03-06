﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using DBengine;

namespace Core
{
    class Program
    {
        /// <summary>
        /// Main entry point for the program
        /// </summary>
        /// <param name="args">Arguments passed to the program through the command line</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");
            //Insert startup args + variables
            //Librarian librarian = new Librarian(); //DBengine database manager
            int[] trafficstats = {0, 0}; //Temporary array until DBengine is operational
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
                        // We detected a setting line that defines the locale code
                        appLang = line.Replace("lang=", null); // Assign it as the language in use
                        Console.WriteLine("Language detected: " + appLang);
                    }
                    else if (line.StartsWith("hostname="))
                    {
                        // Do nothing, this is a valid setting, but it is not used by the core
                    }
                    else
                    {
                        // An unknown setting was entered, just skip it
                        Console.WriteLine("WARNING: Unknown setting: " + line);
                    }
                }
            }

            //Initialize the language file
            using (StreamReader sr = new StreamReader(appDir + "/lang/" + appLang + "/strings.ini")) //To allow for easy disposal of object when we are done.
            {
                for (int i = 0; i < appLangStrings.Length; i++)
                {
                    string line = sr.ReadLine(); // Read a line from the language file
                    try
                    {
                        appLangStrings[i] = line.Replace("{" + i + "}=", ""); // Strip the identifier key out of the string and add it to the language string list
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
            using (StreamReader sr = new StreamReader(appDir + "/modules.ini")) // Bind StreamReader to filestream to modules file
            {
                for (int i = 0; i < appModPaths.Length; i++)
                {
                    appModPaths[i] = sr.ReadLine(); // Add the module to the module list
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
                    // Launch each module on a new thread to prevent basekicks and deadlocks
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var startInfo = new ProcessStartInfo(appDir + i) { Verb = "runas" }; // Run as administrator to bypass firewall
                            Process.Start(startInfo);
                            //Process.Start(appDir + i);
                            Console.WriteLine(appLangStrings[2], i); // Notify user of launching of module
                        }
                        catch (FileNotFoundException e)
                        {
                            // We couldn't find the file, catch the error and notify the user
                            Console.WriteLine(appLangStrings[3], i);
                            Console.WriteLine(appLangStrings[4], e.ToString());
                        }
                        catch
                        {
                            // A weird error occured, notify the user and continue
                            Console.WriteLine(appLangStrings[5], i);
                        }
                    });
                }
            }

            /////////////////////////////
            // END OF LOADING SEQUENCE //
            /////////////////////////////

            //Start listening for commands
            Console.WriteLine("Ready...");
            while(true) // Repeat until program termination
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("GDOTTL")) // Create pipe server
                {
                    try
                    {
                        // Wait for module to connect
                        pipeServer.WaitForConnection();
                        // Create IO connections to pipe
                        StreamReader reader = new StreamReader(pipeServer);
                        StreamWriter writer = new StreamWriter(pipeServer);
                        // Get the incoming message
                        string cmdraw = reader.ReadLine();
#if DEBUG
                        Console.WriteLine(appLangStrings[6], cmdraw); // Post the raw command, if debugging
#endif
                        //string ln = interpretCommand(cmdraw, librarian);
                        // Push the command to the interpreter engine
                        string ln = interpretCommand(cmdraw, ref trafficstats); //Using ref to avoid UNSAFE method attribute with pointers
                        // Check for exit command
                        if (ln.Contains("|EXIT|tl_core"))
                        {
                            string[] msg = ln.Split('|');
                            Console.WriteLine(appLangStrings[7], msg[0], msg[3]);
                            writer.WriteLine("tl_core|SHUTDOWN|tl_all|f");
                            writer.Flush();
                            break; // Kill the application loop
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
            Console.Write(appLangStrings[8]);
            Console.ReadKey();
        }
        /// <summary>
        /// Interprets a given command, then performes the neccessary action
        /// </summary>
        /// <param name="cmd">Raw command from pipe</param>
        /// <param name="lightstatus">Pointer to the lightstatus[] array</param>
        /// <returns>Command to pass back to sender</returns>
        private /*unsafe*/ static string interpretCommand(string cmd, ref int[] lightstatus) //Use for debugging purposes until DBengine is functional
        {
            string[] cmda = cmd.Split('|');
            switch (cmda[2])
            {
                case "tl_core":
                    // SENDER|EXIT|tl_core
                    if (cmda[1] == "EXIT")
                    {
                        return cmda[0] + "|EXIT|tl_core|" + cmda[3];
                    }
                    // SENDER|ECHO|tl_core|Message
                    else if (cmda[1] == "ECHO")
                    {
                        Console.WriteLine(cmda[3]);
                        return "tl_core|STATUS|" + cmda[0] + "|0";
                    }
                    // SENDER|LIGHTSTAT|tl_core|INDEX
                    else if (cmda[1] == "LIGHTSTAT") //Depreciated, copied code from tl_db|GET
                    {
                        // Right now for debuging purposes, return green
                        //return "tl_rest|STATUS|tl_core|0";
                        //Depreciated, uses code below copied from tl_db|GET
                        return "tl_core|STATUS|" + cmda[0] + "|" + lightstatus[Convert.ToInt32(cmda[3])].ToString();
                    }
                    else
                    {
                        return "tl_core|ERROR|" + cmda[0] + "|Unrecognized command";
                    }
                case "tl_db":
                    // SENDER|GET|tl_db|INDEX
                    if (cmda[1] == "GET")
                    {
                        return "tl_db|STATUS|" + cmda[0] + "|" + lightstatus[Convert.ToInt32(cmda[3])];
                    }
                    // SENDER|SET|tl_db|INDEX|VALUE
                    else if (cmda[1] == "SET")
                    {
                        lightstatus[Convert.ToInt32(cmda[3])] = Convert.ToInt32(cmda[4]);
                        return "tl_db|STATUS|" + cmda[0] + "|" + cmda[4];
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
