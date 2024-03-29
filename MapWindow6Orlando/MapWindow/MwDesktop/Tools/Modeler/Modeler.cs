﻿//********************************************************************************************************
// Product Name: MapWindow.Tools.Modeler
// Description:  Creats and displays models
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
// The Original Code is Toolbox.dll for the MapWindow 4.6/6 ToolManager project
//
// The Initial Developer of this Original Code is Brian Marchionni. Created in Nov, 2008.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
// ---------------------|------------|--------------------------------------------------------------------
// Ted Dunsford         | 8/28/2009  | Cleaned up some code using re-sharper
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using MapWindow.Tools.Param;

namespace MapWindow.Tools
{

    /// <summary>
    /// A modeler form which allows users to create models with visual representations of tools
    /// </summary>
    public class Modeler : UserControl
    {
        #region ------------------- Class Variables

        //Decides if the the MapWindow watermark is present in the lowerleft corner
        private bool _showWaterMark = true;
        
        //The tool Manager associated with the modeler
        private ToolManager _toolManager;
        
        //These variables are used for drawing
        private Bitmap _backBuffer;
        private bool _isInitialized;

        //These lists contain all the selected and unselected elements in the model
        private readonly List<ModelElement> _modelElements = new List<ModelElement>();            //All the model elements, selected or otherwise
        private readonly List<ModelElement> _modelElementsSelected = new List<ModelElement>();    //All the selected model elements

        //Used for mouse movements
        private bool _mouseMoveDone = true;
        private bool _mouseMoved;
        private bool _mouseDownOnElement;
        private Point _mouseOldPoint;

        //Used for drawing the selection box when a user left clicks and drags
        private bool _selectBoxDraw;
        private Rectangle _selectBox;
        private Point _selectBoxPt;

        //These variables define how the tool elements of the model are drawn
        private ModelShapes _toolShape = ModelShapes.Rectangle;
        private Color _toolColor = Color.Khaki;
        private Font _toolFont = SystemFonts.DialogFont;

        //These variables define how the data elements of the model are drawn
        private ModelShapes _dataShape = ModelShapes.Ellipse;
        private Color _dataColor = Color.LightGreen;
        private Font _dataFont = SystemFonts.DialogFont;

        //These variables define how the data elements of the model are drawn
        private readonly Color _arrowColor = Color.Black;
        
        //Scrollbar/Zoom stuff
        private Panel _panelHScroll;
        private HScrollBar _horScroll;
        private int _horLastValue;
        private VScrollBar _verScroll;
        private int _verLastValue;
        private float _virtualZoom; //The zoom factor of the model 
        private Point _virtualOffset; //The location of the model origin in the form
        private readonly Timer _scrollTimerHorizontal;
        private readonly Timer _scrollTimerVertical;
        private Boolean _scrollLock;

        //Enables and disables linking
        private Boolean _enableLinking;
        private ArrowElement _linkArrow;

        //Right click context menu
        private ContextMenu _contMenuRc;
        private MenuItem _menuItem1;

        //Used for loading and saving models
        private string _modelFilename;
        private string _defaultFileExtension;
        private string _workingPath;
        private bool _newModel;

        //Execution variables
        private int _maxExecutionThreads;
        private int _toolToExeCount;

        private System.Drawing.Drawing2D.SmoothingMode _drawingQuality;

        #endregion

        #region ------------------- EvenHandlers

        /// <summary>
        /// Occurs when the model filename has changed
        /// </summary>
        public event EventHandler ModelFilenameChanged;

        #endregion

        #region ------------------- Constructor

        /// <summary>
        /// Creates an instance of the spatial modeler Element
        /// </summary>
        public Modeler()
        {
            InitializeComponent();

            //Sets up the scroll bars
            _verScroll.Minimum = 0;
            _verScroll.Maximum = Height -16;
            _verScroll.Value = Height / 2;
            _verLastValue = _verScroll.Value;
            _verScroll.LargeChange = 50;
            _verScroll.SmallChange = 10;
            _verScroll.ValueChanged += VerScrollValueChanged;
            _horScroll.Minimum = 0;
            _horScroll.Maximum = Width -16;
            _horScroll.Value = Width / 2;
            _horLastValue = _horScroll.Value;
            _horScroll.LargeChange = 50;
            _horScroll.SmallChange = 10;
            _horScroll.ValueChanged += HorScrollValueChanged;
            _scrollTimerHorizontal = new Timer();
            _scrollTimerHorizontal.Interval = 100;
            _scrollTimerHorizontal.Tick += ScrollTimerHorizontalTick;
            _scrollTimerVertical = new Timer();
            _scrollTimerVertical.Interval = 100;
            _scrollTimerVertical.Tick += ScrollTimerVerticalTick;
            _scrollLock = false;

            //Sets up the zoom and offset
            _virtualZoom = 1F;
            _virtualOffset = new Point(0, 0);

            //Sets the default project extension
            _defaultFileExtension = "mwm";

            //Default number of simulatenous tool execution threads
            _maxExecutionThreads = 2;
        }

        #endregion

        #region ------------------- ScrollBars

        void ScrollTimerHorizontalTick(object sender, EventArgs e)
        {
            _scrollLock = false;
            _horScroll.Minimum = _horScroll.Minimum - 50;
            _horScroll.Maximum = _horScroll.Maximum + 50;
            _scrollTimerHorizontal.Stop();
        }

        void ScrollTimerVerticalTick(object sender, EventArgs e)
        {
            _scrollLock = false;
            _verScroll.Minimum = _verScroll.Minimum - 50;
            _verScroll.Maximum = _verScroll.Maximum + 50;
            _scrollTimerVertical.Stop();
        }

        void HorScrollValueChanged(object sender, EventArgs e)
        {
            _virtualOffset.X = _virtualOffset.X - (_horScroll.Value - _horLastValue);
            _horLastValue = _horScroll.Value;
            if ((_horScroll.Value <= _horScroll.Minimum) && (_horScroll.Minimum - 50 > (int.MinValue + 50)) && _scrollLock == false)
            {
                _scrollLock = true;
                _scrollTimerHorizontal.Start();
                _horScroll.Minimum = _horScroll.Minimum - 50;
            }
            if ((_horScroll.Value + _horScroll.LargeChange >= _horScroll.Maximum) && (_horScroll.Maximum + 50 < (int.MaxValue - 50)) && _scrollLock == false)
            {
                _scrollLock = true;
                _scrollTimerHorizontal.Start();
                _horScroll.Maximum = _horScroll.Maximum + 50;
            }
            IsInitialized = false;
            Invalidate();
        }

        void VerScrollValueChanged(object sender, EventArgs e)
        {
            _virtualOffset.Y = _virtualOffset.Y - (_verScroll.Value - _verLastValue);
            _verLastValue = _verScroll.Value;
            if ((_verScroll.Value <= _verScroll.Minimum) && (_verScroll.Minimum - 50 > (int.MinValue + 50)) && _scrollLock == false)
            {
                _scrollLock = true;
                _scrollTimerVertical.Start();
                _verScroll.Minimum = _verScroll.Minimum - 50;
            }
            if ((_verScroll.Value + _verScroll.LargeChange >= _verScroll.Maximum) && (_verScroll.Maximum + 50 < (int.MaxValue - 50)) && _scrollLock == false)
            {
                _scrollLock = true;
                _scrollTimerVertical.Start();
                _verScroll.Maximum = _verScroll.Maximum + 50;
            }
            IsInitialized = false;
            Invalidate();
        }

        #endregion

        #region ------------------- Properties

        /// <summary>
        /// The maximum number of processes to execute at the same time
        /// </summary>
        public int MaxExecutionThreads
        {
          get { return _maxExecutionThreads; }
          set { _maxExecutionThreads = value; }
        }

