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
using MapWindow.Geometries;

namespace MapWindow.Analysis.Topology.Algorithm
{
    /// <summary> 
    /// Represents a homogeneous coordinate for 2-D coordinates.
    /// </summary>
    public class HCoordinate
    {
        /// <summary> 
        /// Computes the (approximate) intersection point between two line segments
        /// using homogeneous coordinates.
        /// Note that this algorithm is
        /// not numerically stable; i.e. it can produce intersection points which
        /// lie outside the envelope of the line segments themselves.  In order
        /// to increase the precision of the calculation input points should be normalized
        /// before passing them to this routine.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static Coordinate Intersection(Coordinate p1, Coordinate p2, Coordinate q1, Coordinate q2)            
        {
            HCoordinate l1 = new HCoordinate(new HCoordinate(p1), new HCoordinate(p2));
            HCoordinate l2 = new HCoordinate(new HCoordinate(q1), new HCoordinate(q2));
            HCoordinate intHCoord = new HCoordinate(l1, l2);
            Coordinate intPt = intHCoord.Coordinate;
            return intPt;
        }

        private double x;
        private double y;
        private double w;

        /// <summary>
        /// Direct access to x private field
        /// </summary>
        [Obsolete("This is a simple access to x private field: use GetX() instead.")]
        protected virtual double X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Direct access to y private field
        /// </summary>
        [Obsolete("This is a simple access to y private field: use GetY() instead.")]
        protected virtual double Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Direct access to w private field
        /// </summary>
        [Obsolete("This is a simple access to w private field: how do you use this field for?...")]
        protected virtual double W
        {
            get { return w; }
            set { w = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public HCoordinate()
        {
            x = 0.0;
            y = 0.0;
            w = 1.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        public HCoordinate(double x, double y, double w) 
        {
            this.x = x;
            this.y = y;
            this.w = w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public HCoordinate(Coordinate p) 
        {
            x = p.X;
            y = p.Y;
            w = 1.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public HCoordinate(HCoordinate p1, HCoordinate p2) 
        {
            x = p1.y * p2.w - p2.y * p1.w;
            y = p2.x * p1.w - p1.x * p2.w;
            w = p1.x * p2.y - p2.x * p1.y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual double GetX()
        {
            double a = x/w;
            if((Double.IsNaN(a)) || (Double.IsInfinity(a))) 
                throw new NotRepresentableException();                
            return a;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual double GetY()
        {            
            double a = y/w;
            if((Double.IsNaN(a)) || (Double.IsInfinity(a))) 
                throw new NotRepresentableException();            
            return a;            
        }        

        /// <summary>
        /// 
        /// </summary>
        public virtual Coordinate Coordinate
        {
            get 
            { 
                return new Coordinate(GetX(), GetY()); 
            }
        }
    }
}
