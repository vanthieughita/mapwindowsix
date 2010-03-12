
using System.Collections;
using System.Collections.Generic;
using MapWindow.Geometries;
using MapWindow.Geometries.Utilities;

namespace MapWindow.Analysis.Topology.Simplify
{
    /// <summary>
    /// Simplifies a point, ensuring that
    /// the result is a valid point having the
    /// same dimension and number of components as the input.
    /// The simplification uses a maximum distance difference algorithm
    /// similar to the one used in the Douglas-Peucker algorithm.
    /// In particular, if the input is an areal point
    /// ( <c>Polygon</c> or <c>MultiPolygon</c> )
    /// The result has the same number of shells and holes (rings) as the input,
    /// in the same order
    /// The result rings touch at no more than the number of touching point in the input
    /// (although they may touch at fewer points).
    /// </summary>
    public class TopologyPreservingSimplifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="distanceTolerance"></param>
        /// <returns></returns>
        public static IGeometry Simplify(IGeometry geom, double distanceTolerance)
        {
            TopologyPreservingSimplifier tss = new TopologyPreservingSimplifier(geom);
            tss.DistanceTolerance = distanceTolerance;
            return tss.GetResultGeometry();
        }

        private readonly IGeometry _inputGeom;
        private readonly TaggedLinesSimplifier _lineSimplifier = new TaggedLinesSimplifier();
        private IDictionary _lineStringMap;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputGeom"></param>
        public TopologyPreservingSimplifier(IGeometry inputGeom)
        {
            _inputGeom = inputGeom;            
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual double DistanceTolerance
        {
            get
            {
                return _lineSimplifier.DistanceTolerance;
            }
            set
            {
                _lineSimplifier.DistanceTolerance = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IGeometry GetResultGeometry() 
        {
            _lineStringMap = new Hashtable();
            _inputGeom.Apply(new LineStringMapBuilderFilter(this));
            _lineSimplifier.Simplify(new ArrayList(_lineStringMap.Values));
            IGeometry result = (new LineStringTransformer(this)).Transform(_inputGeom);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private class LineStringTransformer : GeometryTransformer
        {
            private readonly TopologyPreservingSimplifier _container;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            public LineStringTransformer(TopologyPreservingSimplifier container)            
            {
                _container = container;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="coords"></param>
            /// <param name="parent"></param>
            /// <returns></returns>
            protected override IList<Coordinate> TransformCoordinates(IList<Coordinate> coords, IGeometry parent)
            {
                if (parent is LineString) 
                {
                    TaggedLineString taggedLine = (TaggedLineString) _container._lineStringMap[parent];
                    return taggedLine.ResultCoordinates;
                }
                // for anything else (e.g. points) just copy the coordinates
                return base.TransformCoordinates(coords, parent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LineStringMapBuilderFilter : IGeometryComponentFilter
        {
            private readonly TopologyPreservingSimplifier _container;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            public LineStringMapBuilderFilter(TopologyPreservingSimplifier container)            
            {
                _container = container;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="geom"></param>
            public void Filter(IGeometry geom)
            {
                if (geom is LinearRing) 
                {
                    TaggedLineString taggedLine = new TaggedLineString((LineString) geom, 4);
                    _container._lineStringMap.Add(geom, taggedLine);
                }
                else if (geom is LineString) 
                {
                    TaggedLineString taggedLine = new TaggedLineString((LineString) geom, 2);
                    _container._lineStringMap.Add(geom, taggedLine);
                }
            }
        }
    }
}
