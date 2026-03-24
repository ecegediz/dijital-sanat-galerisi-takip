using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiMusteriDuzenleme : Form
    {
        private readonly int _galericiId;
        private readonly int _musteriId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private bool _syncLock = false;
        private int? _currentEserId = null;

        // ==== FİYAT KURALLARI (TL) ====
        private const decimal MIN_FIYAT_TL = 0m;
        private const decimal MAX_FIYAT_TL = 5_000_000m;

        private static readonly Regex NameRegex = new Regex(
            @"^[A-Za-zÇĞİÖŞÜçğıöşü\s'\-\.]{2,60}$",
            RegexOptions.Compiled);

        // ✅ SADECE izin verilen domainler
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+\-]+@(hotmail\.com|gmail\.com|outlook\.com|artflow\.com)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public GalericiMusteriDuzenleme()
        {
            InitializeComponent();
        }

        public GalericiMusteriDuzenleme(int galericiId, int musteriId) : this()
        {
            _galericiId = galericiId;
            _musteriId = musteriId;

            Load -= GalericiMusteriDuzenleme_Load;
            Load += GalericiMusteriDuzenleme_Load;
        }

        private void GalericiMusteriDuzenleme_Load(object sender, EventArgs e)
        {
            if (cmbSergici != null) cmbSergici.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbEser != null) cmbEser.DropDownStyle = ComboBoxStyle.DropDownList;

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

            HookFiyatEvents();

            LoadSanatcilarFromEserler();
            LoadCustomerAndFill();
        }

        // =========================================================
        // ✅ GALERICI NAV STANDARDI
        // =========================================================
        private void NavigateDialog(Form nextForm)
        {
            if (nextForm == null) return;

            this.Hide();
            try
            {
                nextForm.ShowDialog();
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    this.Show();
                    this.BringToFront();
                }
            }
        }

        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;
            nextForm.Show();
            this.Close();
        }

        private void GoMusteriBilgileri()
        {
            this.Close();
        }

        // =====================
        // FİYAT EVENTLERİ
        // =====================
        private void HookFiyatEvents()
        {
            if (txtFiyat == null) return;

            txtFiyat.KeyPress -= txtFiyat_KeyPress;
            txtFiyat.KeyPress += txtFiyat_KeyPress;

            txtFiyat.Enter -= txtFiyat_Enter;
            txtFiyat.Enter += txtFiyat_Enter;

            txtFiyat.Leave -= txtFiyat_Leave;
            txtFiyat.Leave += txtFiyat_Leave;
        }

        private void txtFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                var tb = (TextBox)sender;
                if (tb.Text.Contains(",") || tb.Text.Contains("."))
                    e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void txtFiyat_Enter(object sender, EventArgs e)
        {
            RemoveSuffix(txtFiyat, " TL");
        }

        private void txtFiyat_Leave(object sender, EventArgs e)
        {
            if (txtFiyat == null) return;

            string raw = StripSuffix((txtFiyat.Text ?? "").Trim(), " TL");
            if (string.IsNullOrWhiteSpace(raw))
            {
                txtFiyat.Text = "";
                return;
            }

            if (!TryParseDecimalSmart(raw, out decimal val))
            {
                MessageBox.Show("Fiyat geçersiz. Örnek: 1200,50", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFiyat.Focus();
                txtFiyat.SelectAll();
                return;
            }

            if (val < MIN_FIYAT_TL || val > MAX_FIYAT_TL)
            {
                MessageBox.Show($"Fiyat {MIN_FIYAT_TL:0.00} - {MAX_FIYAT_TL:0.00} TL aralığında olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFiyat.Focus();
                txtFiyat.SelectAll();
                return;
            }

            txtFiyat.Text = val.ToString("N2", new CultureInfo("tr-TR")) + " TL";
        }

        private static void RemoveSuffix(TextBox tb, string suffix)
        {
            if (tb == null) return;
            if (string.IsNullOrWhiteSpace(tb.Text)) return;

            string t = tb.Text.Trim();
            if (t.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                tb.Text = t.Substring(0, t.Length - suffix.Length).Trim();
                tb.SelectionStart = tb.Text.Length;
            }
        }

        private static string StripSuffix(string text, string suffix)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            string t = text.Trim();
            if (t.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                t = t.Substring(0, t.Length - suffix.Length).Trim();
            return t;
        }

        private static bool TryParseDecimalSmart(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string s = input.Trim();
            s = s.Replace("₺", "").Replace("TL", "").Trim();
            s = s.Replace(" ", "");

            if (decimal.TryParse(s, NumberStyles.Number, new CultureInfo("tr-TR"), out value))
                return true;

            s = s.Replace(',', '.');
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return true;

            return false;
        }

        // =====================
        // SANATÇI -> COMBO
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

                    if (cmbSergici != null)
                    {
                        cmbSergici.DisplayMember = "SanatciAdi";
                        cmbSergici.ValueMember = "SanatciAdi";
                        cmbSergici.DataSource = dt;
                    }
                }
            }
        }

        // =====================
        // MÜŞTERİ YÜKLE
        // =====================
        private void LoadCustomerAndFill()
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
SELECT 
    m.AdSoyad,
    m.Eposta,
    s.EserID,
    e.SanatciAdi,
    COALESCE(s.SatisFiyati, e.Fiyat) AS Fiyat
