﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace XNA_GameEngine.Network
{
    abstract class NetworkThread
    {
        protected Thread m_SenderThread;
        protected Thread m_ListenerThread;

        public const int __LISTENER_PORT = 8888;
        public const int __SENDER_PORT = 8880;

        private UdpClient __senderSocket;
        private UdpClient __listenerSocket;

        public NetworkThread(int listenerPort)
        {
            __senderSocket = new UdpClient();
            __listenerSocket = new UdpClient(listenerPort);
            
        }

        public virtual void InitializeThread()
        {
            // Start the listener thread.
            m_ListenerThread = new Thread(new ThreadStart(RunListenerThread));

            // Start the threads.
            try
            {
                m_ListenerThread.Start();   
                Debug.DebugTools.Log("Network", "Threading", "Listener threads started successfully");
            }
            catch (ThreadStateException e)
            {
                Debug.DebugTools.Log("Network", "Threading", e.Message);
            }
        }
        
        public void ShutDown()
        {
            // Shut down the threads.
            try
            {
                m_ListenerThread.Abort();
            }
            catch(ThreadAbortException)
            {
                // Intentionally blank as we wanted to shut this thread down.
            }
        }

        public abstract void RunListenerThread();
        public abstract void RunSenderThread();

        public abstract void SendState(NetworkPacket networkPacket);

        public void SendPacket(byte[] packet, IPEndPoint remoteEndpoint)
        {
            try
            {
                __senderSocket.Send(packet, packet.Length, remoteEndpoint);
            }
            catch (SocketException e)
            {
                Debug.DebugTools.Report("[Network] (send): " + e.ErrorCode + ": " + e.Message);
            }
        }

        public byte[] GetPacket(ref IPEndPoint receiverEndPoint)
        {
            try
            {
                return __listenerSocket.Receive(ref receiverEndPoint);
            }
            catch (SocketException e)
            {
                Debug.DebugTools.Report("[Network] (receive): " + e.ErrorCode + ": " + e.Message);
                return null;
            }
        }

    }
}
