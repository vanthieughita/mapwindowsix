//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The basic module for MapWindow version 6.0
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 3/1/2010 11:43:08 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using MapWindow.Data;
using MapWindow.Geometries;

namespace MapWindow.Analysis
{


    /// <summary>
    /// PointShape
    /// </summary>
    public static class PointShape
    {

        

        /// <summary>
        /// Gets or sets the precision for calculating equality, but this is just a re-direction to Vertex.Epsilon
        /// </summary>
        public static double Epsilon
        {
            get { return Vertex.Epsilon; }
            set { Vertex.Epsilon = value; }
        }


        /// <summary>
        /// Calculates the intersection of a polygon shape without relying on the NTS geometry
        /// </summary>
        /// <param name="pointShape"></param>
        /// <param name="otherShape"></param>
        /// <returns></returns>
        public static bool Intersects(ShapeRange pointShape, ShapeRange otherShape)
        {
            if(pointShape.FeatureType != FeatureTypes.Point && pointShape.FeatureType != FeatureTypes.MultiPoint)
            {
                throw new ArgumentException("The First parameter should be a point shape, but it was featuretype:" + pointShape.FeatureType);
            }

            // Implmented in PolygonShape or line shape.  Point shape is the simplest and just looks for overlapping coordinates.
            if (otherShape.FeatureType == FeatureTypes.Polygon || otherShape.FeatureType == FeatureTypes.Line)
            {
                return otherShape.Intersects(pointShape);
            }

            // For two point-type shapes, test if any vertex from one overlaps with any vertex of the other within Epsilon tollerance
            return VerticesIntersect(pointShape, otherShape);
            
        }
       
        /// <summary>
        /// Returns true if any vertices overlap
        /// </summary>
        /// <returns></returns>
        public static bool VerticesIntersect(ShapeRange pointShape, ShapeRange otherPointShape)
        {
            foreach (PartRange part in pointShape.Parts)
            {
                foreach (PartRange oPart in otherPointShape.Parts)
                {
                    foreach(Vertex v1 in part)
                    {
                        foreach (Vertex v2 in oPart)
                        {
                            if(v1 == v2) return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}
