using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiEserDetayDuzen : Form
    {
        private readonly int _eserId;
        private readonly int _sergiId;
        private readonly int _galericiId;

        private readonly MySqlConnection _baglanti =
            new MySqlConnection("Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;");

        // ===== Kurallar =====
        private const decimal MIN_FIYAT_TL = 1m;
        private const decimal MAX_FIYAT_TL = 10_000m;

        private const decimal MIN_AGIRLIK_KG = 0.01m;
        private const decimal MAX_AGIRLIK_KG = 5000m;

        // Boyut: cm (genişlik x yükseklik)
        private const decimal MIN_BOYUT_CM = 1.5m;
        private const decimal MAX_BOYUT_CM = 500m;

        private static readonly int MIN_YIL = 1500;
        private static readonly int MAX_YIL = DateTime.Now.Year;

        private static readonly Regex RX_ONLY_LETTERS =
            new Regex(@"^[A-Za-zÇĞİÖŞÜçğıöşü\s]+$", RegexOptions.Compiled);

        // "20x30", "20,5 x 30,2 cm", "20×30" vs.
        private static readonly Regex RX_BOYUT = 
            new Regex(@"^\s*(?<w>\d+(?:[.,]\d+)?)\s*[xX×]\s*(?<h>\d+(?:[.,]\d+)?)\s*(cm)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public GalericiEserDetayDuzen(int eserId, int sergiId, int galericiId)
        {
            InitializeComponent();
            _eserId = eserId;
            _sergiId = sergiId;
            _galericiId = galericiId;

            Load -= GalericiEserDetayDuzen_Load;
            Load += GalericiEserDetayDuzen_Load;
        }

        public GalericiEserDetayDuzen(int eserId) : this(eserId, 0, 0) { }
        public GalericiEserDetayDuzen() : this(0, 0, 0) { }

        private void GalericiEserDetayDuzen_Load(object sender, EventArgs e)
        {
            if (pictureBox1 != null)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            SetupKategoriCombo();
            HookValidationEvents();
            SetupReadOnlyAutoFields();
            BindMenuFooterEventsSafe();

            EseriYukle();
        }

        // ==========================
        // MENU + FOOTER (tek pencere)
        // ==========================
        private void NavigateAndClose(Form next)
        {
            if (next != null) next.Show();
            this.Close();
        }

        private void BindMenuFooterEventsSafe()
        {
            // Eğer designer bağları koparsa diye garanti bağlama:
            // (Bu formda bu label'lar varsa)
            TryBindLabel("lblDashboard", lblDashboard_Click);
            TryBindLabel("lblSergiciTanimlama", lblSergiciTanimlama_Click);
            TryBindLabel("lblMusteriBilgileri", lblMusteriBilgileri_Click);
            TryBindLabel("lblSergiler", lblSergiler_Click);
            TryBindLabel("lblRaporEkrani", lblRaporEkrani_Click);
            TryBindLabel("lblGalericiAdi", lblGalericiAdi_Click);
            TryBindLabel("lblCikisYap", lblCikisYap_Click);

            TryBindLabel("lblHakkimizda", lblHakkimizda_Click);
            TryBindLabel("lblYardim", lblYardim_Click);
        }

        private void TryBindLabel(string name, EventHandler handler)
        {
            var c = this.Controls.Find(name, true);
            if (c == null || c.Length == 0) return;

            var lbl = c[0] as Label;
            if (lbl == null) return;

            lbl.Click -= handler;
            lbl.Click += handler;
        }

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
            // Tek pencere mantığı istiyorsan Show+Close:
            NavigateAndClose(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }

        // ==========================
        // UI SETUP
        // ==========================
        private void SetupKategoriCombo()
        {
            if (cmbKategori == null) return;

            cmbKategori.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKategori.Items.Clear();
            cmbKategori.Items.AddRange(new object[]
            {
                "Resim","Heykel","Dijital Sanat","Fotoğraf","Seramik","Enstalasyon",
                "Karışık Teknik","Baskı / Print","Video Sanat"
            });

            if (cmbKategori.Items.Count > 0)
                cmbKategori.SelectedIndex = 0;
        }

        private void EnsureKategoriFromDb(string kat)
        {
            if (cmbKategori == null) return;

            if (string.IsNullOrWhiteSpace(kat))
            {
                cmbKategori.SelectedIndex = 0;
                return;
            }

            int i = cmbKategori.FindStringExact(kat);
            if (i >= 0) cmbKategori.SelectedIndex = i;
            else
            {
                cmbKategori.Items.Add(kat);
                cmbKategori.SelectedIndex = cmbKategori.Items.Count - 1;
            }
        }

        private string SelectedKategori()
        {
            return (cmbKategori?.SelectedItem == null) ? "" : cmbKategori.SelectedItem.ToString();
        }

        private void SetupReadOnlyAutoFields()
        {
            MakeReadOnly(txtStokDurumu);
            MakeReadOnly(txtEserDurumu);
            MakeReadOnly(txtSergi);
            MakeReadOnly(txtDurumu);
        }

        private void MakeReadOnly(TextBox tb)
        {
            if (tb == null) return;
            tb.ReadOnly = true;
            tb.TabStop = false;
            tb.BackColor = SystemColors.Control;
        }

        // ==========================
        // VALIDATION EVENTS
        // ==========================
        private void HookValidationEvents()
        {
            if (txtFiyat != null)
            {
                txtFiyat.KeyPress -= Decimal_KeyPress;
                txtFiyat.KeyPress += Decimal_KeyPress;

                txtFiyat.Leave -= txtFiyat_Leave;
                txtFiyat.Leave += txtFiyat_Leave;
            }

            if (txtAgirlik != null)
            {
                txtAgirlik.KeyPress -= Decimal_KeyPress;
                txtAgirlik.KeyPress += Decimal_KeyPress;

                txtAgirlik.Leave -= txtAgirlik_Leave;
                txtAgirlik.Leave += txtAgirlik_Leave;
            }

            if (txtBoyut != null)
            {
                txtBoyut.Leave -= txtBoyut_Leave;
                txtBoyut.Leave += txtBoyut_Leave;
            }

            if (txtEserAdi != null)
            {
                txtEserAdi.Leave -= LettersOnly_Leave;
                txtEserAdi.Leave += LettersOnly_Leave;
            }

            if (txtSanatci != null)
            {
                txtSanatci.Leave -= LettersOnly_Leave;
                txtSanatci.Leave += LettersOnly_Leave;
            }

            if (txtTeknik != null)
            {
                txtTeknik.Leave -= LettersOnly_Leave;
                txtTeknik.Leave += LettersOnly_Leave;
            }
        }

        private void Decimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;

            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                var tb = sender as TextBox;
                if (tb == null) { e.Handled = true; return; }
                if (!tb.Text.Contains(",") && !tb.Text.Contains(".")) return;
            }

            e.Handled = true;
        }

        private void LettersOnly_Leave(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var val = (tb.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(val)) return;

            if (!RX_ONLY_LETTERS.IsMatch(val))
            {
                MessageBox.Show("Bu alan sadece harf ve boşluk içerebilir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Focus();
                tb.SelectAll();
            }
        }

        private void txtFiyat_Leave(object sender, EventArgs e)
        {
            if (txtFiyat == null) return;
            if (!TryParseDecimal(txtFiyat.Text, out decimal v)) return;

            if (v < MIN_FIYAT_TL || v > MAX_FIYAT_TL)
            {
                MessageBox.Show($"Fiyat aralık dışında.\nMin: {MIN_FIYAT_TL}\nMax: {MAX_FIYAT_TL}",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFiyat.Focus();
                txtFiyat.SelectAll();
                return;
            }

            // TR görünüm istersen "tr-TR" yapabilirsin; ben sabit 2 ondalık verdim:
            txtFiyat.Text = v.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
        }

        private void txtAgirlik_Leave(object sender, EventArgs e)
        {
            if (txtAgirlik == null) return;
            if (!TryParseDecimal(txtAgirlik.Text, out decimal v)) return;

            if (v < MIN_AGIRLIK_KG || v > MAX_AGIRLIK_KG)
            {
                MessageBox.Show($"Ağırlık aralık dışında.\nMin: {MIN_AGIRLIK_KG}\nMax: {MAX_AGIRLIK_KG}",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAgirlik.Focus();
                txtAgirlik.SelectAll();
                return;
            }

            txtAgirlik.Text = v.ToString("0.###", CultureInfo.InvariantCulture) + " kg";
        }

        private void txtBoyut_Leave(object sender, EventArgs e)
        {
            if (txtBoyut == null) return;
            if (!TryParseBoyutCm(txtBoyut.Text, out decimal w, out decimal h)) return;

            if (!IsBetween(w, MIN_BOYUT_CM, MAX_BOYUT_CM) || !IsBetween(h, MIN_BOYUT_CM, MAX_BOYUT_CM))
            {
                MessageBox.Show($"Boyut (cm) aralık dışında.\nHer kenar için Min: {MIN_BOYUT_CM} cm\nMax: {MAX_BOYUT_CM} cm",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBoyut.Focus();
                txtBoyut.SelectAll();
                return;
            }

            txtBoyut.Text = $"{w.ToString("0.##", CultureInfo.InvariantCulture)} x {h.ToString("0.##", CultureInfo.InvariantCulture)} cm";
        }

        private bool IsBetween(decimal v, decimal min, decimal max) => v >= min && v <= max;

        // ==========================
        // PARSE HELPERS (CS1501 FIX)
        // ==========================
        private bool TryParseDecimal(string t, out decimal v)
        {
            v = 0m;
            string s = (t ?? "").Trim();
            if (string.IsNullOrWhiteSpace(s)) return false;

            // 3 parametreli Replace YOK -> .NET Framework uyumlu temizlik
            s = s.Replace("₺", "");
            s = s.Replace("TL", "").Replace("tl", "");
            s = s.Replace("KG", "").Replace("Kg", "").Replace("kg", "");
            s = s.Replace(" ", "");

            // tr-TR dene
            if (decimal.TryParse(s, NumberStyles.Number, new CultureInfo("tr-TR"), out v))
                return true;

            // invariant dene
            s = s.Replace(',', '.');
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v);
        }

        private bool TryParseBoyutCm(string input, out decimal w, out decimal h)
        {
            w = 0m; h = 0m;
            input = (input ?? "").Trim();

            var m = RX_BOYUT.Match(input);
            if (!m.Success)
            {
                MessageBox.Show("Boyut formatı geçersiz.\nÖrnek: 20x30 veya 20,5 x 30,2 (cm).",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBoyut?.Focus();
                txtBoyut?.SelectAll();
                return false;
            }

            var ws = m.Groups["w"].Value.Replace(',', '.');
            var hs = m.Groups["h"].Value.Replace(',', '.');

            if (!decimal.TryParse(ws, NumberStyles.Number, CultureInfo.InvariantCulture, out w) ||
                !decimal.TryParse(hs, NumberStyles.Number, CultureInfo.InvariantCulture, out h))
            {
                MessageBox.Show("Boyut değerleri sayı olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBoyut?.Focus();
                txtBoyut?.SelectAll();
                return false;
            }

            return true;
        }

        // ==========================
        // LOAD FROM DB
        // ==========================
        private void EseriYukle()
        {
            if (_eserId == 0) { this.Close(); return; }

            try
            {
                _baglanti.Open();

                using (var cmd = new MySqlCommand(@"
SELECT 
    e.Baslik, e.SanatciAdi, e.Aciklama, e.Kategori, e.Teknik, e.Boyut, e.Agirlik, e.Fiyat, e.YapimYili, e.Resim,
    s.SergiAdi, s.Durum AS SergiDurumu,
    EXISTS(
        SELECT 1 FROM satislar sa 
        WHERE sa.EserID = e.EserID 
          AND (sa.SatisDurumu='Satıldı' OR sa.SatisDurumu='Tamamlandi' OR sa.SatisDurumu='Tamamlandı')
        LIMIT 1
    ) AS SatildiMi
FROM eserler e
LEFT JOIN sergiler s ON s.SergiID = e.SergiID
WHERE e.EserID=@id
LIMIT 1;", _baglanti))
                {
                    cmd.Parameters.AddWithValue("@id", _eserId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) { this.Close(); return; }

                        if (txtEserAdi != null) txtEserAdi.Text = dr["Baslik"]?.ToString() ?? "";
                        if (txtSanatci != null) txtSanatci.Text = dr["SanatciAdi"]?.ToString() ?? "";
                        if (txtAciklama != null) txtAciklama.Text = dr["Aciklama"]?.ToString() ?? "";
                        if (txtTeknik != null) txtTeknik.Text = dr["Teknik"]?.ToString() ?? "";

                        EnsureKategoriFromDb(dr["Kategori"]?.ToString() ?? "");

                        if (txtBoyut != null)
                            txtBoyut.Text = (dr["Boyut"] == DBNull.Value ? "" : dr["Boyut"].ToString());

                        if (txtAgirlik != null)
                        {
                            var agirlikDb = (dr["Agirlik"] == DBNull.Value ? "" : dr["Agirlik"].ToString());
                            if (TryParseDecimal(agirlikDb, out var ag))
                                txtAgirlik.Text = ag.ToString("0.###", CultureInfo.InvariantCulture) + " kg";
                            else
                                txtAgirlik.Text = agirlikDb;
                        }

                        if (txtFiyat != null)
                        {
                            decimal fiyat = dr["Fiyat"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Fiyat"]);
                            txtFiyat.Text = fiyat.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
                        }

                        if (dtYapimYili != null && dr["YapimYili"] != DBNull.Value)
                        {
                            // DB türü DATE/DATETIME ise düzgün oturur
                            try { dtYapimYili.Value = Convert.ToDateTime(dr["YapimYili"]); }
                            catch { dtYapimYili.Value = DateTime.Now; }
                        }

                        if (pictureBox1 != null && dr["Resim"] != DBNull.Value)
                        {
                            try
                            {
                                byte[] b = (byte[])dr["Resim"];
                                using (var ms = new MemoryStream(b))
                                using (var img = Image.FromStream(ms))
                                    pictureBox1.Image = new Bitmap(img);
                            }
                            catch { pictureBox1.Image = null; }
                        }

                        bool satildi = false;
                        try { satildi = Convert.ToInt32(dr["SatildiMi"]) == 1; } catch { }

                        if (txtStokDurumu != null) txtStokDurumu.Text = satildi ? "Stokta Değil" : "Stokta";
                        if (txtEserDurumu != null) txtEserDurumu.Text = satildi ? "Satıldı" : "Satışta";

                        if (txtSergi != null) txtSergi.Text = dr["SergiAdi"] == DBNull.Value ? "" : dr["SergiAdi"].ToString();
                        if (txtDurumu != null) txtDurumu.Text = dr["SergiDurumu"] == DBNull.Value ? "" : dr["SergiDurumu"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eser yüklenirken hata:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            finally
            {
                if (_baglanti.State == ConnectionState.Open)
                    _baglanti.Close();
            }
        }

        // ==========================
        // SAVE
        // ==========================
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            var eserAdi = (txtEserAdi?.Text ?? "").Trim();
            var sanatci = (txtSanatci?.Text ?? "").Trim();
            var teknik = (txtTeknik?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(eserAdi) || !RX_ONLY_LETTERS.IsMatch(eserAdi))
            {
                MessageBox.Show("Eser adı boş olamaz ve sadece harf içermelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEserAdi?.Focus();
                txtEserAdi?.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(sanatci) || !RX_ONLY_LETTERS.IsMatch(sanatci))
            {
                MessageBox.Show("Sanatçı adı boş olamaz ve sadece harf içermelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSanatci?.Focus();
                txtSanatci?.SelectAll();
                return;
            }

            if (!string.IsNullOrWhiteSpace(teknik) && !RX_ONLY_LETTERS.IsMatch(teknik))
            {
                MessageBox.Show("Teknik alanı sadece harf içermelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTeknik?.Focus();
                txtTeknik?.SelectAll();
                return;
            }

            if (!TryParseDecimal(txtFiyat?.Text, out decimal fiyat) || fiyat < MIN_FIYAT_TL || fiyat > MAX_FIYAT_TL)
            {
                MessageBox.Show($"Fiyat geçersiz. ({MIN_FIYAT_TL}-{MAX_FIYAT_TL})", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFiyat?.Focus();
                txtFiyat?.SelectAll();
                return;
            }

            if (!TryParseDecimal(txtAgirlik?.Text, out decimal agirlik) || agirlik < MIN_AGIRLIK_KG || agirlik > MAX_AGIRLIK_KG)
            {
                MessageBox.Show($"Ağırlık geçersiz. ({MIN_AGIRLIK_KG}-{MAX_AGIRLIK_KG})", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAgirlik?.Focus();
                txtAgirlik?.SelectAll();
                return;
            }

            if (!TryParseBoyutCm(txtBoyut?.Text, out decimal w, out decimal h))
                return;

            if (!IsBetween(w, MIN_BOYUT_CM, MAX_BOYUT_CM) || !IsBetween(h, MIN_BOYUT_CM, MAX_BOYUT_CM))
            {
                MessageBox.Show($"Boyut geçersiz. Her kenar {MIN_BOYUT_CM}-{MAX_BOYUT_CM} cm aralığında olmalı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBoyut?.Focus();
                txtBoyut?.SelectAll();
                return;
            }

            if (dtYapimYili != null)
            {
                int yil = dtYapimYili.Value.Year;
                if (yil < MIN_YIL || yil > MAX_YIL)
                {
                    MessageBox.Show($"Yapım yılı {MIN_YIL}-{MAX_YIL} arası olmalı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtYapimYili.Focus();
                    return;
                }
            }

            var ok = MessageBox.Show("Eser bilgilerini güncellemek istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ok != DialogResult.Yes) return;

            try
            {
                _baglanti.Open();

                var boyutStr =
                    $"{w.ToString("0.##", CultureInfo.InvariantCulture)} x {h.ToString("0.##", CultureInfo.InvariantCulture)} cm";

                using (var cmd = new MySqlCommand(@"
UPDATE eserler SET
  Baslik=@b,
  SanatciAdi=@s,
  Aciklama=@a,
  Kategori=@k,
  Teknik=@t,
  Boyut=@bo,
  Agirlik=@ag,
  Fiyat=@f,
  YapimYili=@y
WHERE EserID=@id;", _baglanti))
                {
                    cmd.Parameters.AddWithValue("@b", eserAdi);
                    cmd.Parameters.AddWithValue("@s", sanatci);
                    cmd.Parameters.AddWithValue("@a", (txtAciklama?.Text ?? "").Trim());
                    cmd.Parameters.AddWithValue("@k", SelectedKategori());
                    cmd.Parameters.AddWithValue("@t", teknik);
                    cmd.Parameters.AddWithValue("@bo", boyutStr);
                    cmd.Parameters.AddWithValue("@ag", agirlik);
                    cmd.Parameters.AddWithValue("@f", fiyat);
                    cmd.Parameters.AddWithValue("@y", dtYapimYili != null ? dtYapimYili.Value : DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", _eserId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Güncellendi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close(); // ShowDialog zinciri bozulmaz
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_baglanti.State == ConnectionState.Open)
                    _baglanti.Close();
            }
        }

        // ==========================
        // CANCEL / BUTTONS
        // ==========================
        private void btnIptal_Click(object sender, EventArgs e) => Close();
        private void iptalButton_Click(object sender, EventArgs e) => Close();
        private void tamamButton_Click(object sender, EventArgs e) => btnKaydet_Click(sender, e);

        // =======================================================
        // DESIGNER EVENT STUB'ları (CS1061 olmaması için)
        // =======================================================
        private void GalericiEserDetayDuzen_FormClosing(object sender, FormClosingEventArgs e) { }
        private void KategoriComboBox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label26_Click(object sender, EventArgs e) { }

        // Designer eski isimlerle event bağladıysa köprü:
        private void textBoxFiyat_KeyPress(object sender, KeyPressEventArgs e) { Decimal_KeyPress(sender, e); }
        private void textBoxAgirlik_KeyPress(object sender, KeyPressEventArgs e) { Decimal_KeyPress(sender, e); }
        private void textBoxBoyut_Leave(object sender, EventArgs e) { txtBoyut_Leave(sender, e); }
        private void textBoxFiyat_Leave(object sender, EventArgs e) { txtFiyat_Leave(sender, e); }
        private void textBoxAgirlik_Leave(object sender, EventArgs e) { txtAgirlik_Leave(sender, e); }
    }
}
