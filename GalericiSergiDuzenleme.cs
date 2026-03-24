using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiDuzenleme : Form
    {
        private readonly int _galericiId;
        private readonly int _sergiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // Kurallar
        private const int MIN_KAPASITE = 1;
        private const int MAX_KAPASITE = 1000;

        private const int MIN_ESER = 0;
        private const int MAX_ESER = 10000;

        private const decimal MIN_KIRA = 1m;
        private const decimal MAX_KIRA = 500000m;

        private const decimal MIN_ESER_BASI = 1m;
        private const decimal MAX_ESER_BASI = 200000m;

        // Tarih kısıtı (çok uzak olmasın)
        // İstersen değiştir: bugünden 5 yıl ileri, 5 yıl geri gibi.
        private const int MIN_YEAR = 2000;
        private const int MAX_YEAR_FROM_NOW = 5;

        private bool _syncLock = false;
        private bool _closingByCode = false;

        // Sadece harf (Türkçe dahil)
        private static readonly Regex OnlyLettersRegex =
            new Regex(@"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,80}$", RegexOptions.Compiled);

        public GalericiSergiDuzenleme()
        {
            InitializeComponent();
        }

        // ✅ Grid’den açacağın constructor
        public GalericiSergiDuzenleme(int galericiId, int sergiId) : this()
        {
            _galericiId = galericiId;
            _sergiId = sergiId;

            Load -= GalericiSergiDuzenleme_Load;
            Load += GalericiSergiDuzenleme_Load;
        }

        private void GalericiSergiDuzenleme_Load(object sender, EventArgs e)
        {
            BindCriticalEvents();

            // ComboBox sadece seçim
            if (cmbSergiTuru != null) cmbSergiTuru.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbTema != null) cmbTema.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbHedefKitle != null) cmbHedefKitle.DropDownStyle = ComboBoxStyle.DropDownList;

            // DB’den doldur
            LoadSergi();

            // toplamı yaz
            RecalcTotal();
        }

        // =========================================================
        // EVENTLERİ GARANTİ BAĞLA (Designer koparsa bile)
        // =========================================================
        private void BindCriticalEvents()
        {
            // Numeric input kısıtları
            if (txtKapasite != null)
            {
                txtKapasite.KeyPress -= OnlyDigit_KeyPress;
                txtKapasite.KeyPress += OnlyDigit_KeyPress;
            }

            if (txtEserSayisi != null)
            {
                txtEserSayisi.KeyPress -= OnlyDigit_KeyPress;
                txtEserSayisi.KeyPress += OnlyDigit_KeyPress;

                txtEserSayisi.TextChanged -= AnyCost_TextChanged;
                txtEserSayisi.TextChanged += AnyCost_TextChanged;
            }

            if (txtGaleriKirasi != null)
            {
                txtGaleriKirasi.KeyPress -= Decimal_KeyPress;
                txtGaleriKirasi.KeyPress += Decimal_KeyPress;

                txtGaleriKirasi.TextChanged -= AnyCost_TextChanged;
                txtGaleriKirasi.TextChanged += AnyCost_TextChanged;

                txtGaleriKirasi.Leave -= Cost_LeaveFormatTL;
                txtGaleriKirasi.Leave += Cost_LeaveFormatTL;
            }

            if (txtEserBasiUcret != null)
            {
                txtEserBasiUcret.KeyPress -= Decimal_KeyPress;
                txtEserBasiUcret.KeyPress += Decimal_KeyPress;

                txtEserBasiUcret.TextChanged -= AnyCost_TextChanged;
                txtEserBasiUcret.TextChanged += AnyCost_TextChanged;

                txtEserBasiUcret.Leave -= Cost_LeaveFormatTL;
                txtEserBasiUcret.Leave += Cost_LeaveFormatTL;
            }

            // Butonlar
            if (btnDuzenle != null)
            {
                btnDuzenle.Click -= btnDuzenle_Click;
                btnDuzenle.Click += btnDuzenle_Click;
            }

            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }

            // Tarih değişimi
            if (dtpBaslangic != null)
            {
                dtpBaslangic.ValueChanged -= dtp_ValueChanged;
                dtpBaslangic.ValueChanged += dtp_ValueChanged;
            }

            if (dtpBitis != null)
            {
                dtpBitis.ValueChanged -= dtp_ValueChanged;
                dtpBitis.ValueChanged += dtp_ValueChanged;
            }

            // Menü
            if (lblDashboard != null) { lblDashboard.Click -= lblDashboard_Click; lblDashboard.Click += lblDashboard_Click; }
            if (lblSergiciTanimlama != null) { lblSergiciTanimlama.Click -= lblSergiciTanimlama_Click; lblSergiciTanimlama.Click += lblSergiciTanimlama_Click; }
            if (lblMusteriBilgileri != null) { lblMusteriBilgileri.Click -= lblMusteriBilgileri_Click; lblMusteriBilgileri.Click += lblMusteriBilgileri_Click; }
            if (lblSergiler != null) { lblSergiler.Click -= lblSergiler_Click; lblSergiler.Click += lblSergiler_Click; }
            if (lblRaporEkrani != null) { lblRaporEkrani.Click -= lblRaporEkrani_Click; lblRaporEkrani.Click += lblRaporEkrani_Click; }
            if (lblGalericiAdi != null) { lblGalericiAdi.Click -= lblGalericiAdi_Click; lblGalericiAdi.Click += lblGalericiAdi_Click; }
            if (lblCikisYap != null) { lblCikisYap.Click -= lblCikisYap_Click; lblCikisYap.Click += lblCikisYap_Click; }

            // Footer
            if (lblHakkimizda != null) { lblHakkimizda.Click -= lblHakkimizda_Click; lblHakkimizda.Click += lblHakkimizda_Click; }
            if (lblYardim != null) { lblYardim.Click -= lblYardim_Click; lblYardim.Click += lblYardim_Click; }
        }

        // =========================================================
        // ORTAK NAV (tek pencere)
        // =========================================================
        private void NavigateAndClose(Form nextForm)
        {
            _closingByCode = true;
            if (nextForm != null) nextForm.Show();
            this.Close();
        }

        // =========================================================
        // DB: Sergiyi çek
        // =========================================================
        private void LoadSergi()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT
  SergiAdi, SergiTuru, SergiTemasi, HedefKitle,
  Kapasite, BaslangicTarihi, BitisTarihi,
  COALESCE(EserSayisi,0) AS EserSayisi,
  COALESCE(GaleriKirasi,0) AS GaleriKirasi,
  COALESCE(EserBasiUcret,0) AS EserBasiUcret,
  Sergici
