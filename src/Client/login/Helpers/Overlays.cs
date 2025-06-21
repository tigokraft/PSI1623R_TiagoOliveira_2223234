using Guna.UI2.WinForms;
using login.Helpers;
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class Overlays
{


    public static async Task IncomeOverlay(Form parentForm, HttpClient _http)
    {
        var overlay = new Guna2Panel
        {
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.Transparent,
            Size = new Size(350, 450),
            Location = new Point((parentForm.ClientSize.Width - 500) / 2, 50),
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
            BorderRadius = 10,
            TabIndex = 1,
        };

        var recurring = new Guna2CheckBox
        {
            Text = "Recurring",
            Size = new Size(300, 30),
            Location = new Point(25, 190),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            CheckedState = {
                FillColor = Color.FromArgb(67, 79, 82),
                BorderColor = Color.FromArgb(67, 79, 82),
            },
            UncheckedState = {
                FillColor = Color.FromArgb(125, 137, 149),
                BorderColor = Color.FromArgb(67, 79, 82),
            },
            Font = new Font("Segoe UI", 9),
        };

        var recurrence = new Guna2ComboBox
        {
            Size = new Size(300, 40),
            Location = new Point(25, 230),
            BorderColor = Color.FromArgb(67, 79, 82),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(18, 20, 20),
            FillColor = Color.FromArgb(18, 20, 20),
            BorderRadius = 10,
            TabIndex = 2,
            Items = { "Weekly", "Monthly", "Yearly" },
        };

        var endDate = new Guna2DateTimePicker
        {
            Size = new Size(300, 40),
            Location = new Point(25, 280),
            BorderColor = Color.FromArgb(67, 79, 82),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(18, 20, 20),
            FillColor = Color.FromArgb(18, 20, 20),
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
            HoverState = {
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
            Image = login.Properties.Resources.close, // properties.resources was not available for some reason lol
            Size = new Size(30, 30),
            Location = new Point(overlay.Width - 40, 10),
            ForeColor = Color.Transparent,
        };

        recurrence.Visible = false;
        endDate.Visible = false;

        bool isRecurring = false;
        recurring.CheckedChanged += (s, ev) =>
        {
            isRecurring = recurring.Checked;
            recurrence.Visible = isRecurring;
            endDate.Visible = isRecurring;

            if (!isRecurring)
            {
                recurrence.SelectedIndex = -1;
                endDate.Value = DateTime.Now;
            }
        };

        closeBtn.Click += (s, ev) => { parentForm.Controls.Remove(overlay); };

        CreateBtn.Click += async (s, ev) =>
        {
            if (isRecurring)
            {
                if (recurrence.SelectedIndex == -1)
                {
                    Cards.Show("Error", "Select recurrence", "OK");
                    return;
                }

                if (endDate.Value < DateTime.Now)
                {
                    MessageBox.Show("End date cannot be in the past.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!decimal.TryParse(amount.Text, out var parsedAmount))
            {
                MessageBox.Show("Enter a valid amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await Tasks.PostIncome(parsedAmount, descr.Text, isRecurring, recurrence.SelectedItem?.ToString() ?? "", endDate.Value.ToString("yyyy-MM-dd"), _http);
            parentForm.Controls.Remove(overlay);
        };

        int selectedCategoryId = -1;

        // fetch from API
        var cats = await CategoriesList.GetCategoriesAsync(_http);

        var categoryPanel = new FlowLayoutPanel
        {
            Size = new Size(300, 40),
            Location = new Point(25, 330),
            BackColor = Color.FromArgb(18, 20, 20),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        foreach (var cat in cats)
        {
            var btn = new Guna2Button
            {
                Text = cat.CategoryName,
                AutoRoundedCorners = true,
                BorderRadius = 15,
                Size = new Size(80, 30),

                // 1) Default (unchecked) look:
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.LightGray,

                // 2) Turn it into a radio-mode button:
                ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton,
                Tag = cat.CategoryId
            };

            // 3) Style for when it IS checked:
            btn.CheckedState.FillColor = Color.FromArgb(60, 60, 60);
            btn.CheckedState.ForeColor = Color.White;

            // 4) Track selection if you need the ID later:
            btn.CheckedChanged += (s, e) =>
            {
                if (btn.Checked)
                    selectedCategoryId = (int)btn.Tag;
            };

            categoryPanel.Controls.Add(btn);
        }

        // “+” button to add a new category (logic later)
        var addCategoryBtn = new Guna2Button
        {
            Text = "+",
            AutoRoundedCorners = true,
            BorderRadius = 15,
            Size = new Size(30, 30),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.LightGray,
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };
        addCategoryBtn.Click += (s, e) =>
        {
            // TODO: open add-category dialog
        };
        categoryPanel.Controls.Add(addCategoryBtn);

        // finally, add into overlay
        overlay.Controls.Add(categoryPanel);

        overlay.Controls.Add(label);
        overlay.Controls.Add(closeBtn);
        overlay.Controls.Add(descr);
        overlay.Controls.Add(amount);
        overlay.Controls.Add(recurring);
        overlay.Controls.Add(recurrence);
        overlay.Controls.Add(endDate);
        overlay.Controls.Add(CreateBtn);

        parentForm.Controls.Add(overlay);
        overlay.BringToFront();
    }
}
