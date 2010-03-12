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
using System.Text;

namespace MapWindow.Main
{
    /// <summary>
    /// The easiest way to implement this is to inherit System.ComponentModel.Component
    /// </summary>
    public interface IComponent : IMarshalByRefObject, System.ComponentModel.IComponent
    {
        
        /// <summary>
        /// Gets the System.CompnoentModel.IContainer that contains the System.ComponentModel.Component
        /// </summary>
        /// <returns>The System.ComponentModel.IContainer that contains the System.ComponentModel.Component,
        /// if any, or null if the System.ComponentModel.Component is not encapsulated in an System.ComponentModel.IContainer.
        /// </returns>
        System.ComponentModel.IContainer Container
        {
            get;
        }

       
    }
}
