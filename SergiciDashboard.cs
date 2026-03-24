using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class SergiciDashboard : Form
    {
        private readonly int _sergiciId;
        private string _sergiciAdi;

        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public SergiciDashboard(int sergiciId, string sergiciAdi)
        {
            InitializeComponent();

            _sergiciId = sergiciId;
            _sergiciAdi = (sergiciAdi ?? "").Trim();

            this.Load -= SergiciDashboard_Load;
            this.Load += SergiciDashboard_Load;
        }

        public SergiciDashboard() : this(0, "") { }

        private void NavigateTo(Form next)
        {
            next.FormClosed += (s, e) => this.Close();
            next.Show();
            this.Hide();
        }

        private void SergiciDashboard_Load(object sender, EventArgs e)
        {
            if (_sergiciId <= 0)
            {
                MessageBox.Show("Dashboard açılırken SergiciID geçersiz geldi.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(_sergiciAdi))
                _sergiciAdi = SergiciAdiniDbdenGetir();

            if (labelSergiciAdi != null)
                labelSergiciAdi.Text = string.IsNullOrWhiteSpace(_sergiciAdi) ? "—" : _sergiciAdi;

            DashboardIstatistikleriniYukle();
            ChartVerileriniYukle();
            SergilerimiYukle();          // ✅ gerçek COUNT eser sayısı
            EnsureEserleriGorButton();   // ✅ "Eserleri Gör" butonu
            HookGridEvents();            // ✅ click event
            LabelStilleriniGuncelle();
        }

        private void HookGridEvents()
        {
            if (dataGridViewSergiciler == null) return;

            dataGridViewSergiciler.CellClick -= dataGridViewSergiciler_CellClick;
            dataGridViewSergiciler.CellClick += dataGridViewSergiciler_CellClick;

            dataGridViewSergiciler.CellDoubleClick -= dataGridViewSergiciler_CellDoubleClick;
            dataGridViewSergiciler.CellDoubleClick += dataGridViewSergiciler_CellDoubleClick;
        }

        private string SergiciAdiniDbdenGetir()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT AdSoyad FROM sergiciler WHERE SanatciID=@Id LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _sergiciId);
                        return (cmd.ExecuteScalar()?.ToString() ?? "").Trim();
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private void DashboardIstatistikleriniYukle()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string ad = _sergiciAdi ?? "";

                    using (var cmd = new MySqlCommand(
                        @"SELECT COUNT(*) FROM sergiler WHERE (@Ad = '' OR Sergici = @Ad);", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelToplamSergi.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM sergiler
WHERE (@Ad = '' OR Sergici = @Ad)
  AND Durum = 'Aktif'
  AND BaslangicTarihi <= CURDATE()
  AND BitisTarihi   >= CURDATE();", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelSergilenen.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM satislar s
INNER JOIN eserler e   ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
  AND s.SatisDurumu = 'Satıldı';", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelSatilan.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM satislar s
INNER JOIN eserler e   ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
  AND s.SatisDurumu = 'Beklemede';", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelBekleyen.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChartVerileriniYukle()
        {
            try
            {
                string ad = _sergiciAdi ?? "";

                DataTable dtKazanc = new DataTable();
                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    DATE_FORMAT(s.SatisTarihi, '%Y-%m') AS Ay,
    SUM(s.SatisFiyati)                  AS ToplamKazanc
FROM satislar s
INNER JOIN eserler e   ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
GROUP BY Ay
ORDER BY Ay;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dtKazanc);
                }

                chartKazanc.Series[0].Points.Clear();
                chartKazanc.DataSource = dtKazanc;
                chartKazanc.Series[0].XValueMember = "Ay";
                chartKazanc.Series[0].YValueMembers = "ToplamKazanc";
                chartKazanc.ChartAreas[0].AxisY.Title = "TL";
                chartKazanc.DataBind();

                DataTable dtSergi = new DataTable();
                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    DATE_FORMAT(se.BaslangicTarihi, '%Y-%m') AS Ay,
    COUNT(*)                                 AS SergiSayisi
FROM sergiler se
WHERE (@Ad = '' OR se.Sergici = @Ad)
GROUP BY Ay
ORDER BY Ay;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dtSergi);
                }

                chartSergiler.Series[0].Points.Clear();
                chartSergiler.DataSource = dtSergi;
                chartSergiler.Series[0].XValueMember = "Ay";
                chartSergiler.Series[0].YValueMembers = "SergiSayisi";
                chartSergiler.ChartAreas[0].AxisY.Title = "Adet";
                chartSergiler.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chart verileri yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Eser sayısını artık COUNT ile getiriyoruz, se.EserSayisi kullanılmıyor
        private void SergilerimiYukle()
        {
            try
            {
                string ad = _sergiciAdi ?? "";
                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    se.SergiID         AS 'SergiID',
    se.SergiAdi        AS 'Sergi Adı',
    se.BaslangicTarihi AS 'Başlangıç Tarihi',
    se.BitisTarihi     AS 'Bitiş Tarihi',

    COUNT(e.EserID)    AS 'Eser Sayısı',

    se.Durum           AS 'Durum',
    COALESCE(se.ToplamMaliyet, 0) AS 'Sergi Ücreti'
FROM sergiler se
LEFT JOIN eserler e ON e.SergiID = se.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
GROUP BY se.SergiID, se.SergiAdi, se.BaslangicTarihi, se.BitisTarihi, se.Durum, se.ToplamMaliyet
ORDER BY se.BaslangicTarihi DESC;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dt);
                }

                dataGridViewSergiciler.ReadOnly = true;
                dataGridViewSergiciler.MultiSelect = false;
                dataGridViewSergiciler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewSergiciler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewSergiciler.AllowUserToAddRows = false;
                dataGridViewSergiciler.AllowUserToDeleteRows = false;

                dataGridViewSergiciler.DataSource = dt;

                // SergiID'yi gizlemek istemiyorsan sil
                if (dataGridViewSergiciler.Columns["SergiID"] != null)
                    dataGridViewSergiciler.Columns["SergiID"].HeaderText = "Sergi ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergi listesi yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureEserleriGorButton()
        {
            if (dataGridViewSergiciler == null) return;
            if (dataGridViewSergiciler.Columns["EserleriGor"] != null) return;

            dataGridViewSergiciler.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "EserleriGor",
                HeaderText = "Eserler",
                Text = "Eserleri Gör",
                UseColumnTextForButtonValue = true,
                Width = 95
            });

            dataGridViewSergiciler.Columns["EserleriGor"].DisplayIndex = dataGridViewSergiciler.Columns.Count - 1;
        }

        private int? GetSelectedSergiId(int rowIndex)
        {
            if (rowIndex < 0) return null;
            if (dataGridViewSergiciler?.Rows == null) return null;

            if (dataGridViewSergiciler.Rows[rowIndex].Cells["SergiID"]?.Value == null) return null;
            object v = dataGridViewSergiciler.Rows[rowIndex].Cells["SergiID"].Value;
            if (v == DBNull.Value) return null;

            return Convert.ToInt32(v);
        }

        private void dataGridViewSergiciler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridViewSergiciler.Columns["EserleriGor"] != null &&
                e.ColumnIndex == dataGridViewSergiciler.Columns["EserleriGor"].Index)
            {
                int? sergiId = GetSelectedSergiId(e.RowIndex);
                if (sergiId == null)
                {
                    MessageBox.Show("SergiID alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi, sergiId.Value));
            }
        }

        private void dataGridViewSergiciler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Çift tıkla da eserleri aç
            if (e.RowIndex < 0) return;

            int? sergiId = GetSelectedSergiId(e.RowIndex);
            if (sergiId == null) return;

            NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi, sergiId.Value));
        }

        private void LabelStilleriniGuncelle()
        {
            Color[] labelColors = { Color.SteelBlue, Color.DarkOrange, Color.SeaGreen, Color.Tomato };
            Label[] statisticLabels = { labelToplamSergi, labelSergilenen, labelSatilan, labelBekleyen };

            for (int i = 0; i < statisticLabels.Length; i++)
            {
                if (statisticLabels[i] == null) continue;

                statisticLabels[i].BackColor = labelColors[i];
                statisticLabels[i].ForeColor = Color.White;
                statisticLabels[i].Font = new Font("Arial", 16, FontStyle.Bold);
                statisticLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                statisticLabels[i].BorderStyle = BorderStyle.FixedSingle;
            }
        }

        // ===== MENÜ / FOOTER =====

        private void dashboardButton_Click(object sender, EventArgs e)
        {
            DashboardIstatistikleriniYukle();
            ChartVerileriniYukle();
            SergilerimiYukle();
            EnsureEserleriGorButton();
        }

        private void eserlerimButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi, null));
        }

        private void raporButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciRaporlar(_sergiciId, _sergiciAdi));
        }

        private void cikisYapButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NavigateTo(new Form1());
            }
        }

        private void hakkımızdaButton_Click(object sender, EventArgs e)
        {
            var frm = new Hakkimizda(Hakkimizda.HomeRole.Sergici, _sergiciId);
            frm.FormClosed += (s, args) => this.Show();

            this.Hide();
            frm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            var frm = new Yardim(Yardim.HomeRole.Sergici, _sergiciId);
            frm.FormClosed += (s, args) => this.Show();

            this.Hide();
            frm.Show();
        }


        // IMPORTANT: Dashboard.Designer'da labelSergiciAdi.Click -> label12_Click bağlı
        private void label12_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciProfilim(_sergiciId, _sergiciAdi));
        }
    }
}
