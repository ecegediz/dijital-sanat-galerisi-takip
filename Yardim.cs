using System;
using System.Windows.Forms;

namespace dijitalsanatgalerisi
{
    public partial class Yardim : Form
    {
        public enum HomeRole
        {
            Admin = 1,
            Galerici = 2,
            Sergici = 3
        }

        private readonly HomeRole _role;
        private readonly int _userId; // GalericiID / SergiciID (Admin için 0 olabilir)

        // ✅ ZORUNLU kullanım: rol + id ile aç
        public Yardim(HomeRole role, int userId = 0)
        {
            InitializeComponent();
            _role = role;
            _userId = userId;
        }

        // Designer için (normal akışta kullanmayın)
        public Yardim() : this(HomeRole.Admin, 0) { }

        private void labelAnasayfayaDon_Click(object sender, EventArgs e) => GoHome();
        private void pictureBoxHome_Click(object sender, EventArgs e) => GoHome();

        private void GoHome()
        {
            try
            {
                switch (_role)
                {
                    case HomeRole.Admin:
                        new AdminDashboard().Show();
                        break;

                    case HomeRole.Galerici:
                        new GalericiDashboard(_userId).Show();
                        break;

                    case HomeRole.Sergici:
                        new SergiciDashboard().Show();
                        break;

                    default:
                        new Form1().Show();
                        break;
                }
            }
            catch
            {
                new Form1().Show();
            }

            this.Close();
        }

        // Designer stub’lar
        private void label4_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void Form27_Load(object sender, EventArgs e) { }
    }
}
