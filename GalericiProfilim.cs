using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiProfilim : Form
    {
        private readonly int _galericiId;

        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        // Eski değerler (iptal / geri alma için)
        private string _orijinalAdSoyad = "";
        private string _orijinalEposta = "";

        private static readonly Regex NameRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,60}$",
            RegexOptions.Compiled);

        // ✅ SADECE izin verilen domainler
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Asıl constructor
        public GalericiProfilim(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            Load -= GalericiProfilim_Load;
            Load += GalericiProfilim_Load;

            Shown -= GalericiProfilim_Shown;
            Shown += GalericiProfilim_Shown;
        }

        public GalericiProfilim() : this(0) { }

        private void GalericiProfilim_Load(object sender, EventArgs e)
        {
            BindCriticalEvents();
            ProfilBilgileriniYukle();
        }

        private void GalericiProfilim_Shown(object sender, EventArgs e)
        {
            BindCriticalEvents();
        }

        // =========================
        // EVENTLER
        // =========================
        private void BindCriticalEvents()
        {
            if (btnGuncelle != null)
            {
                btnGuncelle.Click -= btnGuncelle_Click;
                btnGuncelle.Click += btnGuncelle_Click;
            }

            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }

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

        // =========================
        // NAV
        // =========================
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm != null) nextForm.Show();
            Close();
        }

        private void GoDashboard()
        {
            if (_galericiId > 0)
                NavigateAndClose(new GalericiDashboard(_galericiId));
            else
                Close();
        }

        private bool HasChanges()
        {
            string yeniAd = (txtKullaniciAdi?.Text ?? "").Trim();
            string yeniEposta = (txtEposta?.Text ?? "").Trim();

            return !(string.Equals(yeniAd, _orijinalAdSoyad) &&
                     string.Equals(yeniEposta, _orijinalEposta));
        }

        // =========================
        // PROFİL YÜKLE
        // =========================
        private void ProfilBilgileriniYukle()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "SELECT AdSoyad, Eposta FROM galericiler WHERE GalericiID=@Id LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _galericiId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read()) return;

                            _orijinalAdSoyad = dr["AdSoyad"]?.ToString() ?? "";
                            _orijinalEposta = dr["Eposta"]?.ToString() ?? "";

                            txtKullaniciAdi.Text = _orijinalAdSoyad;
                            txtEposta.Text = _orijinalEposta;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Profil bilgileri yüklenirken hata:\n" + ex.Message);
            }
        }

        // =========================
        // GÜNCELLE
        // =========================
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            string yeniAd = txtKullaniciAdi.Text.Trim();
            string yeniEposta = txtEposta.Text.Trim();

            if (!NameRegex.IsMatch(yeniAd))
            {
                MessageBox.Show("Ad Soyad geçersiz.");
                return;
            }

            // ✅ DOMAIN KONTROLLÜ
            if (!EmailRegex.IsMatch(yeniEposta))
            {
                MessageBox.Show(
                    "E-posta geçersiz.\nSadece şu uzantılar kabul edilir:\n" +
                    "@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!HasChanges())
            {
                MessageBox.Show("Değişiklik yapılmadı.");
                return;
            }

            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "UPDATE galericiler SET AdSoyad=@a, Eposta=@e WHERE GalericiID=@i", conn))
                {
                    cmd.Parameters.AddWithValue("@a", yeniAd);
                    cmd.Parameters.AddWithValue("@e", yeniEposta);
                    cmd.Parameters.AddWithValue("@i", _galericiId);
                    cmd.ExecuteNonQuery();
                }
            }

            _orijinalAdSoyad = yeniAd;
            _orijinalEposta = yeniEposta;

            MessageBox.Show("Profil güncellendi.");
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            txtKullaniciAdi.Text = _orijinalAdSoyad;
            txtEposta.Text = _orijinalEposta;
            GoDashboard();
        }

        // =========================
        // MENÜ
        // =========================
        private void lblDashboard_Click(object sender, EventArgs e) => GoDashboard();
        private void lblSergiciTanimlama_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiciTanim(_galericiId));
        private void lblMusteriBilgileri_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiMusteriBilgileri(_galericiId));
        private void lblSergiler_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiSergiler(_galericiId));
        private void lblRaporEkrani_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiRaporlar(_galericiId));
        private void lblGalericiAdi_Click(object sender, EventArgs e) => NavigateAndClose(new GalericiProfilim(_galericiId));
        private void lblCikisYap_Click(object sender, EventArgs e) => NavigateAndClose(new Form1());

        private void lblHakkimizda_Click(object sender, EventArgs e) =>
            NavigateAndClose(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));

        private void lblYardim_Click(object sender, EventArgs e) =>
            NavigateAndClose(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
    }
}
