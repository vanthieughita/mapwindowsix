//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The core assembly for the MapWindow 6.0 distribution.
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and 
// limitations under the License. 
//
// The Original Code is MapWindow.dll
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 9/12/2009 12:06:20 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Windows.Forms;
using MapWindow.Drawing;

namespace MapWindow.Forms
{
    /// <summary>
    /// DetailedPointSymbolDialog
    /// </summary>
    public class DetailedPointSymbolDialog : Form
    {
        #region Events

        /// <summary>
        /// Occurs whenever the apply changes button is clicked, or else when the ok button is clicked.
        /// </summary>
        public event EventHandler ChangesApplied;

        #endregion

        private Panel panel1;
        private DetailedPointSymbolControl detailedPointSymbolControl1;
        private MapWindow.Components.DialogButtons dialogButtons1;

        #region Private Variables

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #endregion


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailedPointSymbolDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dialogButtons1 = new MapWindow.Components.DialogButtons();
            this.detailedPointSymbolControl1 = new MapWindow.Forms.DetailedPointSymbolControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.dialogButtons1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // dialogButtons1
            // 
            this.dialogButtons1.AccessibleDescription = null;
            this.dialogButtons1.AccessibleName = null;
            resources.ApplyResources(this.dialogButtons1, "dialogButtons1");
            this.dialogButtons1.BackgroundImage = null;
            this.dialogButtons1.Font = null;
            this.dialogButtons1.Name = "dialogButtons1";
            // 
            // detailedPointSymbolControl1
            // 
            this.detailedPointSymbolControl1.AccessibleDescription = null;
            this.detailedPointSymbolControl1.AccessibleName = null;
            resources.ApplyResources(this.detailedPointSymbolControl1, "detailedPointSymbolControl1");
            this.detailedPointSymbolControl1.BackgroundImage = null;
            this.detailedPointSymbolControl1.Font = null;
            this.detailedPointSymbolControl1.Name = "detailedPointSymbolControl1";
            // 
            // DetailedPointSymbolDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.detailedPointSymbolControl1);
            this.Controls.Add(this.panel1);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailedPointSymbolDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of CollectionPropertyGrid
        /// </summary>
        public DetailedPointSymbolDialog()
        {
            InitializeComponent();
            Configure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        public DetailedPointSymbolDialog(IPointSymbolizer original)
        {
            InitializeComponent();
            detailedPointSymbolControl1.Initialize(original);
            Configure();
        }

        private void Configure()
        {
            dialogButtons1.OkClicked += btnOk_Click;
            dialogButtons1.CancelClicked += btnCancel_Click;
            dialogButtons1.ApplyClicked += btnApply_Click;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the symbolizer being used by this control.
        /// </summary>
        public IPointSymbolizer Symbolizer
        {
            get
            {
                if (detailedPointSymbolControl1 == null) return null;
                return detailedPointSymbolControl1.Symbolizer;
            }
            set
            {
                if (detailedPointSymbolControl1 == null) return;
                detailedPointSymbolControl1.Symbolizer = value;
            }
        }

        #endregion

        #region Events

        #endregion

        #region Event Handlers

        private void btnApply_Click(object sender, EventArgs e)
        {
            OnApplyChanges();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            OnApplyChanges();
            Close();
        }



        #endregion

        #region Protected Methods

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
        /// Fires the ChangesApplied event
        /// </summary>
        protected virtual void OnApplyChanges()
        {
            detailedPointSymbolControl1.ApplyChanges();
            if (ChangesApplied != null) ChangesApplied(this, new EventArgs());
        }

        #endregion







    }
}