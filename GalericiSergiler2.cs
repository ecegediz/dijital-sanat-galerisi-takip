using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace dijitalsanatgalerisi
{
    public partial class GalericiSergiler2 : Form
    {
        private const int MAX_ILERI_GUN = 365;
        private const int MAX_SERGI_SURESI = 30;

        private readonly YeniSergiBilgisi _model;
        private readonly int _galericiId;

        private readonly MySqlConnection _baglanti =
            new MySqlConnection("Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;");

        // ====== Dirty tracking ======
        private bool _dirty = false;
        private bool _closingByCode = false;

        public GalericiSergiler2(YeniSergiBilgisi model)
        {
            InitializeComponent();
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _galericiId = model.GalericiId;

            HookEvents();
        }

        public GalericiSergiler2() : this(new YeniSergiBilgisi()) { }

        private void HookEvents()
        {
            // Load garanti
            this.Load -= GalericiSergiler2_Load;
            this.Load += GalericiSergiler2_Load;

            // X ile kapanırken sor
            this.FormClosing -= GalericiSergiler2_FormClosing;
            this.FormClosing += GalericiSergiler2_FormClosing;

            // Tarih değişince dirty
            if (dtpBaslangic != null)
            {
                dtpBaslangic.ValueChanged -= MarkDirty;
                dtpBaslangic.ValueChanged += MarkDirty;
            }
            if (dtpBitis != null)
            {
                dtpBitis.ValueChanged -= MarkDirty;
                dtpBitis.ValueChanged += MarkDirty;
            }

            // Butonlar (Designer bağlamasa bile)
            if (btnIleri1 != null)
            {
                btnIleri1.Click -= btnIleri1_Click;
                btnIleri1.Click += btnIleri1_Click;
            }

            if (btnGeri != null)
            {
                btnGeri.Click -= btnGeri_Click;
                btnGeri.Click += btnGeri_Click;
            }

            if (btnIptal1 != null)
            {
                btnIptal1.Click -= btnIptal1_Click;
                btnIptal1.Click += btnIptal1_Click;
            }

            // Menü eventleri garanti
            if (lblDashboard != null) { lblDashboard.Click -= lblDashboard_Click; lblDashboard.Click += lblDashboard_Click; }
            if (lblSergiciTanimlama != null) { lblSergiciTanimlama.Click -= lblSergiciTanimlama_Click; lblSergiciTanimlama.Click += lblSergiciTanimlama_Click; }
            if (lblMusteriBilgileri != null) { lblMusteriBilgileri.Click -= lblMusteriBilgileri_Click; lblMusteriBilgileri.Click += lblMusteriBilgileri_Click; }
            if (lblSergiler != null) { lblSergiler.Click -= lblSergiler_Click; lblSergiler.Click += lblSergiler_Click; }
            if (lblRaporEkrani != null) { lblRaporEkrani.Click -= lblRaporEkrani_Click; lblRaporEkrani.Click += lblRaporEkrani_Click; }
            if (lblGalericiAdi != null) { lblGalericiAdi.Click -= lblGalericiAdi_Click; lblGalericiAdi.Click += lblGalericiAdi_Click; }
            if (lblCikisYap != null) { lblCikisYap.Click -= lblCikisYap_Click; lblCikisYap.Click += lblCikisYap_Click; }

            // Footer
            if (lblHakkimizda != null) { lblHakkimizda.Click -= lblHakkimizda_Click; lblHakkimizda.Click += lblHakkimizda_Click; }
            if (lblYardim != null) { lblYardim.Click -= lblYardim_Click; lblYardim.Click += lblYardim_Click; }

            // Calendar bold günler
            if (monthCalendar1 != null)
            {
                monthCalendar1.DateChanged -= MonthCalendar1_DateChanged;
                monthCalendar1.DateChanged += MonthCalendar1_DateChanged;
            }
        }

        private void GalericiSergiler2_Load(object sender, EventArgs e)
        {
            if (dtpBaslangic != null)
            {
                dtpBaslangic.MinDate = DateTime.Today;
                dtpBaslangic.MaxDate = DateTime.Today.AddDays(MAX_ILERI_GUN);
            }

            if (dtpBitis != null)
            {
                dtpBitis.MinDate = DateTime.Today;
                dtpBitis.MaxDate = DateTime.Today.AddDays(MAX_ILERI_GUN);
            }

            // Modelden geri yükleme
            if (dtpBaslangic != null && _model.BaslangicTarihi != default)
                dtpBaslangic.Value = _model.BaslangicTarihi;

            if (dtpBitis != null && _model.BitisTarihi != default)
                dtpBitis.Value = _model.BitisTarihi;

            if (monthCalendar1 != null)
            {
                monthCalendar1.MinDate = DateTime.Today;
                monthCalendar1.MaxDate = DateTime.Today.AddDays(MAX_ILERI_GUN);
                DoluGunleriBoldla();
            }

            // İlk açılışta dirty sayma
            _dirty = false;
        }

        private void MonthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DoluGunleriBoldla();
        }

        private void MarkDirty(object sender, EventArgs e)
        {
            if (_closingByCode) return;
            _dirty = true;
        }

        private void GalericiSergiler2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closingByCode) return;

            if (_dirty)
            {
                var dr = MessageBox.Show(
                    "Değişikliklerden vazgeçmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private bool ConfirmDiscardIfDirty()
        {
            if (!_dirty) return true;

            var dr = MessageBox.Show(
                "Değişikliklerden vazgeçmek istediğinizden emin misiniz?\nKaydedilmemiş bilgiler silinecek.",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return dr == DialogResult.Yes;
        }

        private void Navigate(Form nextForm)
        {
            if (nextForm == null) return;

            if (!ConfirmDiscardIfDirty()) return;

            _closingByCode = true;
            nextForm.Show();
            this.Close();
        }

        private void btnIleri1_Click(object sender, EventArgs e)
        {
            DateTime bas = dtpBaslangic.Value.Date;
            DateTime bit = dtpBitis.Value.Date;

            if (bas < DateTime.Today)
            {
                MessageBox.Show("Bugünden önce tarih seçemezsiniz.");
                return;
            }
            if (bas > bit)
            {
                MessageBox.Show("Başlangıç tarihi bitişten sonra olamaz.");
                return;
            }
            if (bas > DateTime.Today.AddDays(MAX_ILERI_GUN) || bit > DateTime.Today.AddDays(MAX_ILERI_GUN))
            {
                MessageBox.Show("En fazla 1 yıl ileri tarih seçebilirsiniz.");
                return;
            }

            int gun = (bit - bas).Days + 1;
            if (gun <= 0 || gun > MAX_SERGI_SURESI)
            {
                MessageBox.Show($"Sergi süresi en fazla {MAX_SERGI_SURESI} gün olabilir.");
                return;
            }

            if (HasDateConflict(bas, bit))
            {
                MessageBox.Show("Bu tarih aralığında başka bir sergi var!", "Tarih Çakışması",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // İstersen ileri butonunda da “emin misin” sor:
            // if (_dirty && MessageBox.Show("Devam etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _model.BaslangicTarihi = bas;
            _model.BitisTarihi = bit;

            _closingByCode = true;
            new GalericiSergiler3(_model).Show();
            this.Close();
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            // Geri = sayfa değişimi => dirty sor
            Navigate(new GalericiYeniSergi(_galericiId, _model));
        }

        private void btnIptal1_Click(object sender, EventArgs e)
        {
            Navigate(new GalericiDashboard(_galericiId));
        }

        private bool HasDateConflict(DateTime bas, DateTime bit)
        {
            try
            {
                _baglanti.Open();
                string sql = @"
SELECT COUNT(*) 
FROM sergiler
WHERE GalericiID = @gid
  AND NOT (BitisTarihi < @bas OR BaslangicTarihi > @bit);";

                using (var cmd = new MySqlCommand(sql, _baglanti))
                {
                    cmd.Parameters.AddWithValue("@gid", _galericiId);
                    cmd.Parameters.AddWithValue("@bas", bas);
                    cmd.Parameters.AddWithValue("@bit", bit);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
            finally
            {
                if (_baglanti.State == ConnectionState.Open)
                    _baglanti.Close();
            }
        }

        private void DoluGunleriBoldla()
        {
            if (monthCalendar1 == null) return;

            DateTime ayBas = new DateTime(monthCalendar1.SelectionStart.Year, monthCalendar1.SelectionStart.Month, 1);
            DateTime aySon = ayBas.AddMonths(1).AddDays(-1);

            List<DateTime> doluGunler = GetDoluGunler(ayBas, aySon);

            monthCalendar1.RemoveAllBoldedDates();
            foreach (var gun in doluGunler) monthCalendar1.AddBoldedDate(gun);
            monthCalendar1.UpdateBoldedDates();
        }

        private List<DateTime> GetDoluGunler(DateTime ayBas, DateTime aySon)
        {
            var list = new List<DateTime>();

            try
            {
                _baglanti.Open();
                string sql = @"
SELECT BaslangicTarihi, BitisTarihi
FROM sergiler
WHERE GalericiID = @gid
  AND NOT (BitisTarihi < @ayBas OR BaslangicTarihi > @aySon);";

                using (var cmd = new MySqlCommand(sql, _baglanti))
                {
                    cmd.Parameters.AddWithValue("@gid", _galericiId);
                    cmd.Parameters.AddWithValue("@ayBas", ayBas);
                    cmd.Parameters.AddWithValue("@aySon", aySon);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime b = Convert.ToDateTime(dr["BaslangicTarihi"]).Date;
                            DateTime s = Convert.ToDateTime(dr["BitisTarihi"]).Date;

                            DateTime cur = b < ayBas ? ayBas : b;
                            DateTime last = s > aySon ? aySon : s;

                            while (cur <= last)
                            {
                                list.Add(cur);
                                cur = cur.AddDays(1);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (_baglanti.State == ConnectionState.Open)
                    _baglanti.Close();
            }

            return list;
        }

        // =========================
        // MENÜ / FOOTER
        // =========================
        private void lblDashboard_Click(object sender, EventArgs e) => Navigate(new GalericiDashboard(_galericiId));
        private void lblSergiciTanimlama_Click(object sender, EventArgs e) => Navigate(new GalericiSergiciTanim(_galericiId));
        private void lblMusteriBilgileri_Click(object sender, EventArgs e) => Navigate(new GalericiMusteriBilgileri(_galericiId));
        private void lblSergiler_Click(object sender, EventArgs e) => Navigate(new GalericiSergiler(_galericiId));
        private void lblRaporEkrani_Click(object sender, EventArgs e) => Navigate(new GalericiRaporlar(_galericiId));
        private void lblGalericiAdi_Click(object sender, EventArgs e) => Navigate(new GalericiProfilim(_galericiId));

        private void lblCikisYap_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;

            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            _closingByCode = true;
            new Form1().Show();
            this.Close();
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            new Hakkimizda().ShowDialog();
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfDirty()) return;
            new Yardim().ShowDialog();
        }

        // Designer alias
        private void btnGeri_Click_1(object sender, EventArgs e) => btnGeri_Click(sender, e);
    }
}
