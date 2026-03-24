using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiEserYonetimi : Form
    {
        private readonly int _sergiId;
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private bool _sergiGecmisMi = false;
        private bool _pastInfoShown = false;

        // ✅ Tek tık = tek geçiş (çift açılmayı bitirir)
        private bool _isNavigating = false;

        public GalericiEserYonetimi(int sergiId, int galericiId)
        {
            InitializeComponent();
            _sergiId = sergiId;
            _galericiId = galericiId;

            this.Load -= GalericiEserYonetimi_Load;
            this.Load += GalericiEserYonetimi_Load;
        }


        public GalericiEserYonetimi() : this(0, 0) { }

        private void GalericiEserYonetimi_Load(object sender, EventArgs e)
        {
            // ❌ BindCriticalEvents() KALDIRILDI (çift açılmanın kaynağı buydu)

            _sergiGecmisMi = SergiGecmisMi();

            GridAyarla();
            EserleriYukle((txtEserAra?.Text ?? "").Trim());

            ApplyPastSergiRules();
        }

        // =========================================================
        // TEK PENCERE NAV (Kilitlemeli)
        // =========================================================
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;
            if (_isNavigating) return;

            _isNavigating = true;
            nextForm.Show();
            this.Close();
        }

        // =========================================================
        // SERGİ GEÇMİŞ Mİ?
        // =========================================================
        private bool SergiGecmisMi()
        {
            if (_sergiId <= 0) return false;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT COUNT(*) 
FROM sergiler
WHERE SergiID=@sid 
  AND DATE(BitisTarihi) < CURDATE()
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", _sergiId);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void ApplyPastSergiRules()
        {
            if (!_sergiGecmisMi) return;

            if (btnYeniEserEkle != null) btnYeniEserEkle.Enabled = false;
            if (btnEserDuzenle != null) btnEserDuzenle.Enabled = false;
            if (btnEserSil != null) btnEserSil.Enabled = false;

            if (!_pastInfoShown)
            {
                _pastInfoShown = true;
                MessageBox.Show(
                    "Bu sergi geçmiş tarihlidir.\nGeçmiş sergilerde eser ekleme, düzenleme veya silme yapılamaz.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private bool EnsureEditableSergi()
        {
            if (_sergiGecmisMi)
            {
                MessageBox.Show("Bu serginin tarihi geçtiği için işlem yapılamaz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // =========================================================
        // GRID
        // =========================================================
        private void GridAyarla()
        {
            if (dataGridView1 == null) return;

            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.ScrollBars = ScrollBars.Both;
        }

        private void EserleriYukle(string arama = "")
        {
            try
            {
                DataTable dt = new DataTable();

                string sql = @"
SELECT
    EserID,
    Baslik       AS 'Eser',
    SanatciAdi   AS 'Sanatçı',
    Kategori,
    Teknik,
    Boyut,
    Agirlik     AS 'Ağırlık',
    YapimYili   AS 'Yapım Yılı',
    Aciklama    AS 'Açıklama',
    Fiyat
FROM eserler
WHERE (@sid = 0 OR SergiID = @sid)";

                if (!string.IsNullOrWhiteSpace(arama))
                    sql += " AND Baslik LIKE @arama";

                sql += " ORDER BY EserID DESC;";

                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@sid", _sergiId);
                        if (!string.IsNullOrWhiteSpace(arama))
                            da.SelectCommand.Parameters.AddWithValue("@arama", "%" + arama + "%");

                        da.Fill(dt);
                    }
                }

                dataGridView1.DataSource = dt;

                SafeHideColumn("EserID");
                SafeSetWidth("Eser", 150);
                SafeSetWidth("Sanatçı", 120);
                SafeSetWidth("Kategori", 120);
                SafeSetWidth("Teknik", 120);
                SafeSetWidth("Boyut", 120);
                SafeSetWidth("Ağırlık", 100);
                SafeSetWidth("Yapım Yılı", 100);
                SafeSetWidth("Fiyat", 100);
                SafeSetWidth("Açıklama", 300);

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eserler yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SafeHideColumn(string colName)
        {
            if (dataGridView1?.Columns == null) return;
            if (dataGridView1.Columns.Contains(colName))
                dataGridView1.Columns[colName].Visible = false;
        }

        private void SafeSetWidth(string colName, int width)
        {
            if (dataGridView1?.Columns == null) return;
            if (dataGridView1.Columns.Contains(colName))
                dataGridView1.Columns[colName].Width = width;
        }

        // =========================================================
        // BUTONLAR (Designer Click eventleri bağlı olmalı)
        // =========================================================
        private void btnAra_Click(object sender, EventArgs e)
        {
            EserleriYukle((txtEserAra?.Text ?? "").Trim());
        }

        private void btnYeniEserEkle_Click(object sender, EventArgs e)
        {
            if (!EnsureEditableSergi()) return;

            using (var frm = new GalericiEserEkle(_sergiId, _galericiId))
            {
                Hide();
                frm.ShowDialog(this);
                Show();
            }

            EserleriYukle((txtEserAra?.Text ?? "").Trim());
        }

        private void btnEserDuzenle_Click(object sender, EventArgs e)
        {
            if (!EnsureEditableSergi()) return;

            if (dataGridView1 == null || dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir eser seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int eserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["EserID"].Value);

            using (var detay = new GalericiEserDetay(eserId, _sergiId, _galericiId))
            {
                Hide();
                detay.ShowDialog(this);
                Show();
            }

            EserleriYukle((txtEserAra?.Text ?? "").Trim());
        }

        private void btnEserSil_Click(object sender, EventArgs e)
        {
            if (!EnsureEditableSergi()) return;

            if (dataGridView1 == null || dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek için bir eser seçin.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int eserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["EserID"].Value);

            var dr = MessageBox.Show(
                "Seçili eseri silmek istediğinize emin misiniz?\nBu işlem geri alınamaz.",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM eserler WHERE EserID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", eserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            EserleriYukle((txtEserAra?.Text ?? "").Trim());
        }

        // =========================================================
        // MENÜ / FOOTER (Designer Click eventleri bağlı olmalı)
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiDashboard(_galericiId));
        private void lblSergiciTanimlama_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiciTanim(_galericiId));
        private void lblMusteriBilgileri_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiMusteriBilgileri(_galericiId));
        private void lblSergiler_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiler(_galericiId));
        private void lblRaporEkrani_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiRaporlar(_galericiId));
        private void lblGalericiAdi_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiProfilim(_galericiId));

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
                NavigateAndClose(new Form1());
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            // Parametreli ise onu kullan
            new Hakkimizda().ShowDialog(this);
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            // Parametreli ise onu kullan
            new Yardim().ShowDialog(this);
        }
    }
}
