using System;
using System.Drawing;
using System.Windows.Forms;

namespace he_dieu_hanh
{
    public partial class MainForm : Form
    {
        private Panel sideMenu;
        private Panel contentPanel;
        private Button btnDashboard, btnEventLog, btnSettings, btnAbout;

        public MainForm()
        {
            this.Text = "Hook Logger - Multi Page";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            InitializeUI();
            ShowPage(new Pages.PageDashboard());
        }

        private void InitializeUI()
        {
            // Thanh menu bên trái
            sideMenu = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(45, 52, 70)
            };

            // Nút menu
            btnDashboard = CreateMenuButton("🏠 Dashboard");
            btnEventLog = CreateMenuButton("🖱️ Event Log");
            btnSettings = CreateMenuButton("⚙️ Settings");

            sideMenu.Controls.AddRange(new Control[] { btnAbout, btnSettings, btnEventLog, btnDashboard });

            // Khu hiển thị trang
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250)
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(sideMenu);

            // Gắn sự kiện
            btnDashboard.Click += (s, e) => ShowPage(new Pages.PageDashboard());
            btnEventLog.Click += (s, e) => ShowPage(new Pages.PageEventLog());
            btnSettings.Click += (s, e) => ShowPage(new Pages.PageSettings());
        }

        private Button CreateMenuButton(string text)
        {
            var btn = new Button()
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 60,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(52, 73, 94);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(64, 90, 120);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 73, 94);

            return btn;
        }

        public void ApplyTheme()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            foreach (Control c in this.Controls)
            {
                ApplyThemeRecursive(c);
            }
        }

        private void ApplyThemeRecursive(Control control)
        {
            control.ForeColor = ThemeManager.ForegroundColor;
            control.BackColor = (control is Panel or GroupBox) ? ThemeManager.PanelColor : ThemeManager.BackgroundColor;

            foreach (Control child in control.Controls)
                ApplyThemeRecursive(child);
        }


        private void ShowPage(UserControl page)
        {
            contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(page);
        }
    }
}
