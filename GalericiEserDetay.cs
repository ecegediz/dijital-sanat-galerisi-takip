using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiEserDetay : Form
    {
        private readonly int _eserId;
        private readonly int _sergiId;
        private readonly int _galericiId;

        private bool _eserVarMi = false;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public GalericiEserDetay(int eserId, int sergiId, int galericiId)
        {
            InitializeComponent();
            _eserId = eserId;
            _sergiId = sergiId;
            _galericiId = galericiId;

            Load -= GalericiEserDetay_Load;
            Load += GalericiEserDetay_Load;
        }

        public GalericiEserDetay() : this(0, 0, 0) { }

        private void GalericiEserDetay_Load(object sender, EventArgs e)
        {
            // Kritik: Detay sayfası genelde dialog zinciri içinde çalışır.
            // Menü/footer "ShowDialog ile aç-kapa" mantığında olmalı.
            BindMenuFooterEvents();

            if (pictureBox1 != null)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            MakeReadOnlyUI();

            if (_eserId <= 0)
            {
                _eserVarMi = false;
                MessageBox.Show("Gösterilecek eser bulunamadı (EserID=0).",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DetaylariYukle();
        }

        // =========================================================
        // ✅ NAV: dialog zincirini bozma (Hide -> ShowDialog -> Show)
        // =========================================================
        private void NavigateDialog(Form nextForm)
        {
            if (nextForm == null) return;

            this.Hide();
            nextForm.ShowDialog();
            this.Show();
        }

        // Login gibi ana akış değişecekse
        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm != null) nextForm.Show();
            this.Close();
        }

        // =========================================================
        // ✅ Designer kopsa bile menü/footer çalışsın
        // =========================================================
        private void BindMenuFooterEvents()
        {
            // Eğer bu label'lar formda yoksa null olur, sorun değil.
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

        // =========================================================
        // READONLY UI
        // =========================================================
        private void MakeReadOnlyUI()
        {
            foreach (Control c in this.Controls)
                SetReadOnlyRecursive(c);
        }

        private void SetReadOnlyRecursive(Control parent)
        {
            if (parent is TextBox tb)
            {
                tb.ReadOnly = true;
                tb.TabStop = false;
                tb.ShortcutsEnabled = true;
                tb.BackColor = SystemColors.Control;
            }

            foreach (Control child in parent.Controls)
                SetReadOnlyRecursive(child);
        }

        // =========================================================
        // LOAD DETAILS
        // =========================================================
        private void DetaylariYukle()
        {
            _eserVarMi = false;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    e.Baslik,
    e.SanatciAdi,
    e.Aciklama,
    e.Kategori,
    e.Teknik,
    e.Boyut,
    e.Agirlik,
    e.Fiyat,
    e.YapimYili,
    e.Durum,
    e.SatisdaAcikMi,
    e.SatisdaMi,
    e.Resim,
    s.SergiAdi,
    s.BaslangicTarihi,
    s.Durum AS SergiDurumu
FROM eserler e
LEFT JOIN sergiler s ON e.SergiID = s.SergiID
WHERE e.EserID = @EserID
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EserID", _eserId);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Eser bulunamadı.", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            _eserVarMi = true;

                            txtEserAdi.Text = dr["Baslik"]?.ToString() ?? "";
                            txtSanatci.Text = dr["SanatciAdi"]?.ToString() ?? "";
                            txtAciklama.Text = dr["Aciklama"]?.ToString() ?? "";
                            txtKategori.Text = dr["Kategori"]?.ToString() ?? "";
                            txtTeknik.Text = dr["Teknik"]?.ToString() ?? "";
                            txtBoyut.Text = dr["Boyut"]?.ToString() ?? "";
                            txtAgirlik.Text = dr["Agirlik"]?.ToString() ?? "";

                            decimal fiyat = dr["Fiyat"] != DBNull.Value ? Convert.ToDecimal(dr["Fiyat"]) : 0m;
                            txtFiyat.Text = fiyat.ToString("N2", new CultureInfo("tr-TR")) + " TL";

                            bool satisAcikMi = dr["SatisdaAcikMi"] != DBNull.Value && Convert.ToInt32(dr["SatisdaAcikMi"]) == 1;
                            bool satildiMi = dr["SatisdaMi"] != DBNull.Value && Convert.ToInt32(dr["SatisdaMi"]) == 1;
                            bool aktifMi = dr["Durum"] != DBNull.Value && Convert.ToInt32(dr["Durum"]) == 1;

                            if (!satisAcikMi && satildiMi) txtStokDurumu.Text = "Satıldı";
                            else if (satisAcikMi) txtStokDurumu.Text = "Satışta";
                            else txtStokDurumu.Text = "Satışta Değil";

                            txtEserDurumu.Text = aktifMi ? "Aktif" : "Pasif";

                            txtSergi.Text = dr["SergiAdi"]?.ToString() ?? "-";

                            if (dr["BaslangicTarihi"] != DBNull.Value &&
                                DateTime.TryParse(dr["BaslangicTarihi"].ToString(), out DateTime bas))
                                txtTarih.Text = bas.ToString("dd.MM.yyyy");
                            else
                                txtTarih.Text = "-";

                            txtDurumu.Text = dr["SergiDurumu"]?.ToString() ?? "-";

                            LoadImageToPictureBox(dr["Resim"], pictureBox1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _eserVarMi = false;
                MessageBox.Show("Eser detayları yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void LoadImageToPictureBox(object dbValue, PictureBox pb)
        {
            if (pb == null) return;

            // Eski resmi temizle
            if (pb.Image != null)
            {
                var old = pb.Image;
                pb.Image = null;
                old.Dispose();
            }

            if (dbValue == DBNull.Value || dbValue == null)
            {
                pb.Image = null;
                return;
            }

            try
            {
                byte[] bytes = (byte[])dbValue;
                if (bytes.Length == 0) { pb.Image = null; return; }

                using (var ms = new MemoryStream(bytes))
                using (var img = Image.FromStream(ms))
                {
                    pb.Image = new Bitmap(img);
                }
            }
            catch
            {
                pb.Image = null;
            }
        }

        // =========================================================
        // BUTTONS
        // =========================================================

        // ✅ Geri: sadece Close (EserYonetimi ShowDialog zinciri ile geri görünür)
        private void geriButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ✅ Düzenle: dialog aç + geri dönünce detay yenile
        private void duzenleButton_Click(object sender, EventArgs e)
        {
            if (!_eserVarMi)
            {
                MessageBox.Show("Düzenlenecek eser bulunamadı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new GalericiEserDetayDuzen(_eserId, _sergiId, _galericiId))
            {
                this.Hide();
                frm.ShowDialog();
                this.Show();
            }

            DetaylariYukle();
        }

        // ==========================================================
        // DESIGNER STUBLAR (kalsın)
        // ==========================================================
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox9_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox13_TextChanged(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }

        // Designer bazen label13'e kapat bağlar
        private void label13_Click(object sender, EventArgs e)
        {
            // Detay sayfasında dashboard'a "dialog" dönmek daha stabil
            NavigateDialog(new GalericiDashboard(_galericiId));
        }

        // ==========================================================
        // ✅ MENÜ: dialog zincirini bozma
        // ==========================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiDashboard(_galericiId));
        }

        private void lblSergiciTanimlama_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiciTanim(_galericiId));
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
            var ok = MessageBox.Show("Çıkış yapmak istiyor musunuz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (ok == DialogResult.Yes)
            {
                // Ana akış değişiyor
                NavigateAndClose(new Form1());
            }
        }

        // ==========================================================
        // ✅ FOOTER: dialog zincirini bozma ve geri dönüş garanti
        // ==========================================================
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
