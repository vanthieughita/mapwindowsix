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
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/18/2008 10:04:28 AM
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

namespace MapWindow.Drawing
{


    /// <summary>
    /// MemberChangedEventArgs
    /// </summary>
    public class TextSymbolChangedEventArgs: EventArgs
    {
        #region Private Variables

        private ILabel _label;
        private ILabelSymbolizer _oldSymbolizer;
        private ILabelSymbolizer _newSymbolizer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of MemberChangedEventArgs
        /// </summary>
        public TextSymbolChangedEventArgs(ILabel label, ILabelSymbolizer oldSymbolizer, ILabelSymbolizer newSymbolizer)
        {
            _label = label;
            _oldSymbolizer = oldSymbolizer;
            _newSymbolizer = newSymbolizer;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the Label that was updated
        /// </summary>
        public virtual ILabel Label
        {
            get { return _label; }
            protected set { _label = value; }
        }

        /// <summary>
        /// Gets the previous symbolizer that this label no longer uses.
        /// </summary>
        public virtual ILabelSymbolizer OldSymbolizer
        {
            get { return _oldSymbolizer; }
            protected set { _oldSymbolizer = value; }
        }

        /// <summary>
        /// Gets the new symbolizer that this label now uses.
        /// </summary>
        public virtual ILabelSymbolizer NewSymbolizer
        {
            get { return _newSymbolizer; }
            set { _newSymbolizer = value; }
        }


        #endregion



    }
}
