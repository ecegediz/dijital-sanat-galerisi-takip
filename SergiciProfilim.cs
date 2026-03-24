using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class SergiciProfilim : Form
    {
        private readonly int _sergiciId;
        private string _sergiciAdi;

        private string _orijinalAdSoyad = "";
        private string _orijinalEposta = "";

        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // ✅ SADECE izin verilen domainler
        private static readonly Regex AllowedEmailRegex =
            new Regex(@"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SergiciProfilim(int sergiciId, string sergiciAdi)
        {
            InitializeComponent();

            _sergiciId = sergiciId;
            _sergiciAdi = (sergiciAdi ?? "").Trim();

            // Designer bazılarını bağlamış olabilir; çifte çalışmayı önlemek için güvenli bağla
            HookMenuAndFooterClicksSafe();

            this.Load += SergiciProfilim_Load;
        }

        public SergiciProfilim() : this(0, "") { }

        // ================= NAVIGATION =================
        private void NavigateTo(Form next)
        {
            if (next == null) return;

            // Yeni form kapanınca bu form da kapansın (arkada açık kalmasın)
            next.FormClosed += (s, e) =>
            {
                try { this.Close(); } catch { /* ignore */ }
            };

            next.Show();
            this.Hide();
        }

        // ================= MENU / FOOTER HOOK =================
        private void HookMenuAndFooterClicksSafe()
        {
            // Designer bazen event bağlıyor, bazen bağlamıyor.
            // Burada önce kaldırıp sonra ekleyerek çifte tetiklemeyi engelliyoruz.

            if (dashboardButton != null)
            {
                dashboardButton.Click -= dashboardButton_Click;
                dashboardButton.Click += dashboardButton_Click;
            }

            if (eserlerimButton != null)
            {
                eserlerimButton.Click -= eserlerimButton_Click;
                eserlerimButton.Click += eserlerimButton_Click;
            }

            if (raporButton != null)
            {
                raporButton.Click -= raporButton_Click;
                raporButton.Click += raporButton_Click;
            }

            if (cikisYapButton != null)
            {
                cikisYapButton.Click -= cikisYapButton_Click;
                cikisYapButton.Click += cikisYapButton_Click;
            }

            if (hakkımızdaButton != null)
            {
                hakkımızdaButton.Click -= hakkımızdaButton_Click;
                hakkımızdaButton.Click += hakkımızdaButton_Click;
            }

            // Profilimde yardım "?" label5
            if (label5 != null)
            {
                label5.Click -= label5_Click;
                label5.Click += label5_Click;
            }

            // İstersen profil adının üstüne tıklanınca da aynı sayfada kal/yenile
            if (labelSergiciAdi != null)
            {
                labelSergiciAdi.Click -= labelSergiciAdi_Click;
                labelSergiciAdi.Click += labelSergiciAdi_Click;
            }
        }

        // ================= LOAD =================
        private void SergiciProfilim_Load(object sender, EventArgs e)
        {
            if (_sergiciId <= 0)
            {
                MessageBox.Show("Profil açılırken geçersiz kullanıcı ID geldi.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            ProfilBilgileriniYukle();
        }

        private void ProfilBilgileriniYukle()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT AdSoyad, Eposta FROM sergiciler WHERE SanatciID=@Id LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _sergiciId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Kullanıcı bulunamadı: " + _sergiciId, "Bilgi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            string ad = (dr["AdSoyad"]?.ToString() ?? "").Trim();
                            string ep = (dr["Eposta"]?.ToString() ?? "").Trim();

                            _orijinalAdSoyad = ad;
                            _orijinalEposta = ep;

                            txtKullaniciAdi.Text = ad;
                            txtEposta.Text = ep;

                            _sergiciAdi = ad;

                            if (labelSergiciAdi != null)
                                labelSergiciAdi.Text = string.IsNullOrWhiteSpace(ad) ? "—" : ad;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Profil bilgileri yüklenirken hata:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= VALIDATION =================
        private bool KullaniciAdiGecerliMi(string ad)
        {
            // Sadece harf, boşluk, apostrof, tire (Türkçe karakterler dahil)
            return Regex.IsMatch(ad, @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'-]+$");
        }

        private bool EmailGecerliMi(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            if (email.Contains(" ")) return false;

            // ✅ Normalize (büyük/küçük harf fark etmesin)
            email = email.Trim().ToLowerInvariant();

            // ✅ SADECE: hotmail.com, gmail.com, outlook.com, artflow.com
            return AllowedEmailRegex.IsMatch(email);
        }

        // ================= UPDATE =================
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            string yeniAd = (txtKullaniciAdi.Text ?? "").Trim();
            string yeniEposta = (txtEposta.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(yeniAd) || string.IsNullOrWhiteSpace(yeniEposta))
            {
                MessageBox.Show("Alanlar boş bırakılamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KullaniciAdiGecerliMi(yeniAd))
            {
                MessageBox.Show("Kullanıcı adı sayı veya geçersiz karakter içeremez.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EmailGecerliMi(yeniEposta))
            {
                MessageBox.Show(
                    "E-posta yalnızca şu uzantılarda olabilir:\n@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com\n\nÖrnek: betul@artflow.com",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool degistiMi =
                !string.Equals(yeniAd, _orijinalAdSoyad, StringComparison.Ordinal) ||
                !string.Equals(yeniEposta, _orijinalEposta, StringComparison.OrdinalIgnoreCase);

            if (!degistiMi)
            {
                MessageBox.Show("Değişiklik yapılmadı.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Profil bilgilerini güncellemek istiyor musunuz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE sergiciler SET AdSoyad=@Ad, Eposta=@E WHERE SanatciID=@Id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", yeniAd);
                        cmd.Parameters.AddWithValue("@E", yeniEposta.Trim().ToLowerInvariant());
                        cmd.Parameters.AddWithValue("@Id", _sergiciId);
                        cmd.ExecuteNonQuery();
                    }
                }

                _orijinalAdSoyad = yeniAd;
                _orijinalEposta = yeniEposta.Trim();

                _sergiciAdi = yeniAd;
                if (labelSergiciAdi != null) labelSergiciAdi.Text = yeniAd;

                MessageBox.Show("Profil başarıyla güncellendi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= MENU EVENTS (Designer bunları arıyor) =================
        private void dashboardButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciDashboard(_sergiciId, _sergiciAdi));
        }

        private void eserlerimButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi));
        }

        private void raporButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciRaporlar(_sergiciId, _sergiciAdi));
        }

        private void cikisYapButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkış yapmak istiyor musunuz?", "Onay",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NavigateTo(new Form1());
            }
        }

        // Profil adı tıklanınca (istersen burayı kaldırabilirsin)
        private void labelSergiciAdi_Click(object sender, EventArgs e)
        {
            // Zaten profil ekranındasın, istersen yenile:
            ProfilBilgileriniYukle();
        }

        // ================= FOOTER EVENTS =================
        private void hakkımızdaButton_Click(object sender, EventArgs e)
        {
            // Modal olsun istersen:
            // new Hakkimizda().ShowDialog();
            new Hakkimizda().Show();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Modal olsun istersen:
            // new Yardim().ShowDialog();
            new Yardim().Show();
        }
    }
}
