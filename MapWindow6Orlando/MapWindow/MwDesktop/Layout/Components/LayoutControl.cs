﻿//********************************************************************************************************
// Product Name: MapWindow.Layout
// Description:  The MapWindow LayoutControl used to setup printing layouts
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
// The Original Code is MapWindow.dll Version 6.0
//
// The Initial Developer of this Original Code is Brian Marchionni. Created in Jul, 2009.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using MapWindow.Layout.Elements;
using MapWindow.Forms;

namespace MapWindow.Layout
{
    ///<summary>
    /// The actual control controling the layout
    ///</summary>
    public class LayoutControl : UserControl
    {
        #region ---------------- Class Variables

        //Document Variables
        private string _filename;

        //Variables used to setup the print area and quality
        private SmoothingMode _drawingQuality;
        private bool _showMargin;

        //Variables used for drawing
        private PointF _paperLocation;      //The location of the paper within the screen coordinants
        private float _zoom;                //The zoom of the paper

        //Variables used for selection and addition
        private RectangleF _mouseBox;

        //Variables used for resize selection
        private Edge _resizeSelectedEdge;
        private Bitmap _resizeTempBitmap;

        //Tracks mouse movement
        private PointF _mouseStartPoint;
        private PointF _lastMousePoint;
        private MouseMode _mouseMode;
        private bool _suppressLEinvalidate;

        //Variables used for the scroll bar
        private HScrollBar _hScrollBar;
        private VScrollBar _vScrollBar;
        private Panel _hScrollBarPanel;

        //The list of all the layout elements currently loaded (Item 0 is drawn last)
        private readonly List<LayoutElement> _layoutElements = new List<LayoutElement>();
        private readonly List<LayoutElement> _selectedLayoutElements = new List<LayoutElement>();
        private LayoutElement _elementToAddWithMouse;
       
        //Controls associated with this
        private LayoutMenuStrip _layoutMenuStrip;
        private LayoutPropertyGrid _layoutPropertyGrip;
        private LayoutZoomToolStrip _layoutZoomToolStrip;
        private LayoutListBox _layoutListBox;
        private LayoutDocToolStrip _layoutDocToolStrip;
        private LayoutInsertToolStrip _layoutInsertToolStrip;
        private LayoutMapToolStrip _layoutMapToolStrip;
        private ContextMenu _contextMenuRight;
        private MenuItem _cMnuMoveUp;

        //Map
        private Map.Map _mapControl;

        //Right click context menu variables;
        private MenuItem _cMnuMoveDown;
        private MenuItem _menuItem2;
        private MenuItem _cMnuSelAli;
        private MenuItem _cMnuMarAli;
        private MenuItem _menuItem4;
        private MenuItem _cMnuDelete;
        private MenuItem _cMnuSelLeft;
        private MenuItem _cMnuSelRight;
        private MenuItem _cMnuSelTop;
        private MenuItem _cMnuSelBottom;
        private MenuItem _cMnuSelHor;
        private MenuItem _cMnuSelVert;
        private MenuItem _cMnuMargLeft;
        private MenuItem _cMnuMargRight;
        private MenuItem _cMnuMargTop;
        private MenuItem _cMnuMargBottom;
        private MenuItem _cMnuMargHor;
        private MenuItem _cMnuMargVert;
        private MenuItem _menuItem19;
        private MenuItem _cMnuSelFit;
        private MenuItem _cMnuMarFit;
        private MenuItem _cMnuSelWidth;
        private MenuItem _cMnuSelHeight;
        private MenuItem _cMnuMargWidth;
        private MenuItem _cMnuMargHeight;

        //Printer Variables
        private PrinterSettings _printerSettings = new PrinterSettings();//Bug 1457. Explicitly create new instance

        /// <summary>
        /// This fires after a element if added or removed
        /// </summary>
        public event EventHandler ElementsChanged;
        
        /// <summary>
        /// This fires after the selection has changed
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Thie fires when the zoom of the layout changes
        /// </summary>
        public event EventHandler ZoomChanged;

        /// <summary>
        /// This fires when the projects file name is changed
        /// </summary>
        public event EventHandler FilenameChanged;

        #endregion

        #region ---------------- Internal Properties

        /// <summary>
        /// Gets the list of layoutElements currently selected in the project
        /// </summary>
        internal List<LayoutElement> SelectedLayoutElements
        {
            get { return _selectedLayoutElements; }
        }

        #endregion

        #region ---------------- Public Properties

        /// <summary>
        /// Gets the list of layoutElements currently loaded in the project
        /// </summary>
        public List<LayoutElement> LayoutElements
        {
            get { return _layoutElements; }
        }

        /// <summary>
        /// Gets or sets the filename of the current project
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set 
            { 
                _filename = value;
                OnFilenameChanged(null);
            }
        }

