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
                if (args.Length == 2)
                {
                    System.Console.WriteLine("Number of args: " + args.Length);
                    if (args[0].Equals("server"))
                    {
                        Network.NetworkParams.isServer = true;
                        Network.NetworkParams.isObserver = false;
                        CoreMain.s_localPlayer = 0;
                        IPAddress clientIP = IPAddress.Parse(args[1]);
                        Network.NetworkParams.clientEndpoint = new IPEndPoint(clientIP, Network.NetworkParams.CLIENT_LISTENER_PORT);
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a server");
                    }
                    else if (args[0].Equals("client"))
                    {
                        Network.NetworkParams.isServer = false;
                        Network.NetworkParams.isObserver = false;
                        CoreMain.s_localPlayer = 1;
                        IPAddress serverIP = IPAddress.Parse(args[1]);
                        Network.NetworkParams.serverEndpoint = new IPEndPoint(serverIP, Network.NetworkParams.SERVER_LISTENER_PORT);
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a client");
                    }
                    else if (args[0].Equals("observer"))
                    {
                        Network.NetworkParams.isServer = false;
                        CoreMain.s_localPlayer = -1;
                        Network.NetworkParams.isObserver = true;
                        IPAddress serverIP = IPAddress.Parse(args[1]);
                        Network.NetworkParams.serverEndpoint = new IPEndPoint(serverIP, Network.NetworkParams.SERVER_LISTENER_PORT);
                        Debug.DebugTools.Report("[Program] (initialization): Starting as a observer");
                    }
                }
                else
                {
                    Network.NetworkParams.isServer = true;
                    CoreMain.s_localPlayer = 0;
                    Debug.DebugTools.Report("[Program] (initialization): Starting as a server");
                }
                game.Run();
            }
        }
    }
#endif
}

