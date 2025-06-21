using System;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace login.Tabs
{
    public partial class Expenses : Form
    {
        public class ExpenseRequest
        {
            public decimal Amount { get; set; }
            public string Tags { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public int CategoryId { get; set; }
        }

        private readonly HttpClient _http;

        public Expenses(HttpClient http)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(22, 25, 32);
            _http = http;

            SetupLayout();
        }

        private void SetupLayout()
        {
            // --- Header Label ---
            var headerLabel = new Label
            {
                Text = "Expenses",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(20, 18),
                AutoSize = true
            };
            this.Controls.Add(headerLabel);

            // --- Add Expense Button (Top Right) ---
            var addBtn = new Guna2Button
            {
                BorderRadius = 10,
                Size = new Size(150, 36),
                Location = new Point(this.ClientSize.Width - 170, 18), // 20px from right
                FillColor = Color.FromArgb(0, 183, 194),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "+ Add Expense",
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            addBtn.Click += AddBtn_Click;
            this.Controls.Add(addBtn);
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            ShowAddExpenseCard();
        }

        private void ShowAddExpenseCard()
        {
            var overlay = new Guna2Panel
            {
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.Transparent,
                Size = new Size(350, 450),
                Location = new Point((this.ClientSize.Width - 350) / 2, (this.ClientSize.Height - 450) / 2),
                Anchor = AnchorStyles.Top,
                BackColor = Color.Transparent,
                Name = "OverlayCard",
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
                BorderRadius = 10,
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
                BorderRadius = 10,
            };

            var Tag = new Guna2TextBox
            {
                PlaceholderText = "Tag",
                Size = new Size(300, 40),
                Location = new Point(25, 190),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                BorderRadius = 10,
            };

            var CreateBtn = new Guna2Button
            {
                FillColor = Color.FromArgb(20, 24, 26),
                BorderColor = Color.FromArgb(39, 42, 44),
                BackColor = Color.FromArgb(18, 20, 20),
                BorderRadius = 10,
                BorderThickness = 1,
                Text = "Add Expense",
                Size = new Size(300, 50),
                Location = new Point(25, 380),
                Font = new Font("Segoe UI", 9)
            };

            var label = new Label
            {
                Text = "Add Expense",
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

            closeBtn.Click += (s, ev) => { this.Controls.Remove(overlay); };
            CreateBtn.Click += async (s, ev) =>
            {
                // Add your logic later
                this.Controls.Remove(overlay);
            };

            overlay.Controls.Add(label);
            overlay.Controls.Add(closeBtn);
            overlay.Controls.Add(descr);
            overlay.Controls.Add(amount);
            overlay.Controls.Add(Tag);
            overlay.Controls.Add(CreateBtn);

            this.Controls.Add(overlay);
            overlay.BringToFront();
        }
    }
}
