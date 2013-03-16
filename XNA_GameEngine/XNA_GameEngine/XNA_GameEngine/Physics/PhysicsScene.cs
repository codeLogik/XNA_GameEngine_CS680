using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Physics
{
    class PhysicsScene
    {
        private BoundingBox2D m_boundaries;

        public PhysicsScene(BoundingBox2D boundaires)
        {
            m_boundaries = boundaires;
        }

        public BoundingBox2D GetBoundaries()
        {
            return m_boundaries;
        }
    }
}
