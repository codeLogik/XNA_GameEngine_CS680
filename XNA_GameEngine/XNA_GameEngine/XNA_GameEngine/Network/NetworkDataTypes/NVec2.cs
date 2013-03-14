using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Network.NetworkDataTypes
{
    [Serializable]
    class NVec2
    {
        public float x;
        public float y;

        #region Constructors
        public NVec2()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }

        public NVec2(float xy)
        {
            this.x = xy;
            this.y = xy;
        }

        public NVec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public NVec2(Vector2 vec2)
        {
            this.x = vec2.X;
            this.y = vec2.Y;
        }

        public NVec2(NVec2 nvec2)
        {
            this.x = nvec2.x;
            this.y = nvec2.y;
        }
        #endregion

        #region Operator_Overloads
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((NVec2)obj);
        }

        public bool Equals(NVec2 nvec2)
        {
            if (nvec2 == null)
            {
                return false;
            }

            if(ReferenceEquals(this, nvec2))
            {
                return true;
            }

            return ((this.x == nvec2.x) && (this.y == nvec2.y));
        }

        public static bool operator ==(NVec2 lhs, NVec2 rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(NVec2 lhs, NVec2 rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return !ReferenceEquals(rhs, null);
            }

            if (ReferenceEquals(rhs, null))
            {
                return !ReferenceEquals(lhs, null);
            }

            return !lhs.Equals(rhs);
        }
        #endregion

        public Vector2 GetVector2()
        {
            return new Vector2(this.x, this.y);
        }
    }
}
