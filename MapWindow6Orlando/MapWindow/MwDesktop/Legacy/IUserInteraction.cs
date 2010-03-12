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
// The Initial Developer of this Original Code is Ted Dunsford. Created 1/20/2009 11:31:12 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System.Drawing;

namespace MapWindow.Legacy
{


    /// <summary>
    /// UserInteraction
    /// </summary>
    public interface IUserInteraction
    {
      

        #region Methods

        /// <summary>
        ///  Prompt the user to select a projection, and return the PROJ4 representation of this
        ///  projection. Specify the dialog caption and an optional default projection ("" for none).
        /// </summary>
        /// <param name="DialogCaption">The text to be displayed on the dialog, e.g. "Please select a projection."</param>
        /// <param name="DefaultProjection">The PROJ4 projection string of the projection to default to, "" for none.</param>
        /// <returns></returns>
        string GetProjectionFromUser(string DialogCaption, string DefaultProjection);

        /// <summary>
        ///  Retrieve a color ramp, defined by a start and end color, from the user.
        /// </summary>
        /// <param name="suggestedStart">The start color to initialize the dialog with.</param>
        /// <param name="suggestedEnd">The end color to initialize the dialog with.</param>
        /// <param name="selectedEnd">The end color that the user selected.</param>
        /// <param name="selectedStart">The start color that the user selected.</param>
        /// <returns></returns>
        bool GetColorRamp(Color suggestedStart, Color suggestedEnd, out Color selectedStart, out Color selectedEnd);

        #endregion

     

    }
}