        /// <summary>
        /// Turns linking by mouse on and off
        /// </summary>
        public Boolean EnableLinking
        {
            get { return _enableLinking; }
            set 
            { 
                _enableLinking = value;
                if (_enableLinking)
                {
                    Cursor = Cursors.Cross;
                    ClearSelectedElements();
                }
                else
                    Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Gets or Sets the drawing quality for the toolbox
        /// </summary>
        [Category("Model Appearance"), Description("Gets or Sets the drawing quality for the toolbox")]
        public System.Drawing.Drawing2D.SmoothingMode DrawingQuality
        {
            get { return _drawingQuality; }
            set { _drawingQuality = value; }
        }

        /// <summary>
        /// Gets or sets the font that will be used to label the data in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the font that will be used to label the data in the modeler")]
        public Font DataFont
        {
            get { return _dataFont; }
            set { _dataFont = value; }
        }

        /// <summary>
        /// Gets or sets the color that data should have in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the color that data should have in the modeler")]
        public Color DataColor
        {
            get { return _dataColor; }
            set { _dataColor = value; }
        }

        /// <summary>
        /// Gets or sets the shape that data should be represented with in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the shape that data should be represented with in the modeler")]
        public ModelShapes DataShape
        {
            get { return _dataShape; }
            set { _dataShape = value; }
        }

        /// <summary>
        /// Gets or sets the font that will be used to label the tools in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the font that will be used to label the tools in the modeler")]
        public Font ToolFont
        {
            get { return _toolFont; }
            set 
            { 
                foreach(ModelElement modelEl in _modelElements)
                {
                    if (modelEl as ToolElement != null)
                        modelEl.Font = value;
                }
                _toolFont = value; 
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Gets or sets the color that tools should have in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the color that tools should be represented with in the modeler")]
        public Color ToolColor
        {
            get { return _toolColor; }
            set 
            { 
                foreach(ModelElement modelEl in _modelElements)
                {
                    if (modelEl as ToolElement != null)
                        modelEl.Color = value;
                }
                _toolColor = value; 
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Gets or sets the shape that tools should be represented with in the modeler
        /// </summary>
        [Category("Model Appearance"), Description("Gets or sets the shape that tools should be represented with in the modeler")]
        public ModelShapes ToolShape
        {
            get { return _toolShape; }
            set
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                foreach (ModelElement modelEl in _modelElements)
                {
                    if (modelEl as ToolElement != null)
                        modelEl.Shape = value;
                }
                sw.Stop();
                Debug.WriteLine("my sw: " + sw.ElapsedMilliseconds);

                _toolShape = value;
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Turns the MapWindow watermark in the lower right hand corner on or off. Defautl true.
        /// </summary>
        [Category("Model Appearance"), Description("Turns the MapWindow watermark in the lower right hand corner on or off")]
        public bool ShowWaterMark
        {
            get { return _showWaterMark; }
            set
            {
                _showWaterMark = value;
                Refresh();
            }
        }

        /// <summary>
        /// Sets the toolManager used to create instances of all the tools.
        /// </summary>
        [Category("Model Appearance"), Description("Sets the toolManager used to create instances of all the tools.")]
        public ToolManager ToolManager
        {
            get { return _toolManager; }
            set { _toolManager = value; }
        }

        /// <summary>
        /// Gets or Sets if the model drawing needs to be initialized
        /// </summary>
        [Category("Model Appearance"), Description("Gets or Sets if the model drawing needs to be initialized (This must always be true at design time)")]
        public bool IsInitialized
        {
            get { return _isInitialized;}
            set 
            {
                _isInitialized = value;
                if(_isInitialized == false)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the zoom factor of the map values below 0 zoom out, values above 1 zoom in. 1 = no zoom
        /// </summary>
        public float ZoomFactor
        {
            get { return _virtualZoom; }
            set
            {
                if (value < 0.0005F)
                    _virtualZoom = 0.0005F;
                else
                    _virtualZoom = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the filename of the current model
        /// </summary>
        public string ModelFilename
        {
            get { return _modelFilename; }
            set 
            {
                _modelFilename = value;
                OnModelFilenameChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the extension used by default for opening, saving and creating new models. ex "mwm"
        /// </summary>
        public string DefaultFileExtension
        {
            get { return _defaultFileExtension; }
            set { _defaultFileExtension= value;}
        }

        /// <summary>
        /// Gets or sets the working path for the model
        /// </summary>
        public string WorkingPath
        {
            get { return _workingPath; }
            set { _workingPath = value; }
        }

        #endregion

        #region ------------------- Private Methods

        /// <summary>
        /// Translates between pixel location and virtual location
        /// </summary>
        /// <param name="pt">A point in pixel coordinantes. 0,0 is the top left of the modeler</param>
        /// <returns>A point in the virtual model coordinantes</returns>
        private Point PixelToVirtual(Point pt)
        {
            return(PixelToVirtual(pt.X, pt.Y));
        }

        /// <summary>
        /// Translates between pixel location and virtual location
        /// </summary>
        /// <param name="x">the X location in pixel coordinantes</param>
        /// <param name="y">the Y location in pixel coordinantes</param>
        /// <returns>A point in the virtual model coordinantes</returns>
        private Point PixelToVirtual(int x, int y)
        {
            int ptX = Convert.ToInt32((x / _virtualZoom - _virtualOffset.X / _virtualZoom));
            int ptY = Convert.ToInt32((y / _virtualZoom - _virtualOffset.Y / _virtualZoom));
            return (new Point(ptX, ptY));
        }

        /// <summary>
        /// Returns a rectangle in virtual coordinantes based on a pixel rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Rectangle PixelRectToVirtualRect(Rectangle rect)
        {
            Point tl = PixelToVirtual(rect.Location);
            Point BR;
            BR = PixelToVirtual(rect.Location.X + rect.Width, rect.Location.Y + rect.Height);
            return (new Rectangle(tl.X, tl.Y, BR.X - tl.X, BR.Y - tl.Y));
        }

        /// <summary>
        /// Translates between virtual model coords and pixels
        /// </summary>
        /// <param name="pt">A point in the virtual model coordinantes</param>
        /// <returns>A point in pixel coordinantes. 0,0 is the top left of the modeler</returns>
        private Point VirtualToPixel(Point pt)
        {
            return(VirtualToPixel(pt.X, pt.Y));
        }

        /// <summary>
        /// Translates between virtual model coords and pixels
        /// </summary>
        /// <param name="x">the X location in virtual model coordinantes</param>
        /// <param name="y">the Y location in virtual model coordinantes</param>
        /// <returns>A point in pixel coordinantes. 0,0 is the top left of the modeler</returns>
        private Point VirtualToPixel(int x, int y)
        {
            int ptX = Convert.ToInt32((x * _virtualZoom) + _virtualOffset.X);
            int ptY = Convert.ToInt32((y * _virtualZoom) + _virtualOffset.Y);
            return (new Point(ptX, ptY));
        }

        /// <summary>
        /// Adds a data element to the modeler based on a parameter descrition
        /// </summary>
        /// <param name="par">The parameter to add to the modeler</param>
        /// <param name="location">A point representing the virtual location of the element</param>
        private DataElement AddData(Parameter par, Point location)
        {
            return(AddData(par, location, par.Name));
        }

        /// <summary>
        /// Adds a data element to the modeler
        /// </summary>
        /// <param name="par">The data set to add to the modeler</param>
        /// <param name="location">A point representing the virtual location of the data element</param>
        /// <param name="name">The name to give the element</param>
        private DataElement AddData(Parameter par, Point location, string name)
        {
            DataElement de = new DataElement(par,_modelElements);
            de.Color = _dataColor;
            de.Font = _dataFont;
            de.Shape = _dataShape;
            if (par.ModelName != "")
                de.Name = par.ModelName;
            else
                de.Name = par.Name;
            AddElement(de, location);
            par.ModelName = de.Name;
            return de;
        }

        /// <summary>
        /// Adds an arrow element given a source and destination element
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="destElement"></param>
        private ArrowElement AddArrow(ModelElement sourceElement, ModelElement destElement)
        {
            ArrowElement ae = new ArrowElement(sourceElement, destElement, _modelElements);
            ae.Color = _arrowColor;
            ae.Name = sourceElement.Name + "_" + destElement.Name;
            AddElement(ae, ae.Location);
            return ae;
        }

        /// <summary>
        /// Adds a element to the model
        /// </summary>
        /// <param name="element">The new model Element to add to the model form</param>
        /// <param name="location">A point representing the virtual location of the element</param>
        private void AddElement(ModelElement element, Point location)
        {
            List<string> elementNames = new List<string>();
            foreach (ModelElement mEle in _modelElements)
                elementNames.Add(mEle.Name);

            string tempName = element.Name;
            int i = 1;
            while (elementNames.Contains(tempName))
            {
                tempName = element.Name + "_" + i;
                i++;
            }

            element.Name = tempName;
            _modelElements.Add(element);
            element.Location = location;
            IsInitialized = false;
        }

        /// <summary>
        /// Clears all the elements in the _selectedElements list,removes their highlights and puts them back in the _modelElements list
        /// </summary>
        private void ClearSelectedElements()
        {
            foreach (ModelElement me in _modelElementsSelected)
                me.Highlighted(false);
            _modelElementsSelected.Clear();
            IsInitialized = false;
        }

        /// <summary>
        /// Removes the specified element from the _selectedElements list
        /// </summary>
        /// <param name="element"></param>
        private void RemoveSelectedElement(ModelElement element)
        {
            if (_modelElementsSelected.Contains(element))
            {
                _modelElementsSelected.Remove(element);
                element.Highlighted(false);
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Adds an element to the _modelElementsSelected Array and highlights it
        /// </summary>
        /// <param name="element"></param>
        private void AddSelectedElement(ModelElement element)
        {
            _modelElementsSelected.Insert(0,element);
            element.Highlighted(true);

            //We check if the element has any arrows connected to it and if it does we select them too
            foreach (ModelElement me in _modelElements)
            {
                if (me as ArrowElement != null)
                {
                    ArrowElement ae = me as ArrowElement;
                    if (ae.StartElement == element || ae.StopElement == element)
                    {
                        _modelElementsSelected.Add(ae);
                        ae.Highlighted(true);
                    }
                }
            }

            IsInitialized = false;
        }

        /// <summary>
        /// Removes a specific element from the model clearing any links it has to other elements
        /// </summary>
        /// <param name="element">The element to remove</param>
        private void DeleteElement(ModelElement element)
        {
            if (element as ToolElement != null)
            {
                ToolElement te = element as ToolElement;
                if (_modelElements.Contains(te))
                    _modelElements.Remove(te);
            }
            else if (element as DataElement != null)
            {
                DataElement de = element as DataElement;
                if (_modelElements.Contains(de))
                    _modelElements.Remove(de);
            }
            else if (element as ArrowElement != null)
            {
                ArrowElement ae = element as ArrowElement;
                if (_modelElements.Contains(ae))
                    _modelElements.Remove(ae);
            }
            else if (element as BlankElement != null)
            {
                if (_modelElements.Contains(element))
                    _modelElements.Remove(element);
            }

        }

        /// <summary>
        /// Adds a tool to the Modeler
        /// </summary>
        /// <param name="tool">the tool to add to the modeler</param>
        /// <param name="location">A point representing the virtual location of the tool</param>
        private ToolElement AddTool(ITool tool, Point location)
        {
            return (AddTool(tool, tool.Name, location));
        }

        /// <summary>
        /// Adds a tool to the Modeler
        /// </summary>
        /// <param name="tool">the tool to add to the modeler</param>
        /// <param name="modelName"></param>
        /// <param name="location">A point representing the virtual location of the tool</param>
        private ToolElement AddTool(ITool tool, string modelName, Point location)
        {
            if (tool == null)
                return null;

            ToolElement te = new ToolElement(tool, _modelElements);
            te.Font = _toolFont;
            te.Color = _toolColor;
            te.Shape = _toolShape;
            te.Name = modelName;
            AddElement(te, location);
            int j = 0;
            foreach (Parameter par in te.Tool.OutputParameters)
            {
                Point newLocation = new Point(te.Location.X + Convert.ToInt32(te.Width * 1.1), te.Location.Y + Convert.ToInt32(j * te.Height * 1.1));
                DataElement de = AddData(par, newLocation);
                par.GenerateDefaultOutput(System.IO.Path.GetDirectoryName(ModelFilename));
                AddArrow(te, de);
                j++;
            }
            return te;
        }

        #endregion

        #region ------------------- public methods

        /// <summary>
        /// Saves the current project, prompting the user to save as if the project is new.
        /// </summary>
        public void SaveModel()
        {
            if (_newModel)
                SaveModel(true, true);
            else
                SaveModel(false, false);
        }

        /// <summary>
        /// Saves the current project to disk.
        /// </summary>
        /// <param name="promptOverwrite">True to prompt for overwrite if file exists</param>
        /// <param name="promptSaveAs">True show save as dialog</param>
        public void SaveModel(bool promptOverwrite, bool promptSaveAs)
        {
            string filename = ModelFilename;
            if (promptSaveAs)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.OverwritePrompt = promptOverwrite;
                sfd.Filter = "Model *." + DefaultFileExtension + "|*." + DefaultFileExtension;
                sfd.DefaultExt = DefaultFileExtension;
                sfd.AddExtension = true;
                sfd.CheckPathExists = true;
                sfd.FileName = ModelFilename;
                if (sfd.ShowDialog(this) == DialogResult.Cancel)
                    return;
                filename = sfd.FileName;
            }

            if (SaveModel(filename))
                ModelFilename = filename;

        }

        /// <summary>
        /// Saves a copy of the model to the selected file without renaming the current project
        /// </summary>
        /// <param name="filename"></param>
        public bool SaveModel(string filename)
        {
            //Creates the model xml document
            XmlDocument modelXmlDoc = new XmlDocument();
            XmlElement root = modelXmlDoc.CreateElement("MapWindowModelFile");
            root.SetAttribute("Version", "1.0");
            modelXmlDoc.AppendChild(root);

            //Saves the Tools and their output configuration to the model
            List<ModelElement> toolElements = _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); });
            foreach (ToolElement te in toolElements)
            {
                XmlElement tool = modelXmlDoc.CreateElement("Tool");
                tool.SetAttribute("ToolUniqueName", te.Tool.UniqueName);
                tool.SetAttribute("ModelName", te.Name);
                tool.SetAttribute("Version", te.Tool.Version.ToString());
                tool.SetAttribute("X", te.Location.X.ToString());
                tool.SetAttribute("Y", te.Location.Y.ToString());

                foreach (DataElement de in te.getChildren())
                {
                    XmlElement outputData = modelXmlDoc.CreateElement("OutputData");
                    outputData.SetAttribute("ParameterName", de.Parameter.Name);
                    outputData.SetAttribute("ModelName", de.Name);
                    outputData.SetAttribute("X", de.Location.X.ToString());
                    outputData.SetAttribute("Y", de.Location.Y.ToString());
                    tool.AppendChild(outputData);
                }

                foreach (DataElement de in te.getParents())
                {
                    XmlElement inputData = modelXmlDoc.CreateElement("InputData");
                    inputData.SetAttribute("ParameterName", de.Parameter.Name);
                    inputData.SetAttribute("ModelName", de.Name);
                    inputData.SetAttribute("X", de.Location.X.ToString());
                    inputData.SetAttribute("Y", de.Location.Y.ToString());
                    tool.AppendChild(inputData);
                }

                foreach (Parameter inputParam in te.Tool.InputParameters)
                {
                    if (inputParam.ParamVisible == ShowParamInModel.No)
                    {
                        XmlElement inputData = modelXmlDoc.CreateElement("InputData");
                        inputData.SetAttribute("ParameterName", inputParam.Name);
                    }
                }

                root.AppendChild(tool);
            }

            modelXmlDoc.Save(filename);
            return true;

        }

        /// <summary>
        /// Prompts the user to load a model and asks them if they want to save the current model first
        /// </summary>
        public void LoadModel()
        {
            //Prompt the user to save
            DialogResult dr = MessageBox.Show(this, MessageStrings.ModelSaveCurrentModel.Replace("%S", System.IO.Path.GetFileNameWithoutExtension(ModelFilename)), "MapWindow Modeler", MessageBoxButtons.YesNoCancel,MessageBoxIcon.Exclamation);
            if (dr == DialogResult.Cancel)
                return;
            if (dr == DialogResult.Yes)
                SaveModel();

            //Prompts user to pick file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Model *." + DefaultFileExtension + "|*." + DefaultFileExtension;
            ofd.DefaultExt = DefaultFileExtension;
            ofd.AddExtension = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog(this) == DialogResult.Cancel)
                return;
            
            LoadModel(ofd.FileName);

        }

        /// <summary>
        /// Loads a model from file closing the existing model without saving it
        /// </summary>
        /// <param name="filename"></param>
        public void LoadModel(string filename)
        {
            CreateNewModel(false);

            //Open the file and make sure its a MapWindowModelFile
            XmlDocument modelXmlDoc = new XmlDocument();
            modelXmlDoc.Load(filename);
            XmlElement root = modelXmlDoc.DocumentElement;
            if (root == null) return;
            if (root.Name == "MapWindowModelFile")
            {
                //Find all the tools and add them
                XmlNode toolXml = root.FirstChild;
                while (toolXml != null && toolXml.Name == "Tool")
                {
                    String toolUniqueName = toolXml.Attributes["ToolUniqueName"].Value;
                    String toolModelName = toolXml.Attributes["ModelName"].Value;
                    Point toolLocation = new Point(Convert.ToInt32(toolXml.Attributes["X"].Value), Convert.ToInt32(toolXml.Attributes["Y"].Value));
                    Version toolVersion = new Version(toolXml.Attributes["Version"].Value);
                    ITool newTool;

                    if (_toolManager.CanCreateTool(toolUniqueName))
                    {
                        newTool = _toolManager.NewTool(toolUniqueName);
                        if (newTool.Version < toolVersion)
                        {
                            MessageBox.Show("Tool version mismatch");
                        }
                        else
                        {
                            //This code creates the tool and restores its outputs locations and names
                            ToolElement te = AddTool(newTool, toolModelName, toolLocation);
                            XmlNode outputDataXml = toolXml.FirstChild;
                            while (outputDataXml != null && outputDataXml.Name == "OutputData")
                            {
                                String dataParameterName = outputDataXml.Attributes["ParameterName"].Value;
                                String dataModelName = outputDataXml.Attributes["ModelName"].Value;
                                Point dataLocation = new Point(Convert.ToInt32(outputDataXml.Attributes["X"].Value), Convert.ToInt32(outputDataXml.Attributes["Y"].Value));
                                foreach (DataElement de in te.getChildren())
                                {
                                    if (de.Parameter.Name == dataParameterName)
                                    {
                                        de.Name = dataModelName;
                                        de.Location = dataLocation;
                                        break;
                                    }
                                }
                                outputDataXml = outputDataXml.NextSibling;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tool could not be created");
                    }

                    toolXml = toolXml.NextSibling;
                }

                //Next we re-join all the input parameters
                toolXml = root.FirstChild;
                while (toolXml != null && toolXml.Name == "Tool")
                {
                    String toolModelName = toolXml.Attributes["ModelName"].Value;
                    ToolElement te = (_modelElements.Find(delegate(ModelElement o) { return (o.Name == toolModelName); }) as ToolElement);
                    XmlNode inputDataXml = toolXml.LastChild;
                    while (inputDataXml != null && inputDataXml.Name == "InputData")
                    {
                        String dataModelName = inputDataXml.Attributes["ModelName"].Value;
                        DataElement de = (_modelElements.Find(delegate(ModelElement o) { return (o.Name == dataModelName); }) as DataElement);
                        if (de != null)
                        {
                            if (te != null)
                            {
                                for (int i = 0; i < te.Tool.InputParameters.Length; i++)
                                {
                                    if (te.Tool.InputParameters[i].DefaultSpecified == false && te.Tool.InputParameters[i].ParamType == de.Parameter.ParamType)
                                    {
                                        AddArrow(de, te);
                                        te.Tool.InputParameters[i].Value = de.Parameter.Value;
                                        te.Tool.InputParameters[i].ModelName = de.Parameter.ModelName;
                                        te.updateStatus();
                                        break;
                                    }
                                }
                            }
                        }
                        inputDataXml = inputDataXml.PreviousSibling;
                    }
                    toolXml = toolXml.NextSibling;                
                }

                //This updates all the arrows so that they properly link to their output ends (since they might have moved since being created)
                foreach (ArrowElement ae in _modelElements.FindAll(delegate(ModelElement o) { return (o as ArrowElement != null); }))
                    ae.updateDimentions();

                //Zooms to the full extent of the model
                ZoomFullExtent();
            }
            else
                MessageBox.Show("The file selected MapWindow Model File");
        }

        /// <summary>
        /// Creates a new model
        /// </summary>
        /// <param name="promptSave">if true prompts user to save current model if it is not saved</param>
        public void CreateNewModel(bool promptSave)
        {

            string fnTemp;
            if (_toolManager.TempPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                fnTemp = _toolManager.TempPath + "New_Model";                
            else
                fnTemp = _toolManager.TempPath + System.IO.Path.DirectorySeparatorChar + "New_Model";
            string filename = fnTemp + "." + DefaultFileExtension;
            int i = 1;
            while (System.IO.File.Exists(filename))
            {
                filename = fnTemp + "_" + i + "." + DefaultFileExtension;
                i++;
            }
            ModelFilename = filename;
            _newModel = true;

            //Resets all the model properties
            _modelElements.Clear();
            _modelElementsSelected.Clear();
            Invalidate();
            _enableLinking = false;
        }

        /// <summary>
        /// Deletes all of the selected elements if it is allowed
        /// </summary>
        public void DeleteSelectedElements()
        {
            if (_modelElementsSelected.Count <= 0)
                return;

            //Thic creates a list of all the model elements that will need to be deleted due to their associations to data and tools
            Boolean promptDelParents = false;
            List<ModelElement> tempSelection = new List<ModelElement>();
            foreach (ModelElement selectedElement in _modelElementsSelected)
            {
                tempSelection.Add(selectedElement);
                if ((selectedElement as DataElement) != null)
                {
                    foreach (ModelElement parentEle in selectedElement.getParents())
                    {
                        if (_modelElementsSelected.Contains(parentEle) == false)
                        {
                            tempSelection.Add(parentEle);
                            promptDelParents = true;
                        }
                    }
                }
                else if ((selectedElement as ToolElement) != null)
                {
                    foreach (ModelElement childEle in selectedElement.getChildren())
                    {
                        if (_modelElementsSelected.Contains(childEle) == false)
                        {
                            tempSelection.Add(childEle);
                            promptDelParents = true;
                        }
                    }
                }
                else if ((selectedElement as ArrowElement) != null)
                {
                    ArrowElement ae = (selectedElement as ArrowElement);
                    if (ae.StartElement as ToolElement != null)
                    {
                        if (_modelElementsSelected.Contains(ae.StartElement) == false || _modelElementsSelected.Contains(ae.StopElement) == false)
                        {
                            promptDelParents = true;
                            tempSelection.Add(ae.StartElement);
                            tempSelection.Add(ae.StopElement);
                        }
                    }
                }
            }

            //We prompt the user to see if they want to delete the parents and children, if they say no we return
            if (promptDelParents)
            {
                if (MessageBox.Show(MessageStrings.ModelerConfirmDeleteSource, MessageStrings.ModelerConfirmDelete, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            //Then we delete all the elements
            ClearSelectedElements();
            foreach (ModelElement selectedElement in tempSelection)
            {
                ArrowElement arrow = (selectedElement as ArrowElement);
                if (arrow != null)
                {
                    ToolElement toolEle = (arrow.StopElement as ToolElement);
                    if (toolEle != null)
                    {
                        foreach (Parameter par in toolEle.Tool.InputParameters)
                        {
                            if (arrow.StartElement != null)
                            {
                                if (par == (arrow.StartElement as DataElement).Parameter)
                                    par.DefaultSpecified = false;
                            }
                        }
                    }

                }
                DeleteElement(selectedElement);
            }

            //Loop through all remaining tool elements to make sure they are not trying to reference something thats been deleted
            List<ModelElement> modelData = _modelElements.FindAll(delegate(ModelElement o) { return (o as DataElement != null); });
            foreach (ToolElement selectedElement in _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); }))
            {
                foreach (Parameter par in selectedElement.Tool.InputParameters)
                {
                    if (par.ParamVisible == ShowParamInModel.Always || par.ParamVisible == ShowParamInModel.Yes)
                    {
                        Parameter parameter = par;
                        if (modelData.Find(delegate(ModelElement o) { return (o as DataElement).Parameter == parameter; }) == null)
                            par.DefaultSpecified = false;
                    }
                }
                selectedElement.updateStatus();
            }

            //Finaly we clean up any arrows that are hanging around
            List<ModelElement> arrowElements = _modelElements.FindAll(delegate(ModelElement o) { return (o as ArrowElement != null); });
            foreach (ArrowElement selectedElement in arrowElements)
            {
                if (!_modelElements.Contains(selectedElement.StartElement) || !_modelElements.Contains(selectedElement.StopElement))
                    DeleteElement(selectedElement);
            }

            Debug.WriteLine("Model elements remaining: " + _modelElements.Count);
        }

        /// <summary>
        /// Zooms the model in 20%
        /// </summary>
        public void ZoomIn()
        {
            Point virtBR = PixelToVirtual(Width, Height);
            Point virtTL = PixelToVirtual(0, 0);
            Point oldVirtCenterPoint = new Point(virtTL.X + ((virtBR.X - virtTL.X) / 2), virtTL.Y + ((virtBR.Y - virtTL.Y) / 2));
            
            //Debuging code
            Debug.WriteLine("Window size: X=" + Width + "Y=" + Height);
            Debug.WriteLine("Old Virtyual Center: X=" + oldVirtCenterPoint.X + "Y=" + oldVirtCenterPoint.Y);

            ZoomFactor = ZoomFactor + .2F * ZoomFactor;

            CenterModelerOnPoint(oldVirtCenterPoint);
            
            IsInitialized = false;
        }

        /// <summary>
        /// Zooms the model out 20%
        /// </summary>
        public void ZoomOut()
        {
            Point virtBR = PixelToVirtual(Width, Height);
            Point virtTL = PixelToVirtual(0, 0);
            Point oldVirtCenterPoint = new Point(virtTL.X + ((virtBR.X - virtTL.X) / 2), virtTL.Y + ((virtBR.Y - virtTL.Y) / 2));

            //Debuging code
            Debug.WriteLine("Window size: X=" + Width + "Y=" + Height);
            Debug.WriteLine("Old Virtyual Center: X=" + oldVirtCenterPoint.X + "Y=" + oldVirtCenterPoint.Y);

            ZoomFactor = ZoomFactor - .23F * ZoomFactor;

            CenterModelerOnPoint(oldVirtCenterPoint);

            IsInitialized = false;
        }

        /// <summary>
        /// Centers the map on a given point
        /// </summary>
        /// <param name="centerPoint">A Point in the modelers virtual coordinant system</param>
        public void CenterModelerOnPoint(Point centerPoint)
        {
            Point virtBR = PixelToVirtual(Width, Height);
            Point virtTL = PixelToVirtual(0, 0);
            Point oldVirtCenterPoint = new Point(virtTL.X + ((virtBR.X - virtTL.X) / 2), virtTL.Y + ((virtBR.Y - virtTL.Y) / 2));

            _virtualOffset = new Point(_virtualOffset.X + Convert.ToInt32((oldVirtCenterPoint.X - centerPoint.X) * _virtualZoom), _virtualOffset.Y + Convert.ToInt32((oldVirtCenterPoint.Y - centerPoint.Y) * _virtualZoom));

            virtBR = PixelToVirtual(Width, Height);
            virtTL = PixelToVirtual(0, 0);
            Point newVirtCenterPoint = new Point(virtTL.X + ((virtBR.X - virtTL.X) / 2), virtTL.Y + ((virtBR.Y - virtTL.Y) / 2));

            //Debuging code
            Debug.WriteLine("New Virtyual Center: X=" + newVirtCenterPoint.X + "Y=" + newVirtCenterPoint.Y);

            IsInitialized = false;

        }

        /// <summary>
        /// Zooms to the extent of all elements in the model and centers the modeler on them
        /// </summary>
        public void ZoomFullExtent()
        {
            //If there are no elements bring the modeler back to the origin with zoom = 1
            if (_modelElements.Count < 1)
            {
                _virtualOffset = new Point(0, 0);
                _virtualZoom = 1F;
                IsInitialized = false;
                return;
            }

            //The top left and right virtual points of the model
            Point virtTL = new Point(_modelElements[0].Location.X,_modelElements[0].Location.Y);
            Point virtBR = new Point(_modelElements[0].Location.X + _modelElements[0].Width ,_modelElements[0].Location.Y+ _modelElements[0].Height);

            //Calculates the full virtual extent
            for (int i = 1; i < _modelElements.Count; i++)
            {
                if (virtTL.X > _modelElements[i].Location.X)
                    virtTL.X = _modelElements[i].Location.X;
                if (virtTL.Y > _modelElements[i].Location.Y)
                    virtTL.Y = _modelElements[i].Location.Y;
                if (virtBR.X < (_modelElements[i].Location.X + _modelElements[i].Width))
                    virtBR.X = (_modelElements[i].Location.X + _modelElements[i].Width);
                if (virtBR.Y < (_modelElements[i].Location.Y + _modelElements[i].Height))
                    virtBR.Y = (_modelElements[i].Location.Y + _modelElements[i].Height);
            }

            //Calculates the new zoom level
            double virtWidth = Math.Abs(virtTL.X - virtBR.X);
            double virtHeight = Math.Abs(virtTL.Y - virtBR.Y);
            if (((Width - 100) / virtWidth) < ((Height - 100) / virtHeight))
                ZoomFactor = Convert.ToSingle((Width - 100) / virtWidth);
            else
                ZoomFactor = Convert.ToSingle((Height - 100) / virtHeight);

            //Centers the modeler on the model elements
            Point centerPoint = new Point(virtTL.X + ((virtBR.X - virtTL.X) / 2), virtTL.Y + ((virtBR.Y - virtTL.Y) / 2));
            CenterModelerOnPoint(centerPoint);

            IsInitialized = false;
        }

        /// <summary>
        /// Creates a default output locations for tools.
        /// </summary>
        /// <param name="par"></param>
        public void GenerateDefaultOutput(Parameter par)
        {
            Data.IFeatureSet addedFeatureSet;
            switch (par.ParamType)
            {
                case "MapWindow FeatureSet Param":
                    addedFeatureSet = new Data.Shapefile();
                    addedFeatureSet.Filename = System.IO.Path.GetDirectoryName(_toolManager.TempPath) + System.IO.Path.DirectorySeparatorChar + par.ModelName + ".shp";
                    par.Value = addedFeatureSet;
                    break;
                case "MapWindow LineFeatureSet Param":
                    addedFeatureSet = new Data.LineShapefile();
                    addedFeatureSet.Filename = System.IO.Path.GetDirectoryName(_toolManager.TempPath) + System.IO.Path.DirectorySeparatorChar + par.ModelName + ".shp";
                    par.Value = addedFeatureSet;
                    break;
                case "MapWindow PointFeatureSet Param":
                    addedFeatureSet = new Data.PointShapefile();
                    addedFeatureSet.Filename = System.IO.Path.GetDirectoryName(_toolManager.TempPath) + System.IO.Path.DirectorySeparatorChar + par.ModelName + ".shp";
                    par.Value = addedFeatureSet;
                    break;
                case "MapWindow PolygonFeatureSet Param":
                    addedFeatureSet = new Data.PolygonShapefile();
                    addedFeatureSet.Filename = System.IO.Path.GetDirectoryName(_toolManager.TempPath) + System.IO.Path.DirectorySeparatorChar + par.ModelName + ".shp";
                    par.Value = addedFeatureSet;
                    break;
                case "MapWindow Raster Param":
                    break;
                default:
                    par.GenerateDefaultOutput(_toolManager.TempPath);
                    break;
            }
        }

        /// <summary>
        /// Executes a model after verifying that it is ready
        /// </summary>
        /// <param name="error">A string parameter which will contains a error string if one is generated</param>
        /// <returns>Returns true if it executed succesfully</returns>
        public bool ExecuteModel(out string error)
        {
            //Make sure all the tools are ready for execution
            foreach (ToolElement te in _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); }))
            {
                if (te.ToolStatus == ToolStatus.Error || te.ToolStatus == ToolStatus.Empty)
                {
                    error = te.Name + " is not ready for execution";
                    return false;
                }
            }

            //The number of tools to execute
            _toolToExeCount = _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); }).Count;

            //First we create the progress form
            ToolProgress progForm = new ToolProgress(_toolToExeCount);

            //We create a background worker thread to execute the tool
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BWToolThreader;

            object[] threadParamerter = new object[1];
            threadParamerter[0] = progForm;

            // Show the progress dialog and kick off the Async thread
            bw.RunWorkerAsync(threadParamerter);
            progForm.ShowDialog(this);

            error = "";

            //Make sure all the tools are ready for execution
            foreach (ToolElement te in _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); }))
                te.ExecutionStatus = ToolExecuteStatus.NotRun;

            return true;
        }

        private void BWToolThreader(object sender, DoWorkEventArgs e)
        {
            object[] threadParamerter = e.Argument as object[];
            if (threadParamerter == null) return;
            ToolProgress progForm = threadParamerter[0] as ToolProgress;

            //Loops through all the tools and executes them
            int i = 0;
            while (i < _toolToExeCount)
            {
                foreach (ToolElement te in _modelElements.FindAll(delegate(ModelElement o) { return (o as ToolElement != null); }))
                {
                    if (te.ExecutionStatus != ToolExecuteStatus.NotRun)
                        continue;

                    bool ready = true;
                    foreach (ModelElement me in te.getParents())
                    {
                        List<ModelElement> parentElements = me.getParents();
                        if (parentElements.Count > 0 && (parentElements[0] as ToolElement).ExecutionStatus != ToolExecuteStatus.Done)
                        {
                            ready = false;
                            break;
                        }
                    }

                    if (ready)
                    {
                        if (progForm != null)
                        {
                            progForm.Progress("", 0, "==================");
                            progForm.Progress("", 0, "Executing Tool: " + te.Name);
                            progForm.Progress("", 0, "==================");
                            te.Tool.Execute(progForm);
                            progForm.Progress("", 0, "==================");
                            progForm.Progress("", 0, "Done Executing Tool: " + te.Name);
                            progForm.Progress("", 0, "==================");
                        }
                        te.ExecutionStatus = ToolExecuteStatus.Done;
                        i++;
                        break;
                    }
                }
            }
            if (progForm != null) progForm.executionComplete();
        }

        #endregion

        #region ------------------- Events

        /// <summary>
        /// On Key Up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            Debug.WriteLine("Key Pressed: " + e.KeyCode);
            base.OnKeyUp(e);
            switch (e.KeyCode)
	        {   
                case Keys.Delete:
                    DeleteSelectedElements();
                    break;
		        default:
                    break;
	        }
        }

        /// <summary>
        /// When the users clicks the mouse this event fires
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDownOnElement = false;
            _mouseMoved = false;
            Point virPt = PixelToVirtual(e.Location);
            _mouseOldPoint = MousePosition;


            //Deals with left clicks when the model is in link mode
            if (e.Button == MouseButtons.Left && _enableLinking)
            {
                //figure out if the user click on a element
                foreach (ModelElement me in _modelElements)
                {
                    if (me.pointInElement(virPt))
                    {
                        DataElement de = me as DataElement;
                        if (de != null)
                        {
                            BlankElement linkDestination = new BlankElement(_modelElements);
                            AddElement(linkDestination, virPt);
                            _linkArrow = AddArrow(de, linkDestination);
                            AddSelectedElement(linkDestination);
                            return;
                        }
                    }
                }
                _linkArrow = null;
                return;
            }

            //Deals with left clicks when the model is not in link mode
            if (e.Button == MouseButtons.Left)
            {
                //If the user left clicked on a selected element we do nothing
                foreach (ModelElement me in _modelElementsSelected)
                {
                    //Point pt = new Point(virPt.X - me.Location.X, virPt.Y - me.Location.Y);
                    if (me.pointInElement(virPt) == true)
                    {
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            RemoveSelectedElement(me);
                            _mouseDownOnElement = false;
                        }
                        _mouseDownOnElement = true;
                        return;
                    }
                }

                //If the user left clicked on a unselected element we clear the selected list and highlight the new element
                foreach (ModelElement me in _modelElements)
                {
                    if (_modelElementsSelected.Contains(me))
                        continue;
                    // Point pt = new Point(virPt.X - me.Location.X, virPt.Y - me.Location.Y);
                    if (me.pointInElement(virPt) == true)
                    {
                        if (Control.ModifierKeys != Keys.Control)
                        {
                            ClearSelectedElements();
                        }
                        _mouseDownOnElement = true;
                        AddSelectedElement(me);
                        break;
                    }
                }
                //If the mouse is clicked on a white area we draw a box
                if (_mouseDownOnElement == false)
                {
                    ClearSelectedElements();
                    _selectBoxDraw = true;
                    _selectBoxPt = e.Location;
                    _selectBox = new Rectangle(virPt.X, virPt.Y, 0, 0);
                }   
            }
        }

        /// <summary>
        /// When the mouse is moved this event fires
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_mouseMoveDone == false)
                return;
            _mouseMoveDone = false;

