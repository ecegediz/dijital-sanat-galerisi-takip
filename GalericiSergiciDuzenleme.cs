using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiciDuzenleme : Form
    {
        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private int _galericiId;
        private int _sanatciId;

        private const decimal MAX_UCRET = 5000m;

        private const string DURUM_AKTIF = "Aktif";
        private const string DURUM_PASIF = "Pasif";

        // ✅ E-posta doğrulama: SADECE bu domainler kabul
        // hotmail.com, gmail.com, outlook.com, artflow.com
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Ad soyad: rakam yok
        private static readonly Regex NameRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,60}$",
            RegexOptions.Compiled);

        // ✅ Designer için
        public GalericiSergiciDuzenleme()
        {
            InitializeComponent();
            HookBaseEvents();
        }

        public GalericiSergiciDuzenleme(int galericiId, int sanatciId) : this()
        {
            _galericiId = galericiId;
            _sanatciId = sanatciId;
        }

        // =========================================================
        // EVENT GARANTİ
        // =========================================================
        private void HookBaseEvents()
        {
            this.Load -= GalericiSergiciDuzenleme_Load;
            this.Load += GalericiSergiciDuzenleme_Load;
        }

        private void GalericiSergiciDuzenleme_Load(object sender, EventArgs e)
        {
            // ✅ Designer kopsa bile: menü/footer/buton eventleri garanti bağla
            BindCriticalEvents();

            SetupLocks();
            SetupDurumCombo();
            LoadSergiciInfo();
            LoadDurumFromSergiler(); // ✅ Toplam eser/sergi textbox kaldırıldı, sadece durum hesaplanıyor
        }

        private void BindCriticalEvents()
        {
            // Butonlar (Designer Name ile aynı olmalı)
            BindButtonClick("btnTamam", btnTamam_Click);
            BindButtonClick("btnIptal", btnIptal_Click);

            // TextBox input filtreleri
            var txtUcret = FindControl<TextBox>("txtAylikUcret");
            if (txtUcret != null)
            {
                txtUcret.KeyPress -= txtAylikUcret_KeyPress;
                txtUcret.KeyPress += txtAylikUcret_KeyPress;

                txtUcret.Leave -= txtAylikUcret_Leave;
                txtUcret.Leave += txtAylikUcret_Leave;
            }

            var txtAd = FindControl<TextBox>("txtAdSoyad");
            if (txtAd != null)
            {
                txtAd.KeyPress -= txtAdSoyad_KeyPress;
                txtAd.KeyPress += txtAdSoyad_KeyPress;
            }

            // Menü (Designer Name ile aynı olmalı)
            BindLabelClick("lblDashboard", lblDashboard_Click);
            BindLabelClick("lblSergiciTanimlama", lblSergiciTanimlama_Click);
            BindLabelClick("lblMusteriBilgileri", lblMusteriBilgileri_Click);
            BindLabelClick("lblSergiler", lblSergiler_Click);
            BindLabelClick("lblRaporEkrani", lblRaporEkrani_Click);
            BindLabelClick("lblGalericiAdi", lblGalericiAdi_Click);
            BindLabelClick("lblCikisYap", lblCikisYap_Click);

            // Footer
            BindLabelClick("lblHakkimizda", lblHakkimizda_Click);
            BindLabelClick("lblYardim", lblYardim_Click);
        }

        private T FindControl<T>(string name) where T : Control
        {
            return this.Controls.Find(name, true).FirstOrDefault() as T;
        }

        private void BindButtonClick(string controlName, EventHandler handler)
        {
            var btn = FindControl<Button>(controlName);
            if (btn == null) return;

            btn.Click -= handler;
            btn.Click += handler;
        }

        private void BindLabelClick(string controlName, EventHandler handler)
        {
            var lbl = FindControl<Label>(controlName);
            if (lbl == null) return;

            lbl.Click -= handler;
            lbl.Click += handler;
        }

        // =========================================================
        // NAV (tek pencere)
        // =========================================================
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm != null) nextForm.Show();
            this.Close();
        }

        // =========================================================
        // KİLİTLER
        // =========================================================
        private void SetupLocks()
        {
            if (dtpKayitTarihi != null) dtpKayitTarihi.Enabled = false;
            if (dtpSonGiris != null) dtpSonGiris.Enabled = false;

            // Durum sistem kontrol eder
            if (cmbDurum != null) cmbDurum.Enabled = false;
        }

        private void SetupDurumCombo()
        {
            if (cmbDurum == null) return;

            cmbDurum.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDurum.Items.Clear();
            cmbDurum.Items.Add(DURUM_AKTIF);
            cmbDurum.Items.Add(DURUM_PASIF);
        }

        // =========================================================
        // DB -> FORM
        // =========================================================
        private void LoadSergiciInfo()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    SanatciID,
    AdSoyad,
    Eposta,
    KayitTarihi,
    COALESCE(AylikUcret,0) AS AylikUcret,
    COALESCE(Durum,'Pasif') AS Durum
