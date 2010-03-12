﻿namespace MapWindow.Forms
{
    /// <summary>
    /// Presumably a form for performing calculations
    /// </summary>
    partial class AttributeCalculator
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeCalculator));
            this.lblFieldTitle = new System.Windows.Forms.Label();
            this.lstViewFields = new System.Windows.Forms.ListView();
            this.lstBoxFunctions = new System.Windows.Forms.ListBox();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.btnClaculate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnMultiply = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.lblDestinationFieldTitle = new System.Windows.Forms.Label();
            this.comDestFieldComboBox = new System.Windows.Forms.ComboBox();
            this.lblAssignment = new System.Windows.Forms.Label();
            this.rtxtComputaion = new System.Windows.Forms.RichTextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblComputaion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFieldTitle
            // 
            this.lblFieldTitle.AccessibleDescription = null;
            this.lblFieldTitle.AccessibleName = null;
            resources.ApplyResources(this.lblFieldTitle, "lblFieldTitle");
            this.lblFieldTitle.Font = null;
            this.lblFieldTitle.Name = "lblFieldTitle";
            // 
            // lstViewFields
            // 
            this.lstViewFields.AccessibleDescription = null;
            this.lstViewFields.AccessibleName = null;
            resources.ApplyResources(this.lstViewFields, "lstViewFields");
            this.lstViewFields.BackgroundImage = null;
            this.lstViewFields.FullRowSelect = true;
            this.lstViewFields.Name = "lstViewFields";
            this.lstViewFields.ShowItemToolTips = true;
            this.lstViewFields.TileSize = new System.Drawing.Size(247, 30);
            this.lstViewFields.UseCompatibleStateImageBehavior = false;
            this.lstViewFields.View = System.Windows.Forms.View.SmallIcon;
            this.lstViewFields.SelectedIndexChanged += new System.EventHandler(this.lstViewFields_SelectedIndexChanged);
            this.lstViewFields.DoubleClick += new System.EventHandler(this.lstViewFields_DoubleClick);
            // 
            // lstBoxFunctions
            // 
            this.lstBoxFunctions.AccessibleDescription = null;
            this.lstBoxFunctions.AccessibleName = null;
            resources.ApplyResources(this.lstBoxFunctions, "lstBoxFunctions");
            this.lstBoxFunctions.BackgroundImage = null;
            this.lstBoxFunctions.Font = null;
            this.lstBoxFunctions.FormattingEnabled = true;
            this.lstBoxFunctions.Items.AddRange(new object[] {
            resources.GetString("lstBoxFunctions.Items"),
            resources.GetString("lstBoxFunctions.Items1"),
            resources.GetString("lstBoxFunctions.Items2"),
            resources.GetString("lstBoxFunctions.Items3"),
            resources.GetString("lstBoxFunctions.Items4"),
            resources.GetString("lstBoxFunctions.Items5"),
            resources.GetString("lstBoxFunctions.Items6"),
            resources.GetString("lstBoxFunctions.Items7"),
            resources.GetString("lstBoxFunctions.Items8"),
            resources.GetString("lstBoxFunctions.Items9"),
            resources.GetString("lstBoxFunctions.Items10"),
            resources.GetString("lstBoxFunctions.Items11"),
            resources.GetString("lstBoxFunctions.Items12"),
            resources.GetString("lstBoxFunctions.Items13"),
            resources.GetString("lstBoxFunctions.Items14"),
            resources.GetString("lstBoxFunctions.Items15"),
            resources.GetString("lstBoxFunctions.Items16"),
            resources.GetString("lstBoxFunctions.Items17"),
            resources.GetString("lstBoxFunctions.Items18"),
            resources.GetString("lstBoxFunctions.Items19"),
            resources.GetString("lstBoxFunctions.Items20"),
            resources.GetString("lstBoxFunctions.Items21"),
            resources.GetString("lstBoxFunctions.Items22"),
            resources.GetString("lstBoxFunctions.Items23"),
            resources.GetString("lstBoxFunctions.Items24"),
            resources.GetString("lstBoxFunctions.Items25"),
            resources.GetString("lstBoxFunctions.Items26"),
            resources.GetString("lstBoxFunctions.Items27"),
            resources.GetString("lstBoxFunctions.Items28"),
            resources.GetString("lstBoxFunctions.Items29"),
            resources.GetString("lstBoxFunctions.Items30"),
            resources.GetString("lstBoxFunctions.Items31"),
            resources.GetString("lstBoxFunctions.Items32"),
            resources.GetString("lstBoxFunctions.Items33"),
            resources.GetString("lstBoxFunctions.Items34"),
            resources.GetString("lstBoxFunctions.Items35"),
            resources.GetString("lstBoxFunctions.Items36"),
            resources.GetString("lstBoxFunctions.Items37"),
            resources.GetString("lstBoxFunctions.Items38"),
            resources.GetString("lstBoxFunctions.Items39"),
            resources.GetString("lstBoxFunctions.Items40"),
            resources.GetString("lstBoxFunctions.Items41"),
            resources.GetString("lstBoxFunctions.Items42"),
            resources.GetString("lstBoxFunctions.Items43"),
            resources.GetString("lstBoxFunctions.Items44"),
            resources.GetString("lstBoxFunctions.Items45"),
            resources.GetString("lstBoxFunctions.Items46"),
            resources.GetString("lstBoxFunctions.Items47"),
            resources.GetString("lstBoxFunctions.Items48"),
            resources.GetString("lstBoxFunctions.Items49"),
            resources.GetString("lstBoxFunctions.Items50"),
            resources.GetString("lstBoxFunctions.Items51"),
            resources.GetString("lstBoxFunctions.Items52"),
            resources.GetString("lstBoxFunctions.Items53"),
            resources.GetString("lstBoxFunctions.Items54"),
            resources.GetString("lstBoxFunctions.Items55"),
            resources.GetString("lstBoxFunctions.Items56"),
            resources.GetString("lstBoxFunctions.Items57"),
            resources.GetString("lstBoxFunctions.Items58"),
            resources.GetString("lstBoxFunctions.Items59"),
            resources.GetString("lstBoxFunctions.Items60"),
            resources.GetString("lstBoxFunctions.Items61"),
            resources.GetString("lstBoxFunctions.Items62"),
            resources.GetString("lstBoxFunctions.Items63"),
            resources.GetString("lstBoxFunctions.Items64"),
            resources.GetString("lstBoxFunctions.Items65"),
            resources.GetString("lstBoxFunctions.Items66"),
            resources.GetString("lstBoxFunctions.Items67"),
            resources.GetString("lstBoxFunctions.Items68"),
            resources.GetString("lstBoxFunctions.Items69"),
            resources.GetString("lstBoxFunctions.Items70"),
            resources.GetString("lstBoxFunctions.Items71"),
            resources.GetString("lstBoxFunctions.Items72"),
            resources.GetString("lstBoxFunctions.Items73"),
            resources.GetString("lstBoxFunctions.Items74")});
            this.lstBoxFunctions.Name = "lstBoxFunctions";
            this.lstBoxFunctions.SelectedIndexChanged += new System.EventHandler(this.lstBoxFunctions_SelectedIndexChanged);
            this.lstBoxFunctions.DoubleClick += new System.EventHandler(this.lstBoxFunctions_DoubleClick);
            // 
            // lblFunctions
            // 
            this.lblFunctions.AccessibleDescription = null;
            this.lblFunctions.AccessibleName = null;
            resources.ApplyResources(this.lblFunctions, "lblFunctions");
            this.lblFunctions.Font = null;
            this.lblFunctions.Name = "lblFunctions";
            // 
            // btnClaculate
            // 
            this.btnClaculate.AccessibleDescription = null;
            this.btnClaculate.AccessibleName = null;
            resources.ApplyResources(this.btnClaculate, "btnClaculate");
            this.btnClaculate.BackgroundImage = null;
            this.btnClaculate.Font = null;
            this.btnClaculate.Name = "btnClaculate";
            this.btnClaculate.UseVisualStyleBackColor = true;
            this.btnClaculate.Click += new System.EventHandler(this.btnClaculate_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleDescription = null;
            this.btnClose.AccessibleName = null;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.BackgroundImage = null;
            this.btnClose.Font = null;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPlus
            // 
            this.btnPlus.AccessibleDescription = null;
            this.btnPlus.AccessibleName = null;
            resources.ApplyResources(this.btnPlus, "btnPlus");
            this.btnPlus.BackgroundImage = null;
            this.btnPlus.Font = null;
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.UseVisualStyleBackColor = true;
            this.btnPlus.Click += new System.EventHandler(this.btnPlus_Click);
            // 
            // btnDivide
            // 
            this.btnDivide.AccessibleDescription = null;
            this.btnDivide.AccessibleName = null;
            resources.ApplyResources(this.btnDivide, "btnDivide");
            this.btnDivide.BackgroundImage = null;
            this.btnDivide.Font = null;
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.btnDivide_Click);
            // 
            // btnMultiply
            // 
            this.btnMultiply.AccessibleDescription = null;
            this.btnMultiply.AccessibleName = null;
            resources.ApplyResources(this.btnMultiply, "btnMultiply");
            this.btnMultiply.BackgroundImage = null;
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.UseVisualStyleBackColor = true;
            this.btnMultiply.Click += new System.EventHandler(this.btnMultiply_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.AccessibleDescription = null;
            this.btnMinus.AccessibleName = null;
            resources.ApplyResources(this.btnMinus, "btnMinus");
            this.btnMinus.BackgroundImage = null;
            this.btnMinus.Font = null;
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lblDestinationFieldTitle
            // 
            this.lblDestinationFieldTitle.AccessibleDescription = null;
            this.lblDestinationFieldTitle.AccessibleName = null;
            resources.ApplyResources(this.lblDestinationFieldTitle, "lblDestinationFieldTitle");
            this.lblDestinationFieldTitle.Font = null;
            this.lblDestinationFieldTitle.Name = "lblDestinationFieldTitle";
            // 
            // comDestFieldComboBox
            // 
            this.comDestFieldComboBox.AccessibleDescription = null;
            this.comDestFieldComboBox.AccessibleName = null;
            resources.ApplyResources(this.comDestFieldComboBox, "comDestFieldComboBox");
            this.comDestFieldComboBox.BackgroundImage = null;
            this.comDestFieldComboBox.Font = null;
            this.comDestFieldComboBox.FormattingEnabled = true;
            this.comDestFieldComboBox.Name = "comDestFieldComboBox";
            // 
            // lblAssignment
            // 
            this.lblAssignment.AccessibleDescription = null;
            this.lblAssignment.AccessibleName = null;
            resources.ApplyResources(this.lblAssignment, "lblAssignment");
            this.lblAssignment.Name = "lblAssignment";
            // 
            // rtxtComputaion
            // 
            this.rtxtComputaion.AccessibleDescription = null;
            this.rtxtComputaion.AccessibleName = null;
            resources.ApplyResources(this.rtxtComputaion, "rtxtComputaion");
            this.rtxtComputaion.BackgroundImage = null;
            this.rtxtComputaion.ForeColor = System.Drawing.SystemColors.Highlight;
            this.rtxtComputaion.Name = "rtxtComputaion";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AccessibleDescription = null;
            this.linkLabel1.AccessibleName = null;
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Font = null;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lblComputaion
            // 
            this.lblComputaion.AccessibleDescription = null;
            this.lblComputaion.AccessibleName = null;
            resources.ApplyResources(this.lblComputaion, "lblComputaion");
            this.lblComputaion.Font = null;
            this.lblComputaion.Name = "lblComputaion";
            // 
            // AttributeCalculator
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.lblComputaion);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.rtxtComputaion);
            this.Controls.Add(this.lblAssignment);
            this.Controls.Add(this.comDestFieldComboBox);
            this.Controls.Add(this.lblDestinationFieldTitle);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.btnMultiply);
            this.Controls.Add(this.btnDivide);
            this.Controls.Add(this.btnPlus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnClaculate);
            this.Controls.Add(this.lblFunctions);
            this.Controls.Add(this.lstBoxFunctions);
            this.Controls.Add(this.lstViewFields);
            this.Controls.Add(this.lblFieldTitle);
            this.Font = null;
            this.Name = "AttributeCalculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        void lstBoxFunctions_DoubleClick(object sender, System.EventArgs e)
        {
            string Fun1Full = "Abs( x ) Atn( x ) Cos( x ) Exp( x ) Fix( x ) Int( x ) Ln( x ) Log( x ) Rnd( x ) Sgn( x ) Sin( x ) Sqr( x ) Cbr( x ) Tan( x ) Acos( x ) Asin( x ) " + " Cosh( x ) Sinh( x ) Tanh( x ) Acosh( x ) Asinh( x ) Atanh( x ) Fact( x ) Not( x ) Erf( x ) Gamma( x ) Gammaln( x ) Digamma( x ) Zeta( x ) Ei( x ) " + " csc( x ) sec( x ) cot( x ) acsc( x ) asec( x ) acot( x ) csch( x ) sech( x ) coth( x ) acsch( x ) asech( x ) acoth( x ) Dec( x ) Rad( x ) Deg( x ) Grad( x ) ";
            string Fun2Full = "Comb( n,k ) Max( a,b ) Min( a,b ) Mcm( a,b ) Mcd( a,b ) Lcm( a,b ) Gcd( a,b ) Mod( a,b ) Beta( a,b ) Root( x,n ) Round( x,d )";
            string temp;

            temp = lstBoxFunctions.SelectedItem.ToString();
            if (Fun1Full.ToLower().Contains(temp.ToLower()))// check mono variable
            {
                if (temp.Length > 2)
                {
                    //temp = "[" + temp +"]";
                    temp = " " + temp + " ";
                    if (temp != null)
                    {
                        Expression = rtxtComputaion.Text;
                        Expression = Expression + temp;
                        //DisplyExpression();
                        rtxtComputaion.Focus();
                        rtxtComputaion.Text = Expression;
                        rtxtComputaion.Select(Expression.Length - 4,1);
                        
                    }
                }
            }
            else if (Fun2Full.ToLower().Contains(temp.ToLower())) // Check di Variable
            {

                if (temp.Length > 4)
                {
                    //temp = "[" + temp +"]";
                    temp = " " + temp + " ";
                    if (temp != null)
                    {
                        Expression = rtxtComputaion.Text;
                        Expression = Expression + temp;
                        //DisplyExpression();
                        rtxtComputaion.Focus();
                        rtxtComputaion.Text = Expression;
                        rtxtComputaion.Select(Expression.Length - 6, 1);
                    }
                }
            }
            else
            {
                //temp = "[" + temp + "]"; //symbols
                temp = " " + temp + " "; //symbols
                if (temp != null)
                {
                    Expression = rtxtComputaion.Text;
                    Expression = Expression + temp;
                    DisplyExpression();
                    
                }
            }

        }

        void lstViewFields_DoubleClick(object sender, System.EventArgs e)
        {
            string temp;
            System.Windows.Forms.ListViewItem item;
            item = lstViewFields.FocusedItem;
            temp = item.Text;
            temp = temp.Remove(temp.Length - 1, 1);
            temp = "[" + temp + "]";
            if (item != null)
            {
                Expression = rtxtComputaion.Text;
                Expression = Expression + temp;
                DisplyExpression();
            }
        }

        #endregion

        private System.Windows.Forms.Label lblFieldTitle;
        private System.Windows.Forms.ListBox lstBoxFunctions;
        private System.Windows.Forms.Label lblFunctions;
        private System.Windows.Forms.Button btnClaculate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnMultiply;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Label lblDestinationFieldTitle;
        private System.Windows.Forms.ComboBox comDestFieldComboBox;
        private System.Windows.Forms.Label lblAssignment;
        private System.Windows.Forms.RichTextBox rtxtComputaion;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label lblComputaion;
        private System.Windows.Forms.ListView lstViewFields;
    }
}