        /// <summary>
        /// Gets or sets  the printer settings for the layout
        /// </summary>
        [Browsable(false)]
        public PrinterSettings PrinterSettings
        {
            get { return _printerSettings; }
            set 
            { 
                _printerSettings = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the layout menu strip to use
        /// </summary>
        public LayoutMenuStrip LayoutMenuStrip
        {
            get { return _layoutMenuStrip; }
            set { if (value==null) return; _layoutMenuStrip = value;_layoutMenuStrip.LayoutControl = this; }
        }

        /// <summary>
        /// Gets or sets the layoutproperty grip to use
        /// </summary>
        public LayoutPropertyGrid LayoutPropertyGrip
        {
            get { return _layoutPropertyGrip; }
            set { if (value == null) return; _layoutPropertyGrip = value; _layoutPropertyGrip.LayoutControl = this; }
        }

        /// <summary>
        /// Gets or sets the Map control to use
        /// </summary>
        public Map.Map MapControl
        {
            get { return _mapControl; }
            set { _mapControl = value; }
        }

        /// <summary>
        /// Gets or sets the layout tool strip to use
        /// </summary>
        public LayoutZoomToolStrip LayoutZoomToolStrip
        {
            get { return _layoutZoomToolStrip; }
            set { if (value == null) return; _layoutZoomToolStrip = value; _layoutZoomToolStrip.LayoutControl = this; }
        }

        /// <summary>
        /// Gets or sets the layout list box
        /// </summary>
        public LayoutListBox LayoutListBox
        {
            get { return _layoutListBox; }
            set { if (value == null) return; _layoutListBox = value; _layoutListBox.LayoutControl = this; }
        }

        /// <summary>
        /// Gets or sets the LayoutDocToolStrip
        /// </summary>
        public LayoutDocToolStrip LayoutDocToolStrip
        {
            get { return _layoutDocToolStrip; }
            set { if (value == null) return; _layoutDocToolStrip = value; _layoutDocToolStrip.LayoutControl = this; }
        }

        /// <summary>
        /// Gets or sets the LayoutInsertToolStrip
        /// </summary>
        public LayoutInsertToolStrip LayoutInsertToolStrip
        {
            get { return _layoutInsertToolStrip; }
            set { if (value == null) return; _layoutInsertToolStrip = value; _layoutInsertToolStrip.LayoutControl = this; }
        }

        /// <summary>
        /// Gets of sets the LayoutMapToolStrip
        /// </summary>
        public LayoutMapToolStrip LayoutMapToolStrip
        {
            get { return _layoutMapToolStrip; }
            set { if (value == null) return; _layoutMapToolStrip = value; _layoutMapToolStrip.LayoutControl = this; }
        }

        /// <summary>
        /// Sets a boolean flag indicating if margins should be shown.
        /// </summary>
        public bool ShowMargin
        {
            get { return _showMargin; }
            set { _showMargin = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets the zoom of the paper
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                PointF paperCenter = ScreenToPaper((Width - _vScrollBar.Width - 4) / 2F, (Height - _hScrollBar.Height - 4) / 2F);
                if (value <= 0.1F)
                    _zoom = 0.1F;
                else
                    _zoom = value;
                CenterPaperOnPoint(paperCenter);
                OnZoomChanged(null);
            }
        }

        /// <summary>
        /// Gets or sets the smoothing mode to use to draw the map
        /// </summary>
        public SmoothingMode DrawingQuality
        {
            get { return _drawingQuality; }
            set { _drawingQuality = value; }
        }

        /// <summary>
        /// Gets or sets the map pan mode
        /// </summary>\
        [Browsable(false)]
        public Boolean MapPanMode
        {
            get
            {
                if (_mouseMode == MouseMode.PanMap || _mouseMode == MouseMode.StartPanMap)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    _mouseMode = MouseMode.StartPanMap;
                else
                    _mouseMode = MouseMode.Default;
            }
        }

        #endregion

        #region ---------------- Private Properties

        /// <summary>
        /// Gets the width of the paper in 1/100 of an inch
        /// </summary>
        private int PaperWidth
        {
            get
            {
                
                if (_printerSettings.DefaultPageSettings.Landscape)
                    return _printerSettings.DefaultPageSettings.PaperSize.Height;
                return _printerSettings.DefaultPageSettings.PaperSize.Width;
            }
        }
        /// <summary>
        /// Gets the heigh of the paper in 1/100 of an inch
        /// </summary>
        private int PaperHeight
        {
            get
            {
                if (_printerSettings.DefaultPageSettings.Landscape)
                    return _printerSettings.DefaultPageSettings.PaperSize.Width;
                return _printerSettings.DefaultPageSettings.PaperSize.Height;
            }
        }

        #endregion

        #region ---------------- Internal Methods

        /// <summary>
        /// Removes the specified layoutElement from the layout
        /// </summary>
        /// <param name="le"></param>
        internal void RemoveFromLayout(LayoutElement le)
        {
            _selectedLayoutElements.Remove(le);
            OnSelectionChanged(null);
            _layoutElements.Remove(le);
            OnElementsChanged(null);
            Invalidate(new Region(PaperToScreen(le.Rectangle)));
        }

        /// <summary>
        /// Clears the the layout of all layoutElements
        /// </summary>
        internal void ClearLayout()
        {
            _selectedLayoutElements.Clear();
            OnSelectionChanged(null);
            _layoutElements.Clear();
            OnElementsChanged(null);
            Invalidate();
        }

        /// <summary>
        /// Adds the specified LayoutElement le to the selection
        /// </summary>
        internal void AddToSelection(LayoutElement le)
        {
            _selectedLayoutElements.Add(le);
            Invalidate(new Region(PaperToScreen(le.Rectangle)));
            OnSelectionChanged(null);
        }

        /// <summary>
        /// Adds the specified LayoutElement le to the selection
        /// </summary>
        internal void AddToSelection(List<LayoutElement> le)
        {
            _selectedLayoutElements.AddRange(le);
            Invalidate();
            OnSelectionChanged(null);
        }

        /// <summary>
        /// Removes the specified layoutElement from the selection
        /// </summary>
        internal void RemoveFromSelection(LayoutElement le)
        {
            _selectedLayoutElements.Remove(le);
            Invalidate(new Region(PaperToScreen(le.Rectangle)));
            OnSelectionChanged(null);
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        internal void ClearSelection()
        {
            _selectedLayoutElements.Clear();
            Invalidate();
            OnSelectionChanged(null);
        }

        #endregion

        #region ---------------- Public Methods

        /// <summary>
        /// This is the constructor, it makes a LayoutControl
        /// </summary>
        public LayoutControl()
        {
            InitializeComponent();
            _printerSettings = new PrinterSettings();
            _filename = "";

            //This code is used to speed up drawing because for some reason accessing .PaperSize is slow with its default value
            //I know its ugly but it speeds things up from 80ms to 0ms
            PageSetupForm tempForm = new PageSetupForm(_printerSettings);
            tempForm.OK_Button_Click(null, null);
            
            _drawingQuality = SmoothingMode.HighQuality;
            _zoom = 1;
            ZoomFitToScreen();
        }

        /// <summary>
        /// Creates a new blank layout
        /// </summary>
        /// <param name="promptSave">Prompts the user if they want to save first</param>
        public void NewLayout(bool promptSave)
        {
            if (_layoutElements.Count > 0 && promptSave)
            {
                DialogResult dr = MessageBox.Show(this, MessageStrings.LayoutSaveFirst, "MapWindow Print Layout", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                    return;
                if (dr == DialogResult.Yes)
                    SaveLayout(true);
            }

            ClearLayout();
            Filename = "";
        }

        /// <summary>
        /// Shows a load dialog box and prompts the user to open a layout file
        /// </summary>
        /// <param name="promptSave">Prompts the user if they want to save first</param>
        public void LoadLayout(bool promptSave)
        {
            if (_layoutElements.Count > 0 && promptSave)
            {
                DialogResult dr = MessageBox.Show(this, MessageStrings.LayoutSaveFirst, "MapWindow Print Layout", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                    return;
                if (dr == DialogResult.OK)
                    SaveLayout(true);
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = MessageStrings.LayoutLoadDialogTitle;
            ofd.CheckFileExists = true;
            ofd.Filter = "MapWindow Layout File (*.mwl)|*.mwl";
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    LoadLayout(ofd.FileName);
                }
                catch(Exception e)
                {
                    MessageBox.Show(MessageStrings.LayoutErrorLoad + e.Message, "MapWindow Print Layout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Loads the selected layoutfile
        /// </summary>
        /// <param name="filename">The layout file to load</param>
        public void LoadLayout(string filename)
        {
            //Open the model xml document
            XmlDocument layoutXmlDoc = new XmlDocument();
            layoutXmlDoc.Load(filename);
            XmlElement root = layoutXmlDoc.DocumentElement;

            List<LayoutElement> loadList = new List<LayoutElement>();

            if (root != null)
            {
                if (root.Name == "MapWindowLayout")
                {
                    //This creates instances of all the elements 
                    XmlNode child = root.LastChild;
                    while (child != null)
                    {
                        LayoutElement newLe = null;
                        switch (child.ChildNodes[0].Name)
                        {
                            case "Bitmap":
                                newLe = new LayoutBitmap();
                                break;
                            case "Legend":
                                newLe = CreateLegendElement();
                                break;
                            case "Map":
                                newLe = CreateMapElement();
                                break;
                            case "NorthArrow":
                                newLe = new LayoutNorthArrow();
                                break;
                            case "Rectangle":
                                newLe = new LayoutRectangle();
                                break;
                            case "ScaleBar":
                                newLe = CreateScaleBarElement();
                                break;
                            case "Text":
                                newLe = new LayoutText();
                                break;
                        }
                        if (newLe != null)
                        {
                            newLe.Name = child.Attributes["Name"].Value;
                            newLe.Invalidated += LeInvalidated;
                            newLe.Rectangle = new RectangleF(float.Parse(child.Attributes["RectangleX"].Value, System.Globalization.CultureInfo.InvariantCulture), float.Parse(child.Attributes["RectangleY"].Value, System.Globalization.CultureInfo.InvariantCulture), float.Parse(child.Attributes["RectangleWidth"].Value, System.Globalization.CultureInfo.InvariantCulture), float.Parse(child.Attributes["RectangleHeight"].Value, System.Globalization.CultureInfo.InvariantCulture));
                            newLe.ResizeStyle = (ResizeStyle)Enum.Parse(typeof(ResizeStyle), child.Attributes["ResizeStyle"].Value);
                            loadList.Insert(0,newLe);
                        }
                        child = child.PreviousSibling;
                    }

                    //Since some of the elements may be dependant on elements already being added we add their other properties after we add them all                
                    child = root.LastChild;
                    for (int i = loadList.Count - 1; i >= 0; i--)
                    {
                        if (child != null)
                        {
                            XmlNode innerChild = child.ChildNodes[0];
                            if (loadList[i] is LayoutBitmap)
                            {
                                LayoutBitmap lb = loadList[i] as LayoutBitmap;
                                if (lb != null)
                                {
                                    lb.Filename = innerChild.Attributes["Filename"].Value;
                                    lb.PreserveAspectRatio = Convert.ToBoolean(innerChild.Attributes["PreserveAspectRatio"].Value);
                                    lb.Draft = Convert.ToBoolean(innerChild.Attributes["Draft"].Value);
                                }
                            }
                            else if (loadList[i] is LayoutLegend)
                            {
                                LayoutLegend ll = loadList[i] as LayoutLegend;
                                if (ll != null)
                                {
                                    ll.LayoutControl = this;
                                    ll.TextHint = (System.Drawing.Text.TextRenderingHint)Enum.Parse(typeof(System.Drawing.Text.TextRenderingHint), innerChild.Attributes["TextHint"].Value);
                                    ll.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromInvariantString(innerChild.Attributes["Color"].Value);
                                    ll.Font = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(innerChild.Attributes["Font"].Value);
                                }
                                int mapIndex = Convert.ToInt32(innerChild.Attributes["Map"].Value);
                                if (mapIndex >= 0)
                                    if (ll != null) ll.Map = loadList[mapIndex] as LayoutMap;
                                string layStr = innerChild.Attributes["Layers"].Value;
                                List<int> layers = new List<int>();
                                while (layStr.EndsWith("|"))
                                {
                                    layStr = layStr.TrimEnd("|".ToCharArray());
                                    layers.Add((int)TypeDescriptor.GetConverter(typeof(int)).ConvertFromString(layStr.Substring(layStr.LastIndexOf("|") + 1)));
                                    layStr = layStr.Substring(0, layStr.LastIndexOf("|") + 1);
                                }
                                if (ll != null)
                                {
                                    ll.NumColumns = (int)TypeDescriptor.GetConverter(typeof(int)).ConvertFromString(innerChild.Attributes["NumColumns"].Value);
                                    ll.Layers = layers;
                                }
                            }
                            else if (loadList[i] is LayoutMap)
                            {
                                LayoutMap lm = loadList[i] as LayoutMap;
                                Geometries.Envelope env = new Geometries.Envelope();
                                env.Minimum.X = (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFromInvariantString(innerChild.Attributes["EnvelopeXmin"].Value);
                                env.Minimum.Y = (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFromInvariantString(innerChild.Attributes["EnvelopeYmin"].Value);
                                env.Maximum.X = (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFromInvariantString(innerChild.Attributes["EnvelopeXmax"].Value);
                                env.Maximum.Y = (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFromInvariantString(innerChild.Attributes["EnvelopeYmax"].Value);
                                if (lm != null) lm.Envelope = env;
                            }
                            else if (loadList[i] is LayoutNorthArrow)
                            {
                                LayoutNorthArrow na = loadList[i] as LayoutNorthArrow;
                                if (na != null)
                                {
                                    na.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromInvariantString(innerChild.Attributes["Color"].Value);
                                    na.NorthArrowStyle = (NorthArrowStyle)Enum.Parse(typeof(NorthArrowStyle), innerChild.Attributes["Style"].Value);
                                    if (innerChild.Attributes["Rotation"] != null)
                                    {
                                        na.Rotation = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromInvariantString(innerChild.Attributes["Rotation"].Value);
                                    }
                                }
                            }
                            else if (loadList[i] is LayoutRectangle)
                            {
                                LayoutRectangle lr = loadList[i] as LayoutRectangle;
                                if (lr != null)
                                {
                                    lr.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(innerChild.Attributes["Color"].Value);
                                    lr.BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(innerChild.Attributes["BackColor"].Value);
                                    lr.OutlineWidth = Convert.ToInt32(innerChild.Attributes["OutlineWidth"].Value);
                                }
                            }
                            else if (loadList[i] is LayoutScaleBar)
                            {
                                LayoutScaleBar lsc = loadList[i] as LayoutScaleBar;
                                if (lsc != null)
                                {
                                    lsc.LayoutControl = this;
                                    lsc.TextHint = (System.Drawing.Text.TextRenderingHint)Enum.Parse(typeof(System.Drawing.Text.TextRenderingHint), innerChild.Attributes["TextHint"].Value);
                                    lsc.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(innerChild.Attributes["Color"].Value);
                                    lsc.Font = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(innerChild.Attributes["Font"].Value);
                                    lsc.BreakBeforeZero = Convert.ToBoolean(innerChild.Attributes["BreakBeforeZero"].Value);
                                    lsc.NumberOfBreaks = Convert.ToInt32(innerChild.Attributes["NumberOfBreaks"].Value);
                                    lsc.Unit = (ScaleBarUnits)Enum.Parse(typeof(ScaleBarUnits), innerChild.Attributes["Unit"].Value);
                                    lsc.UnitText = innerChild.Attributes["UnitText"].Value;
                                }
                                int mapIndex = Convert.ToInt32(innerChild.Attributes["Map"].Value);
                                if (mapIndex >= 0)
                                    if (lsc != null) lsc.Map = loadList[mapIndex] as LayoutMap;
                            }
                            else if (loadList[i] is LayoutText)
                            {
                                LayoutText lt = loadList[i] as LayoutText;
                                if (lt != null)
                                {
                                    lt.TextHint = (System.Drawing.Text.TextRenderingHint)Enum.Parse(typeof(System.Drawing.Text.TextRenderingHint), innerChild.Attributes["TextHint"].Value);
                                    lt.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(innerChild.Attributes["Color"].Value);
                                    lt.Font = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(innerChild.Attributes["Font"].Value);
                                    lt.ContentAlignment = (ContentAlignment)TypeDescriptor.GetConverter(typeof(ContentAlignment)).ConvertFromString(innerChild.Attributes["ContentAlignment"].Value);
                                    lt.Text = innerChild.Attributes["Text"].Value;
                                }
                            }
                        }
                        if (child != null) child = child.PreviousSibling;
                    }
                    _layoutElements.Clear();
                    _selectedLayoutElements.Clear();
                    _layoutElements.InsertRange(0, loadList);
                    Filename = filename;
                    Invalidate();
                    OnElementsChanged(null);
                }
            }
        }

        /// <summary>
        /// Prepapres the layoutcontrol for closing, prompts the user to save if needed
        /// </summary>
        public void CloseLayout()
        {
            if (_layoutElements.Count > 0)
            {
                DialogResult dr = MessageBox.Show(this, MessageStrings.LayoutSaveFirst, "MapWindow Print Layout", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                    SaveLayout(true);
            }
        }

        /// <summary>
        /// Shows a load dialog box and prompts the user to open a layout file
        /// </summary>
        public void SaveLayout(bool promptSaveAs)
        {
            string tempFilename = Filename;
            if (Filename == "" || promptSaveAs)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = MessageStrings.LayoutSaveDialogTitle;
                sfd.Filter = "MapWindow Layout File (*.mwl)|*.mwl";
                sfd.AddExtension = true;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    tempFilename = sfd.FileName;
                }
                else
                    return;
            }
            try
            {
                SaveLayout(tempFilename);
                Filename = tempFilename;
            }
            catch (Exception e)
            {
                MessageBox.Show(MessageStrings.LayoutErrorSave + e.Message, "MapWindow Print Layout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the layout to the specified filename
        /// </summary>
        /// <param name="filename"></param>
        public void SaveLayout(string filename)
        {
            //Creates the model xml document
            XmlDocument layoutXmlDoc = new XmlDocument();

            XmlElement root = layoutXmlDoc.CreateElement("MapWindowLayout");
            layoutXmlDoc.AppendChild(root);

            //Saves the Tools and their output configuration to the model
            foreach (LayoutElement le in _layoutElements)
            {
                XmlElement element = layoutXmlDoc.CreateElement("Element");
                element.SetAttribute("Name", le.Name);
                if (le is LayoutBitmap)
                {
                    LayoutBitmap lb = le as LayoutBitmap;
                    XmlElement bitmap = layoutXmlDoc.CreateElement("Bitmap");
                    bitmap.SetAttribute("Filename", lb.Filename);
                    bitmap.SetAttribute("PreserveAspectRatio", lb.PreserveAspectRatio.ToString());
                    bitmap.SetAttribute("Draft", lb.Draft.ToString());
                    element.AppendChild(bitmap);
                }
                else if (le is LayoutLegend)
                {
                    LayoutLegend ll = le as LayoutLegend;
                    XmlElement legend = layoutXmlDoc.CreateElement("Legend");
                    legend.SetAttribute("TextHint", ll.TextHint.ToString());
                    legend.SetAttribute("Color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(ll.Color));
                    legend.SetAttribute("Font", TypeDescriptor.GetConverter(typeof(Font)).ConvertToInvariantString(ll.Font));
                    legend.SetAttribute("Map", _layoutElements.IndexOf(ll.Map).ToString());
                    string layerString = "";
                    foreach (int i in ll.Layers)
                        layerString = layerString + TypeDescriptor.GetConverter(typeof(int)).ConvertToInvariantString(i) + "|";
                    legend.SetAttribute("Layers", layerString);
                    legend.SetAttribute("NumColumns", TypeDescriptor.GetConverter(typeof(int)).ConvertToInvariantString(ll.NumColumns));
                    element.AppendChild(legend);
                }
                else if (le is LayoutMap)
                {
                    LayoutMap lm = le as LayoutMap;
                    XmlElement map = layoutXmlDoc.CreateElement("Map");
                    map.SetAttribute("EnvelopeXmin", TypeDescriptor.GetConverter(typeof(double)).ConvertToInvariantString(lm.Envelope.Minimum.X));
                    map.SetAttribute("EnvelopeYmin", TypeDescriptor.GetConverter(typeof(double)).ConvertToInvariantString(lm.Envelope.Minimum.Y));
                    map.SetAttribute("EnvelopeXmax", TypeDescriptor.GetConverter(typeof(double)).ConvertToInvariantString(lm.Envelope.Maximum.X));
                    map.SetAttribute("EnvelopeYmax", TypeDescriptor.GetConverter(typeof(double)).ConvertToInvariantString(lm.Envelope.Maximum.Y));
                    element.AppendChild(map);
                }
                else if (le is LayoutNorthArrow)
                {
                    LayoutNorthArrow na = le as LayoutNorthArrow;
                    XmlElement northArrow = layoutXmlDoc.CreateElement("NorthArrow");
                    northArrow.SetAttribute("Color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(na.Color));
                    northArrow.SetAttribute("Style", na.NorthArrowStyle.ToString());
                    northArrow.SetAttribute("Rotation", TypeDescriptor.GetConverter(typeof(float)).ConvertToInvariantString(na.Rotation));
                    element.AppendChild(northArrow);
                }
                else if (le is LayoutRectangle)
                {
                    LayoutRectangle lr = le as LayoutRectangle;
                    XmlElement rectangle = layoutXmlDoc.CreateElement("Rectangle");
                    rectangle.SetAttribute("Color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(lr.Color));
                    rectangle.SetAttribute("BackColor", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(lr.BackColor));
                    rectangle.SetAttribute("OutlineWidth", lr.OutlineWidth.ToString());
                    element.AppendChild(rectangle);
                }
                else if (le is LayoutScaleBar)
                {
                    LayoutScaleBar lsc = le as LayoutScaleBar;
                    XmlElement scaleBar = layoutXmlDoc.CreateElement("ScaleBar");
                    scaleBar.SetAttribute("TextHint", lsc.TextHint.ToString());
                    scaleBar.SetAttribute("Color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(lsc.Color));
                    scaleBar.SetAttribute("Font", TypeDescriptor.GetConverter(typeof(Font)).ConvertToInvariantString(lsc.Font));
                    scaleBar.SetAttribute("BreakBeforeZero", lsc.BreakBeforeZero.ToString());
                    scaleBar.SetAttribute("NumberOfBreaks", lsc.NumberOfBreaks.ToString());
                    scaleBar.SetAttribute("Unit", lsc.Unit.ToString());
                    scaleBar.SetAttribute("UnitText", lsc.UnitText);
                    scaleBar.SetAttribute("Map", _layoutElements.IndexOf(lsc.Map).ToString());
                    element.AppendChild(scaleBar);
                }
                else if (le is LayoutText)
                {
                    LayoutText lt = le as LayoutText;
                    XmlElement layoutText = layoutXmlDoc.CreateElement("Text");
                    layoutText.SetAttribute("TextHint", lt.TextHint.ToString());
                    layoutText.SetAttribute("Color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToInvariantString(lt.Color));
                    layoutText.SetAttribute("Font", TypeDescriptor.GetConverter(typeof(Font)).ConvertToInvariantString(lt.Font));
                    layoutText.SetAttribute("ContentAlignment", lt.ContentAlignment.ToString());
                    layoutText.SetAttribute("Text", lt.Text);
                    element.AppendChild(layoutText);
                }

                element.SetAttribute("RectangleX", TypeDescriptor.GetConverter(typeof(float)).ConvertToInvariantString(le.Rectangle.X));
                element.SetAttribute("RectangleY", TypeDescriptor.GetConverter(typeof(float)).ConvertToInvariantString(le.Rectangle.Y));
                element.SetAttribute("RectangleWidth", TypeDescriptor.GetConverter(typeof(float)).ConvertToInvariantString(le.Rectangle.Width));
                element.SetAttribute("RectangleHeight", TypeDescriptor.GetConverter(typeof(float)).ConvertToInvariantString(le.Rectangle.Height));
                element.SetAttribute("ResizeStyle", le.ResizeStyle.ToString());
                root.AppendChild(element);
            }

            layoutXmlDoc.Save(filename);
        }

        /// <summary>
        /// Adds a layout element to the layout
        /// </summary>
        public void AddToLayout(LayoutElement le)
        {
            string leName = le.Name + " 1";
            int i = 2;
            while (_layoutElements.FindAll(delegate(LayoutElement o) { return (o.Name == leName); }).Count > 0)
            {
                leName = le.Name + " " + i;
                i++;
            }
            le.Name = leName;

            _layoutElements.Insert(0, le);
            OnElementsChanged(null);
            le.Invalidated += LeInvalidated;
            Invalidate(new Region(PaperToScreen(le.Rectangle)));
        }

        /// <summary>
        /// This shows the choose printer dialog
        /// </summary>
        public void ShowChoosePrinterDialog()
        {
            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = _printerSettings;
            pd.ShowDialog();
            Invalidate();
        }

        /// <summary>
        /// This shows the pageSetup dialog
        /// </summary>
        public void ShowPageSetupDialog()
        {
            PageSetupForm setupFrom = new PageSetupForm(_printerSettings);
            setupFrom.ShowDialog(this);
            Invalidate();
        }

        /// <summary>
        /// Prints to the printer currently in PrinterSettings
        /// </summary>
        public void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings = _printerSettings;
            pd.PrintPage += PrintPage;
            pd.Print();
        }

        /// <summary>
        /// Refreshes all of the elements in the layout
        /// </summary>
        public void RefreshElements()
        {
            _suppressLEinvalidate = true;
            foreach (LayoutElement le in _layoutElements)
                le.RefreshElement();
            _suppressLEinvalidate = false;
            Invalidate();
        }

        /// <summary>
        /// This event handler is fired by the print document when it prints and draws the layout to the print document
        /// </summary>
        void PrintPage(object sender, PrintPageEventArgs e)
        {
            LayoutElement le;
            for (int i = LayoutElements.Count - 1; i >= 0; i--)
            {
                le = LayoutElements[i];
                e.Graphics.Clip = new Region(le.Rectangle);
                le.Draw(e.Graphics, true);
            }
        }

        /// <summary>
        /// Zooms into the paper
        /// </summary>
        public void ZoomIn()
        {
            Zoom = Zoom + 0.1F;
        }

        /// <summary>
        /// Zooms out of the paper
        /// </summary>
        public void ZoomOut()
        {
            Zoom = Zoom - 0.1F;
        }

        /// <summary>
        /// Zooms the page to fit to the screen and centers it
        /// </summary>
        public void ZoomFitToScreen()
        {
            float xZoom = (Width - 50) / (PaperWidth * 96F / 100F);
            float yZoom = (Height - 50) / (PaperHeight * 96F / 100F) ;
            if (xZoom < yZoom)
                _zoom = xZoom;
            else
                _zoom = yZoom;
            CenterPaperOnPoint(new PointF(PaperWidth / 2F, PaperHeight / 2F));
            OnZoomChanged(null);
        }

        /// <summary>
        /// Zooms the map element in by 10%
        /// </summary>
        public static void ZoomInMap(LayoutMap lm)
        {
            double tenPerWidth = (lm.Envelope.Maximum.X - lm.Envelope.Minimum.X) / 20;
            double tenPerHeight = (lm.Envelope.Maximum.Y - lm.Envelope.Minimum.Y) / 20;
            Geometries.Envelope envl = new Geometries.Envelope();
            envl.Minimum.X = lm.Envelope.Minimum.X + tenPerWidth;
            envl.Minimum.Y = lm.Envelope.Minimum.Y + tenPerHeight;
            envl.Maximum.X = lm.Envelope.Maximum.X - tenPerWidth;
            envl.Maximum.Y = lm.Envelope.Maximum.Y - tenPerWidth;
            lm.Envelope = envl;
        }

        /// <summary>
        /// Zooms the map element out by 10%
        /// </summary>
        public static void ZoomOutMap(LayoutMap lm)
        {
            double tenPerWidth = (lm.Envelope.Maximum.X - lm.Envelope.Minimum.X) / 20;
            double tenPerHeight = (lm.Envelope.Maximum.Y - lm.Envelope.Minimum.Y) / 20;
            Geometries.Envelope envl = new Geometries.Envelope();
            envl.Minimum.X = lm.Envelope.Minimum.X - tenPerWidth;
            envl.Minimum.Y = lm.Envelope.Minimum.Y - tenPerHeight;
            envl.Maximum.X = lm.Envelope.Maximum.X + tenPerWidth;
            envl.Maximum.Y = lm.Envelope.Maximum.Y + tenPerWidth;
            lm.Envelope = envl;
        }

        /// <summary>
        /// Zooms the map element to the full extent of its layers
        /// </summary>
        public static void ZoomFullExtentMap(LayoutMap lm)
        {
            lm.ZoomToFullExtent();
        }

        /// <summary>
        /// Zoom the map to the extent of the data view
        /// </summary>
        public static void ZoomFullViewExtentMap(LayoutMap lm)
        {
            lm.ZoomViewExtent();
        }

        /// <summary>
        /// Pans the map the specified amount
        /// </summary>
        /// <param name="lm">the layout map to pan</param>
        /// <param name="x">The distance to pan the map on x-axis in screen coord</param>
        /// <param name="y">The distance to pan the map on y-axis in screen coord</param>
        public void PanMap(LayoutMap lm, float x, float y)
        {
            RectangleF mapOnScreen = PaperToScreen(lm.Rectangle);
            lm.PanMap((lm.Envelope.Width / mapOnScreen.Width) * x, (lm.Envelope.Height / mapOnScreen.Height ) * -y);
        }

        /// <summary>
        /// Deletes all of the selected elements from the model
        /// </summary>
        public void DeleteSelected()
        {
            foreach (LayoutElement le in _selectedLayoutElements.ToArray())
                RemoveFromLayout(le);
            Invalidate();
            OnSelectionChanged(null);
        }

        /// <summary>
        /// Inverts the selection
        /// </summary>
        public void InvertSelection()
        {
            List<LayoutElement> unselected = _layoutElements.FindAll(delegate(LayoutElement o) { return (_selectedLayoutElements.Contains(o) == false); });
            _selectedLayoutElements.Clear();
            _selectedLayoutElements.InsertRange(0, unselected);
            OnSelectionChanged(null);
            Invalidate();
        }

        /// <summary>
        /// Moves the selection up by one
        /// </summary>
        public void MoveSelectionUp()
        {
            if (_selectedLayoutElements.Count < 1) return;
            int index = _layoutElements.Count -1;
            foreach (LayoutElement le in _selectedLayoutElements)
            {
                if (index > _layoutElements.IndexOf(le))
                    index = _layoutElements.IndexOf(le);
            }
            if (index == 0) return;

            foreach (LayoutElement le in _selectedLayoutElements)
            {
                index = _layoutElements.IndexOf(le);
                _layoutElements.Remove(le);
                _layoutElements.Insert(index - 1, le);
            }
            OnSelectionChanged(null);
            Invalidate();
        }

        /// <summary>
        /// Moves the selection down by one
        /// </summary>
        public void MoveSelectionDown()
        {
            if (_selectedLayoutElements.Count < 1) return;
            int index = 0;
            int[] indexArray = new int[_selectedLayoutElements.Count];
            for (int i = 0; i < _selectedLayoutElements.Count; i++)
            {
                indexArray[i] = _layoutElements.IndexOf(_selectedLayoutElements[i]);
                if (index < indexArray[i])
                    index = indexArray[i];
            }
            if (index == _layoutElements.Count - 1) return;

            for(int i = 0; i< _selectedLayoutElements.Count; i++)
                _layoutElements.Remove(_selectedLayoutElements[i]);

            for (int i = 0; i < _selectedLayoutElements.Count; i++)
            {
                _layoutElements.Insert(indexArray[i] + 1, _selectedLayoutElements[i]);
            }
            OnSelectionChanged(null);
            Invalidate();
        }

        /// <summary>
        /// Selects All the elements in the layout
        /// </summary>
        public void SelectAll()
        {
            _selectedLayoutElements.Clear();
            _selectedLayoutElements.InsertRange(0, _layoutElements);
            OnSelectionChanged(null);
            Invalidate();
        }

        /// <summary>
        /// Allows the user to click on the layout and drag a rectangle where they want to insert an element
        /// </summary>
        public void AddElementWithMouse(LayoutElement le)
        {
            _elementToAddWithMouse = le;
            ClearSelection();
            _mouseMode = MouseMode.StartInsertNewElement;
            Cursor = Cursors.Cross;
        }

        /// <summary>
        /// Creates an instance of the MapElement and returns it
        /// </summary>
        public virtual LayoutElement CreateMapElement()
        {
            return new LayoutMap(_mapControl);
        }

        /// <summary>
        /// Creates an instance of the LegendElement and returns it
        /// </summary>
        public virtual LayoutElement CreateLegendElement()
        {
            return new LayoutLegend();
        }

        /// <summary>
        /// Creates an instance of the ScaleBarElement and returns it
        /// </summary>
        public virtual LayoutElement CreateScaleBarElement()
        {
            return new LayoutScaleBar();

        }

        /// <summary>
        /// Converts all of the selected layout elements to bitmaps
        /// </summary>
        public virtual void ConvertSelectedToBitmap()
        {
            foreach (LayoutElement le in _selectedLayoutElements.ToArray())
            {
                if (le is LayoutBitmap) continue;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = le.Name;
                sfd.Filter = "Portable Network Graphics (*.png)|*.png|Joint Photographic Experts Group (*.jpg)|*.jpg|Microsoft Bitmap (*.bmp)|*.bmp|Graphics Interchange Format (*.gif)|*.gif|Tagged Image File (*.tif)|*.tif";
                sfd.FilterIndex = 1;
                sfd.AddExtension = true;
                if (sfd.ShowDialog(this) == DialogResult.Cancel)
                    return;
                ConvertElementToBitmap(le, sfd.FileName);
            }
        }

        /// <summary>
        /// Converts a selected layout element into a bitmap and saves it a the specified location removing the old element and replacing it
        /// </summary>
        /// <param name="le"></param>
        /// <param name="filename"></param>
        public virtual void ConvertElementToBitmap(LayoutElement le, string filename)
        {
            if (le is LayoutBitmap) return;
            Bitmap temp = new Bitmap(Convert.ToInt32(le.Size.Width * 3), Convert.ToInt32(le.Size.Height * 3), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            temp.SetResolution(96, 96);
            temp.MakeTransparent();
            Graphics g = Graphics.FromImage(temp);
            g.PageUnit = GraphicsUnit.Pixel;
            g.ScaleTransform(300F/100F, 300F/100F);
            g.TranslateTransform(-le.LocationF.X,-le.LocationF.Y);
            le.Draw(g,true);
            g.Dispose();
            temp.SetResolution(300, 300);
            temp.Save(filename);
            temp.Dispose();
            LayoutBitmap newLb = new LayoutBitmap();
            newLb.Rectangle = le.Rectangle;
            newLb.Name = le.Name;
            newLb.Filename = filename;
            _layoutElements.Insert(_layoutElements.IndexOf(le), newLb);
            _layoutElements.Remove(le);
            _selectedLayoutElements.Insert(_selectedLayoutElements.IndexOf(le), newLb);
            _selectedLayoutElements.Remove(le);
            OnSelectionChanged(null);
            Invalidate();
        }


        /// <summary>
        /// Aligns elements with each other or with the margins
        /// </summary>
        /// <param name="elements">A list of elements to align</param>
        /// <param name="side">The side to align to</param>
        /// <param name="margin">True to align to paper margins, false to align to the most extreme element of the indicated side</param>
        public void AlignElements(List<LayoutElement> elements, Alignments side, bool margin)
        {
            switch (side)
            {
                case Alignments.Left:
                    if (margin)
                    {
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(_printerSettings.DefaultPageSettings.Margins.Left, le.LocationF.Y);
                    }
                    else
                    {
                        float leftMost = float.MaxValue;
                        foreach (LayoutElement le in elements)
                            if (le.LocationF.X < leftMost) leftMost = le.LocationF.X;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(leftMost, le.LocationF.Y);
                    }
                    break;
                case Alignments.Right:
                    if (margin)
                    {
                        float rightMost = PaperWidth - _printerSettings.DefaultPageSettings.Margins.Right;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(rightMost - le.Size.Width, le.LocationF.Y);
                    }
                    else
                    {
                        float rightMost = float.MinValue;
                        foreach (LayoutElement le in elements)
                            if (le.LocationF.X + le.Size.Width > rightMost) rightMost = le.LocationF.X + le.Size.Width;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(rightMost - le.Size.Width, le.LocationF.Y);
                    }
                    break;
                case Alignments.Top:
                    if (margin)
                    {
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, _printerSettings.DefaultPageSettings.Margins.Top);
                    }
                    else
                    {
                        float topMost = float.MaxValue;
                        foreach (LayoutElement le in elements)
                            if (le.LocationF.Y < topMost) topMost = le.LocationF.Y;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, topMost);
                    }
                    break;
                case Alignments.Bottom:
                    if (margin)
                    {
                        float bottomMost = PaperHeight - _printerSettings.DefaultPageSettings.Margins.Bottom;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, bottomMost - le.Size.Height);
                    }
                    else
                    {
                        float bottomMost = float.MinValue;
                        foreach (LayoutElement le in elements)
                            if (le.LocationF.Y + le.Size.Height > bottomMost) bottomMost = le.LocationF.Y + le.Size.Height;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, bottomMost - le.Size.Height);
                    }
                    break;

                case Alignments.Horizontal:
                    if (margin)
                    {
                        float centerHor = PaperWidth / 2F;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(centerHor - (le.Size.Width / 2F), le.LocationF.Y);
                    }
                    else
                    {
                        float centerHor = 0;
                        float widest = 0;
                        foreach (LayoutElement le in elements)
                            if (le.Size.Width > widest)
                            {
                                widest = le.Size.Width;
                                centerHor = le.LocationF.X + (widest / 2F);
                            }
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(centerHor - (le.Size.Width / 2F), le.LocationF.Y);
                    }
                    break;
                case Alignments.Vertical:
                    if (margin)
                    {
                        float centerVer = PaperHeight / 2F;
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, centerVer - (le.Size.Height / 2F));
                    }
                    else
                    {
                        float centerVer = 0;
                        float tallest = 0;
                        foreach (LayoutElement le in elements)
                            if (le.Size.Height > tallest)
                            {
                                tallest = le.Size.Height;
                                centerVer = le.LocationF.Y + (tallest / 2F);
                            }
                        foreach (LayoutElement le in elements)
                            le.LocationF = new PointF(le.LocationF.X, centerVer - (le.Size.Height / 2F));
                    }
                    break;
            }
        }

        /// <summary>
        /// Makes all of the input layout elements have the same width or height
        /// </summary>
        /// <param name="elements">A list of elements to resize to the max size of all elements or the margins</param>
        /// <param name="axis">Fit the width or the height</param>
        /// <param name="margin">True if use margin size false to use arges element in input list</param>
        public void MatchElementsSize(List<LayoutElement> elements, Fit axis, bool margin)
        {
            if (axis == Fit.Width)
            {
                if (margin)
                {
                    float newWidth = PaperWidth - _printerSettings.DefaultPageSettings.Margins.Left - _printerSettings.DefaultPageSettings.Margins.Right;
                    foreach (LayoutElement le in elements)
                        le.Size = new SizeF(newWidth, le.Size.Height);
                }
                else
                {
                    float newWidth = 0;
                    foreach (LayoutElement le in elements)
                        if (le.Size.Width > newWidth) newWidth = le.Size.Width;
                    foreach (LayoutElement le in elements)
                        le.Size = new SizeF(newWidth, le.Size.Height);
                }
            }
            else
            {
                if (margin)
                {
                    float newHeight = PaperHeight - _printerSettings.DefaultPageSettings.Margins.Top - _printerSettings.DefaultPageSettings.Margins.Bottom;
                    foreach (LayoutElement le in elements)
                        le.Size = new SizeF(le.Size.Width, newHeight);
                }
                else
                {
                    float newHeight = 0;
                    foreach (LayoutElement le in elements)
                        if (le.Size.Height > newHeight) newHeight = le.Size.Height;
                    foreach (LayoutElement le in elements)
                        le.Size = new SizeF(le.Size.Width, newHeight);
                }
            }
        }

        #endregion

        #region ---------------- Private Methods

            /// <summary>
            /// Converts a point in screen coordinants to paper coordinants in 1/100 of an inch
            /// </summary>
            /// <returns></returns>
            private PointF ScreenToPaper(PointF screen)
            {
                return ScreenToPaper(screen.X, screen.Y);
            }

            /// <summary>
            /// Converts a point in screen coordinants to paper coordinants in 1/100 of an inch
            /// </summary>
            /// <returns></returns>
            private PointF ScreenToPaper(float screenX, float screenY)
            {
                float paperX = (screenX - _paperLocation.X) / _zoom / 96F * 100F;
                float paperY = (screenY - _paperLocation.Y) / _zoom / 96F * 100F;
                return (new PointF(paperX, paperY));
            }

            /// <summary>
            /// Converts a rectangle in screen coordinants to paper coordiants in 1/100 of an inch
            /// </summary>
            /// <returns></returns>
            private RectangleF ScreenToPaper(RectangleF screen)
            {
                return ScreenToPaper(screen.X, screen.Y, screen.Width, screen.Height);
            }

            /// <summary>
            /// Converts a rectangle in screen coordinants to paper coordiants in 1/100 of an inch
            /// </summary>
            /// <returns></returns>
            private RectangleF ScreenToPaper(float screenX, float screenY, float screenW, float screenH)
            {
                PointF paperTL = ScreenToPaper(screenX, screenY);
                PointF paperBR = ScreenToPaper(screenX + screenW, screenY + screenH);
                return new RectangleF(paperTL.X, paperTL.Y, paperBR.X - paperTL.X, paperBR.Y - paperTL.Y);
            }

            /// <summary>
            /// Converts between a point in paper coordinants in 1/100th of an inch to screen coordinants
            /// </summary>
            /// <returns></returns>
            private PointF PaperToScreen(PointF paper)
            {
                return PaperToScreen(paper.X, paper.Y);
            }

            /// <summary>
            /// Converts between a point in paper coordinants in 1/100th of an inch to screen coordinants
            /// </summary>
            /// <returns></returns>
            private PointF PaperToScreen(float paperX, float paperY)
            {
                float screenX = (paperX / 100F * 96F * _zoom) + _paperLocation.X;
                float screenY = (paperY / 100F * 96F * _zoom) + _paperLocation.Y;
                return (new PointF(screenX, screenY));
            }

            /// <summary>
            /// Converts between a rectangle in paper coordinants in 1/100th of an inch to screen coordinants
            /// </summary>
            /// <returns></returns>
            private RectangleF PaperToScreen(RectangleF paper)
            {
                return PaperToScreen(paper.X, paper.Y, paper.Width, paper.Height);
            }

            /// <summary>
            /// Converts a rectangle in screen coordinants to paper coordiants in 1/100 of an inch
            /// </summary>
            /// <returns></returns>
            private RectangleF PaperToScreen(float paperX, float paperY, float paperW, float paperH)
            {
                PointF screenTL = PaperToScreen(paperX, paperY);
                PointF screenBR = PaperToScreen(paperX + paperW, paperY + paperH);
                return new RectangleF(screenTL.X, screenTL.Y, screenBR.X - screenTL.X, screenBR.Y - screenTL.Y);
            }

            /// <summary>
            /// Centers the layout on a given point
            /// </summary>
            /// <param name="centerPoint">A Point on the paper to center on</param>
            private void CenterPaperOnPoint(PointF centerPoint)
            {
                PointF paperCenterOnScreen = PaperToScreen(centerPoint);
                float diffX = paperCenterOnScreen.X - ((Width - _vScrollBar.Width - 4) / 2F);
                float diffY = paperCenterOnScreen.Y - ((Height - _hScrollBar.Height - 4) / 2F);

                _paperLocation.X = _paperLocation.X - diffX;
                _paperLocation.Y = _paperLocation.Y - diffY;

                UpdateScrollBars();
                Invalidate();
            }

            /// <summary>
            /// Updates the scroll bars so the look and act right
            /// </summary>
            private void UpdateScrollBars()
            {
                if (_zoom == 0) 
                    _zoom = 100; //Bug 1457. Zoom can be zero on older printer drivers don't divide by 0
                RectangleF papVisRect = ScreenToPaper(0F, 0F, Width, Height);
                _hScrollBar.Minimum = -Convert.ToInt32(PaperWidth * 96 / 100.0 * _zoom);
                _hScrollBar.Maximum = Convert.ToInt32(PaperWidth * 96 / 100.0 * _zoom) + Convert.ToInt32(papVisRect.Width) / 2;
                _hScrollBar.LargeChange = Convert.ToInt32(papVisRect.Width) / 2;
                if (Convert.ToInt32(papVisRect.X / 2) < _hScrollBar.Minimum) 
                    _hScrollBar.Value = _hScrollBar.Minimum;
                else if (Convert.ToInt32(papVisRect.X / 2) > _hScrollBar.Maximum - _hScrollBar.LargeChange) 
                    _hScrollBar.Value = _hScrollBar.Maximum - _hScrollBar.LargeChange;
                else
                    _hScrollBar.Value = Convert.ToInt32(papVisRect.X /2);

                _vScrollBar.Minimum = -Convert.ToInt32(PaperHeight * 96 / 100.0 * _zoom);
                _vScrollBar.Maximum = Convert.ToInt32(PaperHeight * 96 / 100.0 * _zoom) + Convert.ToInt32(papVisRect.Height) / 2;
                _vScrollBar.LargeChange = Convert.ToInt32(papVisRect.Height) / 2;
                if (Convert.ToInt32(papVisRect.Y / 2) < _vScrollBar.Minimum)
                    _vScrollBar.Value = _vScrollBar.Minimum;
                else if (Convert.ToInt32(papVisRect.Y / 2) > _vScrollBar.Maximum - _vScrollBar.LargeChange)
                    _vScrollBar.Value = _vScrollBar.Maximum - _vScrollBar.LargeChange;
                else
                    _vScrollBar.Value = Convert.ToInt32(papVisRect.Y /2);
            }

            /// <summary>
            /// Calculates which edge of a rectangle the point intersects with, within a certain limit
            /// </summary>
            private static Edge IntersectElementEdge(RectangleF screen, PointF pt, float limit)
            {
                RectangleF ptRect = new RectangleF(pt.X-limit,pt.Y-limit,2F*limit, 2F*limit);
                if ((pt.X >= screen.X - limit && pt.X <= screen.X + limit) && (pt.Y >= screen.Y - limit && pt.Y <= screen.Y + limit))
                    return Edge.TopLeft;
                if ((pt.X >= screen.X + screen.Width - limit && pt.X <= screen.X + screen.Width + limit) && (pt.Y >= screen.Y - limit && pt.Y <= screen.Y + limit))
                    return Edge.TopRight;
                if ((pt.X >= screen.X + screen.Width - limit && pt.X <= screen.X + screen.Width + limit) && (pt.Y >= screen.Y + screen.Height - limit && pt.Y <= screen.Y + screen.Height + limit))
                    return Edge.BottomRight;
                if ((pt.X >= screen.X - limit && pt.X <= screen.X + limit) && (pt.Y >= screen.Y + screen.Height - limit && pt.Y <= screen.Y + screen.Height + limit))
                    return Edge.BottomLeft;
                if (ptRect.IntersectsWith(new RectangleF(screen.X, screen.Y, screen.Width, 1F)))
                    return Edge.Top;
                if (ptRect.IntersectsWith(new RectangleF(screen.X, screen.Y, 1F, screen.Height)))
                    return Edge.Left;
                if (ptRect.IntersectsWith(new RectangleF(screen.X, screen.Y + screen.Height, screen.Width, 1F)))
                    return Edge.Bottom;
                if (ptRect.IntersectsWith(new RectangleF(screen.X + screen.Width, screen.Y, 1F, screen.Height)))
                    return Edge.Right;
                return Edge.None;
            }

        #endregion

        #region ---------------- Drawing code

        /// <summary>
        /// Drawing code
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //Deal with invalidate rectangles that have a size of 0
            if ((e.ClipRectangle.Width <= 0) || (e.ClipRectangle.Height <= 0)) return;

            //Store the cursor so we can show an hour glass while drawing
            Cursor oldCursor = Cursor;

            //Updates the invalidation rectangle to be a bit bigger to deal with overlaps
            Rectangle invalRect = Rectangle.Inflate(e.ClipRectangle, 5, 5);
            if (invalRect.X < 0) invalRect.X = 0;
            if (invalRect.Y < 0) invalRect.Y = 0;

            //We paint to a temporary buffer to avoid flickering
            Bitmap tempBuffer = new Bitmap(invalRect.Width, invalRect.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graph = Graphics.FromImage(tempBuffer);
            graph.TranslateTransform(-invalRect.X, -invalRect.Y);
            graph.SmoothingMode = _drawingQuality;

            //Fills the background with dark grey
            graph.FillRectangle(Brushes.DarkGray, invalRect);

            //This code draws the paper
            Stopwatch sw = new Stopwatch();
            sw.Start();
            RectangleF paperRect = PaperToScreen(0F, 0F, PaperWidth, PaperHeight);
            graph.FillRectangle(Brushes.White, paperRect.X, paperRect.Y, paperRect.Width , paperRect.Height);
            graph.DrawRectangle(Pens.Black, paperRect.X, paperRect.Y, paperRect.Width, paperRect.Height);
            if (_showMargin)
            {
                paperRect = PaperToScreen(_printerSettings.DefaultPageSettings.Margins.Left, _printerSettings.DefaultPageSettings.Margins.Top,
                    (PaperWidth - _printerSettings.DefaultPageSettings.Margins.Left - _printerSettings.DefaultPageSettings.Margins.Right),
                    (PaperHeight - _printerSettings.DefaultPageSettings.Margins.Top - _printerSettings.DefaultPageSettings.Margins.Bottom));
                graph.DrawRectangle(Pens.LightGray, paperRect.X, paperRect.Y, paperRect.Width, paperRect.Height);
            }
            sw.Stop();
            Debug.WriteLine("Time to draw paper: " + sw.ElapsedMilliseconds);

            //Draws the layout elements
            Region oldClip = graph.Clip;
            LayoutElement le;
            for (int i = LayoutElements.Count - 1; i >= 0; i--)
            {
                le = LayoutElements[i];

                //This code deals with drawins a map when its panning
                if (_mouseMode == MouseMode.PanMap && _selectedLayoutElements.Contains(le) && le is LayoutMap && _selectedLayoutElements.Count == 1)
                {
                    graph.TranslateTransform(_paperLocation.X + _mouseBox.Width, _paperLocation.Y + _mouseBox.Height);
                    graph.ScaleTransform(96F / 100F * _zoom, 96F / 100F * _zoom);
                    graph.Clip = new Region(le.Rectangle);
                    le.Draw(graph, false);
                    graph.ResetTransform();
                    graph.TranslateTransform(-invalRect.X, -invalRect.Y);
                    graph.Clip = oldClip;
                }
                //This code draws the selected elements
                else if (_selectedLayoutElements.Contains(LayoutElements[i]) && _resizeTempBitmap != null)
                {
                    RectangleF papRect = PaperToScreen(_selectedLayoutElements[0].Rectangle);
                    Rectangle clipRect = new Rectangle(Convert.ToInt32(papRect.X), Convert.ToInt32(papRect.Y), Convert.ToInt32(papRect.Width), Convert.ToInt32(papRect.Height));

                    //If its stretch to fit just scale it
                    if (_selectedLayoutElements[0].ResizeStyle == ResizeStyle.StretchToFit)
                        graph.DrawImage(_resizeTempBitmap, clipRect);

                    //If there is no scaling we just draw it with a clipping rectangle
                    else if (_selectedLayoutElements[0].ResizeStyle == ResizeStyle.NoScaling)
                    {
                        graph.Clip = new Region(clipRect);
                        graph.DrawImageUnscaled(_resizeTempBitmap, clipRect);
                        graph.Clip = oldClip;
                    }
                }
                else
                {
                    graph.TranslateTransform(_paperLocation.X, _paperLocation.Y);
                    graph.ScaleTransform(96F / 100F * _zoom, 96F / 100F * _zoom);
                    graph.Clip = new Region(le.Rectangle);
                    le.Draw(graph, false);
                    graph.ResetTransform();
                    graph.TranslateTransform(-invalRect.X, -invalRect.Y);
                    graph.Clip = oldClip;
                }
            }


            //Draws the selection rectangle around each selected item
            Pen selectionPen = new Pen(Color.Black,1F);
            selectionPen.DashPattern = new[] {2.0F, 1.0F};
            selectionPen.DashCap = DashCap.Round;
            foreach (LayoutElement layoutEl in _selectedLayoutElements)
            {
                RectangleF leRect = PaperToScreen(layoutEl.Rectangle);
                graph.DrawRectangle(selectionPen, Convert.ToInt32(leRect.X),Convert.ToInt32(leRect.Y), Convert.ToInt32(leRect.Width), Convert.ToInt32(leRect.Height));
            }
            
            //If the users is dragging a select box or an insert box we draw it here
            if (_mouseMode == MouseMode.CreateSelection || _mouseMode == MouseMode.InsertNewElement)
            {
                Color boxColor;
                if (_mouseMode == MouseMode.CreateSelection)
                    boxColor = SystemColors.Highlight;
                else
                    boxColor = Color.Orange;

                Pen outlinePen = new Pen(boxColor);
                SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(30, boxColor));
                graph.FillRectangle(highlightBrush, _mouseBox.X, _mouseBox.Y, _mouseBox.Width - 1, _mouseBox.Height - 1);
                graph.DrawRectangle(outlinePen, _mouseBox.X, _mouseBox.Y, _mouseBox.Width - 1, _mouseBox.Height - 1);

                //garbage collection
                highlightBrush.Dispose();
            }

            //Draws the temporary bitmap to the screen
            e.Graphics.SmoothingMode = _drawingQuality;
            e.Graphics.DrawImage(tempBuffer, invalRect, new RectangleF(0f,0f,invalRect.Width,invalRect.Height), GraphicsUnit.Pixel);

            //Garbage collection
            tempBuffer.Dispose();    
            graph.Dispose();

            //resets the cursor cuz some times it get jammed
            Cursor = oldCursor;

        }

       /// <summary>
       /// Prevents flicker from any default on paint background operations
       /// </summary>
       /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        #endregion

        #region ---------------- Event Handlers

        /// <summary>
        /// This gets fired when one of the layoutElements gets invalidated
        /// </summary>
        void LeInvalidated(object sender, EventArgs e)
        {
            if (_suppressLEinvalidate) return;
            Invalidate();
        }

        /// <summary>
        /// Fires whenever the LayoutControl is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LayoutControl_Resize(object sender, EventArgs e)
        {
            PointF paperTLPixel = PaperToScreen(new PointF(0, 0));
            PointF paperBRPixel = PaperToScreen(new PointF(PaperWidth, PaperHeight));
            SizeF paperSizeScreen = new SizeF(paperBRPixel.X - paperTLPixel.X, paperBRPixel.Y - paperTLPixel.Y);

            //Sets up the vertical scroll bars
            if (paperSizeScreen.Width <= (Width - _vScrollBar.Width - 4))
            {
                _paperLocation.X = (Width - _vScrollBar.Width - 4 - paperSizeScreen.Width) / 2F;
            }
            else
            {
                _paperLocation.X = 0;
            }

            //Sets up the horizontal scroll bar
            if (paperSizeScreen.Height <= (Height - _hScrollBar.Height - 4))
            {
                _paperLocation.Y = (Height - _hScrollBar.Height - 4 - paperSizeScreen.Height) / 2F;
            }
            else
            {
                _paperLocation.Y = 0;
            }

            UpdateScrollBars();

            //Invalidate the whole thing since we are moving this around
            Invalidate();
        }

        /// <summary>
        /// This fires when the vscrollbar is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _paperLocation.Y = _paperLocation.Y + (e.OldValue - e.NewValue);
            Invalidate();
        }

        /// <summary>
        /// This fires when the hscrollbar is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _paperLocation.X = _paperLocation.X + (e.OldValue - e.NewValue);
            Invalidate();
        }

        //This does stuff when a key is pressed
        void LayoutControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    DeleteSelected();
                    break;
                case Keys.F5:
                    RefreshElements();
                    break;
                default:
                    break;
            }
        }

        void LayoutControl_MouseDown(object sender, MouseEventArgs e)
        {
            //When the user clicks down we start tracking the mouses location
            _mouseStartPoint = new PointF(e.X, e.Y);
            _lastMousePoint = new PointF(e.X, e.Y);
            PointF mousePointPaper = ScreenToPaper(_mouseStartPoint);

            //Deals with left buttons clicks
            if (e.Button == MouseButtons.Left)
            {
                switch (_mouseMode)
                {
                    case MouseMode.Default:

                        //Handles resizing stuff
                        if (_resizeSelectedEdge != Edge.None)
                        {
                            _mouseMode = MouseMode.ResizeSelected;
                            _selectedLayoutElements[0].Resizing = true;
                            if (_selectedLayoutElements[0].ResizeStyle != ResizeStyle.HandledInternally)
                            {
                                RectangleF selecteScreenRect = PaperToScreen(_selectedLayoutElements[0].Rectangle);
                                _resizeTempBitmap = new Bitmap(Convert.ToInt32(selecteScreenRect.Width), Convert.ToInt32(selecteScreenRect.Height),System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                Graphics graph = Graphics.FromImage(_resizeTempBitmap);
                                graph.SmoothingMode = _drawingQuality;
                                graph.ScaleTransform(96F / 100F * _zoom, 96F / 100F * _zoom);
                                graph.TranslateTransform(-_selectedLayoutElements[0].Rectangle.X, -_selectedLayoutElements[0].Rectangle.Y);
                                _selectedLayoutElements[0].Draw(graph, false);
                                graph.Dispose();
                            }
                            return;
                        }

                        //Starts moving selected elements
                        if (ModifierKeys != Keys.Control)
                        {
                            foreach (LayoutElement le in _selectedLayoutElements)
                            {
                                if (le.IntersectsWith(mousePointPaper))
                                {
                                    _mouseMode = MouseMode.MoveSelection;
                                    Cursor = Cursors.SizeAll;
                                    return;
                                }
                            }
                        }

                        //Starts the selection code.
                        _mouseMode = MouseMode.CreateSelection;
                        _mouseBox = new RectangleF(e.X, e.Y, 0F, 0F);
                        break;

                    //Start drag rectangle insert new element
                    case MouseMode.StartInsertNewElement:
                        _mouseMode = MouseMode.InsertNewElement;
                        _mouseBox = new RectangleF(e.X, e.Y, 0F, 0F);
                        break;

                    //Starts the pan mode for the map
                    case MouseMode.StartPanMap:
                        _mouseMode = MouseMode.PanMap;
                        _mouseBox = new RectangleF(e.X, e.Y, 0F, 0F);
                        break;
                }
            }

            //Deals with right button clicks
            if (e.Button == MouseButtons.Right)
            {
                switch (_mouseMode)
                {
                    //If the user was in insert mode we cancel it
                    case(MouseMode.StartInsertNewElement):
                        _mouseMode = MouseMode.Default;
                        _elementToAddWithMouse = null;
                        Cursor = Cursors.Default;
                        break;
                }
            }
        }

        void LayoutControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Handles various different mouse modes
                switch (_mouseMode)
                {
                    //If we are dealing with a selection we look here
                    case MouseMode.CreateSelection:
                        PointF selectBoxTL = ScreenToPaper(_mouseBox.Location);
                        PointF selectBoxBR = ScreenToPaper(_mouseBox.Location.X + _mouseBox.Width, _mouseBox.Location.Y + _mouseBox.Height);
                        RectangleF selectBoxPaper = new RectangleF(selectBoxTL.X, selectBoxTL.Y, selectBoxBR.X - selectBoxTL.X, selectBoxBR.Y - selectBoxTL.Y);

                        if (ModifierKeys == Keys.Control)
                        {
                            foreach (LayoutElement le in _layoutElements)
                            {
                                if (le.IntersectsWith(selectBoxPaper))
                                {
                                    if (_selectedLayoutElements.Contains(le))
                                        _selectedLayoutElements.Remove(le);
                                    else
                                        _selectedLayoutElements.Add(le);
                                    //If the box is just a point only select the top most
                                    if (_mouseBox.Width <= 1 && _mouseBox.Height <= 1)
                                        break;
                                }
                            }
                        }
                        else
                        {
                            _selectedLayoutElements.Clear();
                            foreach (LayoutElement le in _layoutElements)
                            {
                                if (le.IntersectsWith(selectBoxPaper))
                                {
                                    _selectedLayoutElements.Add(le);
                                    //If the box is just a point only select the top most
                                    if (_mouseBox.Width <= 1 && _mouseBox.Height <= 1)
                                        break;
                                }
                            }
                        }
                        OnSelectionChanged(null);
                        _mouseMode = MouseMode.Default;
                        Invalidate();
                        break;

                    //Stops moving the selection
                    case MouseMode.MoveSelection:
                        _mouseMode = MouseMode.Default;
                        Cursor = Cursors.Default;
                        break;

                    //Turns of resize 
                    case MouseMode.ResizeSelected:
                        if (_resizeTempBitmap != null)
                            _resizeTempBitmap.Dispose();
                        _resizeTempBitmap = null;
                        _mouseMode = MouseMode.Default;
                        Cursor = Cursors.Default;
                        Invalidate(new Region(PaperToScreen(_selectedLayoutElements[0].Rectangle)));
                        _selectedLayoutElements[0].Resizing = false;
                        _selectedLayoutElements[0].Size = _selectedLayoutElements[0].Size;
                        break;

                    case MouseMode.InsertNewElement:
                        if (_mouseBox.Width == 0)
                            _mouseBox.Width = 200;                    
                        if (_mouseBox.Height == 0)
                            _mouseBox.Height = 100;
                        if (_mouseBox.Width < 0)
                        {
                            _mouseBox.X = _mouseBox.X + _mouseBox.Width;
                            _mouseBox.Width = -_mouseBox.Width;
                        }
                        if (_mouseBox.Height < 0)
                        {
                            _mouseBox.Y = _mouseBox.Y + _mouseBox.Height;
                            _mouseBox.Height = -_mouseBox.Height;
                        }
                        _elementToAddWithMouse.Rectangle = ScreenToPaper(_mouseBox);
                        AddToLayout(_elementToAddWithMouse);
                        AddToSelection(_elementToAddWithMouse);
                        _elementToAddWithMouse = null;
                        _mouseMode = MouseMode.Default;
                        _mouseBox.Inflate(5, 5);
                        Invalidate(new Region(_mouseBox));
                        break;

                    case MouseMode.PanMap:
                        if (_mouseBox.Width != 0 || _mouseBox.Height != 0)
                            PanMap(_selectedLayoutElements[0] as LayoutMap, _mouseBox.Width, _mouseBox.Height);
                        _mouseMode = MouseMode.StartPanMap;
                        break;

                    case MouseMode.Default:
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                switch (_mouseMode)
                {
                    case MouseMode.Default:
                        if (_selectedLayoutElements.Count < 1)
                        {
                            for (int i = 0; i < _contextMenuRight.MenuItems.Count; i++)
                                _contextMenuRight.MenuItems[i].Enabled = false;
                        }
                        else if (_selectedLayoutElements.Count == 1)
                        {
                            _cMnuSelAli.Enabled = false;
                            _cMnuSelFit.Enabled = false;
                        }
                        _contextMenuRight.Show(this, e.Location);
                        for (int i = 0; i < _contextMenuRight.MenuItems.Count; i++)
                            _contextMenuRight.MenuItems[i].Enabled = true;
                        break;
                }   
            }
        }

        void LayoutControl_MouseMove(object sender, MouseEventArgs e)
        {
            //The amount the mouse moved since the last time
            float deltaX = _lastMousePoint.X - e.X;
            float deltaY = _lastMousePoint.Y - e.Y;
            _lastMousePoint = e.Location;

            //Handles various different mouse modes
            switch (_mouseMode)
            {
                //Deals with inserting new elements
                case MouseMode.InsertNewElement:

                //Deals with creating a selections
                case MouseMode.CreateSelection:
                    Invalidate(new Region(_mouseBox));
                    _mouseBox.Width = Math.Abs(_mouseStartPoint.X - e.X);
                    _mouseBox.Height = Math.Abs(_mouseStartPoint.Y - e.Y);
                    _mouseBox.X = Math.Min(_mouseStartPoint.X, e.X);
                    _mouseBox.Y = Math.Min(_mouseStartPoint.Y, e.Y);
                    Invalidate(new Region(_mouseBox));
                    break;
                
                //Deals with moving the selection
                case MouseMode.MoveSelection:
                    _suppressLEinvalidate = true;
                    foreach (LayoutElement le in _selectedLayoutElements)
                    {
                        RectangleF invalRect = PaperToScreen(le.Rectangle);
                        invalRect.Inflate(5F, 5F);
                        Invalidate(new Region(invalRect));
                        PointF elementLocScreen = PaperToScreen(le.LocationF);
                        le.LocationF = ScreenToPaper(elementLocScreen.X - deltaX, elementLocScreen.Y - deltaY);
                        invalRect = PaperToScreen(le.Rectangle);
                        invalRect.Inflate(5F, 5F);
                        Invalidate(new Region(invalRect));
                        Update();
                    }
                    _suppressLEinvalidate = false;
                    break;

                //This handle mouse movement when in resize mode
                case MouseMode.ResizeSelected:
                    _suppressLEinvalidate = true;
                    RectangleF oldScreenRect = PaperToScreen(_selectedLayoutElements[0].Rectangle);
                    oldScreenRect.Inflate(5F, 5F);
                    Invalidate(new Region(oldScreenRect));
                    oldScreenRect = PaperToScreen(_selectedLayoutElements[0].Rectangle);
                    switch (_resizeSelectedEdge)
                    {
                        case Edge.TopLeft:
                            oldScreenRect.X = oldScreenRect.X - deltaX;
                            oldScreenRect.Y = oldScreenRect.Y - deltaY;
                            oldScreenRect.Width = oldScreenRect.Width + deltaX;
                            oldScreenRect.Height = oldScreenRect.Height + deltaY;
                            break;
                        case Edge.Top:
                            oldScreenRect.Y = oldScreenRect.Y - deltaY;
                            oldScreenRect.Height = oldScreenRect.Height + deltaY;
                            break;
                        case Edge.TopRight:
                            oldScreenRect.Y = oldScreenRect.Y - deltaY;
                            oldScreenRect.Height = oldScreenRect.Height + deltaY;
                            oldScreenRect.Width = oldScreenRect.Width - deltaX;
                            break;
                        case Edge.Right:
                            oldScreenRect.Width = oldScreenRect.Width - deltaX;
                            break;
                        case Edge.BottomRight:
                            oldScreenRect.Width = oldScreenRect.Width - deltaX;
                            oldScreenRect.Height = oldScreenRect.Height - deltaY;
                            break;
                        case Edge.Bottom:
                            oldScreenRect.Height = oldScreenRect.Height - deltaY;
                            break;
                        case Edge.BottomLeft:
                            oldScreenRect.X = oldScreenRect.X - deltaX;
                            oldScreenRect.Width = oldScreenRect.Width + deltaX;
                            oldScreenRect.Height = oldScreenRect.Height - deltaY;
                            break;
                        case Edge.Left:
                            oldScreenRect.X = oldScreenRect.X - deltaX;
                            oldScreenRect.Width = oldScreenRect.Width + deltaX;
                            break;
                    }
                    _selectedLayoutElements[0].Rectangle = ScreenToPaper(oldScreenRect);
                     oldScreenRect.Inflate(5F, 5F);
                    Invalidate(new Region(oldScreenRect));
                    Update();
                    _suppressLEinvalidate = false;
                    break;

                case MouseMode.StartPanMap:
                    if (_selectedLayoutElements.Count == 1 && _selectedLayoutElements[0] is LayoutMap)
                    {
                        if (_selectedLayoutElements[0].IntersectsWith(ScreenToPaper(e.X * 1F, e.Y * 1F)))
                            Cursor = new Cursor(Images.Pan.Handle);
                        else
                            Cursor = Cursors.Default;
                    }
                    break;

                case MouseMode.PanMap:
                    _mouseBox.Width = e.X - _mouseStartPoint.X;
                    _mouseBox.Height = e.Y - _mouseStartPoint.Y;
                    Invalidate(new Region(PaperToScreen(_selectedLayoutElements[0].Rectangle)));
                    break;

                case MouseMode.Default:
                    
                    //If theres only one element selected and were on its edge change the cursor to the resize cursor
                    if (_selectedLayoutElements.Count == 1)
                    {
                        _resizeSelectedEdge = IntersectElementEdge(PaperToScreen(_selectedLayoutElements[0].Rectangle), new PointF(e.X, e.Y), 3F);
                        switch (_resizeSelectedEdge)
                        {
                            case Edge.TopLeft:
                            case Edge.BottomRight:
                                Cursor = Cursors.SizeNWSE;
                                break;
                            case Edge.Top:
                            case Edge.Bottom:
                                Cursor = Cursors.SizeNS;
                                break;
                            case Edge.TopRight:
                            case Edge.BottomLeft:
                                Cursor = Cursors.SizeNESW;
                                break;
                            case Edge.Left:
                            case Edge.Right:
                                Cursor = Cursors.SizeWE;
                                break;
                            case Edge.None:
                                Cursor = Cursors.Default;
                                break;
                        }
                    }
                    break;
            }
        }
       
        #endregion

        #region ---------------- Event Triggers

        /// <summary>
        /// Calls this to indicate the filename has been changed
        /// </summary>
        /// <param name="e"></param>
        private void OnFilenameChanged(EventArgs e)
        {
            if (FilenameChanged != null)
                FilenameChanged(this, e);
        }

        /// <summary>
        /// Calls this to indicate the zoom has been changed
        /// </summary>
        /// <param name="e"></param>
        private void OnZoomChanged(EventArgs e)
        {
            if (ZoomChanged != null)
                ZoomChanged(this, e);
        }

        /// <summary>
        /// Call this to indicate the selection has changed
        /// </summary>
        /// <param name="e"></param>
        private void OnSelectionChanged(EventArgs e)
        {
            if (_layoutMapToolStrip != null)
            {
                if (_selectedLayoutElements.Count == 1 && _selectedLayoutElements[0] is LayoutMap)
                    _layoutMapToolStrip.Enabled = true;
                else
                {
                    _layoutMapToolStrip.Enabled = false;
                    if (_mouseMode == MouseMode.StartPanMap || _mouseMode == MouseMode.PanMap)
                        _mouseMode = MouseMode.Default;
                }
            }

            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        /// <summary>
        /// Call this to indicate elements were added or removed
        /// </summary>
        /// <param name="e"></param>
        private void OnElementsChanged(EventArgs e)
        {
            if (ElementsChanged != null)
                ElementsChanged(this, e);
        }

        #endregion

        #region ---------------- Context Menu

        private void cMnuMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectionUp();
        }

        private void cMnuMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectionDown();
        }

        private void cMnuDelete_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void cMnuSelLeft_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Left, false);
        }

        private void cMnuSelRight_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Right, false);
        }

        private void cMnuMargLeft_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Left, true);
        }

        private void cMnuMargRight_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Right, true);
        }

