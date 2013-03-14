using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Core;
using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Network;
using XNA_GameEngine.Network.NetworkDataTypes;

namespace XNA_GameEngine.Network
{
    class NetworkObject : ICoreComponent
    {
        private NetGOState m_netState;
        private NetGOState m_previousNetState;

        public NetworkObject(GameObject parentGO)
            : base(parentGO)
        {
            m_netState = new NetGOState(0);
            m_previousNetState = null;
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
            
            Physics.PhysicsObject physObj = (Physics.PhysicsObject)m_ownerGO.GetComponentByTypeOrNULL(ComponentType.COMPONENT_Physics);

            if (/*physObj != null && m_previousNetState != null*/false)
            {
                // Set the new velocity of the object based on the direction of the previous known position to the next. Make sure to just get
                // this normalized direction and multiply it by the current speed.  Physics will handle adjusting the velocity appropriately.
                // This should ensure that the movement changes are smooth.
                Vector2 previousPos = m_previousNetState.m_position.GetVector2();
                Vector2 newPos = netGOstate.m_position.GetVector2();
                Vector2 direction = newPos - previousPos;
                direction.Normalize();

                float fSpeed = physObj.GetSpeed();
                physObj.SetVelocity(direction * fSpeed);
            }
            else
            {
                // We don't have a physics object, so we just update the position.
                m_ownerGO.SetPosition(netGOstate.m_position.GetVector2());
            }
            
            // TODO @tom:  Add support for doing the same kind of interpolation as above for rotation and angular velocity.
            m_ownerGO.SetRotation(netGOstate.m_rotation);

            // Cache the previous network state
            m_previousNetState = netGOstate;
        }

        public NetGOState GetCurrentNetState()
        {
            // Update the network object and return it.
            Update(null);
            return m_netState;
        }
    }
}
