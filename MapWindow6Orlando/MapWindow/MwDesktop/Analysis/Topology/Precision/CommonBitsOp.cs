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
namespace MapWindow.Analysis.Topology.Precision
{
    /// <summary> 
    /// Provides versions of Geometry spatial functions which use
    /// common bit removal to reduce the likelihood of robustness problems.
    /// In the current implementation no rounding is performed on the
    /// reshifted result point, which means that it is possible
    /// that the returned Geometry is invalid.
    /// Client classes should check the validity of the returned result themselves.
    /// </summary>
    public class CommonBitsOp
    {
        private bool returnToOriginalPrecision = true;
        private CommonBitsRemover cbr;

        /// <summary>
        /// Creates a new instance of class, which reshifts result <c>Geometry</c>s.
        /// </summary>
        public CommonBitsOp() : this(true) { }

        /// <summary>
        /// Creates a new instance of class, specifying whether
        /// the result <c>Geometry</c>s should be reshifted.
        /// </summary>
        /// <param name="returnToOriginalPrecision"></param>
        public CommonBitsOp(bool returnToOriginalPrecision)
        {
            this.returnToOriginalPrecision = returnToOriginalPrecision;
        }

        /// <summary>
        /// Computes the set-theoretic intersection of two <c>Geometry</c>s, using enhanced precision.
        /// </summary>
        /// <param name="geom0">The first Geometry.</param>
        /// <param name="geom1">The second Geometry.</param>
        /// <returns>The Geometry representing the set-theoretic intersection of the input Geometries.</returns>
        public virtual IGeometry Intersection(IGeometry geom0, IGeometry geom1)
        {
            IGeometry[] geom = RemoveCommonBits(geom0, geom1);
            return ComputeResultPrecision(geom[0].Intersection(geom[1]));
        }

        /// <summary>
        /// Computes the set-theoretic union of two <c>Geometry</c>s, using enhanced precision.
        /// </summary>
        /// <param name="geom0">The first Geometry.</param>
        /// <param name="geom1">The second Geometry.</param>
        /// <returns>The Geometry representing the set-theoretic union of the input Geometries.</returns>
        public virtual IGeometry Union(IGeometry geom0, IGeometry geom1)
        {
            IGeometry[] geom = RemoveCommonBits(geom0, geom1);
            return ComputeResultPrecision(geom[0].Union(geom[1]));
        }

        /// <summary>
        /// Computes the set-theoretic difference of two <c>Geometry</c>s, using enhanced precision.
        /// </summary>
        /// <param name="geom0">The first Geometry.</param>
        /// <param name="geom1">The second Geometry, to be subtracted from the first.</param>
        /// <returns>The Geometry representing the set-theoretic difference of the input Geometries.</returns>
        public virtual IGeometry Difference(IGeometry geom0, IGeometry geom1)
        {
            IGeometry[] geom = RemoveCommonBits(geom0, geom1);
            return ComputeResultPrecision(geom[0].Difference(geom[1]));
        }

        /// <summary
        /// > Computes the set-theoretic symmetric difference of two geometries,
        /// using enhanced precision.
        /// </summary>
        /// <param name="geom0">The first Geometry.</param>
        /// <param name="geom1">The second Geometry.</param>
        /// <returns>The Geometry representing the set-theoretic symmetric difference of the input Geometries.</returns>
        public virtual IGeometry SymDifference(IGeometry geom0, IGeometry geom1)
        {
            IGeometry[] geom = RemoveCommonBits(geom0, geom1);
            return ComputeResultPrecision(geom[0].SymmetricDifference(geom[1]));
        }

        /// <summary>
        /// Computes the buffer a point, using enhanced precision.
        /// </summary>
        /// <param name="geom0">The Geometry to buffer.</param>
        /// <param name="distance">The buffer distance.</param>
        /// <returns>The Geometry representing the buffer of the input Geometry.</returns>
        public virtual IGeometry Buffer(IGeometry geom0, double distance)
        {
            IGeometry geom = RemoveCommonBits(geom0);
            return ComputeResultPrecision((Geometry)geom.Buffer(distance));
        }

        /// <summary>
        /// If required, returning the result to the orginal precision if required.
        /// In this current implementation, no rounding is performed on the
        /// reshifted result point, which means that it is possible
        /// that the returned Geometry is invalid.
        /// </summary>
        /// <param name="result">The result Geometry to modify.</param>
        /// <returns>The result Geometry with the required precision.</returns>
        private IGeometry ComputeResultPrecision(IGeometry result)
        {
            if (returnToOriginalPrecision)
                cbr.AddCommonBits(result);
            return result;
        }

        /// <summary>
        /// Computes a copy of the input <c>Geometry</c> with the calculated common bits
        /// removed from each coordinate.
        /// </summary>
        /// <param name="geom0">The Geometry to remove common bits from.</param>
        /// <returns>A copy of the input Geometry with common bits removed.</returns>
        private IGeometry RemoveCommonBits(IGeometry geom0)
        {
            cbr = new CommonBitsRemover();
            cbr.Add(geom0);
            IGeometry geom = cbr.RemoveCommonBits((Geometry)geom0.Clone());
            return geom;
        }

        /// <summary>
        /// Computes a copy of each input <c>Geometry</c>s with the calculated common bits
        /// removed from each coordinate.
        /// </summary>
        /// <param name="geom0">A Geometry to remove common bits from.</param>
        /// <param name="geom1">A Geometry to remove common bits from.</param>
        /// <returns>
        /// An array containing copies
        /// of the input Geometry's with common bits removed.
        /// </returns>
        private IGeometry[] RemoveCommonBits(IGeometry geom0, IGeometry geom1)
        {
            cbr = new CommonBitsRemover();
            cbr.Add(geom0);
            cbr.Add(geom1);
            IGeometry[] geom = new Geometry[2];
            geom[0] = cbr.RemoveCommonBits((Geometry) geom0.Clone());
            geom[1] = cbr.RemoveCommonBits((Geometry) geom1.Clone());
            return geom;
        }
    }
}
