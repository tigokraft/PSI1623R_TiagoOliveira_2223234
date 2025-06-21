namespace login.Tabs
{
    partial class Incomes
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
            this.label2 = new System.Windows.Forms.Label();
            this.AddBtn = new Guna.UI2.WinForms.Guna2Button();
            this.closeapp = new Guna.UI2.WinForms.Guna2ControlBox();
            this.cmbCat = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cmbMonths = new Guna.UI2.WinForms.Guna2ComboBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(25, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Incomes";
            // 
            // AddBtn
            // 
            this.AddBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.AddBtn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.AddBtn.BorderRadius = 6;
            this.AddBtn.BorderThickness = 1;
            this.AddBtn.CheckedState.Parent = this.AddBtn;
            this.AddBtn.CustomImages.Parent = this.AddBtn;
            this.AddBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.AddBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.AddBtn.ForeColor = System.Drawing.Color.White;
            this.AddBtn.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.AddBtn.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(34)))), ((int)(((byte)(36)))));
            this.AddBtn.HoverState.Parent = this.AddBtn;
            this.AddBtn.Image = global::login.Properties.Resources.plus_white;
            this.AddBtn.Location = new System.Drawing.Point(695, 17);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.ShadowDecoration.Parent = this.AddBtn;
            this.AddBtn.Size = new System.Drawing.Size(150, 36);
            this.AddBtn.TabIndex = 7;
            this.AddBtn.Text = "Add Income";
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // closeapp
            // 
            this.closeapp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeapp.BackColor = System.Drawing.Color.Transparent;
            this.closeapp.FillColor = System.Drawing.Color.Transparent;
            this.closeapp.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.closeapp.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.closeapp.HoverState.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.closeapp.HoverState.Parent = this.closeapp;
            this.closeapp.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.closeapp.Location = new System.Drawing.Point(850, 3);
            this.closeapp.Name = "closeapp";
            this.closeapp.PressedColor = System.Drawing.Color.Transparent;
            this.closeapp.ShadowDecoration.Parent = this.closeapp;
            this.closeapp.Size = new System.Drawing.Size(45, 29);
            this.closeapp.TabIndex = 18;
            // 
            // cmbCat
            // 
            this.cmbCat.Animated = true;
            this.cmbCat.BackColor = System.Drawing.Color.Transparent;
            this.cmbCat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.cmbCat.BorderRadius = 6;
            this.cmbCat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.cmbCat.FocusedColor = System.Drawing.Color.Empty;
            this.cmbCat.FocusedState.Parent = this.cmbCat;
            this.cmbCat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.cmbCat.FormattingEnabled = true;
            this.cmbCat.HoverState.Parent = this.cmbCat;
            this.cmbCat.ItemHeight = 30;
            this.cmbCat.ItemsAppearance.Parent = this.cmbCat;
            this.cmbCat.Location = new System.Drawing.Point(258, 70);
            this.cmbCat.Name = "cmbCat";
            this.cmbCat.ShadowDecoration.Parent = this.cmbCat;
            this.cmbCat.Size = new System.Drawing.Size(215, 36);
            this.cmbCat.TabIndex = 19;
            this.cmbCat.SelectedIndexChanged += new System.EventHandler(this.cmbCat_SelectedIndexChanged);
            // 
            // cmbMonths
            // 
            this.cmbMonths.Animated = true;
            this.cmbMonths.BackColor = System.Drawing.Color.Transparent;
            this.cmbMonths.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.cmbMonths.BorderRadius = 6;
            this.cmbMonths.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMonths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonths.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.cmbMonths.FocusedColor = System.Drawing.Color.Empty;
            this.cmbMonths.FocusedState.Parent = this.cmbMonths;
            this.cmbMonths.Font = new System.Drawing.Font("Inter Medium", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMonths.ForeColor = System.Drawing.Color.White;
            this.cmbMonths.FormattingEnabled = true;
            this.cmbMonths.HoverState.Parent = this.cmbMonths;
            this.cmbMonths.ItemHeight = 30;
            this.cmbMonths.ItemsAppearance.Font = new System.Drawing.Font("Inter Medium", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMonths.ItemsAppearance.ForeColor = System.Drawing.Color.White;
            this.cmbMonths.ItemsAppearance.Parent = this.cmbMonths;
            this.cmbMonths.Location = new System.Drawing.Point(25, 70);
            this.cmbMonths.MaxDropDownItems = 5;
            this.cmbMonths.Name = "cmbMonths";
            this.cmbMonths.ShadowDecoration.Parent = this.cmbMonths;
            this.cmbMonths.Size = new System.Drawing.Size(215, 36);
            this.cmbMonths.TabIndex = 20;
            // 
            // Incomes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.cmbMonths);
            this.Controls.Add(this.cmbCat);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.closeapp);
            this.Controls.Add(this.label2);
            this.Name = "Incomes";
            this.Text = "Incomes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2Button AddBtn;
        private Guna.UI2.WinForms.Guna2ControlBox closeapp;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCat;
        private Guna.UI2.WinForms.Guna2ComboBox cmbMonths;
    }
}