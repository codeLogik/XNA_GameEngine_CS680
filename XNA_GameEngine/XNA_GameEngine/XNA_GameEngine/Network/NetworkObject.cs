using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNA_GameEngine.Core;
using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Network;
using XNA_GameEngine.Network.NetworkDataTypes;

namespace XNA_GameEngine.Network
{
    class NetworkObject : ICoreComponent
    {
        private NetGOState m_netState;

        public NetworkObject(GameObject parentGO)
            : base(parentGO)
        {
            m_netState = new NetGOState(0);
            m_Type = ComponentType.COMPONENT_Networking;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // TODO @tom:  Handle logic here for updating the net state of the game object.  Do not
            // update position if we are the server as we have authority.
            m_netState.m_goRef = m_ownerGO.GetRef();
            m_netState.m_position = new NVec2(m_ownerGO.GetPosition());
            m_netState.m_rotation = m_ownerGO.GetRotation();
            m_netState.m_currentFrameNumber = GameplayWorld.GetInstance().GetCurrentFrameNumber();
        }

        public void UpdateFromNetwork(NetGOState netGOstate)
        {
            // Update the game object state from the network GO state.
            m_ownerGO.SetPosition(netGOstate.m_position.GetVector2());
            m_ownerGO.SetRotation(netGOstate.m_rotation);
        }

        public NetGOState GetCurrentNetState()
        {
            // Update the network object and return it.
            Update(null);
            return m_netState;
        }
    }
}
