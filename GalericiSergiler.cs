using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiler : Form
    {
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private DataTable _sergiTablosu;

        // Slider controls (Designer Name'leri ile eşleşmeli)
        private PictureBox SliderPictureBox => picSlider;
        private Button PrevButton => btnPrev;
        private Button NextButton => btnNext;
        private Label EserAdiLabel => lblEserAdi;

        private readonly List<EserImageItem> _eserImages = new List<EserImageItem>();
        private int _currentIndex = -1;

        // ✅ Tek tık = tek navigasyon kilidi
        private bool _isNavigating = false;

        public GalericiSergiler(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            // ✅ Eventleri 1 kere bağla (Load içinde bağlama yok)
            this.Load += GalericiSergiler_Load;

            if (dataGridView1 != null)
            {
                dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
                dataGridView1.CellClick += dataGridView1_CellClick;
            }

            if (PrevButton != null) PrevButton.Click += PrevButton_Click;
            if (NextButton != null) NextButton.Click += NextButton_Click;

            if (comboBoxFiltre != null) comboBoxFiltre.SelectedIndexChanged += comboBoxFiltre_SelectedIndexChanged;

            if (SliderPictureBox != null)
            {
                SliderPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                SafeSetSliderImage(null);
            }
        }

        public GalericiSergiler() : this(0) { }

        private void GalericiSergiler_Load(object sender, EventArgs e)
        {
            SetupFiltreCombo();
            SyncSergiDurumlari();

            SergileriYukle();
            GridAyarla();

            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells
                    .Cast<DataGridViewCell>()
                    .FirstOrDefault(c => c.Visible) ?? dataGridView1.Rows[0].Cells[0];
            }

            DetaylariGuncelle();
        }

        // =========================================================
        // ✅ NAV (Kilitlemeli)
        // =========================================================
        private void NavigateDialog(Form nextForm)
        {
            if (nextForm == null) return;
            if (_isNavigating) return;

            _isNavigating = true;
            this.Hide();
            try
            {
                nextForm.ShowDialog(this);
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    this.Show();
                    this.BringToFront();
                }
                _isNavigating = false;
            }

            RefreshAll();
        }

        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;
            if (_isNavigating) return;

            _isNavigating = true;
            nextForm.Show();
            this.Close();
        }

        private void RefreshAll()
        {
            SyncSergiDurumlari();
            SergileriYukle();
            GridAyarla();

            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells
                    .Cast<DataGridViewCell>()
                    .FirstOrDefault(c => c.Visible) ?? dataGridView1.Rows[0].Cells[0];
            }

            DetaylariGuncelle();
        }

        // =========================================================
        // Filtre Combo
        // =========================================================
        private void SetupFiltreCombo()
        {
            if (comboBoxFiltre == null) return;

            comboBoxFiltre.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFiltre.Items.Clear();
            comboBoxFiltre.Items.Add("Hepsi");
            comboBoxFiltre.Items.Add("Aktif");
            comboBoxFiltre.Items.Add("Geçmiş");
            comboBoxFiltre.Items.Add("Bekleyen");
            comboBoxFiltre.SelectedIndex = 0;
        }

        private void comboBoxFiltre_SelectedIndexChanged(object sender, EventArgs e)
        {
            // filtre değişince aynı "Ara" mantığını çalıştır
            btnAra_Click(sender, EventArgs.Empty);
        }

        // =========================================================
        // Durum senkron (Aktif/Geçmiş/Bekleyen)
        // =========================================================
        private void SyncSergiDurumlari()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(@"
UPDATE sergiler
SET Durum='Geçmiş'
WHERE GalericiID=@gid
  AND BitisTarihi < CURDATE()
  AND (Durum='Aktif' OR Durum IS NULL OR Durum='');", conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new MySqlCommand(@"
UPDATE sergiler
SET Durum='Aktif'
WHERE GalericiID=@gid
  AND CURDATE() BETWEEN DATE(BaslangicTarihi) AND DATE(BitisTarihi)
  AND (Durum IS NULL OR Durum='' OR Durum='Bekleyen');", conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new MySqlCommand(@"
UPDATE sergiler
SET Durum='Bekleyen'
WHERE GalericiID=@gid
  AND BaslangicTarihi > CURDATE()
  AND (Durum IS NULL OR Durum='' OR Durum='Aktif');", conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // sessiz geçilebilir
            }
        }

        // =========================================================
        // Sergileri yükle
        // =========================================================
        private void SergileriYukle()
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    s.SergiID,
    s.SergiAdi,
    s.SergiTuru,
    s.SergiTemasi,
    s.HedefKitle,
    s.Kapasite,
    COUNT(e.EserID) AS EserSayisi,
    s.BaslangicTarihi,
    s.BitisTarihi,
    s.Sergici,
    s.Durum
