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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/23/2008 8:22:45 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MapWindow;
using MapWindow.Main;
using MapWindow.Data;
using MapWindow.Drawing;
using MapWindow.Geometries;

namespace MapWindow.Data
{


    /// <summary>
    /// RasterFileTypes
    /// </summary>
    public enum RasterFileTypes
    {
        /// <summary>
        /// Ascii
        /// </summary>
        ASCII,
        /// <summary>
        /// Binary interlaced Layers
        /// </summary>
        BIL,
        /// <summary>
        /// BGD (Original MapWindow format)
        /// </summary>
        BINARY,
        /// <summary>
        /// DTED
        /// </summary>
        DTED,
        /// <summary>
        /// Wavelet format
        /// </summary>
        ECW,
        /// <summary>
        /// ArcGIS format
        /// </summary>
        ESRI,
        /// <summary>
        /// FLT
        /// </summary>
        FLT,
        /// <summary>
        /// GeoTiff
        /// </summary>
        GeoTiff,
        /// <summary>
        /// SID
        /// </summary>
        MrSID,
        /// <summary>
        /// AUX
        /// </summary>
        PAUX,
        /// <summary>
        /// PCIDsk
        /// </summary>
        PCIDsk,
        /// <summary>
        /// SDTS
        /// </summary>
        SDTS,
        /// <summary>
        /// Custom - specified as string
        /// </summary>
        CUSTOM

    }
}
