using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNA_GameEngine.Core;
using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Network;

namespace XNA_GameEngine.Network
{
    class NetworkObject : ICoreComponent
    {
        private NetGOState m_netState;

        public NetworkObject(GameObject parentGO)
            : base(parentGO)
        {
            m_Type = ComponentType.COMPONENT_Networking;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // TODO @tom:  Handle logic here for updating the net state of the game object.  Do not
            // update position if we are the server as we have authority.
        }
    }
}
