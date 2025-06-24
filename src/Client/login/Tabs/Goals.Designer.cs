namespace login.Tabs
{
    partial class Goals
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
            this.AddBtn = new Guna.UI2.WinForms.Guna2Button();
            this.closeapp = new Guna.UI2.WinForms.Guna2ControlBox();
            this.SuspendLayout();
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
            this.AddBtn.Location = new System.Drawing.Point(695, 15);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.ShadowDecoration.Parent = this.AddBtn;
            this.AddBtn.Size = new System.Drawing.Size(150, 36);
            this.AddBtn.TabIndex = 8;
            this.AddBtn.Text = "Add Goal";
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
            this.closeapp.Location = new System.Drawing.Point(845, 0);
            this.closeapp.Name = "closeapp";
            this.closeapp.PressedColor = System.Drawing.Color.Transparent;
            this.closeapp.ShadowDecoration.Parent = this.closeapp;
            this.closeapp.Size = new System.Drawing.Size(45, 29);
            this.closeapp.TabIndex = 19;
            this.closeapp.Click += new System.EventHandler(this.closeapp_Click);
            // 
            // Goals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.closeapp);
            this.Controls.Add(this.AddBtn);
            this.Name = "Goals";
            this.Text = "Goals";
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button AddBtn;
        private Guna.UI2.WinForms.Guna2ControlBox closeapp;
    }
}