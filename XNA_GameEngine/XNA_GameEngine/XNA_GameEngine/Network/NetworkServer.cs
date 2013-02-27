using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace XNA_GameEngine.Network
{
    class NetworkServer : NetworkThread
    {
        // Collection of remote end points that the server is concerned about broadcasting to
        public LinkedList<IPEndPoint> m_remoteIPEndpoints;

        // Network input states
        private LinkedList<NetInputState> m_receivedInputStates;
        
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
            m_receivedInputStates = new LinkedList<NetInputState>();
        }

        public override void InitializeThread()
        {
            // Start the uplink and downlink threads.
            m_uplinkThread = new Thread(new ThreadStart(RunUplinkThread));
            m_downlinkThread = new Thread(new ThreadStart(RunDownlinkThread));

            // Start the threads.
            m_uplinkThread.Start();
            m_downlinkThread.Start();
        }

        public void AddRemoteIPEndPoint(IPEndPoint remoteEndPoint)
        {
            m_remoteIPEndpoints.AddLast(remoteEndPoint);
        }

        public void ClearRemoteEndPoints()
        {
            m_remoteIPEndpoints.Clear();
        }

        public void GetReceivedInputStates(ref LinkedList<NetInputState> recNetInputStates)
        {
            lock (s_lockNetSyncInput)
            {
                recNetInputStates = m_receivedInputStates;
                m_receivedInputStates = null;
            }
        }

        public void QueueSimulatedGameState(NetGameState gameState)
        {
            lock (s_lockSimGameState)
            {
                m_simulatedGameState = gameState;
            }
        }

        public override void RunUplinkThread()
        {
            // TODO @tom: This is the loop that runs so long as the server state is not disconnect.
            for (; ; )
            {
                // We need to collect network game states from the UDP socket and accumulate the states into
                // the appropriate buffers for the various systems within the engine to handle.  All of this is
                // done with locking and unlocking the buffers as these are within the critical section.

                // Get a copy of the NetGameState from NetworkManager
                
            }
        }

        public override void RunDownlinkThread()
        {
            // TODO @tom: This is the loop that runs so long as the server state is not disconnect.
            for (; ; )
            {
                // We need to collect network game states from the UDP socket and accumulate the states into
                // the appropriate buffers for the various systems within the engine to handle.  All of this is
                // done with locking and unlocking the buffers as these are within the critical section.
            }
        }

        public void SendGameState()
        {

        }

        
    }
}
