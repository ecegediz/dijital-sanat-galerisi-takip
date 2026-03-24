using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dijitalsanatgalerisi
{
    public partial class AdminGalericiDzn : Form
    {
        private int galleryId;
        private AdminDashboard mainForm;

        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private const decimal MaxAllowedPrice = 100000m;

        private const string DURUM_AKTIF = "Aktif";
        private const string DURUM_PASIF = "Pasif";

        private const string ODEME_ODENDI = "Odendi";
        private const string ODEME_BEKLEMEDE = "Beklemede";

        private static readonly DateTime SafeMinDate = new DateTime(2000, 1, 1);

        private string _origAdSoyad = "";
        private string _origEposta = "";
        private string _origAylikUcret = "";

        private bool _closingByCode = false;

        // ✅ SADECE izin verilen domainler
        private static readonly Regex AllowedEmailRegex =
            new Regex(@"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AdminGalericiDzn()
        {
            InitializeComponent();

            this.Load -= AdminGalericiDzn_Load;
            this.Load += AdminGalericiDzn_Load;

            // Buttons
            if (btnOdendi != null)
            {
                btnOdendi.Click -= btnOdendi_Click;
                btnOdendi.Click += btnOdendi_Click;
            }
            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }
            if (btnTamam != null)
            {
                btnTamam.Click -= btnTamam_Click;
                btnTamam.Click += btnTamam_Click;
            }

            // Input restrictions
            if (txtAdSoyad != null)
            {
                txtAdSoyad.KeyPress -= txtAdSoyad_KeyPress;
                txtAdSoyad.KeyPress += txtAdSoyad_KeyPress;
            }

            if (txtAylikUcret != null)
            {
                txtAylikUcret.KeyPress -= txtAylikUcret_KeyPress;
                txtAylikUcret.KeyPress += txtAylikUcret_KeyPress;
            }

            if (txtEposta != null)
            {
                txtEposta.TextChanged -= txtEposta_TextChanged;
                txtEposta.TextChanged += txtEposta_TextChanged;
            }
        }

        public void SetGalleryInfo(int id, AdminDashboard dashboardForm)
        {
            galleryId = id;
            mainForm = dashboardForm;
        }

        private void AdminGalericiDzn_Load(object sender, EventArgs e)
        {
            SetupLocksAndCombos();
            LoadCurrentInfoFromDb();
        }

        // =========================================================
        // NAV: Yeni form aç, bu formu kapat
        // =========================================================
        private void NavigateAndClose(Form next)
        {
            if (next == null) return;

            _closingByCode = true;
            next.Show();
            this.Close();
        }

        private void SetupLocksAndCombos()
        {
            // Tarihler admin tarafından değişmesin
            if (dtpKayitTarihi != null) { dtpKayitTarihi.Enabled = false; dtpKayitTarihi.TabStop = false; }
            if (dtpSonGiris != null) { dtpSonGiris.Enabled = false; dtpSonGiris.TabStop = false; }
            if (dtpSonOdemeTarihi != null) { dtpSonOdemeTarihi.Enabled = false; dtpSonOdemeTarihi.TabStop = false; }
            if (dtpSonrakiOdemeTarihi != null) { dtpSonrakiOdemeTarihi.Enabled = false; dtpSonrakiOdemeTarihi.TabStop = false; }

            // Comboboxlar sadece gösterim (değerler DB’den/otomatik)
            if (cmbDurum != null)
            {
                cmbDurum.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbDurum.Items.Clear();
                cmbDurum.Items.Add(DURUM_AKTIF);
                cmbDurum.Items.Add(DURUM_PASIF);
                cmbDurum.Enabled = false;
                cmbDurum.TabStop = false;
            }

            if (cmbBuAyOdemeDurumu != null)
            {
                cmbBuAyOdemeDurumu.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbBuAyOdemeDurumu.Items.Clear();
                cmbBuAyOdemeDurumu.Items.Add(ODEME_BEKLEMEDE);
                cmbBuAyOdemeDurumu.Items.Add(ODEME_ODENDI);
                cmbBuAyOdemeDurumu.Enabled = false;
                cmbBuAyOdemeDurumu.TabStop = false;
            }

            // Varsayılanlar
            SetComboSafe(cmbDurum, DURUM_PASIF);
            SetComboSafe(cmbBuAyOdemeDurumu, ODEME_BEKLEMEDE);
        }

        private void LoadCurrentInfoFromDb()
        {
            if (galleryId <= 0)
            {
                MessageBox.Show("Geçersiz GalericiID!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            using (var conn = new MySqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();

                    string sql = @"
SELECT 
    AdSoyad,
    Eposta,
    COALESCE(AylikUcret,0) AS AylikUcret,
    KayitTarihi,
    SonGirisTarihi,
    SonOdemeTarihi,
    SonrakiOdemeTarihi,
    COALESCE(BuAyOdemeDurumu,'Beklemede') AS BuAyOdemeDurumu
FROM galericiler
WHERE GalericiID = @id
LIMIT 1;";

                    string adSoyad, eposta, buAyDurum;
                    decimal aylik;
                    DateTime kayit, sonGiris, sonOdeme, sonraki;
                    DateTime today = DateTime.Today;

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", galleryId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Galerici bulunamadı!", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Close();
                                return;
                            }

                            adSoyad = dr["AdSoyad"]?.ToString() ?? "";
                            eposta = dr["Eposta"]?.ToString() ?? "";
                            aylik = (dr["AylikUcret"] == DBNull.Value) ? 0m : Convert.ToDecimal(dr["AylikUcret"]);

                            kayit = ClampToSafe(ReadDateOrDefault(dr["KayitTarihi"], today));
                            sonGiris = ClampToSafe(ReadDateOrDefault(dr["SonGirisTarihi"], today));
                            sonOdeme = ClampToSafe(ReadDateOrDefault(dr["SonOdemeTarihi"], today));
                            sonraki = ClampToSafe(ReadDateOrDefault(dr["SonrakiOdemeTarihi"], today));

                            buAyDurum = dr["BuAyOdemeDurumu"]?.ToString() ?? ODEME_BEKLEMEDE;
                        }
                    }

                    // UI set
                    if (txtAdSoyad != null) txtAdSoyad.Text = adSoyad;
                    if (txtEposta != null) txtEposta.Text = eposta;
                    if (txtAylikUcret != null) txtAylikUcret.Text = aylik.ToString("N2", new CultureInfo("tr-TR"));

                    // Senin eski kuralın: bugünden eski olmasın
                    if (kayit < today) kayit = today;
                    if (sonGiris < today) sonGiris = today;
                    if (sonOdeme < today) sonOdeme = today;
                    if (sonraki < today) sonraki = today;

                    if (dtpKayitTarihi != null) dtpKayitTarihi.Value = kayit;
                    if (dtpSonGiris != null) dtpSonGiris.Value = sonGiris;
                    if (dtpSonOdemeTarihi != null) dtpSonOdemeTarihi.Value = sonOdeme;
                    if (dtpSonrakiOdemeTarihi != null) dtpSonrakiOdemeTarihi.Value = sonraki;

                    SetComboSafe(cmbBuAyOdemeDurumu, buAyDurum);

                    // Durum otomatik
                    string abonelik = (sonraki.Date < today) ? DURUM_PASIF : DURUM_AKTIF;
                    SetComboSafe(cmbDurum, abonelik);

                    // DB güncelle (istenen: edit ekranda otomatik yazsın)
                    AutoUpdateDurum(conn, abonelik, buAyDurum, sonOdeme.Date, sonraki.Date);

                    // Snapshot
                    _origAdSoyad = txtAdSoyad?.Text ?? "";
                    _origEposta = txtEposta?.Text ?? "";
                    _origAylikUcret = txtAylikUcret?.Text ?? "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bilgiler yüklenirken hata:\n" + ex.Message, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AutoUpdateDurum(MySqlConnection conn, string abonelikDurumu, string buAyOdemeDurumu, DateTime sonOdeme, DateTime sonrakiOdeme)
        {
            string sql = @"
UPDATE galericiler
SET 
    AbonelikDurumu = @durum,
    BuAyOdemeDurumu = @buAy,
    SonOdemeTarihi = @son,
    SonrakiOdemeTarihi = @sonraki
WHERE GalericiID = @id;";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@durum", abonelikDurumu);
                cmd.Parameters.AddWithValue("@buAy", buAyOdemeDurumu);
                cmd.Parameters.AddWithValue("@son", sonOdeme);
                cmd.Parameters.AddWithValue("@sonraki", sonrakiOdeme);
                cmd.Parameters.AddWithValue("@id", galleryId);
                cmd.ExecuteNonQuery();
            }
        }

        // ===========================
        // TAMAM (KAYDET)
        // ===========================
        private void btnTamam_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string adSoyad, out string eposta, out decimal aylikUcret))
                return;

            using (var conn = new MySqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();

                    string sql = @"
UPDATE galericiler
SET 
    AdSoyad = @ad,
    Eposta = @eposta,
    AylikUcret = @ucret
WHERE GalericiID = @id;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", adSoyad);
                        cmd.Parameters.AddWithValue("@eposta", eposta);
                        cmd.Parameters.AddWithValue("@ucret", aylikUcret);
                        cmd.Parameters.AddWithValue("@id", galleryId);
                        cmd.ExecuteNonQuery();
                    }

                    try { mainForm?.RefreshAll(); } catch { }

                    _origAdSoyad = txtAdSoyad?.Text ?? "";
                    _origEposta = txtEposta?.Text ?? "";
                    _origAylikUcret = txtAylikUcret?.Text ?? "";

                    MessageBox.Show("Değişiklikler kaydedildi.", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kaydetme sırasında hata:\n" + ex.Message, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===========================
        // İPTAL
        // ===========================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var dr = MessageBox.Show(
                    "Değişiklikleri kaydetmeden çıkmak istiyor musunuz?",
                    "Uyarı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes) return;
            }

            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool HasUnsavedChanges()
        {
            string a = txtAdSoyad?.Text ?? "";
            string e = txtEposta?.Text ?? "";
            string u = txtAylikUcret?.Text ?? "";

            return !string.Equals(a, _origAdSoyad, StringComparison.Ordinal)
                || !string.Equals(e, _origEposta, StringComparison.Ordinal)
                || !string.Equals(u, _origAylikUcret, StringComparison.Ordinal);
        }

        // ===========================
        // ÖDENDİ
        // ===========================
        private void btnOdendi_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bu galericinin bu ayki ödemesi alındı mı?\nEvet derseniz ödeme 'Ödendi' olarak kaydedilecektir.",
                "Ödeme Teyidi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            if (!ValidateInputs(out string adSoyad, out string eposta, out decimal aylikUcret))
                return;

            DateTime today = DateTime.Today;
            DateTime next = today.AddMonths(1);

            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"
