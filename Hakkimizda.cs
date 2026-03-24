using System;
using System.Windows.Forms;

namespace dijitalsanatgalerisi
{
    public partial class Hakkimizda : Form
    {
        public enum HomeRole
        {
            Admin = 1,
            Galerici = 2,
            Sergici = 3
        }

        private readonly HomeRole _role;
        private readonly int _userId; // GalericiID veya SergiciID (Admin için 0 olabilir)

        // ✅ ZORUNLU: rol + id ile aç
        public Hakkimizda(HomeRole role, int userId = 0)
        {
            InitializeComponent();
            _role = role;
            _userId = userId;
        }

        // Designer için (asla normal akışta bunu kullanma)
        public Hakkimizda() : this(HomeRole.Admin, 0) { }

        // "Anasayfaya Dön" label/picturebox click
        private void labelAnasayfayaDon_Click(object sender, EventArgs e) => GoHome();
        private void pictureBoxHome_Click(object sender, EventArgs e) => GoHome();

        private void GoHome()
        {
            try
            {
                switch (_role)
                {
                    case HomeRole.Admin:
                        new AdminDashboard().Show(); // ✅ Admin ana sayfa
                        break;

                    case HomeRole.Galerici:
                        // ✅ Galerici ana sayfa (ID şart)
                        new GalericiDashboard(_userId).Show();
                        break;

                    case HomeRole.Sergici:
                        // ✅ Sergici ana sayfa
                        new SergiciDashboard().Show();
                        break;

                    default:
                        new Form1().Show(); // emniyet
                        break;
                }
            }
            catch
            {
                // her ihtimale karşı
                new Form1().Show();
            }

            this.Close();
        }

        // Stub’lar kalsın
        private void label10_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label15_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
    }
}
