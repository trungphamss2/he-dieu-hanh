using System;
using System.Drawing;
using System.Windows.Forms;

namespace he_dieu_hanh.Pages
{
    public partial class PageSettings : UserControl
    {
        private Label lblTitle;
        private Panel panelContent;
        private GroupBox groupAppSettings;
        // private CheckBox chkAutoSave; <-- ĐÃ BỎ
        private CheckBox chkThemeToggle; // Công tắc chuyển đổi Theme

        public PageSettings()
        {
            InitializeUI();

            // 1. Đồng bộ trạng thái ban đầu của công tắc Theme
            SyncThemeState();
            // 2. Áp dụng theme lần đầu
            ApplyTheme();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Empty;

            // Tiêu đề trang
            lblTitle = new Label()
            {
                Text = "⚙️ Cài đặt ứng dụng",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Panel chứa nội dung chính
            panelContent = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40),
            };

            // Nhóm cài đặt ứng dụng
            groupAppSettings = new GroupBox()
            {
                Text = "Tùy chọn ứng dụng",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Width = 600,
                Height = 150, // Đã giảm chiều cao GroupBox
            };

            // Checkbox 1: Theme Toggle (Được di chuyển lên trên)
            chkThemeToggle = new CheckBox()
            {
                Font = new Font("Segoe UI", 11),
                Top = 50, // Vị trí mới
                Left = 40,
                AutoSize = true,
            };

            // Gắn sự kiện để chuyển đổi Theme
            chkThemeToggle.CheckedChanged += ChkThemeToggle_CheckedChanged;
            groupAppSettings.Controls.Add(chkThemeToggle);

            // Gắn control
            panelContent.Controls.Add(groupAppSettings);

            // Canh giữa group trong panel
            groupAppSettings.Left = (panelContent.Width - groupAppSettings.Width) / 2;
            groupAppSettings.Top = 80;
            groupAppSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Xử lý sự kiện Resize để canh giữa lại
            panelContent.Resize += (s, e) =>
            {
                groupAppSettings.Left = (panelContent.Width - groupAppSettings.Width) / 2;
            };

            // Thêm các thành phần vào trang
            this.Controls.Add(panelContent);
            this.Controls.Add(lblTitle);
        }

        /// <summary>
        /// Đồng bộ trạng thái Checkbox với trạng thái ThemeManager
        /// </summary>
        private void SyncThemeState()
        {
            // Tắt sự kiện để tránh gọi ToggleTheme() khi chỉ đang đồng bộ trạng thái
            chkThemeToggle.CheckedChanged -= ChkThemeToggle_CheckedChanged;

            chkThemeToggle.Checked = ThemeManager.IsDarkMode;
            chkThemeToggle.Text = ThemeManager.IsDarkMode ? " Đã bật Chế độ tối" : " Bật chế độ tối";

            chkThemeToggle.CheckedChanged += ChkThemeToggle_CheckedChanged;
        }

        private void ChkThemeToggle_CheckedChanged(object sender, EventArgs e)
        {
            // 1. Chuyển đổi trạng thái Theme trong class ThemeManager
            ThemeManager.ToggleTheme();

            // 2. Cập nhật giao diện ngay lập tức
            ApplyTheme();

            // 3. Cập nhật lại text của checkbox
            chkThemeToggle.Text = ThemeManager.IsDarkMode ? " Đã bật Chế độ tối" : " Bật chế độ tối";
        }

        /// <summary>
        /// Áp dụng màu sắc Theme cho tất cả các controls trên trang này dựa trên ThemeManager.
        /// </summary>
        public void ApplyTheme()
        {
            // 1. Theme chung cho toàn bộ UserControl
            this.BackColor = ThemeManager.BackgroundColor;

            // 2. Tiêu đề
            lblTitle.ForeColor = ThemeManager.ForegroundColor;

            // 3. Panel/Groupbox
            groupAppSettings.BackColor = ThemeManager.PanelColor;
            groupAppSettings.ForeColor = ThemeManager.ForegroundColor;

            // 4. Các Checkbox (chỉ còn chkThemeToggle)
            chkThemeToggle.ForeColor = ThemeManager.ForegroundColor;
            chkThemeToggle.BackColor = ThemeManager.PanelColor;
        }
    }
}