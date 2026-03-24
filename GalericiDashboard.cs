using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiDashboard : Form
    {
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private DataTable sergiciData;

        public GalericiDashboard(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            InitializeSergiciData();

            this.Load -= GalericiDashboard_Load;
            this.Load += GalericiDashboard_Load;
        }

        private void GalericiDashboard_Load(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            // Form kapanırken patlamasın
            if (IsDisposed || !IsHandleCreated) return;

            UpdateSergiciDurumlari();
            LoadSergiciData();
            SetupCharts();
            UpdateStatistics();
        }

        // =========================================================
        // ✅ DOĞRU NAV: ShowDialog zincirini BOZMAZ
        // =========================================================
        private void NavigateDialog(Form nextForm)
        {
            if (nextForm == null) return;

            this.Hide();
            try
            {
                nextForm.ShowDialog();
            }
            finally
            {
                // Dialog kapanınca geri dön
                if (!this.IsDisposed)
                {
                    this.Show();
                    this.BringToFront();
                    RefreshAll();
                }
            }
        }

        // Login gibi “ana akış” değişiyorsa bunu kullan
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;

            nextForm.Show();
            this.Close();
        }

        // ==============================
        // GRID DATATABLE
        // ==============================
        private void InitializeSergiciData()
        {
            sergiciData = new DataTable();
            sergiciData.Columns.Add("SanatciID", typeof(int));
            sergiciData.Columns.Add("Sergici", typeof(string));
            sergiciData.Columns.Add("Eposta", typeof(string));
            sergiciData.Columns.Add("Ucret", typeof(decimal));
            sergiciData.Columns.Add("Durum", typeof(string));
            sergiciData.Columns.Add("AktifSergi", typeof(int));
        }

        // ==============================
        // SERGİCİ DURUMU (Aktif/Pasif)
        // ==============================
        private void UpdateSergiciDurumlari()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
UPDATE sergiciler s
SET s.Durum =
    CASE
        WHEN EXISTS (
            SELECT 1
            FROM sergiler e
            WHERE e.GalericiID = s.GalericiID
              AND e.SanatciID  = s.SanatciID
              AND CURDATE() BETWEEN DATE(e.BaslangicTarihi) AND DATE(e.BitisTarihi)
        ) THEN 'Aktif'
        ELSE 'Pasif'
    END
