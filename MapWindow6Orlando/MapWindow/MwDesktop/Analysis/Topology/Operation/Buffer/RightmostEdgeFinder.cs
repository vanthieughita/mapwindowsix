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

using System.Collections;
using System.Collections.Generic;
using MapWindow.Geometries;
using MapWindow.GeometriesGraph;
using MapWindow.Analysis.Topology.Algorithm;
using MapWindow.Analysis.Topology.Utilities;
namespace MapWindow.Analysis.Topology.Operation.Buffer
{
    /// <summary>
    /// A RightmostEdgeFinder find the DirectedEdge in a list which has the highest coordinate,
    /// and which is oriented L to R at that point. (I.e. the right side is on the RHS of the edge.)
    /// </summary>
    public sealed class RightmostEdgeFinder
    {        
        private int _minIndex = -1;
        private Coordinate _minCoord;
        private DirectedEdge _minDe;
        private DirectedEdge _orientedDe;



        /// <summary>
        /// 
        /// </summary>
        public DirectedEdge Edge
        {
            get
            {
                return _orientedDe;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Coordinate Coordinate
        {
            get
            {
                return _minCoord;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirEdgeList"></param>
        public void FindEdge(IList dirEdgeList)
        {
            /*
             * Check all forward DirectedEdges only.  This is still general,
             * because each edge has a forward DirectedEdge.
             */
            for (IEnumerator i = dirEdgeList.GetEnumerator(); i.MoveNext(); )
            {
                DirectedEdge de = (DirectedEdge)i.Current;
                if (!de.IsForward) continue;
                CheckForRightmostCoordinate(de);
            }

            /*
             * If the rightmost point is a node, we need to identify which of
             * the incident edges is rightmost.
             */
            Assert.IsTrue(_minIndex != 0 || _minCoord.Equals(_minDe.Coordinate), "inconsistency in rightmost processing");
            if (_minIndex == 0)            
                 FindRightmostEdgeAtNode();            
            else FindRightmostEdgeAtVertex();            

            /*
             * now check that the extreme side is the R side.
             * If not, use the sym instead.
             */
            _orientedDe = _minDe;
            Positions rightmostSide = GetRightmostSide(_minDe, _minIndex);
            if (rightmostSide == Positions.Left)            
                _orientedDe = _minDe.Sym;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FindRightmostEdgeAtNode()
        {
            Node node = _minDe.Node;
            DirectedEdgeStar star = (DirectedEdgeStar)node.Edges;
            _minDe = star.GetRightmostEdge();
            // the DirectedEdge returned by the previous call is not
            // necessarily in the forward direction. Use the sym edge if it isn't.
            if (!_minDe.IsForward)
            {
                _minDe = _minDe.Sym;
                _minIndex = _minDe.Edge.Coordinates.Count - 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FindRightmostEdgeAtVertex()
        {
            /*
             * The rightmost point is an interior vertex, so it has a segment on either side of it.
             * If these segments are both above or below the rightmost point, we need to
             * determine their relative orientation to decide which is rightmost.
             */
            IList<Coordinate> pts = _minDe.Edge.Coordinates;
            Assert.IsTrue(_minIndex > 0 && _minIndex < pts.Count, "rightmost point expected to be interior vertex of edge");
            Coordinate pPrev = pts[_minIndex - 1];
            Coordinate pNext = pts[_minIndex + 1];
            int orientation = CGAlgorithms.ComputeOrientation(_minCoord, pNext, pPrev);
            bool usePrev = false;
            // both segments are below min point
            if (pPrev.Y < _minCoord.Y && pNext.Y < _minCoord.Y && orientation == CGAlgorithms.CounterClockwise)            
                usePrev = true;            
            else if (pPrev.Y > _minCoord.Y && pNext.Y > _minCoord.Y && orientation == CGAlgorithms.Clockwise)            
                usePrev = true;            
            // if both segments are on the same side, do nothing - either is safe
            // to select as a rightmost segment
            if (usePrev) _minIndex = _minIndex - 1;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        private void CheckForRightmostCoordinate(DirectedEdge de)
        {
            IList<Coordinate> coord = de.Edge.Coordinates;
            for (int i = 0; i < coord.Count - 1; i++)
            {
                // only check vertices which are the start or end point of a non-horizontal segment
                // <FIX> MD 19 Sep 03 - NO!  we can test all vertices, since the rightmost must have a non-horiz segment adjacent to it
                if (_minCoord == null || coord[i].X > _minCoord.X)
                {
                    _minDe = de;
                    _minIndex = i;
                    _minCoord = coord[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Positions GetRightmostSide(DirectedEdge de, int index)
        {
            Positions side = GetRightmostSideOfSegment(de, index);
            if (side < 0)
                side = GetRightmostSideOfSegment(de, index - 1);
            if (side < 0)
            {
                // reaching here can indicate that segment is horizontal                
                _minCoord = null;
                CheckForRightmostCoordinate(de);
            }
            return side;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Positions GetRightmostSideOfSegment(EdgeEnd de, int i)
        {
            Edge e = de.Edge;
            IList<Coordinate> coord = e.Coordinates;

            if (i < 0 || i + 1 >= coord.Count) 
                return Positions.Parallel;
            if (coord[i].Y == coord[i + 1].Y)
                return Positions.Parallel;    

            Positions pos = Positions.Left;
            if (coord[i].Y < coord[i + 1].Y) 
                pos = Positions.Right;

            return pos;
        }
    }
}
