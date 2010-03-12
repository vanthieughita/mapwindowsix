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
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/17/2008 8:35:34 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using MapWindow.Data;
using MapWindow.Geometries;
using MapWindow.Main;
using MapWindow.Serialization;

namespace MapWindow.Drawing
{


    /// <summary>
    /// LabelLayer
    /// </summary>
    public class LabelLayer : Layer, ILabelLayer
    {
        #region Events

        /// <summary>
        /// Occurs after the selection has been cleared
        /// </summary>
        public event EventHandler<FeatureChangeArgs> SelectionCleared;

        /// <summary>
        /// Occurs after the selection is updated by the addition of new members
        /// </summary>
        public event EventHandler<FeatureChangeEnvelopeArgs> SelectionExtended;


        #endregion

        #region Private Variables

        private IFeatureSet _featureSet;
        private IFeatureLayer _featureLayer;
        private IList<ILabel> _labels;
        [Serialize("Symbology")]
        private ILabelScheme _symbology;
        private Dictionary<IFeature, LabelDrawState> _drawnStates;
        private FastLabelDrawnState[] _fastDrawnStates;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of LabelLayer
        /// </summary>
        public LabelLayer()
        {
            Configure();
        }

        /// <summary>
        /// Creates a new layer that uses the attributes from the given featureSet
        /// </summary>
        /// <param name="inFeatureSet"></param>
        public LabelLayer(IFeatureSet inFeatureSet)
        {
            _featureSet = inFeatureSet;
            Configure();
        }

        /// <summary>
        /// Creates a new label layer based on the features in the 
        /// </summary>
        /// <param name="inFeatureLayer"></param>
        public LabelLayer(IFeatureLayer inFeatureLayer)
        {
            _featureSet = inFeatureLayer.DataSet;
            _featureLayer = inFeatureLayer;
            Configure();
        }

        private void Configure()
        {
            if (_featureSet != null) Envelope = _featureSet.Envelope;
            _symbology = new LabelScheme();

        }


        #endregion

        #region Methods

        /// <summary>
        /// Clears the current selection, reverting the geometries back to their
        /// normal colors.
        /// </summary>
        public void ClearSelection()
        {
           
        }

      
        /// <summary>
        /// This builds the _drawnStates based on the current label scheme.
        /// </summary>
        public virtual void CreateLabels()
        {
            if(_featureSet != null)
            {
                if (_featureSet.IndexMode)
                {
                    CreateIndexedLabels();
                    return;
                }
            }
            _drawnStates = new Dictionary<IFeature, LabelDrawState>();
            if (_featureSet == null) return;
            DataTable dt = _featureSet.DataTable; // if working correctly, this should auto-populate
            if (Symbology == null) return;
           
            foreach (ILabelCategory category in Symbology.Categories)
            {
                List<IFeature> features;
                if (category.FilterExpression != null)
                {
                    features = FeatureSet.SelectByAttribute(category.FilterExpression);
                }
                else
                {
                    features = FeatureSet.Features.ToList();
                }

                foreach (IFeature feature in features)
                {
                    if(_drawnStates.ContainsKey(feature))
                    {
                        _drawnStates[feature] = new LabelDrawState(category);
                    }
                    else
                    {
                        _drawnStates.Add(feature, new LabelDrawState(category));
                    }
                    if(!_drawnStates.ContainsKey(feature))
                    {
                        bool keyfailed = true;
                    }
                }

            }

        }

        /// <summary>
        /// This builds the _drawnStates based on the current label scheme.
        /// </summary>
        protected void CreateIndexedLabels()
        {
            if (_featureSet == null) return;
            _fastDrawnStates = new FastLabelDrawnState[_featureSet.ShapeIndices.Count];
            
            //DataTable dt = _featureSet.DataTable; // if working correctly, this should auto-populate
            if (Symbology == null) return;

            foreach (ILabelCategory category in Symbology.Categories)
            {
                if (category.FilterExpression != null)
                {
                    List<int> features = FeatureSet.SelectIndexByAttribute(category.FilterExpression);
                    foreach (int feature in features)
                    {
                        _fastDrawnStates[feature] = new FastLabelDrawnState(category);
                    }
                }
                else
                {
                    for(int i = 0; i < _fastDrawnStates.Length; i++)
                    {
                        _fastDrawnStates[i] = new FastLabelDrawnState(category);
                    }
                }
            }
        }




        /// <summary>
        /// Highlights the values from a specified region.  This will not unselect any members,
        /// so if you want to select a new region instead of an old one, first use ClearSelection.
        /// This is the default selection that only tests the anchorpoint, not the entire label.
        /// </summary>
        /// <param name="region">An IEnvelope showing a 3D selection box for intersection testing.</param>
        /// <returns>True if any members were added to the current selection.</returns>
        public bool Select(IEnvelope region)
        {
            List<IFeature> features = FeatureSet.Select(region);
            if (features.Count == 0) return false;
            foreach (IFeature feature in features)
            {
                _drawnStates[feature].Selected = true;
            }
            return true;
        }

