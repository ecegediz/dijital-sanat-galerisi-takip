using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiler3 : Form
    {
        private readonly YeniSergiBilgisi _model;
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // Mantıklı aralıklar (istersen değiştir)
        private const decimal MIN_KIRA = 1m;
        private const decimal MAX_KIRA = 5_000_000m;

        private const decimal MIN_ESER_BASI = 1m;
        private const decimal MAX_ESER_BASI = 250_000m;

        private bool _syncLock = false;
        private bool _dirty = false;
        private bool _closingByCode = false;

        public GalericiSergiler3(YeniSergiBilgisi model)
        {
            InitializeComponent();
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _galericiId = _model.GalericiId;

            this.Load -= GalericiSergiler3_Load;
            this.Load += GalericiSergiler3_Load;

            this.FormClosing -= GalericiSergiler3_FormClosing;
            this.FormClosing += GalericiSergiler3_FormClosing;
        }

        public GalericiSergiler3() : this(new YeniSergiBilgisi()) { }

        private void GalericiSergiler3_Load(object sender, EventArgs e)
        {
            BindCriticalEvents();

            _syncLock = true;

            // Sergi adı + süresi otomatik, değişmesin
            if (txtSergiAdi != null)
            {
                txtSergiAdi.Text = _model.SergiAdi ?? "";
                txtSergiAdi.ReadOnly = true;
                txtSergiAdi.Enabled = false;
            }

            int sureGun = 0;
            if (_model.BaslangicTarihi != default && _model.BitisTarihi != default)
                sureGun = (int)(_model.BitisTarihi.Date - _model.BaslangicTarihi.Date).TotalDays + 1;

            if (txtSuresi != null)
            {
                txtSuresi.Text = sureGun > 0 ? sureGun.ToString() : "";
                txtSuresi.ReadOnly = true;
                txtSuresi.Enabled = false;
            }

            // Geri gelindiyse modelden doldur
            if (_model.EserSayisi > 0 && eserSayisiTextBox != null && string.IsNullOrWhiteSpace(eserSayisiTextBox.Text))
                eserSayisiTextBox.Text = _model.EserSayisi.ToString();

            if (_model.GaleriKirasi > 0 && galeriKirasiTextBox != null && string.IsNullOrWhiteSpace(galeriKirasiTextBox.Text))
                galeriKirasiTextBox.Text = _model.GaleriKirasi.ToString("0.00", CultureInfo.InvariantCulture);

            if (_model.EserBasiUcret > 0 && eserBasıUcretTextBox != null && string.IsNullOrWhiteSpace(eserBasıUcretTextBox.Text))
                eserBasıUcretTextBox.Text = _model.EserBasiUcret.ToString("0.00", CultureInfo.InvariantCulture);

            _syncLock = false;

            RecalcTotal();
            _dirty = false;
        }

        // =========================================================
        // EVENT BAĞLAMA (Designer kopsa bile çalışsın)
        // =========================================================
        private void BindCriticalEvents()
        {
            // KeyPress kısıtları
            if (eserSayisiTextBox != null)
            {
                eserSayisiTextBox.KeyPress -= eserSayisiTextBox_KeyPress; // Designer bekliyor
                eserSayisiTextBox.KeyPress += eserSayisiTextBox_KeyPress;

                eserSayisiTextBox.TextChanged -= AnyField_TextChanged;
                eserSayisiTextBox.TextChanged += AnyField_TextChanged;
            }

            if (galeriKirasiTextBox != null)
            {
                galeriKirasiTextBox.KeyPress -= SayisalTextBox_KeyPress; // Designer bekliyor
                galeriKirasiTextBox.KeyPress += SayisalTextBox_KeyPress;

                galeriKirasiTextBox.TextChanged -= AnyField_TextChanged;
                galeriKirasiTextBox.TextChanged += AnyField_TextChanged;

                galeriKirasiTextBox.Leave -= Money_LeaveFormat;
                galeriKirasiTextBox.Leave += Money_LeaveFormat;
            }

            if (eserBasıUcretTextBox != null)
            {
                eserBasıUcretTextBox.KeyPress -= SayisalTextBox_KeyPress; // Designer bekliyor
                eserBasıUcretTextBox.KeyPress += SayisalTextBox_KeyPress;

                eserBasıUcretTextBox.TextChanged -= AnyField_TextChanged;
                eserBasıUcretTextBox.TextChanged += AnyField_TextChanged;

                eserBasıUcretTextBox.Leave -= Money_LeaveFormat;
                eserBasıUcretTextBox.Leave += Money_LeaveFormat;
            }

            // Butonlar (Designer’da farklı isim bağlıysa bile wrapper’lar aşağıda var)
            if (onaylaButton != null)
            {
                onaylaButton.Click -= onaylaButton_Click;
                onaylaButton.Click += onaylaButton_Click;
            }

            if (iptalButton != null)
            {
                iptalButton.Click -= iptalButton_Click;
                iptalButton.Click += iptalButton_Click;
            }

            if (oncekiButton != null)
            {
                oncekiButton.Click -= oncekiButton_Click;
                oncekiButton.Click += oncekiButton_Click;
            }

            // Menü / Footer (senin formunda label isimleri varsa çalışır)
            if (lblDashboard != null) { lblDashboard.Click -= lblDashboard_Click; lblDashboard.Click += lblDashboard_Click; }
            if (lblSergiciTanimlama != null) { lblSergiciTanimlama.Click -= lblSergiciTanimlama_Click; lblSergiciTanimlama.Click += lblSergiciTanimlama_Click; }
            if (lblMusteriBilgileri != null) { lblMusteriBilgileri.Click -= lblMusteriBilgileri_Click; lblMusteriBilgileri.Click += lblMusteriBilgileri_Click; }
            if (lblSergiler != null) { lblSergiler.Click -= lblSergiler_Click; lblSergiler.Click += lblSergiler_Click; }
            if (lblRaporEkrani != null) { lblRaporEkrani.Click -= lblRaporEkrani_Click; lblRaporEkrani.Click += lblRaporEkrani_Click; }
            if (lblGalericiAdi != null) { lblGalericiAdi.Click -= lblGalericiAdi_Click; lblGalericiAdi.Click += lblGalericiAdi_Click; }
            if (lblCikisYap != null) { lblCikisYap.Click -= lblCikisYap_Click; lblCikisYap.Click += lblCikisYap_Click; }

            if (lblHakkimizda != null) { lblHakkimizda.Click -= lblHakkimizda_Click; lblHakkimizda.Click += lblHakkimizda_Click; }
            if (lblYardim != null) { lblYardim.Click -= lblYardim_Click; lblYardim.Click += lblYardim_Click; }
        }

        private void AnyField_TextChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;
            _dirty = true;
            RecalcTotal();
        }

        // =========================================================
        // DESIGNER’IN İSTEDİĞİ EVENT İSİMLERİ (HATA BURADAYDI)
        // =========================================================

        // Decimal alanlar: digit + tek ayraç (virgül/nokta)
        private void SayisalTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            if (sender is TextBox tb && (e.KeyChar == ',' || e.KeyChar == '.'))
            {
                if (tb.Text.Contains(",") || tb.Text.Contains("."))
                    e.Handled = true;
            }
        }

        // eser sayısı: sadece digit
        private void eserSayisiTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // =========================================================
        // PARSE + FORMAT
        // =========================================================
        private bool TryParseDecimalTR(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string s = input.Trim()
                .Replace("₺", "")
                .Replace("TL", "")
                .Trim()
                .Replace(',', '.');

            return decimal.TryParse(
                s,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value);
        }

        private void Money_LeaveFormat(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (TryParseDecimalTR(tb.Text, out decimal v) && v >= 0)
                    tb.Text = v.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        private void RecalcTotal()
        {
            if (toplamFiyatLabel == null) return;

            int eser = 0;
            if (eserSayisiTextBox != null)
                int.TryParse(eserSayisiTextBox.Text.Trim(), out eser);

            decimal kira = 0m;
            if (galeriKirasiTextBox != null)
                TryParseDecimalTR(galeriKirasiTextBox.Text, out kira);

            decimal eb = 0m;
            if (eserBasıUcretTextBox != null)
                TryParseDecimalTR(eserBasıUcretTextBox.Text, out eb);

            decimal toplam = kira + (eser * eb);
            toplamFiyatLabel.Text = $"{toplam:0.00} TL";
        }

        // =========================================================
        // GÜVENLİ NAV (Uygulama kapanmasın diye)
        // =========================================================
        private void NavigateSafe(Form next)
        {
            if (next == null) return;

            _closingByCode = true;

            // yeni formu göster
            next.Show();

            // bu formu gizle (start form kapanınca uygulama kapanmasın)
            this.Hide();

            // yeni form kapanınca bunu kapat
            next.FormClosed += (s, e) =>
            {
                try
                {
                    if (!this.IsDisposed) this.Close();
                }
                catch { }
            };
        }

        // Menü/Çıkış öncesi “vazgeçilsin mi?”
        private bool ConfirmDiscardIfDirty()
        {
            if (!_dirty) return true;

            var dr = MessageBox.Show(
                "Yaptığınız değişiklikler kaydedilmedi.\nDeğişikliklerden vazgeçmek istiyor musunuz?",
                "Uyarı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return dr == DialogResult.Yes;
        }

        private void GalericiSergiler3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;

            if (!ConfirmDiscardIfDirty())
                e.Cancel = true;
        }

        // =========================================================
        // ONAYLA (INSERT) + İPTAL + ÖNCEKİ
        // =========================================================
        private void onaylaButton_Click(object sender, EventArgs e)
        {
            var sure = MessageBox.Show(
                "Yeni sergi oluşturmak istediğinizden emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (sure != DialogResult.Yes) return;

            if (eserSayisiTextBox == null || !int.TryParse(eserSayisiTextBox.Text, out int eser) || eser <= 0)
            {
                MessageBox.Show("Eser sayısı hatalı. En az 1 olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                eserSayisiTextBox?.Focus();
                return;
            }

            if (galeriKirasiTextBox == null || !TryParseDecimalTR(galeriKirasiTextBox.Text, out decimal kira) || kira < MIN_KIRA || kira > MAX_KIRA)
            {
                MessageBox.Show($"Galeri kirası hatalı. {MIN_KIRA:n0} - {MAX_KIRA:n0} arası olmalı.\nOndalık için virgül veya nokta kullanabilirsiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                galeriKirasiTextBox?.Focus();
                return;
            }

            if (eserBasıUcretTextBox == null || !TryParseDecimalTR(eserBasıUcretTextBox.Text, out decimal eb) || eb < MIN_ESER_BASI || eb > MAX_ESER_BASI)
            {
                MessageBox.Show($"Eser başı ücret hatalı. {MIN_ESER_BASI:n0} - {MAX_ESER_BASI:n0} arası olmalı.\nOndalık için virgül veya nokta kullanabilirsiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                eserBasıUcretTextBox?.Focus();
                return;
            }

            _model.EserSayisi = eser;
            _model.GaleriKirasi = kira;
            _model.EserBasiUcret = eb;

            decimal toplam = kira + (eser * eb);

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        // Sergici yoksa ekle
                        if (!_model.SanatciID.HasValue || _model.SanatciID.Value <= 0)
                        {
                            using (var cmdS = new MySqlCommand(@"
INSERT INTO sergiciler (AdSoyad, Eposta, Sifre, GalericiID, Durum)
VALUES (@a,@e,@s,@g,'Aktif');
SELECT LAST_INSERT_ID();", conn, tx))
                            {
                                cmdS.Parameters.AddWithValue("@a", _model.SergiciAdSoyad);
                                cmdS.Parameters.AddWithValue("@e", _model.SergiciEposta);
                                cmdS.Parameters.AddWithValue("@s", _model.SergiciSifre);
                                cmdS.Parameters.AddWithValue("@g", _galericiId);

                                _model.SanatciID = Convert.ToInt32(cmdS.ExecuteScalar());
                            }
                        }
                        else
                        {
                            using (var cmdAktif = new MySqlCommand(
                                "UPDATE sergiciler SET Durum='Aktif' WHERE SanatciID=@sid AND GalericiID=@gid;",
                                conn, tx))
                            {
                                cmdAktif.Parameters.AddWithValue("@sid", _model.SanatciID.Value);
                                cmdAktif.Parameters.AddWithValue("@gid", _galericiId);
                                cmdAktif.ExecuteNonQuery();
                            }
                        }

                        // Sergi INSERT
                        using (var cmdE = new MySqlCommand(@"
INSERT INTO sergiler
(SergiAdi, SergiTuru, SergiTemasi, HedefKitle, Kapasite,
 BaslangicTarihi, BitisTarihi,
 EserSayisi,
 GaleriKirasi, EserBasiUcret, ToplamMaliyet,
 Sergici, Durum, GalericiID, SanatciID)
VALUES
(@adi, @tur, @tema, @hk, @kap,
 @bas, @bit,
 @es,
 @k, @eb, @top,
 @ser, 'Aktif', @gid, @sid);", conn, tx))
                        {
                            cmdE.Parameters.AddWithValue("@adi", _model.SergiAdi);
                            cmdE.Parameters.AddWithValue("@tur", _model.SergiTuru);
                            cmdE.Parameters.AddWithValue("@tema", _model.SergiTemasi);
                            cmdE.Parameters.AddWithValue("@hk", _model.HedefKitle);
                            cmdE.Parameters.AddWithValue("@kap", _model.Kapasite);
                            cmdE.Parameters.AddWithValue("@bas", _model.BaslangicTarihi);
                            cmdE.Parameters.AddWithValue("@bit", _model.BitisTarihi);

                            cmdE.Parameters.AddWithValue("@es", _model.EserSayisi);
                            cmdE.Parameters.AddWithValue("@k", _model.GaleriKirasi);
                            cmdE.Parameters.AddWithValue("@eb", _model.EserBasiUcret);
                            cmdE.Parameters.AddWithValue("@top", toplam);

                            cmdE.Parameters.AddWithValue("@ser", _model.SergiciAdSoyad);
                            cmdE.Parameters.AddWithValue("@gid", _galericiId);
                            cmdE.Parameters.AddWithValue("@sid", _model.SanatciID.Value);

                            cmdE.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Tüm kayıtlar başarıyla oluşturuldu.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                _dirty = false;
                NavigateSafe(new GalericiSergiler(_galericiId)); // ✅ Sergiler ekranına dön
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void iptalButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "İptal etmek istiyor musunuz?\nKaydedilmemiş bilgiler silinecek.",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _dirty = false;
            NavigateSafe(new GalericiSergiler(_galericiId)); // ✅ İptalde de Sergiler’e dön
        }

        private void oncekiButton_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;

            _dirty = false;
            NavigateSafe(new GalericiSergiler2(_model));
        }

        // =========================================================
        // MENÜ / FOOTER (Değişiklik uyarılı)
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiDashboard(_galericiId));
        }

        private void lblSergiciTanimlama_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiSergiciTanim(_galericiId));
        }

        private void lblMusteriBilgileri_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiMusteriBilgileri(_galericiId));
        }

        private void lblSergiler_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiSergiler(_galericiId));
        }

        private void lblRaporEkrani_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiRaporlar(_galericiId));
        }

        private void lblGalericiAdi_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            _dirty = false;
            NavigateSafe(new GalericiProfilim(_galericiId));
        }

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;

            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _dirty = false;
            NavigateSafe(new Form1());
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            // dialog, navigasyon değil
            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            // dialog, navigasyon değil
            new Yardim().ShowDialog();
        }

        // =========================================================
        // DESIGNER ALIAS (Eğer Designer bunları çağırıyorsa kırılmasın)
        // =========================================================
        private void onaylaButton_Click_1(object sender, EventArgs e) => onaylaButton_Click(sender, e);
        private void iptalButton_Click_1(object sender, EventArgs e) => iptalButton_Click(sender, e);
        private void oncekiButton_Click_1(object sender, EventArgs e) => oncekiButton_Click(sender, e);
    }
}
