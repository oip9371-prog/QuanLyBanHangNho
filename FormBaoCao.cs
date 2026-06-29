using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHangNho.DataService;

namespace QuanLyBanHangNho.Forms.BaoCao
{
    public class FormBaoCao : Form
    {
        private DataGridView dgv = new();
        private Label lblTongDoanhThu = new(), lblTongChi = new(), lblLoiNhuan = new();
        private DateTimePicker dtpFrom = new(), dtpTo = new();

        public FormBaoCao()
        {
            BackColor = Color.FromArgb(248, 250, 252);
            Padding = new Padding(20);
            AutoScroll = true;
            AutoScrollMinSize = new Size(1000, 800);

            var hdr = new Panel { Height = 60, BackColor = Color.White, Dock = DockStyle.Top };
            hdr.Controls.Add(new Label { Text = "📊 Báo Cáo Doanh Thu & Lợi Nhuận", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = UI.Dark, Location = new Point(20, 15), AutoSize = true });
            Controls.Add(hdr);

            var filterPanel = new Panel { Location = new Point(20, 80), Size = new Size(860, 70), BackColor = Color.White };
            filterPanel.Controls.Add(new Label { Text = "Từ ngày:", Location = new Point(18, 16), AutoSize = true, ForeColor = UI.Muted });
            dtpFrom = new DateTimePicker { Location = new Point(18, 36), Size = new Size(150, 28), Format = DateTimePickerFormat.Short };
            dtpFrom.Value = DateTime.Now.AddMonths(-1);
            dtpFrom.ValueChanged += (s, e) => LoadData();
            filterPanel.Controls.Add(dtpFrom);

            filterPanel.Controls.Add(new Label { Text = "Đến ngày:", Location = new Point(188, 16), AutoSize = true, ForeColor = UI.Muted });
            dtpTo = new DateTimePicker { Location = new Point(188, 36), Size = new Size(150, 28), Format = DateTimePickerFormat.Short };
            dtpTo.Value = DateTime.Now;
            dtpTo.ValueChanged += (s, e) => LoadData();
            filterPanel.Controls.Add(dtpTo);

            var btnRefresh = new Guna.UI2.WinForms.Guna2Button { Text = "🔄 Làm mới", Location = new Point(358, 32), Size = new Size(120, 32), FillColor = UI.Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 6 };
            btnRefresh.Click += (s, e) => LoadData();
            filterPanel.Controls.Add(btnRefresh);

            Controls.Add(filterPanel);

            var statPanel = new Panel { Location = new Point(20, 160), Size = new Size(860, 100), BackColor = Color.White };
            lblTongDoanhThu = UI.MakeStat(statPanel, "0", "Tổng Doanh Thu", 10);
            UI.MakeStat(statPanel, "|", string.Empty, 280);
            lblTongChi = UI.MakeStat(statPanel, "0", "Tổng Chi (Giá Nhập)", 300);
            UI.MakeStat(statPanel, "|", string.Empty, 580);
            lblLoiNhuan = UI.MakeStat(statPanel, "0", "Lợi Nhuận", 600);
            Controls.Add(statPanel);

            Controls.Add(new Label { Text = "Chi tiết bán hàng:", Location = new Point(20, 272), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = UI.Dark });
            dgv = UI.MakeDgv(this, 20, 296, 860, 400);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var db = new AppDbContext();
                var fromDate = dtpFrom.Value.Date;
                var toDate = dtpTo.Value.Date.AddDays(1);

                var hoaDonList = db.HoaDon
                    .Where(h => h.NgayLap >= fromDate && h.NgayLap < toDate)
                    .OrderByDescending(h => h.NgayLap)
                    .Select(h => new
                    {
                        h.MaHD,
                        NgayLap = h.NgayLap.ToString("dd/MM/yyyy"),
                        NhanVien = h.NhanVien != null ? h.NhanVien.TenNV : "",
                        KhachHang = h.KhachHang != null ? h.KhachHang.TenKH : "Khách vãng lai",
                        SoSanPham = h.ChiTiet.Count,
                        DoanhThu = h.ChiTiet.Sum(c => c.SoLuongBan * c.DonGiaBan),
                        GiaNhap = h.ChiTiet.Sum(c => c.SoLuongBan * (c.HangHoa != null ? c.HangHoa.GiaNhap : 0)),
                        LoiNhuan = h.ChiTiet.Sum(c => c.SoLuongBan * (c.DonGiaBan - (c.HangHoa != null ? c.HangHoa.GiaNhap : 0)))
                    }).ToList();

                dgv.DataSource = hoaDonList;

                if (dgv.Columns.Count > 0)
                {
                    if (dgv.Columns.Contains("MaHD"))
                    {
                        dgv.Columns["MaHD"].HeaderText = "Mã HĐ";
                        dgv.Columns["MaHD"].Width = 70;
                    }
                    if (dgv.Columns.Contains("NgayLap"))
                    {
                        dgv.Columns["NgayLap"].HeaderText = "Ngày Lập";
                        dgv.Columns["NgayLap"].Width = 90;
                    }
                    if (dgv.Columns.Contains("NhanVien"))
                        dgv.Columns["NhanVien"].HeaderText = "Nhân Viên";
                    if (dgv.Columns.Contains("KhachHang"))
                        dgv.Columns["KhachHang"].HeaderText = "Khách Hàng";
                    if (dgv.Columns.Contains("SoSanPham"))
                    {
                        dgv.Columns["SoSanPham"].HeaderText = "SL";
                        dgv.Columns["SoSanPham"].Width = 50;
                    }
                    if (dgv.Columns.Contains("DoanhThu"))
                    {
                        dgv.Columns["DoanhThu"].HeaderText = "Doanh Thu";
                        dgv.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                    }
                    if (dgv.Columns.Contains("GiaNhap"))
                    {
                        dgv.Columns["GiaNhap"].HeaderText = "Giá Nhập";
                        dgv.Columns["GiaNhap"].DefaultCellStyle.Format = "N0";
                    }
                    if (dgv.Columns.Contains("LoiNhuan"))
                    {
                        dgv.Columns["LoiNhuan"].HeaderText = "Lợi Nhuận";
                        dgv.Columns["LoiNhuan"].DefaultCellStyle.Format = "N0";
                        dgv.Columns["LoiNhuan"].DefaultCellStyle.ForeColor = UI.Green;
                    }
                }

                decimal tongDoanhThu = hoaDonList.Sum(x => x.DoanhThu);
                decimal tongGiaNhap = hoaDonList.Sum(x => x.GiaNhap);
                decimal loiNhuan = tongDoanhThu - tongGiaNhap;

                lblTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " đ";
                lblTongChi.Text = tongGiaNhap.ToString("N0") + " đ";
                lblLoiNhuan.Text = loiNhuan.ToString("N0") + " đ";
                lblLoiNhuan.ForeColor = loiNhuan >= 0 ? UI.Green : UI.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}