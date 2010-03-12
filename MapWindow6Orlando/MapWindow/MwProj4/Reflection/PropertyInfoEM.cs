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
// The Initial Developer of this Original Code is Ted Dunsford. Created 6/2/2009 3:29:27 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace MapWindow.Projections    
{


    /// <summary>
    /// PropertyInfoEM
    /// </summary>
    public static class ProjPropertyInfoEm
    {
       
        /// <summary>
        /// Gets the first member in the enumerable collection of property info with the specified name.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyInfo GetFirst(this IEnumerable<PropertyInfo> self, string name)
        {
            Func<PropertyInfo, bool> criteria = current => (current.Name == name);
            return self.First(criteria);
        }

        /// <summary>
        /// Determines whether there is a member with the specified name
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<PropertyInfo> self, string name)
        {
            foreach (PropertyInfo info in self)
            {
                if(info.Name == name) return true;
            }
            return false;
        }


    }
}
