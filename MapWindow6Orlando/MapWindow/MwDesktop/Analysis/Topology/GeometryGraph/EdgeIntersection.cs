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
using System.IO;

using MapWindow.Geometries;
namespace MapWindow.GeometriesGraph
{
    /// <summary> 
    /// An EdgeIntersection represents a point on an
    /// edge which intersects with another edge.
    /// The intersection may either be a single point, or a line segment
    /// (in which case this point is the start of the line segment)
    /// The label attached to this intersection point applies to
    /// the edge from this point forwards, until the next
    /// intersection or the end of the edge.
    /// The intersection point must be precise.
    /// </summary>
    public class EdgeIntersection : IComparable
    {
        private Coordinate coordinate;   

        /// <summary>
        /// The point of intersection.
        /// </summary>
        public virtual Coordinate Coordinate
        {
            get
            {
                return coordinate; 
            }
            set
            {
                coordinate = value; 
            }
        }

        private int segmentIndex;  

        /// <summary>
        /// The index of the containing line segment in the parent edge.
        /// </summary>
        public virtual int SegmentIndex
        {
            get 
            {
                return segmentIndex; 
            }
            set
            {
                segmentIndex = value; 
            }
        }

        private double dist;       

        /// <summary>
        /// The edge distance of this point along the containing line segment.
        /// </summary>
        public virtual double Distance
        {
            get
            {
                return dist; 
            }
            set
            {
                dist = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="dist"></param>
        public EdgeIntersection(Coordinate coord, int segmentIndex, double dist) 
        {
            this.coordinate = new Coordinate(coord);
            this.segmentIndex = segmentIndex;
            this.dist = dist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
        {
            EdgeIntersection other = (EdgeIntersection)obj;
            return Compare(other.SegmentIndex, other.Distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <param name="dist"></param>
        /// <returns>
        /// -1 this EdgeIntersection is located before the argument location,
        /// 0 this EdgeIntersection is at the argument location,
        /// 1 this EdgeIntersection is located after the argument location.
        /// </returns>
        public virtual int Compare(int segmentIndex, double dist)
        {
            if (this.SegmentIndex < segmentIndex) 
                return -1;
            if (this.SegmentIndex > segmentIndex) 
                return 1;
            if (this.Distance < dist) 
                return -1;
            if (this.Distance > dist) 
                return 1;
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSegmentIndex"></param>
        /// <returns></returns>
        public virtual bool IsEndPoint(int maxSegmentIndex)
        {
            if (SegmentIndex == 0 && Distance == 0.0) 
                return true;
            if (SegmentIndex == maxSegmentIndex) 
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outstream"></param>
        public virtual void Write(StreamWriter outstream)
        {
            outstream.Write(Coordinate);
            outstream.Write(" seg # = " + SegmentIndex);
            outstream.WriteLine(" dist = " + Distance);
        }
    }
}
