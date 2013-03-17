using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Network
{
    class NetworkClient : NetworkThread
    {
        // The game state received from the server.
        private NetGameState m_receivedGameState;

        static object s_recGameStateLock = new object();

        private IPEndPoint serverEndPoint;

        public NetworkClient()
            : base(NetworkParams.CLIENT_LISTENER_PORT)
        {
            serverEndPoint = Network.NetworkParams.serverEndpoint;
        }

        public void GetNetGameState(ref NetGameState netGameState)
        {
            lock (s_recGameStateLock)
            {
                netGameState = m_receivedGameState;
                m_receivedGameState = null;
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
                //Debug.DebugTools.Report("[Networking] (client): Received packet from server of size: " + packet.Length); 

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

        // Deprecated.  Sending is no longer done asynchronously
        public override void RunSenderThread()
        {
            // TODO @tom: This needs to be implemented for sending the state up to the server.
            GameObject go = GameplayWorld.GetInstance().GetPlayerGameObjectOrNULL(CoreMain.s_localPlayer);

            //if(go != null)
            //{
            //    NetGOState netGOstate = go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Networking);
        }

        public override void SendState(NetworkPacket networkPacket)
        {
            if (networkPacket is NetGOState)
            {
                NetGOState netGOstate = (NetGOState)networkPacket;
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(memStream, netGOstate);
                }
                catch (SerializationException e)
                {
                    Debug.DebugTools.Report("[Network] (serialization): " + e.Message);
                }

                SendPacket(memStream.GetBuffer(), serverEndPoint);
                //Debug.DebugTools.Report("[Network] (packet): Successfully sent " + memStream.GetBuffer().Count() + " bytes");
            }
        }
    }
}
