using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;
using login.Helpers;
using login.Tabs;
using static login.Helpers.Tasks;
using static System.Net.WebRequestMethods;

public static class Overlays
{
    /// <summary>
    /// Shows the "Add/Edit Income" overlay with a horizontal scrollable category selector.
    /// </summary>
    public static async Task IncomeOverlay(Form parentForm, HttpClient _http, Incomes.Income incomeToEdit = null)
    {
        await Task.Yield();

        // OVERLAY PANEL
        var overlay = new Guna2Panel
        {
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            Size = new Size(350, 500),
            Location = new Point((parentForm.ClientSize.Width - 350) / 2, 50),
            Anchor = AnchorStyles.Top,
            Name = "IncomeOverlay"
        };

        // TITLE
        var titleLabel = new Label
        {
            Text = incomeToEdit == null ? "Add Income" : "Edit Income",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // DESCRIPTION
        var descrBox = new Guna2TextBox
        {
            PlaceholderText = "Description",
            Size = new Size(300, 40),
            Location = new Point(25, 60),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        // AMOUNT
        var amountBox = new Guna2TextBox
        {
            PlaceholderText = "Amount",
            Size = new Size(300, 40),
            Location = new Point(25, 110),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        // RECURRING
        var recurringChk = new Guna2CheckBox
        {
            Text = "Recurring",
            Size = new Size(300, 30),
            Location = new Point(25, 160),
            ForeColor = Color.White,
            CheckedState =
            {
                FillColor = Color.FromArgb(67, 79, 82),
                BorderColor = Color.FromArgb(67, 79, 82)
            },
            UncheckedState =
            {
                FillColor = Color.FromArgb(125, 137, 149),
                BorderColor = Color.FromArgb(67, 79, 82)
            },
            Font = new Font("Segoe UI", 9)
        };
        var recurrenceCombo = new Guna2ComboBox
        {
            Items = { "Weekly", "Monthly", "Yearly" },
            Size = new Size(300, 40),
            Location = new Point(25, 195),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        var endDatePicker = new Guna2DateTimePicker
        {
            Size = new Size(300, 40),
            Location = new Point(25, 245),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        bool isRecurring = false;
        recurrenceCombo.Visible = false;
        endDatePicker.Visible = false;
        recurringChk.CheckedChanged += (s, ev) =>
        {
            isRecurring = recurringChk.Checked;
            recurrenceCombo.Visible = isRecurring;
            endDatePicker.Visible = isRecurring;
            if (!isRecurring)
            {
                recurrenceCombo.SelectedIndex = -1;
                endDatePicker.Value = DateTime.Now;
            }
        };

        // CLOSE BUTTON
        var closeBtn = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(overlay.Width - 40, 10),
            ForeColor = Color.Transparent
        };
        closeBtn.Click += (s, ev) => parentForm.Controls.Remove(overlay);

        // 1) Fetch and track selection
        int selectedCategoryId = -1;
        var categories = await CategoriesList.GetCategoriesAsync(_http);

        // ---------------- CONTEXT MENU STRIP for Categories ------------------
        var catContextMenu = new ContextMenuStrip();
        var editCatItem = new ToolStripMenuItem("Edit Category");
        var deleteCatItem = new ToolStripMenuItem("Delete Category");
        catContextMenu.Items.AddRange(new ToolStripItem[] { editCatItem, deleteCatItem });

        editCatItem.Click += async (s, e) =>
        {
            int categoryId = (int)catContextMenu.Tag;
            var cat = categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (cat == null)
            {
                Cards.Show("Error", "Category not found.", "OK");
                return;
            }
            await EditCategoryOverlay(parentForm, _http, cat.CategoryId, cat.CategoryName, cat.Color);
            parentForm.Controls.Remove(overlay); // Remove old overlay after editing
        };

        deleteCatItem.Click += async (s, e) =>
        {
            int categoryId = (int)catContextMenu.Tag;
            var confirm = Cards.Show("Delete Category", "This will delete the category. Continue?", "OK");
            if (confirm == DialogResult.OK)
            {
                var resp = await _http.DeleteAsync($"api/category/{categoryId}");
                if (resp.IsSuccessStatusCode)
                {
                    Cards.Show("Success", "Category deleted.", "OK");
                    parentForm.Controls.Remove(overlay);
                    await IncomeOverlay(parentForm, _http);
                }
                else
                {
                    Cards.Show("Error", "Failed to delete category.", "OK");
                }
            }
        };
        // ------------------------------------------------------

        // 2) Mask panel (clips its child)
        var maskPanel = new Guna2Panel
        {
            Size = new Size(300, 40),
            Location = new Point(25, 295),
            BackColor = Color.FromArgb(18, 20, 20),
            BorderRadius = 10,
            ShadowDecoration = { Enabled = false }
        };

        // 3) Inner FlowLayoutPanel (auto-sized row)
        var inner = new FlowLayoutPanel
        {
            Location = Point.Empty,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        // 4) Build each pill
        foreach (var cat in categories)
        {
            int dot = 12;
            int textW = TextRenderer.MeasureText(cat.CategoryName, new Font("Segoe UI", 9)).Width;

            var btn = new Guna2Button
            {
                Text = cat.CategoryName,
                AutoRoundedCorners = true,
                BorderRadius = 15,
                Size = new Size(dot + 6 + textW + 20, 30),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.LightGray,
                ButtonMode = ButtonMode.RadioButton,
                Tag = cat.CategoryId,
                Font = new Font("Segoe UI", 9)
            };
            btn.CheckedState.FillColor = Color.FromArgb(60, 60, 60);
            btn.CheckedState.ForeColor = Color.White;

            // draw the color-dot
            var bmp = new Bitmap(dot, dot);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(cat.Color)), 0, 0, dot, dot);
            }
            btn.Image = bmp;
            btn.ImageSize = new Size(dot, dot);
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(dot + 6, 0, 0, 0);

            btn.CheckedChanged += (s, e) =>
            {
                if (btn.Checked) selectedCategoryId = (int)btn.Tag;
            };

            // ----------- Right-click context menu logic -------------
            btn.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    catContextMenu.Tag = btn.Tag;
                    catContextMenu.Show(btn, new Point(e.X, e.Y));
                }
            };
            // --------------------------------------------------------

            inner.Controls.Add(btn);
        }

        // 5) The “+” button must live in that same row
        var plusBtn = new Guna2Button
        {
            Text = "+",
            AutoRoundedCorners = true,
            BorderRadius = 15,
            Size = new Size(30, 30),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.LightGray,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Margin = new Padding(10, 0, 10, 0)
        };
        plusBtn.Click += async (s, e) =>
        {
            parentForm.Controls.Remove(overlay);
            await CategoryOverlay(parentForm, _http);
        };
        inner.Controls.Add(plusBtn);

        // 6) Put inner into mask
        maskPanel.Controls.Add(inner);

        // 7) Slim Guna scrollbar underneath
        var hScroll = new Guna2HScrollBar
        {
            Location = new Point(maskPanel.Left, maskPanel.Bottom + 4),
            Size = new Size(maskPanel.Width, 6),
            Minimum = 0,
            LargeChange = maskPanel.Width,
            FillColor = Color.FromArgb(40, 40, 40),
            ThumbColor = Color.FromArgb(100, 100, 100),
            BorderRadius = 3
        };
        Action updateMax = () =>
        {
            inner.PerformLayout();
            hScroll.Maximum = Math.Max(0, inner.Width - maskPanel.Width);
        };
        inner.ControlAdded += (s, e) => updateMax();
        inner.ControlRemoved += (s, e) => updateMax();
        hScroll.Scroll += (s, e) => inner.Left = -e.NewValue;

        updateMax(); // Initial max update

        // 8) Add both to your overlay
        overlay.Controls.Add(maskPanel);
        overlay.Controls.Add(hScroll);

        // ─── ADD / EDIT INCOME BUTTON ───────────────────────────────────────
        var createBtn = new Guna2Button
        {
            Text = incomeToEdit == null ? "Add Income" : "Save Changes",
            Size = new Size(300, 50),
            Location = new Point(25, overlay.Bottom - 140),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            BorderThickness = 1,
            Font = new Font("Segoe UI", 9)
        };

        // ── PREFILL FIELDS IF EDITING ─────────────────────
        if (incomeToEdit != null)
        {
            descrBox.Text = incomeToEdit.Descr;
            amountBox.Text = incomeToEdit.Amount.ToString("0.##");
            // Find and select the right category
            foreach (Guna2Button catBtn in inner.Controls.OfType<Guna2Button>())
            {
                if (catBtn.Tag is int catId && catId == incomeToEdit.CategoryId)
                {
                    catBtn.Checked = true;
                    selectedCategoryId = catId;
                    break;
                }
            }
        }

        // ── BUTTON CLICK HANDLER ───────────────────────────
        createBtn.Click += async (s, ev) =>
        {
            var description = descrBox.Text.Trim();
            var amountText = amountBox.Text.Trim();

            // Hard validation, all required!
            if (string.IsNullOrEmpty(description))
            {
                Cards.Show("Validation Error", "Description is required.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(amountText))
            {
                Cards.Show("Validation Error", "Amount is required.", "OK");
                return;
            }
            if (!decimal.TryParse(amountText, out var amount) || amount <= 0)
            {
                Cards.Show("Validation Error", "Amount must be a positive number.", "OK");
                return;
            }
            if (selectedCategoryId == -1)
            {
                Cards.Show("Validation Error", "Select a category.", "OK");
                return;
            }

            bool isRec = recurringChk.Checked;
            string recurrence = recurrenceCombo.SelectedItem?.ToString();
            if (isRec && string.IsNullOrEmpty(recurrence))
            {
                Cards.Show("Validation Error", "Choose a recurrence type.", "OK");
                return;
            }
            string endDate = isRec ? endDatePicker.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

            bool success = false;
            if (incomeToEdit == null)
            {
                // ADD NEW
                success = await Tasks.PostIncome(
                    amount,
                    description,
                    isRec,
                    isRec ? recurrence : "",
                    endDate,
                    _http,
                    selectedCategoryId
                );
            }
            else
            {
                // UPDATE
                var payload = new
                {
                    incomeToEdit.IncomeId,
                    Amount = amount,
                    Descr = description,
                    Date = incomeToEdit.Date, // Or allow edit if you want
                    CategoryId = selectedCategoryId
                };
                var json = JsonSerializer.Serialize(payload);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var resp = await _http.PutAsync($"api/income/{incomeToEdit.IncomeId}", content);
                    success = resp.IsSuccessStatusCode;
                    if (!success)
                        Cards.Show("Error", $"Error updating income: {resp.StatusCode}", "OK");
                }
            }

            if (success)
            {
                parentForm.Controls.Remove(overlay);
                if (parentForm is Incomes incomesForm)
                {
                    await incomesForm.InvokeAsync(() => incomesForm.ListLoader());
                }
            }
        };

        // ASSEMBLE CONTROLS
        overlay.Controls.AddRange(new Control[]
        {
            titleLabel, descrBox, amountBox, recurringChk,
            recurrenceCombo, endDatePicker, closeBtn
        });
        overlay.Controls.Add(createBtn);
        parentForm.Controls.Add(overlay);
        overlay.BringToFront();
    }

    /// <summary>
    /// Shows the "Add Category" overlay.
    /// </summary>
    public static async Task CategoryOverlay(Form parentForm, HttpClient _http)
    {
        // Container panel
        var panel = new Guna2Panel
        {
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            Size = new Size(350, 250),
            Location = new Point((parentForm.ClientSize.Width - 350) / 2, 100),
            Anchor = AnchorStyles.Top
        };

        // Title
        var title = new Label
        {
            Text = "Add Category",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // Name input
        var nameBox = new Guna2TextBox
        {
            PlaceholderText = "Category Name",
            Size = new Size(300, 40),
            Location = new Point(25, 60),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderColor = Color.FromArgb(67, 79, 82),
            BorderRadius = 10
        };

        // Color input
        var colorBox = new Guna2TextBox
        {
            PlaceholderText = "#RRGGBB",
            Size = new Size(300, 40),
            Location = new Point(25, 110),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderColor = Color.FromArgb(67, 79, 82),
            BorderRadius = 10
        };

        // Save button
        var saveBtn = new Guna2Button
        {
            Text = "Save",
            Size = new Size(140, 40),
            Location = new Point(25, 170),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            Font = new Font("Segoe UI", 9)
        };

        // Cancel button
        var cancelBtn = new Guna2Button
        {
            Text = "Cancel",
            Size = new Size(140, 40),
            Location = new Point(185, 170),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            Font = new Font("Segoe UI", 9)
        };

        // Cancel logic: close this overlay and reopen IncomeOverlay
        cancelBtn.Click += (s, e) =>
        {
            parentForm.Controls.Remove(panel);
            _ = IncomeOverlay(parentForm, _http);
        };

        // Save logic: validate inputs, POST, then close + refresh
        saveBtn.Click += async (s, e) =>
        {
            var name = nameBox.Text.Trim();
            var color = colorBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Cards.Show("Validation Error", "Please enter a category name.", "OK");
                return;
            }
            if (!Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$"))
            {
                Cards.Show("Validation Error", "Please enter a valid hex color (e.g. #FFA500).", "OK");
                return;
            }

            var payload = new { CategoryName = name, Color = color };
            var json = JsonSerializer.Serialize(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var resp = await _http.PostAsync("api/category", content);
                if (!resp.IsSuccessStatusCode)
                {
                    Cards.Show("Error", $"Error saving category: {resp.StatusCode}", "OK");
                    return;
                }
            }

            // Tear down and reopen income overlay (so new category appears)
            parentForm.Controls.Remove(panel);
            await IncomeOverlay(parentForm, _http);
        };

        // Add controls and show
        panel.Controls.AddRange(new Control[] { title, nameBox, colorBox, saveBtn, cancelBtn });
        parentForm.Controls.Add(panel);
        panel.BringToFront();
    }

    public static async Task EditCategoryOverlay(Form parentForm, HttpClient _http, int categoryId, string currentName, string currentColor)
    {
        var panel = new Guna2Panel
        {
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            Size = new Size(350, 250),
            Location = new Point((parentForm.ClientSize.Width - 350) / 2, 100),
            Anchor = AnchorStyles.Top
        };

        var title = new Label
        {
            Text = "Edit Category",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        var nameBox = new Guna2TextBox
        {
            PlaceholderText = "Category Name",
            Text = currentName,
            Size = new Size(300, 40),
            Location = new Point(25, 60),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderColor = Color.FromArgb(67, 79, 82),
            BorderRadius = 10
        };

        var colorBox = new Guna2TextBox
        {
            PlaceholderText = "#RRGGBB",
            Text = currentColor,
            Size = new Size(300, 40),
            Location = new Point(25, 110),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderColor = Color.FromArgb(67, 79, 82),
            BorderRadius = 10
        };

        var saveBtn = new Guna2Button
        {
            Text = "Save",
            Size = new Size(140, 40),
            Location = new Point(25, 170),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            Font = new Font("Segoe UI", 9)
        };

        var cancelBtn = new Guna2Button
        {
            Text = "Cancel",
            Size = new Size(140, 40),
            Location = new Point(185, 170),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            Font = new Font("Segoe UI", 9)
        };

        cancelBtn.Click += (s, e) =>
        {
            parentForm.Controls.Remove(panel);
        };

        saveBtn.Click += async (s, e) =>
        {
            var name = nameBox.Text.Trim();
            var color = colorBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Cards.Show("Validation Error", "Please enter a category name.", "OK");
                return;
            }
            if (!Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$"))
            {
                Cards.Show("Validation Error", "Please enter a valid hex color (e.g. #FFA500).", "OK");
                return;
            }

            var payload = new { CategoryId = categoryId, CategoryName = name, Color = color };
            var json = JsonSerializer.Serialize(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var resp = await _http.PutAsync($"api/category/{categoryId}", content);
                if (!resp.IsSuccessStatusCode)
                {
                    Cards.Show("Error", $"Error updating category: {resp.StatusCode}", "OK");
                    return;
                }
            }

            parentForm.Controls.Remove(panel);
            // After editing, close overlays and re-open income overlay so it's always fresh:
            foreach (Control c in parentForm.Controls.OfType<Guna2Panel>().ToList())
            {
                if (c.Name == "IncomeOverlay") parentForm.Controls.Remove(c);
            }
            await IncomeOverlay(parentForm, _http);
        };

        panel.Controls.AddRange(new Control[] { title, nameBox, colorBox, saveBtn, cancelBtn });
        parentForm.Controls.Add(panel);
        panel.BringToFront();
    }

    public static async Task PostExpense(string desc, decimal amt, int categoryId, HttpClient _http)
    {
        var payload = new 
        {
            Amount = amt,
            Description = desc,
            CategoryId = categoryId,
            Date = DateTime.Now
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("api/expense/", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Failed: {response.StatusCode} - {error}");
        }
    }


    public static async Task ExpenseOverlay(Form parentForm, HttpClient _http)
    {
        await Task.Yield();

        const int overlayWidth = 350, overlayHeight = 500, marginX = 25, controlWidth = 300, gap = 15, labelGap = 4;
        var overlay = new Guna2Panel
        {
            Name = "ExpenseOverlay",
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            Size = new Size(overlayWidth, overlayHeight),
            Location = new Point((parentForm.ClientSize.Width - overlayWidth) / 2, 50),
            Anchor = AnchorStyles.Top
        };
        overlay.SuspendLayout();

        int currY = 20;
        // ── Title ──────────────────────────────────────────────────────
        var title = new Label
        {
            Text = "Add Expense",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(marginX, currY),
            AutoSize = true
        };
        overlay.Controls.Add(title);
        currY += title.Height + gap;

        // ── Description ───────────────────────────────────────────────
        var lblDesc = new Label { Text = "Description", ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(marginX, currY), AutoSize = true };
        overlay.Controls.Add(lblDesc);
        currY += lblDesc.Height + labelGap;

        var txtDesc = new Guna2TextBox
        {
            PlaceholderText = "e.g. Coffee, Uber, etc.",
            Size = new Size(controlWidth, 40),
            Location = new Point(marginX, currY),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(txtDesc);
        currY += txtDesc.Height + gap;

        // ── Amount ───────────────────────────────────────────────────
        var lblAmt = new Label { Text = "Amount", ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(marginX, currY), AutoSize = true };
        overlay.Controls.Add(lblAmt);
        currY += lblAmt.Height + labelGap;

        var txtAmt = new Guna2TextBox
        {
            PlaceholderText = "0.00",
            Size = new Size(controlWidth, 40),
            Location = new Point(marginX, currY),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(txtAmt);
        currY += txtAmt.Height + gap;

        // ── Date ─────────────────────────────────────────────────────
        var lblDate = new Label { Text = "Date", ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(marginX, currY), AutoSize = true };
        overlay.Controls.Add(lblDate);
        currY += lblDate.Height + labelGap;

        var dtPicker = new Guna2DateTimePicker
        {
            Size = new Size(controlWidth, 40),
            Location = new Point(marginX, currY),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(dtPicker);
        currY += dtPicker.Height + gap;

        // ── Category ─────────────────────────────────────────────────
        var lblCat = new Label { Text = "Category", ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(marginX, currY), AutoSize = true };
        overlay.Controls.Add(lblCat);
        currY += lblCat.Height + labelGap;

        var maskPanel = new Guna2Panel
        {
            Size = new Size(controlWidth, 40),
            Location = new Point(marginX, currY),
            FillColor = Color.FromArgb(18, 20, 20),
            BorderRadius = 10,
            ShadowDecoration = { Enabled = false }
        };
        var inner = new FlowLayoutPanel { Location = Point.Empty, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        maskPanel.Controls.Add(inner);
        overlay.Controls.Add(maskPanel);

        int selectedCategoryId = -1;
        var categories = await CategoriesList.GetCategoriesAsync(_http);
        foreach (var cat in categories)
        {
            int dot = 12;
            int textW = TextRenderer.MeasureText(cat.CategoryName, new Font("Segoe UI", 9)).Width;
            var btn = new Guna2Button
            {
                Text = cat.CategoryName,
                AutoRoundedCorners = true,
                BorderRadius = 15,
                Size = new Size(dot + 6 + textW + 20, 30),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.LightGray,
                ButtonMode = ButtonMode.RadioButton,
                Tag = cat.CategoryId,
                Font = new Font("Segoe UI", 9)
            };
            btn.CheckedState.FillColor = Color.FromArgb(60, 60, 60);
            btn.CheckedState.ForeColor = Color.White;

            var bmp = new Bitmap(dot, dot);
            using (var g = Graphics.FromImage(bmp))
                g.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(cat.Color)), 0, 0, dot, dot);

            btn.Image = bmp;
            btn.ImageSize = new Size(dot, dot);
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(dot + 6, 0, 0, 0);
            btn.CheckedChanged += (s, e) => { if (btn.Checked) selectedCategoryId = (int)btn.Tag; };

            inner.Controls.Add(btn);
        }
        currY += maskPanel.Height + labelGap;

        // ── Scrollbar ────────────────────────────────────────────────
        var hScroll = new Guna2HScrollBar
        {
            Location = new Point(marginX, currY),
            Size = new Size(controlWidth, 6),
            Minimum = 0,
            LargeChange = controlWidth,
            FillColor = Color.FromArgb(40, 40, 40),
            ThumbColor = Color.FromArgb(100, 100, 100),
            BorderRadius = 3
        };
        Action updateMax = () => { inner.PerformLayout(); hScroll.Maximum = Math.Max(0, inner.Width - maskPanel.Width); };
        inner.ControlAdded += (s, e) => updateMax();
        inner.ControlRemoved += (s, e) => updateMax();
        hScroll.Scroll += (s, e) => inner.Left = -e.NewValue;
        updateMax();
        overlay.Controls.Add(hScroll);
        currY += hScroll.Height + gap;

        // ── Add Expense Button ────────────────────────────────────────
        var btnAdd = new Guna2Button
        {
            Text = "Add Expense",
            Size = new Size(controlWidth, 50),
            Location = new Point(marginX, currY),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            BorderThickness = 1,
            Font = new Font("Segoe UI", 9)
        };
        btnAdd.Click += async (s, ev) =>
        {
            var description = txtDesc.Text.Trim();
            var amountText = txtAmt.Text.Trim();
            if (string.IsNullOrEmpty(description))
            {
                Cards.Show("Validation Error", "Description is required.", "OK");
                return;
            }
            if (!decimal.TryParse(amountText, out var amount) || amount <= 0)
            {
                Cards.Show("Validation Error", "Amount must be a positive number.", "OK");
                return;
            }
            if (selectedCategoryId == -1)
            {
                Cards.Show("Validation Error", "Select a category.", "OK");
                return;
            }

            // build payload
            var payload = new
            {
                Description = description,
                Amount = amount,
                CategoryId = selectedCategoryId,
                Date = dtPicker.Value
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("api/expense", content);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                Cards.Show("Error", $"Failed to save expense: {resp.StatusCode} – {err}", "OK");
                return;
            }

            parentForm.Controls.Remove(overlay);
            if (parentForm is Expenses expForm)
                await expForm.InvokeAsync(() => expForm.ListLoader());
        };
        overlay.Controls.Add(btnAdd);

        // ── Close Button ───────────────────────────────────────────────
        var btnClose = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(overlayWidth - 40, 10),
            BackColor = Color.Transparent
        };
        btnClose.Click += (s, ev) => parentForm.Controls.Remove(overlay);
        overlay.Controls.Add(btnClose);

        overlay.ResumeLayout();
        parentForm.Controls.Add(overlay);
        overlay.BringToFront();
    }





    /// <summary>
    /// Edit Expense Overlay: pre-filled, updates on save
    /// </summary>
    public static async Task EditExpenseOverlay(Form parentForm, HttpClient _http, Expenses.Expense expense)
    {
        await Task.Yield();

        var overlay = new Guna2Panel
        {
            Name = "ExpenseEditOverlay",
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(40, 40, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            Size = new Size(350, 500),
            Location = new Point((parentForm.ClientSize.Width - 350) / 2, 50),
            Anchor = AnchorStyles.Top
        };

        // Title
        var titleLabel = new Label
        {
            Text = "Edit Expense",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        overlay.Controls.Add(titleLabel);

        // Description
        var descrBox = new Guna2TextBox
        {
            PlaceholderText = "Description",
            Text = expense.Description,
            Size = new Size(300, 40),
            Location = new Point(25, 60),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(descrBox);

        // Amount
        var amountBox = new Guna2TextBox
        {
            PlaceholderText = "Amount",
            Text = expense.Amount.ToString("0.##"),
            Size = new Size(300, 40),
            Location = new Point(25, 120),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(amountBox);

        // Date
        var datePicker = new Guna2DateTimePicker
        {
            Value = expense.Date,
            Size = new Size(300, 40),
            Location = new Point(25, 180),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(datePicker);

        // Category selector
        int selectedCategoryId = expense.CategoryId;
        var categories = await CategoriesList.GetCategoriesAsync(_http);

        var maskPanel = new Guna2Panel
        {
            Size = new Size(300, 40),
            Location = new Point(25, 240),
            BackColor = Color.FromArgb(18, 20, 20),
            BorderRadius = 10
        };
        var innerFlow = new FlowLayoutPanel
        {
            Location = Point.Empty,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        maskPanel.Controls.Add(innerFlow);
        overlay.Controls.Add(maskPanel);

        foreach (var cat in categories)
        {
            int dot = 12;
            int textW = TextRenderer.MeasureText(cat.CategoryName, new Font("Segoe UI", 9)).Width;

            var btn = new Guna2Button
            {
                Text = cat.CategoryName,
                AutoRoundedCorners = true,
                BorderRadius = 15,
                Size = new Size(dot + 6 + textW + 20, 30),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.LightGray,
                ButtonMode = ButtonMode.RadioButton,
                Tag = cat.CategoryId,
                Font = new Font("Segoe UI", 9),
                Checked = cat.CategoryId == expense.CategoryId
            };
            btn.CheckedState.FillColor = Color.FromArgb(60, 60, 60);
            btn.CheckedState.ForeColor = Color.White;

            var bmp = new Bitmap(dot, dot);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(cat.Color)), 0, 0, dot, dot);
            }
            btn.Image = bmp;
            btn.ImageSize = new Size(dot, dot);
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(dot + 6, 0, 0, 0);

            btn.CheckedChanged += (s, ev) =>
            {
                if (btn.Checked) selectedCategoryId = (int)btn.Tag;
            };

            innerFlow.Controls.Add(btn);
        }

        // Scrollbar under categories
        var hScroll = new Guna2HScrollBar
        {
            Location = new Point(maskPanel.Left, maskPanel.Bottom + 4),
            Size = new Size(maskPanel.Width, 6),
            Minimum = 0,
            LargeChange = maskPanel.Width,
            FillColor = Color.FromArgb(40, 40, 40),
            ThumbColor = Color.FromArgb(100, 100, 100),
            BorderRadius = 3
        };
        Action updateMax = () =>
        {
            innerFlow.PerformLayout();
            hScroll.Maximum = Math.Max(0, innerFlow.Width - maskPanel.Width);
        };
        innerFlow.ControlAdded += (s, ev) => updateMax();
        innerFlow.ControlRemoved += (s, ev) => updateMax();
        hScroll.Scroll += (s, ev) => innerFlow.Left = -ev.NewValue;
        updateMax();
        overlay.Controls.Add(hScroll);

        // Save button
        var saveBtn = new Guna2Button
        {
            Text = "Save",
            Size = new Size(300, 50),
            Location = new Point(25, overlay.Height - 120),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            BorderThickness = 1,
            Font = new Font("Segoe UI", 9)
        };
        saveBtn.Click += async (s, ev) =>
        {
            var desc = descrBox.Text.Trim();
            var amtTxt = amountBox.Text.Trim();
            if (string.IsNullOrEmpty(desc))
            {
                Cards.Show("Validation Error", "Description is required.", "OK");
                return;
            }
            if (!decimal.TryParse(amtTxt, out var amt) || amt <= 0)
            {
                Cards.Show("Validation Error", "Amount must be a positive number.", "OK");
                return;
            }
            if (selectedCategoryId == -1)
            {
                Cards.Show("Validation Error", "Select a category.", "OK");
                return;
            }

            var payload = new
            {
                Description = desc,
                Amount = amt,
                CategoryId = selectedCategoryId,
                Date = datePicker.Value
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync($"api/expense/{expense.ExpenseId}", content);

            if (resp.IsSuccessStatusCode)
            {
                parentForm.Controls.Remove(overlay);
                if (parentForm is Expenses expForm)
                    expForm.ListLoader();
            }
            else
            {
                var err = await resp.Content.ReadAsStringAsync();
                Cards.Show("Error", $"Failed to update expense: {resp.StatusCode} – {err}", "OK");
            }
        };
        overlay.Controls.Add(saveBtn);

        // Close button
        var closeBtn = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(overlay.Width - 40, 10),
            BackColor = Color.Transparent
        };
        closeBtn.Click += (s, ev) => parentForm.Controls.Remove(overlay);
        overlay.Controls.Add(closeBtn);

        parentForm.Controls.Add(overlay);
        overlay.BringToFront();
    }

    public static async Task GoalOverlay(Form parentForm, HttpClient _http, dynamic goalToEdit = null)
    {
        await Task.Yield();

        const int W = 350, H = 350, M = 25, CW = 300, G = 20;
        var overlay = new Guna2Panel
        {
            Name = "GoalOverlay",
            Size = new Size(W, H),
            Location = new Point((parentForm.ClientSize.Width - W) / 2, 50),
            FillColor = Color.FromArgb(18, 20, 20),
            BorderColor = Color.FromArgb(40, 40, 40),
            BorderThickness = 1,
            BorderRadius = 10,
            Anchor = AnchorStyles.Top
        };

        // Close button
        var closeBtn = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(W - 40, 10),
            BackColor = Color.Transparent
        };
        closeBtn.Click += (s, e) => parentForm.Controls.Remove(overlay);

        // Title
        var titleLabel = new Label
        {
            Text = goalToEdit == null ? "Add Goal" : "Edit Goal",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(M, 20),
            AutoSize = true
        };

        // Name
        var nameBox = new Guna2TextBox
        {
            PlaceholderText = "Goal Name",
            Size = new Size(CW, 40),
            Location = new Point(M, titleLabel.Bottom + G),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        // Amount
        var amountBox = new Guna2TextBox
        {
            PlaceholderText = "Target Amount",
            Size = new Size(CW, 40),
            Location = new Point(M, nameBox.Bottom + G),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        // Deadline
        var deadlinePicker = new Guna2DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now,
            Size = new Size(CW, 40),
            Location = new Point(M, amountBox.Bottom + G),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };

        // Prefill for editing
        if (goalToEdit != null)
        {
            nameBox.Text = goalToEdit.Name;
            amountBox.Text = goalToEdit.TargetAmount.ToString("0.##");
            deadlinePicker.Value = goalToEdit.Deadline;
        }

        // Save button
        var saveBtn = new Guna2Button
        {
            Text = goalToEdit == null ? "Add Goal" : "Save Changes",
            Size = new Size(CW, 50),
            Location = new Point(M, H - 80),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            BorderThickness = 1,
            Font = new Font("Segoe UI", 9)
        };
        saveBtn.Click += async (s, ev) =>
        {
            // validation
            var name = nameBox.Text.Trim();
            var amtTxt = amountBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Cards.Show("Validation Error", "Name is required.", "OK");
                return;
            }
            if (!decimal.TryParse(amtTxt, out var target) || target <= 0)
            {
                Cards.Show("Validation Error", "Target amount must be positive.", "OK");
                return;
            }

            // build DTO & call API
            var dto = new
            {
                Name = name,
                TargetAmount = target,
                Deadline = deadlinePicker.Value.Date
            };
            var json = JsonSerializer.Serialize(dto);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage resp;
                if (goalToEdit == null)
                    resp = await _http.PostAsync("api/goal", content);
                else
                    resp = await _http.PutAsync($"api/goal/{goalToEdit.GoalId}", content);

                if (!resp.IsSuccessStatusCode)
                {
                    var verb = goalToEdit == null ? "create" : "update";
                    Cards.Show("Error", $"Failed to {verb} goal: {resp.StatusCode}", "OK");
                    return;
                }
            }

            // remove overlay
            parentForm.Controls.Remove(overlay);

            // **Directly refresh your Goals list**
            if (parentForm is Goals goalsForm)
            {
                goalsForm.ListLoader();
            }
        };

        // assemble controls
        overlay.Controls.AddRange(new Control[]
        {
        titleLabel,
        nameBox,
        amountBox,
        deadlinePicker,
        saveBtn,
        closeBtn
        });

        parentForm.Controls.Add(overlay);
        overlay.BringToFront();
    }





}

public static class FormExtensions
{
    public static async Task InvokeAsync(this Control control, Action action)
    {
        if (control.InvokeRequired)
            await control.InvokeAsync(() => action());
        else
            action();
    }
}