            //Makes sure the mouse actually moved before we do anything
            if ((MousePosition.X != _mouseOldPoint.X) || (MousePosition.Y != _mouseOldPoint.Y))
            {                

                _mouseMoved = true;
                Point virtualOld = PixelToVirtual(_mouseOldPoint);
                Point virtualNew = PixelToVirtual(MousePosition);
                Point virtualDelta = new Point(virtualNew.X - virtualOld.X, virtualNew.Y - virtualOld.Y);
                int inflateFactor = Convert.ToInt32(5 * _virtualZoom);
                if (inflateFactor < 5) inflateFactor = 5;

                //If were in link mode draw a update link arrow
                if (_linkArrow != null)
                {
                    //This code invalidates the elements OLD location
                    Point pt1 = VirtualToPixel(_linkArrow.Location.X + _linkArrow.StartPoint.X, _linkArrow.Location.Y + _linkArrow.StartPoint.Y);
                    Point pt2 = VirtualToPixel(_linkArrow.Location.X + _linkArrow.StopPoint.X, _linkArrow.Location.Y + _linkArrow.StopPoint.Y);
                    Rectangle invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                    invalid.Inflate(inflateFactor,inflateFactor);
                    Invalidate(invalid);
                    
                    //Sets the new dimentions
                    _linkArrow.StopElement.Location = PixelToVirtual(e.Location);
                    _linkArrow.updateDimentions();

                    //This code invalidates the elements NEW location
                    pt1 = VirtualToPixel(_linkArrow.Location.X + _linkArrow.StartPoint.X, _linkArrow.Location.Y + _linkArrow.StartPoint.Y);
                    pt2 = VirtualToPixel(_linkArrow.Location.X + _linkArrow.StopPoint.X, _linkArrow.Location.Y + _linkArrow.StopPoint.Y);
                    invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                    invalid.Inflate(inflateFactor,inflateFactor);
                    Invalidate(invalid);

                    Application.DoEvents();
                    _mouseOldPoint = MousePosition;
                }

                //If the mouse was clicked and held on a item this code fires
                if (_mouseDownOnElement)
                {
                    if ((Math.Abs(virtualDelta.X) >= 1) || (Math.Abs(virtualDelta.Y) >= 1))
                    {
                        //This moves elements that are not arrows
                        foreach (ModelElement me in _modelElementsSelected)
                        {
                            if ((me as ArrowElement) == null)
                            {
                                //This code invalidates the elements OLD location
                                Point pt1 = VirtualToPixel(me.Location);
                                Point pt2 = VirtualToPixel(me.Location.X + me.Width, me.Location.Y + me.Height);
                                Rectangle invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                                invalid.Inflate(inflateFactor, inflateFactor);
                                Invalidate(invalid);
                                
                                //Updates the elements location
                                me.Location = new Point(me.Location.X + virtualDelta.X, me.Location.Y + virtualDelta.Y);

                                //This code invalidates the elements NEW location
                                pt1 = VirtualToPixel(me.Location);
                                pt2 = VirtualToPixel(me.Location.X + me.Width, me.Location.Y + me.Height);
                                invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                                invalid.Inflate(inflateFactor, inflateFactor);
                                Invalidate(invalid);
                            }
                        }
                        //This moves all the arrow elements second so they don't drag
                        foreach (ModelElement me in _modelElementsSelected)
                        {
                            if ((me as ArrowElement) != null)
                            {
                                ArrowElement ar = me as ArrowElement;

                                //This code invalidates the elements OLD location
                                Point pt1 = VirtualToPixel(ar.Location.X + ar.StartPoint.X, ar.Location.Y + ar.StartPoint.Y);
                                Point pt2 = VirtualToPixel(ar.Location.X + ar.StopPoint.X, ar.Location.Y + ar.StopPoint.Y);

                                //Updates the elements location
                                ar.updateDimentions();

                                Rectangle invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                                invalid.Inflate(inflateFactor, inflateFactor);
                                Invalidate(invalid);

                                //This code invalidates the elements NEW location
                                pt1 = VirtualToPixel(ar.Location.X + ar.StartPoint.X, ar.Location.Y + ar.StartPoint.Y);
                                pt2 = VirtualToPixel(ar.Location.X + ar.StopPoint.X, ar.Location.Y + ar.StopPoint.Y);
                                invalid = new Rectangle(Math.Max(0, Math.Min(Math.Min(pt1.X, pt2.X) - 5, Width)), Math.Max(0, Math.Min(Math.Min(pt1.Y, pt2.Y) - 5, Height)), Math.Max(pt1.X, pt2.X) + 10, Math.Max(pt1.Y, pt2.Y) + 10);
                                invalid.Inflate(inflateFactor, inflateFactor);
                                Invalidate(invalid);
                            }
                        }
                        _mouseOldPoint = MousePosition;
                    }
                    Application.DoEvents();
                }
                //If the mouse was clicked outside of an element
                if (_selectBoxDraw)
                {
                    Invalidate(_selectBox);
                    _selectBox.Width = Math.Abs(_selectBoxPt.X - e.X);
                    _selectBox.Height = Math.Abs(_selectBoxPt.Y - e.Y);
                    _selectBox.X = Math.Min(_selectBoxPt.X, e.X);
                    _selectBox.Y = Math.Min(_selectBoxPt.Y, e.Y);
                    Invalidate(_selectBox);
                }
            }
            _mouseMoveDone = true;
        }

