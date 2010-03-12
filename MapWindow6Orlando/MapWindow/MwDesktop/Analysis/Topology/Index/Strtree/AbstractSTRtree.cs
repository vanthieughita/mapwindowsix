//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The basic module for MapWindow version 6.0
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/  Alternately, you can access an earlier version of this content from
// the Net Topology Suite, which is protected by the GNU Lesser Public License
// http://www.gnu.org/licenses/lgpl.html and the sourcecode for the Net Topology Suite
// can be obtained here: http://sourceforge.net/projects/nts.
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is from the Net Topology Suite
//
// The Initial Developer to integrate this code into MapWindow 6.0 is Ted Dunsford.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Collections;
using System.Text;

using MapWindow.Analysis.Topology.Utilities;

namespace MapWindow.Analysis.Topology.Index.Strtree
{
    /// <summary>
    /// Base class for STRtree and SIRtree. STR-packed R-trees are described in:
    /// P. Rigaux, Michel Scholl and Agnes Voisard. Spatial Databases With
    /// Application To GIS. Morgan Kaufmann, San Francisco, 2002.
    /// <para>
    /// This implementation is based on Boundables rather than just AbstractNodes,
    /// because the STR algorithm operates on both nodes and
    /// data, both of which are treated here as Boundables.
    /// </para>
    /// </summary>
    public abstract class AbstractSTRtree
    {

        #region Private Variables

        private AbstractNode _root;
        private bool built = false;
        private ArrayList itemBoundables = new ArrayList();
        private int nodeCapacity;

        #endregion

        /// <returns>
        /// A test for intersection between two bounds, necessary because subclasses
        /// of AbstractSTRtree have different implementations of bounds.
        /// </returns>
        protected interface IIntersectsOp
        {            
            /// <summary>
            /// For STRtrees, the bounds will be Envelopes; 
            /// for SIRtrees, Intervals;
            /// for other subclasses of AbstractSTRtree, some other class.
            /// </summary>
            /// <param name="aBounds">The bounds of one spatial object.</param>
            /// <param name="bBounds">The bounds of another spatial object.</param>                        
            /// <returns>Whether the two bounds intersect.</returns>
            bool Intersects(object aBounds, object bBounds);
        }

       
      

        /// <summary> 
        /// Constructs an AbstractSTRtree with the specified maximum number of child
        /// nodes that a node may have.
        /// </summary>
        /// <param name="nodeCapacity"></param>
        public AbstractSTRtree(int nodeCapacity)
        {
            Assert.IsTrue(nodeCapacity > 1, "Node capacity must be greater than 1");
            this.nodeCapacity = nodeCapacity;
        }