        /// <summary>
        /// Removes the features in the given region 
        /// </summary>
        /// <param name="region">the geographic region to remove the feature from the selection on this layer</param>
        /// <returns>Boolean true if any features were removed from the selection.</returns>
        public bool UnSelect(IEnvelope region)
        {
            List<IFeature> features = FeatureSet.Select(region);
            if (features.Count == 0) return false;
            foreach (IFeature feature in features)
            {
                _drawnStates[feature].Selected = false;
            }
            return true;
        }


        #endregion

        #region Properties

       

        /// <summary>
        /// Gets or sets the dictionary that quickly identifies the category for
        /// each label.
        /// </summary>
        [ShallowCopy]
        public Dictionary<IFeature, LabelDrawState> DrawnStates
        {
            get { return _drawnStates; }
            set 
            { 
                _drawnStates = value;
            }
        }

        /// <summary>
        /// Gets or sets the indexed collection of drawn states
        /// </summary>
        public FastLabelDrawnState[] FastDrawnStates
        {
            get { return _fastDrawnStates; }
            set { _fastDrawnStates = value; }
        }

        
       
        /// <summary>
        /// Gets or sets the featureSet that defines the text for the labels on this layer.
        /// </summary>
        [ShallowCopy]
        public IFeatureSet FeatureSet
        {
            get { return _featureSet; }
            set { _featureSet = value; }
        }

        /// <summary>
        /// Gets or sets an optional layer to link this layer to.  If this is specified, then drawing will
        /// be associated with this layer.  This also updates the FeatureSet property.
        /// </summary>
        [ShallowCopy]
        public IFeatureLayer FeatureLayer
        {
            get { return _featureLayer; }
            set
            {
                _featureLayer = value;
                _featureSet = _featureLayer.DataSet;
            }
        }

        /// <summary>
        /// Gets or sets a valid IList of ILabels.  This can just be a List of labels, but allows for
        /// custom list development later.
        /// </summary>
        public IList<ILabel> Labels
        {
            get { return _labels; }
            set { _labels = value; }
        }

        /// <summary>
        /// Gets or sets the labeling scheme as a collection of categories.
        /// </summary>
        public ILabelScheme Symbology
        {
            get { return _symbology; }
            set 
            { 
                _symbology = value;
                CreateLabels(); // update the drawn state with the new categories
               
            }
        }


       
        /// <summary>
        /// Gets or sets the selection symbolizer from the first TextSymbol group.
        /// </summary>
        [ShallowCopy]
        public ILabelSymbolizer SelectionSymbolizer
        {
            get 
            {
                if (_symbology == null) return null;
                if (_symbology.Categories == null) return null;
                if (_symbology.Categories.Count == 0) return null;
                return _symbology.Categories[0].SelectionSymbolizer; 
            }
            set
            {
                if (_symbology == null) _symbology = new LabelScheme();
                if (_symbology.Categories == null) _symbology.Categories = new BaseList<ILabelCategory>();
                if (_symbology.Categories.Count == 0) _symbology.Categories.Add(new LabelCategory());
                _symbology.Categories[0].SelectionSymbolizer = value;
            }
        }

        /// <summary>
        /// Gets or sets the regular symbolizer from the first TextSymbol group.
        /// </summary>
        [ShallowCopy]
        public ILabelSymbolizer Symbolizer
        {
            get
            {
                if (_symbology == null) return null;
                if (_symbology.Categories == null) return null;
                if (_symbology.Categories.Count == 0) return null;
                return _symbology.Categories[0].Symbolizer;
            }
            set
            {
                if (_symbology == null) _symbology = new LabelScheme();
                if (_symbology.Categories == null) _symbology.Categories = new BaseList<ILabelCategory>();
                if (_symbology.Categories.Count == 0) _symbology.Categories.Add(new LabelCategory());
                _symbology.Categories[0].Symbolizer = value;
            }
        }
        

        #endregion

        #region Protected Methods

        

        /// <summary>
        /// Fires the selection cleared event
        /// </summary>
        protected virtual void OnSelectionCleared(FeatureChangeArgs args)
        {
            if (SelectionCleared != null) SelectionCleared(this, args); 
        }


        /// <summary>
        /// Fires the selection extended event
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnSelectionExtended(FeatureChangeEnvelopeArgs args)
        {
            if (SelectionExtended != null) SelectionExtended(this, args);
        }

        

        #endregion

    }
}
