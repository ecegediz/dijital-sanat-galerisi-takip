using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class AdminBasvurular : Form
    {
        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public AdminBasvurular()
        {
            InitializeComponent();

            // Load eventini tekilleştir
            this.Load -= AdminBasvurular_Load;
            this.Load += AdminBasvurular_Load;
        }

        private void AdminBasvurular_Load(object sender, EventArgs e)
        {
            GridAyarla();
            HookEventsOnce();
            BasvurulariYukle();
        }

        // =========================
        // EVENTLERİ TEK BAĞLA (çift açılma önlemi)
        // =========================
        private void HookEventsOnce()
        {
            if (btnSifre != null)
            {
                btnSifre.Click -= btnSifreVer_Click;
                btnSifre.Click += btnSifreVer_Click;
            }

            if (btnSil != null)
            {
                btnSil.Click -= btnSil_Click;
                btnSil.Click += btnSil_Click;
            }

            // Menü / footer label eventleri (Designer bağlı olsa bile sorun olmaz)
            if (lblDashboard != null)
            {
                lblDashboard.Click -= lblDashboard_Click;
                lblDashboard.Click += lblDashboard_Click;
            }

            if (lblGalericiTanimlama != null)
            {
                lblGalericiTanimlama.Click -= lblGalericiTanimlama_Click;
                lblGalericiTanimlama.Click += lblGalericiTanimlama_Click;
            }

            if (lblSifreBasvurulari != null)
            {
                lblSifreBasvurulari.Click -= lblSifreBasvurulari_Click;
                lblSifreBasvurulari.Click += lblSifreBasvurulari_Click;
            }

            if (lblRaporlar != null)
            {
                lblRaporlar.Click -= lblRaporlar_Click;
                lblRaporlar.Click += lblRaporlar_Click;
            }

            if (lblCikis != null)
            {
                lblCikis.Click -= lblCikis_Click;
                lblCikis.Click += lblCikis_Click;
            }

            if (lblHakkimizda != null)
            {
                lblHakkimizda.Click -= lblHakkimizda_Click;
                lblHakkimizda.Click += lblHakkimizda_Click;
            }

            if (lblYardim != null)
            {
                lblYardim.Click -= lblYardim_Click;
                lblYardim.Click += lblYardim_Click;
            }
        }

        // =========================
        // GRID AYAR
        // =========================
        private void GridAyarla()
        {
            if (dataGridView1 == null) return;

            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =========================
        // BAŞVURULARI LİSTELE
        // =========================
        private void BasvurulariYukle()
        {
            if (dataGridView1 == null) return;

            try
            {
                using (var baglanti = new MySqlConnection(ConnStr))
                {
                    baglanti.Open();

                    string sql = @"
SELECT 
    BasvuruID,
    AdSoyad,
    Eposta,
    Mesaj
FROM galericibasvurular
WHERE OnayDurumu = 'Beklemede'
ORDER BY BasvuruID DESC;";

                    using (var adapter = new MySqlDataAdapter(sql, baglanti))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }

                    if (dataGridView1.Columns["BasvuruID"] != null)
                        dataGridView1.Columns["BasvuruID"].HeaderText = "ID";
                    if (dataGridView1.Columns["AdSoyad"] != null)
                        dataGridView1.Columns["AdSoyad"].HeaderText = "Kullanıcı";
                    if (dataGridView1.Columns["Eposta"] != null)
                        dataGridView1.Columns["Eposta"].HeaderText = "E-posta";
                    if (dataGridView1.Columns["Mesaj"] != null)
                        dataGridView1.Columns["Mesaj"].HeaderText = "Mesaj";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Başvurular yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Ortak: Seçili başvuru ID al
        // =========================
        private bool TryGetSelectedBasvuruId(out int basvuruId)
        {
            basvuruId = 0;

            if (dataGridView1 == null || dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen bir başvuru seçin!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            object idCell = dataGridView1.CurrentRow.Cells["BasvuruID"]?.Value;
            if (idCell == null || idCell == DBNull.Value)
            {
                MessageBox.Show("Seçili satır geçersiz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            basvuruId = Convert.ToInt32(idCell);
            return true;
        }

        // =========================
        // ŞİFRE VER BUTONU
        // =========================
        private void btnSifreVer_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedBasvuruId(out int basvuruId))
                return;

            string adSoyad = dataGridView1.CurrentRow.Cells["AdSoyad"]?.Value?.ToString() ?? "";
            string eposta = dataGridView1.CurrentRow.Cells["Eposta"]?.Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(eposta))
            {
                MessageBox.Show("Bu başvuruda e-posta boş. Şifre veremezsiniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // STANDART AKIŞ: Hide + ShowDialog + geri gelince liste yenile
            using (var f = new AdminGalericiTanimlama(adSoyad, eposta, basvuruId))
            {
                this.Hide();
                f.ShowDialog();
                this.Show();
            }

            BasvurulariYukle();
        }

        // =========================
        // SİL BUTONU
        // =========================
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedBasvuruId(out int basvuruId))
                return;

            var confirm = MessageBox.Show(
                "Bu başvuruyu silmek istediğinizden emin misiniz?",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var baglanti = new MySqlConnection(ConnStr))
                {
                    baglanti.Open();

                    string sqlDelete = "DELETE FROM galericibasvurular WHERE BasvuruID = @id";
                    using (var cmd = new MySqlCommand(sqlDelete, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@id", basvuruId);
                        int affected = cmd.ExecuteNonQuery();

                        if (affected <= 0)
                        {
                            MessageBox.Show("Silinecek başvuru bulunamadı.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                MessageBox.Show("Başvuru silindi.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                BasvurulariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme sırasında hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // MENÜ NAVİGASYON (STANDART)
        // =========================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminDashboard())
                f.ShowDialog();
            this.Show();
            BasvurulariYukle();
        }

        private void lblGalericiTanimlama_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminGalericiTanimlama())
                f.ShowDialog();
            this.Show();
            BasvurulariYukle();
        }

        private void lblSifreBasvurulari_Click(object sender, EventArgs e)
        {
            // Zaten bu sayfa: sadece yenile
            BasvurulariYukle();
        }

        private void lblRaporlar_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new AdminRaporlar())
                f.ShowDialog();
            this.Show();
            BasvurulariYukle();
        }

        private void lblCikis_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            // Form1 ShowDialog zinciri ile geri gelir
            this.Close();
        }

        // =========================
        // FOOTER (STANDART)
        // =========================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new Hakkimizda(Hakkimizda.HomeRole.Admin, 0))
                f.ShowDialog();
            this.Show();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new Yardim(Yardim.HomeRole.Admin, 0))
                f.ShowDialog();
            this.Show();
        }

        // Designer stub’lar (kalsın)
        private void label3_Click(object sender, EventArgs e) { }
        private void Form6_Load(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}
