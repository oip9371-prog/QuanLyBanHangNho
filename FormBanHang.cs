using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHangNho.DataService;
using QuanLyBanHangNho.Model;

namespace QuanLyBanHangNho.Forms.BanHang
{
    public class FormBanHang : Form
    {
        private ComboBox cboNV = new(), cboKH = new(), cboHH = new();
        private NumericUpDown numSL = new();
        private DataGridView dgvCart = new();
        private Label lblTong = new();
        private List<CartItem> _cart = new();

        class CartItem { public string MaHH = ""; public string TenHH = ""; public int SL; public decimal DonGia; public decimal ThanhTien => SL * DonGia; }

        public FormBanHang()
        {
            BackColor = Color.FromArgb(248, 250, 252);
            Padding = new Padding(20);
            Font = new Font("Segoe UI", 9.5f);
            AutoScroll = true;
            AutoScrollMinSize = new Size(1000, 880);

            var card = UI.MakeCard(this, 20, 20, 880, 820);

            var hdr = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = UI.Blue };
            hdr.Controls.Add(new Label { Text = "🧾  Lập Hóa Đơn Mới", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            card.Controls.Add(hdr);

            int lx = 16, ly = 56;

            card.Controls.Add(new Label { Text = "Nhân viên:", Location = new Point(lx, ly), AutoSize = true, ForeColor = UI.Muted });
            cboNV = new ComboBox { Location = new Point(lx, ly + 20), Size = new Size(200, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            card.Controls.Add(cboNV);

            card.Controls.Add(new Label { Text = "Khách hàng:", Location = new Point(lx + 220, ly), AutoSize = true, ForeColor = UI.Muted });
            cboKH = new ComboBox { Location = new Point(lx + 220, ly + 20), Size = new Size(240, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            card.Controls.Add(cboKH);

            card.Controls.Add(new Label { Text = "Số điện thoại KH:", Location = new Point(lx + 480, ly), AutoSize = true, ForeColor = UI.Muted });
            var lblPhone = new Label { Location = new Point(lx + 480, ly + 20), Size = new Size(150, 28), ForeColor = UI.Text, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleLeft };
            card.Controls.Add(lblPhone);
            cboKH.SelectedIndexChanged += (s, e) => {
                if (cboKH.SelectedValue != null)
                {
                    try
                    {
                        using var db = new AppDbContext();
                        var kh = db.KhachHang.Find((int)cboKH.SelectedValue);
                        lblPhone.Text = kh?.SoDienThoai ?? "";
                    }
                    catch { lblPhone.Text = ""; }
                }
            };

            ly += 66;
            card.Controls.Add(new Panel { Location = new Point(lx, ly), Size = new Size(840, 1), BackColor = UI.Border });
            ly += 10;

            card.Controls.Add(new Label { Text = "Hàng hóa:", Location = new Point(lx, ly), AutoSize = true, ForeColor = UI.Muted });
            cboHH = new ComboBox { Location = new Point(lx, ly + 20), Size = new Size(280, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            card.Controls.Add(cboHH);

            card.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(lx + 300, ly), AutoSize = true, ForeColor = UI.Muted });
            numSL = new NumericUpDown { Location = new Point(lx + 300, ly + 20), Size = new Size(90, 28), Minimum = 1, Maximum = 9999, Value = 1, Font = new Font("Segoe UI", 9.5f) };
            card.Controls.Add(numSL);

            card.Controls.Add(new Label { Text = "Giá bán:", Location = new Point(lx + 410, ly), AutoSize = true, ForeColor = UI.Muted });
            var lblGia = new Label { Location = new Point(lx + 410, ly + 20), Size = new Size(100, 28), ForeColor = UI.Text, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleRight };
            card.Controls.Add(lblGia);
            cboHH.SelectedIndexChanged += (s, e) => {
                if (cboHH.SelectedValue != null)
                {
                    try
                    {
                        dynamic item = cboHH.SelectedItem;
                        lblGia.Text = item.GiaBan.ToString("N0") + " đ";
                    }
                    catch { lblGia.Text = ""; }
                }
            };

            var btnAdd = new Guna.UI2.WinForms.Guna2Button { Text = "➕ Thêm vào giỏ", Location = new Point(lx + 530, ly + 16), Size = new Size(150, 34), FillColor = UI.Green, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 7 };
            btnAdd.Click += (s, e) => AddToCart();
            card.Controls.Add(btnAdd);

            ly += 66;
            card.Controls.Add(new Panel { Location = new Point(lx, ly), Size = new Size(840, 1), BackColor = UI.Border });
            ly += 6;

            card.Controls.Add(new Label { Text = "GIỎ HÀNG", Location = new Point(lx, ly), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = UI.Muted });
            ly += 22;

            dgvCart = UI.MakeDgv(card, lx, ly, 840, 260);
            dgvCart.Columns.Add("MaHH", "Mã HH");
            dgvCart.Columns.Add("TenHH", "Tên Hàng Hóa");
            dgvCart.Columns.Add("SL", "SL");
            dgvCart.Columns.Add("DonGia", "Đơn Giá");
            dgvCart.Columns.Add("ThanhTien", "Thành Tiền");
            dgvCart.Columns["MaHH"].Width = 70;
            dgvCart.Columns["SL"].Width = 50;
            dgvCart.Columns["DonGia"].Width = 120;
            dgvCart.Columns["ThanhTien"].Width = 130;
            dgvCart.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            dgvCart.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

            var btnDel = new Guna.UI2.WinForms.Guna2Button { Text = "🗑️ Xóa dòng", Location = new Point(lx, ly + 270), Size = new Size(120, 32), FillColor = UI.Red, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 7 };
            btnDel.Click += (s, e) => { if (dgvCart.CurrentRow != null) { _cart.RemoveAt(dgvCart.CurrentRow.Index); RefreshCart(); } };
            card.Controls.Add(btnDel);

            var btnClear = new Guna.UI2.WinForms.Guna2Button { Text = "🔄 Xóa hết", Location = new Point(lx + 130, ly + 270), Size = new Size(120, 32), FillColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 7 };
            btnClear.Click += (s, e) => { _cart.Clear(); RefreshCart(); };
            card.Controls.Add(btnClear);

            lblTong = new Label { Location = new Point(lx + 350, ly + 272), AutoSize = true, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = UI.Blue };
            card.Controls.Add(lblTong);

            var btnLuu = new Guna.UI2.WinForms.Guna2Button { Text = "💾 Lưu Hóa Đơn", Location = new Point(lx + 690, ly + 310), Size = new Size(150, 40), FillColor = UI.Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), BorderRadius = 8 };
            btnLuu.Click += (s, e) => SaveHoaDon();
            card.Controls.Add(btnLuu);

            LoadCombos();
        }

        private void LoadCombos()
        {
            using var db = new AppDbContext();
            cboNV.DataSource = db.NhanVien.Where(n => n.TrangThai).Select(n => new { n.MaNV, n.TenNV }).ToList();
            cboNV.DisplayMember = "TenNV";
            cboNV.ValueMember = "MaNV";

            var khList = db.KhachHang.Select(k => new { k.MaKH, k.TenKH }).ToList();
            cboKH.DataSource = khList;
            cboKH.DisplayMember = "TenKH";
            cboKH.ValueMember = "MaKH";

            var hhList = db.HangHoa.Where(h => h.SoLuongTon > 0).Select(h => new { h.MaHH, Display = h.TenHH + " [Tồn: " + h.SoLuongTon + "] - " + h.GiaBan.ToString("N0") + "đ", h.GiaBan }).ToList();
            cboHH.DataSource = hhList;
            cboHH.DisplayMember = "Display";
            cboHH.ValueMember = "MaHH";
        }

        private void AddToCart()
        {
            if (cboHH.SelectedValue == null) return;
            var maHH = cboHH.SelectedValue.ToString()!;
            using var db = new AppDbContext();
            var hh = db.HangHoa.Find(maHH);
            if (hh == null) return;
            int sl = (int)numSL.Value;
            var existing = _cart.FirstOrDefault(c => c.MaHH == maHH);
            int totalSL = sl + (existing?.SL ?? 0);
            if (totalSL > hh.SoLuongTon) { MessageBox.Show($"Tồn kho chỉ còn {hh.SoLuongTon} sản phẩm!", "Không đủ hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (existing != null) existing.SL += sl;
            else _cart.Add(new CartItem { MaHH = hh.MaHH, TenHH = hh.TenHH, SL = sl, DonGia = hh.GiaBan });
            RefreshCart();
        }

        private void RefreshCart()
        {
            dgvCart.Rows.Clear();
            foreach (var it in _cart)
                dgvCart.Rows.Add(it.MaHH, it.TenHH, it.SL, it.DonGia, it.ThanhTien);
            lblTong.Text = "Tổng: " + _cart.Sum(x => x.ThanhTien).ToString("N0") + " đ";
        }

        private void SaveHoaDon()
        {
            if (_cart.Count == 0) { MessageBox.Show("Giỏ hàng trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cboNV.SelectedValue == null) { MessageBox.Show("Chọn nhân viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                using var db = new AppDbContext();
                var hd = new HoaDon { NgayLap = DateTime.Now, MaNV = cboNV.SelectedValue.ToString(), MaKH = cboKH.SelectedValue != null ? (int?)cboKH.SelectedValue : null };
                db.HoaDon.Add(hd);
                db.SaveChanges();
                foreach (var it in _cart)
                {
                    db.ChiTietHoaDon.Add(new ChiTietHoaDon { MaHD = hd.MaHD, MaHH = it.MaHH, SoLuongBan = it.SL, DonGiaBan = it.DonGia });
                    var hh = db.HangHoa.Find(it.MaHH);
                    if (hh != null) hh.SoLuongTon -= it.SL;
                }
                db.SaveChanges();
                MessageBox.Show($"✅ Lưu hóa đơn #{hd.MaHD} thành công!\nTổng tiền: {_cart.Sum(x => x.ThanhTien):N0} đ", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _cart.Clear();
                RefreshCart();
                LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }

    public class FormDanhSachHoaDon : Form
    {
        private DataGridView dgvHD = new(), dgvCT = new();

        public FormDanhSachHoaDon()
        {
            BackColor = Color.FromArgb(248, 250, 252);
            Padding = new Padding(20);
            AutoScroll = true;
            AutoScrollMinSize = new Size(1000, 800);

            Controls.Add(new Label { Text = "Danh Sách Hóa Đơn", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = UI.Dark });

            var btnRefresh = new Guna.UI2.WinForms.Guna2Button { Text = "🔄 Tải lại", Location = new Point(720, 20), Size = new Size(100, 30), FillColor = UI.Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 6 };
            btnRefresh.Click += (s, e) => Load_();
            Controls.Add(btnRefresh);

            dgvHD = UI.MakeDgv(this, 20, 54, 880, 280);
            dgvHD.SelectionChanged += (s, e) => LoadChiTiet();

            Controls.Add(new Label { Text = "Chi tiết hóa đơn:", Location = new Point(20, 346), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = UI.Muted });
            dgvCT = UI.MakeDgv(this, 20, 370, 880, 280);

            Load_();
        }

        private void Load_()
        {
            try
            {
                using var db = new AppDbContext();
                var list = db.HoaDon.OrderByDescending(h => h.MaHD)
                    .Select(h => new
                    {
                        h.MaHD,
                        NgayLap = h.NgayLap.ToString("dd/MM/yyyy HH:mm"),
                        NhanVien = h.NhanVien != null ? h.NhanVien.TenNV : "",
                        KhachHang = h.KhachHang != null ? h.KhachHang.TenKH : "Khách vãng lai",
                        TongTien = h.ChiTiet.Sum(c => c.SoLuongBan * c.DonGiaBan)
                    }).ToList();
                dgvHD.DataSource = list;
                if (dgvHD.Columns.Count > 0)
                {
                    dgvHD.Columns["MaHD"].HeaderText = "Mã HD";
                    dgvHD.Columns["MaHD"].Width = 60;
                    dgvHD.Columns["NgayLap"].HeaderText = "Ngày Lập";
                    dgvHD.Columns["NgayLap"].Width = 140;
                    dgvHD.Columns["NhanVien"].HeaderText = "Nhân Viên";
                    dgvHD.Columns["KhachHang"].HeaderText = "Khách Hàng";
                    dgvHD.Columns["TongTien"].HeaderText = "Tổng Tiền";
                    dgvHD.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                    dgvHD.Columns["TongTien"].Width = 130;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void LoadChiTiet()
        {
            try
            {
                if (dgvHD.SelectedRows.Count == 0) return;
                int maHD = (int)dgvHD.SelectedRows[0].Cells["MaHD"].Value;
                using var db = new AppDbContext();
                var list = db.ChiTietHoaDon.Where(c => c.MaHD == maHD)
                    .Select(c => new { TenHH = c.HangHoa != null ? c.HangHoa.TenHH : "", c.SoLuongBan, c.DonGiaBan, ThanhTien = c.SoLuongBan * c.DonGiaBan }).ToList();
                dgvCT.DataSource = list;
                if (dgvCT.Columns.Count > 0)
                {
                    dgvCT.Columns["TenHH"].HeaderText = "Hàng Hóa";
                    dgvCT.Columns["SoLuongBan"].HeaderText = "SL";
                    dgvCT.Columns["SoLuongBan"].Width = 60;
                    dgvCT.Columns["DonGiaBan"].HeaderText = "Đơn Giá";
                    dgvCT.Columns["DonGiaBan"].DefaultCellStyle.Format = "N0";
                    dgvCT.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                    dgvCT.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}