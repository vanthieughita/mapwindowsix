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
// The Initial Developer of this Original Code is Ted Dunsford. Created 4/28/2009 11:55:36 AM
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
    /// SpaceTimeSupport
    /// </summary>
    public enum SpaceTimeSupport
    {
        /// <summary>
        /// Spatial (X, Y, Z or M information only)
        /// </summary>
        Spatial,
        /// <summary>
        /// Temporal (time information only)
        /// </summary>
        Temporal,
        /// <summary>
        /// SpatioTemporal (time and space information)
        /// </summary>
        SpatioTemporal,
        /// <summary>
        /// Other (no temporal or spatial information)
        /// </summary>
        Other
    }
}