using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XNA_GameEngine.Network
{
    abstract class NetworkThread
    {
        protected Thread m_uplinkThread;
        protected Thread m_downlinkThread;

        public const int UPLINK_PORT = 8888;
        public const int DOWNLINK_PORT = 8880;

        public NetworkThread()
        {
        }

        public abstract void InitializeThread();
        
        public void ShutDown()
        {
            // Shut down the threads.
            try
            {
                m_uplinkThread.Abort();
            }
            catch(ThreadAbortException)
            {
                // Intentionally blank as we wanted to shut this thread down.
            }

            try
            {
                m_downlinkThread.Abort();
            }
            catch(ThreadAbortException)
            {
                // Intentionally blank as we wanted to shut this thread down.
            }
        }

        public abstract void RunUplinkThread();
        public abstract void RunDownlinkThread();
    }
}