FROM sergiciler
WHERE SanatciID = @sid AND GalericiID = @gid
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", _sanatciId);
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Sergici bulunamadı (ID/Galerici eşleşmiyor).", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                                return;
                            }

                            if (txtAdSoyad != null) txtAdSoyad.Text = dr["AdSoyad"]?.ToString() ?? "";
                            if (txtEposta != null) txtEposta.Text = dr["Eposta"]?.ToString() ?? "";

                            if (dtpKayitTarihi != null)
                                dtpKayitTarihi.Value = dr["KayitTarihi"] == DBNull.Value
                                    ? DateTime.Now
                                    : Convert.ToDateTime(dr["KayitTarihi"]);

                            if (txtAylikUcret != null)
                            {
                                decimal u = dr["AylikUcret"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["AylikUcret"]);
                                txtAylikUcret.Text = FormatTL(u);
                            }

                            SetComboSafe(cmbDurum, dr["Durum"]?.ToString() ?? DURUM_PASIF);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergici bilgileri yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Toplam eser/sergi textbox kaldırıldı.
        // Sadece sistem durumunu hesaplıyor: aktif sergi varsa Aktif, yoksa Pasif.
        private void LoadDurumFromSergiler()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT
   SUM(CASE 
        WHEN CURDATE() BETWEEN DATE(BaslangicTarihi) AND DATE(BitisTarihi) THEN 1 
        ELSE 0 
   END) AS AktifSergi
