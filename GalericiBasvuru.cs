using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiBasvuru : Form
    {
        // VERİTABANI BAĞLANTISI
        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // VALIDASYON REGEXLERİ
        // AdSoyad: Türkçe harf + boşluk + '-' + '\'' (örn: Ayşe Yılmaz, Ali-Kaya, O'Connor)
        private static readonly Regex RxAdSoyad =
            new Regex(@"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-]{2,60}$", RegexOptions.Compiled);

        // ✅ Email: sadece izin verilen domainler
        private static readonly Regex RxEmail =
            new Regex(@"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Mesaj için basit limit (istersen artır)
        private const int MaxMesajLen = 500;

        public GalericiBasvuru()
        {
            InitializeComponent();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // KAYDET BUTONU
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string adSoyadTrim = (txtAdSoyad.Text ?? "").Trim();
            string epostaTrim = (txtEposta.Text ?? "").Trim();
            string mesaj = (txtMesaj.Text ?? "").Trim();

            // 1) Zorunlu alan kontrolü
            if (string.IsNullOrWhiteSpace(adSoyadTrim) || string.IsNullOrWhiteSpace(epostaTrim))
            {
                MessageBox.Show("Ad-Soyad ve E-posta alanları zorunludur!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Ad Soyad format kontrolü
            if (!RxAdSoyad.IsMatch(adSoyadTrim))
            {
                MessageBox.Show(
                    "Ad-Soyad sadece harflerden oluşmalıdır.\nBoşluk, '-' ve ' karakterleri kullanılabilir.\nÖrnek: Ayşe Yılmaz",
                    "Geçersiz Ad-Soyad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtAdSoyad.Focus();
                txtAdSoyad.SelectAll();
                return;
            }

            // 3) Email format kontrolü
            if (!RxEmail.IsMatch(epostaTrim))
            {
                MessageBox.Show(
                    "Lütfen geçerli bir e-posta giriniz.\nSadece şu uzantılar kabul edilir:\n@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Geçersiz E-posta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtEposta.Focus();
                txtEposta.SelectAll();
                return;
            }

            // 4) Eposta standardizasyonu (DB tutarlılığı için)
            epostaTrim = epostaTrim.ToLowerInvariant();

            // 5) Mesaj uzunluk kontrolü (DB'yi korur)
            if (mesaj.Length > MaxMesajLen)
            {
                MessageBox.Show($"Mesaj en fazla {MaxMesajLen} karakter olabilir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMesaj.Focus();
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    // 6) Aynı e-posta ile tekrar başvuru kontrolü (opsiyonel ama iyi)
                    using (var cmdCheck = new MySqlCommand(@"
SELECT COUNT(*)
FROM galericibasvurular
WHERE Eposta = @eposta;", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@eposta", epostaTrim);
                        long exists = Convert.ToInt64(cmdCheck.ExecuteScalar());
                        if (exists > 0)
                        {
                            MessageBox.Show(
                                "Bu e-posta ile daha önce başvuru yapılmış.\nAdmin onayı bekleniyor olabilir.",
                                "Bilgi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // 7) INSERT
                    using (var cmd = new MySqlCommand(@"
INSERT INTO galericibasvurular (AdSoyad, Eposta, Mesaj)
VALUES (@adSoyad, @eposta, @mesaj);", conn))
                    {
                        cmd.Parameters.AddWithValue("@adSoyad", adSoyadTrim);
                        cmd.Parameters.AddWithValue("@eposta", epostaTrim);
                        cmd.Parameters.AddWithValue("@mesaj", mesaj);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Başvurunuz alındı! Admin onayı bekleniyor.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Başvuru kaydedilirken hata oluştu:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // FOOTER (HAKKIMIZDA / ?)
        // =========================

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            new Yardim().ShowDialog();
        }

        // Designer stub'lar (istersen silebilirsin ama Designer bağlıysa kalsın)
        private void label6_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void Form3_Load(object sender, EventArgs e) { }
    }
}
