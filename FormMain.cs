using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHangNho
{
    public class FormMain : Form
    {
        private readonly Color C_DARK = Color.FromArgb(15, 23, 42);
        private readonly Color C_BLUE = Color.FromArgb(37, 99, 235);
        private readonly Color C_HOVER = Color.FromArgb(30, 58, 138);
        private readonly Color C_BORDER = Color.FromArgb(226, 232, 240);
        private readonly Color C_BG_LIGHT = Color.FromArgb(248, 250, 252);
        private readonly Color C_BG_CONTENT = Color.FromArgb(245, 247, 250);
        private readonly Color C_TEXT_PRIMARY = Color.FromArgb(15, 23, 42);
        private readonly Color C_TEXT_SECONDARY = Color.FromArgb(51, 65, 85);
        private readonly Color C_HOVER_BG = Color.FromArgb(239, 246, 255);
        private readonly Color C_DIVIDER = Color.FromArgb(241, 245, 249);

        private readonly Panel pnlIconBar = new();
        private readonly Panel pnlSubMenu = new();
        private readonly Panel pnlContent = new();
        private readonly Panel pnlTopBar = new();
        private readonly Panel pnlContentHost = new();
        private readonly Label lblTopTitle = new();
        private readonly Label lblSubTitle = new();
        private string _active = "";

        public FormMain()
        {
            BuildLayout();
            Activate_("hanghoa");
        }

        private void BuildLayout()
        {
            Text = "Quản Lý Bán Hàng Nhỏ";
            ClientSize = new Size(1480, 900);
            MinimumSize = new Size(1280, 760);
            WindowState = FormWindowState.Maximized;
            BackColor = C_BG_LIGHT;
            Font = new Font("Segoe UI", 10f);

            pnlIconBar.Dock = DockStyle.Left;
            pnlIconBar.Width = 88;
            pnlIconBar.BackColor = C_DARK;

            var pnlLogo = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = C_BLUE };
            pnlLogo.Controls.Add(new Label { Text = "🏪", Font = new Font("Segoe UI", 22), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            pnlIconBar.Controls.Add(pnlLogo);

            pnlSubMenu.Dock = DockStyle.Left;
            pnlSubMenu.Width = 240;
            pnlSubMenu.BackColor = Color.White;
            pnlSubMenu.AutoScroll = true;
            pnlSubMenu.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 1, BackColor = C_BORDER });
            Controls.Add(pnlIconBar);
            Controls.Add(pnlSubMenu);

            var pnlSubHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            pnlSubHeader.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = C_BORDER });
            lblSubTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblSubTitle.ForeColor = C_TEXT_PRIMARY;
            lblSubTitle.AutoSize = true;
            lblSubTitle.Location = new Point(16, 18);
            pnlSubHeader.Controls.Add(lblSubTitle);
            pnlSubMenu.Controls.Add(pnlSubHeader);

            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = C_BG_CONTENT;

            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Height = 60;
            pnlTopBar.BackColor = Color.White;
            pnlTopBar.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = C_BORDER });
            lblTopTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTopTitle.ForeColor = C_TEXT_PRIMARY;
            lblTopTitle.AutoSize = true;
            lblTopTitle.Location = new Point(20, 17);
            pnlTopBar.Controls.Add(lblTopTitle);

            pnlContentHost.Dock = DockStyle.Fill;
            pnlContentHost.Padding = new Padding(16);
            pnlContentHost.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlContentHost);
            pnlContent.Controls.Add(pnlTopBar);
            Controls.Add(pnlContent);

            AddIconBtn("📦", "Hàng\nHóa", "hanghoa");
            AddIconBtn("🧾", "Bán\nHàng", "banhang");
            AddIconBtn("📥", "Nhập\nHàng", "nhaphang");
            AddIconBtn("👥", "Khách\nHàng", "khachhang");
            AddIconBtn("👷", "Nhân\nViên", "nhanvien");
            AddIconBtn("📊", "Báo\nCáo", "baocao");
        }

        private void AddIconBtn(string icon, string label, string key)
        {
            var pnl = new Panel
            {
                Size = new Size(88, 78),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = key,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 6)
            };
            var li = new Label { Text = icon, Font = new Font("Segoe UI", 20), ForeColor = Color.White, Size = new Size(88, 38), Location = new Point(0, 8), TextAlign = ContentAlignment.MiddleCenter, Tag = key };
            var lt = new Label { Text = label, Font = new Font("Segoe UI", 7), ForeColor = Color.FromArgb(148, 163, 184), Size = new Size(88, 24), Location = new Point(0, 44), TextAlign = ContentAlignment.TopCenter, Tag = key };

            Action<bool> hov = on => { if ((string)pnl.Tag != _active) pnl.BackColor = on ? C_HOVER : Color.Transparent; };
            pnl.MouseEnter += (s, e) => hov(true); 
            pnl.MouseLeave += (s, e) => hov(false);
            li.MouseEnter += (s, e) => hov(true); 
            li.MouseLeave += (s, e) => hov(false);
            lt.MouseEnter += (s, e) => hov(true); 
            lt.MouseLeave += (s, e) => hov(false);

            pnl.Click += (s, e) => Activate_(key);
            li.Click += (s, e) => Activate_(key);
            lt.Click += (s, e) => Activate_(key);

            pnl.Controls.Add(li); 
            pnl.Controls.Add(lt);
            pnlIconBar.Controls.Add(pnl);
        }

        private void SetSubItems(string title, (string icon, string lbl, Action act)[] items)
        {
            lblSubTitle.Text = title;
            for (int i = pnlSubMenu.Controls.Count - 1; i >= 0; i--)
                if (pnlSubMenu.Controls[i].Tag?.ToString() == "sub")
                    pnlSubMenu.Controls.RemoveAt(i);

            int y = 54;
            foreach (var item in items)
            {
                var act = item.act;
                var pi = new Panel { Location = new Point(0, y), Size = new Size(240, 46), BackColor = Color.Transparent, Cursor = Cursors.Hand, Tag = "sub" };
                var bar = new Panel { Size = new Size(3, 46), BackColor = C_BLUE, Visible = false };
                var lbl = new Label { Text = $"   {item.icon}  {item.lbl}", Font = new Font("Segoe UI", 9.5f), ForeColor = C_TEXT_SECONDARY, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                pi.Controls.Add(lbl); 
                pi.Controls.Add(bar);
                pi.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = C_DIVIDER });

                Action<bool> hov = on => { pi.BackColor = on ? C_HOVER_BG : Color.Transparent; lbl.ForeColor = on ? C_BLUE : C_TEXT_SECONDARY; bar.Visible = on; };
                pi.MouseEnter += (s, e) => hov(true); 
                pi.MouseLeave += (s, e) => hov(false);
                lbl.MouseEnter += (s, e) => hov(true); 
                lbl.MouseLeave += (s, e) => hov(false);
                pi.Click += (s, e) => act(); 
                lbl.Click += (s, e) => act();
                pnlSubMenu.Controls.Add(pi); 
                y += 46;
            }
        }

        private void ShowChild(Form child)
        {
            pnlContentHost.Controls.Clear();
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            child.AutoScroll = true;
            child.AutoScrollMinSize = new Size(1100, 760);
            child.Padding = new Padding(0);
            pnlContentHost.Controls.Add(child);
            child.BringToFront();
            child.Show();
        }

        private void Activate_(string key)
        {
            _active = key;
            foreach (Control c in pnlIconBar.Controls)
                if (c is Panel p && p.Tag is string k) 
                    p.BackColor = k == key ? C_BLUE : Color.Transparent;

            switch (key)
            {
                case "hanghoa":
                    lblTopTitle.Text = "📦  Quản Lý Hàng Hóa";
                    SetSubItems("Hàng Hóa", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.HangHoa.FormHangHoa()))),
                    });
                    ShowChild(new Forms.HangHoa.FormHangHoa()); 
                    break;

                case "banhang":
                    lblTopTitle.Text = "🧾  Bán Hàng";
                    SetSubItems("Bán Hàng", new[] {
                        ("➕", "Tạo hóa đơn mới", (Action)(() => ShowChild(new Forms.BanHang.FormBanHang()))),
                        ("📋", "Danh sách hóa đơn", (Action)(() => ShowChild(new Forms.BanHang.FormDanhSachHoaDon()))),
                    });
                    ShowChild(new Forms.BanHang.FormBanHang()); 
                    break;

                case "nhaphang":
                    lblTopTitle.Text = "📥  Nhập Hàng";
                    SetSubItems("Nhập Hàng", new[] {
                        ("➕", "Tạo phiếu nhập mới", (Action)(() => ShowChild(new Forms.NhapHang.FormNhapHang()))),
                        ("📋", "Danh sách phiếu nhập", (Action)(() => ShowChild(new Forms.NhapHang.FormDanhSachPhieuNhap()))),
                    });
                    ShowChild(new Forms.NhapHang.FormNhapHang()); 
                    break;

                case "khachhang":
                    lblTopTitle.Text = "👥  Khách Hàng";
                    SetSubItems("Khách Hàng", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.KhachHang.FormKhachHang()))),
                    });
                    ShowChild(new Forms.KhachHang.FormKhachHang()); 
                    break;

                case "nhanvien":
                    lblTopTitle.Text = "👷  Nhân Viên";
                    SetSubItems("Nhân Viên", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.NhanVien.FormNhanVien()))),
                    });
                    ShowChild(new Forms.NhanVien.FormNhanVien()); 
                    break;

                case "baocao":
                    lblTopTitle.Text = "📊  Báo Cáo Doanh Thu";
                    SetSubItems("Báo Cáo", new[] {
                        ("📊", "Doanh thu & Lợi nhuận", (Action)(() => ShowChild(new Forms.BaoCao.FormBaoCao()))),
                    });
                    ShowChild(new Forms.BaoCao.FormBaoCao()); 
                    break;
            }
        }
    }
}