FROM sergiler
WHERE GalericiID=@gid AND SanatciID=@sid;";

                    int aktifSergi = 0;

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.Parameters.AddWithValue("@sid", _sanatciId);

                        object val = cmd.ExecuteScalar();
                        aktifSergi = (val == DBNull.Value || val == null) ? 0 : Convert.ToInt32(val);
                    }

                    string durum = aktifSergi > 0 ? DURUM_AKTIF : DURUM_PASIF;
                    SetComboSafe(cmbDurum, durum);

                    using (var upd = new MySqlCommand(
                        "UPDATE sergiciler SET Durum=@d WHERE GalericiID=@gid AND SanatciID=@sid;",
                        conn))
                    {
                        upd.Parameters.AddWithValue("@d", durum);
                        upd.Parameters.AddWithValue("@gid", _galericiId);
                        upd.Parameters.AddWithValue("@sid", _sanatciId);
                        upd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Durum yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TAMAM
        // =========================================================
        private void btnTamam_Click(object sender, EventArgs e)
        {
            if (!ValidateEditableFields(out string ad, out string ep, out decimal ucret))
                return;

            var confirm = MessageBox.Show(
                "Sergici bilgilerini güncellemek istediğinizden emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
UPDATE sergiciler
SET AdSoyad=@ad, Eposta=@ep, AylikUcret=@ucret
WHERE SanatciID=@sid AND GalericiID=@gid;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", ad);
                        cmd.Parameters.AddWithValue("@ep", ep);
                        cmd.Parameters.AddWithValue("@ucret", ucret);
                        cmd.Parameters.AddWithValue("@sid", _sanatciId);
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        int affected = cmd.ExecuteNonQuery();
                        if (affected <= 0)
                        {
                            MessageBox.Show("Güncellenecek kayıt bulunamadı.", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                MessageBox.Show("Sergici bilgileri güncellendi.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // İPTAL
        // =========================================================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Değişikliklerden vazgeçmek istiyor musunuz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            NavigateAndClose(new GalericiDashboard(_galericiId));
        }

        // =========================================================
        // VALIDATION
        // =========================================================
        private bool ValidateEditableFields(out string ad, out string ep, out decimal ucret)
        {
            ad = (txtAdSoyad?.Text ?? "").Trim();
            ep = (txtEposta?.Text ?? "").Trim();
            ucret = 0m;

            if (string.IsNullOrWhiteSpace(ad))
            {
                MessageBox.Show("Ad-Soyad boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAdSoyad?.Focus();
                return false;
            }

            if (!NameRegex.IsMatch(ad))
            {
                MessageBox.Show("Ad-Soyad geçersiz. Rakam içeremez.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAdSoyad?.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ep))
            {
                MessageBox.Show("E-posta boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEposta?.Focus();
                return false;
            }

            // ✅ SADECE izinli domainler
            if (!EmailRegex.IsMatch(ep))
            {
                MessageBox.Show(
                    "E-posta geçersiz.\nSadece şu uzantılar kabul edilir:\n@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtEposta?.Focus();
                return false;
            }

            if (!TryParseTL(txtAylikUcret?.Text, out ucret))
            {
                MessageBox.Show("Aylık ücret geçerli bir sayı olmalıdır. Örnek: 1500,50", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAylikUcret?.Focus();
                return false;
            }

            if (ucret <= 0m)
            {
                MessageBox.Show("Aylık ücret 0 TL olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAylikUcret?.Focus();
                return false;
            }

            if (ucret > MAX_UCRET)
            {
                MessageBox.Show($"Aylık ücret en fazla {MAX_UCRET:0.00} TL olabilir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAylikUcret?.Focus();
                return false;
            }

            if (txtAylikUcret != null) txtAylikUcret.Text = FormatTL(ucret);
            return true;
        }

        // =========================================================
        // INPUT FILTER + FORMAT
        // =========================================================
        private void txtAylikUcret_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            // virgül/nokta -> sadece 1 kere
            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                string t = txtAylikUcret.Text ?? "";
                if (t.Contains(",") || t.Contains(".")) e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void txtAylikUcret_Leave(object sender, EventArgs e)
        {
            if (txtAylikUcret == null) return;

            if (!TryParseTL(txtAylikUcret.Text, out var v)) return;
            txtAylikUcret.Text = FormatTL(v);
        }

        private void txtAdSoyad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (char.IsControl(e.KeyChar)) return;

            if (char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) ||
                e.KeyChar == '\'' || e.KeyChar == '-' || e.KeyChar == '.')
                return;

            e.Handled = true;
        }

        private bool TryParseTL(string text, out decimal value)
        {
            value = 0m;

            string s = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.Replace("₺", "").Replace("TL", "").Replace("tl", "").Trim();
            s = s.Replace(" ", "");

            if (s.Contains(".") && s.Contains(","))
                s = s.Replace(".", "");

            s = s.Replace(',', '.');

            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private string FormatTL(decimal value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
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

        // =========================================================
        // MENÜ + FOOTER (tek pencere)
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

        private void lblHakkimizda_Click(object sender, EventArgs e) => new Hakkimizda().ShowDialog();
        private void lblYardim_Click(object sender, EventArgs e) => new Yardim().ShowDialog();

        // =========================================================
        // DESIGNER STUBLAR (kalabilir)
        // =========================================================
        private void cmbDurum_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void btnOdendi_Click(object sender, EventArgs e) { }
        private void cmbDurum_Click(object sender, EventArgs e) { }
        private void txtToplamEserSayisi_TextChanged(object sender, EventArgs e) { } // Designer'da kalmışsa sorun olmaz
    }
}
