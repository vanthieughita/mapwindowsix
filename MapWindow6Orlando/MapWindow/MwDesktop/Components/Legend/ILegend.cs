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
// The Initial Developer of this Original Code is Ted Dunsford. Created in August, 2007.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Collections.Generic;
using MapWindow.Drawing;
namespace MapWindow.Components
{
    /// <summary>
    /// The easiest way to implement this is to inherit a UserControl, and then
    /// implement the members specific to the legend.
    /// </summary>
    public interface ILegend
    {

        #region Events

        /// <summary>
        /// Occurs when the drag method is used to alter the order of layers or
        /// groups in the legend.
        /// </summary>
        event EventHandler OrderChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Given the current list of Maps or 3DMaps, it
        /// rebuilds the treeview nodes
        /// </summary>
        void RefreshNodes();



        #endregion

        #region Properties

        /// <summary>
        /// Adds a map frame as a root node, and links an event handler to update
        /// when the mapframe triggers an ItemChanged event.
        /// </summary>
        /// <param name="mapFrame"></param>
        void AddMapFrame(IFrame mapFrame);


        /// <summary>
        /// Removes the specified map frame if it is a root node.
        /// </summary>
        /// <param name="mapFrame"></param>
        /// <param name="preventRefresh">Boolean, if true, removing the map frame will not automatically force a refresh of the legend.</param>
        void RemoveMapFrame(IFrame mapFrame, bool preventRefresh);

        /// <summary>
        /// Gets or sets the list of map frames being displayed by this legend.
        /// </summary>
        List<ILegendItem> RootNodes
        {
            get;
            set;
        }

        #endregion

    }
}