FROM musteriler m
LEFT JOIN satislar s ON s.MusteriID = m.MusteriID
LEFT JOIN eserler e ON e.EserID = s.EserID
WHERE m.MusteriID=@mid AND (@gid=0 OR m.GalericiID=@gid)
LIMIT 1;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", _musteriId);
                    cmd.Parameters.AddWithValue("@gid", _galericiId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            MessageBox.Show("Müşteri bulunamadı.", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            GoMusteriBilgileri();
                            return;
                        }

                        _syncLock = true;

                        if (txtMusteriAdi != null) txtMusteriAdi.Text = dr["AdSoyad"]?.ToString() ?? "";
                        if (txtEposta != null) txtEposta.Text = dr["Eposta"]?.ToString() ?? "";

                        _currentEserId = (dr["EserID"] != DBNull.Value) ? Convert.ToInt32(dr["EserID"]) : (int?)null;

                        string sanatciAdi = dr["SanatciAdi"]?.ToString() ?? "";
                        SetComboByText(cmbSergici, sanatciAdi);

                        LoadEserlerBySanatciAllowCurrent(sanatciAdi, _currentEserId);

                        if (_currentEserId.HasValue && cmbEser != null)
                            cmbEser.SelectedValue = _currentEserId.Value;

                        if (txtFiyat != null)
                        {
                            if (dr["Fiyat"] != DBNull.Value)
                            {
                                decimal f = Convert.ToDecimal(dr["Fiyat"]);
                                txtFiyat.Text = f.ToString("N2", new CultureInfo("tr-TR")) + " TL";
                            }
                            else
                            {
                                txtFiyat.Text = "";
                            }
                        }

                        _syncLock = false;
                    }
                }
            }
        }

        // =====================
        // ESER LİSTESİ (satılmış gizle, mevcut eser hariç)
        // =====================
        private void LoadEserlerBySanatciAllowCurrent(string sanatciAdi, int? allowEserId)
        {
            if (cmbEser == null) return;

            if (string.IsNullOrWhiteSpace(sanatciAdi))
            {
                cmbEser.DataSource = null;
                return;
            }

            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
SELECT e.EserID, e.Baslik, e.Fiyat
FROM eserler e
WHERE e.SanatciAdi = @ad
  AND (
        e.EserID = @allowId
        OR NOT EXISTS (SELECT 1 FROM satislar s WHERE s.EserID = e.EserID)
      )
ORDER BY e.Baslik;";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ad", sanatciAdi);
                    da.SelectCommand.Parameters.AddWithValue("@allowId",
                        allowEserId.HasValue ? (object)allowEserId.Value : DBNull.Value);

                    var dt = new DataTable();
                    da.Fill(dt);

                    cmbEser.DisplayMember = "Baslik";
                    cmbEser.ValueMember = "EserID";
                    cmbEser.DataSource = dt;
                }
            }
        }

        private void cmbSergici_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;
            if (cmbSergici?.SelectedValue == null) return;

            string sanatciAdi = cmbSergici.SelectedValue.ToString();

            _syncLock = true;

            _currentEserId = null;
            LoadEserlerBySanatciAllowCurrent(sanatciAdi, null);
            if (txtFiyat != null) txtFiyat.Text = "";

            _syncLock = false;
        }

        private void cmbEser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncLock) return;
            FillPriceFromSelectedEser();
        }

        private void FillPriceFromSelectedEser()
        {
            if (cmbEser?.SelectedValue == null || txtFiyat == null)
            {
                if (txtFiyat != null) txtFiyat.Text = "";
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
                    txtFiyat.Text = fiyat.ToString("N2", new CultureInfo("tr-TR")) + " TL";
                }
            }
        }

        // =====================
        // DÜZENLE
        // =====================
        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            string ad = (txtMusteriAdi?.Text ?? "").Trim();
            string ep = (txtEposta?.Text ?? "").Trim();

            if (!NameRegex.IsMatch(ad))
            {
                MessageBox.Show("İsim sadece harflerden oluşmalı (rakam yasak).",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EmailRegex.IsMatch(ep))
            {
                MessageBox.Show(
                    "E-posta formatı hatalı.\nSadece şu uzantılar kabul edilir:\n" +
                    "@hotmail.com\n@gmail.com\n@outlook.com\n@artflow.com",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbEser?.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir eser seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fiyatRaw = StripSuffix((txtFiyat?.Text ?? "").Trim(), " TL");
            if (!TryParseDecimalSmart(fiyatRaw, out decimal fiyat) || fiyat < MIN_FIYAT_TL || fiyat > MAX_FIYAT_TL)
            {
                MessageBox.Show($"Fiyat geçerli olmalı ve {MIN_FIYAT_TL:0.00} - {MAX_FIYAT_TL:0.00} TL aralığında olmalıdır.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFiyat?.Focus();
                txtFiyat?.SelectAll();
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
                        using (var cmd = new MySqlCommand(
                            "UPDATE musteriler SET AdSoyad=@ad, Eposta=@ep WHERE MusteriID=@mid AND (@gid=0 OR GalericiID=@gid);",
                            conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@ep", ep);
                            cmd.Parameters.AddWithValue("@mid", _musteriId);
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            cmd.ExecuteNonQuery();
                        }

                        if (!_currentEserId.HasValue || eserId != _currentEserId.Value)
                        {
                            using (var chk = new MySqlCommand("SELECT COUNT(*) FROM satislar WHERE EserID=@eid;", conn, tx))
                            {
                                chk.Parameters.AddWithValue("@eid", eserId);
                                long adet = Convert.ToInt64(chk.ExecuteScalar());
                                if (adet > 0)
                                {
                                    MessageBox.Show("Seçtiğiniz eser zaten satılmış. Başka eser seçin.",
                                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    tx.Rollback();
                                    return;
                                }
                            }
                        }

                        long satisVar;
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM satislar WHERE MusteriID=@mid;", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@mid", _musteriId);
                            satisVar = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        if (satisVar == 0)
                        {
                            using (var cmd = new MySqlCommand(@"
INSERT INTO satislar (EserID, GalericiID, MusteriID, SatisFiyati, SatisDurumu, MustAdSoyad, MustEposta)
VALUES (@eid, @gid, @mid, @f, 'Tamamlandi', @ad, @ep);", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@eid", eserId);
                                cmd.Parameters.AddWithValue("@gid", _galericiId);
                                cmd.Parameters.AddWithValue("@mid", _musteriId);
                                cmd.Parameters.AddWithValue("@f", fiyat);
                                cmd.Parameters.AddWithValue("@ad", ad);
                                cmd.Parameters.AddWithValue("@ep", ep);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (var cmd = new MySqlCommand(@"
UPDATE satislar
SET EserID=@eid, SatisFiyati=@f, MustAdSoyad=@ad, MustEposta=@ep
WHERE MusteriID=@mid;", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@eid", eserId);
                                cmd.Parameters.AddWithValue("@f", fiyat);
                                cmd.Parameters.AddWithValue("@ad", ad);
                                cmd.Parameters.AddWithValue("@ep", ep);
                                cmd.Parameters.AddWithValue("@mid", _musteriId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GoMusteriBilgileri();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================
        // İPTAL
        // =====================
        private void btnIptal_Click(object sender, EventArgs e)
        {
            GoMusteriBilgileri();
        }

        // =====================
        // COMBO HELPER
        // =====================
        private void SetComboByText(ComboBox combo, string text)
        {
            if (combo == null || string.IsNullOrWhiteSpace(text)) return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is DataRowView drv)
                {
                    string val = drv[combo.DisplayMember]?.ToString() ?? "";
                    if (string.Equals(val, text, StringComparison.OrdinalIgnoreCase))
                    {
                        combo.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        // =====================
        // MENU
        // =====================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblSergiciTanimlama_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiciTanim(_galericiId));
        }

        private void lblMusteriBilgileri_Click(object sender, EventArgs e)
        {
            this.Close();
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

            NavigateAndClose(new Form1());
        }

        // =====================
        // FOOTER
        // =====================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            NavigateDialog(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            NavigateDialog(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }
    }
}
