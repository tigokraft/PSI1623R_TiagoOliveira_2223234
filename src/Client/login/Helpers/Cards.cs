using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace login.Helpers
{
    public partial class Cards : Form
    {
        public Cards()
        {
            // Initialize form
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize Elipse for rounded corners
            guna2Elipse1.TargetControl = this;
            guna2Elipse1.BorderRadius = 20;
        }

        public void Mbox(string title, string body, string btnText)
        {
            this.Size = new Size(400, 200);
            this.Text = title;
            this.BackColor = Color.FromArgb(18, 20, 20);

            var label = new Label
            {
                Text = body,
                AutoSize = false,
                Size = new Size(360, 100),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            var btn1 = new Guna2Button
            {
                Text = btnText,
                Size = new Size(100, 35),
                Location = new Point(this.Width / 2 - 50, 130),
                Animated = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(40, 44, 48),
                ForeColor = Color.White,
                DialogResult = DialogResult.OK // Important for returning result
            };

            btn1.Click += (sender, e) => this.Close(); // Close the form on button click

            this.Controls.Add(label);
            this.Controls.Add(btn1);

            this.AcceptButton = btn1; // Enter key triggers the button
        }

        public static DialogResult Show(string title, string body, string btnText = "OK")
        {
            using (var form = new Cards())
            {
                form.Mbox(title, body, btnText);
                return form.ShowDialog();
            }
        }
    }
}
