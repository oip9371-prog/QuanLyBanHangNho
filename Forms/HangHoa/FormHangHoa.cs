using Microsoft.EntityFrameworkCore;
using QuanLyBanHangNho.DataService;
using QuanLyBanHangNho.Model;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBanHangNho.Forms.HangHoa
{
    public class FormHangHoa : Form
    {
        private DataGridView dgv = new();
        private Guna.UI2.WinForms.Guna2TextBox txtSearch = new();
        private Label lblTong = new(), lblTon = new(), lblCanhBao = new();

        public FormHangHoa()
        {
            Text = "Quản lý hàng hóa";
            BackColor = UI.Bg;
            Padding = new Padding(0);
            MinimumSize = new Size(920, 700);
            AutoScaleMode = AutoScaleMode.Dpi;

            var mainContainer = new Panel { Dock = DockStyle.Fill, BackColor = UI.Bg };

            var header = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = UI.White };
            header.Controls.Add(new Label
            {
                Text = "Quản Lý Hàng Hóa",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UI.Dark,
                Location = new Point(20, 12),
                AutoSize = true
            });
            mainContainer.Controls.Add(header);

            var contentContainer = new Panel { Dock = DockStyle.Fill, BackColor = UI.Bg };

            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = UI.White,
                AutoScroll = true
            };
            contentContainer.Controls.Add(leftPanel);

            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UI.White,
                Padding = new Padding(20)
            };
            contentContainer.Controls.Add(rightPanel);

            mainContainer.Controls.Add(contentContainer);
            Controls.Add(mainContainer);

            leftPanel.Controls.Add(new Label
            {
                Text = "Tìm kiếm & thao tác",
                Location = new Point(18, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UI.Dark
            });

            txtSearch = UI.MakeSearch(leftPanel, 18, 48, 244);
            txtSearch.PlaceholderText = "🔍 Tìm mã hoặc tên...";
            txtSearch.TextChanged += (s, e) => Load_();

            UI.MakeBtn(leftPanel, "➕ Thêm mới", UI.Green, 18, 100, 244, (s, e) => OpenDialog(null));
            UI.MakeBtn(leftPanel, "✏️ Sửa", Color.FromArgb(245, 158, 11), 18, 150, 244, (s, e) =>
            {
                var maHH = GetSelectedMaHH();
                if (string.IsNullOrWhiteSpace(maHH))
                {
                    MessageBox.Show("Chọn hàng cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                OpenDialog(maHH);
            });
            UI.MakeBtn(leftPanel, "🗑️ Xóa", UI.Red, 18, 200, 244, (s, e) => DeleteSelected());
            UI.MakeBtn(leftPanel, "🔄 Tải lại", UI.Blue, 18, 250, 118, (s, e) => Load_());
            UI.MakeBtn(leftPanel, "🧹 Xóa filter", UI.Dark, 144, 250, 118, (s, e) => txtSearch.Clear());

            leftPanel.Controls.Add(new Label
            {
                Text = "Thống kê nhanh",
                Location = new Point(18, 318),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UI.Dark
            });

            var statCard = UI.MakeCard(leftPanel, 18, 350, 244, 270);
            statCard.BackColor = UI.White;
            lblTong = UI.MakeStat(statCard, "0", "Tổng mặt hàng", 0);
            UI.MakeStat(statCard, "|", string.Empty, 180);
            lblTon = UI.MakeStat(statCard, "0", "Tổng tồn kho", 200);
            UI.MakeStat(statCard, "|", string.Empty, 360);
            lblCanhBao = UI.MakeStat(statCard, "0", "Sắp hết hàng (≤10)", 380);

            rightPanel.Controls.Add(new Label
            {
                Text = "Danh sách hàng hóa",
                Location = new Point(18, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UI.Dark,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            });

            dgv = UI.MakeDgv(rightPanel, 18, 52, 1000, 580);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.DoubleClick += (s, e) =>
            {
                var maHH = GetSelectedMaHH();
                if (!string.IsNullOrWhiteSpace(maHH))
                    OpenDialog(maHH);
            };

            Load_();
        }

        private void Load_()
        {
            try
            {
                using var db = new AppDbContext();

                var q = db.HangHoa.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string key = txtSearch.Text.Trim();

                    q = q.Where(h =>
                        h.MaHH.Contains(key) ||
                        h.TenHH.Contains(key));
                }

                var list = q
                    .OrderBy(h => h.MaHH)
                    .Select(h => new
                    {
                        h.MaHH,
                        h.TenHH,
                        h.GiaNhap,
                        h.GiaBan,
                        h.SoLuongTon
                    })
                    .ToList();

                dgv.DataSource = null;
                dgv.DataSource = list;

                dgv.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách hàng hóa.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void OpenDialog(string? maHH)
        {
            using var f = new FormThemSuaHH(maHH);
            if (f.ShowDialog() == DialogResult.OK)
                Load_();
        }

        private string? GetSelectedMaHH()
        {
            if (dgv.SelectedRows.Count == 0)
                return null;
            var row = dgv.SelectedRows[0];
            if (!dgv.Columns.Contains("MaHH"))
                return null;
            return row.Cells["MaHH"]?.Value?.ToString();
        }

        private string? GetSelectedTenHH()
        {
            if (dgv.SelectedRows.Count == 0)
                return null;
            var row = dgv.SelectedRows[0];
            if (!dgv.Columns.Contains("TenHH"))
                return null;
            return row.Cells["TenHH"]?.Value?.ToString();
        }

        private void DeleteSelected()
        {
            var maHH = GetSelectedMaHH();
            var ten = GetSelectedTenHH();
            if (string.IsNullOrWhiteSpace(maHH))
            {
                MessageBox.Show("Chọn hàng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa '{ten ?? maHH}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                bool hasTrans = db.ChiTietHoaDon.Any(x => x.MaHH == maHH) || db.ChiTietPhieuNhap.Any(x => x.MaHH == maHH);
                if (hasTrans)
                {
                    MessageBox.Show("Không thể xóa vì đã có giao dịch!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var hh = db.HangHoa.Find(maHH);
                if (hh != null)
                {
                    db.HangHoa.Remove(hh);
                    db.SaveChanges();
                }

                Load_();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class FormThemSuaHH : Form
    {
        private readonly string? _maHH;
        private Guna.UI2.WinForms.Guna2TextBox txtMa = new(), txtTen = new(), txtGiaNhap = new(), txtGiaBan = new(), txtTon = new();

        public FormThemSuaHH(string? maHH)
        {
            _maHH = maHH;
            Text = maHH == null ? "Thêm hàng hóa" : "Sửa hàng hóa";
            Size = new Size(500, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = UI.White;

            UI.MakeHeader(this, Text);

            int y = 70;
            txtMa = UI.MakeField(this, "Mã hàng hóa *", ref y, 420);
            txtTen = UI.MakeField(this, "Tên hàng hóa *", ref y, 420);
            txtGiaNhap = UI.MakeField(this, "Giá nhập (đ) *", ref y, 420);
            txtGiaBan = UI.MakeField(this, "Giá bán (đ) *", ref y, 420);

            if (maHH != null)
            {
                txtMa.ReadOnly = true;
                txtTon = UI.MakeField(this, "Số lượng tồn", ref y, 420);
            }

            if (maHH != null)
                LoadData();

            var pnlBot = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = UI.Bg };
            pnlBot.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = UI.Border });
            UI.MakeBtn(pnlBot, "💾 Lưu", UI.Blue, 16, 12, 120, (s, e) => Save_());
            UI.MakeBtn(pnlBot, "Hủy", Color.FromArgb(226, 232, 240), 152, 12, 110, (s, e) => DialogResult = DialogResult.Cancel);
            Controls.Add(pnlBot);
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            var hh = db.HangHoa.Find(_maHH);
            if (hh == null) return;

            txtMa.Text = hh.MaHH;
            txtTen.Text = hh.TenHH;
            txtGiaNhap.Text = hh.GiaNhap.ToString();
            txtGiaBan.Text = hh.GiaBan.ToString();
            if (txtTon != null)
                txtTon.Text = hh.SoLuongTon.ToString();
        }

        private void Save_()
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtGiaNhap.Text, out var gn) || gn < 0 || !decimal.TryParse(txtGiaBan.Text, out var gb) || gb < 0)
            {
                MessageBox.Show("Giá nhập/bán không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();
                if (_maHH == null)
                {
                    if (db.HangHoa.Any(h => h.MaHH == txtMa.Text.Trim()))
                    {
                        MessageBox.Show("Mã hàng đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    db.HangHoa.Add(new Model.HangHoa
                    {
                        MaHH = txtMa.Text.Trim(),
                        TenHH = txtTen.Text.Trim(),
                        GiaNhap = gn,
                        GiaBan = gb,
                        SoLuongTon = 0
                    });
                }
                else
                {
                    var hh = db.HangHoa.Find(_maHH);
                    if (hh != null)
                    {
                        hh.TenHH = txtTen.Text.Trim();
                        hh.GiaNhap = gn;
                        hh.GiaBan = gb;
                        if (!string.IsNullOrWhiteSpace(txtTon.Text) && int.TryParse(txtTon.Text, out var ton) && ton >= 0)
                            hh.SoLuongTon = ton;
                    }
                }

                db.SaveChanges();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
