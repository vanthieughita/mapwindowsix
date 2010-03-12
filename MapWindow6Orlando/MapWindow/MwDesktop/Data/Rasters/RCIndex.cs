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
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/17/2008 6:42:19 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************


namespace MapWindow.Data
{


    /// <summary>
    /// A Row, Column indexer for some return types.
    /// </summary>
    public struct RcIndex 
    {
        /// <summary>
        /// The zero based integer row index
        /// </summary>
        public int Row;

        /// <summary>
        /// The zero based integer column index
        /// </summary>
        public int Column;

        /// <summary>
        /// Creates a new RcIndex structure with the specified coordinates
        /// </summary>
        /// <param name="row">The integer row index</param>
        /// <param name="column">The integer column index</param>
        public RcIndex(int row, int column)
        {
            Row = row;
            Column = column;
        }
      
        /// <summary>
        /// Gets a boolean that is true if either row or column index has no value
        /// </summary>
        /// <returns>Boolean, true if either row or column has no value</returns>
        public bool IsEmpty()
        {
            return (Row == int.MinValue && Column == int.MinValue);
        }

        /// <summary>
        /// Returns a new RcIndex that is defined as empty when both indices are int.
        /// </summary>
        public static RcIndex Empty
        {
            get { return new RcIndex(int.MinValue, int.MinValue); }       
        }
       
    }
}
