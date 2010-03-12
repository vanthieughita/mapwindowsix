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

namespace MapWindow.Analysis.Topology.Planargraph
{
    /// <summary>
    /// Represents an undirected edge of a {PlanarGraph}. An undirected edge
    /// in fact simply acts as a central point of reference for two opposite
    /// <c>DirectedEdge</c>s.
    /// Usually a client using a <c>PlanarGraph</c> will subclass <c>Edge</c>
    /// to add its own application-specific data and methods.
    /// </summary>
    public class Edge : GraphComponent
    {
        /// <summary>
        /// The two DirectedEdges associated with this Edge. 
        /// </summary>
        protected DirectedEdge[] dirEdge;

        /// <summary>
        /// Constructs an Edge whose DirectedEdges are not yet set. Be sure to call
        /// <c>SetDirectedEdges(DirectedEdge, DirectedEdge)</c>.
        /// </summary>
        public Edge() { }

        /// <summary>
        /// Constructs an Edge initialized with the given DirectedEdges, and for each
        /// DirectedEdge: sets the Edge, sets the symmetric DirectedEdge, and adds
        /// this Edge to its from-Node.
        /// </summary>
        /// <param name="de0"></param>
        /// <param name="de1"></param>
        public Edge(DirectedEdge de0, DirectedEdge de1)
        {
            SetDirectedEdges(de0, de1);
        }

        /// <summary>
        /// Initializes this Edge's two DirectedEdges, and for each DirectedEdge: sets the
        /// Edge, sets the symmetric DirectedEdge, and adds this Edge to its from-Node.
        /// </summary>
        /// <param name="de0"></param>
        /// <param name="de1"></param>
        public virtual void SetDirectedEdges(DirectedEdge de0, DirectedEdge de1)
        {
            dirEdge = new DirectedEdge[] { de0, de1, };
            de0.Edge = this;
            de1.Edge = this;
            de0.Sym = de1;
            de1.Sym = de0;
            de0.FromNode.AddOutEdge(de0);
            de1.FromNode.AddOutEdge(de1);
        }

        /// <summary> 
        /// Returns one of the DirectedEdges associated with this Edge.
        /// </summary>
        /// <param name="i">0 or 1.</param>
        /// <returns></returns>
        public virtual DirectedEdge GetDirEdge(int i)
        {
            return dirEdge[i];
        }

        /// <summary>
        /// Returns the DirectedEdge that starts from the given node, or null if the
        /// node is not one of the two nodes associated with this Edge.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <returns></returns>
        public virtual DirectedEdge GetDirEdge(Node fromNode)
        {
            if (dirEdge[0].FromNode == fromNode) 
                return dirEdge[0];
            if (dirEdge[1].FromNode == fromNode) 
                return dirEdge[1];
            // node not found
            // possibly should throw an exception here?
            return null;
        }

        /// <summary> 
        /// If <c>node</c> is one of the two nodes associated with this Edge,
        /// returns the other node; otherwise returns null.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual Node GetOppositeNode(Node node)
        {
            if (dirEdge[0].FromNode == node) 
                return dirEdge[0].ToNode;
            if (dirEdge[1].FromNode == node) 
                return dirEdge[1].ToNode;
            // node not found
            // possibly should throw an exception here?
            return null;
        }

        /// <summary>
        /// Removes this edge from its containing graph.
        /// </summary>
        internal void Remove()
        {
            this.dirEdge = null;
        }

        /// <summary>
        /// Tests whether this component has been removed from its containing graph.
        /// </summary>
        /// <value></value>
        public override bool IsRemoved
        {
            get
            {
                return dirEdge == null;
            }
        }

        /// <summary>
        /// The line sequencer class seems to need to se this directly
        /// </summary>
        public new bool IsVisited
        {
            get
            {
                return base.IsVisited;
            }
            set
            {
                base.IsVisited = value;
            }
        }

        /*
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Edge))
                return false;
            if (!base.Equals(obj))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;            

            Edge other = obj as Edge;
            for(int i = 0; i < dirEdge.Length; i++)
                if(dirEdge[i] != other.dirEdge[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = 29 * base.GetHashCode();
            result += 14 + 29 * dirEdge.Length.GetHashCode();
            for (int i = 0; i < dirEdge.Length; i++)
                result += 14 + 29 * dirEdge.GetHashCode();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(Edge o1, Edge o2)
        {
            return Object.Equals(o1, o2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator !=(Edge o1, Edge o2)
        {
            return !(o1 == o2);
        }

        */

    }
}
