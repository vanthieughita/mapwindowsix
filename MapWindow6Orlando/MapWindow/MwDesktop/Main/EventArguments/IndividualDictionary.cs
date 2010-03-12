//********************************************************************************************************
// Product Name: MapWindow.Interfaces Alpha
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
    /// Carries event arguments for the generic IEventList
    /// </summary>
    public class IndividualDictionary<TKey, TValue> : System.EventArgs
    {
       
        private TValue _value;
        private TKey _key;
        
      

        #region Methods

        /// <summary>
        /// Creates a new instance of a ListEventArgs class
        /// </summary>
        /// <param name="inKey">The key of type &lt;TKey&gt; being referenced</param>
        /// <param name="inValue">an object of type &lt;TValue&gt; that is being referenced</param>
        public IndividualDictionary(TKey inKey, TValue inValue)
        {
            _value = inValue;
            _key = inKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the key of the item being referenced by this event
        /// </summary>
        public TKey Key
        {
            get { return _key; }
            protected set { _key = value; }
        }

        /// <summary>
        /// Gets the actual item being referenced by this event
        /// </summary>
        public TValue Value
        {
            get { return _value; }
            protected set  { _value = value; }
        }

       
        #endregion

    }
}
