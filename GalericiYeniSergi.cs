using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiYeniSergi : Form
    {
        private const int MIN_KAPASITE = 1;
        private const int MAX_KAPASITE = 1000;

        private readonly int _galericiId;
        private readonly YeniSergiBilgisi _model;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // Sergi adı: Türkçe harf + boşluk + ' - . (2-80)
        private static readonly Regex SergiAdiRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,80}$",
            RegexOptions.Compiled);

        // ====== Dirty tracking ======
        private bool _dirty = false;
        private bool _closingByCode = false;

        public GalericiYeniSergi(int galericiId, YeniSergiBilgisi model)
        {
            InitializeComponent();
            _galericiId = galericiId;
            _model = model ?? new YeniSergiBilgisi { GalericiId = galericiId };

            HookEvents();
        }

        public GalericiYeniSergi(int galericiId)
            : this(galericiId, new YeniSergiBilgisi { GalericiId = galericiId })
        { }

        public GalericiYeniSergi() : this(0) { }

        private void HookEvents()
        {
            // Load garanti
            this.Load -= GalericiYeniSergi_Load;
            this.Load += GalericiYeniSergi_Load;

            // FormClosing: X ile kapanırken de uyarı
            this.FormClosing -= GalericiYeniSergi_FormClosing;
            this.FormClosing += GalericiYeniSergi_FormClosing;

            // Kapasite sadece digit
            if (txtKapasite != null)
            {
                txtKapasite.KeyPress -= TxtKapasite_KeyPress;
                txtKapasite.KeyPress += TxtKapasite_KeyPress;
            }

            // Butonlar
            if (btnIleri != null)
            {
                btnIleri.Click -= btnIleri_Click;
                btnIleri.Click += btnIleri_Click;
            }

            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }

            // Dirty tracking (kullanıcı değiştirince)
            if (txtSergiAdi != null) { txtSergiAdi.TextChanged -= MarkDirty; txtSergiAdi.TextChanged += MarkDirty; }
            if (txtKapasite != null) { txtKapasite.TextChanged -= MarkDirty; txtKapasite.TextChanged += MarkDirty; }

            if (comboBoxSergiTuru != null) { comboBoxSergiTuru.SelectedIndexChanged -= MarkDirty; comboBoxSergiTuru.SelectedIndexChanged += MarkDirty; }
            if (comboBoxTema != null) { comboBoxTema.SelectedIndexChanged -= MarkDirty; comboBoxTema.SelectedIndexChanged += MarkDirty; }
            if (comboBoxHedefKitle != null) { comboBoxHedefKitle.SelectedIndexChanged -= MarkDirty; comboBoxHedefKitle.SelectedIndexChanged += MarkDirty; }

            // Menü eventleri (garanti bağla)
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

        private void GalericiYeniSergi_Load(object sender, EventArgs e)
        {
            // ComboBox’lar sadece seçim
            if (comboBoxSergiTuru != null) comboBoxSergiTuru.DropDownStyle = ComboBoxStyle.DropDownList;
            if (comboBoxTema != null) comboBoxTema.DropDownStyle = ComboBoxStyle.DropDownList;
            if (comboBoxHedefKitle != null) comboBoxHedefKitle.DropDownStyle = ComboBoxStyle.DropDownList;

            // Sergici modelden otomatik doldur ve kilitle
            AutoFillSergiciFromModel();

            // Modelden geri yükleme
            if (txtSergiAdi != null && string.IsNullOrWhiteSpace(txtSergiAdi.Text))
                txtSergiAdi.Text = _model.SergiAdi ?? "";

            if (txtKapasite != null && string.IsNullOrWhiteSpace(txtKapasite.Text) && _model.Kapasite > 0)
                txtKapasite.Text = _model.Kapasite.ToString();

            if (comboBoxSergiTuru != null && comboBoxSergiTuru.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(_model.SergiTuru))
                comboBoxSergiTuru.SelectedItem = _model.SergiTuru;

            if (comboBoxTema != null && comboBoxTema.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(_model.SergiTemasi))
                comboBoxTema.SelectedItem = _model.SergiTemasi;

            if (comboBoxHedefKitle != null && comboBoxHedefKitle.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(_model.HedefKitle))
                comboBoxHedefKitle.SelectedItem = _model.HedefKitle;

            // İlk yüklemede dirty sayma
            _dirty = false;
        }

        private void MarkDirty(object sender, EventArgs e)
        {
            if (_closingByCode) return;
            _dirty = true;
        }

        private void GalericiYeniSergi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;

            // X ile kapanırken de sor
            if (_dirty)
            {
                var dr = MessageBox.Show(
                    "Değişikliklerden vazgeçmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        // Designer bağlıysa hata olmasın:
        private void txtSergici_TextChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }

        private void TxtKapasite_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void AutoFillSergiciFromModel()
        {
            SetTextIfExists("txtSergici", _model.SergiciAdSoyad, readOnly: true, enabled: false);
            SetTextIfExists("txtSergiciAdSoyad", _model.SergiciAdSoyad, readOnly: true, enabled: false);

            SetTextIfExists("txtEposta", _model.SergiciEposta, readOnly: true, enabled: false);
            SetTextIfExists("txtSergiciEposta", _model.SergiciEposta, readOnly: true, enabled: false);
        }

        private void SetTextIfExists(string controlName, string value, bool readOnly, bool enabled)
        {
            try
            {
                var arr = this.Controls.Find(controlName, true);
                if (arr == null || arr.Length == 0) return;

                foreach (var c in arr)
                {
                    if (c is TextBox tb)
                    {
                        tb.Text = value ?? "";
                        tb.ReadOnly = readOnly;
                        tb.Enabled = enabled;
                    }
                }
            }
            catch { }
        }

        // Sergi adı aynı galericide tekrar var mı?
        private bool IsSergiAdiUniqueForGalerici(int galericiId, string sergiAdi, out string error)
        {
            error = "";

            if (galericiId <= 0) return true;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT COUNT(*)
FROM sergiler
WHERE GalericiID = @gid
  AND TRIM(LOWER(SergiAdi)) = TRIM(LOWER(@ad))
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", galericiId);
                        cmd.Parameters.AddWithValue("@ad", sergiAdi);

                        long count = Convert.ToInt64(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                error = "Sergi adı kontrol edilirken veritabanı hatası:\n" + ex.Message;
                return false;
            }
        }

        // =========================
        // NAV / CONFIRM
        // =========================
        private bool ConfirmDiscardIfDirty()
        {
            if (!_dirty) return true;

            var dr = MessageBox.Show(
                "Değişikliklerden vazgeçmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return dr == DialogResult.Yes;
        }

        private void Navigate(Form nextForm)
        {
            if (nextForm == null) return;

            // sayfa değiştirilecekse dirty sor
            if (!ConfirmDiscardIfDirty()) return;

            _closingByCode = true;
            nextForm.Show();
            this.Close();
        }

        // =========================
        // İLERİ
        // =========================
        private void btnIleri_Click(object sender, EventArgs e)
        {
            string sergiAdi = (txtSergiAdi?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(sergiAdi))
            {
                MessageBox.Show("Sergi adı boş olamaz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSergiAdi?.Focus();
                return;
            }

            if (!SergiAdiRegex.IsMatch(sergiAdi))
            {
                MessageBox.Show(
                    "Sergi adı geçersiz.\nSadece harf, boşluk, apostrof (') , tire (-) ve nokta (.) kullanılabilir.\nRakam kullanmayın.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSergiAdi?.Focus();
                return;
            }

            if (!IsSergiAdiUniqueForGalerici(_galericiId, sergiAdi, out string dbErr))
            {
                if (!string.IsNullOrWhiteSpace(dbErr))
                {
                    MessageBox.Show(dbErr, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show(
                    "Bu sergi adı ile zaten bir serginiz mevcut.\nLütfen farklı bir sergi adı giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSergiAdi?.Focus();
                txtSergiAdi?.SelectAll();
                return;
            }

            if (comboBoxSergiTuru == null || comboBoxSergiTuru.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen sergi türü seçiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxSergiTuru?.Focus();
                return;
            }

            if (comboBoxTema == null || comboBoxTema.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen tema seçiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxTema?.Focus();
                return;
            }

            if (comboBoxHedefKitle == null || comboBoxHedefKitle.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen hedef kitle seçiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxHedefKitle?.Focus();
                return;
            }

            if (!int.TryParse((txtKapasite?.Text ?? "").Trim(), out int kap) ||
                kap < MIN_KAPASITE || kap > MAX_KAPASITE)
            {
                MessageBox.Show($"Kapasite {MIN_KAPASITE}–{MAX_KAPASITE} arası olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKapasite?.Focus();
                return;
            }

            // Modeli doldur
            _model.GalericiId = _galericiId;
            _model.SergiAdi = sergiAdi;
            _model.SergiTuru = comboBoxSergiTuru.SelectedItem?.ToString() ?? "";
            _model.SergiTemasi = comboBoxTema.SelectedItem?.ToString() ?? "";
            _model.HedefKitle = comboBoxHedefKitle.SelectedItem?.ToString() ?? "";
            _model.Kapasite = kap;

            // Burada ilerlemek KAYDETME değil; ama kullanıcı zaten bilerek "İleri" diyor.
            // Yine de sayfa değişimi olduğu için ConfirmDiscardIfDirty() burada uygulanmıyor.
            // Çünkü bu değişiklikler bir sonraki sayfaya aktarılıyor.
            _closingByCode = true;
            new GalericiSergiler2(_model).Show();
            this.Close();
        }

        // =========================
        // İPTAL
        // =========================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            Navigate(new GalericiDashboard(_galericiId));
        }

        // =========================
        // MENÜ
        // =========================
        private void lblDashboard_Click(object sender, EventArgs e) => Navigate(new GalericiDashboard(_galericiId));
        private void lblSergiciTanimlama_Click(object sender, EventArgs e) => Navigate(new GalericiSergiciTanim(_galericiId));
        private void lblMusteriBilgileri_Click(object sender, EventArgs e) => Navigate(new GalericiMusteriBilgileri(_galericiId));
        private void lblSergiler_Click(object sender, EventArgs e) => Navigate(new GalericiSergiler(_galericiId));
        private void lblRaporEkrani_Click(object sender, EventArgs e) => Navigate(new GalericiRaporlar(_galericiId));
        private void lblGalericiAdi_Click(object sender, EventArgs e) => Navigate(new GalericiProfilim(_galericiId));

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;

            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _closingByCode = true;
            new Form1().Show();
            this.Close();
        }

        // =========================
        // FOOTER
        // =========================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            // İstersen dialog açmadan önce de sor:
            if (!ConfirmDiscardIfDirty()) return;

            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            // İstersen dialog açmadan önce de sor:
            if (!ConfirmDiscardIfDirty()) return;

            new Yardim().ShowDialog();
        }
    }
}
