using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace dijitalsanatgalerisi
{
    public partial class AdminDashboard : Form
    {
        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private DataTable galleryData;

        public AdminDashboard()
        {
            InitializeComponent();
            InitializeGalleryData();

            // Load eventini 1 kez bağla
            this.Load -= AdminDashboard_Load;
            this.Load += AdminDashboard_Load;
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            RefreshAll();
        }

        // =========================
        // MERKEZİ YENİLEME
        // =========================
        public void RefreshAll()
        {
            if (IsDisposed || !IsHandleCreated) return;

            try { AutoPassiveCheck(); }
            catch (Exception ex)
            {
                MessageBox.Show("Otomatik pasife çekme sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (IsDisposed || !IsHandleCreated) return;

            try { LoadGaleriFromDatabase(); }
            catch (Exception ex)
            {
                MessageBox.Show("Galericiler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (IsDisposed || !IsHandleCreated) return;

            try { LoadCharts(); }
            catch (Exception ex)
            {
                MessageBox.Show("Grafikler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // OTOMATİK PASİF KONTROLÜ
        // =========================
        private void AutoPassiveCheck()
        {
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                string sql = @"
UPDATE galericiler
SET AbonelikDurumu='Pasif',
    BuAyOdemeDurumu='Beklemede'
WHERE KullaniciTipi='Galerici'
  AND SonrakiOdemeTarihi IS NOT NULL
  AND CURDATE() > SonrakiOdemeTarihi;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =========================
        // DATATABLE ŞEMASI
        // =========================
        private void InitializeGalleryData()
        {
            galleryData = new DataTable();
            galleryData.Columns.Add("ID", typeof(int));
            galleryData.Columns.Add("Galerici", typeof(string));
            galleryData.Columns.Add("Eposta", typeof(string));
            galleryData.Columns.Add("Sifre", typeof(string));
            galleryData.Columns.Add("Durum", typeof(string));
            galleryData.Columns.Add("Fiyat", typeof(decimal));
        }

        // =========================
        // GRID AYAR
        // =========================
        private void SetupDataGridView()
        {
            if (dataGridView1 == null) return;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = galleryData;

            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            EnsureGridButtonColumn("Detay", "Detay", "Detay");
            EnsureGridButtonColumn("Sil", "Sil", "Sil");

            // ÇİFT BAĞLANMA ÖNLEMİ
            dataGridView1.CellClick -= DataGridView1_CellClick;
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void EnsureGridButtonColumn(string name, string headerText, string buttonText)
        {
            if (dataGridView1.Columns[name] != null) return;

            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = headerText,
                Text = buttonText,
                UseColumnTextForButtonValue = true
            });
        }

        // =========================
        // VERİ ÇEK
        // =========================
        private void LoadGaleriFromDatabase()
        {
            if (galleryData == null) InitializeGalleryData();

            galleryData.Rows.Clear();

            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                string sql = @"
SELECT 
    GalericiID,
    AdSoyad,
    Eposta,
    Sifre,
    COALESCE(AbonelikDurumu,'Pasif') AS Durum,
    COALESCE(AylikUcret,0) AS AylikUcret
FROM galericiler
WHERE KullaniciTipi='Galerici'
ORDER BY GalericiID DESC;";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    var temp = new DataTable();
                    da.Fill(temp);

                    foreach (DataRow r in temp.Rows)
                    {
                        galleryData.Rows.Add(
                            Convert.ToInt32(r["GalericiID"]),
                            r["AdSoyad"]?.ToString() ?? "",
                            r["Eposta"]?.ToString() ?? "",
                            r["Sifre"]?.ToString() ?? "",
                            r["Durum"]?.ToString() ?? "Pasif",
                            Convert.ToDecimal(r["AylikUcret"] ?? 0)
                        );
                    }
                }
            }

            dataGridView1.Refresh();
        }

        // =========================
        // CHART
        // =========================
        private void LoadCharts()
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (chart1 == null || chart2 == null) return;

            chart1.Series.Clear();
            chart2.Series.Clear();

            if (galleryData == null || galleryData.Rows.Count == 0)
                return;

            var gelir = new Series("Aylık Ücret")
            {
                ChartType = SeriesChartType.Column
            };

            int aktif = 0, pasif = 0;

            foreach (DataRow r in galleryData.Rows)
            {
                string ad = (r["Galerici"]?.ToString() ?? "").Trim();

                decimal fiyat = 0;
                try { fiyat = Convert.ToDecimal(r["Fiyat"]); } catch { fiyat = 0; }

                gelir.Points.AddXY(ad, fiyat);

                string durum = (r["Durum"]?.ToString() ?? "").Trim().ToLower();
                if (durum == "aktif") aktif++;
                else pasif++;
            }

            chart1.Series.Add(gelir);

            var durumSeries = new Series("Durum")
            {
                ChartType = SeriesChartType.Column
            };
            durumSeries.Points.AddXY("Aktif", aktif);
            durumSeries.Points.AddXY("Pasif", pasif);

            chart2.Series.Add(durumSeries);
        }

        // =========================
        // GRID BUTONLARI
        // =========================
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView1 == null) return;

            string colName = dataGridView1.Columns[e.ColumnIndex]?.Name ?? "";
            if (string.IsNullOrWhiteSpace(colName)) return;

            object idObj = dataGridView1.Rows[e.RowIndex].Cells["ID"]?.Value;
            if (idObj == null || idObj == DBNull.Value) return;

            int id = Convert.ToInt32(idObj);
            string ad = dataGridView1.Rows[e.RowIndex].Cells["Galerici"]?.Value?.ToString() ?? "";

            if (colName == "Detay")
            {
                // Dashboard KAPATMA YOK -> Hide + ShowDialog
                var detay = new AdminGalericiDty();
                detay.SetGalleryInfo(id, this);

                this.Hide();
                detay.ShowDialog();
                this.Show();

                // geri dönünce yenile
                RefreshAll();
                return;
            }

            if (colName == "Sil")
            {
                var onay = MessageBox.Show(
                    $"'{ad}' adlı galericiyi SİLMEK istiyor musunuz?\nBu işlem geri alınamaz!",
                    "Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (onay != DialogResult.Yes) return;

                try
                {
                    using (var conn = new MySqlConnection(ConnStr))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand("DELETE FROM galericiler WHERE GalericiID=@id;", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Galerici başarıyla silindi.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefreshAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme sırasında hata:\n" + ex.Message, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================
        // MENÜ
        // =========================
        private void dashboardlabel_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void galericitanimlamalabel_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminGalericiTanimlama())
                f.ShowDialog();
            this.Show();
            RefreshAll();
        }

        private void şifrebasvurularılabel_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminBasvurular())
                f.ShowDialog();
            this.Show();
            RefreshAll();
        }

        private void raporlarlabel_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminRaporlar())
                f.ShowDialog();
            this.Show();
            RefreshAll();
        }

        // =========================
        // ÇIKIŞ
        // =========================
        private void DoLogout()
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            this.Close(); // Form1 ShowDialog zinciri ile geri gelir
        }

        private void cikisyaplabel_Click(object sender, EventArgs e) => DoLogout();
        private void cikisyaplabel_Click_1(object sender, EventArgs e) => DoLogout();

        // =========================
        // FOOTER
        // =========================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new Hakkimizda(Hakkimizda.HomeRole.Admin, 0))
                f.ShowDialog();
            this.Show();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new Yardim(Yardim.HomeRole.Admin, 0))
                f.ShowDialog();
            this.Show();
        }

        private void label18_Click(object sender, EventArgs e) { }
        private void label17_Click(object sender, EventArgs e) { }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
