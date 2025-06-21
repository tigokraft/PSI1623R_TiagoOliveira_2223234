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
using System.Text.Json;
using System.Xml;

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

            Setup();
        }

        private void Setup()
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year - 5, now.Month, 1);

            cmbMonths.Items.Clear(); // Clear existing items
            cmbMonths.Items.Add("All Months"); // Add "All Months" at the top

            // Build the list ascending, then reverse
            List<string> months = new List<string>();
            while (start <= now)
            {
                months.Add($"{start:MMMM yyyy}");
                start = start.AddMonths(1);
            }
            months.Reverse(); // Descending order

            foreach (var month in months)
                cmbMonths.Items.Add(month);

            cmbMonths.DropDownHeight = 200;
            cmbMonths.DropDownStyle = ComboBoxStyle.DropDownList; // Set dropdown style
            cmbMonths.SelectedIndex = 0; // Select "All Months" by default
            cmbMonths.MaxDropDownItems = 5; // Limit the number of items shown in the dropdown
        }

        private async void AddBtn_Click(object sender, EventArgs e)
        {
            await Overlays.IncomeOverlay(this, _http);
        }

        private void cmbCat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
