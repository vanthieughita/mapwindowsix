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
using System.Collections.Generic;
using MapWindow.Geometries;

namespace MapWindow.Analysis.Topology.Operation.Predicate
{
    /// <summary>
    /// Optimized implementation of spatial predicate "contains"
    /// for cases where the first <c>Geometry</c> is a rectangle.    
    /// As a further optimization,
    /// this class can be used directly to test many geometries against a single rectangle.
    /// </summary>
    public class RectangleContains
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Contains(Polygon rectangle, IGeometry b)
        {
            RectangleContains rc = new RectangleContains(rectangle);
            return rc.Contains(b);
        }

        private Polygon _rectangle;
        private readonly IEnvelope _rectEnv;

        /// <summary>
        /// Create a new contains computer for two geometries.
        /// </summary>
        /// <param name="rectangle">A rectangular geometry.</param>
        public RectangleContains(Polygon rectangle)
        {
            _rectangle = rectangle;
            _rectEnv = rectangle.EnvelopeInternal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public bool Contains(IGeometry geom)
        {
            if (!_rectEnv.Contains(geom.EnvelopeInternal))
                return false;
            // check that geom is not contained entirely in the rectangle boundary
            if (IsContainedInBoundary(geom))
                return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        private bool IsContainedInBoundary(IGeometry geom)
        {
            // polygons can never be wholely contained in the boundary
            if (geom is IPolygon || geom is IMultiPolygon) 
                return false;
            if (geom is IPoint) 
                return IsPointContainedInBoundary(geom.Coordinate);
            if (geom is ILineString) 
                return IsLineStringContainedInBoundary(geom);

            for (int i = 0; i < geom.NumGeometries; i++) 
            {
                IGeometry comp = ((Geometry)geom).GetGeometryN(i);
                if (!IsContainedInBoundary(comp))
                    return false;
            }
            return true;
        }

       
        /// <summary>
        /// Given any valid implementation of ICoordinate, which
        /// will basically provide an X, Y or Z values, this will determine
        /// if the rectangle contains the point.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private bool IsPointContainedInBoundary(Coordinate pt)
        {
            // we already know that the point is contained in the rectangle envelope
            if (!(pt.X == _rectEnv.Minimum.X || pt.X == _rectEnv.Maximum.X))
                return false;
            if (!(pt.Y == _rectEnv.Minimum.Y || pt.Y == _rectEnv.Maximum.Y))
                return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool IsLineStringContainedInBoundary(IBasicGeometry line)
        {
            IList<Coordinate> seq = line.Coordinates;

            for (int i = 0; i < seq.Count - 1; i++)
            {
                if (!IsLineSegmentContainedInBoundary(seq[i], seq[i+1]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        private bool IsLineSegmentContainedInBoundary(Coordinate p0, Coordinate p1)
        {
            if (p0.Equals(p1))
                return IsPointContainedInBoundary(p0);
            // we already know that the segment is contained in the rectangle envelope
            if (p0.X == p1.X)
            {
                if (p0.X == _rectEnv.Minimum.X || 
                    p0.X == _rectEnv.Maximum.X)
                        return true;
            }
            else if (p0.Y == p1.Y)
            {
                if (p0.Y == _rectEnv.Minimum.Y || 
                    p0.Y == _rectEnv.Maximum.Y)
                        return true;
            }
            /*
             * Either both x and y values are different
             * or one of x and y are the same, but the other ordinate is not the same as a boundary ordinate
             * In either case, the segment is not wholely in the boundary
             */
            return false;         
        }
    }
}
