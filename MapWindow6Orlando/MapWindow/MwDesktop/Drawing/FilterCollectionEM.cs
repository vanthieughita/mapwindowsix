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
// The Initial Developer of this Original Code is Ted Dunsford. Created 3/24/2009 8:43:21 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System.Collections.Generic;
using MapWindow.Data;
using MapWindow.Geometries;

namespace MapWindow.Drawing
{


    /// <summary>
    /// FilterCollectionEM
    /// </summary>
    public static class FilterCollectionEM
    {
      
        #region Methods

        /// <summary>
        /// Adds all the features in the specified range.  This will throw an exception if the
        /// the features are not already in the feature list, since this is simply trying 
        /// to select those features.
        /// </summary>
        /// <param name="self">The IFilterCollection to add the range to</param>
        /// <param name="features">The features being selected.</param>
        public static void AddRange(this IFeatureSelection self, IEnumerable<IFeature> features)
        {
            self.SuspendChanges();
            foreach (IFeature f in features)
            {
                self.Add(f);
            }
            self.ResumeChanges();
           
        }

        /// <summary>
        /// This uses extent checking (rather than full polygon intersection checking).  It will add
        /// any members that are either contained by or intersect with the specified region
        /// depending on the SelectionMode property.  The order of operation is the region
        /// acting on the feature, so Contains, for instance, would work with points.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="region"></param>
        /// <returns>True if any item was actually added to the collection</returns>
        public static bool AddRegion(this  IFeatureSelection self, IEnvelope region)
        {
            IEnvelope ignoreMe;
            return self.AddRegion(region, out ignoreMe);
        }

        /// <summary>
        /// Removes the entire list of features
        /// </summary>
        /// <param name="self">The IFilterCollection to remove the range from</param>
        /// <param name="features">The enumerable collection of IFeatures.</param>
        public static void RemoveRange(this  IFeatureSelection self, IEnumerable<IFeature> features)
        {
            self.SuspendChanges();
            foreach (IFeature f in features)
            {
                self.Remove(f);
            }
            self.ResumeChanges();
        }


        /// <summary>
        /// Tests each member currently in the selected features based on 
        /// the SelectionMode.  If it passes, it will remove the feature from
        /// the selection.
        /// </summary>
        /// <param name="self">The IFilterCollection that this should be applied to</param>
        /// <param name="region">The geographic region to remove</param>
        /// <returns>Boolean, true if the collection was changed</returns>
        public static bool RemoveRegion(this  IFeatureSelection self, IEnvelope region)
        {
            IEnvelope ignoreMe;
            return self.RemoveRegion(region, out ignoreMe);
        }


        #endregion

   
    }
}
