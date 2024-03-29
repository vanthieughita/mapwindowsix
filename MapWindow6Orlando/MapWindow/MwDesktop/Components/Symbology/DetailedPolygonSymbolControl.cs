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
// The Initial Developer of this Original Code is Ted Dunsford. Created 5/19/2009 2:56:10 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MapWindow.Components;
using MapWindow.Drawing;
using MapWindow.Main;

namespace MapWindow.Forms
{
    /// <summary>
    /// DetailedPolygonSymbolDialog
    /// </summary>
    public class DetailedPolygonSymbolControl : UserControl
    {
        #region Events

        /// <summary>
        /// Occurs whenever the apply changes button is clicked, or else when the ok button is clicked.
        /// </summary>
        public event EventHandler ChangesApplied;

        /// <summary>
        /// Occurs when the the Add To Custom Symbols button is clicked 
        /// </summary>
        public event EventHandler<PolygonSymbolizerEventArgs> AddToCustomSymbols;
        #endregion


        #region Private Variables

        private PatternCollectionControl ccPatterns;
        private GroupBox groupBox1;
        private ComboBox cmbUnits;
        private Label lblUnits;
        private Label label3;
        private Label lblPreview;
        private Label lblScaleMode;
        private ComboBox cmbScaleMode;
        private CheckBox chkSmoothing;
        private Label lblPatternType;
        private ComboBox cmbPatternType;
        private TabControl tabPatternProperties;
        private TabPage tabSimple;
        private Label lblColorSimple;
        private ColorButton cbColorSimple;
        private RampSlider sldOpacitySimple;


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private IPolygonSymbolizer _original;
        private IPolygonSymbolizer _symbolizer;
        private bool _ignoreChanges;
        private Button btnAddToCustom;
        private TabPage tabPicture;
        private Label label4;
        private Label lblTileMode;
        private ComboBox cmbTileMode;
        private Button btnLoadImage;
        private TextBox txtImage;
        private AngleControl angTileAngle;
        private DoubleBox dbxScaleY;
        private DoubleBox dbxScaleX;
        private TabPage tabGradient;
        private Label lblEndColor;
        private Label lblStartColor;
        private OutlineControl ocOutline;
        private AngleControl angGradientAngle;
        private ComboBox cmbGradientType;
        private Tools.Modeler modeler1;
        private GradientControl sliderGradient;
        private TabPage tabHatch;
        private RampSlider hatchForeOpacity;
        private ColorButton hatchForeColor;
        private Label label2;
        private RampSlider hatchBackOpacity;
        private ColorButton hatchBackColor;
        private Label label1;
        private Label lblHatchStyle;
        private ComboBox cmbHatchStyle;
        private HelpProvider helpProvider1;
        private bool _disableUnitWarning; 