WHERE s.GalericiID = @gid;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergici durum güncelleme hatası:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==============================
        // GRID DOLDUR
        // ==============================
        private void LoadSergiciData()
        {
            if (sergiciData == null) InitializeSergiciData();
            sergiciData.Rows.Clear();

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    s.SanatciID,
    s.AdSoyad,
    s.Eposta,
    COALESCE(s.AylikUcret,0) AS Ucret,
    COALESCE(s.Durum,'Pasif') AS Durum,
    COALESCE((
        SELECT COUNT(*)
        FROM sergiler e
        WHERE e.GalericiID = s.GalericiID
          AND e.SanatciID  = s.SanatciID
          AND CURDATE() BETWEEN DATE(e.BaslangicTarihi) AND DATE(e.BitisTarihi)
    ),0) AS AktifSergi
FROM sergiciler s
WHERE s.GalericiID = @gid
ORDER BY s.SanatciID DESC;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                int id = Convert.ToInt32(dr["SanatciID"]);
                                string ad = dr["AdSoyad"]?.ToString() ?? "";
                                string ep = dr["Eposta"]?.ToString() ?? "";
                                decimal ucret = dr["Ucret"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Ucret"]);
                                string durum = dr["Durum"]?.ToString() ?? "Pasif";
                                int aktifSergi = dr["AktifSergi"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AktifSergi"]);

                                sergiciData.Rows.Add(id, ad, ep, ucret, durum, aktifSergi);
                            }
                        }
                    }
                }

                if (dataGridView1 == null) return;

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = sergiciData;

                if (dataGridView1.Columns["SanatciID"] != null) dataGridView1.Columns["SanatciID"].HeaderText = "ID";
                if (dataGridView1.Columns["Sergici"] != null) dataGridView1.Columns["Sergici"].HeaderText = "Sergici";
                if (dataGridView1.Columns["Eposta"] != null) dataGridView1.Columns["Eposta"].HeaderText = "E-posta";

                if (dataGridView1.Columns["Ucret"] != null)
                {
                    dataGridView1.Columns["Ucret"].HeaderText = "Aylık Ücret";
                    dataGridView1.Columns["Ucret"].DefaultCellStyle.Format = "0.00";
                }

                if (dataGridView1.Columns["Durum"] != null) dataGridView1.Columns["Durum"].HeaderText = "Durum";
                if (dataGridView1.Columns["AktifSergi"] != null) dataGridView1.Columns["AktifSergi"].HeaderText = "Aktif Sergi";

                EnsureGridButtons();
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergiciler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureGridButtons()
        {
            if (dataGridView1 == null) return;

            if (dataGridView1.Columns["Duzenle"] == null)
            {
                dataGridView1.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Duzenle",
                    HeaderText = "Düzenle",
                    Text = "Düzenle",
                    UseColumnTextForButtonValue = true
                });
            }

            if (dataGridView1.Columns["Sil"] == null)
            {
                dataGridView1.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Sil",
                    HeaderText = "Sil",
                    Text = "Sil",
                    UseColumnTextForButtonValue = true
                });
            }

            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // ID güvenli çek
            object idObj = dataGridView1.Rows[e.RowIndex].Cells["SanatciID"]?.Value;
            if (idObj == null || idObj == DBNull.Value) return;

            int sanatciId = Convert.ToInt32(idObj);

            // DÜZENLE
            if (dataGridView1.Columns["Duzenle"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Duzenle"].Index)
            {
                NavigateDialog(new GalericiSergiciDuzenleme(_galericiId, sanatciId));
                return;
            }

            // SİL
            if (dataGridView1.Columns["Sil"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Sil"].Index)
            {
                string ad = dataGridView1.Rows[e.RowIndex].Cells["Sergici"]?.Value?.ToString() ?? "";

                var confirm = MessageBox.Show(
                    $"'{ad}' adlı sergiciyi silmek istediğinizden emin misiniz?\nBu sergiciye ait sergiler de silinecek.",
                    "Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                if (DeleteSergiciCascade(sanatciId))
                    RefreshAll();
            }
        }

        private bool DeleteSergiciCascade(int sanatciId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var cmd = new MySqlCommand(
                            "DELETE FROM sergiler WHERE GalericiID=@gid AND SanatciID=@sid;",
                            conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            cmd.Parameters.AddWithValue("@sid", sanatciId);
                            cmd.ExecuteNonQuery();
                        }

                        int affected;
                        using (var cmd = new MySqlCommand(
                            "DELETE FROM sergiciler WHERE GalericiID=@gid AND SanatciID=@sid;",
                            conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            cmd.Parameters.AddWithValue("@sid", sanatciId);
                            affected = cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme işlemi sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ===============================
        // CHARTS
        // ===============================
        private void SetupCharts()
        {
            SetupKazancChart();
            SetupSergiChart();
        }

        private void SetupKazancChart()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    DATE_FORMAT(SatisTarihi, '%Y-%m') AS Ay,
    COALESCE(SUM(SatisFiyati),0) AS ToplamKazanc
FROM satislar
WHERE GalericiID = @gid
GROUP BY DATE_FORMAT(SatisTarihi, '%Y-%m')
ORDER BY Ay;";

                    if (chartKazanc == null) return;

                    chartKazanc.Series.Clear();
                    chartKazanc.Titles.Clear();

                    var s = new Series("Toplam Kazanç")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.SteelBlue,
                        IsValueShownAsLabel = true
                    };

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            bool any = false;
                            while (dr.Read())
                            {
                                any = true;
                                string ay = dr["Ay"]?.ToString() ?? "";
                                decimal kazanc = dr["ToplamKazanc"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["ToplamKazanc"]);
                                s.Points.AddXY(ay, kazanc);
                            }

                            if (!any)
                                s.Points.AddXY(DateTime.Now.ToString("yyyy-MM"), 0);
                        }
                    }

                    chartKazanc.Series.Add(s);

                    if (chartKazanc.ChartAreas.Count > 0)
                    {
                        chartKazanc.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chartKazanc.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    }

                    chartKazanc.Titles.Add("Aylık Toplam Kazanç");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kazanç grafiği yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupSergiChart()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    COALESCE(Durum,'(Boş)') AS Durum,
    COUNT(*) AS Adet
FROM sergiler
WHERE GalericiID = @gid
GROUP BY COALESCE(Durum,'(Boş)');";

                    if (chartSergi == null) return;

                    chartSergi.Series.Clear();
                    chartSergi.Titles.Clear();

                    var s = new Series("Sergi Sayısı")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true
                    };

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            bool any = false;
                            while (dr.Read())
                            {
                                any = true;
                                string durum = dr["Durum"]?.ToString() ?? "";
                                int adet = dr["Adet"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Adet"]);
                                s.Points.AddXY(durum, adet);
                            }

                            if (!any)
                            {
                                s.Points.AddXY("Aktif", 0);
                                s.Points.AddXY("Geçmiş", 0);
                                s.Points.AddXY("Bekleyen", 0);
                            }
                        }
                    }

                    chartSergi.Series.Add(s);

                    if (chartSergi.ChartAreas.Count > 0)
                    {
                        chartSergi.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chartSergi.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    }

                    chartSergi.Titles.Add("Sergi Durumlarına Göre Sayı");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergi grafiği yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===============================
        // İSTATİSTİK
        // ===============================
        private void UpdateStatistics()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    (SELECT COUNT(*) FROM sergiler WHERE GalericiID = @gid) AS ToplamSergi,
    (SELECT COUNT(*) FROM sergiler WHERE GalericiID = @gid AND Durum = 'Aktif') AS Aktif,
    (SELECT COUNT(*) FROM sergiler WHERE GalericiID = @gid AND Durum = 'Geçmiş') AS Gecmis,
    (SELECT COUNT(*) FROM sergiler WHERE GalericiID = @gid AND Durum = 'Bekleyen') AS Bekleyen,
    (SELECT COALESCE(SUM(SatisFiyati),0) FROM satislar WHERE GalericiID = @gid) AS ToplamKazanc;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (lblToplamSergi != null) lblToplamSergi.Text = dr["ToplamSergi"].ToString();
                                if (lblSergilenen != null) lblSergilenen.Text = dr["Aktif"].ToString();
                                if (lblSatilan != null) lblSatilan.Text = dr["Gecmis"].ToString();
                                if (lblBekleyen != null) lblBekleyen.Text = dr["Bekleyen"].ToString();

                                decimal toplamKazanc = dr["ToplamKazanc"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["ToplamKazanc"]);
                                if (lblToplamKazanc != null) lblToplamKazanc.Text = toplamKazanc.ToString("0.00") + " TL";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===============================
        // MENÜ EVENTLER
        // ===============================
        private void LabelDashboard_Click(object sender, EventArgs e) => RefreshAll();

        private void LabelSergiciTanimlama_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiciTanim(_galericiId));
        }

        private void LabelMusteriBilgileri_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiMusteriBilgileri(_galericiId));
        }

        private void LabelSergiler_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiler(_galericiId));
        }

        private void LabelRaporEkrani_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiRaporlar(_galericiId));
        }

        private void LabelGalericiProfilim_Click_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiProfilim(_galericiId));
        }

        private void LabelCikisYap_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            NavigateAndClose(new Form1());
        }

        // ===============================
        // FOOTER
        // ===============================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            // ✅ Doğru: ShowDialog -> kapanınca dashboard'a geri döner
            NavigateDialog(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            // ✅ Doğru: ShowDialog -> kapanınca dashboard'a geri döner
            NavigateDialog(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }

        // Designer bazen footer için farklı label event’i bağlar
        private void label9_Click(object sender, EventArgs e)
        {
            lblHakkimizda_Click(sender, e);
        }

        // DESIGNER STUBLAR
        private void label3_Click(object sender, EventArgs e) { }
        private void chart1_Click(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void ButtonDetay_Click(object sender, EventArgs e) { }

        private void chart3_Click(object sender, EventArgs e)
        {

        }
    }
}
