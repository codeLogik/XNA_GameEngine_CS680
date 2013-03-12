using System;
using System.Net;

namespace XNA_GameEngine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            

            using (CoreMain game = new CoreMain())
            {
                // Gather command line arguements for whether we are the server or client instance.
                if (args.Length > 0)
                {
                    if (args[0].Equals("server"))
                    {
                        CoreMain.isServer = true;
                        CoreMain.s_localPlayer = 0;
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a server");
                    }
                    else if (args[0].Equals("client"))
                    {
                        CoreMain.isServer = false;
                        CoreMain.s_localPlayer = 1;
                        IPAddress ip = IPAddress.Parse(args[1]);
                        IPEndPoint ep = new IPEndPoint(ip, 8888);
                        CoreMain.serverEndpoint = ep;
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a client");
                    }
                    else if (args[0].Equals("observer"))
                    {
                        CoreMain.isServer = false;
                        CoreMain.s_localPlayer = -1;
                        IPAddress ip = IPAddress.Parse(args[1]);
                        IPEndPoint ep = new IPEndPoint(ip, 8888);
                        CoreMain.serverEndpoint = ep;
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a observer");
                    }
                }
                else
                {
                    CoreMain.isServer = true;
                    CoreMain.s_localPlayer = 0;
                    Debug.DebugTools.Report("[Program] (initialization): Starting as a server");
                }
                game.Run();
            }
        }
    }
#endif
}

