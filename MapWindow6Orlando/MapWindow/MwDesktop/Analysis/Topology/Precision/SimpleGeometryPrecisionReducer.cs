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
using MapWindow.Geometries.Utilities;
using MapWindow.Geometries;
namespace MapWindow.Analysis.Topology.Precision
{
    /// <summary>
    /// Reduces the precision of a <c>Geometry</c>
    /// according to the supplied {PrecisionModel}, without
    /// attempting to preserve valid topology.
    /// The topology of the resulting point may be invalid if
    /// topological collapse occurs due to coordinates being shifted.
    /// It is up to the client to check this and handle it if necessary.
    /// Collapses may not matter for some uses. An example
    /// is simplifying the input to the buffer algorithm.
    /// The buffer algorithm does not depend on the validity of the input point.
    /// </summary>
    public class SimpleGeometryPrecisionReducer
    {
        private readonly PrecisionModel _newPrecisionModel;
        private bool _removeCollapsed = true;
        private bool _changePrecisionModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pm"></param>
        public SimpleGeometryPrecisionReducer(PrecisionModel pm)
        {
            _newPrecisionModel = pm;
        }

        /// <summary>
        /// Sets whether the reduction will result in collapsed components
        /// being removed completely, or simply being collapsed to an (invalid)
        /// Geometry of the same type.
        /// </summary>
        public virtual bool RemoveCollapsedComponents
        {
            get
            {
                return _removeCollapsed;
            }
            set
            {
                _removeCollapsed = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the PrecisionModel of the new reduced Geometry
        /// will be changed to be the PrecisionModel supplied to
        /// specify the reduction.  
        /// The default is to not change the precision model.
        /// </summary>
        public virtual bool ChangePrecisionModel
        {
            get
            {
                return _changePrecisionModel;
            }
            set
            {
                _changePrecisionModel = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public virtual IGeometry Reduce(IGeometry geom)
        {
            GeometryEditor geomEdit;
            if (_changePrecisionModel) 
            {
                // GeometryFactory newFactory = new GeometryFactory(_newPrecisionModel, geom.SRID);
                GeometryFactory newFactory = new GeometryFactory(_newPrecisionModel);
                geomEdit = new GeometryEditor(newFactory);
            }
            else
            // don't change point factory
            geomEdit = new GeometryEditor();
            return geomEdit.Edit(geom, new PrecisionReducerCoordinateOperation(this));
        }

        /// <summary>
        /// 
        /// </summary>
        private class PrecisionReducerCoordinateOperation : GeometryEditor.CoordinateOperation
        {
            private readonly SimpleGeometryPrecisionReducer _container;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            public PrecisionReducerCoordinateOperation(SimpleGeometryPrecisionReducer container)
            {
                _container = container;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="coordinates"></param>
            /// <param name="geom"></param>
            /// <returns></returns>
            public override IList<Coordinate> Edit(IList<Coordinate> coordinates, IGeometry geom)
            {
                if (coordinates.Count == 0) 
                    return null;

                Coordinate[] reducedCoords = new Coordinate[coordinates.Count];
                // copy coordinates and reduce
                for (int i = 0; i < coordinates.Count; i++) 
                {
                    Coordinate coord = new Coordinate(coordinates[i]);
                    new PrecisionModel(_container._newPrecisionModel).MakePrecise(coord);
                    reducedCoords[i] = coord;
                }

                // remove repeated points, to simplify returned point as much as possible
                CoordinateList noRepeatedCoordList = new CoordinateList(reducedCoords, false);
                Coordinate[] noRepeatedCoords = noRepeatedCoordList.ToCoordinateArray();

                /*
                * Check to see if the removal of repeated points
                * collapsed the coordinate List to an invalid length
                * for the type of the parent point.
                * It is not necessary to check for Point collapses, since the coordinate list can
                * never collapse to less than one point.
                * If the length is invalid, return the full-length coordinate array
                * first computed, or null if collapses are being removed.
                * (This may create an invalid point - the client must handle this.)
                */
                int minLength = 0;
                if (geom is LineString) 
                    minLength = 2;
                if (geom is LinearRing) 
                    minLength = 4;

                Coordinate[] collapsedCoords = reducedCoords;
                if (_container._removeCollapsed) 
                    collapsedCoords = null;

                // return null or orginal length coordinate array
                if (noRepeatedCoords.Length < minLength) 
                    return collapsedCoords;                

                // ok to return shorter coordinate array
                return noRepeatedCoords;
            }
        }
    }
}
