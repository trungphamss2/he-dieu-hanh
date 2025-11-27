using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace he_dieu_hanh.Pages
{
    public partial class PageEventLog : UserControl
    {
        private FlowLayoutPanel topPanel;
        private Panel bottomPanel;
        private DataGridView dgvEvents;
        private Label lblStatus;
        private ProgressBar progressReplay;

        private Button btnStartRecord, btnStopRecord, btnSaveLog, btnLoadLog, btnClearLog;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        // Thuộc tính để người khác truyền vào lớp logic (chỉ khai báo, không xử lý)
        public IEventLogger Logger { get; set; }

        public PageEventLog()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ThemeManager.BackgroundColor;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // === KHỞI TẠO CÁC CONTROL UI ===

            // ==== THANH CÔNG CỤ TRÊN ====
            topPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = ThemeManager.PanelColor
            };

            btnStartRecord = CreateButton("🟢 Start Record", Color.FromArgb(52, 152, 219));
            btnStopRecord = CreateButton("⛔ Stop Record", Color.FromArgb(231, 76, 60));
            btnSaveLog = CreateButton("💾 Save Log", Color.FromArgb(46, 204, 113));
            btnLoadLog = CreateButton("📂 Load Log", Color.FromArgb(241, 196, 15));
            btnClearLog = CreateButton("🧹 Clear Log", Color.FromArgb(127, 140, 141));

            topPanel.Controls.AddRange(new Control[]
            {
                btnStartRecord, btnStopRecord, btnSaveLog, btnLoadLog, btnClearLog
            });

            // ==== BẢNG LOG (DataGridView) ====
            dgvEvents = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.BackgroundColor,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10),
                ForeColor = ThemeManager.ForegroundColor,
                GridColor = Color.FromArgb(200, 200, 200)
            };

            // Thiết lập Cột
            dgvEvents.Columns.Add("colIndex", "#");
            dgvEvents.Columns.Add("colTime", "Time");
            dgvEvents.Columns.Add("colType", "Type");
            dgvEvents.Columns.Add("colDetails", "Details");

            // Cột nút Replay
            var replayColumn = new DataGridViewButtonColumn()
            {
                HeaderText = "Replay",
                Name = "colReplay",
                Text = "▶️ Replay",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            dgvEvents.Columns.Add(replayColumn);

            // Gắn sự kiện để xử lý nhấp nút Replay (UI chỉ hiển thị MessageBox)
            dgvEvents.CellClick += DgvEvents_CellClick;

            // Định dạng Cell/Header (UI/UX)
            dgvEvents.CellFormatting += DgvEvents_CellFormatting;
            dgvEvents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvEvents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 220, 220);
            dgvEvents.EnableHeadersVisualStyles = false;

            // ==== THANH TRẠNG THÁI ====
            bottomPanel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(10),
                BackColor = ThemeManager.PanelColor
            };

            lblStatus = new Label()
            {
                Text = "Status: Ready (No logic loaded)", // Cập nhật trạng thái mặc định
                Dock = DockStyle.Left,
                Width = 350,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = ThemeManager.ForegroundColor
            };

            progressReplay = new ProgressBar()
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            bottomPanel.Controls.Add(progressReplay);
            bottomPanel.Controls.Add(lblStatus);

            // === THÊM CONTROLS VÀO USER CONTROL ===
            this.Controls.Add(dgvEvents);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);

            // === HIỆU ỨNG NÚT ===
            AddButtonEffects();
        }

        private void DgvEvents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvEvents.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                e.CellStyle.BackColor = Color.FromArgb(52, 152, 219);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        // CHỨC NĂNG CƠ BẢN CỦA UI: Hiển thị MessageBox khi nhấn Replay
        private void DgvEvents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvEvents.Columns[e.ColumnIndex].Name == "colReplay")
            {
                string eventType = dgvEvents.Rows[e.RowIndex].Cells["colType"].Value?.ToString() ?? "N/A";
                string details = dgvEvents.Rows[e.RowIndex].Cells["colDetails"].Value?.ToString() ?? "N/A";

                // Chỉ hiển thị thông báo, không thực hiện Replay logic
                MessageBox.Show($"[UI Simulation] Tín hiệu Replay được gửi cho:\n\nLoại: {eventType}\nChi tiết: {details}",
                                 "Replay Event - UI Only", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private Button CreateButton(string text, Color baseColor)
        {
            var btn = new Button()
            {
                Text = text,
                ForeColor = Color.White,
                BackColor = baseColor,
                Width = 130,
                Height = 40,
                Margin = new Padding(5),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 15, 15));
            return btn;
        }

        private void AddButtonEffects()
        {
            foreach (Control control in topPanel.Controls)
            {
                if (control is Button btn)
                {
                    // Hiệu ứng Hover (UX)
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = ControlPaint.Light(btn.BackColor);
                        btn.Cursor = Cursors.Hand;
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = ControlPaint.Dark(btn.BackColor, 0.1f);
                        btn.Cursor = Cursors.Default;
                    };
                    // Xử lý Click (chỉ xử lý các lệnh cơ bản/UI)
                    btn.Click += Btn_Click;
                }
            }
        }

        // CHỨC NĂNG CƠ BẢN CỦA UI: Xử lý nút và cập nhật Status/Clear Log
        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                lblStatus.Text = $"Status: {btn.Text} clicked (Waiting for logic)"; // Cập nhật trạng thái

                switch (btn.Text)
                {
                    case "🧹 Clear Log":
                        dgvEvents.Rows.Clear();
                        lblStatus.Text = "Status: Log cleared";
                        break;
                    case "💾 Save Log":
                        // Giữ lại hàm UI cho phép chọn file
                        SimulatedSaveLogToFile();
                        break;
                    case "📂 Load Log":
                        // Giữ lại hàm UI cho phép chọn file
                        SimulatedLoadLogFromFile();
                        break;
                    // Start/Stop sẽ được người khác gán hàm logic vào
                    default:
                        // Nếu là Start/Stop, chỉ cập nhật trạng thái
                        break;
                }
            }
        }

        // Hàm giúp người khác hiển thị log lên DataGridView
        public void AddEvent(string type, string details)
        {
            // Dùng Invoke/BeginInvoke để đảm bảo an toàn luồng (cần thiết cho logic người khác)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string>(AddEvent), type, details);
                return;
            }

            dgvEvents.Rows.Add(
                dgvEvents.Rows.Count + 1,
                DateTime.Now.ToString("HH:mm:ss"),
                type,
                details
            );
            // Cuộn xuống dòng cuối (UX)
            if (dgvEvents.Rows.Count > 0)
            {
                dgvEvents.FirstDisplayedScrollingRowIndex = dgvEvents.Rows.Count - 1;
            }
        }

        // --- CÁC HÀM CHỈ LÀ UI SIMULATION CHO VIỆC LƯU/TẢI FILE ---
        // Người khác sẽ cần viết logic thực sự để parse dữ liệu.

        private void SimulatedSaveLogToFile()
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt",
                FileName = "event_log.txt"
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // KHÔNG LƯU DỮ LIỆU THỰC SỰ
                    lblStatus.Text = $"Status: Save dialog opened and closed → {Path.GetFileName(sfd.FileName)}";
                }
            }
        }

        private void SimulatedLoadLogFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // KHÔNG TẢI DỮ LIỆU THỰC SỰ
                    lblStatus.Text = $"Status: Load dialog opened and closed ← {Path.GetFileName(ofd.FileName)}";
                }
            }
        }
    }
}