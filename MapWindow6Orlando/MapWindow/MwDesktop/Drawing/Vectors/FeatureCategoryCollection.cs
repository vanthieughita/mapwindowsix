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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/20/2009 3:56:16 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using MapWindow.Main;
namespace MapWindow.Drawing
{


    /// <summary>
    /// FeatureSchemeCategoryCollection
    /// </summary>
    public class FeatureCategoryCollection : ChangeEventList<IFeatureCategory>
    {


        private IFeatureScheme _scheme;

        /// <summary>
        /// Creates a new instance of the FeatureSchemeCategoryCollection
        /// </summary>
        public FeatureCategoryCollection()
        {
            Add(new FeatureCategory()); // default grouping
        }

        /// <summary>
        /// Occurs when including legend items
        /// </summary>
        /// <param name="item"></param>
        protected override void OnInclude(IFeatureCategory item)
        {
  
            item.SetParentItem(_scheme.AppearsInLegend ? _scheme : _scheme.GetParentItem());
            base.OnInclude(item);
        }

        /// <summary>
        /// Occurs when excluding legend items
        /// </summary>
        /// <param name="item"></param>
        protected override void OnExclude(IFeatureCategory item)
        {
            item.SetParentItem(null);
        }

        /// <summary>
        /// Optionally allows the scheme to identify itself for future reference.
        /// </summary>
        public IFeatureScheme Scheme
        {
            get { return _scheme; }
            set { _scheme = value; }
        }

    }
}