FROM sergiler
WHERE SergiID=@id AND (@gid=0 OR GalericiID=@gid)
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _sergiId);
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Sergi bulunamadı.", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Close();
                                return;
                            }

                            _syncLock = true;

                            if (txtSergiAdi != null) txtSergiAdi.Text = dr["SergiAdi"]?.ToString() ?? "";
                            if (txtSergiSahibi != null) txtSergiSahibi.Text = dr["Sergici"]?.ToString() ?? "";

                            if (cmbSergiTuru != null) cmbSergiTuru.Text = dr["SergiTuru"]?.ToString() ?? "";
                            if (cmbTema != null) cmbTema.Text = dr["SergiTemasi"]?.ToString() ?? "";
                            if (cmbHedefKitle != null) cmbHedefKitle.Text = dr["HedefKitle"]?.ToString() ?? "";

                            if (txtKapasite != null) txtKapasite.Text = Convert.ToInt32(dr["Kapasite"]).ToString();

                            if (dtpBaslangic != null) dtpBaslangic.Value = Convert.ToDateTime(dr["BaslangicTarihi"]).Date;
                            if (dtpBitis != null) dtpBitis.Value = Convert.ToDateTime(dr["BitisTarihi"]).Date;

                            if (txtEserSayisi != null) txtEserSayisi.Text = Convert.ToInt32(dr["EserSayisi"]).ToString();

                            decimal kira = Convert.ToDecimal(dr["GaleriKirasi"]);
                            decimal eb = Convert.ToDecimal(dr["EserBasiUcret"]);

                            if (txtGaleriKirasi != null) txtGaleriKirasi.Text = kira == 0 ? "" : kira.ToString("0.00", CultureInfo.InvariantCulture);
                            if (txtEserBasiUcret != null) txtEserBasiUcret.Text = eb == 0 ? "" : eb.ToString("0.00", CultureInfo.InvariantCulture);

                            _syncLock = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergi bilgileri yüklenemedi:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        // =========================================================
        // INPUT FILTERS
        // =========================================================
        private void OnlyDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Decimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
                e.Handled = true;

            var tb = sender as TextBox;
            if (tb != null && (e.KeyChar == ',' || e.KeyChar == '.'))
            {
                if (tb.Text.Contains(",") || tb.Text.Contains("."))
                    e.Handled = true;
            }
        }

        private void AnyCost_TextChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;
            RecalcTotal();
        }

        private void Cost_LeaveFormatTL(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (TryParseDecimalTR(tb.Text, out decimal v) && v > 0)
                tb.Text = v.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void RecalcTotal()
        {
            if (lblToplamFiyat == null) return;

            int eser = 0;
            int.TryParse((txtEserSayisi?.Text ?? "").Trim(), out eser);

            decimal kira = 0m;
            TryParseDecimalTR(txtGaleriKirasi?.Text, out kira);

            decimal eb = 0m;
            TryParseDecimalTR(txtEserBasiUcret?.Text, out eb);

            decimal toplam = kira + (eser * eb);
            lblToplamFiyat.Text = $"{toplam:0.00} TL";
        }

        private bool TryParseDecimalTR(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string s = input.Trim()
                .Replace("₺", "")
                .Replace("TL", "")
                .Trim()
                .Replace(',', '.');

            return decimal.TryParse(s, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out value);
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            // İstersen burada da ek kontroller/hesaplar yapılabilir
        }

        // =========================================================
        // VALIDATION + UPDATE
        // =========================================================
        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            // ✅ Emin misin?
            var confirm = MessageBox.Show("Sergiyi güncellemek istediğinizden emin misiniz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            string sergiAdi = (txtSergiAdi?.Text ?? "").Trim();
            string sergiSahibi = (txtSergiSahibi?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(sergiAdi))
            {
                MessageBox.Show("Sergi adı boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSergiAdi?.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(sergiSahibi))
            {
                MessageBox.Show("Sergi sahibi boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSergiSahibi?.Focus();
                return;
            }

            if (!OnlyLettersRegex.IsMatch(sergiSahibi))
            {
                MessageBox.Show("Sergi sahibi sadece harflerden oluşmalıdır (rakam yasak).", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSergiSahibi?.Focus();
                return;
            }

            // combo sadece seçim
            if (cmbSergiTuru == null || cmbSergiTuru.SelectedIndex < 0)
            {
                MessageBox.Show("Sergi türü sadece listeden seçilmelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSergiTuru?.Focus();
                return;
            }

            if (cmbTema == null || cmbTema.SelectedIndex < 0)
            {
                MessageBox.Show("Sergi teması sadece listeden seçilmelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTema?.Focus();
                return;
            }

            if (cmbHedefKitle == null || cmbHedefKitle.SelectedIndex < 0)
            {
                MessageBox.Show("Hedef kitle sadece listeden seçilmelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbHedefKitle?.Focus();
                return;
            }

            if (!int.TryParse((txtKapasite?.Text ?? "").Trim(), out int kapasite) ||
                kapasite < MIN_KAPASITE || kapasite > MAX_KAPASITE)
            {
                MessageBox.Show($"Kapasite {MIN_KAPASITE} – {MAX_KAPASITE} arası olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKapasite?.Focus();
                return;
            }

            if (!int.TryParse((txtEserSayisi?.Text ?? "").Trim(), out int eserSayisi) ||
                eserSayisi < MIN_ESER || eserSayisi > MAX_ESER)
            {
                MessageBox.Show($"Eser sayısı {MIN_ESER} – {MAX_ESER} arası olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEserSayisi?.Focus();
                return;
            }

            if (!TryParseDecimalTR(txtGaleriKirasi?.Text, out decimal kira) || kira < MIN_KIRA || kira > MAX_KIRA)
            {
                MessageBox.Show($"Galeri kirası hatalı. {MIN_KIRA} – {MAX_KIRA} arası olmalı.\nOndalık için virgül veya nokta kullanabilirsiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGaleriKirasi?.Focus();
                return;
            }

            if (!TryParseDecimalTR(txtEserBasiUcret?.Text, out decimal eserBasi) || eserBasi < MIN_ESER_BASI || eserBasi > MAX_ESER_BASI)
            {
                MessageBox.Show($"Eser başı ücret hatalı. {MIN_ESER_BASI} – {MAX_ESER_BASI} arası olmalı.\nOndalık için virgül veya nokta kullanabilirsiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEserBasiUcret?.Focus();
                return;
            }

            DateTime bas = dtpBaslangic.Value.Date;
            DateTime bit = dtpBitis.Value.Date;

            // boşluk kontrolü: DateTimePicker boş olmaz ama yine de mantıksal kontrol
            if (bas == DateTime.MinValue || bit == DateTime.MinValue)
            {
                MessageBox.Show("Başlangıç ve bitiş tarihi seçilmelidir.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (bas > bit)
            {
                MessageBox.Show("Başlangıç tarihi bitiş tarihinden sonra olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Çok uzak tarih kontrolü
            int maxYear = DateTime.Now.Year + MAX_YEAR_FROM_NOW;
            if (bas.Year < MIN_YEAR || bit.Year < MIN_YEAR || bas.Year > maxYear || bit.Year > maxYear)
            {
                MessageBox.Show($"Tarih aralığı çok uzak. {MIN_YEAR} ile {maxYear} arasında olmalı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tarih çakışma (kendi sergisini hariç tut)
            if (HasDateConflictExceptSelf(bas, bit))
            {
                MessageBox.Show("Bu tarih aralığında başka bir sergi var!", "Tarih Çakışması",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal toplam = kira + (eserSayisi * eserBasi);

            // UPDATE
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
UPDATE sergiler
SET
  SergiAdi=@adi,
  SergiTuru=@tur,
  SergiTemasi=@tema,
  HedefKitle=@hedef,
  Sergici=@sahip,
  Kapasite=@kap,
  BaslangicTarihi=@bas,
  BitisTarihi=@bit,
  EserSayisi=@es,
  GaleriKirasi=@kira,
  EserBasiUcret=@eb,
  ToplamMaliyet=@top
WHERE SergiID=@id AND (@gid=0 OR GalericiID=@gid);";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@adi", sergiAdi);
                        cmd.Parameters.AddWithValue("@tur", (cmbSergiTuru.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@tema", (cmbTema.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@hedef", (cmbHedefKitle.Text ?? "").Trim());
                        cmd.Parameters.AddWithValue("@sahip", sergiSahibi);

                        cmd.Parameters.AddWithValue("@kap", kapasite);
                        cmd.Parameters.AddWithValue("@bas", bas);
                        cmd.Parameters.AddWithValue("@bit", bit);

                        cmd.Parameters.AddWithValue("@es", eserSayisi);
                        cmd.Parameters.AddWithValue("@kira", kira);
                        cmd.Parameters.AddWithValue("@eb", eserBasi);
                        cmd.Parameters.AddWithValue("@top", toplam);

                        cmd.Parameters.AddWithValue("@id", _sergiId);
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        int affected = cmd.ExecuteNonQuery();
                        if (affected <= 0)
                        {
                            MessageBox.Show("Güncelleme başarısız (kayıt bulunamadı).", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                MessageBox.Show("Sergi güncellendi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool HasDateConflictExceptSelf(DateTime bas, DateTime bit)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT COUNT(*)
FROM sergiler
WHERE GalericiID=@gid
  AND SergiID <> @id
  AND NOT (BitisTarihi < @bas OR BaslangicTarihi > @bit);";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.Parameters.AddWithValue("@id", _sergiId);
                        cmd.Parameters.AddWithValue("@bas", bas);
                        cmd.Parameters.AddWithValue("@bit", bit);
                        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                // DB hatasında riske girmeyelim
                return true;
            }
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Kaydetmeden çıkmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            Close();
        }

        // =========================================================
        // MENÜ GEÇİŞLERİ (tek pencere)
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

            if (dr != DialogResult.Yes) return;

            NavigateAndClose(new Form1());
        }

        // =========================================================
        // FOOTER
        // =========================================================
        private void lblHakkimizda_Click(object sender, EventArgs e) => new Hakkimizda().ShowDialog();
        private void lblYardim_Click(object sender, EventArgs e) => new Yardim().ShowDialog();

        // Designer boş event (gerekirse)
        private void label1_Click(object sender, EventArgs e) { }
    }
}
