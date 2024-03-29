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
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/13/2008 2:01:06 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Windows.Forms;
namespace MapWindow.Components
{


    /// <summary>
    /// SelectEventArgs
    /// </summary>
    public class SelectEventArgs: EventArgs
    {
        #region Private Variables

        private bool _isSelected;
        private Keys _modifiers;


        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of SelectEventArgs
        /// </summary>
        public SelectEventArgs(bool isSelected)
        {
            _isSelected = isSelected;
            _modifiers = System.Windows.Forms.Control.ModifierKeys;
        }

        #endregion

    

        #region Properties

        /// <summary>
        /// Gets whether or not the new state of the item is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            protected set { _isSelected = value; }
        }

        /// <summary>
        /// Gets the modifiers that existed when this event is fired.
        /// use like if(Modifiers and Keys.Shift){}
        /// </summary>
        public Keys Modifiers
        {
            get { return _modifiers; }
            protected set { _modifiers = value; }
        }

        #endregion



    }
}
