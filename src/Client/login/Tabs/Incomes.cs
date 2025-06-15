using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using login.Helpers;

namespace login.Tabs
{
    public partial class Incomes : Form
    {
        private readonly HttpClient _http;

        public Incomes(HttpClient http)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            _http = http;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var overlay = new Guna2Panel
            {
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.Transparent,
                Size = new Size(350, 450),
                Location = new Point((this.ClientSize.Width - 300) / 2, 50),
                Anchor = AnchorStyles.Top,
                BackColor = Color.Transparent,
                Name = "OverlayCard",
            };

            var descr = new Guna2TextBox
            {
                PlaceholderText = "Description",
                Size = new Size(300, 40),
                Location = new Point(25, 130),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                FocusedState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 0,
            };

            var amount = new Guna2TextBox
            {
                PlaceholderText = "Amount",
                Size = new Size(300, 40),
                Location = new Point(25, 70),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                FocusedState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 1,
            };

            var recurring = new Guna2CheckBox
            {
                Text = "Recurring",
                Size = new Size(300, 20),
                Location = new Point(25, 170),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                CheckedState =
                {
                    FillColor = Color.FromArgb(67, 79, 82),
                    BorderColor = Color.FromArgb(67, 79, 82),
                },
                UncheckedState =
                {
                    FillColor = Color.FromArgb(18, 20, 20),
                    BorderColor = Color.FromArgb(67, 79, 82),
                },
            };

            var recurrence = new Guna2ComboBox
            {
                Size = new Size(300, 40),
                Location = new Point(25, 230),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 2,

                Items =
                {
                    "Weekly",
                    "Monthly",
                    "Yearly"
                },
            };

            var endDate = new Guna2DateTimePicker
            {
                Size = new Size(300, 40),
                Location = new Point(25, 250),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 3,
            };



            var CreateBtn = new Guna2Button
            {
                FillColor = Color.FromArgb(20, 24, 26),
                BorderColor = Color.FromArgb(39, 42, 44),
                BackColor = Color.FromArgb(18, 20, 20),
                BorderRadius = 10,
                BorderThickness = 1,
                Text = "Add Income",
                Size = new Size(300, 50),
                Location = new Point(25, 380),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                Font = new Font("Segoe UI", 9),
            };


            var label = new Label
            {
                Text = "Add Income",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 20)
            };

            var closeBtn = new Guna2ImageButton
            {
                Image = Properties.Resources.close,
                Size = new Size(30, 30),
                Location = new Point(overlay.Width - 40, 10),
                ForeColor = Color.Transparent,
            };


            bool isRecurring = false;
            recurring.CheckedChanged += (s, ev) =>
            {
                if (recurring.Checked)
                {
                    recurrence.Visible = true;
                    endDate.Visible = true;
                    isRecurring = true;
                    recurrence.SelectedIndex = -1;
                }
                else
                {
                    recurrence.Visible = false;
                    endDate.Visible = false;
                }
            };


            closeBtn.Click += (s, ev) => { this.Controls.Remove(overlay); };
            CreateBtn.Click += async (s, ev) =>
            {
                if (isRecurring)
                {
                    if (recurrence.SelectedIndex == -1)
                    {
                        var result = Cards.Show("Error", "Select recurrence", "OK");
                        return;
                    }
                    if (endDate.Value < DateTime.Now)
                    {
                        MessageBox.Show("End date cannot be in the past.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    recurrence.SelectedIndex = -1;
                    endDate.Value = DateTime.Now;
                    //await Helpers.Tasks.PostIncome(descr.Text, Convert.ToDecimal(amount.Text), , _http);
                    this.Controls.Remove(overlay);
                };
            };
            overlay.Controls.Add(label);
            overlay.Controls.Add(closeBtn);
            overlay.Controls.Add(descr);
            overlay.Controls.Add(amount);
            overlay.Controls.Add(recurring);
            overlay.Controls.Add(recurrence);
            overlay.Controls.Add(endDate);
            overlay.Controls.Add(CreateBtn);

            this.Controls.Add(overlay);
            overlay.BringToFront();
        }
    }
}