        #endregion


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailedPolygonSymbolControl));
            this.btnAddToCustom = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.lblUnits = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.lblScaleMode = new System.Windows.Forms.Label();
            this.cmbScaleMode = new System.Windows.Forms.ComboBox();
            this.chkSmoothing = new System.Windows.Forms.CheckBox();
            this.lblPatternType = new System.Windows.Forms.Label();
            this.cmbPatternType = new System.Windows.Forms.ComboBox();
            this.tabPatternProperties = new System.Windows.Forms.TabControl();
            this.tabSimple = new System.Windows.Forms.TabPage();
            this.lblColorSimple = new System.Windows.Forms.Label();
            this.tabPicture = new System.Windows.Forms.TabPage();
            this.lblTileMode = new System.Windows.Forms.Label();
            this.cmbTileMode = new System.Windows.Forms.ComboBox();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.txtImage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabGradient = new System.Windows.Forms.TabPage();
            this.cmbGradientType = new System.Windows.Forms.ComboBox();
            this.lblEndColor = new System.Windows.Forms.Label();
            this.lblStartColor = new System.Windows.Forms.Label();
            this.tabHatch = new System.Windows.Forms.TabPage();
            this.lblHatchStyle = new System.Windows.Forms.Label();
            this.cmbHatchStyle = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.cbColorSimple = new MapWindow.Components.ColorButton();
            this.sldOpacitySimple = new MapWindow.Components.RampSlider();
            this.dbxScaleY = new MapWindow.Components.DoubleBox();
            this.dbxScaleX = new MapWindow.Components.DoubleBox();
            this.angTileAngle = new MapWindow.Components.AngleControl();
            this.sliderGradient = new MapWindow.Components.GradientControl();
            this.angGradientAngle = new MapWindow.Components.AngleControl();
            this.hatchBackOpacity = new MapWindow.Components.RampSlider();
            this.hatchBackColor = new MapWindow.Components.ColorButton();
            this.hatchForeOpacity = new MapWindow.Components.RampSlider();
            this.hatchForeColor = new MapWindow.Components.ColorButton();
            this.modeler1 = new MapWindow.Tools.Modeler();
            this.ocOutline = new MapWindow.Components.OutlineControl();
            this.ccPatterns = new MapWindow.Components.PatternCollectionControl();
            this.groupBox1.SuspendLayout();
            this.tabPatternProperties.SuspendLayout();
            this.tabSimple.SuspendLayout();
            this.tabPicture.SuspendLayout();
            this.tabGradient.SuspendLayout();
            this.tabHatch.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddToCustom
            // 
            this.btnAddToCustom.AccessibleDescription = null;
            this.btnAddToCustom.AccessibleName = null;
            resources.ApplyResources(this.btnAddToCustom, "btnAddToCustom");
            this.btnAddToCustom.BackgroundImage = null;
            this.btnAddToCustom.Font = null;
            this.helpProvider1.SetHelpKeyword(this.btnAddToCustom, null);
            this.helpProvider1.SetHelpNavigator(this.btnAddToCustom, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnAddToCustom.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.btnAddToCustom, null);
            this.btnAddToCustom.Name = "btnAddToCustom";
            this.helpProvider1.SetShowHelp(this.btnAddToCustom, ((bool)(resources.GetObject("btnAddToCustom.ShowHelp"))));
            this.btnAddToCustom.UseVisualStyleBackColor = true;
            this.btnAddToCustom.Click += new System.EventHandler(this.btnAddToCustom_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.cmbUnits);
            this.groupBox1.Controls.Add(this.lblUnits);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblPreview);
            this.groupBox1.Controls.Add(this.lblScaleMode);
            this.groupBox1.Controls.Add(this.cmbScaleMode);
            this.groupBox1.Controls.Add(this.chkSmoothing);
            this.groupBox1.Font = null;
            this.helpProvider1.SetHelpKeyword(this.groupBox1, null);
            this.helpProvider1.SetHelpNavigator(this.groupBox1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("groupBox1.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.groupBox1, null);
            this.groupBox1.Name = "groupBox1";
            this.helpProvider1.SetShowHelp(this.groupBox1, ((bool)(resources.GetObject("groupBox1.ShowHelp"))));
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // cmbUnits
            // 
            this.cmbUnits.AccessibleDescription = null;
            this.cmbUnits.AccessibleName = null;
            resources.ApplyResources(this.cmbUnits, "cmbUnits");
            this.cmbUnits.BackgroundImage = null;
            this.cmbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnits.Font = null;
            this.cmbUnits.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbUnits, null);
            this.helpProvider1.SetHelpNavigator(this.cmbUnits, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbUnits.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbUnits, null);
            this.cmbUnits.Items.AddRange(new object[] {
            resources.GetString("cmbUnits.Items"),
            resources.GetString("cmbUnits.Items1"),
            resources.GetString("cmbUnits.Items2"),
            resources.GetString("cmbUnits.Items3"),
            resources.GetString("cmbUnits.Items4"),
            resources.GetString("cmbUnits.Items5"),
            resources.GetString("cmbUnits.Items6")});
            this.cmbUnits.Name = "cmbUnits";
            this.helpProvider1.SetShowHelp(this.cmbUnits, ((bool)(resources.GetObject("cmbUnits.ShowHelp"))));
            this.cmbUnits.SelectedIndexChanged += new System.EventHandler(this.cmbUnits_SelectedIndexChanged);
            // 
            // lblUnits
            // 
            this.lblUnits.AccessibleDescription = null;
            this.lblUnits.AccessibleName = null;
            resources.ApplyResources(this.lblUnits, "lblUnits");
            this.lblUnits.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblUnits, null);
            this.helpProvider1.SetHelpNavigator(this.lblUnits, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblUnits.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblUnits, null);
            this.lblUnits.Name = "lblUnits";
            this.helpProvider1.SetShowHelp(this.lblUnits, ((bool)(resources.GetObject("lblUnits.ShowHelp"))));
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.helpProvider1.SetHelpKeyword(this.label3, null);
            this.helpProvider1.SetHelpNavigator(this.label3, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("label3.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.label3, null);
            this.label3.Name = "label3";
            this.helpProvider1.SetShowHelp(this.label3, ((bool)(resources.GetObject("label3.ShowHelp"))));
            // 
            // lblPreview
            // 
            this.lblPreview.AccessibleDescription = null;
            this.lblPreview.AccessibleName = null;
            resources.ApplyResources(this.lblPreview, "lblPreview");
            this.lblPreview.BackColor = System.Drawing.Color.White;
            this.lblPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPreview.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblPreview, null);
            this.helpProvider1.SetHelpNavigator(this.lblPreview, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblPreview.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblPreview, null);
            this.lblPreview.Name = "lblPreview";
            this.helpProvider1.SetShowHelp(this.lblPreview, ((bool)(resources.GetObject("lblPreview.ShowHelp"))));
            // 
            // lblScaleMode
            // 
            this.lblScaleMode.AccessibleDescription = null;
            this.lblScaleMode.AccessibleName = null;
            resources.ApplyResources(this.lblScaleMode, "lblScaleMode");
            this.lblScaleMode.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblScaleMode, null);
            this.helpProvider1.SetHelpNavigator(this.lblScaleMode, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblScaleMode.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblScaleMode, null);
            this.lblScaleMode.Name = "lblScaleMode";
            this.helpProvider1.SetShowHelp(this.lblScaleMode, ((bool)(resources.GetObject("lblScaleMode.ShowHelp"))));
            // 
            // cmbScaleMode
            // 
            this.cmbScaleMode.AccessibleDescription = null;
            this.cmbScaleMode.AccessibleName = null;
            resources.ApplyResources(this.cmbScaleMode, "cmbScaleMode");
            this.cmbScaleMode.BackgroundImage = null;
            this.cmbScaleMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScaleMode.Font = null;
            this.cmbScaleMode.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbScaleMode, null);
            this.helpProvider1.SetHelpNavigator(this.cmbScaleMode, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbScaleMode.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbScaleMode, null);
            this.cmbScaleMode.Items.AddRange(new object[] {
            resources.GetString("cmbScaleMode.Items"),
            resources.GetString("cmbScaleMode.Items1"),
            resources.GetString("cmbScaleMode.Items2")});
            this.cmbScaleMode.Name = "cmbScaleMode";
            this.helpProvider1.SetShowHelp(this.cmbScaleMode, ((bool)(resources.GetObject("cmbScaleMode.ShowHelp"))));
            this.cmbScaleMode.SelectedIndexChanged += new System.EventHandler(this.cmbScaleMode_SelectedIndexChanged);
            // 
            // chkSmoothing
            // 
            this.chkSmoothing.AccessibleDescription = null;
            this.chkSmoothing.AccessibleName = null;
            resources.ApplyResources(this.chkSmoothing, "chkSmoothing");
            this.chkSmoothing.BackgroundImage = null;
            this.chkSmoothing.Font = null;
            this.helpProvider1.SetHelpKeyword(this.chkSmoothing, null);
            this.helpProvider1.SetHelpNavigator(this.chkSmoothing, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("chkSmoothing.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.chkSmoothing, null);
            this.chkSmoothing.Name = "chkSmoothing";
            this.helpProvider1.SetShowHelp(this.chkSmoothing, ((bool)(resources.GetObject("chkSmoothing.ShowHelp"))));
            this.chkSmoothing.UseVisualStyleBackColor = true;
            this.chkSmoothing.CheckedChanged += new System.EventHandler(this.chkSmoothing_CheckedChanged);
            // 
            // lblPatternType
            // 
            this.lblPatternType.AccessibleDescription = null;
            this.lblPatternType.AccessibleName = null;
            resources.ApplyResources(this.lblPatternType, "lblPatternType");
            this.lblPatternType.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblPatternType, null);
            this.helpProvider1.SetHelpNavigator(this.lblPatternType, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblPatternType.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblPatternType, null);
            this.lblPatternType.Name = "lblPatternType";
            this.helpProvider1.SetShowHelp(this.lblPatternType, ((bool)(resources.GetObject("lblPatternType.ShowHelp"))));
            // 
            // cmbPatternType
            // 
            this.cmbPatternType.AccessibleDescription = null;
            this.cmbPatternType.AccessibleName = null;
            resources.ApplyResources(this.cmbPatternType, "cmbPatternType");
            this.cmbPatternType.BackgroundImage = null;
            this.cmbPatternType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatternType.Font = null;
            this.cmbPatternType.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbPatternType, null);
            this.helpProvider1.SetHelpNavigator(this.cmbPatternType, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbPatternType.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbPatternType, null);
            this.cmbPatternType.Items.AddRange(new object[] {
            resources.GetString("cmbPatternType.Items"),
            resources.GetString("cmbPatternType.Items1"),
            resources.GetString("cmbPatternType.Items2"),
            resources.GetString("cmbPatternType.Items3")});
            this.cmbPatternType.Name = "cmbPatternType";
            this.helpProvider1.SetShowHelp(this.cmbPatternType, ((bool)(resources.GetObject("cmbPatternType.ShowHelp"))));
            this.cmbPatternType.SelectedIndexChanged += new System.EventHandler(this.cmbPatternType_SelectedIndexChanged);
            // 
            // tabPatternProperties
            // 
            this.tabPatternProperties.AccessibleDescription = null;
            this.tabPatternProperties.AccessibleName = null;
            resources.ApplyResources(this.tabPatternProperties, "tabPatternProperties");
            this.tabPatternProperties.BackgroundImage = null;
            this.tabPatternProperties.Controls.Add(this.tabSimple);
            this.tabPatternProperties.Controls.Add(this.tabPicture);
            this.tabPatternProperties.Controls.Add(this.tabGradient);
            this.tabPatternProperties.Controls.Add(this.tabHatch);
            this.tabPatternProperties.Font = null;
            this.helpProvider1.SetHelpKeyword(this.tabPatternProperties, null);
            this.helpProvider1.SetHelpNavigator(this.tabPatternProperties, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabPatternProperties.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.tabPatternProperties, null);
            this.tabPatternProperties.Name = "tabPatternProperties";
            this.tabPatternProperties.SelectedIndex = 0;
            this.helpProvider1.SetShowHelp(this.tabPatternProperties, ((bool)(resources.GetObject("tabPatternProperties.ShowHelp"))));
            // 
            // tabSimple
            // 
            this.tabSimple.AccessibleDescription = null;
            this.tabSimple.AccessibleName = null;
            resources.ApplyResources(this.tabSimple, "tabSimple");
            this.tabSimple.BackgroundImage = null;
            this.tabSimple.Controls.Add(this.lblColorSimple);
            this.tabSimple.Controls.Add(this.cbColorSimple);
            this.tabSimple.Controls.Add(this.sldOpacitySimple);
            this.tabSimple.Font = null;
            this.helpProvider1.SetHelpKeyword(this.tabSimple, null);
            this.helpProvider1.SetHelpNavigator(this.tabSimple, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabSimple.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.tabSimple, null);
            this.tabSimple.Name = "tabSimple";
            this.helpProvider1.SetShowHelp(this.tabSimple, ((bool)(resources.GetObject("tabSimple.ShowHelp"))));
            this.tabSimple.UseVisualStyleBackColor = true;
            // 
            // lblColorSimple
            // 
            this.lblColorSimple.AccessibleDescription = null;
            this.lblColorSimple.AccessibleName = null;
            resources.ApplyResources(this.lblColorSimple, "lblColorSimple");
            this.lblColorSimple.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblColorSimple, null);
            this.helpProvider1.SetHelpNavigator(this.lblColorSimple, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblColorSimple.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblColorSimple, null);
            this.lblColorSimple.Name = "lblColorSimple";
            this.helpProvider1.SetShowHelp(this.lblColorSimple, ((bool)(resources.GetObject("lblColorSimple.ShowHelp"))));
            // 
            // tabPicture
            // 
            this.tabPicture.AccessibleDescription = null;
            this.tabPicture.AccessibleName = null;
            resources.ApplyResources(this.tabPicture, "tabPicture");
            this.tabPicture.BackgroundImage = null;
            this.tabPicture.Controls.Add(this.dbxScaleY);
            this.tabPicture.Controls.Add(this.dbxScaleX);
            this.tabPicture.Controls.Add(this.angTileAngle);
            this.tabPicture.Controls.Add(this.lblTileMode);
            this.tabPicture.Controls.Add(this.cmbTileMode);
            this.tabPicture.Controls.Add(this.btnLoadImage);
            this.tabPicture.Controls.Add(this.txtImage);
            this.tabPicture.Controls.Add(this.label4);
            this.tabPicture.Font = null;
            this.helpProvider1.SetHelpKeyword(this.tabPicture, null);
            this.helpProvider1.SetHelpNavigator(this.tabPicture, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabPicture.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.tabPicture, null);
            this.tabPicture.Name = "tabPicture";
            this.helpProvider1.SetShowHelp(this.tabPicture, ((bool)(resources.GetObject("tabPicture.ShowHelp"))));
            this.tabPicture.UseVisualStyleBackColor = true;
            // 
            // lblTileMode
            // 
            this.lblTileMode.AccessibleDescription = null;
            this.lblTileMode.AccessibleName = null;
            resources.ApplyResources(this.lblTileMode, "lblTileMode");
            this.lblTileMode.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblTileMode, null);
            this.helpProvider1.SetHelpNavigator(this.lblTileMode, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblTileMode.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblTileMode, null);
            this.lblTileMode.Name = "lblTileMode";
            this.helpProvider1.SetShowHelp(this.lblTileMode, ((bool)(resources.GetObject("lblTileMode.ShowHelp"))));
            // 
            // cmbTileMode
            // 
            this.cmbTileMode.AccessibleDescription = null;
            this.cmbTileMode.AccessibleName = null;
            resources.ApplyResources(this.cmbTileMode, "cmbTileMode");
            this.cmbTileMode.BackgroundImage = null;
            this.cmbTileMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTileMode.Font = null;
            this.cmbTileMode.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbTileMode, null);
            this.helpProvider1.SetHelpNavigator(this.cmbTileMode, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbTileMode.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbTileMode, null);
            this.cmbTileMode.Items.AddRange(new object[] {
            resources.GetString("cmbTileMode.Items"),
            resources.GetString("cmbTileMode.Items1"),
            resources.GetString("cmbTileMode.Items2"),
            resources.GetString("cmbTileMode.Items3"),
            resources.GetString("cmbTileMode.Items4")});
            this.cmbTileMode.Name = "cmbTileMode";
            this.helpProvider1.SetShowHelp(this.cmbTileMode, ((bool)(resources.GetObject("cmbTileMode.ShowHelp"))));
            this.cmbTileMode.SelectedIndexChanged += new System.EventHandler(this.cmbTileMode_SelectedIndexChanged);
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.AccessibleDescription = null;
            this.btnLoadImage.AccessibleName = null;
            resources.ApplyResources(this.btnLoadImage, "btnLoadImage");
            this.btnLoadImage.BackgroundImage = null;
            this.btnLoadImage.Font = null;
            this.helpProvider1.SetHelpKeyword(this.btnLoadImage, null);
            this.helpProvider1.SetHelpNavigator(this.btnLoadImage, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnLoadImage.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.btnLoadImage, null);
            this.btnLoadImage.Name = "btnLoadImage";
            this.helpProvider1.SetShowHelp(this.btnLoadImage, ((bool)(resources.GetObject("btnLoadImage.ShowHelp"))));
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // txtImage
            // 
            this.txtImage.AccessibleDescription = null;
            this.txtImage.AccessibleName = null;
            resources.ApplyResources(this.txtImage, "txtImage");
            this.txtImage.BackgroundImage = null;
            this.txtImage.Font = null;
            this.helpProvider1.SetHelpKeyword(this.txtImage, null);
            this.helpProvider1.SetHelpNavigator(this.txtImage, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtImage.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.txtImage, null);
            this.txtImage.Name = "txtImage";
            this.helpProvider1.SetShowHelp(this.txtImage, ((bool)(resources.GetObject("txtImage.ShowHelp"))));
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.helpProvider1.SetHelpKeyword(this.label4, null);
            this.helpProvider1.SetHelpNavigator(this.label4, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("label4.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.label4, null);
            this.label4.Name = "label4";
            this.helpProvider1.SetShowHelp(this.label4, ((bool)(resources.GetObject("label4.ShowHelp"))));
            // 
            // tabGradient
            // 
            this.tabGradient.AccessibleDescription = null;
            this.tabGradient.AccessibleName = null;
            resources.ApplyResources(this.tabGradient, "tabGradient");
            this.tabGradient.BackgroundImage = null;
            this.tabGradient.Controls.Add(this.sliderGradient);
            this.tabGradient.Controls.Add(this.cmbGradientType);
            this.tabGradient.Controls.Add(this.lblEndColor);
            this.tabGradient.Controls.Add(this.lblStartColor);
            this.tabGradient.Controls.Add(this.angGradientAngle);
            this.tabGradient.Font = null;
            this.helpProvider1.SetHelpKeyword(this.tabGradient, null);
            this.helpProvider1.SetHelpNavigator(this.tabGradient, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabGradient.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.tabGradient, null);
            this.tabGradient.Name = "tabGradient";
            this.helpProvider1.SetShowHelp(this.tabGradient, ((bool)(resources.GetObject("tabGradient.ShowHelp"))));
            this.tabGradient.UseVisualStyleBackColor = true;
            // 
            // cmbGradientType
            // 
            this.cmbGradientType.AccessibleDescription = null;
            this.cmbGradientType.AccessibleName = null;
            resources.ApplyResources(this.cmbGradientType, "cmbGradientType");
            this.cmbGradientType.BackgroundImage = null;
            this.cmbGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGradientType.Font = null;
            this.cmbGradientType.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbGradientType, null);
            this.helpProvider1.SetHelpNavigator(this.cmbGradientType, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbGradientType.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbGradientType, null);
            this.cmbGradientType.Items.AddRange(new object[] {
            resources.GetString("cmbGradientType.Items"),
            resources.GetString("cmbGradientType.Items1"),
            resources.GetString("cmbGradientType.Items2")});
            this.cmbGradientType.Name = "cmbGradientType";
            this.helpProvider1.SetShowHelp(this.cmbGradientType, ((bool)(resources.GetObject("cmbGradientType.ShowHelp"))));
            this.cmbGradientType.SelectedIndexChanged += new System.EventHandler(this.cmbGradientType_SelectedIndexChanged);
            // 
            // lblEndColor
            // 
            this.lblEndColor.AccessibleDescription = null;
            this.lblEndColor.AccessibleName = null;
            resources.ApplyResources(this.lblEndColor, "lblEndColor");
            this.lblEndColor.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblEndColor, null);
            this.helpProvider1.SetHelpNavigator(this.lblEndColor, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblEndColor.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblEndColor, null);
            this.lblEndColor.Name = "lblEndColor";
            this.helpProvider1.SetShowHelp(this.lblEndColor, ((bool)(resources.GetObject("lblEndColor.ShowHelp"))));
            // 
            // lblStartColor
            // 
            this.lblStartColor.AccessibleDescription = null;
            this.lblStartColor.AccessibleName = null;
            resources.ApplyResources(this.lblStartColor, "lblStartColor");
            this.lblStartColor.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblStartColor, null);
            this.helpProvider1.SetHelpNavigator(this.lblStartColor, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblStartColor.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblStartColor, null);
            this.lblStartColor.Name = "lblStartColor";
            this.helpProvider1.SetShowHelp(this.lblStartColor, ((bool)(resources.GetObject("lblStartColor.ShowHelp"))));
            // 
            // tabHatch
            // 
            this.tabHatch.AccessibleDescription = null;
            this.tabHatch.AccessibleName = null;
            resources.ApplyResources(this.tabHatch, "tabHatch");
            this.tabHatch.BackgroundImage = null;
            this.tabHatch.Controls.Add(this.lblHatchStyle);
            this.tabHatch.Controls.Add(this.cmbHatchStyle);
            this.tabHatch.Controls.Add(this.label2);
            this.tabHatch.Controls.Add(this.hatchBackOpacity);
            this.tabHatch.Controls.Add(this.hatchBackColor);
            this.tabHatch.Controls.Add(this.label1);
            this.tabHatch.Controls.Add(this.hatchForeOpacity);
            this.tabHatch.Controls.Add(this.hatchForeColor);
            this.tabHatch.Font = null;
            this.helpProvider1.SetHelpKeyword(this.tabHatch, null);
            this.helpProvider1.SetHelpNavigator(this.tabHatch, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabHatch.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.tabHatch, null);
            this.tabHatch.Name = "tabHatch";
            this.helpProvider1.SetShowHelp(this.tabHatch, ((bool)(resources.GetObject("tabHatch.ShowHelp"))));
            this.tabHatch.UseVisualStyleBackColor = true;
            // 
            // lblHatchStyle
            // 
            this.lblHatchStyle.AccessibleDescription = null;
            this.lblHatchStyle.AccessibleName = null;
            resources.ApplyResources(this.lblHatchStyle, "lblHatchStyle");
            this.lblHatchStyle.Font = null;
            this.helpProvider1.SetHelpKeyword(this.lblHatchStyle, null);
            this.helpProvider1.SetHelpNavigator(this.lblHatchStyle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lblHatchStyle.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.lblHatchStyle, null);
            this.lblHatchStyle.Name = "lblHatchStyle";
            this.helpProvider1.SetShowHelp(this.lblHatchStyle, ((bool)(resources.GetObject("lblHatchStyle.ShowHelp"))));
            // 
            // cmbHatchStyle
            // 
            this.cmbHatchStyle.AccessibleDescription = null;
            this.cmbHatchStyle.AccessibleName = null;
            resources.ApplyResources(this.cmbHatchStyle, "cmbHatchStyle");
            this.cmbHatchStyle.BackgroundImage = null;
            this.cmbHatchStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHatchStyle.Font = null;
            this.cmbHatchStyle.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.cmbHatchStyle, null);
            this.helpProvider1.SetHelpNavigator(this.cmbHatchStyle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cmbHatchStyle.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cmbHatchStyle, null);
            this.cmbHatchStyle.Name = "cmbHatchStyle";
            this.helpProvider1.SetShowHelp(this.cmbHatchStyle, ((bool)(resources.GetObject("cmbHatchStyle.ShowHelp"))));
            this.cmbHatchStyle.SelectedIndexChanged += new System.EventHandler(this.cmbHatchStyle_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.helpProvider1.SetHelpKeyword(this.label2, null);
            this.helpProvider1.SetHelpNavigator(this.label2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("label2.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.label2, null);
            this.label2.Name = "label2";
            this.helpProvider1.SetShowHelp(this.label2, ((bool)(resources.GetObject("label2.ShowHelp"))));
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.helpProvider1.SetHelpKeyword(this.label1, null);
            this.helpProvider1.SetHelpNavigator(this.label1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("label1.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.label1, null);
            this.label1.Name = "label1";
            this.helpProvider1.SetShowHelp(this.label1, ((bool)(resources.GetObject("label1.ShowHelp"))));
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = null;
            // 
            // cbColorSimple
            // 
            this.cbColorSimple.AccessibleDescription = null;
            this.cbColorSimple.AccessibleName = null;
            resources.ApplyResources(this.cbColorSimple, "cbColorSimple");
            this.cbColorSimple.BackgroundImage = null;
            this.cbColorSimple.BevelRadius = 4;
            this.cbColorSimple.Color = System.Drawing.Color.Blue;
            this.cbColorSimple.Font = null;
            this.helpProvider1.SetHelpKeyword(this.cbColorSimple, null);
            this.helpProvider1.SetHelpNavigator(this.cbColorSimple, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbColorSimple.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.cbColorSimple, null);
            this.cbColorSimple.LaunchDialogOnClick = true;
            this.cbColorSimple.Name = "cbColorSimple";
            this.cbColorSimple.RoundingRadius = 10;
            this.helpProvider1.SetShowHelp(this.cbColorSimple, ((bool)(resources.GetObject("cbColorSimple.ShowHelp"))));
            this.cbColorSimple.ColorChanged += new System.EventHandler(this.cbColorSimple_ColorChanged);
            // 
            // sldOpacitySimple
            // 
            this.sldOpacitySimple.AccessibleDescription = null;
            this.sldOpacitySimple.AccessibleName = null;
            resources.ApplyResources(this.sldOpacitySimple, "sldOpacitySimple");
            this.sldOpacitySimple.BackgroundImage = null;
            this.sldOpacitySimple.ColorButton = null;
            this.sldOpacitySimple.FlipRamp = false;
            this.sldOpacitySimple.FlipText = false;
            this.sldOpacitySimple.Font = null;
            this.helpProvider1.SetHelpKeyword(this.sldOpacitySimple, null);
            this.helpProvider1.SetHelpNavigator(this.sldOpacitySimple, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("sldOpacitySimple.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.sldOpacitySimple, null);
            this.sldOpacitySimple.InvertRamp = false;
            this.sldOpacitySimple.Maximum = 1;
            this.sldOpacitySimple.MaximumColor = System.Drawing.Color.CornflowerBlue;
            this.sldOpacitySimple.Minimum = 0;
            this.sldOpacitySimple.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.sldOpacitySimple.Name = "sldOpacitySimple";
            this.sldOpacitySimple.NumberFormat = null;
            this.sldOpacitySimple.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sldOpacitySimple.RampRadius = 8F;
            this.sldOpacitySimple.RampText = "Opacity";
            this.sldOpacitySimple.RampTextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.sldOpacitySimple.RampTextBehindRamp = true;
            this.sldOpacitySimple.RampTextColor = System.Drawing.Color.Black;
            this.sldOpacitySimple.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetShowHelp(this.sldOpacitySimple, ((bool)(resources.GetObject("sldOpacitySimple.ShowHelp"))));
            this.sldOpacitySimple.ShowMaximum = true;
            this.sldOpacitySimple.ShowMinimum = true;
            this.sldOpacitySimple.ShowTicks = true;
            this.sldOpacitySimple.ShowValue = false;
            this.sldOpacitySimple.SliderColor = System.Drawing.Color.SteelBlue;
            this.sldOpacitySimple.SliderRadius = 4F;
            this.sldOpacitySimple.TickColor = System.Drawing.Color.DarkGray;
            this.sldOpacitySimple.TickSpacing = 5F;
            this.sldOpacitySimple.Value = 0;
            this.sldOpacitySimple.ValueChanged += new System.EventHandler(this.sldOpacitySimple_ValueChanged);
            // 
            // dbxScaleY
            // 
            this.dbxScaleY.AccessibleDescription = null;
            this.dbxScaleY.AccessibleName = null;
            resources.ApplyResources(this.dbxScaleY, "dbxScaleY");
            this.dbxScaleY.BackColorInvalid = System.Drawing.Color.Salmon;
            this.dbxScaleY.BackColorRegular = System.Drawing.Color.Empty;
            this.dbxScaleY.BackgroundImage = null;
            this.dbxScaleY.Caption = "Y Ratio:";
            this.dbxScaleY.Font = null;
            this.helpProvider1.SetHelpKeyword(this.dbxScaleY, null);
            this.helpProvider1.SetHelpNavigator(this.dbxScaleY, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("dbxScaleY.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.dbxScaleY, null);
            this.dbxScaleY.InvalidHelp = "The value entered could not be correctly parsed into a valid double precision flo" +
                "ating point value.";
            this.dbxScaleY.IsValid = true;
            this.dbxScaleY.Name = "dbxScaleY";
            this.dbxScaleY.NumberFormat = null;
            this.dbxScaleY.RegularHelp = "Enter a double precision floating point value.";
            this.helpProvider1.SetShowHelp(this.dbxScaleY, ((bool)(resources.GetObject("dbxScaleY.ShowHelp"))));
            this.dbxScaleY.Value = 0;
            this.dbxScaleY.TextChanged += new System.EventHandler(this.dbxScaleY_TextChanged);
            // 
            // dbxScaleX
            // 
            this.dbxScaleX.AccessibleDescription = null;
            this.dbxScaleX.AccessibleName = null;
            resources.ApplyResources(this.dbxScaleX, "dbxScaleX");
            this.dbxScaleX.BackColorInvalid = System.Drawing.Color.Salmon;
            this.dbxScaleX.BackColorRegular = System.Drawing.Color.Empty;
            this.dbxScaleX.BackgroundImage = null;
            this.dbxScaleX.Caption = "X Ratio:";
            this.dbxScaleX.Font = null;
            this.helpProvider1.SetHelpKeyword(this.dbxScaleX, null);
            this.helpProvider1.SetHelpNavigator(this.dbxScaleX, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("dbxScaleX.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.dbxScaleX, null);
            this.dbxScaleX.InvalidHelp = "The value entered could not be correctly parsed into a valid double precision flo" +
                "ating point value.";
            this.dbxScaleX.IsValid = true;
            this.dbxScaleX.Name = "dbxScaleX";
            this.dbxScaleX.NumberFormat = null;
            this.dbxScaleX.RegularHelp = "Enter a double precision floating point value.";
            this.helpProvider1.SetShowHelp(this.dbxScaleX, ((bool)(resources.GetObject("dbxScaleX.ShowHelp"))));
            this.dbxScaleX.Value = 0;
            this.dbxScaleX.TextChanged += new System.EventHandler(this.dbxScaleX_TextChanged);
            // 
            // angTileAngle
            // 
            this.angTileAngle.AccessibleDescription = null;
            this.angTileAngle.AccessibleName = null;
            resources.ApplyResources(this.angTileAngle, "angTileAngle");
            this.angTileAngle.Angle = 0;
            this.angTileAngle.BackColor = System.Drawing.SystemColors.Control;
            this.angTileAngle.BackgroundImage = null;
            this.angTileAngle.Caption = "&Angle:";
            this.angTileAngle.Clockwise = false;
            this.angTileAngle.Font = null;
            this.helpProvider1.SetHelpKeyword(this.angTileAngle, null);
            this.helpProvider1.SetHelpNavigator(this.angTileAngle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("angTileAngle.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.angTileAngle, null);
            this.angTileAngle.KnobColor = System.Drawing.Color.SteelBlue;
            this.angTileAngle.Name = "angTileAngle";
            this.helpProvider1.SetShowHelp(this.angTileAngle, ((bool)(resources.GetObject("angTileAngle.ShowHelp"))));
            this.angTileAngle.StartAngle = 0;
            this.angTileAngle.AngleChanged += new System.EventHandler(this.angTileAngle_AngleChanged);
            // 
            // sliderGradient
            // 
            this.sliderGradient.AccessibleDescription = null;
            this.sliderGradient.AccessibleName = null;
            resources.ApplyResources(this.sliderGradient, "sliderGradient");
            this.sliderGradient.BackgroundImage = null;
            this.sliderGradient.EndValue = 0.8F;
            this.sliderGradient.Font = null;
            this.helpProvider1.SetHelpKeyword(this.sliderGradient, null);
            this.helpProvider1.SetHelpNavigator(this.sliderGradient, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("sliderGradient.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.sliderGradient, null);
            this.sliderGradient.MaximumColor = System.Drawing.Color.Blue;
            this.sliderGradient.MinimumColor = System.Drawing.Color.Lime;
            this.sliderGradient.Name = "sliderGradient";
            this.helpProvider1.SetShowHelp(this.sliderGradient, ((bool)(resources.GetObject("sliderGradient.ShowHelp"))));
            this.sliderGradient.SlidersEnabled = true;
            this.sliderGradient.StartValue = 0.2F;
            this.sliderGradient.GradientChanging += new System.EventHandler(this.sliderGradient_GradientChanging);
            // 
            // angGradientAngle
            // 
            this.angGradientAngle.AccessibleDescription = null;
            this.angGradientAngle.AccessibleName = null;
            resources.ApplyResources(this.angGradientAngle, "angGradientAngle");
            this.angGradientAngle.Angle = 0;
            this.angGradientAngle.BackColor = System.Drawing.SystemColors.Control;
            this.angGradientAngle.BackgroundImage = null;
            this.angGradientAngle.Caption = "&Angle:";
            this.angGradientAngle.Clockwise = false;
            this.angGradientAngle.Font = null;
            this.helpProvider1.SetHelpKeyword(this.angGradientAngle, null);
            this.helpProvider1.SetHelpNavigator(this.angGradientAngle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("angGradientAngle.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.angGradientAngle, null);
            this.angGradientAngle.KnobColor = System.Drawing.Color.SteelBlue;
            this.angGradientAngle.Name = "angGradientAngle";
            this.helpProvider1.SetShowHelp(this.angGradientAngle, ((bool)(resources.GetObject("angGradientAngle.ShowHelp"))));
            this.angGradientAngle.StartAngle = 0;
            this.angGradientAngle.AngleChanged += new System.EventHandler(this.angGradientAngle_AngleChanged);
            // 
            // hatchBackOpacity
            // 
            this.hatchBackOpacity.AccessibleDescription = null;
            this.hatchBackOpacity.AccessibleName = null;
            resources.ApplyResources(this.hatchBackOpacity, "hatchBackOpacity");
            this.hatchBackOpacity.BackgroundImage = null;
            this.hatchBackOpacity.ColorButton = null;
            this.hatchBackOpacity.FlipRamp = false;
            this.hatchBackOpacity.FlipText = false;
            this.hatchBackOpacity.Font = null;
            this.helpProvider1.SetHelpKeyword(this.hatchBackOpacity, null);
            this.helpProvider1.SetHelpNavigator(this.hatchBackOpacity, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("hatchBackOpacity.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.hatchBackOpacity, null);
            this.hatchBackOpacity.InvertRamp = false;
            this.hatchBackOpacity.Maximum = 1;
            this.hatchBackOpacity.MaximumColor = System.Drawing.Color.CornflowerBlue;
            this.hatchBackOpacity.Minimum = 0;
            this.hatchBackOpacity.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.hatchBackOpacity.Name = "hatchBackOpacity";
            this.hatchBackOpacity.NumberFormat = null;
            this.hatchBackOpacity.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.hatchBackOpacity.RampRadius = 8F;
            this.hatchBackOpacity.RampText = "Opacity";
            this.hatchBackOpacity.RampTextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.hatchBackOpacity.RampTextBehindRamp = true;
            this.hatchBackOpacity.RampTextColor = System.Drawing.Color.Black;
            this.hatchBackOpacity.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetShowHelp(this.hatchBackOpacity, ((bool)(resources.GetObject("hatchBackOpacity.ShowHelp"))));
            this.hatchBackOpacity.ShowMaximum = true;
            this.hatchBackOpacity.ShowMinimum = true;
            this.hatchBackOpacity.ShowTicks = true;
            this.hatchBackOpacity.ShowValue = false;
            this.hatchBackOpacity.SliderColor = System.Drawing.Color.Tan;
            this.hatchBackOpacity.SliderRadius = 4F;
            this.hatchBackOpacity.TickColor = System.Drawing.Color.DarkGray;
            this.hatchBackOpacity.TickSpacing = 5F;
            this.hatchBackOpacity.Value = 0;
            this.hatchBackOpacity.ValueChanged += new System.EventHandler(this.hatchBackOpacity_ValueChanged);
            // 
            // hatchBackColor
            // 
            this.hatchBackColor.AccessibleDescription = null;
            this.hatchBackColor.AccessibleName = null;
            resources.ApplyResources(this.hatchBackColor, "hatchBackColor");
            this.hatchBackColor.BackgroundImage = null;
            this.hatchBackColor.BevelRadius = 4;
            this.hatchBackColor.Color = System.Drawing.Color.Blue;
            this.hatchBackColor.Font = null;
            this.helpProvider1.SetHelpKeyword(this.hatchBackColor, null);
            this.helpProvider1.SetHelpNavigator(this.hatchBackColor, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("hatchBackColor.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.hatchBackColor, null);
            this.hatchBackColor.LaunchDialogOnClick = true;
            this.hatchBackColor.Name = "hatchBackColor";
            this.hatchBackColor.RoundingRadius = 10;
            this.helpProvider1.SetShowHelp(this.hatchBackColor, ((bool)(resources.GetObject("hatchBackColor.ShowHelp"))));
            this.hatchBackColor.ColorChanged += new System.EventHandler(this.hatchBackColor_ColorChanged);
            // 
            // hatchForeOpacity
            // 
            this.hatchForeOpacity.AccessibleDescription = null;
            this.hatchForeOpacity.AccessibleName = null;
            resources.ApplyResources(this.hatchForeOpacity, "hatchForeOpacity");
            this.hatchForeOpacity.BackgroundImage = null;
            this.hatchForeOpacity.ColorButton = null;
            this.hatchForeOpacity.FlipRamp = false;
            this.hatchForeOpacity.FlipText = false;
            this.hatchForeOpacity.Font = null;
            this.helpProvider1.SetHelpKeyword(this.hatchForeOpacity, null);
            this.helpProvider1.SetHelpNavigator(this.hatchForeOpacity, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("hatchForeOpacity.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.hatchForeOpacity, null);
            this.hatchForeOpacity.InvertRamp = false;
            this.hatchForeOpacity.Maximum = 1;
            this.hatchForeOpacity.MaximumColor = System.Drawing.Color.CornflowerBlue;
            this.hatchForeOpacity.Minimum = 0;
            this.hatchForeOpacity.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.hatchForeOpacity.Name = "hatchForeOpacity";
            this.hatchForeOpacity.NumberFormat = null;
            this.hatchForeOpacity.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.hatchForeOpacity.RampRadius = 8F;
            this.hatchForeOpacity.RampText = "Opacity";
            this.hatchForeOpacity.RampTextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.hatchForeOpacity.RampTextBehindRamp = true;
            this.hatchForeOpacity.RampTextColor = System.Drawing.Color.Black;
            this.hatchForeOpacity.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetShowHelp(this.hatchForeOpacity, ((bool)(resources.GetObject("hatchForeOpacity.ShowHelp"))));
            this.hatchForeOpacity.ShowMaximum = true;
            this.hatchForeOpacity.ShowMinimum = true;
            this.hatchForeOpacity.ShowTicks = true;
            this.hatchForeOpacity.ShowValue = false;
            this.hatchForeOpacity.SliderColor = System.Drawing.Color.Tan;
            this.hatchForeOpacity.SliderRadius = 4F;
            this.hatchForeOpacity.TickColor = System.Drawing.Color.DarkGray;
            this.hatchForeOpacity.TickSpacing = 5F;
            this.hatchForeOpacity.Value = 0;
            this.hatchForeOpacity.ValueChanged += new System.EventHandler(this.hatchForeOpacity_ValueChanged);
            // 
            // hatchForeColor
            // 
            this.hatchForeColor.AccessibleDescription = null;
            this.hatchForeColor.AccessibleName = null;
            resources.ApplyResources(this.hatchForeColor, "hatchForeColor");
            this.hatchForeColor.BackgroundImage = null;
            this.hatchForeColor.BevelRadius = 4;
            this.hatchForeColor.Color = System.Drawing.Color.Blue;
            this.hatchForeColor.Font = null;
            this.helpProvider1.SetHelpKeyword(this.hatchForeColor, null);
            this.helpProvider1.SetHelpNavigator(this.hatchForeColor, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("hatchForeColor.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.hatchForeColor, null);
            this.hatchForeColor.LaunchDialogOnClick = true;
            this.hatchForeColor.Name = "hatchForeColor";
            this.hatchForeColor.RoundingRadius = 10;
            this.helpProvider1.SetShowHelp(this.hatchForeColor, ((bool)(resources.GetObject("hatchForeColor.ShowHelp"))));
            this.hatchForeColor.ColorChanged += new System.EventHandler(this.hatchForeColor_ColorChanged);
            // 
            // modeler1
            // 
            this.modeler1.AccessibleDescription = null;
            this.modeler1.AccessibleName = null;
            this.modeler1.AllowDrop = true;
            resources.ApplyResources(this.modeler1, "modeler1");
            this.modeler1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.modeler1.BackgroundImage = null;
            this.modeler1.Cursor = System.Windows.Forms.Cursors.Default;
            this.modeler1.DataColor = System.Drawing.Color.LightGreen;
            this.modeler1.DataFont = new System.Drawing.Font("Tahoma", 8F);
            this.modeler1.DataShape = MapWindow.Tools.ModelShapes.Ellipse;
            this.modeler1.DefaultFileExtension = "mwm";
            this.modeler1.DrawingQuality = System.Drawing.Drawing2D.SmoothingMode.Default;
            this.modeler1.EnableLinking = false;
            this.modeler1.Font = null;
            this.helpProvider1.SetHelpKeyword(this.modeler1, null);
            this.helpProvider1.SetHelpNavigator(this.modeler1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("modeler1.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.modeler1, null);
            this.modeler1.IsInitialized = true;
            this.modeler1.MaxExecutionThreads = 2;
            this.modeler1.ModelFilename = null;
            this.modeler1.Name = "modeler1";
            this.helpProvider1.SetShowHelp(this.modeler1, ((bool)(resources.GetObject("modeler1.ShowHelp"))));
            this.modeler1.ShowWaterMark = true;
            this.modeler1.ToolColor = System.Drawing.Color.Khaki;
            this.modeler1.ToolFont = new System.Drawing.Font("Tahoma", 8F);
            this.modeler1.ToolManager = null;
            this.modeler1.ToolShape = MapWindow.Tools.ModelShapes.Rectangle;
            this.modeler1.WorkingPath = null;
            this.modeler1.ZoomFactor = 1F;
            // 
            // ocOutline
            // 
            this.ocOutline.AccessibleDescription = null;
            this.ocOutline.AccessibleName = null;
            resources.ApplyResources(this.ocOutline, "ocOutline");
            this.ocOutline.BackgroundImage = null;
            this.ocOutline.Font = null;
            this.helpProvider1.SetHelpKeyword(this.ocOutline, null);
            this.helpProvider1.SetHelpNavigator(this.ocOutline, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("ocOutline.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.ocOutline, null);
            this.ocOutline.Name = "ocOutline";
            this.ocOutline.Pattern = null;
            this.helpProvider1.SetShowHelp(this.ocOutline, ((bool)(resources.GetObject("ocOutline.ShowHelp"))));
            this.ocOutline.OutlineChanged += new System.EventHandler(this.ocOutline_OutlineChanged);
            // 
            // ccPatterns
            // 
            this.ccPatterns.AccessibleDescription = null;
            this.ccPatterns.AccessibleName = null;
            resources.ApplyResources(this.ccPatterns, "ccPatterns");
            this.ccPatterns.BackgroundImage = null;
            this.ccPatterns.Font = null;
            this.helpProvider1.SetHelpKeyword(this.ccPatterns, null);
            this.helpProvider1.SetHelpNavigator(this.ccPatterns, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("ccPatterns.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this.ccPatterns, null);
            this.ccPatterns.Name = "ccPatterns";
            this.helpProvider1.SetShowHelp(this.ccPatterns, ((bool)(resources.GetObject("ccPatterns.ShowHelp"))));
            this.ccPatterns.Load += new System.EventHandler(this.ccPatterns_Load);
            // 
            // DetailedPolygonSymbolControl
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAddToCustom);
            this.Controls.Add(this.modeler1);
            this.Controls.Add(this.ocOutline);
            this.Controls.Add(this.tabPatternProperties);
            this.Controls.Add(this.lblPatternType);
            this.Controls.Add(this.cmbPatternType);
            this.Controls.Add(this.ccPatterns);
            this.Font = null;
            this.helpProvider1.SetHelpKeyword(this, null);
            this.helpProvider1.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.helpProvider1.SetHelpString(this, null);
            this.Name = "DetailedPolygonSymbolControl";
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPatternProperties.ResumeLayout(false);
            this.tabSimple.ResumeLayout(false);
            this.tabSimple.PerformLayout();
            this.tabPicture.ResumeLayout(false);
            this.tabPicture.PerformLayout();
            this.tabGradient.ResumeLayout(false);
            this.tabGradient.PerformLayout();
            this.tabHatch.ResumeLayout(false);
            this.tabHatch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of CollectionPropertyGrid
        /// </summary>
        public DetailedPolygonSymbolControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new dialog where the properties displayed on the form are a duplicate of the original properties,
        /// and those properties will be copied back to the original on an apply changes or an ok click.
        /// </summary>
        /// <param name="original"></param>
        public DetailedPolygonSymbolControl(IPolygonSymbolizer original)
        {
            InitializeComponent();
            Configure();
            Initialize(original);
        }


       

        private void Configure()
        {
            ccPatterns.SelectedItemChanged += ccPatterns_SelectedItemChanged;
            ccPatterns.AddClicked += ccPatterns_AddClicked;
            lblPreview.Paint += lblPreview_Paint;
            ocOutline.ChangesApplied += ocOutline_ChangesApplied;

            
        }

        void ocOutline_ChangesApplied(object sender, EventArgs e)
        {
            _original.CopyProperties(_symbolizer);
        }

        void lblPreview_Paint(object sender, PaintEventArgs e)
        {
            UpdatePreview(e.Graphics);
        }

        
       
        #endregion

        #region Methods

        /// <summary>
        /// Assigns the original symbolizer to this control
        /// </summary>
        /// <param name="original">The polygon symbolizer to modify.</param>
        public void Initialize(IPolygonSymbolizer original)
        {
            _symbolizer = original.Copy();
            _original = original;
            ccPatterns.Patterns = _symbolizer.Patterns;
            ccPatterns.RefreshList();
            if (_symbolizer.Patterns.Count > 0)
            {
                ccPatterns.SelectedPattern = _symbolizer.Patterns[0];
            }
            ocOutline.Pattern = ccPatterns.SelectedPattern;
            UpdatePreview();
            UpdatePatternControls();

        }

        /// <summary>
        /// Forces the current settings to be written back to the original symbolizer
        /// </summary>
        public void ApplyChanges()
        {
            OnApplyChanges();
        }

        #endregion

        #region Properties

        #endregion

        #region Events

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
            _original.CopyProperties(_symbolizer);            
            if (ChangesApplied != null) ChangesApplied(this, new EventArgs());
        }

        /// <summary>
        /// Fires the AddToCustomSymbols event
        /// </summary>
        protected virtual void OnAddToCustomSymbols()
        {
            if (AddToCustomSymbols != null) AddToCustomSymbols(this, new PolygonSymbolizerEventArgs(_symbolizer));
        }
        


        #endregion

        #region Private Functions

        private void UpdatePatternControls()
        {
            _ignoreChanges = true;
            cmbScaleMode.SelectedItem = _symbolizer.ScaleMode.ToString();
            chkSmoothing.Checked = _symbolizer.Smoothing;
            _disableUnitWarning = true;
            cmbUnits.SelectedItem = _symbolizer.Units.ToString();
            _disableUnitWarning = false;
            ocOutline.Pattern = ccPatterns.SelectedPattern;
            ISimplePattern sp = ccPatterns.SelectedPattern as ISimplePattern;
            if (sp != null)
            {
                UpdateSimplePatternControls(sp);
            }
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                UpdatePicturePatternControls(pp);
            }
            IGradientPattern gp = ccPatterns.SelectedPattern as IGradientPattern;
            if (gp != null)
            {
                UpdateGradientPatternControls(gp);
            }
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
            {
                UpdateHatchPatternControls(hp);
            }
            _ignoreChanges = false;
            UpdatePreview();
        }

        private void UpdateGradientPatternControls(IGradientPattern pattern)
        {
            cmbPatternType.SelectedItem = "Gradient";
            cmbGradientType.SelectedItem = pattern.GradientType.ToString();
            sliderGradient.MinimumColor = pattern.Colors[0];
            sliderGradient.MaximumColor = pattern.Colors[pattern.Colors.Length - 1];
            angGradientAngle.Angle = (int)pattern.Angle;
        }

        private void UpdatePicturePatternControls(IPicturePattern pattern)
        {
            cmbPatternType.SelectedItem = "Picture";
            txtImage.Text = System.IO.Path.GetFileName(pattern.PictureFilename);
            cmbTileMode.SelectedItem = pattern.WrapMode.ToString();
            angTileAngle.Angle = (int)pattern.Angle;
            dbxScaleX.Value = pattern.Scale.X;
            dbxScaleY.Value = pattern.Scale.Y;
            
        }

        private void UpdateSimplePatternControls(ISimplePattern pattern)
        {
            cmbPatternType.SelectedItem = "Simple";
            cbColorSimple.Color = pattern.FillColor;
            sldOpacitySimple.Value = pattern.Opacity;
            sldOpacitySimple.MaximumColor = pattern.FillColor.ToOpaque();
        }

        private void UpdateHatchPatternControls(IHatchPattern pattern)
        {
            cmbPatternType.SelectedItem = "Hatch";
            cmbHatchStyle.SelectedItem = pattern.HatchStyle;
            hatchForeColor.Color = pattern.ForeColor;
            hatchForeOpacity.Value = pattern.ForeColorOpacity;
            hatchBackColor.Color = pattern.BackColor;
            hatchBackOpacity.Value = pattern.BackColorOpacity;
        }

        private void UpdatePreview(Graphics g)
        {
            g.Clear(Color.White);
          
            _symbolizer.Draw(g, new Rectangle(5, 5, lblPreview.Width - 10, lblPreview.Height - 10));
        }

        private void UpdatePreview()
        {
            ccPatterns.Refresh();
            Graphics g = lblPreview.CreateGraphics();
            UpdatePreview(g);
            g.Dispose();
        }

        #endregion

        #region Event Handlers

       

        void ccPatterns_AddClicked(object sender, EventArgs e)
        {
            string patternType = (string)cmbPatternType.SelectedItem;
            switch (patternType)
            {
                case "Simple":
                    ccPatterns.Patterns.Insert(0, new SimplePattern());
                    break;
                case "Picture":
                    ccPatterns.Patterns.Insert(0, new PicturePattern());
                    break;
                case "Gradient":
                    ccPatterns.Patterns.Insert(0, new GradientPattern());
                    break;
            }
            ccPatterns.RefreshList();
            ccPatterns.SelectedPattern = ccPatterns.Patterns[0];
            UpdatePreview();
        }


        void ccPatterns_SelectedItemChanged(object sender, EventArgs e)
        {
            if (ccPatterns.SelectedPattern != null)
            {
                UpdatePatternControls();
            }
            UpdatePreview();
        }



       


        private void cmbScaleMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            _symbolizer.ScaleMode = Global.ParseEnum<ScaleModes>(cmbScaleMode.SelectedItem.ToString());
        }

        private void chkSmoothing_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            _symbolizer.Smoothing = chkSmoothing.Checked;
            UpdatePreview();
        }

        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            if (_disableUnitWarning) return;
            if (cmbUnits.SelectedItem.ToString() == "World" && _symbolizer.ScaleMode != ScaleModes.Geographic)
            {
                if (MessageBox.Show("Chosing this option will force the drawing to use a Geographic ScaleMode.  Do you wish to continue?", "Change Scale Mode?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    cmbUnits.SelectedItem = _symbolizer.Units.ToString();
                    return;
                }
                _symbolizer.ScaleMode = ScaleModes.Geographic;
            }
            if (cmbUnits.SelectedItem.ToString() != "World" && _symbolizer.ScaleMode == ScaleModes.Geographic)
            {
                if (MessageBox.Show("Chosing this option will force the drawing to use a Symbolic scalemode.  Do you wish to continue?", "Change Scale Mode?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    cmbUnits.SelectedItem = _symbolizer.Units.ToString();
                    return;
                }
                _symbolizer.ScaleMode = ScaleModes.Symbolic;
            }
            GraphicsUnit destination = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), cmbUnits.SelectedItem.ToString());

            GraphicsUnit source = _symbolizer.Units;
            if (source == GraphicsUnit.Inch && destination == GraphicsUnit.Millimeter)
            {
            }
            if (source == GraphicsUnit.Millimeter && destination == GraphicsUnit.Inch)
            {
            }

            UpdatePatternControls();
        }

        private void cbColorSimple_ColorChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            ISimplePattern sp = ccPatterns.SelectedPattern as ISimplePattern;
            if (sp != null)
            {
                sp.FillColor = cbColorSimple.Color;
            }
            sldOpacitySimple.MaximumColor = cbColorSimple.Color.ToOpaque();
            UpdatePreview();
        }

        private void sldOpacitySimple_ValueChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            ISimplePattern sp = ccPatterns.SelectedPattern as ISimplePattern;
            if (sp != null)
            {
                sp.Opacity = (float)sldOpacitySimple.Value;
                _ignoreChanges = true;
                cbColorSimple.Color = sp.FillColor;
                _ignoreChanges = false;
            }
            UpdatePreview();
        }

       

        private void btnAddToCustom_Click(object sender, EventArgs e)
        {
            OnAddToCustomSymbols();
        }

        private void cmbPatternType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)cmbPatternType.SelectedItem == "Simple")
            {
                if (tabPatternProperties.TabPages.Contains(tabPicture))
                {
                    tabPatternProperties.TabPages.Remove(tabPicture);
                }
                if (tabPatternProperties.TabPages.Contains(tabGradient))
                {
                    tabPatternProperties.TabPages.Remove(tabGradient);
                }
                if (tabPatternProperties.TabPages.Contains(tabHatch))
                {
                    tabPatternProperties.TabPages.Remove(tabHatch);
                }
                if (tabPatternProperties.TabPages.Contains(tabSimple) == false)
                {
                    tabPatternProperties.TabPages.Add(tabSimple);
                    tabPatternProperties.SelectedTab = tabSimple;
                }
            }
            if ((string)cmbPatternType.SelectedItem == "Picture")
            {
                if (tabPatternProperties.TabPages.Contains(tabSimple))
                {
                    tabPatternProperties.TabPages.Remove(tabSimple);
                }
                if (tabPatternProperties.TabPages.Contains(tabGradient))
                {
                    tabPatternProperties.TabPages.Remove(tabGradient);
                }
                if (tabPatternProperties.TabPages.Contains(tabHatch))
                {
                    tabPatternProperties.TabPages.Remove(tabHatch);
                }
                if (tabPatternProperties.TabPages.Contains(tabPicture) == false)
                {
                    tabPatternProperties.TabPages.Add(tabPicture);
                    tabPatternProperties.SelectedTab = tabPicture;
                }
            }
            if ((string)cmbPatternType.SelectedItem == "Gradient")
            {
                if (tabPatternProperties.TabPages.Contains(tabSimple))
                {
                    tabPatternProperties.TabPages.Remove(tabSimple);
                }
                if (tabPatternProperties.TabPages.Contains(tabPicture))
                {
                    tabPatternProperties.TabPages.Remove(tabPicture);
                }
                if (tabPatternProperties.TabPages.Contains(tabHatch))
                {
                    tabPatternProperties.TabPages.Remove(tabHatch);
                }
                if (tabPatternProperties.TabPages.Contains(tabGradient) == false)
                {
                    tabPatternProperties.TabPages.Add(tabGradient);
                    tabPatternProperties.SelectedTab = tabGradient;
                }
            }
            if ((string)cmbPatternType.SelectedItem == "Hatch")
            {
                if (tabPatternProperties.TabPages.Contains(tabSimple))
                {
                    tabPatternProperties.TabPages.Remove(tabSimple);
                }
                if (tabPatternProperties.TabPages.Contains(tabPicture))
                {
                    tabPatternProperties.TabPages.Remove(tabPicture);
                }
                if (tabPatternProperties.TabPages.Contains(tabGradient))
                {
                    tabPatternProperties.TabPages.Remove(tabGradient);
                }
                if (tabPatternProperties.TabPages.Contains(tabHatch) == false)
                {
                    tabPatternProperties.TabPages.Add(tabHatch);
                    tabPatternProperties.SelectedTab = tabHatch;
                }
            }
            if (_ignoreChanges) return;
            int index = ccPatterns.Patterns.IndexOf(ccPatterns.SelectedPattern);
            if (index == -1) return;
            IPattern oldPattern = ccPatterns.SelectedPattern;
            if ((string)cmbPatternType.SelectedItem == "Simple")
            {
                SimplePattern sp = new SimplePattern();
                if (oldPattern != null) sp.CopyOutline(oldPattern);
                ccPatterns.Patterns[index] = sp;
                ccPatterns.RefreshList();
                ccPatterns.SelectedPattern = sp;
                UpdateSimplePatternControls(sp);
            }
            if ((string)cmbPatternType.SelectedItem == "Picture")
            {
                PicturePattern pp = new PicturePattern();
                if (oldPattern != null) pp.CopyOutline(oldPattern);
                ccPatterns.Patterns[index] = pp;
                ccPatterns.RefreshList();
                ccPatterns.SelectedPattern = pp;
                UpdatePicturePatternControls(pp);
            }
            if ((string)cmbPatternType.SelectedItem == "Gradient")
            {
                GradientPattern gp = new GradientPattern();
                if (oldPattern != null) gp.CopyOutline(oldPattern);
                ccPatterns.Patterns[index] = gp;
                ccPatterns.RefreshList();
                ccPatterns.SelectedPattern = gp;
                UpdateGradientPatternControls(gp);
            }
            if ((string)cmbPatternType.SelectedItem == "Hatch")
            {
                HatchPattern hp = new HatchPattern();
                if (oldPattern != null) hp.CopyOutline(oldPattern);
                ccPatterns.Patterns[index] = hp;
                ccPatterns.RefreshList();
                ccPatterns.SelectedPattern = hp;
            }
            
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if(_ignoreChanges)return;
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = pp.DialogFilter;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                pp.Open(ofd.FileName);
                txtImage.Text = System.IO.Path.GetFileName(ofd.FileName);
            }
            UpdatePreview();
        }



       

        private void cmbTileMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                pp.WrapMode = Global.ParseEnum<WrapMode>((string)cmbTileMode.SelectedItem);
            }
            UpdatePreview();
        }

        private void angTileAngle_AngleChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                pp.Angle = angTileAngle.Angle;
            }
            UpdatePreview();
        }

        private void dbxScaleX_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                pp.Scale.X = dbxScaleX.Value;
            }
            UpdatePreview();
        }

        private void dbxScaleY_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IPicturePattern pp = ccPatterns.SelectedPattern as IPicturePattern;
            if (pp != null)
            {
                pp.Scale.Y = dbxScaleY.Value;
            }
            UpdatePreview();
        }


        #endregion

     

        private void ocOutline_OutlineChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void angGradientAngle_AngleChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IGradientPattern gp = ccPatterns.SelectedPattern as IGradientPattern;
            if (gp != null)
            {
                gp.Angle = angGradientAngle.Angle;
            }
            UpdatePreview();
        }

        private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IGradientPattern gp = ccPatterns.SelectedPattern as IGradientPattern;
            if (gp != null)
            {
                gp.GradientType = Global.ParseEnum<GradientTypes>((string)cmbGradientType.SelectedItem);
                if (gp.GradientType == GradientTypes.Linear)
                {
                    lblStartColor.Text = "Start Color";
                    lblEndColor.Text = "End Color";
                }
                else
                {
                    lblStartColor.Text = "Surround Color";
                    lblEndColor.Text = "Center Color";
                }
            }
            
            UpdatePreview();
        }

        private void sliderGradient_GradientChanging(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IGradientPattern gp = ccPatterns.SelectedPattern as IGradientPattern;
            if (gp != null)
            {
                gp.Colors = new[] { sliderGradient.MinimumColor, sliderGradient.MinimumColor, sliderGradient.MaximumColor, sliderGradient.MaximumColor };
                gp.Positions = new[] { 0F, sliderGradient.StartValue, sliderGradient.EndValue, 1F };
            }
            UpdatePreview();
        }

        private void hatchForeColor_ColorChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
                hp.ForeColor = hatchForeColor.Color;
            UpdatePreview();
        }

        private void hatchBackColor_ColorChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
                hp.BackColor = hatchBackColor.Color;
            UpdatePreview();
        }

        private void hatchForeOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
                hp.ForeColorOpacity = (float)hatchForeOpacity.Value;
            UpdatePreview();
        }

        private void hatchBackOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
                hp.BackColorOpacity = (float)hatchBackOpacity.Value;
            UpdatePreview();
        }

        private void ccPatterns_Load(object sender, EventArgs e)
        {
            Array hatchs = Enum.GetValues(typeof(HatchStyle));
            foreach (object style in hatchs)
                cmbHatchStyle.Items.Add(style);
        }

        private void cmbHatchStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChanges) return;
            IHatchPattern hp = ccPatterns.SelectedPattern as IHatchPattern;
            if (hp != null)
                hp.HatchStyle = (HatchStyle)cmbHatchStyle.SelectedItem;
            UpdatePreview();
        }
      
    }
}