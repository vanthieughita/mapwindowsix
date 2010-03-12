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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/20/2009 3:54:49 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using MapWindow.Main;

namespace MapWindow.Drawing
{


    /// <summary>
    /// This is simply an alias to make things a tad (though not much) more understandable
    /// </summary>
    public class LineCategoryCollection : ChangeEventList<ILineCategory>
    {
        private ILineScheme _scheme;


        /// <summary>
        /// Occurs when a category indicates that its filter expression should be used
        /// to select its members.
        /// </summary>
        public event EventHandler<ExpressionEventArgs> SelectFeatures;


        /// <summary>
        /// Instructs the parent layer to select features matching the specified expression
        /// </summary>
        /// <param name="e">The event args</param>
        /// <param name="sender">The object sender</param>
        protected virtual void OnSelectFeatures(object sender, ExpressionEventArgs e)
        {
            if (SelectFeatures != null) SelectFeatures(sender, e);
        }

        

        /// <summary>
        /// Ensures that newly added categories can navigate to higher legend items.
        /// </summary>
        /// <param name="item">The newly added legend item.</param>
        protected override void OnInclude(ILineCategory item)
        {
            if (_scheme == null) return;
            item.SelectFeatures += OnSelectFeatures;
            item.SetParentItem(_scheme.AppearsInLegend ? _scheme : _scheme.GetParentItem());
            base.OnInclude(item);
        }

        /// <summary>
        /// Overrides the default OnCopy behavior to remove the duplicte SelectFeatures event handler
        /// </summary>
        /// <param name="copy"></param>
        protected override void OnCopy(CopyList<ILineCategory> copy)
        {
            LineCategoryCollection lcc = copy as LineCategoryCollection;
            if (lcc != null && lcc.SelectFeatures != null)
            {
                foreach (var handler in lcc.SelectFeatures.GetInvocationList())
                {
                    lcc.SelectFeatures -= (EventHandler<ExpressionEventArgs>)handler;
                }
            }
            base.OnCopy(copy);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void OnExclude(ILineCategory item)
        {
            if (item == null) return;
            item.SelectFeatures -= OnSelectFeatures;
            item.SetParentItem(null);
            base.OnExclude(item);
        }

        /// <summary>
        /// Gets or sets the parent line scheme
        /// </summary>
        public ILineScheme Scheme
        {
            get { return _scheme; }
            set
            {
                _scheme = value;
                UpdateItemParentPointers();
            }
        }

        /// <summary>
        /// Cycles through all the categories and resets the parent item.
        /// </summary>
        private void UpdateItemParentPointers()
        {
            foreach (ILineCategory item in InnerList)
            {
                if (_scheme == null)
                {
                    item.SetParentItem(null);
                }
                else
                {
                    item.SetParentItem(_scheme.AppearsInLegend ? _scheme : _scheme.GetParentItem());
                }
            }
        }
        
    }
}
