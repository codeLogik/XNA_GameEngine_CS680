using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Physics
{
    class QuadNode
    {
        private LinkedList<PhysicsObject> m_physicsObjects;
        private LinkedList<PhysicsObject> m_needFixing;
        private QuadNode m_parent;
        private QuadNode[] m_childNodes;
        private int m_iDepth;
        private bool m_bHasSubdivided;
        BoundingBox2D m_boundaryBox;

        public QuadNode(QuadNode parent, BoundingBox2D boundaries, int depth)
        {
            m_parent = parent;
            m_childNodes = new QuadNode[4];
            m_physicsObjects = new LinkedList<PhysicsObject>();
            m_iDepth = depth;
            m_boundaryBox = boundaries;
            m_bHasSubdivided = false;
        }

        public void AddPhysicsObject(PhysicsObject physicsObject)
        {
            if (m_physicsObjects.Count > (4 * m_iDepth) && !m_bHasSubdivided)
            {
                m_physicsObjects.AddLast(physicsObject);
                this.SubDivide();
            }
            else if (m_bHasSubdivided)
            {
                bool added = false;
                for (int i = 0; i < 4; i++)
                {
                    if (m_childNodes[i].EntirelyContains(physicsObject))
                    {
                        added = true;
                        m_childNodes[i].AddPhysicsObject(physicsObject);
                        break;
                    }
                }

                if (!added)
                {
                    m_physicsObjects.AddLast(physicsObject);
                }
            }
            else
            {
                m_physicsObjects.AddLast(physicsObject);
            }
        }

        public int Count()
        {
            return m_physicsObjects.Count;
        }

        private void BubbleUpObject(PhysicsObject physObj) {
            BoundingBox2D objectBox = physObj.GetCollider().GetBoundingBox();
            if (m_boundaryBox.EntirelyContains(objectBox))
            {
                AddPhysicsObject(physObj);
            }
            else
            {
                if (m_parent == null)
                {
                    m_physicsObjects.AddLast(physObj);
                }
                else
                {
                    m_parent.BubbleUpObject(physObj);
                }
            }
        }
        
        public void UpdateObjects(GameTime time)
        {
            m_needFixing = new LinkedList<PhysicsObject>();
            LinkedList<PhysicsObject> temp = m_physicsObjects;
            m_physicsObjects = new LinkedList<PhysicsObject>();
            foreach (PhysicsObject physObj in temp)
            {
                Vector2 position = physObj.GetPosition();
                double angularRotation = physObj.GetRotation();
                physObj.Update(time);

                // Check if the object moved at all
                if (position != physObj.GetPosition() || angularRotation != physObj.GetRotation())
                {
                    // Put object in fixing list
                    m_needFixing.AddLast(physObj);
                }
                else
                {
                    m_physicsObjects.AddLast(physObj);
                }
            }

            if (m_bHasSubdivided)
            {
                for (int i = 0; i < 4; i++)
                {
                    m_childNodes[i].UpdateObjects(time);
                }
            }
        }

        public void FixTree()
        {
            if (m_needFixing != null)
            {
                foreach (PhysicsObject physObj in m_needFixing)
                {
                    BubbleUpObject(physObj);
                }
                m_needFixing = null;
            }

            if (m_bHasSubdivided)
            {
                for (int i = 0; i < 4; i++)
                {
                    m_childNodes[i].FixTree();
                }
            }
        }

        public void CollideObjects()
        {
            LinkedList<PhysicsObject> collidedObjects = new LinkedList<PhysicsObject>();
            foreach (PhysicsObject firstObj in m_physicsObjects)
            {
                foreach (PhysicsObject secondObj in collidedObjects)
                {
                    firstObj.CollideWith(secondObj);
                }

                if (m_bHasSubdivided)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        m_childNodes[i].CollideAllWith(firstObj);
                    }
                }
                collidedObjects.AddLast(firstObj);
            }

            if (m_bHasSubdivided)
            {
                for (int i = 0; i < 4; i++)
                {
                    m_childNodes[i].CollideObjects();
                }
            }
        }

        public void ReapEmptyChildren()
        {
            if (!m_bHasSubdivided)
            {
                return;
            }

            bool emptyChildren = true;
            for (int i = 0; i < 4; i++)
            {
                m_childNodes[i].ReapEmptyChildren();
                if (m_childNodes[i].m_bHasSubdivided || m_childNodes[i].Count() > 0)
                {
                    emptyChildren = false;
                }
            }

            if (emptyChildren)
            {
                m_childNodes[0] = null;
                m_childNodes[1] = null;
                m_childNodes[2] = null;
                m_childNodes[3] = null;
                m_bHasSubdivided = false;
            }
        }

        private void CollideAllWith(PhysicsObject physObj)
        {
            foreach (PhysicsObject myObj in m_physicsObjects)
            {
                physObj.CollideWith(myObj);
            }

            if (m_bHasSubdivided)
            {
                for (int i = 0; i < 4; i++)
                {
                    m_childNodes[i].CollideAllWith(physObj);
                }
            }
        }
        
        public bool EntirelyContains(PhysicsObject physicsObject)
        {
            BoundingBox2D physicsObjectBox = physicsObject.GetCollider().GetBoundingBox();
            return m_boundaryBox.EntirelyContains(physicsObjectBox);
        }

        private void SubDivide()
        {
            LinkedList<PhysicsObject> temp = m_physicsObjects;
            m_physicsObjects = new LinkedList<PhysicsObject>();

            Vector2 myTopLeft = m_boundaryBox.GetTopLeft();
            Vector2 myBottomRight = m_boundaryBox.GetBottomRight();

            Vector2 halves = (myBottomRight - myTopLeft) / 2.0f;

            BoundingBox2D topLeft = new BoundingBox2D(myTopLeft, myTopLeft + halves);
            BoundingBox2D bottomLeft = new BoundingBox2D(new Vector2(myTopLeft.X, myTopLeft.Y + halves.Y), new Vector2(myTopLeft.X, myTopLeft.Y + halves.Y) + halves);
            BoundingBox2D topRight = new BoundingBox2D(new Vector2(myTopLeft.X + halves.X, myTopLeft.Y), new Vector2(myTopLeft.X + halves.X, myTopLeft.Y) + halves);
            BoundingBox2D bottomRight = new BoundingBox2D(myTopLeft + halves, myBottomRight);

            m_childNodes[0] = new QuadNode(this, topLeft, m_iDepth + 1);
            m_childNodes[1] = new QuadNode(this, bottomLeft, m_iDepth + 1);
            m_childNodes[2] = new QuadNode(this, topRight, m_iDepth + 1);
            m_childNodes[3] = new QuadNode(this, bottomRight, m_iDepth + 1);

            m_bHasSubdivided = true;

            LinkedList<PhysicsObject>.Enumerator enumerator = temp.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PhysicsObject physObj = enumerator.Current;
                AddPhysicsObject(physObj);
            }
        }
    }

    class QuadTree
    {
        QuadNode root;
        int m_numUpdates;
        int reapChildrenEvery;

        public QuadTree(BoundingBox2D boundaries)
        {
            root = new QuadNode(null, boundaries, 0);
            reapChildrenEvery = 10;
        }

        public void AddPhysicsObject(PhysicsObject physObj)
        {
            root.AddPhysicsObject(physObj);
        }

        public void UpdateObjects(GameTime time)
        {
            root.UpdateObjects(time);
            root.FixTree();
            root.CollideObjects();
            m_numUpdates++;
            if (m_numUpdates >= reapChildrenEvery)
            {
                m_numUpdates = 0;
                root.ReapEmptyChildren();
            }
        }
    }
}
