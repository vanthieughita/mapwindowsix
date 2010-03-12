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
// The Initial Developer of this Original Code is Ted Dunsford. Created 8/5/2008 2:12:37 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace MapWindow.Map
{


    /// <summary>
    /// Tool
    /// </summary>
    public class MapFunction: IMapFunction
    {
        #region Events

        ///// <summary>
        ///// Occurs before drawing
        ///// </summary>
        //public event EventHandler<GeoDrawVerifyArgs> BeforeDrawing;
        /// <summary>
        /// Occurs during a mouse down event
        /// </summary>
        public event EventHandler<GeoMouseArgs> MouseDown;
        /// <summary>
        /// Occurs during a mouse move event
        /// </summary>
        public event EventHandler<GeoMouseArgs> MouseMove;
        /// <summary>
        /// Occurs during a mouse up event
        /// </summary>
        public event EventHandler<GeoMouseArgs> MouseUp;
        /// <summary>
        /// Occurs during a mousewheel event
        /// </summary>
        public event EventHandler<GeoMouseArgs> MouseWheel;
        /// <summary>
        /// Occurs during a double click event
        /// </summary>
        public event EventHandler<GeoMouseArgs> MouseDoubleClick;
        /// <summary>
        /// Occurs during a key up event
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;


        #endregion

        #region Private Variables



        private Image _buttonImage;
        private Bitmap _cursorBitmap;
        private bool _preventBackBuffer;
        private IMap _map;
        private bool _enabled;
        private string _name;

        #endregion
      
        #region Constructors

        /// <summary>
        /// Creates a new instance of Tool
        /// </summary>
        public MapFunction()
        {
            _enabled = false;
        }

        /// <summary>
        /// Combines the constructor with an automatic call to the init method.  If you use this constructor
        /// overload, then it is not necessary to also call the init method.  The init method is supported
        /// because constructors cannot be specified through an interface.
        /// </summary>
        /// <param name="inMap">Any valid IMap interface</param>
        public MapFunction(IMap inMap)
        {
            Init(inMap);
        }
      

        #endregion

       
        #region Methods

        /// <summary>
        /// Cancels whatever drawing was being done by the tool and resets the cursor to an arrow.
        /// </summary>
        public virtual void Cancel()
        {

        }


        /// <summary>
        /// This is the method that is called by the drawPanel.  The graphics coordinates are
        /// in pixels relative to the image being edited.
        /// </summary>
        public void Draw(MapDrawArgs args)
        {
            //if (OnBeforeDrawing(args) == true) return; // handled
            OnDraw(args);
        }

        /// <summary>
        /// Organizes the map that htis tool will work with.
        /// </summary>
        /// <param name="inMap"></param>
        public void Init(IMap inMap)
        {
            _map = inMap;
            _enabled = true;
        }

        /// <summary>
        /// Gets an available name given the base name.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public string GetAvailableName(string baseName)
        {
            string newName = baseName;
            int i = 1;
            if (_map != null)
            {
                if (_map.MapTools != null)
                {
                    while(_map.MapTools.ContainsKey(newName))
                    {
                        newName = newName + i;
                        i++;
                    }
                }
            }
            return newName;
        }

        ///// <summary>
        ///// Fires the BeforeDrawing event and allows users to intercept drawing if they want
        ///// </summary>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //protected virtual bool OnBeforeDrawing(MapDrawArgs e)
        //{
        //    if (BeforeDrawing == null) return false;
        //    GeoDrawVerifyArgs args = new GeoDrawVerifyArgs(e);
        //    BeforeDrawing(this, args);
        //    return args.Handled;
        //}
        /// <summary>
        /// This allows sub-classes to customize the drawing that occurs.  All drawing is done
        /// in the image coordinate space, where 0,0 is the upper left corner of the image.
        /// </summary>
        /// <param name="e">A PaintEventArgs where the graphics object is already in image coordinates</param>
        protected virtual void OnDraw(MapDrawArgs e)
        {

        }

        /// <summary>
        /// Forces this tool to execute whatever behavior should occur during a double click even on the panel
        /// </summary>
        /// <param name="e"></param>
        public void DoMouseDoubleClick(GeoMouseArgs e)
        {
            OnMouseDoubleClick(e);
        }
        /// <summary>
        /// Fires the DoubleClick event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseDoubleClick(GeoMouseArgs e)
        {
            if (MouseDoubleClick == null) return;
            MouseDoubleClick(this, e);
        }

        /// <summary>
        /// Instructs this tool to 
        /// </summary>
        /// <param name="e"></param>
        public void DoKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }
        /// <summary>
        /// fires the KeyUp event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp == null) return;
            KeyUp(this, e);
        }

        /// <summary>
        /// Instructs this tool to perform any actions that should occur on the MouseDown event
        /// </summary>
        /// <param name="e">A MouseEventArgs relative to the drawing panel</param>
        public void DoMouseDown(GeoMouseArgs e)
        {
            OnMouseDown(e);
        }
        /// <summary>
        /// fires the MouseDown event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseDown(GeoMouseArgs e)
        {
            if (MouseDown == null) return;
            MouseDown(this, e);
        }

        /// <summary>
        /// Instructs this tool to perform any actions that should occur on the MouseUp event
        /// </summary>
        /// <param name="e">A MouseEventArgs relative to the drawing panel</param>
        public void DoMouseUp(GeoMouseArgs e)
        {
            OnMouseUp(e);
        }
        /// <summary>
        /// Fires the MouseUP event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseUp(GeoMouseArgs e)
        {
            if (MouseUp == null) return;
            MouseUp(this, e);

        }

        /// <summary>
        /// Instructs this tool to perform any actions that should occur on the MouseMove event
        /// </summary>
        /// <param name="e">A MouseEventArgs relative to the drawing panel</param>
        public void DoMouseMove(GeoMouseArgs e)
        {
            OnMouseMove(e);
        }
        /// <summary>
        /// allows for inheriting tools to control OnMouseMove
        /// </summary>
        /// <param name="e">A GeoMouseArgs parameter</param>
        protected virtual void OnMouseMove(GeoMouseArgs e)
        {
            if (MouseMove == null) return;
            MouseMove(this, e);
        }

        /// <summary>
        /// Instructs this tool to perform any actions that should occur on the MouseWheel event
        /// </summary>
        /// <param name="e">A MouseEventArgs relative to the drawing panel</param>
        public void DoMouseWheel(GeoMouseArgs e)
        {
            OnMouseWheel(e);
        }
        /// <summary>
        /// Allows for inheriting tools to override the behavior
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseWheel(GeoMouseArgs e)
        {
            if (MouseWheel == null) return;
            MouseWheel(this, e);
        }




        #endregion

        #region Properties

        /// <summary>
        /// Describes a button image
        /// </summary>
        [Category("Appearance"), Description("This controls is the image that will be used for buttons that activate this tool.")]
        public Image ButtonImage
        {
            get { return _buttonImage; }
            set { _buttonImage = value; }
        }

        /// <summary>
        /// This controls the cursor that this tool uses, unless the action has been cancelled by attempting
        /// to use the tool outside the bounds of the image.
        /// </summary>
        [Category("Appearance"), Description("This controls the cursor that this tool uses, unless the action has been cancelled by attempting to use the tool outside the bounds of the image.")]
        public Bitmap CursorBitmap
        {
            get { return _cursorBitmap; }
            set { _cursorBitmap = value; }
        }

        /// <summary>
        /// Gets or sets a boolean that is true if this tool should be handed drawing instructions
        /// from the screen.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            protected set // Externally you can only Activate or Deactivate, not directly enable/disable
            {
                _enabled = value;
            }
            
        }

        /// <summary>
        /// Gets or sets the basic map that this tool interacts with.  This can alternately be set using 
        /// the Init method.
        /// </summary>
        public IMap Map
        {
            get { return _map; }
            set { _map = value; }
        }

        /// <summary>
        /// Gets or sets the name that attempts to identify this plugin uniquely.  If the 
        /// name is already in the tools list, this will modify the name stored here.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set 
            {
                _name = GetAvailableName(value);
            }
        }

        /// <summary>
        /// If this is false, then the typical contents from the map's back buffer are drawn first,
        /// followed by the contents of this tool.
        /// </summary>
        public virtual bool PreventBackBuffer
        {
            get { return _preventBackBuffer; }
            protected set { _preventBackBuffer = value; }
        }



        #endregion

        #region Protected Methods

        /// <summary>
        /// Forces activation
        /// </summary>
        public void Activate()
        {
            OnActivate();
        }

        /// <summary>
        /// Deactivate is like when someone clicks on a different button.  It may not
        /// involve the whole plugin being unloaded.
        /// </summary>
        public void Deactivate()
        {
            OnDeactivate();
        }

        /// <summary>
        /// Here, the entire plugin is unloading, so if there are any residual states
        /// that are not taken care of, this should remove them.
        /// </summary>
        public void Unload()
        {
            OnUnload();
        }

        /// <summary>
        /// This occurs when the entire plugin is being unloaded.
        /// </summary>
        protected virtual void OnUnload()
        {

        }

        /// <summary>
        /// This is fired when enabled is set to true, and firing this will set enabled to true
        /// </summary>
        protected virtual void OnActivate()
        {
            _enabled = true;
        }

        /// <summary>
        /// this is fired when enabled is set to false, and firing this will set enabled to false.
        /// </summary>
        protected virtual void OnDeactivate()
        {
            _enabled = false;
        }

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            
        }



        #endregion


        #region IMapFunction Members

        

        public void DoKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        #endregion
    }
}
