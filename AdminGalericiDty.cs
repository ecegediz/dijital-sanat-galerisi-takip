using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class AdminGalericiDty : Form
    {
        private int galleryId;

        // Detay genelde AdminDashboard üzerinden açılır
        private AdminDashboard mainForm;

        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private const string DURUM_AKTIF = "Aktif";
        private const string DURUM_PASIF = "Pasif";
        private const string ODEME_BEKLEMEDE = "Beklemede";
        private const string ODEME_ODENDI = "Odendi";

        private bool _closingByCode = false;

        public AdminGalericiDty()
        {
            InitializeComponent();

            this.Load -= Form8_Load;
            this.Load += Form8_Load;
        }

        public void SetGalleryInfo(int id, AdminDashboard dashboardForm)
        {
            galleryId = id;
            mainForm = dashboardForm;

            SetReadOnlyMode();
            LoadGalleryInfo();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            SetReadOnlyMode();

            if (galleryId > 0)
                LoadGalleryInfo();
        }

        // =========================================================
        // ✅ MODAL NAV: Hide -> ShowDialog -> geri dön (Refresh)
        // =========================================================
        private void OpenModal(Form next)
        {
            if (next == null) return;

            this.Hide();
            try
            {
                next.StartPosition = FormStartPosition.CenterScreen;
                next.ShowDialog();
            }
            finally
            {
                try { next.Dispose(); } catch { }

                if (!_closingByCode && !this.IsDisposed)
                {
                    this.Show();
                    this.BringToFront();
                    // geri dönünce güncel veri göster
                    LoadGalleryInfo();
                    mainForm?.RefreshAll();
                }
            }
        }

        // =========================================================
        // ✅ Logout: admin formları kapat, login göster
        // =========================================================
        private void LogoutToLogin()
        {
            var forms = Application.OpenForms.Cast<Form>().ToArray();
            var login = forms.OfType<Form1>().FirstOrDefault();

            foreach (var f in forms)
            {
                if (f == login) continue;
                if (f == this) continue;
                try { f.Close(); } catch { }
            }

            if (login != null)
            {
                login.Show();
                login.BringToFront();
            }
            else
            {
                new Form1().Show();
            }

            _closingByCode = true;
            this.Close();
        }

        // =========================================================
        // Detay ekranını "tam görüntüleme" moduna al
        // =========================================================
        private void MakeDisplayOnlyTextBox(TextBox tb)
        {
            if (tb == null) return;

            tb.ReadOnly = true;
            tb.TabStop = false;
            tb.ShortcutsEnabled = false;
            tb.Cursor = Cursors.Default;
            tb.BackColor = SystemColors.Control;
            tb.BorderStyle = BorderStyle.None;
        }

        private void SetReadOnlyMode()
        {
            MakeDisplayOnlyTextBox(txtAdSoyad);
            MakeDisplayOnlyTextBox(txtEposta);
            MakeDisplayOnlyTextBox(txtAylikUcret);

            if (dtpKayitTarihi != null) { dtpKayitTarihi.Enabled = false; dtpKayitTarihi.TabStop = false; }
            if (dtpSonGiris != null) { dtpSonGiris.Enabled = false; dtpSonGiris.TabStop = false; }
            if (dtpSonOdemeTarihi != null) { dtpSonOdemeTarihi.Enabled = false; dtpSonOdemeTarihi.TabStop = false; }
            if (dtpSonrakiOdemeTarihi != null) { dtpSonrakiOdemeTarihi.Enabled = false; dtpSonrakiOdemeTarihi.TabStop = false; }

            if (cmbDurum != null)
            {
                cmbDurum.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cmbDurum.Items.Count == 0)
                {
                    cmbDurum.Items.Add(DURUM_AKTIF);
                    cmbDurum.Items.Add(DURUM_PASIF);
                }
                cmbDurum.Enabled = false;
                cmbDurum.TabStop = false;
            }

            if (cmbBuAyOdemeDurumu != null)
            {
                cmbBuAyOdemeDurumu.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cmbBuAyOdemeDurumu.Items.Count == 0)
                {
                    cmbBuAyOdemeDurumu.Items.Add(ODEME_BEKLEMEDE);
                    cmbBuAyOdemeDurumu.Items.Add(ODEME_ODENDI);
                }
                cmbBuAyOdemeDurumu.Enabled = false;
                cmbBuAyOdemeDurumu.TabStop = false;
            }
        }

        // =========================================================
        // LOAD
        // =========================================================
        private void LoadGalleryInfo()
        {
            if (galleryId <= 0) return;

            using (var conn = new MySqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();

                    string sqlGalerici = @"
SELECT 
    AdSoyad,
    Eposta,
    KayitTarihi,
    SonGirisTarihi,
    AylikUcret,
    SonOdemeTarihi,
    SonrakiOdemeTarihi,
    BuAyOdemeDurumu,
    AbonelikDurumu
FROM galericiler
WHERE GalericiID = @id
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sqlGalerici, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", galleryId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Galerici bulunamadı!", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                                _closingByCode = true;
                                this.Close();
                                return;
                            }

                            if (txtAdSoyad != null) txtAdSoyad.Text = dr["AdSoyad"]?.ToString() ?? "";
                            if (txtEposta != null) txtEposta.Text = dr["Eposta"]?.ToString() ?? "";

                            if (dtpKayitTarihi != null)
                                dtpKayitTarihi.Value = dr["KayitTarihi"] == DBNull.Value
                                    ? DateTime.Today
                                    : Convert.ToDateTime(dr["KayitTarihi"]);

                            if (dtpSonGiris != null)
                                dtpSonGiris.Value = dr["SonGirisTarihi"] == DBNull.Value
                                    ? DateTime.Today
                                    : Convert.ToDateTime(dr["SonGirisTarihi"]);

                            if (txtAylikUcret != null)
                            {
                                decimal ucret = dr["AylikUcret"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["AylikUcret"]);
                                txtAylikUcret.Text = ucret.ToString("N2", new CultureInfo("tr-TR"));
                            }

                            if (dtpSonOdemeTarihi != null)
                                dtpSonOdemeTarihi.Value = dr["SonOdemeTarihi"] == DBNull.Value
                                    ? DateTime.Today
                                    : Convert.ToDateTime(dr["SonOdemeTarihi"]);

                            if (dtpSonrakiOdemeTarihi != null)
                                dtpSonrakiOdemeTarihi.Value = dr["SonrakiOdemeTarihi"] == DBNull.Value
                                    ? DateTime.Today
                                    : Convert.ToDateTime(dr["SonrakiOdemeTarihi"]);

                            if (cmbBuAyOdemeDurumu != null)
                            {
                                string odemeDurumu = dr["BuAyOdemeDurumu"]?.ToString() ?? ODEME_BEKLEMEDE;
                                cmbBuAyOdemeDurumu.SelectedItem = NormalizeOdemeDurumu(odemeDurumu);
                            }

                            if (cmbDurum != null)
                            {
                                string abonelikDurum = dr["AbonelikDurumu"]?.ToString() ?? DURUM_PASIF;
                                cmbDurum.SelectedItem = NormalizeAbonelikDurumu(abonelikDurum);
                            }
                        }
                    }

                    SetReadOnlyMode();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Galerici detayları yüklenirken hata:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string NormalizeAbonelikDurumu(string dbVal)
        {
            if (string.IsNullOrWhiteSpace(dbVal)) return DURUM_PASIF;
            return dbVal.Trim().Equals(DURUM_AKTIF, StringComparison.OrdinalIgnoreCase) ? DURUM_AKTIF : DURUM_PASIF;
        }

        private string NormalizeOdemeDurumu(string dbVal)
        {
            if (string.IsNullOrWhiteSpace(dbVal)) return ODEME_BEKLEMEDE;
            return dbVal.Trim().Equals(ODEME_ODENDI, StringComparison.OrdinalIgnoreCase) ? ODEME_ODENDI : ODEME_BEKLEMEDE;
        }

        // =========================================================
        // ✅ DÜZENLE (tek ekran): Detay Hide -> Edit ShowDialog -> geri dönünce refresh
        // =========================================================
        private void button2_Click(object sender, EventArgs e)
        {
            if (galleryId <= 0)
            {
                MessageBox.Show("Geçersiz GalericiID!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var editForm = new AdminGalericiDzn();
            editForm.SetGalleryInfo(galleryId, mainForm);

            OpenModal(editForm);
        }

        // =========================================================
        // SOL MENÜ
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            // Dashboard zaten açıksa ona dön, değilse modal aç
            if (mainForm != null && !mainForm.IsDisposed)
            {
                _closingByCode = true;
                this.Close();
                try
                {
                    mainForm.Show();
                    mainForm.BringToFront();
                    mainForm.RefreshAll();
                }
                catch { }
                return;
            }

            // dashboard yoksa modal açıp geri dönmek yerine kapatıp dashboard açmak daha temiz
            this.Hide();
            using (var dash = new AdminDashboard())
            {
                dash.ShowDialog();
            }
            _closingByCode = true;
            this.Close();
        }

        private void lblGalericiTanimlama_Click(object sender, EventArgs e)
        {
            if (mainForm != null && !mainForm.IsDisposed)
                OpenModal(new AdminGalericiTanimlama(mainForm));
            else
                OpenModal(new AdminGalericiTanimlama());
        }

        private void lblSifreBasvurulari_Click(object sender, EventArgs e)
        {
            OpenModal(new AdminBasvurular());
        }

        private void lblRaporlar_Click(object sender, EventArgs e)
        {
            // AdminRaporlar'ı modal aç
            OpenModal(new AdminRaporlar(mainForm));
        }

        private void lblCikis_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            LogoutToLogin();
        }

        // =========================================================
        // ✅ FOOTER
        // =========================================================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            OpenModal(new Hakkimizda(Hakkimizda.HomeRole.Admin, 0));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            OpenModal(new Yardim(Yardim.HomeRole.Admin, 0));
        }

        private void label13_Click(object sender, EventArgs e)
        {
            _closingByCode = true;
            this.Close();
        }

        // Designer stub’ları
        private void label17_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void lblraporlar_Click_1(object sender, EventArgs e) { }
        private void lblcikisyap_Click(object sender, EventArgs e) { }
    }
}