        /// <summary>
        /// When the user mouses up after a single click this event fires
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Point virPt = PixelToVirtual(e.Location);

            //If we are in link mode we run this
            if (_linkArrow != null)
            {
                ClearSelectedElements();
                ModelElement[] tempElements = new ModelElement[_modelElements.Count];
                _modelElements.CopyTo(tempElements);
                foreach (ModelElement me in tempElements)
                {
                    if (me.pointInElement(virPt) && me != _linkArrow.StartElement)
                    {
                        ToolElement te = me as ToolElement;
                        if (te != null) //If the user let go over a tool we try to link to to, assuming it doesn't create a loop
                        {
                            if (_linkArrow.StartElement.isDownstreamOf(te)) //If the tool is the data sets parent
                            {
                                MessageBox.Show(MessageStrings.linkErrorCircle, MessageStrings.linkError, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                            bool showError = true;
                            for (int i = 0; i < te.Tool.InputParameters.Length; i++)
                            {
                                DataElement linkStart = _linkArrow.StartElement as DataElement;
                                if (te.Tool.InputParameters[i].DefaultSpecified == false && te.Tool.InputParameters[i].ParamType == linkStart.Parameter.ParamType)
                                {
                                    AddArrow(linkStart, te);
                                    te.Tool.InputParameters[i].Value = linkStart.Parameter.Value;
                                    te.Tool.InputParameters[i].ModelName = linkStart.Parameter.ModelName;
                                    showError = false;
                                    te.updateStatus();
                                    break;
                                }
                            }
                            if (showError) MessageBox.Show(MessageStrings.linkNoFreeInput, MessageStrings.linkError, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                        MessageBox.Show(MessageStrings.linkErrorToData, MessageStrings.linkError ,MessageBoxButtons.OK,MessageBoxIcon.Information);
                        break;
                    }
                }
                DeleteElement(_linkArrow.StopElement);
                DeleteElement(_linkArrow);
                _linkArrow = null;
                IsInitialized = false;
                Invalidate();
                Application.DoEvents();
                return;
            }

            //If we detect the user clicked on a element and didn't move the mouse we select that element and clear the others
            if (_mouseMoved == false && _mouseDownOnElement)
            {
                if (ModifierKeys != Keys.Control)
                {
                    ClearSelectedElements();
                    foreach (ModelElement me in _modelElements)
                    {
                        if (_modelElementsSelected.Contains(me))
                            continue;
                        if (me.pointInElement(virPt))
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                RemoveSelectedElement(me);
                                _mouseDownOnElement = false;
                            }
                            AddSelectedElement(me);
                            _mouseDownOnElement = false;
                            return;
                        }
                    }
                }
            }

            //When the user lets go of the select box we find out which elements it selected
            if (_selectBoxDraw && _mouseMoved)
            {
                ClearSelectedElements();
                List <ModelElement> elementBox = new List<ModelElement>();
                foreach (ModelElement me in _modelElements)
                {
                    if (_modelElementsSelected.Contains(me))
                        continue;
                    if (me.elementInRectangle(PixelRectToVirtualRect(_selectBox)))
                    {
                        elementBox.Add(me);
                    }
                }
                for (int i = elementBox.Count - 1;i >= 0; i-- )
                {
                    AddSelectedElement(elementBox[i]);
                }
            }

            //After a mouse up we reset the mouse variables
            _selectBoxDraw = false;
            Invalidate(_selectBox);
            _mouseDownOnElement = false;
            _mouseMoved = false;
        }

        /// <summary>
        /// Occurs when the mouse leaves.
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnMouseLeave(EventArgs e)
        {
            Debug.WriteLine("mouse left");

        }

        /// <summary>
        /// When the user double clicks on the model this event fires
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            MouseEventArgs mouseE = e as MouseEventArgs;

            //If the user left clicked on a selected element we do nothing
            if (mouseE == null) return;
            Point virPt = PixelToVirtual(mouseE.X, mouseE.Y);
            foreach (ModelElement me in _modelElementsSelected)
            {
                //Point pt = new Point(virPt.X - me.Location.X, virPt.Y - me.Location.Y);
                if (me.pointInElement(virPt))
                {
                    //We create a new instance if its a toolelement
                    ToolElement meAsTool = me as ToolElement;
                    if (meAsTool != null)
                    {
                        //All of the parameters are copied from the original one
                        ITool newToolCopy = _toolManager.NewTool(meAsTool.Tool.UniqueName);
                        for (int i = 0; i < (meAsTool.Tool.InputParameters.Length); i++)
                            if (newToolCopy.InputParameters[i] != null) newToolCopy.InputParameters[i] = meAsTool.Tool.InputParameters[i].Copy();
                        for (int i = 0; i < (meAsTool.Tool.OutputParameters.Length); i++)
                            if (newToolCopy.OutputParameters[i] != null) newToolCopy.OutputParameters[i] = meAsTool.Tool.OutputParameters[i].Copy();
                        
                        //This code ensures that no children are passed into the tool
                        List<ModelElement> tempList = new List<ModelElement>();
                        foreach (ModelElement mEle in _modelElements)
                        {
                            if (mEle.isDownstreamOf(meAsTool)) continue;
                            tempList.Add(mEle);
                        }
                        ToolElement tempTool = new ToolElement(newToolCopy, tempList);
                        
                        //If the user hits ok we dispose of the saved copy if they don't we restore the old values
                        if (tempTool.DoubleClick())
                        {
                            for (int i = 0; i < (meAsTool.Tool.InputParameters.Length); i++)
                                if ( meAsTool.Tool.InputParameters[i] != null) meAsTool.Tool.InputParameters[i] = tempTool.Tool.InputParameters[i];
                            for (int i = 0; i < (meAsTool.Tool.OutputParameters.Length); i++)
                            {
                                if (tempTool.Tool.OutputParameters[i] != null)
                                {
                                    if (tempTool.Tool.OutputParameters[i].DefaultSpecified)
                                    {
                                        meAsTool.Tool.OutputParameters[i].ModelName = tempTool.Tool.OutputParameters[i].ModelName;
                                        meAsTool.Tool.OutputParameters[i].Value = tempTool.Tool.OutputParameters[i].Value;
                                    }
                                }
                            }

                            //First we clear all the arrows linking to the current tool (We use a temporary array since we will be removing items from the list)
                            ModelElement[] tempArray = new ModelElement[_modelElements.Count];
                            _modelElements.CopyTo(tempArray);
                            foreach (ModelElement mele in tempArray)
                            {
                                ArrowElement ae = mele as ArrowElement;
                                if (ae != null)
                                {
                                    if (ae.StopElement == meAsTool)
                                        DeleteElement(ae);
                                }
                            }

                            //We go through each parameters of the tool to the model if they haven't been added already
                            int j = 0;
                            foreach (Parameter par in meAsTool.Tool.InputParameters)
                            {
                                if (par == null) continue;

                                //If its not supposed to be visible we ignore it.
                                if (par.ParamVisible == ShowParamInModel.No)
                                    continue;

                                //If no default has been specified continue
                                if (par.DefaultSpecified == false)
                                    continue;

                                //If the parameter has not been assigned a model name we add the data to the model
                                bool addParam = true;
                                foreach (ModelElement modElem in _modelElements)
                                {
                                    DataElement dataElem = modElem as DataElement;
                                    if (dataElem == null) continue;
                                    if (dataElem.Parameter.Value == par.Value) { addParam = false; break; }
                                }
                                if (addParam)
                                {
                                    Point newLocation = new Point(meAsTool.Location.X - Convert.ToInt32(meAsTool.Width * 1.1), meAsTool.Location.Y + Convert.ToInt32(j * meAsTool.Height * 1.1));
                                    AddArrow(AddData(par, newLocation), meAsTool);
                                    j++;
                                }
                                else
                                {
                                    //If the parameter has already been added we make sure that we have linked to it properly
                                    tempArray = new ModelElement[_modelElements.Count];
                                    _modelElements.CopyTo(tempArray);
                                    foreach (ModelElement mele in tempArray)
                                    {
                                        DataElement de = mele as DataElement;
                                        if (de != null)
                                        {
                                            if (par.ModelName == de.Parameter.ModelName)
                                            {
                                                AddArrow(mele, meAsTool);
                                            }
                                        }
                                    }
                                }
                            }
                            //This updates the status light
                            meAsTool.updateStatus();
                        }
                    }
                    else
                    {
                        me.DoubleClick();
                    }
                    Invalidate();
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a tool to the modeler
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (!drgevent.Data.GetDataPresent(DataFormats.StringFormat)) return;
            string fromDrag = drgevent.Data.GetData(DataFormats.StringFormat) as string;
            if (fromDrag == null) return;
            if (fromDrag.StartsWith("ITool: "))
            {
                fromDrag = fromDrag.Replace("ITool: ", "");
                if (_toolManager.CanCreateTool(fromDrag))
                {
                    ITool tool = _toolManager.NewTool(fromDrag);
                    AddTool(tool, PixelToVirtual(this.PointToClient(MousePosition)));
                    return;
                }
            }
        }

        /// <summary>
        /// Allows a drag into the modeler
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (drgevent.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string fromDrag = drgevent.Data.GetData(DataFormats.StringFormat) as string;
                if (fromDrag != null)
                {
                    if (fromDrag.StartsWith("ITool: "))
                    {
                        fromDrag = fromDrag.Replace("ITool: ","");
                        if (_toolManager.CanCreateTool(fromDrag))
                        {
                            drgevent.Effect = DragDropEffects.Copy;
                            return;
                        }
                    }
                }
            }
            drgevent.Effect = DragDropEffects.None;
        }

        #endregion

        #region ------------------- Drawing code

        /// <summary>
        /// Paints the elements to the backbuffer
        /// </summary>
        private void UpdateBackBuffer()
        {
            _backBuffer = new Bitmap(Width, Height);
            Graphics graph = Graphics.FromImage(_backBuffer);
            graph.SmoothingMode = _drawingQuality;
            graph.FillRectangle(Brushes.White, 0, 0, _backBuffer.Width, _backBuffer.Height);

            //When the backbuffer is updated this code draws the watermark
            if (_showWaterMark)
            {
                Bitmap watermark = Images.MapWindowLogoPale;
                if ((_backBuffer.Width > watermark.Width) && (_backBuffer.Height > watermark.Height))
                {
                    graph.DrawImage(watermark, _backBuffer.Width - watermark.Width - 18, _backBuffer.Height - watermark.Height -18 ,watermark.Width , watermark.Height);
                }
            }

            //Check if there are any model elements to draw
            foreach (ModelElement me in _modelElements)
            {
                if (_modelElementsSelected.Contains(me) == false)
                {
                    System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                    Point translator = VirtualToPixel(new Point(me.Location.X, me.Location.Y));
                    m.Translate(translator.X,translator.Y);
                    m.Scale(_virtualZoom, _virtualZoom);
                    graph.Transform = m;
                    me.Paint(graph);
                    graph.Transform = new System.Drawing.Drawing2D.Matrix();
                }
            }

            //Updates is initialized
            IsInitialized = true;
        }

        /// <summary>
        /// When the element is called on to be painted this method is called
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
                base.OnPaint(e);
                Bitmap tempBuffer = new Bitmap(Width, Height);
                Graphics tempGraph = Graphics.FromImage(tempBuffer);
                tempGraph.SmoothingMode = _drawingQuality;
                Rectangle inflatedInvalidationRect = Rectangle.Inflate(e.ClipRectangle,5,5);

                if (IsInitialized)
                {
                    tempGraph.DrawImage(_backBuffer, inflatedInvalidationRect, inflatedInvalidationRect, GraphicsUnit.Pixel);
                }
                else
                {
                    UpdateBackBuffer();
                    tempGraph.DrawImage(_backBuffer, new Point(0, 0));
                }

                //Draws selected shapes last 
                if (_modelElementsSelected.Count > 0)
                {
                    for (int i = _modelElementsSelected.Count - 1; i >= 0; i--)
                    {
                        ModelElement me = _modelElementsSelected[i];
                        System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                        Point translator = VirtualToPixel(me.Location.X,me.Location.Y);
                        m.Translate(translator.X, translator.Y);
                        m.Scale(_virtualZoom, _virtualZoom);
                        tempGraph.Transform = m;
                        me.Paint(tempGraph);
                        tempGraph.Transform = new System.Drawing.Drawing2D.Matrix();
                    }
                }

                //If the users is dragging a select box we draw it here
                if (_selectBoxDraw)
                {
                    SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(30,SystemColors.Highlight));
                    tempGraph.FillRectangle(highlightBrush, Rectangle.Inflate(_selectBox,-1,-1));
                    Rectangle outlineRect = new Rectangle(_selectBox.X, _selectBox.Y, _selectBox.Width - 1, _selectBox.Height - 1);
                    tempGraph.DrawRectangle(SystemPens.Highlight, outlineRect);

                    //garbage collection
                    highlightBrush.Dispose();
                }

