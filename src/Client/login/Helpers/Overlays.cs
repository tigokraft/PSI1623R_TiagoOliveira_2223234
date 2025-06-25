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
    // In your Overlays class:
    public static async Task IncomeOverlay(Form parentForm, HttpClient _http, Incomes.Income incomeToEdit = null)
    {
        await Task.Yield();

        // ── Layout constants ───────────────────────────────────────────────
        const int W = 380;
        const int BaseHeight = 460;   // height without recurrence block
        const int MX = 25;
        const int CTRLW = 330;
        const int GAP = 15;
        const int LGAP = 4;
        const int initialTop = 85;    // start this higher so there's room above

        // ── Main overlay panel ────────────────────────────────────────────
        var overlay = new Guna2Panel
        {
            Name = "IncomeOverlay",
            Size = new Size(W, BaseHeight),
            Location = new Point((parentForm.ClientSize.Width - W) / 2, initialTop),
            FillColor = Color.FromArgb(18, 20, 20),
            BorderColor = Color.FromArgb(40, 40, 40),
            BorderThickness = 1,
            BorderRadius = 10,
            Anchor = AnchorStyles.Top,
            AutoScroll = false
        };
        overlay.SuspendLayout();

        int currY = 20;

        // ── Close button ────────────────────────────────────────────────
        var btnClose = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(W - 40, 10),
            BackColor = Color.Transparent
        };
        btnClose.Click += (s, e) => parentForm.Controls.Remove(overlay);
        overlay.Controls.Add(btnClose);

        // ── Helpers ─────────────────────────────────────────────────────
        Label MakeLabel(string text)
        {
            var lbl = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Location = new Point(MX, currY),
                AutoSize = true
            };
            overlay.Controls.Add(lbl);
            currY += lbl.Height + LGAP;
            return lbl;
        }
        Guna2TextBox MakeTextBox(string placeholder)
        {
            var tb = new Guna2TextBox
            {
                PlaceholderText = placeholder,
                Size = new Size(CTRLW, 40),
                Location = new Point(MX, currY),
                BorderColor = Color.FromArgb(67, 79, 82),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.White,
                BorderRadius = 10
            };
            overlay.Controls.Add(tb);
            currY += tb.Height + GAP;
            return tb;
        }

        // ── Title ───────────────────────────────────────────────────────
        var lblTitle = new Label
        {
            Text = incomeToEdit == null ? "Add Income" : "Edit Income",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(MX, currY),
            AutoSize = true
        };
        overlay.Controls.Add(lblTitle);
        currY += lblTitle.Height + GAP;

        // ── Description ────────────────────────────────────────────────
        MakeLabel("Description");
        var txtDesc = MakeTextBox("e.g. Salary, Bonus…");

        // ── Amount ─────────────────────────────────────────────────────
        MakeLabel("Amount");
        var txtAmt = MakeTextBox("0.00");

        // ── Date occurred ──────────────────────────────────────────────
        MakeLabel("Date");
        var dtOccurred = new Guna2DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now,
            Size = new Size(CTRLW, 40),
            Location = new Point(MX, currY),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        overlay.Controls.Add(dtOccurred);
        currY += dtOccurred.Height + GAP;

        // ── Recurring toggle ───────────────────────────────────────────
        var chkRecurring = new Guna2CheckBox
        {
            Text = "Recurring",
            Size = new Size(CTRLW, 30),
            Location = new Point(MX, currY),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9),
            CheckedState = { FillColor = Color.FromArgb(67, 79, 82), BorderColor = Color.FromArgb(67, 79, 82) },
            UncheckedState = { FillColor = Color.FromArgb(125, 137, 149), BorderColor = Color.FromArgb(67, 79, 82) }
        };
        overlay.Controls.Add(chkRecurring);
        currY += chkRecurring.Height + GAP;

        // ── Recurrence + End Date panel ────────────────────────────────
        int recPanelY = currY;
        var recPanel = new Panel
        {
            Location = new Point(MX, recPanelY),
            Size = new Size(CTRLW, 0),
            AutoScroll = false
        };
        overlay.Controls.Add(recPanel);

        // inside recPanel: Recurrence
        var lblRec = new Label
        {
            Text = "Recurrence",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9),
            Location = new Point(0, 0),
            AutoSize = true
        };
        recPanel.Controls.Add(lblRec);

        var cmbRec = new Guna2ComboBox
        {
            Items = { "Weekly", "Monthly", "Yearly" },
            Size = new Size(CTRLW, 40),
            Location = new Point(0, lblRec.Height + LGAP),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        recPanel.Controls.Add(cmbRec);

        // inside recPanel: End Date
        int afterRecY = lblRec.Height + LGAP + cmbRec.Height + GAP;
        var lblEnd = new Label
        {
            Text = "End Date",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9),
            Location = new Point(0, afterRecY),
            AutoSize = true
        };
        recPanel.Controls.Add(lblEnd);

        var dtEnd = new Guna2DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now,
            Size = new Size(CTRLW, 40),
            Location = new Point(0, afterRecY + lblEnd.Height + LGAP),
            BorderColor = Color.FromArgb(67, 79, 82),
            FillColor = Color.FromArgb(18, 20, 20),
            ForeColor = Color.White,
            BorderRadius = 10
        };
        recPanel.Controls.Add(dtEnd);

        int fullRecH = afterRecY + lblEnd.Height + LGAP + dtEnd.Height;

        // ── Category selector placeholder ──────────────────────────────
        var lblCat = new Label { ForeColor = Color.White, Font = new Font("Segoe UI", 9), AutoSize = true };
        var pnlCat = new Guna2Panel
        {
            Size = new Size(CTRLW, 40),
            FillColor = Color.FromArgb(18, 20, 20),
            BorderRadius = 10,
            ShadowDecoration = { Enabled = false }
        };
        var flowCat = new FlowLayoutPanel
        {
            Location = Point.Empty,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        pnlCat.Controls.Add(flowCat);

        var scrollCat = new Guna2HScrollBar
        {
            Minimum = 0,
            LargeChange = CTRLW,
            FillColor = Color.FromArgb(40, 40, 40),
            ThumbColor = Color.FromArgb(100, 100, 100),
            BorderRadius = 3
        };
        overlay.Controls.Add(lblCat);
        overlay.Controls.Add(pnlCat);
        overlay.Controls.Add(scrollCat);

        // ── Save/Add button ────────────────────────────────────────────
        var btnSave = new Guna2Button
        {
            Size = new Size(CTRLW, 50),
            FillColor = Color.FromArgb(20, 24, 26),
            BorderColor = Color.FromArgb(39, 42, 44),
            BorderRadius = 10,
            BorderThickness = 1,
            Font = new Font("Segoe UI", 9)
        };
        overlay.Controls.Add(btnSave);

        // ── Reposition routine ─────────────────────────────────────────
        void RepositionAll()
        {
            // resize recPanel
            recPanel.Height = chkRecurring.Checked ? fullRecH : 0;

            // move category label & panel
            int y0 = recPanelY + recPanel.Height + GAP;
            lblCat.Text = "Category";
            lblCat.Location = new Point(MX, y0);

            pnlCat.Location = new Point(MX, y0 + lblCat.Height + LGAP);
            flowCat.PerformLayout();

            scrollCat.Location = new Point(MX, pnlCat.Bottom + LGAP);
            scrollCat.Size = new Size(CTRLW, 6);
            scrollCat.Maximum = Math.Max(0, flowCat.Width - CTRLW);

            // move button
            btnSave.Location = new Point(MX, scrollCat.Bottom + GAP);
            btnSave.Text = incomeToEdit == null ? "Add Income" : "Save Changes";

            // grow overlay symmetrically
            int newHeight = Math.Max(BaseHeight, btnSave.Bottom + GAP);
            int delta = newHeight - overlay.Height;
            overlay.Height = newHeight;
            overlay.Top -= delta / 2;  // shift up half the growth
        }

        chkRecurring.CheckedChanged += (s, e) => RepositionAll();
        RepositionAll();

        // ── Load categories ─────────────────────────────────────────────
        int selectedCategoryId = -1;
        var categories = await CategoriesList.GetCategoriesAsync(_http);
        var catMenu = new ContextMenuStrip();
        var miEditCat = new ToolStripMenuItem("Edit Category");
        var miDelCat = new ToolStripMenuItem("Delete Category");
        catMenu.Items.AddRange(new[] { miEditCat, miDelCat });

        miEditCat.Click += async (s, e) =>
        {
            int cid = (int)catMenu.Tag;
            var c = categories.First(x => x.CategoryId == cid);
            await EditCategoryOverlay(parentForm, _http, cid, c.CategoryName, c.Color);
            parentForm.Controls.Remove(overlay);
        };
        miDelCat.Click += async (s, e) =>
        {
            int cid = (int)catMenu.Tag;
            if (Cards.Show("Delete Category", "This will delete the category. Continue?", "OK") == DialogResult.OK)
            {
                var resp = await _http.DeleteAsync($"api/category/{cid}");
                if (resp.IsSuccessStatusCode)
                {
                    Cards.Show("Success", "Category deleted.", "OK");
                    parentForm.Controls.Remove(overlay);
                    await IncomeOverlay(parentForm, _http);
                }
                else Cards.Show("Error", "Failed to delete category.", "OK");
            }
        };

        foreach (var c in categories)
        {
            int dot = 12;
            int textW = TextRenderer.MeasureText(c.CategoryName, new Font("Segoe UI", 9)).Width;
            var btn = new Guna2Button
            {
                Text = c.CategoryName,
                AutoRoundedCorners = true,
                BorderRadius = 15,
                Size = new Size(dot + 6 + textW + 20, 30),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.LightGray,
                ButtonMode = ButtonMode.RadioButton,
                Tag = c.CategoryId,
                Font = new Font("Segoe UI", 9)
            };
            btn.CheckedState.FillColor = Color.FromArgb(60, 60, 60);
            btn.CheckedState.ForeColor = Color.White;
            var bmp = new Bitmap(dot, dot);
            using (var g = Graphics.FromImage(bmp))
                g.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(c.Color)), 0, 0, dot, dot);
            btn.Image = bmp;
            btn.ImageSize = new Size(dot, dot);
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(dot + 6, 0, 0, 0);

            btn.CheckedChanged += (s, e) => { if (btn.Checked) selectedCategoryId = (int)btn.Tag; };
            btn.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    catMenu.Tag = btn.Tag;
                    catMenu.Show(btn, new Point(e.X, e.Y));
                }
            };
            flowCat.Controls.Add(btn);
        }

        // “+” button
        var plus = new Guna2Button
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
        plus.Click += async (s, e) =>
        {
            parentForm.Controls.Remove(overlay);
            await CategoryOverlay(parentForm, _http);
        };
        flowCat.Controls.Add(plus);
        flowCat.PerformLayout();
        RepositionAll();

        // ── Save/Add click ─────────────────────────────────────────────
        btnSave.Click += async (s, ev) =>
        {
            var desc = txtDesc.Text.Trim();
            var amtTxt = txtAmt.Text.Trim();
            if (string.IsNullOrEmpty(desc))
            {
                Cards.Show("Validation Error", "Description is required.", "OK"); return;
            }
            if (!decimal.TryParse(amtTxt, out var amt) || amt <= 0)
            {
                Cards.Show("Validation Error", "Amount must be a positive number.", "OK"); return;
            }
            if (selectedCategoryId == -1)
            {
                Cards.Show("Validation Error", "Select a category.", "OK"); return;
            }
            if (chkRecurring.Checked && cmbRec.SelectedIndex < 0)
            {
                Cards.Show("Validation Error", "Choose a recurrence type.", "OK"); return;
            }

            string dateStr = dtOccurred.Value.ToString("yyyy-MM-dd");
            bool isRec = chkRecurring.Checked;
            string recStr = isRec ? cmbRec.SelectedItem.ToString() : "";
            string endDateStr = isRec ? dtEnd.Value.ToString("yyyy-MM-dd")
                                      : DateTime.Now.ToString("yyyy-MM-dd");

            bool success = false;
            if (incomeToEdit == null)
            {
                success = await Tasks.PostIncome(amt, dateStr, desc, isRec, recStr, endDateStr, _http, selectedCategoryId);
            }
            else
            {
                var payload = new { incomeToEdit.IncomeId, Amount = amt, Descr = desc, Date = dateStr, CategoryId = selectedCategoryId };
                var json = JsonSerializer.Serialize(payload);
                var contentReq = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _http.PutAsync($"api/income/{incomeToEdit.IncomeId}", contentReq);
                success = resp.IsSuccessStatusCode;
                if (!success) Cards.Show("Error", $"Error updating income: {resp.StatusCode}", "OK");
            }

            if (success)
            {
                parentForm.Controls.Remove(overlay);
                if (parentForm is Incomes inc) await inc.InvokeAsync(() => inc.ListLoader());
            }
        };

        overlay.ResumeLayout();
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

        // Title
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

        // Description
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

        // Amount
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

        // Date
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

        // Category
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
        var inner = new FlowLayoutPanel
        {
            Location = Point.Empty,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        maskPanel.Controls.Add(inner);
        overlay.Controls.Add(maskPanel);
        currY += maskPanel.Height + labelGap;

        // Scrollbar under categories
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
        overlay.Controls.Add(hScroll);
        currY += hScroll.Height + gap;

        // Load categories
        int selectedCategoryId = -1;
        var categories = await CategoriesList.GetCategoriesAsync(_http);
        var catContextMenu = new ContextMenuStrip();
        var editCatItem = new ToolStripMenuItem("Edit Category");
        var deleteCatItem = new ToolStripMenuItem("Delete Category");
        catContextMenu.Items.AddRange(new ToolStripItem[] { editCatItem, deleteCatItem });
        editCatItem.Click += async (s, e) =>
        {
            int cid = (int)catContextMenu.Tag;
            var cat = categories.First(c => c.CategoryId == cid);
            await EditCategoryOverlay(parentForm, _http, cid, cat.CategoryName, cat.Color);
            parentForm.Controls.Remove(overlay);
        };
        deleteCatItem.Click += async (s, e) =>
        {
            int cid = (int)catContextMenu.Tag;
            if (Cards.Show("Delete Category", "This will delete the category. Continue?", "OK") == DialogResult.OK)
            {
                var resp = await _http.DeleteAsync($"api/category/{cid}");
                if (resp.IsSuccessStatusCode)
                {
                    Cards.Show("Success", "Category deleted.", "OK");
                    parentForm.Controls.Remove(overlay);
                    await ExpenseOverlay(parentForm, _http);
                }
                else Cards.Show("Error", "Failed to delete category.", "OK");
            }
        };

        // Render each category as a pill
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
            btn.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    catContextMenu.Tag = btn.Tag;
                    catContextMenu.Show(btn, new Point(e.X, e.Y));
                }
            };

            inner.Controls.Add(btn);
        }

        // ★ Add-New-Category “+” button ★
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

        // Add Expense Button
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

        // Close Button
        var btnClose2 = new Guna2ImageButton
        {
            Image = login.Properties.Resources.close,
            Size = new Size(30, 30),
            Location = new Point(overlayWidth - 40, 10),
            BackColor = Color.Transparent
        };
        btnClose2.Click += (s, ev) => parentForm.Controls.Remove(overlay);
        overlay.Controls.Add(btnClose2);

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