        private void cMnuMargTop_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Top, true);
        }

        private void cMnuMargBottom_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Bottom, true);
        }

        private void cMnuSelTop_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Top, false);
        }

        private void cMnuSelBottom_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Bottom, false);
        }

        private void cMnuSelHor_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Horizontal, false);
        }

        private void cMnuSelVert_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Vertical, false);
        }

        private void cMnuMargHor_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Horizontal, true);
        }

        private void cMnuMargVert_Click(object sender, EventArgs e)
        {
            AlignElements(_selectedLayoutElements, Alignments.Vertical, true);
        }

        private void cMnuSelWidth_Click(object sender, EventArgs e)
        {
            MatchElementsSize(_selectedLayoutElements, Fit.Width, false);
            AlignElements(_selectedLayoutElements, Alignments.Horizontal, false);
        }

        private void cMnuSelHeight_Click(object sender, EventArgs e)
        {
            MatchElementsSize(_selectedLayoutElements, Fit.Height, false);
            AlignElements(_selectedLayoutElements, Alignments.Vertical, false);
        }

        private void cMnuMargWidth_Click(object sender, EventArgs e)
        {
            MatchElementsSize(_selectedLayoutElements, Fit.Width, true);
            AlignElements(_selectedLayoutElements, Alignments.Horizontal, true);
        }

        private void cMnuMargHeight_Click(object sender, EventArgs e)
        {
            MatchElementsSize(_selectedLayoutElements, Fit.Height, true);
            AlignElements(_selectedLayoutElements, Alignments.Vertical, true);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControl));
            this._hScrollBarPanel = new System.Windows.Forms.Panel();
            this._hScrollBar = new System.Windows.Forms.HScrollBar();
            this._vScrollBar = new System.Windows.Forms.VScrollBar();
            this._contextMenuRight = new System.Windows.Forms.ContextMenu();
            this._cMnuMoveUp = new System.Windows.Forms.MenuItem();
            this._cMnuMoveDown = new System.Windows.Forms.MenuItem();
            this._menuItem2 = new System.Windows.Forms.MenuItem();
            this._cMnuSelAli = new System.Windows.Forms.MenuItem();
            this._cMnuSelLeft = new System.Windows.Forms.MenuItem();
            this._cMnuSelRight = new System.Windows.Forms.MenuItem();
            this._cMnuSelTop = new System.Windows.Forms.MenuItem();
            this._cMnuSelBottom = new System.Windows.Forms.MenuItem();
            this._cMnuSelHor = new System.Windows.Forms.MenuItem();
            this._cMnuSelVert = new System.Windows.Forms.MenuItem();
            this._cMnuMarAli = new System.Windows.Forms.MenuItem();
            this._cMnuMargLeft = new System.Windows.Forms.MenuItem();
            this._cMnuMargRight = new System.Windows.Forms.MenuItem();
            this._cMnuMargTop = new System.Windows.Forms.MenuItem();
            this._cMnuMargBottom = new System.Windows.Forms.MenuItem();
            this._cMnuMargHor = new System.Windows.Forms.MenuItem();
            this._cMnuMargVert = new System.Windows.Forms.MenuItem();
            this._menuItem19 = new System.Windows.Forms.MenuItem();
            this._cMnuSelFit = new System.Windows.Forms.MenuItem();
            this._cMnuSelWidth = new System.Windows.Forms.MenuItem();
            this._cMnuSelHeight = new System.Windows.Forms.MenuItem();
            this._cMnuMarFit = new System.Windows.Forms.MenuItem();
            this._cMnuMargWidth = new System.Windows.Forms.MenuItem();
            this._cMnuMargHeight = new System.Windows.Forms.MenuItem();
            this._menuItem4 = new System.Windows.Forms.MenuItem();
            this._cMnuDelete = new System.Windows.Forms.MenuItem();
            this._hScrollBarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _hScrollBarPanel
            // 
            this._hScrollBarPanel.Controls.Add(this._hScrollBar);
            resources.ApplyResources(this._hScrollBarPanel, "_hScrollBarPanel");
            this._hScrollBarPanel.Name = "_hScrollBarPanel";
            // 
            // _hScrollBar
            // 
            resources.ApplyResources(this._hScrollBar, "_hScrollBar");
            this._hScrollBar.Name = "_hScrollBar";
            this._hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
            // 
            // _vScrollBar
            // 
            resources.ApplyResources(this._vScrollBar, "_vScrollBar");
            this._vScrollBar.Name = "_vScrollBar";
            this._vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // _contextMenuRight
            // 
            this._contextMenuRight.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._cMnuMoveUp,
            this._cMnuMoveDown,
            this._menuItem2,
            this._cMnuSelAli,
            this._cMnuMarAli,
            this._menuItem19,
            this._cMnuSelFit,
            this._cMnuMarFit,
            this._menuItem4,
            this._cMnuDelete});
            // 
            // _cMnuMoveUp
            // 
            this._cMnuMoveUp.Index = 0;
            this._cMnuMoveUp.Text = global::MapWindow.MessageStrings.LayoutMenuStripSelectMoveUp;
            this._cMnuMoveUp.Click += new System.EventHandler(this.cMnuMoveUp_Click);
            // 
            // _cMnuMoveDown
            // 
            this._cMnuMoveDown.Index = 1;
            this._cMnuMoveDown.Text = global::MapWindow.MessageStrings.LayoutMenuStripSelectMoveDown;
            this._cMnuMoveDown.Click += new System.EventHandler(this.cMnuMoveDown_Click);
            // 
            // _menuItem2
            // 
            this._menuItem2.Index = 2;
            resources.ApplyResources(this._menuItem2, "_menuItem2");
            // 
            // _cMnuSelAli
            // 
            this._cMnuSelAli.Index = 3;
            this._cMnuSelAli.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._cMnuSelLeft,
            this._cMnuSelRight,
            this._cMnuSelTop,
            this._cMnuSelBottom,
            this._cMnuSelHor,
            this._cMnuSelVert});
            this._cMnuSelAli.Text = global::MapWindow.MessageStrings.LayoutCmnuSelectionAlignment;
            // 
            // _cMnuSelLeft
            // 
            this._cMnuSelLeft.Index = 0;
            this._cMnuSelLeft.Text = global::MapWindow.MessageStrings.LayoutCmnuLeft;
            this._cMnuSelLeft.Click += new System.EventHandler(this.cMnuSelLeft_Click);
            // 
            // _cMnuSelRight
            // 
            this._cMnuSelRight.Index = 1;
            this._cMnuSelRight.Text = global::MapWindow.MessageStrings.LayoutCmnuRight;
            this._cMnuSelRight.Click += new System.EventHandler(this.cMnuSelRight_Click);
            // 
            // _cMnuSelTop
            // 
            this._cMnuSelTop.Index = 2;
            this._cMnuSelTop.Text = global::MapWindow.MessageStrings.LayoutCmnuTop;
            this._cMnuSelTop.Click += new System.EventHandler(this.cMnuSelTop_Click);
            // 
            // _cMnuSelBottom
            // 
            this._cMnuSelBottom.Index = 3;
            this._cMnuSelBottom.Text = global::MapWindow.MessageStrings.LayoutCmnuBottom;
            this._cMnuSelBottom.Click += new System.EventHandler(this.cMnuSelBottom_Click);
            // 
            // _cMnuSelHor
            // 
            this._cMnuSelHor.Index = 4;
            this._cMnuSelHor.Text = global::MapWindow.MessageStrings.LayoutCmnuHor;
            this._cMnuSelHor.Click += new System.EventHandler(this.cMnuSelHor_Click);
            // 
            // _cMnuSelVert
            // 
            this._cMnuSelVert.Index = 5;
            this._cMnuSelVert.Text = global::MapWindow.MessageStrings.LayoutCmnuVert;
            this._cMnuSelVert.Click += new System.EventHandler(this.cMnuSelVert_Click);
            // 
            // _cMnuMarAli
            // 
            this._cMnuMarAli.Index = 4;
            this._cMnuMarAli.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._cMnuMargLeft,
            this._cMnuMargRight,
            this._cMnuMargTop,
            this._cMnuMargBottom,
            this._cMnuMargHor,
            this._cMnuMargVert});
            this._cMnuMarAli.Text = global::MapWindow.MessageStrings.LayoutCmnuMargAlign;
            // 
            // _cMnuMargLeft
            // 
            this._cMnuMargLeft.Index = 0;
            this._cMnuMargLeft.Text = global::MapWindow.MessageStrings.LayoutCmnuLeft;
            this._cMnuMargLeft.Click += new System.EventHandler(this.cMnuMargLeft_Click);
            // 
            // _cMnuMargRight
            // 
            this._cMnuMargRight.Index = 1;
            this._cMnuMargRight.Text = global::MapWindow.MessageStrings.LayoutCmnuRight;
            this._cMnuMargRight.Click += new System.EventHandler(this.cMnuMargRight_Click);
            // 
            // _cMnuMargTop
            // 
            this._cMnuMargTop.Index = 2;
            this._cMnuMargTop.Text = global::MapWindow.MessageStrings.LayoutCmnuTop;
            this._cMnuMargTop.Click += new System.EventHandler(this.cMnuMargTop_Click);
            // 
            // _cMnuMargBottom
            // 
            this._cMnuMargBottom.Index = 3;
            this._cMnuMargBottom.Text = global::MapWindow.MessageStrings.LayoutCmnuBottom;
            this._cMnuMargBottom.Click += new System.EventHandler(this.cMnuMargBottom_Click);
            // 
            // _cMnuMargHor
            // 
            this._cMnuMargHor.Index = 4;
            this._cMnuMargHor.Text = global::MapWindow.MessageStrings.LayoutCmnuHor;
            this._cMnuMargHor.Click += new System.EventHandler(this.cMnuMargHor_Click);
            // 
            // _cMnuMargVert
            // 
            this._cMnuMargVert.Index = 5;
            this._cMnuMargVert.Text = global::MapWindow.MessageStrings.LayoutCmnuVert;
            this._cMnuMargVert.Click += new System.EventHandler(this.cMnuMargVert_Click);
            // 
            // _menuItem19
            // 
            this._menuItem19.Index = 5;
            resources.ApplyResources(this._menuItem19, "_menuItem19");
            // 
            // _cMnuSelFit
            // 
            this._cMnuSelFit.Index = 6;
            this._cMnuSelFit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._cMnuSelWidth,
            this._cMnuSelHeight});
            this._cMnuSelFit.Text = global::MapWindow.MessageStrings.LayoutCmnuSelectionFit;
            // 
            // _cMnuSelWidth
            // 
            this._cMnuSelWidth.Index = 0;
            this._cMnuSelWidth.Text = global::MapWindow.MessageStrings.LayoutCmnuWidth;
            this._cMnuSelWidth.Click += new System.EventHandler(this.cMnuSelWidth_Click);
            // 
            // _cMnuSelHeight
            // 
            this._cMnuSelHeight.Index = 1;
            this._cMnuSelHeight.Text = global::MapWindow.MessageStrings.LayoutCmnuHeight;
            this._cMnuSelHeight.Click += new System.EventHandler(this.cMnuSelHeight_Click);
            // 
            // _cMnuMarFit
            // 
            this._cMnuMarFit.Index = 7;
            this._cMnuMarFit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._cMnuMargWidth,
            this._cMnuMargHeight});
            this._cMnuMarFit.Text = global::MapWindow.MessageStrings.LayoutCmnuMarginFit;
            // 
            // _cMnuMargWidth
            // 
            this._cMnuMargWidth.Index = 0;
            this._cMnuMargWidth.Text = global::MapWindow.MessageStrings.LayoutCmnuWidth;
            this._cMnuMargWidth.Click += new System.EventHandler(this.cMnuMargWidth_Click);
            // 
            // _cMnuMargHeight
            // 
            this._cMnuMargHeight.Index = 1;
            this._cMnuMargHeight.Text = global::MapWindow.MessageStrings.LayoutCmnuHeight;
            this._cMnuMargHeight.Click += new System.EventHandler(this.cMnuMargHeight_Click);
            // 
            // _menuItem4
            // 
            this._menuItem4.Index = 8;
            resources.ApplyResources(this._menuItem4, "_menuItem4");
            // 
            // _cMnuDelete
            // 
            this._cMnuDelete.Index = 9;
            this._cMnuDelete.Text = global::MapWindow.MessageStrings.LayoutMenuStripSelectDelete;
            this._cMnuDelete.Click += new System.EventHandler(this.cMnuDelete_Click);
            // 
            // LayoutControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._vScrollBar);
            this.Controls.Add(this._hScrollBarPanel);
            this.Name = "LayoutControl";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LayoutControl_MouseMove);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LayoutControl_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LayoutControl_MouseDown);
            this.Resize += new System.EventHandler(this.LayoutControl_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LayoutControl_MouseUp);
            this._hScrollBarPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
