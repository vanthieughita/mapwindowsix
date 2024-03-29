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
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/24/2009 12:34:51 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;

namespace MapWindow.Main
{


    /// <summary>
    /// IChangeEvent
    /// </summary>
    public interface ISuspendEvents:  ICloneable
    {
       
        /// <summary>
        /// Resumes event sending and fires a ListChanged event if any changes have taken place.
        /// This will not track all the individual changes that may have fired in the meantime.
        /// </summary>
        void ResumeEvents();
       
        /// <summary>
        /// Temporarilly suspends notice events, allowing a large number of changes.
        /// </summary>
        void SuspendEvents();
        

        /// <summary>
        /// Gets whether or not the list is currently suspended
        /// </summary>
        bool EventsSuspended
        {
            get;
        }

    }
}
