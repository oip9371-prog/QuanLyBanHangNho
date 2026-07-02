using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHangNho
{
    public class FormMain : Form
    {
        private readonly Color C_TOPBAR_BG = Color.FromArgb(245, 245, 245);
        private readonly Color C_ICON_BG = Color.FromArgb(237, 237, 237);
        private readonly Color C_ICON_ACTIVE = Color.FromArgb(0, 173, 239);
        private readonly Color C_ICON_HOVER = Color.FromArgb(230, 230, 230);
        private readonly Color C_SUBMENU_BG = Color.White;
        private readonly Color C_SUBMENU_HOVER = Color.FromArgb(245, 245, 245);
        private readonly Color C_SUBMENU_ACTIVE = Color.FromArgb(230, 244, 255);
        private readonly Color C_SUBMENU_BORDER = Color.FromArgb(0, 173, 239);
        private readonly Color C_CONTENT_TOPBAR = Color.White;
        private readonly Color C_CONTENT_BG = Color.FromArgb(250, 250, 250);
        private readonly Color C_TEXT_PRIMARY = Color.FromArgb(30, 30, 30);
        private readonly Color C_TEXT_SECONDARY = Color.FromArgb(120, 120, 120);
        private readonly Color C_BORDER = Color.FromArgb(220, 220, 220);
        private readonly Color C_DIVIDER = Color.FromArgb(235, 235, 235);

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
            BackColor = C_CONTENT_BG;
            Font = new Font("Segoe UI", 9.5f);

            pnlIconBar.Dock = DockStyle.Top;
            pnlIconBar.Height = 60;
            pnlIconBar.BackColor = C_TOPBAR_BG;
            pnlIconBar.BorderStyle = BorderStyle.FixedSingle;
            pnlIconBar.AutoScroll = true;

            var pnlLogo = new Panel { Dock = DockStyle.Left, Width = 60, BackColor = C_TOPBAR_BG };
            pnlLogo.Controls.Add(new Label { Text = "🏪", Font = new Font("Segoe UI", 24), ForeColor = C_ICON_ACTIVE, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            pnlIconBar.Controls.Add(pnlLogo);

            var pnlDivider = new Panel { Dock = DockStyle.Left, Width = 1, BackColor = C_DIVIDER };
            pnlIconBar.Controls.Add(pnlDivider);

            Controls.Add(pnlIconBar);

            pnlSubMenu.Dock = DockStyle.Left;
            pnlSubMenu.Width = 220;
            pnlSubMenu.BackColor = C_SUBMENU_BG;
            pnlSubMenu.AutoScroll = true;
            pnlSubMenu.BorderStyle = BorderStyle.FixedSingle;

            var pnlSubHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = C_SUBMENU_BG };
            pnlSubHeader.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = C_DIVIDER });
            lblSubTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSubTitle.ForeColor = C_TEXT_PRIMARY;
            lblSubTitle.AutoSize = true;
            lblSubTitle.Location = new Point(16, 19);
            pnlSubHeader.Controls.Add(lblSubTitle);
            pnlSubMenu.Controls.Add(pnlSubHeader);

            Controls.Add(pnlSubMenu);

            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = C_CONTENT_BG;

            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Height = 60;
            pnlTopBar.BackColor = C_CONTENT_TOPBAR;
            pnlTopBar.BorderStyle = BorderStyle.FixedSingle;
            lblTopTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTopTitle.ForeColor = C_TEXT_PRIMARY;
            lblTopTitle.AutoSize = true;
            lblTopTitle.Location = new Point(20, 18);
            pnlTopBar.Controls.Add(lblTopTitle);

            pnlContentHost.Dock = DockStyle.Fill;
            pnlContentHost.Padding = new Padding(16);
            pnlContentHost.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlContentHost);
            pnlContent.Controls.Add(pnlTopBar);
            Controls.Add(pnlContent);

            AddIconBtn("📊", "hanghoa");
            AddIconBtn("👤", "banhang");
            AddIconBtn("📤", "nhaphang");
            AddIconBtn("👥", "khachhang");
            AddIconBtn("👨", "nhanvien");
            AddIconBtn("📈", "baocao");
        }

        private void AddIconBtn(string icon, string key)
        {
            var pnl = new Panel
            {
                Size = new Size(70, 60),
                BackColor = C_ICON_BG,
                Cursor = Cursors.Hand,
                Tag = key,
                Dock = DockStyle.Left,
                Margin = new Padding(0)
            };

            var lbl = new Label 
            { 
                Text = icon, 
                Font = new Font("Segoe UI", 22), 
                ForeColor = C_TEXT_PRIMARY, 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter, 
                Tag = key,
                Cursor = Cursors.Hand
            };

            Action<bool> hov = on => 
            { 
                if ((string)pnl.Tag != _active) 
                    pnl.BackColor = on ? C_ICON_HOVER : C_ICON_BG;
            };

            pnl.MouseEnter += (s, e) => hov(true);
            pnl.MouseLeave += (s, e) => hov(false);
            lbl.MouseEnter += (s, e) => hov(true);
            lbl.MouseLeave += (s, e) => hov(false);

            pnl.Click += (s, e) => Activate_(key);
            lbl.Click += (s, e) => Activate_(key);

            pnl.Controls.Add(lbl);
            pnlIconBar.Controls.Add(pnl);
        }

        private void SetSubItems(string title, (string icon, string lbl, Action act)[] items)
        {
            lblSubTitle.Text = title;
            for (int i = pnlSubMenu.Controls.Count - 1; i >= 0; i--)
                if (pnlSubMenu.Controls[i].Tag?.ToString() == "sub")
                    pnlSubMenu.Controls.RemoveAt(i);

            int y = 60;
            foreach (var item in items)
            {
                var act = item.act;
                var pi = new Panel 
                { 
                    Location = new Point(0, y), 
                    Size = new Size(220, 40), 
                    BackColor = Color.Transparent, 
                    Cursor = Cursors.Hand, 
                    Tag = "sub" 
                };

                var bar = new Panel 
                { 
                    Dock = DockStyle.Left, 
                    Width = 3, 
                    BackColor = C_SUBMENU_BORDER, 
                    Visible = false 
                };

                var lbl = new Label 
                { 
                    Text = $"  {item.icon}  {item.lbl}", 
                    Font = new Font("Segoe UI", 9.5f), 
                    ForeColor = C_TEXT_SECONDARY, 
                    Dock = DockStyle.Fill, 
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };

                pi.Controls.Add(lbl);
                pi.Controls.Add(bar);

                Action<bool> hov = on => 
                { 
                    pi.BackColor = on ? C_SUBMENU_HOVER : Color.Transparent; 
                    lbl.ForeColor = on ? C_ICON_ACTIVE : C_TEXT_SECONDARY; 
                    bar.Visible = on;
                };

                pi.MouseEnter += (s, e) => hov(true);
                pi.MouseLeave += (s, e) => hov(false);
                lbl.MouseEnter += (s, e) => hov(true);
                lbl.MouseLeave += (s, e) => hov(false);
                pi.Click += (s, e) => act();
                lbl.Click += (s, e) => act();

                pnlSubMenu.Controls.Add(pi);
                y += 40;
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
            {
                if (c is Panel p && p.Tag is string k)
                {
                    p.BackColor = k == key ? C_SUBMENU_ACTIVE : C_ICON_BG;
                    foreach (Control lc in p.Controls)
                    {
                        if (lc is Label l)
                            l.ForeColor = k == key ? C_ICON_ACTIVE : C_TEXT_PRIMARY;
                    }
                }
            }

            switch (key)
            {
                case "hanghoa":
                    lblTopTitle.Text = "Quản Lý Hàng Hóa";
                    SetSubItems("Hàng Hóa", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.HangHoa.FormHangHoa()))),
                    });
                    ShowChild(new Forms.HangHoa.FormHangHoa());
                    break;

                case "banhang":
                    lblTopTitle.Text = "Bán Hàng";
                    SetSubItems("Bán Hàng", new[] {
                        ("➕", "Tạo hóa đơn mới", (Action)(() => ShowChild(new Forms.BanHang.FormBanHang()))),
                        ("📋", "Danh sách hóa đơn", (Action)(() => ShowChild(new Forms.BanHang.FormDanhSachHoaDon()))),
                    });
                    ShowChild(new Forms.BanHang.FormBanHang());
                    break;

                case "nhaphang":
                    lblTopTitle.Text = "Nhập Hàng";
                    SetSubItems("Nhập Hàng", new[] {
                        ("➕", "Tạo phiếu nhập mới", (Action)(() => ShowChild(new Forms.NhapHang.FormNhapHang()))),
                        ("📋", "Danh sách phiếu nhập", (Action)(() => ShowChild(new Forms.NhapHang.FormDanhSachPhieuNhap()))),
                    });
                    ShowChild(new Forms.NhapHang.FormNhapHang());
                    break;

                case "khachhang":
                    lblTopTitle.Text = "Khách Hàng";
                    SetSubItems("Khách Hàng", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.KhachHang.FormKhachHang()))),
                    });
                    ShowChild(new Forms.KhachHang.FormKhachHang());
                    break;

                case "nhanvien":
                    lblTopTitle.Text = "Nhân Viên";
                    SetSubItems("Nhân Viên", new[] {
                        ("📋", "Danh sách", (Action)(() => ShowChild(new Forms.NhanVien.FormNhanVien()))),
                    });
                    ShowChild(new Forms.NhanVien.FormNhanVien());
                    break;

                case "baocao":
                    lblTopTitle.Text = "Báo Cáo Doanh Thu";
                    SetSubItems("Báo Cáo", new[] {
                        ("📊", "Doanh thu & Lợi nhuận", (Action)(() => ShowChild(new Forms.BaoCao.FormBaoCao()))),
                    });
                    ShowChild(new Forms.BaoCao.FormBaoCao());
                    break;
            }
        }
    }
}
