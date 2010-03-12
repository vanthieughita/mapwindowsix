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
// The Initial Developer of this Original Code is Ted Dunsford. Created 3/17/2009 9:05:41 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************


using System.Collections.Generic;

namespace MapWindow.Data
{


    /// <summary>
    /// RangeIndex
    /// </summary>
    public class VertexRange: IEnumerable<Vertex>
    {
        // Even though it is normally taboo to allow for public fields, for performance they will be
        // much faster than working through property accessors.

        private int _shapeOffset;
        private int _partOffset;


        /// <summary>
        /// For parts, controlling the part offset is perhaps more useful that controlling the shape offset.
        /// </summary>
        public int PartOffset
        {
            get { return _partOffset; }
            set { _partOffset = value; }
        }

        /// <summary>
        /// The StartIndex is the sum of the shape offset and the part offset.  Controlling them separately
        /// allows the entire shape offset to be adjusted independantly after the part is created.
        /// </summary>
        public int ShapeOffset
        {
            get { return _shapeOffset; }
            set { _shapeOffset = value; }
        }


        /// <summary>
        /// The integer index of the first vertex included in this range.  This is overridden 
        /// in the case of part ranges to also take into account the shape start index.
        /// </summary>
        public int StartIndex
        {
            get { return _shapeOffset + _partOffset; }
        }

        

        /// <summary>
        /// the integer index of the last vertex included in this range
        /// </summary>
        public int EndIndex
        {
            get { return StartIndex + _numVertices -1; }
        }

       
     
       

        #region Enumerator

        /// <summary>
        /// The enumerator is here to provide an easy method for cycling vertex values
        /// in each range.  This sort of defeats the point because it adds
        /// two method calls for advancing each step (one to MoveNext and one to
        /// access the property.  The whole point of loading everything
        /// into a single array of vertices in the first place is to avoid
        /// property accessors slowing down the process.  However, it's here
        /// if someone wants it.
        /// </summary>
        public class VertexRangeEnumerator : IEnumerator<Vertex> 
        {
            private readonly double[] _vertices;
            private readonly int _start;
            private readonly int _end;
            private Vertex _current;
            private int _index;
            /// <summary>
            /// Creates a new instance of the VertexRangeEnumerator
            /// </summary>
            /// <param name="vertices">The vertices to create</param>
            /// <param name="start">The integer index of the first included vertex </param>
            /// <param name="end">The integer index of the last included vertex</param>
            public VertexRangeEnumerator(double[] vertices, int start, int end)
            {
                _start = start;
                _end = end;
                _index = start - 1;
                _vertices = vertices;
            }

            #region IEnumerator<Vertex> Members

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public Vertex Current
            {
                get { return _current; }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            #endregion

            #region IDisposable Members
            /// <summary>
            /// This does nothing
            /// </summary>
            public void Dispose()
            {
               
            }

            #endregion

            #region IEnumerator Members

            
            /// <summary>
            /// Advances the enumerator to the next position
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _index++;
                if (_index > _end) return false;
                _current = new Vertex(_vertices[_index * 2], _vertices[_index * 2 + 1]);
                return true;
            }

            /// <summary>
            /// Resets this enumerator to the beginning of the range of vertices
            /// </summary>
            public void Reset()
            {
                _index = _start -1;
            }

            #endregion
        }

        #endregion


        #region Private Variables

        // Internally keep track of the vertices array.
        private double[] _vertices;
        private int _numVertices;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty vertex range that can have the vertices and number of vertices assigned later
        /// </summary>
        public VertexRange()
        {
            
        }

        /// <summary>
        /// Creates a new instance of RangeIndex
        /// </summary>
        /// <param name="allVertices">An array of all the vertex locations</param>
        public VertexRange(double[] allVertices, int shapeOffset, int partOffset)
        {
            _shapeOffset = shapeOffset;
            _partOffset = partOffset;
            _vertices = allVertices;
            
        }

        #endregion

     
        /// <summary>
        /// Gets an enumerator.  This exists to make it easier to cycle values,
        /// but in truth should not be used because it re-adds the property accessor
        /// which slows down the data access, which is the whole point of putting
        /// all the vertices into a jagged array of doubles in the first place.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Vertex> GetEnumerator()
        {
            return new VertexRangeEnumerator(_vertices, StartIndex, EndIndex);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the number of vertices.  This will also set the EndIndex
        /// relative to the start position.
        /// </summary>
        public int NumVertices
        {
            get { return _numVertices; }
            set
            {
                _numVertices = value;
                
            }
        }

        /// <summary>
        /// Gets or sets the vertices
        /// </summary>
        public double[] Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }
        

     
    }
}
