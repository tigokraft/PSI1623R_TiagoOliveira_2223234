using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using LiveCharts.WinForms;
using login.Helpers;
using login.Tabs;
using System.Runtime.InteropServices;

namespace login
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _http;

        public MainForm(HttpClient httpClient)
        {
            InitializeComponent();
            _http = httpClient;    

            // load default state
            Overview overview = new Overview(_http);
            overview.TopLevel = false;
            former.Controls.Clear();
            former.Controls.Add(overview);
            overview.Show();
        }

        
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ExpensesBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            IncomeBtn.FillColor = Color.FromArgb(100, 14, 18, 18);

            Expenses exp = new Expenses(_http);
            exp.TopLevel = false;
            former.Controls.Clear();
            former.Controls.Add(exp);
            exp.Show();
        }

        private void OvBtn_Click(object sender, EventArgs e)
        {
            OvBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            IncomeBtn.FillColor = Color.FromArgb(100, 14, 18, 18);

            Overview ov = new Overview(_http);
            ov.TopLevel = false;
            former.Controls.Clear();
            former.Controls.Add(ov);
            former.Controls.Add(ov);
            ov.Show();
        }

        private void budgetBtn_Click(object sender, EventArgs e)
        {
            budgetBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            IncomeBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }

        private void GoalsBtn_Click(object sender, EventArgs e)
        {
            GoalsBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            IncomeBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }
        private void IncomeBtn_Click(object sender, EventArgs e)
        {
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            IncomeBtn.FillColor = Color.FromArgb(100, 27, 43, 48);

            Incomes inc = new Incomes(_http);
            inc.TopLevel = false;
            former.Controls.Clear();
            former.Controls.Add(inc);
            inc.Show();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("auth.token"))
            {
                File.Delete("auth.token");
                Application.Exit();
            }
        }

        private void BalanceTxt_Click(object sender, EventArgs e)
        {

        }


    }
}
