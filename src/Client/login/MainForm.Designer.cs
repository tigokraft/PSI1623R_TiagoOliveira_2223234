namespace login
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.budgetBtn = new Guna.UI2.WinForms.Guna2Button();
            this.ExpensesBtn = new Guna.UI2.WinForms.Guna2Button();
            this.GoalsBtn = new Guna.UI2.WinForms.Guna2Button();
            this.OvBtn = new Guna.UI2.WinForms.Guna2Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.former = new Guna.UI2.WinForms.Guna2Panel();
            this.closeapp = new Guna.UI2.WinForms.Guna2ControlBox();
            this.panel1.SuspendLayout();
            this.former.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.TargetControl = this.label1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Inter Medium", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "FinSync";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.panel1.Controls.Add(this.guna2Button1);
            this.panel1.Controls.Add(this.budgetBtn);
            this.panel1.Controls.Add(this.ExpensesBtn);
            this.panel1.Controls.Add(this.GoalsBtn);
            this.panel1.Controls.Add(this.OvBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-1, -1);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(201, 601);
            this.panel1.TabIndex = 0;
            // 
            // guna2Button1
            // 
            this.guna2Button1.Animated = true;
            this.guna2Button1.BorderRadius = 8;
            this.guna2Button1.CheckedState.Parent = this.guna2Button1;
            this.guna2Button1.CustomImages.Parent = this.guna2Button1;
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.guna2Button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.3F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.HoverState.Parent = this.guna2Button1;
            this.guna2Button1.Image = global::login.Properties.Resources.settings;
            this.guna2Button1.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button1.Location = new System.Drawing.Point(12, 550);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.guna2Button1.ShadowDecoration.Parent = this.guna2Button1;
            this.guna2Button1.Size = new System.Drawing.Size(175, 40);
            this.guna2Button1.TabIndex = 9;
            this.guna2Button1.Text = "Settings";
            this.guna2Button1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // budgetBtn
            // 
            this.budgetBtn.Animated = true;
            this.budgetBtn.BorderRadius = 8;
            this.budgetBtn.CheckedState.Parent = this.budgetBtn;
            this.budgetBtn.CustomImages.Parent = this.budgetBtn;
            this.budgetBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.budgetBtn.Font = new System.Drawing.Font("Inter", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.budgetBtn.ForeColor = System.Drawing.Color.White;
            this.budgetBtn.HoverState.Parent = this.budgetBtn;
            this.budgetBtn.Image = global::login.Properties.Resources.budget;
            this.budgetBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.budgetBtn.Location = new System.Drawing.Point(12, 190);
            this.budgetBtn.Name = "budgetBtn";
            this.budgetBtn.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.budgetBtn.ShadowDecoration.Parent = this.budgetBtn;
            this.budgetBtn.Size = new System.Drawing.Size(175, 40);
            this.budgetBtn.TabIndex = 8;
            this.budgetBtn.Text = "Budgets";
            this.budgetBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.budgetBtn.Click += new System.EventHandler(this.budgetBtn_Click);
            // 
            // ExpensesBtn
            // 
            this.ExpensesBtn.Animated = true;
            this.ExpensesBtn.BorderRadius = 8;
            this.ExpensesBtn.CheckedState.Parent = this.ExpensesBtn;
            this.ExpensesBtn.CustomImages.Parent = this.ExpensesBtn;
            this.ExpensesBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ExpensesBtn.Font = new System.Drawing.Font("Inter", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpensesBtn.ForeColor = System.Drawing.Color.White;
            this.ExpensesBtn.HoverState.Parent = this.ExpensesBtn;
            this.ExpensesBtn.Image = global::login.Properties.Resources.expense;
            this.ExpensesBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ExpensesBtn.Location = new System.Drawing.Point(12, 135);
            this.ExpensesBtn.Name = "ExpensesBtn";
            this.ExpensesBtn.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.ExpensesBtn.ShadowDecoration.Parent = this.ExpensesBtn;
            this.ExpensesBtn.Size = new System.Drawing.Size(175, 40);
            this.ExpensesBtn.TabIndex = 7;
            this.ExpensesBtn.Text = "Expenses";
            this.ExpensesBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ExpensesBtn.Click += new System.EventHandler(this.guna2Button2_Click);
            // 
            // GoalsBtn
            // 
            this.GoalsBtn.Animated = true;
            this.GoalsBtn.BorderRadius = 8;
            this.GoalsBtn.CheckedState.Parent = this.GoalsBtn;
            this.GoalsBtn.CustomImages.Parent = this.GoalsBtn;
            this.GoalsBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.GoalsBtn.Font = new System.Drawing.Font("Inter", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoalsBtn.ForeColor = System.Drawing.Color.White;
            this.GoalsBtn.HoverState.Parent = this.GoalsBtn;
            this.GoalsBtn.Image = global::login.Properties.Resources.goal;
            this.GoalsBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.GoalsBtn.Location = new System.Drawing.Point(12, 245);
            this.GoalsBtn.Name = "GoalsBtn";
            this.GoalsBtn.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.GoalsBtn.ShadowDecoration.Parent = this.GoalsBtn;
            this.GoalsBtn.Size = new System.Drawing.Size(175, 40);
            this.GoalsBtn.TabIndex = 6;
            this.GoalsBtn.Text = "Goals";
            this.GoalsBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.GoalsBtn.Click += new System.EventHandler(this.GoalsBtn_Click);
            // 
            // OvBtn
            // 
            this.OvBtn.Animated = true;
            this.OvBtn.BorderRadius = 8;
            this.OvBtn.CheckedState.Parent = this.OvBtn;
            this.OvBtn.CustomImages.Parent = this.OvBtn;
            this.OvBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.OvBtn.Font = new System.Drawing.Font("Inter", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OvBtn.ForeColor = System.Drawing.Color.White;
            this.OvBtn.HoverState.Parent = this.OvBtn;
            this.OvBtn.Image = global::login.Properties.Resources.over;
            this.OvBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.OvBtn.Location = new System.Drawing.Point(12, 80);
            this.OvBtn.Name = "OvBtn";
            this.OvBtn.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(43)))));
            this.OvBtn.ShadowDecoration.Parent = this.OvBtn;
            this.OvBtn.Size = new System.Drawing.Size(175, 40);
            this.OvBtn.TabIndex = 5;
            this.OvBtn.Text = "Overview";
            this.OvBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.OvBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.OvBtn.Click += new System.EventHandler(this.OvBtn_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.ForeColor = System.Drawing.Color.Thistle;
            this.panel2.Location = new System.Drawing.Point(200, -1);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 601);
            this.panel2.TabIndex = 1;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.HoverState.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.guna2ControlBox1.HoverState.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.guna2ControlBox1.Location = new System.Drawing.Point(1059, 3);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.PressedColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.ShadowDecoration.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.Size = new System.Drawing.Size(45, 29);
            this.guna2ControlBox1.TabIndex = 7;
            // 
            // former
            // 
            this.former.Controls.Add(this.closeapp);
            this.former.Location = new System.Drawing.Point(200, 0);
            this.former.Name = "former";
            this.former.ShadowDecoration.Parent = this.former;
            this.former.Size = new System.Drawing.Size(900, 600);
            this.former.TabIndex = 8;
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
            this.closeapp.Location = new System.Drawing.Point(855, 0);
            this.closeapp.Name = "closeapp";
            this.closeapp.PressedColor = System.Drawing.Color.Transparent;
            this.closeapp.ShadowDecoration.Parent = this.closeapp;
            this.closeapp.Size = new System.Drawing.Size(45, 29);
            this.closeapp.TabIndex = 16;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1100, 600);
            this.Controls.Add(this.former);
            this.Controls.Add(this.guna2ControlBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "Overview";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.former.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm1;
        private Guna.UI2.WinForms.Guna2Button OvBtn;
        private Guna.UI2.WinForms.Guna2Button GoalsBtn;
        private Guna.UI2.WinForms.Guna2Button ExpensesBtn;
        private Guna.UI2.WinForms.Guna2Button budgetBtn;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2Panel former;
        private Guna.UI2.WinForms.Guna2ControlBox closeapp;
    }
}