        /// <summary> 
        /// Creates parent nodes, grandparent nodes, and so forth up to the root
        /// node, for the data that has been inserted into the tree. Can only be
        /// called once, and thus can be called only after all of the data has been
        /// inserted into the tree.
        /// </summary>
        public virtual void Build()
        {
            Assert.IsTrue(!built);            
            _root = (itemBoundables.Count == 0) ? CreateNode(0) : CreateHigherLevels(itemBoundables, -1);
            built = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected abstract AbstractNode CreateNode(int level);

        /// <summary>
        /// Sorts the childBoundables then divides them into groups of size M, where
        /// M is the node capacity.
        /// </summary>
        /// <param name="childBoundables"></param>
        /// <param name="newLevel"></param>
        protected virtual IList CreateParentBoundables(IList childBoundables, int newLevel)
        {
            Assert.IsTrue(childBoundables.Count != 0);
            ArrayList parentBoundables = new ArrayList();
            parentBoundables.Add(CreateNode(newLevel));
            ArrayList sortedChildBoundables = new ArrayList(childBoundables);            
            sortedChildBoundables.Sort(GetComparer());            
            for (IEnumerator i = sortedChildBoundables.GetEnumerator(); i.MoveNext(); )
            {
                IBoundable childBoundable = (IBoundable)i.Current;
                if (LastNode(parentBoundables).ChildBoundables.Count == NodeCapacity)
                    parentBoundables.Add(CreateNode(newLevel));                
                LastNode(parentBoundables).AddChildBoundable(childBoundable);
            }
            return parentBoundables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected virtual AbstractNode LastNode(IList nodes)
        {
            return (AbstractNode)nodes[nodes.Count - 1];
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual int CompareDoubles(double a, double b)
        {
            return a > b ? 1 : a < b ? -1 : 0;
        }

        /// <summary>
        /// Creates the levels higher than the given level.
        /// </summary>
        /// <param name="boundablesOfALevel">The level to build on.</param>
        /// <param name="level">the level of the Boundables, or -1 if the boundables are item
        /// boundables (that is, below level 0).</param>
        /// <returns>The root, which may be a ParentNode or a LeafNode.</returns>
        private AbstractNode CreateHigherLevels(IList boundablesOfALevel, int level)
        {
            Assert.IsTrue(boundablesOfALevel.Count != 0);
            IList parentBoundables = CreateParentBoundables(boundablesOfALevel, level + 1);
            if (parentBoundables.Count == 1)
                return (AbstractNode)parentBoundables[0];            
            return CreateHigherLevels(parentBoundables, level + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual AbstractNode Root
        {
            get
            {
                return _root;
            }
            protected set
            {
                _root = value;
            }
        }

        /// <summary> 
        /// Returns the maximum number of child nodes that a node may have.
        /// </summary>
        public virtual int NodeCapacity
        {
            get
            {
                return nodeCapacity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Count
        {
            get
            {
                if (!built) 
                    Build();                 
                if (itemBoundables.Count == 0)                
                    return 0;
                return GetSize(_root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual int GetSize(AbstractNode node)
        {
            int size = 0;
            for (IEnumerator i = node.ChildBoundables.GetEnumerator(); i.MoveNext(); ) 
            {
                IBoundable childBoundable = (IBoundable) i.Current;
                if (childBoundable is AbstractNode)                 
                    size += GetSize((AbstractNode)childBoundable);
                else if (childBoundable is ItemBoundable) 
                    size += 1;            
            }
            return size;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Depth
        {
            get
            {
                if (!built)  
                    Build();                 
                if (itemBoundables.Count == 0)                
                    return 0;
                return GetDepth(_root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual int GetDepth(AbstractNode node)
        {
            int maxChildDepth = 0;
            for (IEnumerator i = node.ChildBoundables.GetEnumerator(); i.MoveNext(); ) 
            {
                IBoundable childBoundable = (IBoundable) i.Current;
                if (childBoundable is AbstractNode) 
                {
                    int childDepth = GetDepth((AbstractNode) childBoundable);
                    if (childDepth > maxChildDepth)
                        maxChildDepth = childDepth;
                }
            }
            return maxChildDepth + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="item"></param>
        protected virtual void Insert(object bounds, object item)
        {
            Assert.IsTrue(!built, "Cannot insert items into an STR packed R-tree after it has been built.");
            itemBoundables.Add(new ItemBoundable(bounds, item));
        }

        /// <summary>
        /// Also builds the tree, if necessary.
        /// </summary>
        /// <param name="searchBounds"></param>
        protected virtual IList Query(object searchBounds)
        {
            if (!built)             
                Build();             
            ArrayList matches = new ArrayList();            
            if (itemBoundables.Count == 0)
            {
                Assert.IsTrue(_root.Bounds == null);
                return matches;
            }
            if (IntersectsOp.Intersects(_root.Bounds, searchBounds))
                Query(searchBounds, _root, matches);            
            return matches;
        }

        /// <summary>
        /// Also builds the tree, if necessary.
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <param name="visitor"></param>
        protected virtual void Query(Object searchBounds, IItemVisitor visitor)
        {
            if(!built) 
                Build(); 

            if(itemBoundables.Count == 0)
                Assert.IsTrue(_root.Bounds == null);

            if (IntersectsOp.Intersects(_root.Bounds, searchBounds))
                Query(searchBounds, _root, visitor);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <param name="node"></param>
        /// <param name="matches"></param>
        private void Query(object searchBounds, AbstractNode node, IList matches) 
        {
            foreach(object obj in node.ChildBoundables) 
            {
                IBoundable childBoundable = (IBoundable)obj;
                if (!IntersectsOp.Intersects(childBoundable.Bounds, searchBounds)) 
                    continue;      
      
                if(childBoundable is AbstractNode) 
                    Query(searchBounds, (AbstractNode) childBoundable, matches);      
                else if(childBoundable is ItemBoundable) 
                    matches.Add(((ItemBoundable)childBoundable).Item);
                else Assert.ShouldNeverReachHere();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <param name="node"></param>
        /// <param name="visitor"></param>
        private void Query(object searchBounds, AbstractNode node, IItemVisitor visitor) 
        {
            foreach(object obj in node.ChildBoundables) 
            {
                IBoundable childBoundable = (IBoundable)obj;
                if(!IntersectsOp.Intersects(childBoundable.Bounds, searchBounds))
                    continue;      
                if(childBoundable is AbstractNode) 
                    Query(searchBounds, (AbstractNode)childBoundable, visitor);      
                else if (childBoundable is ItemBoundable) 
                    visitor.VisitItem(((ItemBoundable)childBoundable).Item);            
                else Assert.ShouldNeverReachHere();
            }
        }

        /// <returns>
        /// A test for intersection between two bounds, necessary because subclasses
        /// of AbstractSTRtree have different implementations of bounds.
        /// </returns>
        protected abstract IIntersectsOp IntersectsOp { get; }        

        /// <summary>
        /// Also builds the tree, if necessary.
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <param name="item"></param>
        protected virtual bool Remove(object searchBounds, object item)
        {
            if (!built)             
                Build();                         
            if (itemBoundables.Count == 0)
                Assert.IsTrue(_root.Bounds == null);
            if (IntersectsOp.Intersects(_root.Bounds, searchBounds))
                return Remove(searchBounds, _root, item);            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RemoveItem(AbstractNode node, object item)
        {
            IBoundable childToRemove = null;
            for (IEnumerator i = node.ChildBoundables.GetEnumerator(); i.MoveNext(); ) 
            {
                IBoundable childBoundable = (IBoundable) i.Current;
                if (childBoundable is ItemBoundable)                 
                    if (((ItemBoundable) childBoundable).Item == item)
                        childToRemove = childBoundable;                
            }
            if (childToRemove != null) 
            {
                node.ChildBoundables.Remove(childToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool Remove(object searchBounds, AbstractNode node, object item) 
        {
            // first try removing item from this node
            bool found = RemoveItem(node, item);
            if (found)
                return true;
            AbstractNode childToPrune = null;
            // next try removing item from lower nodes
            for (IEnumerator i = node.ChildBoundables.GetEnumerator(); i.MoveNext(); ) 
            {
                IBoundable childBoundable = (IBoundable) i.Current;
                if (!IntersectsOp.Intersects(childBoundable.Bounds, searchBounds)) 
                    continue;                
                if (childBoundable is AbstractNode) 
                {
                    found = Remove(searchBounds, (AbstractNode) childBoundable, item);
                    // if found, record child for pruning and exit
                    if (found) 
                    {
                        childToPrune = (AbstractNode) childBoundable;
                        break;
                    }
                }
            }
            // prune child if possible
            if (childToPrune != null)             
                if (childToPrune.ChildBoundables.Count == 0) 
                    node.ChildBoundables.Remove(childToPrune);                        
            return found;
        }   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected virtual IList BoundablesAtLevel(int level)
        {            
            IList boundables = new ArrayList();
            BoundablesAtLevel(level, _root, ref boundables);
            return boundables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">-1 to get items.</param>
        /// <param name="top"></param>
        /// <param name="boundables"></param>      
        private void BoundablesAtLevel(int level, AbstractNode top, ref IList boundables) 
        {
            Assert.IsTrue(level > -2);
            if (top.Level == level) 
            {
                boundables.Add(top);
                return;
            }
            for (IEnumerator i = top.ChildBoundables.GetEnumerator(); i.MoveNext(); ) 
            {
                IBoundable boundable = (IBoundable) i.Current;
                if (boundable is AbstractNode) 
                    BoundablesAtLevel(level, (AbstractNode)boundable, ref boundables);            
                else 
                {
                    Assert.IsTrue(boundable is ItemBoundable);
                    if (level == -1)                     
                        boundables.Add(boundable);                     
                }
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IComparer GetComparer();
    }
}
