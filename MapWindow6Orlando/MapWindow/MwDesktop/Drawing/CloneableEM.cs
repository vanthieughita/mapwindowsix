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
// The Initial Developer of this Original Code is Ted Dunsford. Created 6/2/2009 9:11:30 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;

namespace MapWindow
{


    /// <summary>
    /// CloneableEM
    /// </summary>
    public static class CloneableEM
    {
        /// <summary>
        /// The type parameter T is optional, so the intended use would be like:
        /// ObjectType copy = myObject.Copy();
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="original">The original object</param>
        /// <returns>A new object of the same type as the type being copied.</returns>
        public static T Copy<T>(this T original) where T: class, ICloneable
        {
            if(original != null) return original.Clone() as T;
            return null;
        }



    }
}
