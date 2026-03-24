using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class Form1 : Form
    {
        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public Form1()
        {
            InitializeComponent();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            string eposta = (textBox1.Text ?? "").Trim();
            string sifre = (textBox2.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(eposta) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen e-posta ve şifre giriniz!");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    // 1) GALERICILER
                    string sqlGal = @"
SELECT GalericiID, KullaniciTipi, COALESCE(AbonelikDurumu,'Pasif') AS AbonelikDurumu
FROM galericiler
WHERE Eposta = @eposta AND Sifre = @sifre
LIMIT 1;";

                    using (var cmd = new MySqlCommand(sqlGal, conn))
                    {
                        cmd.Parameters.AddWithValue("@eposta", eposta);
                        cmd.Parameters.AddWithValue("@sifre", sifre);

                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                string tip = (r["KullaniciTipi"]?.ToString() ?? "").Trim();
                                string durum = (r["AbonelikDurumu"]?.ToString() ?? "Pasif").Trim();

                                if (string.Equals(tip, "Admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    this.Hide();
                                    using (var f = new AdminDashboard())
                                        f.ShowDialog();
                                    this.Show();
                                    return;
                                }

                                if (string.Equals(tip, "Galerici", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.Equals(durum, "Aktif", StringComparison.OrdinalIgnoreCase))
                                    {
                                        MessageBox.Show(
                                            "Hesabınız pasif durumda. Giriş yapabilmek için aboneliğinizin aktif olması gerekiyor.",
                                            "Pasif Hesap",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                                        return;
                                    }

                                    int galericiId = Convert.ToInt32(r["GalericiID"]);

                                    this.Hide();
                                    using (var f = new GalericiDashboard(galericiId))
                                        f.ShowDialog();
                                    this.Show();
                                    return;
                                }
                            }
                        }
                    }

                    // 2) SERGICILER
                    string sqlSer = @"
SELECT SanatciID, AdSoyad
FROM sergiciler
WHERE Eposta = @eposta AND Sifre = @sifre
LIMIT 1;";

                    int sanatciIdFound = 0;
                    string adSoyadFound = "";

                    using (var cmd2 = new MySqlCommand(sqlSer, conn))
                    {
                        cmd2.Parameters.AddWithValue("@eposta", eposta);
                        cmd2.Parameters.AddWithValue("@sifre", sifre);

                        using (var r2 = cmd2.ExecuteReader())
                        {
                            if (r2.Read())
                            {
                                sanatciIdFound = Convert.ToInt32(r2["SanatciID"]);
                                adSoyadFound = (r2["AdSoyad"]?.ToString() ?? "").Trim();
                            }
                        }
                    }

                    if (sanatciIdFound > 0)
                    {
                        // Durum kolonu kontrol
                        bool hasDurumColumn;
                        using (var cmdCheck = new MySqlCommand(@"
SELECT COUNT(*) 
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'sergiciler'
  AND COLUMN_NAME = 'Durum';", conn))
                        {
                            hasDurumColumn = Convert.ToInt64(cmdCheck.ExecuteScalar()) > 0;
                        }

                        if (hasDurumColumn)
                        {
                            using (var cmdDurum = new MySqlCommand(
                                "SELECT COALESCE(Durum,'Aktif') FROM sergiciler WHERE SanatciID=@id LIMIT 1;", conn))
                            {
                                cmdDurum.Parameters.AddWithValue("@id", sanatciIdFound);
                                string durum = (cmdDurum.ExecuteScalar()?.ToString() ?? "Aktif").Trim();

                                if (!string.Equals(durum, "Aktif", StringComparison.OrdinalIgnoreCase))
                                {
                                    MessageBox.Show(
                                        "Hesabınız pasif durumda. Giriş yapamazsınız.",
                                        "Pasif Hesap",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        this.Hide();
                        using (var f = new SergiciDashboard(sanatciIdFound, adSoyadFound))
                            f.ShowDialog();
                        this.Show();
                        return;
                    }

                    MessageBox.Show("Hatalı e-posta veya şifre!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası:\n" + ex.Message);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            using (var f = new GalericiBasvuru())
                f.ShowDialog();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            using (var f = new Hakkimizda())
                f.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            using (var f = new Yardim())
                f.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void label7_Click_1(object sender, EventArgs e) { }
    }
}
