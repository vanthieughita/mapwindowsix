//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The core libraries for the MapWindow 6.0 project.
//
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is MapWindow.dll for the MapWindow 6.0 project
//
// The Initial Developer of this Original Code is Ted Dunsford. Created in 2007.
// 
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
// Modified to support VersionID in July 2008 by Ted Dunsford
//********************************************************************************************************
using System;
using System.Collections.Generic;
namespace MapWindow.Geometries
{
    /// <summary>
    /// Creates CoordinateSequences represented as an array of Coordinates.
    /// </summary>
    [Serializable]
    public sealed class CoordinateListSequenceFactory : ICoordinateSequenceFactory
    {
        /// <summary>
        /// Creates an instance of the CoordinateListSequenceFactory
        /// </summary>
        public static readonly CoordinateListSequenceFactory Instance = new CoordinateListSequenceFactory();



       
        /// <summary>
        ///  Returns a CoordinateArraySequence based on the given array (the array is not copied).
        /// </summary>
        /// <param name="coordinates">the coordinates, which may not be null nor contain null elements.</param>
        /// <returns></returns>
        public ICoordinateSequence Create(IEnumerable<Coordinate> coordinates) 
        {
            return new CoordinateListSequence(coordinates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordSeq"></param>
        /// <returns></returns>
        public ICoordinateSequence Create(ICoordinateSequence coordSeq) 
        {
            return new CoordinateListSequence(coordSeq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICoordinateSequence Create() 
        {
            return new CoordinateListSequence();
        }

        /// <summary>
        /// Creates a new CoordinateListSequence and populates it with a single coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public ICoordinateSequence Create(Coordinate coordinate)
        {
            return new CoordinateListSequence(coordinate);
        }

       
    }
}
