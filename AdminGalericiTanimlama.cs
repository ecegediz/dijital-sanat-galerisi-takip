using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class AdminGalericiTanimlama : Form
    {
        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // Dashboard referansı (varsa geri dönmek için)
        private AdminDashboard mainForm;

        // Başvurudan geldiyse otomatik doldur
        private string _adSoyad;
        private string _eposta;
        private int _basvuruId;

        private const decimal MaxAllowedPrice = 1000m;

        // Form içinde değişiklik yapıldı mı?
        private bool _dirty = false;
        private bool _closingByCode = false;

        // ✅ Sadece izin verilen domainler
        private static readonly Regex AllowedEmailRegex =
            new Regex(@"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // =======================
        // CTOR'lar
        // =======================
        public AdminGalericiTanimlama(AdminDashboard dashboard)
        {
            InitializeComponent();
            mainForm = dashboard;
            WireEvents();
        }

        public AdminGalericiTanimlama(string adSoyad, string eposta, int basvuruId)
        {
            InitializeComponent();
            _adSoyad = adSoyad;
            _eposta = eposta;
            _basvuruId = basvuruId;
            WireEvents();
        }

        public AdminGalericiTanimlama()
        {
            InitializeComponent();
            WireEvents();
        }

        // =======================
        // Event bağlama (güvenli)
        // =======================
        private void WireEvents()
        {
            this.Load -= AdminGalericiTanimlama_Load;
            this.Load += AdminGalericiTanimlama_Load;

            this.FormClosing -= AdminGalericiTanimlama_FormClosing;
            this.FormClosing += AdminGalericiTanimlama_FormClosing;

            // Kaydet
            if (btnKaydet != null)
            {
                btnKaydet.Click -= btnKaydet_Click;
                btnKaydet.Click += btnKaydet_Click;
            }

            // İptal
            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }

            // Dirty tracking
            if (txtAd != null) txtAd.TextChanged += MarkDirty;
            if (txtEposta != null) txtEposta.TextChanged += MarkDirty;
            if (txtSifre != null) txtSifre.TextChanged += MarkDirty;
            if (txtAylikUcret != null) txtAylikUcret.TextChanged += MarkDirty;

            // Password
            if (txtSifre != null)
                txtSifre.UseSystemPasswordChar = true;
        }

        private void MarkDirty(object sender, EventArgs e) => _dirty = true;

        private void AdminGalericiTanimlama_Load(object sender, EventArgs e)
        {
            // Başvurudan geldiyse doldur
            if (!string.IsNullOrWhiteSpace(_adSoyad) && txtAd != null)
                txtAd.Text = _adSoyad;

            if (!string.IsNullOrWhiteSpace(_eposta) && txtEposta != null)
                txtEposta.Text = _eposta;

            // İlk açılışta dirty sayma
            _dirty = false;
        }

        // =======================
        // Modal navigasyon helper
        // =======================
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
                }
            }
        }

        // =======================
        // Logout helper
        // =======================
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

        // =======================
        // KAYDET
        // =======================
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string ad = (txtAd?.Text ?? "").Trim();
            string eposta = (txtEposta?.Text ?? "").Trim();
            string sifre = (txtSifre?.Text ?? "");
            string ucretText = (txtAylikUcret?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(ad) ||
                string.IsNullOrWhiteSpace(eposta) ||
                string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Ad-Soyad, E-posta ve Şifre zorunludur!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Normalize (case-insensitive çalışsın + DB tutarlılığı)
            eposta = eposta.ToLowerInvariant();

            if (!IsValidEmail(eposta))
            {
                MessageBox.Show(
                    "E-posta geçersiz!\nSadece şu uzantılar kabul edilir:\n@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!IsStrongPassword(sifre, out string sifreHata))
            {
                MessageBox.Show(sifreHata, "Zayıf Şifre",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryParsePrice(ucretText, out decimal aylikUcret))
            {
                MessageBox.Show("Aylık ücret geçerli bir sayı olmalıdır! (örn: 100,50)", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (aylikUcret <= 0m || aylikUcret > MaxAllowedPrice)
            {
                MessageBox.Show($"Aylık ücret 1 - {MaxAllowedPrice:0} TL arasında olmalıdır!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime kayit = DateTime.Now;
            DateTime sonrakiOdeme = kayit.Date.AddMonths(1);

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    // E-posta benzersiz mi?
                    using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM galericiler WHERE Eposta=@eposta;", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@eposta", eposta);
                        long count = Convert.ToInt64(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Bu e-posta ile zaten kayıt var!", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert galerici
                    string sqlInsert = @"
INSERT INTO galericiler
(Eposta, Sifre, KullaniciTipi, YetkiTipi, AdSoyad,
 AylikUcret, KayitTarihi, SonrakiOdemeTarihi,
 BuAyOdemeDurumu, AbonelikDurumu, ToplamCiro)
VALUES
(@eposta, @sifre, 'Galerici', 'Galerici', @ad,
 @ucret, @kayit, @sonraki,
 'Beklemede', 'Pasif', 0);";

                    using (var insertCmd = new MySqlCommand(sqlInsert, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@eposta", eposta);
                        insertCmd.Parameters.AddWithValue("@sifre", sifre);
                        insertCmd.Parameters.AddWithValue("@ad", ad);
                        insertCmd.Parameters.AddWithValue("@ucret", aylikUcret);
                        insertCmd.Parameters.AddWithValue("@kayit", kayit);
                        insertCmd.Parameters.AddWithValue("@sonraki", sonrakiOdeme);
                        insertCmd.ExecuteNonQuery();
                    }

                    // Başvurudan geldiyse onayla
                    if (_basvuruId > 0)
                    {
                        using (var cmd = new MySqlCommand(@"
UPDATE galericibasvurular
SET OnayDurumu='Onaylandı', IslemTarihi=NOW()
WHERE BasvuruID=@id;", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _basvuruId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Galerici kaydedildi.\nİlk ödeme beklemede (Pasif).", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                try { mainForm?.RefreshAll(); } catch { }

                _dirty = false;
                _closingByCode = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =======================
        // İPTAL + X KAPANIŞ UYARISI
        // =======================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            if (!_dirty)
            {
                _closingByCode = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            var dr = MessageBox.Show(
                "İptal etmek istediğinizden emin misiniz?\nDeğişiklikler kaydedilmeyecek.",
                "İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                _closingByCode = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void AdminGalericiTanimlama_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;
            if (!_dirty) return;

            var dr = MessageBox.Show(
                "Çıkmak istiyor musunuz?\nDeğişiklikler kaydedilmeyecek.",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes)
                e.Cancel = true;
        }

        // =======================================================
        // ✅ MENÜ CLICK HANDLER’LARI (Designer bunları arıyor)
        // =======================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
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

            this.Hide();
            using (var dash = new AdminDashboard())
                dash.ShowDialog();

            _closingByCode = true;
            this.Close();
        }

        private void lblGalericiTanimlama_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAd != null) txtAd.Clear();
                if (txtEposta != null) txtEposta.Clear();
                if (txtSifre != null) txtSifre.Clear();
                if (txtAylikUcret != null) txtAylikUcret.Clear();

                _basvuruId = 0;
                _adSoyad = "";
                _eposta = "";
                _dirty = false;
            }
            catch { }
        }

        private void lblSifreBasvurulari_Click(object sender, EventArgs e)
        {
            OpenModal(new AdminBasvurular());
            try { mainForm?.RefreshAll(); } catch { }
        }

        private void lblRaporlar_Click(object sender, EventArgs e)
        {
            OpenModal(new AdminRaporlar());
        }

        private void lblCikis_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;
            LogoutToLogin();
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            OpenModal(new Hakkimizda(Hakkimizda.HomeRole.Admin, 0));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            OpenModal(new Yardim(Yardim.HomeRole.Admin, 0));
        }

        // =======================================================
        // HELPERS
        // =======================================================
        private bool TryParsePrice(string text, out decimal value)
        {
            value = 0m;
            string s = (text ?? "").Trim()
                .Replace("₺", "")
                .Replace("TL", "")
                .Trim();

            s = s.Replace(',', '.');
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        // ✅ SADECE izinli domainler
        private bool IsValidEmail(string email)
        {
            email = (email ?? "").Trim();
            return AllowedEmailRegex.IsMatch(email);
        }

        private bool IsStrongPassword(string pwd, out string msg)
        {
            pwd = pwd ?? "";
            var errors = new StringBuilder();

            if (pwd.Length < 8) errors.AppendLine("• Şifre en az 8 karakter olmalıdır.");
            if (Regex.IsMatch(pwd, @"\s")) errors.AppendLine("• Şifre boşluk içeremez.");
            if (!Regex.IsMatch(pwd, @"[A-Z]")) errors.AppendLine("• En az 1 büyük harf içermelidir.");
            if (!Regex.IsMatch(pwd, @"[a-z]")) errors.AppendLine("• En az 1 küçük harf içermelidir.");
            if (!Regex.IsMatch(pwd, @"\d")) errors.AppendLine("• En az 1 rakam içermelidir.");
            if (!Regex.IsMatch(pwd, @"[!@#$%^&*()_\-+=\[\]{};:'"",.<>/?\\|]"))
                errors.AppendLine("• En az 1 özel karakter içermelidir (örn: . ! @ # ).");

            msg = errors.ToString().Trim();
            return msg.Length == 0;
        }

        // =======================================================
        // Designer stub’ları (varsa kalsın)
        // =======================================================
        private void Label2_Click(object sender, EventArgs e) { }
        private void Label4_Click(object sender, EventArgs e) { }
        private void Label6_Click(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }
        private void Label7_Click(object sender, EventArgs e) { }
        private void Label8_Click(object sender, EventArgs e) { }
        private void Button1_Click(object sender, EventArgs e) { }
    }
}
