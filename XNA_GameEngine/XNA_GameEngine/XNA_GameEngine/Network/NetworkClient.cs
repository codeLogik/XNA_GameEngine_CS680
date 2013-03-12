using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace XNA_GameEngine.Network
{
    class NetworkClient : NetworkThread
    {
        // The game state received from the server.
        private NetGameState m_receivedGameState;

        static object s_recGameStateLock = new object();

        private IPEndPoint serverEndPoint;

        public NetworkClient()
            : base()
        {
            serverEndPoint = null;
        }

        public NetGameState GetNetGameState()
        {
            lock (s_recGameStateLock)
            {
                return m_receivedGameState;
            }
        } 

        public override void RunListenerThread()
        {
            // Run forever and gather up nework game states from the server.
            for (; ; )
            {
                // Get packet from the server containing the network game state and deserialize it.
                // This is a blocking call and will not unblock until packets have been received.
                IPEndPoint serverEndPoint = null;
                byte[] packet = GetPacket(ref serverEndPoint);

                NetGameState incomingGameState = null;
                MemoryStream memStream = new MemoryStream(packet);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    Object obj = formatter.Deserialize(memStream);
                    incomingGameState = (NetGameState)obj;
                }
                catch (SerializationException e)
                {
                    Debug.DebugTools.Log("Networking", "deserialize", e.Message);
                }

                if (incomingGameState != null)
                {
                    // Store the network game state for the network manager to process.
                    lock (s_recGameStateLock)
                    {
                        m_receivedGameState = incomingGameState;
                    }
                }
            }
        }

        public override void RunSenderThread()
        {
            // TODO @tom: This needs to be implemented for sending the state up to the server.
        }
    }
}
