using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiMusteriPDF : Form
    {
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private bool _syncLock = false;

        private const decimal MIN_FIYAT_TL = 0m;
        private const decimal MAX_FIYAT_TL = 5_000_000m;

        private static readonly CultureInfo TR = new CultureInfo("tr-TR");

        private static readonly Regex NameRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,60}$",
            RegexOptions.Compiled);

        // ✅ E-posta: SADECE izin verilen domainler
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public GalericiMusteriPDF(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            Load -= GalericiMusteriPDF_Load;
            Load += GalericiMusteriPDF_Load;

            // Event bazen load'da kopuyor -> Shown'da da güvence
            Shown -= GalericiMusteriPDF_Shown;
            Shown += GalericiMusteriPDF_Shown;
        }

        public GalericiMusteriPDF() : this(0) { }

        private void GalericiMusteriPDF_Load(object sender, EventArgs e)
        {
            PrepareUi();
            BindCriticalEvents();

            LoadSanatcilarFromEserler();
            LoadEserlerBySelectedSanatci(); // ilk yükleme
        }

        private void GalericiMusteriPDF_Shown(object sender, EventArgs e)
        {
            // Designer/WinForms bazen eventleri “unutuyor” -> tekrar bağla
            BindCriticalEvents();
        }

        // =====================
        // UI Hazırlık
        // =====================
        private void PrepareUi()
        {
            if (txtFiyat != null)
            {
                txtFiyat.ReadOnly = true;
                txtFiyat.Text = "";
            }

            if (cmbSergici != null) cmbSergici.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbEser != null) cmbEser.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // =====================
        // Event'leri garanti bağla
        // =====================
        private void BindCriticalEvents()
        {
            if (cmbSergici != null)
            {
                cmbSergici.SelectedIndexChanged -= cmbSergici_SelectedIndexChanged;
                cmbSergici.SelectedIndexChanged += cmbSergici_SelectedIndexChanged;
            }

            if (cmbEser != null)
            {
                cmbEser.SelectedIndexChanged -= cmbEser_SelectedIndexChanged;
                cmbEser.SelectedIndexChanged += cmbEser_SelectedIndexChanged;
            }

            if (btnEkle != null)
            {
                btnEkle.Click -= btnEkle_Click;
                btnEkle.Click += btnEkle_Click;
            }

            if (btnIptal != null)
            {
                btnIptal.Click -= btnIptal_Click;
                btnIptal.Click += btnIptal_Click;
            }

            // Menü
            lblDashboard.Click -= lblDashboard_Click; lblDashboard.Click += lblDashboard_Click;
            lblSergiciTanimlama.Click -= lblSergiciTanimlama_Click; lblSergiciTanimlama.Click += lblSergiciTanimlama_Click;
            lblMusteriBilgileri.Click -= lblMusteriBilgileri_Click; lblMusteriBilgileri.Click += lblMusteriBilgileri_Click;
            lblSergiler.Click -= lblSergiler_Click; lblSergiler.Click += lblSergiler_Click;
            lblRaporEkrani.Click -= lblRaporEkrani_Click; lblRaporEkrani.Click += lblRaporEkrani_Click;
            lblGalericiAdi.Click -= lblGalericiAdi_Click; lblGalericiAdi.Click += lblGalericiAdi_Click;
            lblCikisYap.Click -= lblCikisYap_Click; lblCikisYap.Click += lblCikisYap_Click;

            // Footer
            lblHakkimizda.Click -= lblHakkimizda_Click; lblHakkimizda.Click += lblHakkimizda_Click;
            lblYardim.Click -= lblYardim_Click; lblYardim.Click += lblYardim_Click;
        }

        // =====================
        // Ortak Navigation (tek pencere)
        // =====================
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm != null) nextForm.Show();
            Close();
        }

        private void GoMusteriBilgileri()
        {
            NavigateAndClose(new GalericiMusteriBilgileri(_galericiId));
        }

        // =====================
        // Sanatçı Combobox
        // =====================
        private void LoadSanatcilarFromEserler()
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
SELECT DISTINCT SanatciAdi
FROM eserler
WHERE SanatciAdi IS NOT NULL AND TRIM(SanatciAdi) <> ''
ORDER BY SanatciAdi;";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    cmbSergici.DisplayMember = "SanatciAdi";
                    cmbSergici.ValueMember = "SanatciAdi";
                    cmbSergici.DataSource = dt;
                }
            }
        }

        private void cmbSergici_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;

            _syncLock = true;
            LoadEserlerBySelectedSanatci();
            txtFiyat.Text = "";
            _syncLock = false;
        }

        // =====================
        // Eser Combobox (satılmış olanlar görünmez)
        // =====================
        private void LoadEserlerBySelectedSanatci()
        {
            if (cmbSergici.SelectedValue == null)
            {
                cmbEser.DataSource = null;
                txtFiyat.Text = "";
                return;
            }

            string sanatciAdi = cmbSergici.SelectedValue.ToString().Trim();
            if (string.IsNullOrWhiteSpace(sanatciAdi))
            {
                cmbEser.DataSource = null;
                txtFiyat.Text = "";
                return;
            }

            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
SELECT e.EserID, e.Baslik, e.Fiyat
FROM eserler e
WHERE e.SanatciAdi = @ad
  AND NOT EXISTS (SELECT 1 FROM satislar s WHERE s.EserID = e.EserID)
ORDER BY e.Baslik;";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ad", sanatciAdi);

                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        cmbEser.DataSource = null;
                        txtFiyat.Text = "";
                        return;
                    }

                    cmbEser.DisplayMember = "Baslik";
                    cmbEser.ValueMember = "EserID";
                    cmbEser.DataSource = dt;
                }
            }
        }

        private void cmbEser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;
            FillPriceFromSelectedEser();
        }

        private void FillPriceFromSelectedEser()
        {
            if (cmbEser.SelectedValue == null)
            {
                txtFiyat.Text = "";
                return;
            }

            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("SELECT Fiyat FROM eserler WHERE EserID=@eid LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@eid", Convert.ToInt32(cmbEser.SelectedValue));
                    object o = cmd.ExecuteScalar();

                    decimal fiyat = (o == null || o == DBNull.Value) ? 0m : Convert.ToDecimal(o);

                    if (fiyat < MIN_FIYAT_TL) fiyat = MIN_FIYAT_TL;
                    if (fiyat > MAX_FIYAT_TL) fiyat = MAX_FIYAT_TL;

                    txtFiyat.Text = fiyat.ToString("N2", TR) + " TL";
                }
            }
        }

        // =====================
        // Past sergi kontrol
        // =====================
        private bool IsEserInPastSergi(int eserId, MySqlConnection conn, MySqlTransaction tx)
        {
            string sql = @"
SELECT COUNT(*)
FROM eserler e
JOIN sergiler sg ON sg.SergiID = e.SergiID
WHERE e.EserID=@eid
  AND DATE(sg.BitisTarihi) < CURDATE()
LIMIT 1;";

            using (var cmd = new MySqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@eid", eserId);
                long adet = Convert.ToInt64(cmd.ExecuteScalar());
                return adet > 0;
            }
        }

        // =====================
        // Ekle (müşteri + satış)
        // =====================
        private void btnEkle_Click(object sender, EventArgs e)
        {
            string ad = (txtMusteriAdi.Text ?? "").Trim();
            string ep = (txtEposta.Text ?? "").Trim();

            if (!NameRegex.IsMatch(ad))
            {
                MessageBox.Show("İsim sadece harflerden oluşmalı (rakam yasak).",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ SADECE belirli domainler
            if (!EmailRegex.IsMatch(ep))
            {
                MessageBox.Show(
                    "E-posta formatı hatalı.\nSadece şu uzantılar kabul edilir:\n" +
                    "@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbEser.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir eser seçin.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int eserId = Convert.ToInt32(cmbEser.SelectedValue);

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        if (IsEserInPastSergi(eserId, conn, tx))
                        {
                            MessageBox.Show("Sergi tarihi geçtiği için bu eser satılamaz.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tx.Rollback();
                            return;
                        }

                        using (var check = new MySqlCommand("SELECT COUNT(*) FROM satislar WHERE EserID=@eid;", conn, tx))
                        {
                            check.Parameters.AddWithValue("@eid", eserId);
                            long adet = Convert.ToInt64(check.ExecuteScalar());
                            if (adet > 0)
                            {
                                MessageBox.Show("Bu eser zaten satılmış. Tekrar satılamaz.",
                                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tx.Rollback();
                                return;
                            }
                        }

                        int musteriId;
                        using (var cmd = new MySqlCommand(@"
INSERT INTO musteriler (GalericiID, AdSoyad, Eposta)
VALUES (@gid, @ad, @ep);
SELECT LAST_INSERT_ID();", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@ep", ep);
                            musteriId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        decimal fiyat;
                        using (var p = new MySqlCommand("SELECT Fiyat FROM eserler WHERE EserID=@eid LIMIT 1;", conn, tx))
                        {
                            p.Parameters.AddWithValue("@eid", eserId);
                            object o = p.ExecuteScalar();
                            fiyat = (o == null || o == DBNull.Value) ? 0m : Convert.ToDecimal(o);
                        }

                        if (fiyat < MIN_FIYAT_TL || fiyat > MAX_FIYAT_TL)
                        {
                            MessageBox.Show("Eser fiyatı geçersiz görünüyor. Lütfen eseri kontrol edin.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tx.Rollback();
                            return;
                        }

                        using (var cmd = new MySqlCommand(@"
INSERT INTO satislar
(EserID, GalericiID, MusteriID, SatisFiyati, SatisTarihi, SatisDurumu, MustAdSoyad, MustEposta)
VALUES
(@eid, @gid, @mid, @f, NOW(), 'Tamamlandi', @ad, @ep);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@eid", eserId);
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            cmd.Parameters.AddWithValue("@mid", musteriId);
                            cmd.Parameters.AddWithValue("@f", fiyat);
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@ep", ep);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Müşteri başarıyla eklendi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                GoMusteriBilgileri();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri eklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            GoMusteriBilgileri();
        }

        // =====================
        // Designer stub (kırılmasın)
        // =====================
        private void label3_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void txtEposta_TextChanged(object sender, EventArgs e) { }

        // =====================
        // MENU (tek pencere)
        // =====================
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

        // =====================
        // FOOTER (rol + tek pencere)
        // =====================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }
    }
}
