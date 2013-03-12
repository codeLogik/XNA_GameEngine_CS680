using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace XNA_GameEngine.Network
{
    class NetworkServer : NetworkThread
    {
        // Collection of remote end points that the server is concerned about broadcasting to
        public LinkedList<IPEndPoint> m_remoteIPEndpoints;
        public Dictionary<IPAddress, int> m_playerToIPMap;

        // Network input states
        private LinkedList<NetGOState> m_receivedGOStates;
        
        // Network game state to be broadcast to the clients and observers.
        private NetGameState m_simulatedGameState;

        // Mutex locks for shared data
        private static object s_lockNetSyncInput = new object();
        private static object s_lockRecGameState = new object();
        private static object s_lockSimGameState = new object();

        public NetworkServer()
            : base()
        {
            m_remoteIPEndpoints = new LinkedList<IPEndPoint>();
            m_receivedGOStates = new LinkedList<NetGOState>();
            m_playerToIPMap = new Dictionary<IPAddress, int>();

            IPAddress ip = IPAddress.Parse("192.168.1.85");
            IPEndPoint ep = new IPEndPoint(ip, 8888);
            m_remoteIPEndpoints.AddFirst(ep);
        }

        public void AddRemoteIPEndPoint(IPEndPoint remoteEndPoint, int playerID)
        {
            // Add the remote end point to the map and list.
            m_remoteIPEndpoints.AddLast(remoteEndPoint);
            m_playerToIPMap.Add(remoteEndPoint.Address, playerID);
        }

        public void ClearRemoteEndPoints()
        {
            m_remoteIPEndpoints.Clear();
        }

        public void GetReceivedGOStates(ref LinkedList<NetGOState> recNetGOStates)
        {
            lock (s_lockNetSyncInput)
            {
                recNetGOStates = m_receivedGOStates;
                m_receivedGOStates = null;
            }
        }

        public void QueueSimulatedGameState(NetGameState gameState)
        {
            lock (s_lockSimGameState)
            {
                m_simulatedGameState = gameState;
            }
        }

        public override void RunSenderThread()
        {
            // TODO @tom: This is the loop that runs so long as the server state is not disconnect.
            for (; ; )
            {
                // We need to collect network game states from the UDP socket and accumulate the states into
                // the appropriate buffers for the various systems within the engine to handle.  All of this is
                // done with locking and unlocking the buffers as these are within the critical section.

                // Get a copy of the NetGameState from NetworkManager
                NetGameState simGameState = null;
                lock (s_lockSimGameState)
                {
                    simGameState = m_simulatedGameState;
                }

                // Build packet and send.
                if (simGameState != null)
                {
                    MemoryStream memStream = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(memStream, simGameState);
                    }
                    catch (SerializationException e)
                    {
                        Debug.DebugTools.Report("[Network] (serialization): " + e.Message);
                    }

                    foreach (IPEndPoint remoteEndPoint in m_remoteIPEndpoints)
                    {
                         SendPacket(memStream.GetBuffer(), remoteEndPoint);
                        //Debug.DebugTools.Report("[Network] (packet): Successfully sent " + memStream.GetBuffer().Count() + " bytes");
                    }
                }
                
            }
        }

        public override void SendState(NetworkPacket networkPacket)
        {
            if (networkPacket is NetGameState)
            {
                NetGameState netGameState = (NetGameState)networkPacket;

                MemoryStream memStream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(memStream, netGameState);
                }
                catch (SerializationException e)
                {
                    Debug.DebugTools.Report("[Network] (serialization): " + e.Message);
                }

                foreach (IPEndPoint remoteEndPoint in m_remoteIPEndpoints)
                {
                    SendPacket(memStream.GetBuffer(), remoteEndPoint);
                    //Debug.DebugTools.Report("[Network] (packet): Successfully sent " + memStream.GetBuffer().Count() + " bytes");
                }
            }
        }

        public override void RunListenerThread()
        {
            // TODO @tom: This is the loop that runs so long as the server state is not disconnect.
            for (; ; )
            {
                // We need to collect network game states from the UDP socket and accumulate the states into
                // the appropriate buffers for the various systems within the engine to handle.  All of this is
                // done with locking and unlocking the buffers as these are within the critical section.

                // Get packet from the network interface.  This is a blocking action that will wait until a packet 
                // has been received
                IPEndPoint incomingEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = GetPacket(ref incomingEndPoint);

                // Extract the input packet we are waiting for.
                NetGOState incomingGOState = null;
                MemoryStream memStream = new MemoryStream(packet);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    Object obj = formatter.Deserialize(memStream);
                    incomingGOState = (NetGOState)obj;
                }
                catch (SerializationException e)
                {
                    Debug.DebugTools.Log("Networking", "deserialization", e.Message);
                }

                // If we successfully got the packet and deserialized correctly then queue it to be processed by the network manager.
                if (incomingGOState != null)
                {
                    m_receivedGOStates.AddLast(incomingGOState);
                }
            }
        }
    } // End class
} // End namespace
