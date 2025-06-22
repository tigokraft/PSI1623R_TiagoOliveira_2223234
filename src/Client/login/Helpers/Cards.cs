using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace login.Helpers
{
    public partial class Cards : Form
    {
        private Guna2Panel outlinePanel;
        private Panel contentPanel;
        private Point mouseOffset;
        private bool isMouseDown = false;

        public Cards()
        {
            InitializeComponent();

            // Window styling
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 190);
            this.BackColor = Color.FromArgb(18, 20, 20);

            // Rounded corners
            if (guna2Elipse1 != null)
            {
                guna2Elipse1.TargetControl = this;
                guna2Elipse1.BorderRadius = 18;
            }

            // Outline panel
            outlinePanel = new Guna2Panel
            {
                Size = new Size(this.Width, this.Height),
                Location = new Point(0, 0),
                BorderColor = Color.FromArgb(80, 80, 90),
                BorderThickness = 2,
                FillColor = Color.FromArgb(18, 20, 20),
                ShadowDecoration = { Enabled = true, Shadow = new Padding(4, 4, 4, 4) },
                BorderRadius = 18
            };
            this.Controls.Add(outlinePanel);

            // Content panel (for labels & buttons)
            contentPanel = new Panel
            {
                Size = new Size(this.Width, this.Height),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(18, 20, 20) // Match the parent panel, not Transparent!
            };
            // ***** FIX: Make contentPanel a child of outlinePanel, not the Form *****
            outlinePanel.Controls.Add(contentPanel);

            // Enable dragging by clicking anywhere
            outlinePanel.MouseDown += Card_MouseDown;
            outlinePanel.MouseMove += Card_MouseMove;
            outlinePanel.MouseUp += Card_MouseUp;
            contentPanel.MouseDown += Card_MouseDown;
            contentPanel.MouseMove += Card_MouseMove;
            contentPanel.MouseUp += Card_MouseUp;
        }

        // Make draggable
        private void Card_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }

        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                this.Location = mousePos;
            }
        }

        private void Card_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public void Mbox(string title, string body, string btnText1 = "OK", string btnText2 = null)
        {
            // Remove only previous content
            contentPanel.Controls.Clear();

            // Title label
            var titleLabel = new Label
            {
                Text = title,
                AutoSize = false,
                Size = new Size(this.Width - 40, 28),
                Location = new Point(20, 16),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Semibold", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20) // Not Transparent
            };
            contentPanel.Controls.Add(titleLabel);

            // Message label
            var label = new Label
            {
                Text = body,
                AutoSize = false,
                Size = new Size(this.Width - 60, 54),
                Location = new Point(30, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20) // Not Transparent
            };
            contentPanel.Controls.Add(label);

            // First button
            var btn1 = new Guna2Button
            {
                Text = btnText1,
                Size = new Size(100, 38),
                BorderRadius = 10,
                FillColor = Color.FromArgb(44, 46, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                DialogResult = (btnText1.ToLower().Contains("yes")) ? DialogResult.Yes : DialogResult.OK,
                Animated = true,
                Location = btnText2 == null
                    ? new Point(this.Width / 2 - 50, 120)
                    : new Point(this.Width / 2 - 110, 120),
                BorderColor = Color.FromArgb(44, 46, 54),
                BorderThickness = 2
            };
            btn1.HoverState.BorderColor = Color.FromArgb(100, 180, 255);
            btn1.Click += (sender, e) => this.Close();
            btn1.ShadowDecoration.Enabled = true;
            contentPanel.Controls.Add(btn1);
            this.AcceptButton = btn1;

            if (btnText2 != null)
            {
                var btn2 = new Guna2Button
                {
                    Text = btnText2,
                    Size = new Size(100, 38),
                    BorderRadius = 10,
                    FillColor = Color.FromArgb(90, 24, 24),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                    DialogResult = DialogResult.No,
                    Animated = true,
                    Location = new Point(this.Width / 2 + 10, 120),
                    BorderColor = Color.FromArgb(90, 24, 24),
                    BorderThickness = 2
                };
                btn2.HoverState.BorderColor = Color.FromArgb(255, 90, 90);
                btn2.Click += (sender, e) => this.Close();
                btn2.ShadowDecoration.Enabled = true;
                contentPanel.Controls.Add(btn2);
                this.CancelButton = btn2;
            }
        }

        public static DialogResult Show(string title, string body, string btnText1 = "OK", string btnText2 = null)
        {
            using (var form = new Cards())
            {
                form.Mbox(title, body, btnText1, btnText2);
                return form.ShowDialog();
            }
        }
    }
}
