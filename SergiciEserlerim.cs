using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class SergiciEserlerim : Form
    {
        private readonly int _sergiciId;
        private readonly int? _sergiIdFilter;
        private string _sergiciAdi;

        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public SergiciEserlerim(int sergiciId, string sergiciAdi) : this(sergiciId, sergiciAdi, null) { }

        // ✅ Yeni: sergi filtreli açılabilsin
        public SergiciEserlerim(int sergiciId, string sergiciAdi, int? sergiIdFilter)
        {
            InitializeComponent();

            _sergiciId = sergiciId;
            _sergiciAdi = (sergiciAdi ?? "").Trim();
            _sergiIdFilter = sergiIdFilter;

            HookMenuAndFooterClicks();

            this.Load -= SergiciEserlerim_Load;
            this.Load += SergiciEserlerim_Load;

            dataGridViewEserler.SelectionChanged -= dataGridViewEserler_SelectionChanged;
            dataGridViewEserler.SelectionChanged += dataGridViewEserler_SelectionChanged;
        }

        public SergiciEserlerim() : this(0, "", null) { }

        private void NavigateTo(Form next)
        {
            next.FormClosed += (s, e) => this.Close();
            next.Show();
            this.Hide();
        }

        private void HookMenuAndFooterClicks()
        {
            if (dashboardButton != null) dashboardButton.Click += dashboardButton_Click_Click;
            if (eserlerimButton != null) eserlerimButton.Click += eserlerimButton_Click;
            if (raporButton != null) raporButton.Click += raporButton_Click;
            if (cikisYapButton != null) cikisYapButton.Click += cikisYapButton_Click;

            if (labelSergiciAdi != null) labelSergiciAdi.Click += labelSergiciAdi_Click;

            if (hakkımızdaButton != null) hakkımızdaButton.Click += hakkımızdaButton_Click;
            if (label1 != null) label1.Click += label1_Click;
        }

        private void SergiciEserlerim_Load(object sender, EventArgs e)
        {
            if (_sergiciId <= 0)
            {
                MessageBox.Show("Eserlerim açılırken SergiciID=0 geldi. ID taşımıyorsun.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(_sergiciAdi))
                _sergiciAdi = SergiciAdiniDbdenGetir();

            if (labelSergiciAdi != null)
                labelSergiciAdi.Text = string.IsNullOrWhiteSpace(_sergiciAdi) ? "—" : _sergiciAdi;

            EserleriGetir();
        }

        private string SergiciAdiniDbdenGetir()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT AdSoyad FROM sergiciler WHERE SanatciID=@Id LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _sergiciId);
                        return (cmd.ExecuteScalar()?.ToString() ?? "").Trim();
                    }
                }
            }
            catch { return ""; }
        }

        // ✅ Eğer _sergiIdFilter doluysa sadece o serginin eserleri gelir
        private void EserleriGetir()
        {
            try
            {
                DataTable dt = new DataTable();

                string sql = @"
SELECT
    e.EserID                                   AS EserID,
    COALESCE(se.SergiAdi, '-')                 AS SergiAdi,
    e.Baslik                                   AS EserAdi,
    e.SanatciAdi                               AS Sanatci,
    e.Fiyat                                    AS Fiyat,
    e.Durum                                    AS Durum,
    e.Teknik                                   AS Teknik,
    e.Boyut                                    AS Boyut,
    e.YapimYili                                AS YapimYili,
    e.Kategori                                 AS Kategori,
    e.Aciklama                                 AS Aciklama,
    e.Agirlik                                  AS Agirlik,
    e.Resim                                    AS ResimBlob
FROM eserler e
LEFT JOIN sergiler se ON se.SergiID = e.SergiID
WHERE se.SanatciID = @SanatciID
  AND (@SergiID IS NULL OR se.SergiID = @SergiID)
ORDER BY e.EserID DESC;";

                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@SanatciID", _sergiciId);
                    da.SelectCommand.Parameters.AddWithValue("@SergiID", (object)_sergiIdFilter ?? DBNull.Value);
                    da.Fill(dt);
                }

                dataGridViewEserler.AutoGenerateColumns = true;
                dataGridViewEserler.DataSource = dt;

                dataGridViewEserler.ReadOnly = true;
                dataGridViewEserler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewEserler.MultiSelect = false;
                dataGridViewEserler.AllowUserToAddRows = false;
                dataGridViewEserler.AllowUserToDeleteRows = false;
                dataGridViewEserler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridViewEserler.Columns["ResimBlob"] != null)
                    dataGridViewEserler.Columns["ResimBlob"].Visible = false;

                if (dataGridViewEserler.Columns["EserID"] != null) dataGridViewEserler.Columns["EserID"].HeaderText = "Eser ID";
                if (dataGridViewEserler.Columns["SergiAdi"] != null) dataGridViewEserler.Columns["SergiAdi"].HeaderText = "Sergi Adı";
                if (dataGridViewEserler.Columns["EserAdi"] != null) dataGridViewEserler.Columns["EserAdi"].HeaderText = "Eser Adı";
                if (dataGridViewEserler.Columns["Sanatci"] != null) dataGridViewEserler.Columns["Sanatci"].HeaderText = "Sanatçı";
                if (dataGridViewEserler.Columns["Fiyat"] != null) dataGridViewEserler.Columns["Fiyat"].HeaderText = "Fiyat";
                if (dataGridViewEserler.Columns["Durum"] != null) dataGridViewEserler.Columns["Durum"].HeaderText = "Durum";

                if (dataGridViewEserler.Rows.Count > 0)
                {
                    dataGridViewEserler.ClearSelection();
                    dataGridViewEserler.Rows[0].Selected = true;
                    ApplySelectedRowToRightPanel(dataGridViewEserler.Rows[0]);
                }
                else
                {
                    ClearRightPanel();
                    MessageBox.Show("Bu filtreye uygun eser bulunamadı.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eserler yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewEserler_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewEserler.CurrentRow == null) return;
            ApplySelectedRowToRightPanel(dataGridViewEserler.CurrentRow);
        }

        private void ApplySelectedRowToRightPanel(DataGridViewRow row)
        {
            SetText(lblEserId, row, "EserID");
            SetText(lblSergiAdi, row, "SergiAdi");
            SetText(lblEserAdi, row, "EserAdi");
            SetText(lblSanatci, row, "Sanatci");
            SetText(lblFiyat, row, "Fiyat");
            SetText(lblDurum, row, "Durum");

            LoadSelectedImageFromRow(row);
        }

        private void SetText(Control c, DataGridViewRow row, string colName)
        {
            if (c == null) return;
            if (row?.DataGridView?.Columns == null) { c.Text = ""; return; }
            if (!row.DataGridView.Columns.Contains(colName)) { c.Text = ""; return; }

            object v = row.Cells[colName].Value;
            c.Text = (v == null || v == DBNull.Value) ? "" : v.ToString();
        }

        private void LoadSelectedImageFromRow(DataGridViewRow row)
        {
            if (pictureBoxEser == null) return;

            pictureBoxEser.Image = null;
            pictureBoxEser.SizeMode = PictureBoxSizeMode.Zoom;

            if (row?.DataGridView?.Columns == null) return;
            if (!row.DataGridView.Columns.Contains("ResimBlob")) return;

            object blobObj = row.Cells["ResimBlob"].Value;
            if (blobObj == null || blobObj == DBNull.Value) return;

            try
            {
                byte[] bytes = (byte[])blobObj;
                using (var ms = new MemoryStream(bytes))
                {
                    pictureBoxEser.Image = Image.FromStream(ms);
                }
            }
            catch
            {
                pictureBoxEser.Image = null;
            }
        }

        private void ClearRightPanel()
        {
            if (pictureBoxEser != null) pictureBoxEser.Image = null;

            if (lblEserId != null) lblEserId.Text = "";
            if (lblSergiAdi != null) lblSergiAdi.Text = "";
            if (lblEserAdi != null) lblEserAdi.Text = "";
            if (lblSanatci != null) lblSanatci.Text = "";
            if (lblFiyat != null) lblFiyat.Text = "";
            if (lblDurum != null) lblDurum.Text = "";
        }

        // ===== MENÜ EVENTLERİ =====
        private void dashboardButton_Click_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciDashboard(_sergiciId, _sergiciAdi));
        }

        private void eserlerimButton_Click(object sender, EventArgs e)
        {
            // Menüden gelince filtreyi kaldırıp tüm eserleri göster:
            NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi, null));
        }

        private void raporButton_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciRaporlar(_sergiciId, _sergiciAdi));
        }

        private void cikisYapButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NavigateTo(new Form1());
            }
        }

        private void labelSergiciAdi_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciProfilim(_sergiciId, _sergiciAdi));
        }

        // ===== FOOTER EVENTLERİ =====
        private void hakkımızdaButton_Click(object sender, EventArgs e)
        {
            new Hakkimizda().Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            new Yardim().Show();
        }

        // Designer stub
        private void pictureBoxEser_Click(object sender, EventArgs e) { }
        private void dataGridViewEserler_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
