using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Colliders
{
    abstract class ICollider
    {
        private GameObject m_parent;
        public ICollider(GameObject gameObject)
        {
            m_parent = gameObject;
        }

        public GameObject GetParent()
        {
            return m_parent;
        }

        public bool CollidesWith(ICollider other)
        {
            if (other is SquareCollider)
            {
                return CollidesWith((SquareCollider)other);
            }
            else if (other is CircleCollider)
            {
                return CollidesWith((CircleCollider)other);
            }
            return false;
        }

        public abstract bool CollidesWith(SquareCollider other);
        public abstract bool CollidesWith(CircleCollider other);
    }
}
