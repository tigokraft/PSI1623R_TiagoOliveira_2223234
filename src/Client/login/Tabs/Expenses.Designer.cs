namespace login.Tabs
{
    partial class Expenses
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
            this.Close = new Guna.UI2.WinForms.Guna2ControlBox();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Close.BackColor = System.Drawing.Color.Transparent;
            this.Close.FillColor = System.Drawing.Color.Transparent;
            this.Close.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.Close.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.Close.HoverState.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Close.HoverState.Parent = this.Close;
            this.Close.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.Close.Location = new System.Drawing.Point(850, 3);
            this.Close.Name = "Close";
            this.Close.PressedColor = System.Drawing.Color.Transparent;
            this.Close.ShadowDecoration.Parent = this.Close;
            this.Close.Size = new System.Drawing.Size(45, 29);
            this.Close.TabIndex = 18;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // Expenses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.Close);
            this.ForeColor = System.Drawing.Color.Coral;
            this.Name = "Expenses";
            this.Text = "Expenses";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2Button AddBtn;
        private Guna.UI2.WinForms.Guna2ControlBox closeapp;
        private Guna.UI2.WinForms.Guna2ControlBox Close;
    }
}