FROM sergiler s
LEFT JOIN eserler e ON e.SergiID = s.SergiID
WHERE (@gid = 0 OR s.GalericiID = @gid)
GROUP BY
    s.SergiID, s.SergiAdi, s.SergiTuru, s.SergiTemasi, s.HedefKitle, s.Kapasite,
    s.BaslangicTarihi, s.BitisTarihi, s.Sergici, s.Durum
ORDER BY s.BaslangicTarihi DESC;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            _sergiTablosu = new DataTable();
                            da.Fill(_sergiTablosu);
                        }
                    }
                }

                if (dataGridView1 == null) return;

                dataGridView1.DataSource = null;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = _sergiTablosu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergiler yüklenirken hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridAyarla()
        {
            if (dataGridView1 == null || dataGridView1.DataSource == null) return;

            if (dataGridView1.Columns["SergiID"] != null)
                dataGridView1.Columns["SergiID"].Visible = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.RowHeadersVisible = false;

            if (dataGridView1.Columns["SergiAdi"] != null) dataGridView1.Columns["SergiAdi"].HeaderText = "Sergi Adı";
            if (dataGridView1.Columns["SergiTuru"] != null) dataGridView1.Columns["SergiTuru"].HeaderText = "Sergi Türü";
            if (dataGridView1.Columns["SergiTemasi"] != null) dataGridView1.Columns["SergiTemasi"].HeaderText = "Tema";
            if (dataGridView1.Columns["HedefKitle"] != null) dataGridView1.Columns["HedefKitle"].HeaderText = "Hedef Kitle";
            if (dataGridView1.Columns["Kapasite"] != null) dataGridView1.Columns["Kapasite"].HeaderText = "Kapasite";
            if (dataGridView1.Columns["EserSayisi"] != null) dataGridView1.Columns["EserSayisi"].HeaderText = "Eser Sayısı";
            if (dataGridView1.Columns["BaslangicTarihi"] != null) dataGridView1.Columns["BaslangicTarihi"].HeaderText = "Başlangıç Tarihi";
            if (dataGridView1.Columns["BitisTarihi"] != null) dataGridView1.Columns["BitisTarihi"].HeaderText = "Bitiş Tarihi";
            if (dataGridView1.Columns["Sergici"] != null) dataGridView1.Columns["Sergici"].HeaderText = "Sergici";
            if (dataGridView1.Columns["Durum"] != null) dataGridView1.Columns["Durum"].HeaderText = "Durum";

            EnsureDuzenleButonu();
            EnsureSilButonu();

            if (dataGridView1.Columns["Duzenle"] != null)
                dataGridView1.Columns["Duzenle"].DisplayIndex = dataGridView1.Columns.Count - 2;
            if (dataGridView1.Columns["Sil"] != null)
                dataGridView1.Columns["Sil"].DisplayIndex = dataGridView1.Columns.Count - 1;
        }

        private void EnsureDuzenleButonu()
        {
            if (dataGridView1 == null) return;
            if (dataGridView1.Columns["Duzenle"] != null) return;

            var editBtn = new DataGridViewButtonColumn
            {
                Name = "Duzenle",
                HeaderText = "Düzenle",
                Text = "Düzenle",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dataGridView1.Columns.Add(editBtn);
        }

        private void EnsureSilButonu()
        {
            if (dataGridView1 == null) return;
            if (dataGridView1.Columns["Sil"] != null) return;

            var delBtn = new DataGridViewButtonColumn
            {
                Name = "Sil",
                HeaderText = "Sil",
                Text = "Sil",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dataGridView1.Columns.Add(delBtn);
        }

        // =========================================================
        // DETAY + SLIDER
        // =========================================================
        private void DetaylariGuncelle()
        {
            if (dataGridView1 == null || dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.DataBoundItem == null)
            {
                lblSergiAdi.Text = "";
                lblTarih.Text = "";
                lblSergici.Text = "";
                lblDurum.Text = "";
                ClearSlider("Sergi seçilmedi.");
                return;
            }

            var rowView = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                ClearSlider("Sergi seçilmedi.");
                return;
            }

            DataRow row = rowView.Row;

            lblSergiAdi.Text = row["SergiAdi"]?.ToString() ?? "";
            lblSergici.Text = row["Sergici"]?.ToString() ?? "";

            if (DateTime.TryParse(Convert.ToString(row["BaslangicTarihi"]), out DateTime bas) &&
                DateTime.TryParse(Convert.ToString(row["BitisTarihi"]), out DateTime bit))
                lblTarih.Text = $"{bas:dd MMMM yyyy} - {bit:dd MMMM yyyy}";
            else
                lblTarih.Text = "";

            string durum = row["Durum"]?.ToString() ?? "";
            lblDurum.Text = durum;

            if (durum.Equals("Aktif", StringComparison.OrdinalIgnoreCase))
                lblDurum.ForeColor = Color.ForestGreen;
            else if (durum.Equals("Geçmiş", StringComparison.OrdinalIgnoreCase))
                lblDurum.ForeColor = Color.Gray;
            else
                lblDurum.ForeColor = Color.OrangeRed;

            int sergiId = Convert.ToInt32(row["SergiID"]);
            LoadEserImagesForSergi(sergiId);

            _currentIndex = _eserImages.Count > 0 ? 0 : -1;
            ShowCurrentImage();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) => DetaylariGuncelle();

        private void LoadEserImagesForSergi(int sergiId)
        {
            _eserImages.Clear();

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT EserID, Baslik, Resim
FROM eserler
WHERE SergiID=@sid
  AND Resim IS NOT NULL
ORDER BY EserID;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", sergiId);

                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                object resimObj = rd["Resim"];
                                if (resimObj == null || resimObj == DBNull.Value) continue;
                                if (!(resimObj is byte[] bytes)) continue;
                                if (bytes.Length == 0) continue;

                                _eserImages.Add(new EserImageItem
                                {
                                    EserID = Convert.ToInt32(rd["EserID"]),
                                    Baslik = rd["Baslik"]?.ToString() ?? "",
                                    ImageBytes = bytes
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                // sessiz geçilebilir
            }
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (_eserImages.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _eserImages.Count - 1;
            ShowCurrentImage();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (_eserImages.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _eserImages.Count) _currentIndex = 0;
            ShowCurrentImage();
        }

        private void ShowCurrentImage()
        {
            if (_currentIndex < 0 || _currentIndex >= _eserImages.Count)
            {
                ClearSlider("Bu sergide fotoğraflı eser bulunamadı.");
                return;
            }

            var item = _eserImages[_currentIndex];

            if (SliderPictureBox != null)
            {
                using (var ms = new MemoryStream(item.ImageBytes))
                {
                    var newBmp = new Bitmap(Image.FromStream(ms));
                    SafeSetSliderImage(newBmp);
                }
            }

            if (EserAdiLabel != null)
                EserAdiLabel.Text = item.Baslik;

            UpdateSliderButtons();
        }

        private void SafeSetSliderImage(Image newImage)
        {
            if (SliderPictureBox == null) return;

            var old = SliderPictureBox.Image;
            SliderPictureBox.Image = newImage;

            if (old != null)
            {
                try { old.Dispose(); } catch { }
            }
        }

        private void UpdateSliderButtons()
        {
            bool has = _eserImages.Count > 0;
            if (PrevButton != null) PrevButton.Enabled = has;
            if (NextButton != null) NextButton.Enabled = has;
        }

        private void ClearSlider(string message)
        {
            _eserImages.Clear();
            _currentIndex = -1;

            SafeSetSliderImage(null);
            if (EserAdiLabel != null) EserAdiLabel.Text = message;

            UpdateSliderButtons();
        }

        private class EserImageItem
        {
            public int EserID { get; set; }
            public string Baslik { get; set; }
            public byte[] ImageBytes { get; set; }
        }

        // =========================================================
        // DÜZENLE / SİL
        // =========================================================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null) return;
            if (e.RowIndex < 0) return;

            dataGridView1.ClearSelection();
            dataGridView1.Rows[e.RowIndex].Selected = true;

            int sergiId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["SergiID"].Value);
            string sergiAdi = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["SergiAdi"]?.Value ?? "");

            if (dataGridView1.Columns["Duzenle"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Duzenle"].Index)
            {
                NavigateDialog(new GalericiSergiDuzenleme(_galericiId, sergiId));
                return;
            }

            if (dataGridView1.Columns["Sil"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Sil"].Index)
            {
                var confirm = MessageBox.Show(
                    $"'{sergiAdi}' adlı sergiyi silmek istediğinizden emin misiniz?\n" +
                    "Bu sergiye bağlı eserler ve ilişkili kayıtlar da silinecektir.",
                    "Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                if (DeleteSergiCascade(sergiId))
                {
                    MessageBox.Show("Sergi ve bağlı kayıtlar silindi.", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefreshAll();
                }
                return;
            }
        }

        private bool DeleteSergiCascade(int sergiId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        if (ColumnExists(conn, tx, "satislar", "SergiID"))
                        {
                            using (var cmd = new MySqlCommand("DELETE FROM satislar WHERE SergiID=@id;", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", sergiId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        if (ColumnExists(conn, tx, "odemeler", "SergiID"))
                        {
                            using (var cmd = new MySqlCommand("DELETE FROM odemeler WHERE SergiID=@id;", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", sergiId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM eserler WHERE SergiID=@id;", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", sergiId);
                            cmd.ExecuteNonQuery();
                        }

                        int affected;
                        using (var cmd = new MySqlCommand(
                            "DELETE FROM sergiler WHERE SergiID=@id AND (@gid=0 OR GalericiID=@gid);",
                            conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", sergiId);
                            cmd.Parameters.AddWithValue("@gid", _galericiId);
                            affected = cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme işlemi sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool ColumnExists(MySqlConnection conn, MySqlTransaction tx, string tableName, string columnName)
        {
            using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = @t
  AND COLUMN_NAME = @c;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@t", tableName);
                cmd.Parameters.AddWithValue("@c", columnName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // =========================================================
        // ARA / FİLTRE
        // =========================================================
        private void btnAra_Click(object sender, EventArgs e)
        {
            if (_sergiTablosu == null) return;

            string arama = (textBox1?.Text ?? "").Trim();
            string durumSecimi = comboBoxFiltre?.SelectedItem?.ToString() ?? "Hepsi";

            var view = _sergiTablosu.DefaultView;
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(arama))
            {
                string temiz = arama.Replace("'", "''");
                parts.Add($"SergiAdi LIKE '%{temiz}%'");
            }

            if (durumSecimi == "Aktif" || durumSecimi == "Geçmiş" || durumSecimi == "Bekleyen")
                parts.Add($"Durum = '{durumSecimi}'");

            view.RowFilter = string.Join(" AND ", parts);

            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells
                    .Cast<DataGridViewCell>()
                    .FirstOrDefault(c => c.Visible) ?? dataGridView1.Rows[0].Cells[0];
            }

            DetaylariGuncelle();
        }

        // =========================================================
        // ESER YÖNETİMİ
        // =========================================================
        private void btnEserYonetimi_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen bir sergi seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int sergiId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["SergiID"].Value);
            NavigateAndClose(new GalericiEserYonetimi(sergiId, _galericiId));

        }

        // =========================================================
        // YENİ SERGİ EKLE
        // =========================================================
        private void btnSergiEkle_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen tablodan sergi sahibini seçiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sergiciAd = Convert.ToString(dataGridView1.CurrentRow.Cells["Sergici"]?.Value ?? "").Trim();
            if (string.IsNullOrWhiteSpace(sergiciAd))
            {
                MessageBox.Show("Seçilen kayıtta sergici bilgisi yok. Lütfen başka bir sergi seçiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryGetSergiciByAd(sergiciAd, out int sanatciId, out string eposta))
            {
                var res = MessageBox.Show(
                    $"'{sergiciAd}' adlı sergici sistemde kayıtlı değil.\n" +
                    "Yeni sergi oluşturmak için önce sergiciyi tanımlamalısınız.\n\n" +
                    "Sergici tanımlama ekranına geçilsin mi?",
                    "Uyarı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                {
                    NavigateDialog(new GalericiSergiciTanim(_galericiId));
                }
                return;
            }

            var model = new YeniSergiBilgisi
            {
                GalericiId = _galericiId,
                SanatciID = sanatciId,
                SergiciAdSoyad = sergiciAd,
                SergiciEposta = eposta
            };

            NavigateDialog(new GalericiYeniSergi(_galericiId, model));
        }

        private bool TryGetSergiciByAd(string adSoyad, out int sanatciId, out string eposta)
        {
            sanatciId = 0;
            eposta = "";

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT SanatciID, Eposta
FROM sergiciler
WHERE GalericiID=@gid AND AdSoyad=@ad
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", _galericiId);
                        cmd.Parameters.AddWithValue("@ad", adSoyad);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read()) return false;

                            sanatciId = Convert.ToInt32(dr["SanatciID"]);
                            eposta = Convert.ToString(dr["Eposta"] ?? "");
                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // =========================================================
        // MENÜ + FOOTER (Designer Click eventleri bağlı olmalı)
        // =========================================================
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
            NavigateDialog(new GalericiMusteriBilgileri(_galericiId));
        }

        private void lblSergiler_Click(object sender, EventArgs e)
        {
            RefreshAll();
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
