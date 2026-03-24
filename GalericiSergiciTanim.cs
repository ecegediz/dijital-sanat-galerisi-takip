using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiciTanim : Form
    {
        private readonly int _galericiId;

        private bool _dirty = false;
        private bool _closingByCode = false;

        // ✅ Ad Soyad: Türkçe karakter + boşluk + ' - .  (rakam yok)
        private static readonly Regex NameRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,60}$",
            RegexOptions.Compiled);

        // ✅ E-posta: SADECE izin verilen domainler
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // ✅ Şifre: 8+, büyük, küçük, sayı, sembol
        private static readonly Regex PasswordRegex = new Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            RegexOptions.Compiled);

        public GalericiSergiciTanim(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            this.Load -= GalericiSergiciTanim_Load;
            this.Load += GalericiSergiciTanim_Load;

            this.FormClosing -= GalericiSergiciTanim_FormClosing;
            this.FormClosing += GalericiSergiciTanim_FormClosing;
        }

        public GalericiSergiciTanim() : this(0) { }

        // =========================================================
        // LOAD
        // =========================================================
        private void GalericiSergiciTanim_Load(object sender, EventArgs e)
        {
            BindCriticalEvents();

            if (txtSifre != null)
                txtSifre.UseSystemPasswordChar = true;

            _dirty = false;
        }

        // =========================================================
        // EVENT GARANTİ
        // =========================================================
        private void BindCriticalEvents()
        {
            // Dirty tracking
            if (txtAdSoyad != null)
            {
                txtAdSoyad.TextChanged -= MarkDirty;
                txtAdSoyad.TextChanged += MarkDirty;
            }

            if (txtEposta != null)
            {
                txtEposta.TextChanged -= MarkDirty;
                txtEposta.TextChanged += MarkDirty;
            }

            if (txtSifre != null)
            {
                txtSifre.TextChanged -= MarkDirty;
                txtSifre.TextChanged += MarkDirty;
            }

            // Butonlar
            if (btnKaydet != null)
            {
                btnKaydet.Click -= btnKaydet_Click;
                btnKaydet.Click += btnKaydet_Click;
            }

            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
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

        private void MarkDirty(object sender, EventArgs e)
        {
            _dirty = true;
        }

        // =========================================================
        // ✅ DOĞRU NAV (ShowDialog zincirini BOZMAZ)
        // =========================================================
        private void NavigateDialog(Form nextForm)
        {
            if (nextForm == null) return;

            _closingByCode = true;   // FormClosing onayı tetiklenmesin
            this.Hide();
            nextForm.ShowDialog();   // ✅ üst formlar geri fırlamaz
            this.Show();
            _closingByCode = false;
        }

        private void GoDashboard()
        {
            NavigateDialog(new GalericiDashboard(_galericiId));
        }

        // =========================================================
        // VALIDATION
        // =========================================================
        private bool ValidateInputs(out string ad, out string ep, out string sf)
        {
            ad = (txtAdSoyad?.Text ?? "").Trim();
            ep = (txtEposta?.Text ?? "").Trim();
            sf = (txtSifre?.Text ?? "");

            if (_galericiId <= 0)
            {
                MessageBox.Show("GalericiID alınamadı. Bu formu ID ile açmalısın.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(ep) || string.IsNullOrWhiteSpace(sf))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!NameRegex.IsMatch(ad))
            {
                MessageBox.Show("Ad Soyad geçersiz. Rakam içeremez ve 2-60 karakter olmalıdır.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAdSoyad?.Focus();
                return false;
            }

            // ✅ SADECE belirli domainler
            if (!EmailRegex.IsMatch(ep))
            {
                MessageBox.Show(
                    "Geçerli e-posta giriniz.\nSadece şu uzantılar kabul edilir:\n" +
                    "@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtEposta?.Focus();
                return false;
            }

            if (!PasswordRegex.IsMatch(sf))
            {
                MessageBox.Show("Şifre en az 8 karakter olmalı ve büyük harf + küçük harf + sayı + sembol içermelidir.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSifre?.Focus();
                return false;
            }

            return true;
        }

        // =========================================================
        // KAYDET
        // =========================================================
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string ad, out string ep, out string sf))
                return;

            var model = new YeniSergiBilgisi
            {
                GalericiId = _galericiId,
                SergiciAdSoyad = ad,
                SergiciEposta = ep,
                SergiciSifre = sf
            };

            // ✅ Show+Close değil, dialog akışı
            NavigateDialog(new GalericiYeniSergi(_galericiId, model));

            // Eğer Kaydetten sonra bu sayfa da kapansın istersen:
            // this.Close();
        }

        // =========================================================
        // İPTAL
        // =========================================================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            if (!_dirty)
            {
                GoDashboard();
                return;
            }

            var dr = MessageBox.Show(
                "İptal etmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                "İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            GoDashboard();
        }

        // X ile kapanma
        private void GalericiSergiciTanim_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;
            if (!_dirty) return;

            var dr = MessageBox.Show(
                "İptal etmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                "İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes)
                e.Cancel = true;
        }

        // =========================================================
        // MENÜ
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e) => GoDashboard();

        private void lblSergiciTanimlama_Click(object sender, EventArgs e)
        {
            // zaten bu sayfadasın
        }

        private void lblMusteriBilgileri_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiMusteriBilgileri(_galericiId));
        }

        private void lblSergiler_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiler(_galericiId));
        }

        private void lblRaporEkrani_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiRaporlar(_galericiId));
        }

        private void lblGalericiAdi_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiProfilim(_galericiId));
        }

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _closingByCode = true;
            new Form1().Show();
            this.Close();
        }

        // =========================================================
        // FOOTER
        // =========================================================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            new Yardim().ShowDialog();
        }

        // =========================================================
        // DESIGNER STUB (kalabilir)
        // =========================================================
        private void label13_Click(object sender, EventArgs e) => GoDashboard();

        private void label16_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _closingByCode = true;
            new Form1().Show();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e) { }
    }
}
