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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/23/2008 9:25:23 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using MapWindow.Data;
using MapWindow.Main;
using MapWindow.Geometries;

namespace MapWindow.Plugins
{


    /// <summary>
    /// IFeatureProvider
    /// </summary>
    public interface VectorProvider : IDataProvider
    {
        /// <summary>
        /// This create new method implies that this provider has the priority for creating a new file.
        /// An instance of the dataset should be created and then returned.  By this time, the filename
        /// will already be checked to see if it exists, and deleted if the user wants to overwrite it.
        /// </summary>
        /// <param name="filename">The string filename for the new instance</param>
        /// <param name="featureType">Point, Line, Polygon etc.  Sometimes this will be specified, sometimes it will be "Unspecified"</param>
        /// <param name="inRam">Boolean, true if the dataset should attempt to store data entirely in ram</param>
        /// <param name="progressHandler">An IProgressHandler for status messages.</param>
        /// <returns>An IRaster</returns>
        IFeatureSet CreateNew(string filename, FeatureTypes featureType, bool inRam, IProgressHandler progressHandler);


        /// <summary>
        /// This tests the specified file in order to determine what type of vector the file contains.
        /// This returns unspecified if the file format is not supported by this provider.
        /// </summary>
        /// <param name="filename">The string filename to test</param>
        /// <returns>A FeatureType clarifying what sort of features are stored on the data type.</returns>
        FeatureTypes GetFeatureType(string filename);
    }
}