                //Draws the temporary bitmap to the screen
                e.Graphics.SmoothingMode = _drawingQuality;
                e.Graphics.DrawImage(tempBuffer, inflatedInvalidationRect, inflatedInvalidationRect, GraphicsUnit.Pixel);

                //Garbage collection
                tempBuffer.Dispose();    
                tempGraph.Dispose();
        }

        /// <summary>
        /// When the form draws the background do nothing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Redraws the background when the form is resized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            IsInitialized = false;
            base.OnResize(e);
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        protected IContainer components;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Modeler));
            this._panelHScroll = new System.Windows.Forms.Panel();
            this._horScroll = new System.Windows.Forms.HScrollBar();
            this._verScroll = new System.Windows.Forms.VScrollBar();
            this._contMenuRc = new System.Windows.Forms.ContextMenu();
            this._menuItem1 = new System.Windows.Forms.MenuItem();
            this._panelHScroll.SuspendLayout();
            this.SuspendLayout();
            // 
            // _panelHScroll
            // 
            this._panelHScroll.BackColor = System.Drawing.SystemColors.Control;
            this._panelHScroll.Controls.Add(this._horScroll);
            resources.ApplyResources(this._panelHScroll, "_panelHScroll");
            this._panelHScroll.Name = "_panelHScroll";
            // 
            // _horScroll
            // 
            resources.ApplyResources(this._horScroll, "_horScroll");
            this._horScroll.Name = "_horScroll";
            // 
            // _verScroll
            // 
            resources.ApplyResources(this._verScroll, "_verScroll");
            this._verScroll.Name = "_verScroll";
            // 
            // _contMenuRc
            // 
            this._contMenuRc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuItem1});
            // 
            // _menuItem1
            // 
            this._menuItem1.Index = 0;
            resources.ApplyResources(this._menuItem1, "_menuItem1");
            // 
            // Modeler
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this._verScroll);
            this.Controls.Add(this._panelHScroll);
            this.Name = "Modeler";
            this._panelHScroll.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region ------------------- Event Change

        /// <summary>
        /// Fires when the Filename of the project is changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnModelFilenameChanged(EventArgs e)
        {
            if (ModelFilenameChanged != null)
                ModelFilenameChanged(this, e);
        }

        #endregion
    }
}
