﻿//********************************************************************************************************
// Product Name: MapWindow.Tools.ITool
// Description:  Interface for tools for the MapWindow toolbox
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
// The Original Code is Toolbox.dll for the MapWindow 4.6/6 ToolManager project
//
// The Initial Developer of this Original Code is Brian Marchionni. Created in Oct, 2008.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MapWindow.Tools.Param;

namespace MapWindow.Tools
{
    /// <summary>
    /// Defines the way a tool interfaces with the toolbox
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// A UniqueName Identifying this Tool, if another tool with the same UniqueName exists this tool will not be loaded
        /// </summary>
        string UniqueName
        {
            get;
        }
        
        /// <summary>
        /// Returns the name of the tool
        /// </summary>
        string Name
        { 
            get;
        }

        /// <summary>
        /// Returns the category of tool that the ITool should be added to
        /// </summary>
        string Category
        {
            get;
        }

        /// <summary>
        /// Returns the Version of the tool
        /// </summary>
        Version Version
        {
            get;
        }

        /// <summary>
        /// Returns the Author of the tool's name
        /// </summary>
        string Author
        {
            get;
        }

        /// <summary>
        /// The parameters array should be populated with default values here
        /// </summary>
        /// <returns></returns>
        void Initialize();

        /// <summary>
        /// Returns a description of what the tool does for inclusion in the help section of the toolbox list
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Returns a brief description displayed when the user hovers over the tool in the toolbox
        /// </summary>
        string ToolTip
        {
            get;
        }
        
        /// <summary>
        /// Fires when one of the paramters value has beend changed, usually when a user changes a input or output parameters value, this can be used to populate other parameters default values.
        /// </summary>
        void ParameterChanged(Parameter sender);
        
        /// <summary>
        /// Gets or Sets the input paramater array
        /// </summary>
        Parameter[] InputParameters
        {
            get;
        }

        /// <summary>
        /// Gets or Sets the output paramater array
        /// </summary>
        Parameter[] OutputParameters
        {
            get;
        }
        
        /// <summary>
        /// Once the parameters have been configured the Execute command can be called, it returns true if succesful
        /// </summary>
        /// <param name="cancelProgressHandler">A cancel progress handler that used to indicate how much of the tool is done</param>
        /// <returns></returns>
        bool Execute(MapWindow.Tools.ICancelProgressHandler cancelProgressHandler);

        /// <summary>
        /// 32x32 Bitmap - The Large icon that will appears in the Tool Dialog Next to the tools name
        /// </summary>
        System.Drawing.Bitmap Icon
        {
            get;
        }

        /// <summary>
        /// Image displayed in the help area when no input field is selected
        /// </summary>
        System.Drawing.Bitmap HelpImage
        {
            get;
        }

        /// <summary>
        /// Help text to be displayed when no input field is selected
        /// </summary>
        String HelpText
        {
            get;
        }

        /// <summary>
        /// This is set before the tool is executed to provide a folder where the tool can save temporary data
        /// </summary>
        string WorkingPath
        {
            set;
        }

        /// <summary>
        /// Returns the address of the tools help web page in HTTP://... format. Return a empty string to hide the help hyperlink.
        /// </summary>
        string HelpURL
        {
            get;
        }
    }
}
