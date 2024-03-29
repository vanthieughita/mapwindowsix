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
// The Initial Developer of this Original Code is Ted Dunsford. Created 9/14/2009 8:50:58 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************


using System;
using System.Drawing;
using MapWindow.Main;
using MapWindow.Serialization;

namespace MapWindow.Drawing
{


    /// <summary>
    /// EditorSettings
    /// </summary>
    [Serializable]
    public class EditorSettings : Descriptor
    {
        #region Private Variables

        private int _numBreaks;
        private bool _useColor;
        private bool _hsl;
        private Color _startColor;
        private Color _endColor;
        private int _hueShift;
        private IntervalMethods _intervalMethod;
        private int _maxSampleCount;
        private bool _rampColors;
        private IntervalSnapMethods _intervalSnapMethod;
        private int _intervalRoundingDigits;
        private string _excludeExpression;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of EditorSettings
        /// </summary>
        public EditorSettings()
        {
            _hsl = true;
            _useColor = true;
            _startColor = Global.ColorFromHSL(5, .7, .7);
            _endColor = Global.ColorFromHSL(345, .8, .8);
            _maxSampleCount = 10000;
            _intervalMethod = IntervalMethods.EqualInterval;
            _rampColors = true;
            _intervalSnapMethod = IntervalSnapMethods.Rounding;
            _intervalRoundingDigits = 0;
            _numBreaks = 5;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties




        /// <summary>
        /// Gets or sets the characteristics of the "right" color.
        /// </summary>
        [Serialize("EndColor")]
        public Color EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        /// <summary>
        /// Gets or sets a string that allows the user to use any of the
        /// data fields to eliminate values from being considered as part
        /// of the histogram for statistical interval calculations.
        /// </summary>
        [Serialize("ExcludeExpression")]
        public string ExcludeExpression
        {
            get { return _excludeExpression; }
            set { _excludeExpression = value; }
        }


        /// <summary>
        /// Gets or sets a boolean indicating to display
        /// the hue, saturation, lightness as bounds
        /// instead of start-color, end-color.
        /// </summary>
        [Serialize("HueSatLight")]
        public bool HueSatLight
        {
            get { return _hsl; }
            set { _hsl = value; }
        }

      

        /// <summary>
        /// Gets or sets the hue shift.
        /// </summary>
        [Serialize("HueShift")]
        public int HueShift
        {
            get { return _hueShift; }
            set { _hueShift = value; }
        }


        /// <summary>
        /// Gets or sets the interval method
        /// </summary>
        [Serialize("IntervalMethod")]
        public IntervalMethods IntervalMethod
        {
            get { return _intervalMethod; }
            set { _intervalMethod = value; }
        }


        /// <summary>
        /// Gets or sets the maximum sample count.
        /// </summary>
        [Serialize("MaxSampleCount")]
        public int MaxSampleCount
        {
            get { return _maxSampleCount; }
            set { _maxSampleCount = value; }
        }

        /// <summary>
        /// Gets or sets the integer count if equal breaks are used
        /// </summary>
        [Serialize("NumBreaks")]
        public int NumBreaks
        {
            get { return _numBreaks; }
            set { _numBreaks = value; }
        }

        /// <summary>
        /// Gets or sets whether this editor should ramp the colors, 
        /// or use randomly generated colors.  The default is random.
        /// </summary>
        [Serialize("RampColors")]
        public bool RampColors
        {
            get { return _rampColors; }
            set { _rampColors = value; }
        }
       

        /// <summary>
        /// Gets or sets the characteristics of the "left" color.
        /// </summary>
        [Serialize("StartColor")]
        public Color StartColor
        {
            get { return _startColor; }
            set { _startColor = value; }
        }
       
        /// <summary>
        /// Gets or sets whether to use the color specifications
        /// </summary>
        [Serialize("UseColorRange")]
        public bool UseColorRange
        {
            get { return _useColor; }
            set { _useColor = value; }
        }

        /// <summary>
        /// Gets or sets how intervals like equal breaks choose the
        /// actual values, and whether they are rounded or snapped.
        /// </summary>
        [Serialize("IntervalSnapMethod")]
        public IntervalSnapMethods IntervalSnapMethod
        {
            get { return _intervalSnapMethod; }
            set { _intervalSnapMethod = value; }
        }

        /// <summary>
        /// Gets or sets the number of digits to preserve when IntervalSnapMethod is set to Rounding
        /// </summary>
        [Serialize("IntervalRoundingDigits")]
        public int IntervalRoundingDigits
        {
            get { return _intervalRoundingDigits; }
            set { _intervalRoundingDigits = value; }
        }
       

        #endregion



    }
}
