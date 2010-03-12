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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/25/2009 2:40:04 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************


namespace MapWindow.Drawing
{


    /// <summary>
    /// Add, remove and clear methods don't work on all the categorical sub-filters, but rather only on the 
    /// most immediate.  
    /// </summary>
    public enum FilterTypes
    {
        /// <summary>
        /// Categories
        /// </summary>
        Category,
        /// <summary>
        /// Chunks
        /// </summary>
        Chunk,
        /// <summary>
        /// Selected or unselected
        /// </summary>
        Selection,
        /// <summary>
        /// Visible or not
        /// </summary>
        Visible,


    }
}
