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
// The Initial Developer of this Original Code is Ted Dunsford. Created 8/18/2009 3:45:39 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Windows.Forms;
using MapWindow.Projections;

namespace MapWindow.Forms
{
    /// <summary>
    /// ProjectionSelectDialog
    /// </summary>
    public class ProjectionSelectDialog : Form
    {
        #region Events

        /// <summary>
        /// Occurs whenever the apply changes button is clicked, or else when the ok button is clicked.
        /// </summary>
        public event EventHandler ChangesApplied;

        #endregion

        private Panel panel1;
        private Button btnApply;
        private Button btnCancel;
        private Button cmdOk;
        private ComboBox cmbMajorCategory;
        private ComboBox cmbMinorCategory;
        private GroupBox grpType;
        private RadioButton radGeographic;
        private RadioButton radProjected;

        private ProjectionInfo _selectedCoordinateSystem;
        private Button button1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectionSelectDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmbMajorCategory = new System.Windows.Forms.ComboBox();
            this.cmbMinorCategory = new System.Windows.Forms.ComboBox();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.radGeographic = new System.Windows.Forms.RadioButton();
            this.radProjected = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.grpType.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.cmdOk);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmdOk
            // 
            resources.ApplyResources(this.cmdOk, "cmdOk");
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmbMajorCategory
            // 
            resources.ApplyResources(this.cmbMajorCategory, "cmbMajorCategory");
            this.cmbMajorCategory.FormattingEnabled = true;
            this.cmbMajorCategory.Name = "cmbMajorCategory";
            this.cmbMajorCategory.SelectedIndexChanged += new System.EventHandler(this.cmbMajorCategory_SelectedIndexChanged);
            // 
            // cmbMinorCategory
            // 
            resources.ApplyResources(this.cmbMinorCategory, "cmbMinorCategory");
            this.cmbMinorCategory.FormattingEnabled = true;
            this.cmbMinorCategory.Name = "cmbMinorCategory";
            this.cmbMinorCategory.SelectedIndexChanged += new System.EventHandler(this.cmbMinorCategory_SelectedIndexChanged);
            // 
            // grpType
            // 
            resources.ApplyResources(this.grpType, "grpType");
            this.grpType.Controls.Add(this.radGeographic);
            this.grpType.Controls.Add(this.radProjected);
            this.grpType.Name = "grpType";
            this.grpType.TabStop = false;
            // 
            // radGeographic
            // 
            resources.ApplyResources(this.radGeographic, "radGeographic");
            this.radGeographic.Name = "radGeographic";
            this.radGeographic.UseVisualStyleBackColor = true;
            // 
            // radProjected
            // 
            resources.ApplyResources(this.radProjected, "radProjected");
            this.radProjected.Checked = true;
            this.radProjected.Name = "radProjected";
            this.radProjected.TabStop = true;
            this.radProjected.UseVisualStyleBackColor = true;
            this.radProjected.CheckedChanged += new System.EventHandler(this.radProjected_CheckedChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ProjectionSelectDialog
            // 
            this.AcceptButton = this.cmdOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.cmbMinorCategory);
            this.Controls.Add(this.cmbMajorCategory);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectionSelectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ProjectionSelectDialog_Load);
            this.panel1.ResumeLayout(false);
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of CollectionPropertyGrid
        /// </summary>
        public ProjectionSelectDialog()
        {
            InitializeComponent();
            LoadMajorCategories();
            LoadMinorCategories();
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently chosen coordinate system
        /// </summary>
        public ProjectionInfo SelectedCoordinateSystem
        {
            get { return _selectedCoordinateSystem; }
            set { _selectedCoordinateSystem = value; }
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


        private void cmdOk_Click(object sender, EventArgs e)
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
            if (ChangesApplied != null) ChangesApplied(this, new EventArgs());
        }

        #endregion

        private void ProjectionSelectDialog_Load(object sender, EventArgs e)
        {
            LoadMajorCategories();
            
        }

        private void radProjected_CheckedChanged(object sender, EventArgs e)
        {
            LoadMajorCategories();
            
        }

        private void LoadMajorCategories()
        {
            if(radProjected.Checked)
            {
                cmbMajorCategory.SuspendLayout();
                cmbMajorCategory.Items.Clear();
                string[] names = KnownCoordinateSystems.Projected.Names;
                foreach (string name in names)
                {
                    cmbMajorCategory.Items.Add(name);
                }
                cmbMajorCategory.SelectedIndex = 0;
                cmbMajorCategory.ResumeLayout();
            }
            else
            {
                cmbMajorCategory.SuspendLayout();
                cmbMajorCategory.Items.Clear();
                string[] names = KnownCoordinateSystems.Geographic.Names;
                foreach (string name in names)
                {
                    cmbMajorCategory.Items.Add(name);
                }
                cmbMajorCategory.SelectedIndex = 0;
                cmbMajorCategory.ResumeLayout();
            }
        }

        private void LoadMinorCategories()
        {
            if (radProjected.Checked)
            {
                CoordinateSystemCategory c = KnownCoordinateSystems.Projected.GetCategory((string) cmbMajorCategory.SelectedItem);
                if (c == null) return;
                cmbMinorCategory.SuspendLayout();
                cmbMinorCategory.Items.Clear();
                string[] names = c.Names;
                foreach (string name in names)
                {
                    cmbMinorCategory.Items.Add(name);
                }
                cmbMinorCategory.SelectedIndex = 0;
                _selectedCoordinateSystem = c.GetProjection(names[0]);
                cmbMinorCategory.ResumeLayout();
            }
            else
            {
                CoordinateSystemCategory c = KnownCoordinateSystems.Geographic.GetCategory((string)cmbMajorCategory.SelectedItem);
                cmbMinorCategory.SuspendLayout();
                cmbMinorCategory.Items.Clear();
                string[] names = c.Names;
                foreach (string name in names)
                {
                    cmbMinorCategory.Items.Add(name);
                }
                cmbMinorCategory.SelectedIndex = 0;
                _selectedCoordinateSystem = c.GetProjection(names[0]);
                cmbMinorCategory.ResumeLayout();
            }
        }

        private void cmbMajorCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMinorCategories();
        }

        private void cmbMinorCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(radProjected.Checked)
            {
                CoordinateSystemCategory c = KnownCoordinateSystems.Projected.GetCategory((string) cmbMajorCategory.SelectedItem);
                _selectedCoordinateSystem = c.GetProjection((string)cmbMinorCategory.SelectedItem);
                cmbMinorCategory.ResumeLayout();
                
            }
            else
            {
                CoordinateSystemCategory c = KnownCoordinateSystems.Geographic.GetCategory((string)cmbMajorCategory.SelectedItem);
                _selectedCoordinateSystem = c.GetProjection((string)cmbMinorCategory.SelectedItem);
                cmbMinorCategory.ResumeLayout();
                
            }
        }

        
      







    }
}