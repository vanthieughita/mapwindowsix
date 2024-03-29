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
// The Initial Developer of this Original Code is Ted Dunsford. Created in 2008 (probably for the second time).
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
namespace MapWindow.Drawing
{
    /// <summary>
    /// A layer with drawing characteristics for LineStrings
    /// </summary>
    public interface ILineLayer: IFeatureLayer
    {

       
      
        #region Methods

       

     
       
        #endregion  

        #region Properties
 
     

        /// <summary>
        /// Gets or sets the drawing characteristics to use for this layer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Unable to assign a non-point symbolizer to a PointLayer</exception>
        new ILineSymbolizer Symbolizer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LineSymbolizer to use for selected properties.
        /// </summary>
        new ILineSymbolizer SelectionSymbolizer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the categorical scheme for categorical drawing.
        /// </summary>
        new ILineScheme Symbology
        {
            get;
            set;
        }


      
        #endregion



      
      
  

    }
}
