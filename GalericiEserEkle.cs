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
    public partial class GalericiEserEkle : Form
    {
        private readonly int _sergiId;
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private bool _dirty = false;
        private bool _closingByCode = false;
        private byte[] _selectedImageBytes = null;

        private const int MAX_IMAGE_BYTES = 2 * 1024 * 1024;

        private const decimal MIN_FIYAT = 1m;
        private const decimal MAX_FIYAT = 10_000m;

        private const decimal MIN_BOYUT_CM = 1m;
        private const decimal MAX_BOYUT_CM = 500m;

        private const decimal MIN_AGIRLIK_KG = 0.01m;
        private const decimal MAX_AGIRLIK_KG = 500m;

        private const int MIN_YEAR = 1500;

        private static readonly Regex NameLikeRegex =
            new Regex(@"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,80}$", RegexOptions.Compiled);

        private static readonly Regex BoyutRegex =
            new Regex(@"^\s*\d+([.,]\d+)?\s*[xX]\s*\d+([.,]\d+)?\s*$", RegexOptions.Compiled);

        // TL/kg/cm gibi birimleri (case-insensitive) temizlemek için
        private static readonly Regex UnitRegex =
            new Regex(@"\b(₺|tl|kg|cm)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Rakam/virgül/nokta ve eksi harici karakterleri atmak için
        private static readonly Regex NonNumericRegex =
            new Regex(@"[^\d\.,\-]", RegexOptions.Compiled);

        public GalericiEserEkle(int sergiId, int galericiId)
        {
            InitializeComponent();
            _sergiId = sergiId;
            _galericiId = galericiId;

            Load -= GalericiEserEkle_Load;
            Load += GalericiEserEkle_Load;

            FormClosing -= GalericiEserEkle_FormClosing;
            FormClosing += GalericiEserEkle_FormClosing;

            HookEvents();
        }

        public GalericiEserEkle(int sergiId) : this(sergiId, 0) { }
        public GalericiEserEkle() : this(0, 0) { }

        private void GalericiEserEkle_Load(object sender, EventArgs e)
        {
            var ok = MessageBox.Show(
                "Yeni eser eklemek istediğinize emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (ok != DialogResult.Yes)
            {
                _closingByCode = true;
                Close();
                return;
            }

            SetupKategoriVeTeknik();

            if (pictureBox1 != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Cursor = Cursors.Hand;
            }
        }

        private void SetupKategoriVeTeknik()
        {
            if (cmbKategori != null)
            {
                cmbKategori.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbKategori.Items.Clear();
                cmbKategori.Items.AddRange(new object[]
                {
                    "Resim","Heykel","Fotoğraf","Dijital Sanat","Seramik",
                    "Enstalasyon","Karışık Teknik","Gravür"
                });
                if (cmbKategori.Items.Count > 0) cmbKategori.SelectedIndex = 0;
            }

            if (cmbTeknik != null)
            {
                cmbTeknik.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbTeknik.Items.Clear();
                cmbTeknik.Items.AddRange(new object[]
                {
                    "Yağlı Boya","Akrilik","Suluboya","Karakalem",
                    "Dijital Baskı","Bronz","Mermer","Karışık Teknik"
                });
                if (cmbTeknik.Items.Count > 0) cmbTeknik.SelectedIndex = 0;
            }
        }

        // ===================== EVENTS =====================
        private void HookEvents()
        {
            if (textBoxEserAdi != null) textBoxEserAdi.TextChanged += MarkDirty;
            if (textBoxSanatci != null) textBoxSanatci.TextChanged += MarkDirty;
            if (textBoxAciklama != null) textBoxAciklama.TextChanged += MarkDirty;
            if (textBoxBoyut != null) textBoxBoyut.TextChanged += MarkDirty;
            if (textBoxAgirlik != null) textBoxAgirlik.TextChanged += MarkDirty;
            if (textBoxFiyat != null) textBoxFiyat.TextChanged += MarkDirty;

            if (cmbKategori != null) cmbKategori.SelectedIndexChanged += MarkDirty;
            if (cmbTeknik != null) cmbTeknik.SelectedIndexChanged += MarkDirty;

            if (textBoxFiyat != null)
            {
                textBoxFiyat.KeyPress -= textBoxFiyat_KeyPress;
                textBoxFiyat.KeyPress += textBoxFiyat_KeyPress;

                textBoxFiyat.Leave -= textBoxFiyat_Leave;
                textBoxFiyat.Leave += textBoxFiyat_Leave;
            }

            if (textBoxAgirlik != null)
            {
                textBoxAgirlik.KeyPress -= DecimalOnly_KeyPress;
                textBoxAgirlik.KeyPress += DecimalOnly_KeyPress;

                textBoxAgirlik.Leave -= textBoxAgirlik_Leave;
                textBoxAgirlik.Leave += textBoxAgirlik_Leave;
            }

            if (textBoxBoyut != null)
            {
                textBoxBoyut.KeyPress -= Boyut_KeyPress;
                textBoxBoyut.KeyPress += Boyut_KeyPress;

                textBoxBoyut.Leave -= textBoxBoyut_Leave;
                textBoxBoyut.Leave += textBoxBoyut_Leave;
            }

            if (pictureBox1 != null)
            {
                pictureBox1.Click -= pictureBox1_Click;
                pictureBox1.Click += pictureBox1_Click;
            }

            if (ekleButton != null)
            {
                ekleButton.Click -= ekleButton_Click;
                ekleButton.Click += ekleButton_Click;
            }

            if (iptalButton != null)
            {
                iptalButton.Click -= iptalButton_Click;
                iptalButton.Click += iptalButton_Click;
            }

            if (btnResmiKaldir != null)
            {
                btnResmiKaldir.Click -= btnResmiKaldir_Click;
                btnResmiKaldir.Click += btnResmiKaldir_Click;
            }

            if (lblDashboard != null) { lblDashboard.Click -= lblDashboard_Click; lblDashboard.Click += lblDashboard_Click; }
            if (lblSergiler != null) { lblSergiler.Click -= lblSergiler_Click; lblSergiler.Click += lblSergiler_Click; }
            if (lblSergiciTanimlama != null) { lblSergiciTanimlama.Click -= lblSergiciTanimlama_Click; lblSergiciTanimlama.Click += lblSergiciTanimlama_Click; }
            if (lblMusteriBilgileri != null) { lblMusteriBilgileri.Click -= lblMusteriBilgileri_Click; lblMusteriBilgileri.Click += lblMusteriBilgileri_Click; }
            if (lblRaporEkrani != null) { lblRaporEkrani.Click -= lblRaporEkrani_Click; lblRaporEkrani.Click += lblRaporEkrani_Click; }
            if (lblCikisYap != null) { lblCikisYap.Click -= lblCikisYap_Click; lblCikisYap.Click += lblCikisYap_Click; }

            if (lblHakkimizda != null) { lblHakkimizda.Click -= lblHakkimizda_Click; lblHakkimizda.Click += lblHakkimizda_Click; }
            if (lblYardim != null) { lblYardim.Click -= lblYardim_Click; lblYardim.Click += lblYardim_Click; }
        }

        private void MarkDirty(object sender, EventArgs e) => _dirty = true;

        // ===================== ORTAK NAV =====================
        private bool ConfirmLoseChangesIfNeeded()
        {
            if (!_dirty) return true;

            var dr = MessageBox.Show(
                "Kaydedilmemiş değişiklikler kaybolacak. Devam edilsin mi?",
                "Uyarı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return dr == DialogResult.Yes;
        }

        private void NavigateAndClose(Form next)
        {
            if (!ConfirmLoseChangesIfNeeded()) return;

            _closingByCode = true;
            next?.Show();
            Close();
        }

        private void ReturnToSergiler()
        {
            if (_galericiId > 0)
                NavigateAndClose(new GalericiSergiler(_galericiId));
            else
            {
                _closingByCode = true;
                Close();
            }
        }

        // ===================== RESIM =====================
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JPG / PNG|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                FileInfo fi = new FileInfo(ofd.FileName);
                if (fi.Length > MAX_IMAGE_BYTES)
                {
                    MessageBox.Show("Resim 2MB'tan büyük olamaz.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedImageBytes = File.ReadAllBytes(ofd.FileName);

                if (pictureBox1.Image != null)
                {
                    var old = pictureBox1.Image;
                    pictureBox1.Image = null;
                    old.Dispose();
                }

                pictureBox1.Image = Image.FromFile(ofd.FileName);
                _dirty = true;
            }
        }

        // ===================== EKLE =====================
        private void ekleButton_Click(object sender, EventArgs e)
        {
            var ok = MessageBox.Show("Eseri eklemek istediğinize emin misiniz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (ok != DialogResult.Yes) return;

            if (!ValidateInputs(out decimal fiyat, out string boyutDb, out string agirlikDb))
                return;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(@"
INSERT INTO eserler
(Baslik,SanatciAdi,Aciklama,Kategori,Teknik,Boyut,Agirlik,
 Fiyat,YapimYili,Durum,SatisdaAcikMi,SergiID,Resim)
VALUES
(@b,@s,@a,@k,@t,@bo,@ag,@f,@y,1,1,@sid,@r);", conn))
                    {
                        cmd.Parameters.AddWithValue("@b", (textBoxEserAdi?.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@s", (textBoxSanatci?.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@a", (textBoxAciklama?.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@k", cmbKategori?.SelectedItem?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@t", cmbTeknik?.SelectedItem?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@bo", boyutDb);
                        cmd.Parameters.AddWithValue("@ag", agirlikDb);
                        cmd.Parameters.AddWithValue("@f", fiyat);
                        cmd.Parameters.AddWithValue("@y", dtyapimyili.Value.Date);
                        cmd.Parameters.AddWithValue("@sid", _sergiId);

                        cmd.Parameters.Add("@r", MySqlDbType.LongBlob).Value =
                            _selectedImageBytes ?? (object)DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Eser eklendi.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                _dirty = false;
                ReturnToSergiler();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eser ekleme sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== VALIDATION =====================
        private bool ValidateInputs(out decimal fiyat, out string boyutDb, out string agirlikDb)
        {
            fiyat = 0m;
            boyutDb = "";
            agirlikDb = "";

            string eserAdi = (textBoxEserAdi?.Text ?? "").Trim();
            string sanatci = (textBoxSanatci?.Text ?? "").Trim();
            string boyutInput = (textBoxBoyut?.Text ?? "").Trim();
            string agirlikInput = (textBoxAgirlik?.Text ?? "").Trim();
            string fiyatInput = (textBoxFiyat?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(eserAdi))
            {
                MessageBox.Show("Eser adı boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEserAdi?.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(sanatci) || !NameLikeRegex.IsMatch(sanatci))
            {
                MessageBox.Show("Sanatçı adı geçersiz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxSanatci?.Focus();
                return false;
            }

            if (!TryParseDecimalTR(fiyatInput, out fiyat) || fiyat < MIN_FIYAT || fiyat > MAX_FIYAT)
            {
                MessageBox.Show($"Fiyat geçersiz. {MIN_FIYAT:0.00} TL - {MAX_FIYAT:0.00} TL aralığında olmalıdır.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxFiyat?.Focus();
                return false;
            }

            if (!BoyutRegex.IsMatch(boyutInput))
            {
                MessageBox.Show("Boyut formatı hatalı. Örnek: 50x70 (cm)", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxBoyut?.Focus();
                return false;
            }

            ParseBoyut(boyutInput, out decimal en, out decimal boy);

            if (en < MIN_BOYUT_CM || en > MAX_BOYUT_CM || boy < MIN_BOYUT_CM || boy > MAX_BOYUT_CM)
            {
                MessageBox.Show($"Boyut aralığı geçersiz. {MIN_BOYUT_CM:0.##} - {MAX_BOYUT_CM:0.##} cm aralığında olmalı.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxBoyut?.Focus();
                return false;
            }

            boyutDb = $"{en:0.##}x{boy:0.##} cm";

            if (!TryParseDecimalTR(agirlikInput, out decimal agKg) || agKg < MIN_AGIRLIK_KG || agKg > MAX_AGIRLIK_KG)
            {
                MessageBox.Show($"Ağırlık geçersiz. {MIN_AGIRLIK_KG:0.##} - {MAX_AGIRLIK_KG:0.##} kg aralığında olmalı.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAgirlik?.Focus();
                return false;
            }

            agirlikDb = $"{agKg:0.###} kg";

            int y = dtyapimyili.Value.Year;
            if (y < MIN_YEAR || y > DateTime.Now.Year)
            {
                MessageBox.Show("Yapım yılı geçersiz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ParseBoyut(string input, out decimal en, out decimal boy)
        {
            en = 0m; boy = 0m;

            string s = (input ?? "").Trim().ToLowerInvariant();
            s = s.Replace(" ", "");
            s = s.Replace('X', 'x');

            var parts = s.Split('x');
            if (parts.Length != 2) return;

            TryParseDecimalTR(parts[0], out en);
            TryParseDecimalTR(parts[1], out boy);
        }

        // ✅ CS1501 fix burada: Replace(…, …, StringComparison…) yok.
        private bool TryParseDecimalTR(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string s = input.Trim();

            // Birimleri temizle (case-insensitive)
            s = UnitRegex.Replace(s, "");

            // Gereksiz karakterleri temizle
            s = NonNumericRegex.Replace(s, "");

            s = s.Trim();
            if (string.IsNullOrWhiteSpace(s)) return false;

            // 1.234,56 -> 1234.56
            if (s.Contains(".") && s.Contains(","))
                s = s.Replace(".", "");

            // TR virgül -> nokta
            s = s.Replace(',', '.');

            return decimal.TryParse(
                s,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value);
        }

        // ===================== IPTAL / KAPANIŞ =====================
        private void iptalButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "İptal etmek istiyor musunuz?\nKaydedilmemiş bilgiler silinecek.",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _dirty = false;
            ReturnToSergiler();
        }

        private void GalericiEserEkle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;

            if (_dirty)
            {
                var dr = MessageBox.Show(
                    "Kaydedilmemiş değişiklikler kaybolacak. Çıkılsın mı?",
                    "Uyarı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        // ===================== NUMERIC INPUT =====================
        private void textBoxFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            var tb = sender as TextBox;
            if (tb != null && (e.KeyChar == ',' || e.KeyChar == '.'))
            {
                if (tb.Text.Contains(",") || tb.Text.Contains("."))
                    e.Handled = true;
            }
        }

        private void textBoxFiyat_Leave(object sender, EventArgs e)
        {
            if (textBoxFiyat == null) return;
            if (TryParseDecimalTR(textBoxFiyat.Text, out decimal v))
                textBoxFiyat.Text = v.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void DecimalOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            var tb = sender as TextBox;
            if (tb != null && (e.KeyChar == ',' || e.KeyChar == '.'))
            {
                if (tb.Text.Contains(",") || tb.Text.Contains("."))
                    e.Handled = true;
            }
        }

        private void textBoxAgirlik_Leave(object sender, EventArgs e)
        {
            if (textBoxAgirlik == null) return;
            if (TryParseDecimalTR(textBoxAgirlik.Text, out decimal v))
                textBoxAgirlik.Text = v.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private void Boyut_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' && e.KeyChar != '.' &&
                e.KeyChar != 'x' && e.KeyChar != 'X')
            {
                e.Handled = true;
                return;
            }
        }

        private void textBoxBoyut_Leave(object sender, EventArgs e)
        {
            if (textBoxBoyut == null) return;

            string s = (textBoxBoyut.Text ?? "").Trim();
            if (!BoyutRegex.IsMatch(s)) return;

            ParseBoyut(s, out decimal en, out decimal boy);
            if (en > 0 && boy > 0)
                textBoxBoyut.Text = $"{en:0.##}x{boy:0.##}";
        }

        // ===================== MENU =====================
        private void lblDashboard_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiDashboard(_galericiId));
        private void lblSergiler_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiler(_galericiId));
        private void lblSergiciTanimlama_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiciTanim(_galericiId));
        private void lblMusteriBilgileri_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiMusteriBilgileri(_galericiId));
        private void lblRaporEkrani_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiRaporlar(_galericiId));

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _dirty = false;
            _closingByCode = true;
            new Form1().Show();
            Close();
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            new Yardim().ShowDialog();
        }

        // ===================== RESMİ KALDIR =====================
        private void btnResmiKaldir_Click(object sender, EventArgs e)
        {
            if (_selectedImageBytes == null && (pictureBox1 == null || pictureBox1.Image == null))
            {
                MessageBox.Show("Kaldırılacak bir resim yok.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dr = MessageBox.Show(
                "Seçili resmi kaldırmak istiyor musunuz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _selectedImageBytes = null;

            if (pictureBox1 != null && pictureBox1.Image != null)
            {
                var oldImg = pictureBox1.Image;
                pictureBox1.Image = null;
                oldImg.Dispose();
            }

            _dirty = true;

            MessageBox.Show("Resim kaldırıldı.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // =======================================================
        // DESIGNER KÖPRÜ STUB’LAR (Designer farklı isim bağladıysa)
        // =======================================================
        private void textBoxAgirlik_KeyPress(object sender, KeyPressEventArgs e) => DecimalOnly_KeyPress(sender, e);
        private void textBoxBoyut_KeyPress(object sender, KeyPressEventArgs e) => Boyut_KeyPress(sender, e);
    }
}