UPDATE galericiler
SET
    AdSoyad = @ad,
    Eposta = @eposta,
    AylikUcret = @ucret,
    SonOdemeTarihi = @sonOdeme,
    SonrakiOdemeTarihi = @sonrakiOdeme,
    BuAyOdemeDurumu = @buAy,
    AbonelikDurumu = @durum
WHERE GalericiID = @id;";

                        using (var cmd = new MySqlCommand(sql, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ad", adSoyad);
                            cmd.Parameters.AddWithValue("@eposta", eposta);
                            cmd.Parameters.AddWithValue("@ucret", aylikUcret);
                            cmd.Parameters.AddWithValue("@sonOdeme", today);
                            cmd.Parameters.AddWithValue("@sonrakiOdeme", next);
                            cmd.Parameters.AddWithValue("@buAy", ODEME_ODENDI);
                            cmd.Parameters.AddWithValue("@durum", DURUM_AKTIF);
                            cmd.Parameters.AddWithValue("@id", galleryId);

                            if (cmd.ExecuteNonQuery() <= 0)
                                throw new Exception("Güncellenecek kayıt bulunamadı.");
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show("Ödeme işlenirken hata:\n" + ex.Message, "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            if (dtpSonOdemeTarihi != null) dtpSonOdemeTarihi.Value = today;
            if (dtpSonrakiOdemeTarihi != null) dtpSonrakiOdemeTarihi.Value = next;
            SetComboSafe(cmbBuAyOdemeDurumu, ODEME_ODENDI);
            SetComboSafe(cmbDurum, DURUM_AKTIF);

            try { mainForm?.RefreshAll(); } catch { }

            MessageBox.Show("Ödeme işlendi: Bu ay 'Ödendi' olarak kaydedildi.", "Başarılı",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            Close();
        }

        // ===========================
        // VALIDATION
        // ===========================
        private bool ValidateInputs(out string adSoyad, out string eposta, out decimal aylikUcret)
        {
            adSoyad = (txtAdSoyad?.Text ?? "").Trim();
            eposta = (txtEposta?.Text ?? "").Trim();
            aylikUcret = 0m;

            if (string.IsNullOrWhiteSpace(adSoyad))
            {
                MessageBox.Show("Ad Soyad boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Harf + boşluk (Türkçe dahil)
            if (!Regex.IsMatch(adSoyad, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Ad Soyad sadece harflerden oluşmalıdır (boşluk hariç başka karakter olamaz).", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (adSoyad.Length < 3)
            {
                MessageBox.Show("Ad Soyad çok kısa.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(eposta))
            {
                MessageBox.Show("E-posta boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (eposta.Contains(" "))
            {
                MessageBox.Show("E-posta boşluk içeremez.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ✅ Normalize
            eposta = eposta.ToLowerInvariant();

            // ✅ SADECE izinli domainler
            if (!AllowedEmailRegex.IsMatch(eposta))
            {
                MessageBox.Show(
                    "E-posta yalnızca şu uzantılarda olabilir:\n@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!TryParsePrice(txtAylikUcret?.Text, out aylikUcret))
            {
                MessageBox.Show("Aylık ücret geçerli bir sayı olmalıdır. Örnek: 100,50", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (aylikUcret <= 0m)
            {
                MessageBox.Show("Aylık ücret 0 veya negatif olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (aylikUcret > MaxAllowedPrice)
            {
                MessageBox.Show($"Aylık ücret çok yüksek. En fazla {MaxAllowedPrice:N2} TL olabilir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ===========================
        // INPUT RESTRICTIONS
        // ===========================
        private void txtAdSoyad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!(char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)))
                e.Handled = true;
        }

        private void txtAylikUcret_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                string t = txtAylikUcret?.Text ?? "";
                if (t.Contains(",") || t.Contains("."))
                    e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void txtEposta_TextChanged(object sender, EventArgs e)
        {
            if (txtEposta == null) return;

            if (txtEposta.Text.Contains(" "))
            {
                txtEposta.Text = txtEposta.Text.Replace(" ", "");
                txtEposta.SelectionStart = txtEposta.TextLength;
            }
        }

        // ===========================
        // HELPERS
        // ===========================
        private bool TryParsePrice(string text, out decimal value)
        {
            value = 0m;
            string s = (text ?? "").Trim();

            // ₺, TL, boşluk
            s = Regex.Replace(s, @"\s|₺|tl", "", RegexOptions.IgnoreCase);

            // TR virgül -> nokta
            s = s.Replace(',', '.');

            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void SetComboSafe(ComboBox combo, string val)
        {
            if (combo == null) return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (string.Equals(combo.Items[i].ToString(), val, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.Items.Add(val);
            combo.SelectedItem = val;
        }

        private DateTime ReadDateOrDefault(object dbVal, DateTime fallback)
        {
            if (dbVal == null || dbVal == DBNull.Value) return fallback;
            if (DateTime.TryParse(dbVal.ToString(), out DateTime d)) return d;
            return fallback;
        }

        private DateTime ClampToSafe(DateTime d)
        {
            if (d < SafeMinDate) return DateTime.Today;
            return d;
        }

        // =========================================================
        // ✅ MENÜ + FOOTER CLICK HANDLERS
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            if (mainForm != null && !mainForm.IsDisposed)
            {
                try
                {
                    mainForm.Show();
                    mainForm.BringToFront();
                    mainForm.RefreshAll();
                }
                catch { }

                _closingByCode = true;
                this.Close();
                return;
            }

            NavigateAndClose(new AdminDashboard());
        }

        private void lblGalericiTanimlama_Click_1(object sender, EventArgs e)
        {
            if (mainForm != null && !mainForm.IsDisposed)
                NavigateAndClose(new AdminGalericiTanimlama(mainForm));
            else
                NavigateAndClose(new AdminGalericiTanimlama());
        }

        private void lblgalericitanimlama_Click_1(object sender, EventArgs e)
        {
            lblGalericiTanimlama_Click_1(sender, e);
        }

        private void lblSifreBasvurulari_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new AdminBasvurular());
        }

        private void lblRaporlar_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new AdminRaporlar());
        }

        private void lblCikis_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            try { mainForm?.Hide(); } catch { }

            new Form1().Show();
            _closingByCode = true;
            this.Close();
        }

        private void lblcikisyap_Click(object sender, EventArgs e)
        {
            lblCikis_Click(sender, e);
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new Hakkimizda(Hakkimizda.HomeRole.Admin, 0));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new Yardim(Yardim.HomeRole.Admin, 0));
        }

        // =========================================================
        // Designer stub (kalsın)
        // =========================================================
        private void label2_Click(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void cmbDurum_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) { }
    }
}
