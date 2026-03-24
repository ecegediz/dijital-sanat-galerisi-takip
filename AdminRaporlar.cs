using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClosedXML.Excel;

// QuestPDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace dijitalsanatgalerisi
{
    public partial class AdminRaporlar : Form
    {
        private readonly string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private DataTable galleryData;

        // ✅ Bu formu kim açtıysa geri göstermek için
        private readonly Form _owner;

        public AdminRaporlar() : this(null) { }

        public AdminRaporlar(Form owner)
        {
            InitializeComponent();
            _owner = owner;

            InitializeGalleryData();

            // Designer'da Form7_Load bağlıysa uyumlu kalsın
            this.Load -= Form7_Load;
            this.Load += Form7_Load;

            // Form tekrar aktif olunca yenile
            this.Activated -= AdminRaporlar_Activated;
            this.Activated += AdminRaporlar_Activated;

            // ✅ ÖNEMLİ: Designer isimleri farklı olsa bile patlamasın
            BindOptionalEventsByName();
        }

        private void AdminRaporlar_Activated(object sender, EventArgs e)
        {
            YenidenYukle();
        }

        // =========================
        // SAFE FIND HELPERS
        // =========================
        private T FindCtrl<T>(string name) where T : Control
        {
            return this.Controls.Find(name, true).FirstOrDefault() as T;
        }

        private void BindOptionalEventsByName()
        {
            // --- Buttons (Designer bazen farklı isim bağlar) ---
            var btnPdf = FindCtrl<Button>("btnPdf");
            if (btnPdf != null)
            {
                btnPdf.Click -= btnPdf_Click;
                btnPdf.Click += btnPdf_Click;
            }

            var btnExcel = FindCtrl<Button>("btnExcel");
            if (btnExcel != null)
            {
                btnExcel.Click -= btnExcel_Click;
                btnExcel.Click += btnExcel_Click;
            }

            // Designer bazen "button1" kullanıyor olabilir
            var button1 = FindCtrl<Button>("button1");
            if (button1 != null)
            {
                button1.Click -= button1_Click;
                button1.Click += button1_Click;
            }

            // --- Menu / Footer Labels (varsa bağla) ---
            var lblDashboard = FindCtrl<Label>("lblDashboard");
            if (lblDashboard != null)
            {
                lblDashboard.Click -= lblDashboard_Click;
                lblDashboard.Click += lblDashboard_Click;
            }

            var lblGalericiTanimlama = FindCtrl<Label>("lblGalericiTanimlama");
            if (lblGalericiTanimlama != null)
            {
                lblGalericiTanimlama.Click -= lblGalericiTanimlama_Click;
                lblGalericiTanimlama.Click += lblGalericiTanimlama_Click;
            }

            var lblSifreBasvurulari = FindCtrl<Label>("lblSifreBasvurulari");
            if (lblSifreBasvurulari != null)
            {
                lblSifreBasvurulari.Click -= lblSifreBasvurulari_Click;
                lblSifreBasvurulari.Click += lblSifreBasvurulari_Click;
            }

            var lblRaporlar = FindCtrl<Label>("lblRaporlar");
            if (lblRaporlar != null)
            {
                lblRaporlar.Click -= lblRaporlar_Click;
                lblRaporlar.Click += lblRaporlar_Click;
            }

            var lblCikis = FindCtrl<Label>("lblCikis");
            if (lblCikis != null)
            {
                lblCikis.Click -= lblCikis_Click;
                lblCikis.Click += lblCikis_Click;
            }

            var lblHakkimizda = FindCtrl<Label>("lblHakkimizda");
            if (lblHakkimizda != null)
            {
                lblHakkimizda.Click -= lblHakkimizda_Click;
                lblHakkimizda.Click += lblHakkimizda_Click;
            }

            var lblYardim = FindCtrl<Label>("lblYardim");
            if (lblYardim != null)
            {
                lblYardim.Click -= lblYardim_Click;
                lblYardim.Click += lblYardim_Click;
            }

            // Bazı tasarımlarda footer/geri için label13 / label9 / label4 kullanılmış olabilir
            var label13 = FindCtrl<Label>("label13");
            if (label13 != null)
            {
                label13.Click -= label13_Click;
                label13.Click += label13_Click;
            }

            var label9 = FindCtrl<Label>("label9");
            if (label9 != null)
            {
                label9.Click -= label9_Click;
                label9.Click += label9_Click;
            }

            var label4 = FindCtrl<Label>("label4");
            if (label4 != null)
            {
                label4.Click -= label4_Click;
                label4.Click += label4_Click;
            }

            // chart/dataGrid eventleri Designer bağlıysa kalsın, yoksa sorun değil
        }

        private void InitializeGalleryData()
        {
            galleryData = new DataTable();
            galleryData.Columns.Add("ID", typeof(int));
            galleryData.Columns.Add("Galerici", typeof(string));
            galleryData.Columns.Add("Eposta", typeof(string));
            galleryData.Columns.Add("Durum", typeof(string));   // AbonelikDurumu
            galleryData.Columns.Add("Fiyat", typeof(decimal));  // AylikUcret
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            YenidenYukle();
        }

        private void SetupDataGridView()
        {
            if (dataGridView1 == null) return;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = galleryData;

            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].HeaderText = "ID";
            if (dataGridView1.Columns["Galerici"] != null) dataGridView1.Columns["Galerici"].HeaderText = "Galerici";
            if (dataGridView1.Columns["Eposta"] != null) dataGridView1.Columns["Eposta"].HeaderText = "E-posta";
            if (dataGridView1.Columns["Durum"] != null) dataGridView1.Columns["Durum"].HeaderText = "Durum";
            if (dataGridView1.Columns["Fiyat"] != null) dataGridView1.Columns["Fiyat"].HeaderText = "Aylık Ücret";
        }

        public void YenidenYukle()
        {
            try
            {
                LoadGaleriFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                LoadCharts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Grafikler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGaleriFromDatabase()
        {
            if (galleryData == null) InitializeGalleryData();
            galleryData.Rows.Clear();

            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();

                string sql = @"
SELECT 
    GalericiID,
    AdSoyad,
    Eposta,
    COALESCE(AbonelikDurumu,'Pasif') AS AbonelikDurumu,
    COALESCE(AylikUcret,0)           AS AylikUcret
FROM galericiler
WHERE KullaniciTipi = 'Galerici'
ORDER BY GalericiID DESC;";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    var temp = new DataTable();
                    da.Fill(temp);

                    foreach (DataRow row in temp.Rows)
                    {
                        int id = Convert.ToInt32(row["GalericiID"]);
                        string ad = row["AdSoyad"]?.ToString() ?? "";
                        string eposta = row["Eposta"]?.ToString() ?? "";
                        string durum = row["AbonelikDurumu"]?.ToString() ?? "Pasif";
                        decimal ucret = row["AylikUcret"] == DBNull.Value ? 0m : Convert.ToDecimal(row["AylikUcret"]);

                        galleryData.Rows.Add(id, ad, eposta, durum, ucret);
                    }
                }
            }

            dataGridView1?.Refresh();
        }

        private void LoadCharts()
        {
            LoadRevenueChart();
            LoadGalleryCountChart();
        }

        private void LoadRevenueChart()
        {
            if (chart1 == null) return;

            chart1.Series.Clear();

            var series = new Series("Aylık Ücret")
            {
                ChartType = SeriesChartType.Column
            };

            if (galleryData != null)
            {
                foreach (DataRow row in galleryData.Rows)
                {
                    string ad = row["Galerici"]?.ToString() ?? "";
                    decimal ucret = row["Fiyat"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Fiyat"]);
                    series.Points.AddXY(ad, ucret);
                }
            }

            chart1.Series.Add(series);
            if (chart1.ChartAreas.Count > 0) chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private void LoadGalleryCountChart()
        {
            if (chart2 == null) return;

            chart2.Series.Clear();

            int aktif = 0, pasif = 0;
            if (galleryData != null)
            {
                foreach (DataRow row in galleryData.Rows)
                {
                    string durum = (row["Durum"]?.ToString() ?? "").Trim().ToLowerInvariant();
                    if (durum == "aktif") aktif++;
                    else pasif++;
                }
            }

            var series = new Series("Toplam Galerici")
            {
                ChartType = SeriesChartType.Column
            };

            series.Points.AddXY("Aktif", aktif);
            series.Points.AddXY("Pasif", pasif);

            chart2.Series.Add(series);
            if (chart2.ChartAreas.Count > 0) chart2.ChartAreas[0].RecalculateAxesScale();
        }

        // =========================
        // EXPORT: EXCEL (ClosedXML)
        // =========================
        private void ExportExcel()
        {
            try
            {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Admin_Rapor.xlsx");

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Rapor");

                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Galerici";
                    ws.Cell(1, 3).Value = "E-posta";
                    ws.Cell(1, 4).Value = "Durum";
                    ws.Cell(1, 5).Value = "Aylık Ücret";

                    ws.Range("A1:E1").Style.Font.Bold = true;
                    ws.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.LightGray;

                    int r = 2;
                    foreach (DataRow dr in galleryData.Rows)
                    {
                        ws.Cell(r, 1).Value = Convert.ToInt32(dr["ID"]);
                        ws.Cell(r, 2).Value = dr["Galerici"]?.ToString() ?? "";
                        ws.Cell(r, 3).Value = dr["Eposta"]?.ToString() ?? "";
                        ws.Cell(r, 4).Value = dr["Durum"]?.ToString() ?? "";

                        decimal ucret = dr["Fiyat"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Fiyat"]);
                        ws.Cell(r, 5).Value = ucret;

                        r++;
                    }

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(path);
                }

                MessageBox.Show("Excel Masaüstüne kaydedildi: Admin_Rapor.xlsx",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try { Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true }); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel oluşturulurken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // EXPORT: PDF (QuestPDF)
        // =========================
        private void ExportPdf()
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Admin_Rapor.pdf");

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(25);

                        page.Header().Row(r =>
                        {
                            r.RelativeItem().Text("ART FLOW - Admin Raporu").FontSize(18).SemiBold();
                            r.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).FontSize(10);
                        });

                        page.Content().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40);  // ID
                                cols.RelativeColumn(2);   // Galerici
                                cols.RelativeColumn(3);   // Eposta
                                cols.RelativeColumn(1);   // Durum
                                cols.RelativeColumn(1);   // Ücret
                            });

                            table.Header(h =>
                            {
                                h.Cell().Padding(5).Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.SemiBold()).Text("ID");

                                h.Cell().Padding(5).Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.SemiBold()).Text("Galerici");

                                h.Cell().Padding(5).Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.SemiBold()).Text("Eposta");

                                h.Cell().Padding(5).Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.SemiBold()).Text("Durum");

                                h.Cell().Padding(5).Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.SemiBold()).AlignRight().Text("Aylık Ücret");
                            });

                            foreach (DataRow dr in galleryData.Rows)
                            {
                                int id = Convert.ToInt32(dr["ID"]);
                                string ad = dr["Galerici"]?.ToString() ?? "";
                                string ep = dr["Eposta"]?.ToString() ?? "";
                                string durum = dr["Durum"]?.ToString() ?? "";
                                decimal ucret = dr["Fiyat"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Fiyat"]);

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(ad);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(ep);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(durum);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(ucret.ToString("0.00"));
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Sayfa ").FontSize(9);
                            x.CurrentPageNumber().FontSize(9);
                            x.Span(" / ").FontSize(9);
                            x.TotalPages().FontSize(9);
                        });
                    });
                }).GeneratePdf(path);

                MessageBox.Show("PDF Masaüstüne kaydedildi: Admin_Rapor.pdf",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try { Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true }); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF oluşturulurken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // BUTTON EVENT'LERİ
        // =========================
        private void btnPdf_Click(object sender, EventArgs e)
        {
            YenidenYukle();
            ExportPdf();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            YenidenYukle();
            ExportExcel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            YenidenYukle();
            ExportPdf();
        }

        // =========================
        // ✅ MODAL NAV (tek ekran)
        // =========================
        private void OpenModal(Form next)
        {
            if (next == null) return;

            this.Hide();
            try
            {
                next.StartPosition = FormStartPosition.CenterScreen;
                next.ShowDialog();
            }
            finally
            {
                try { next.Dispose(); } catch { }
                this.Show();
                this.BringToFront();
                YenidenYukle();
            }
        }

        private void LogoutToLogin()
        {
            var forms = Application.OpenForms.Cast<Form>().ToArray();
            var login = forms.OfType<Form1>().FirstOrDefault();

            foreach (var f in forms)
            {
                if (f == login) continue;
                if (f == this) continue;
                try { f.Close(); } catch { }
            }

            if (login != null)
            {
                login.Show();
                login.BringToFront();
            }
            else
            {
                new Form1().Show();
            }

            this.Close();
        }

        // =========================
        // MENU + FOOTER
        // =========================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            this.Close();

            if (_owner != null && !_owner.IsDisposed)
            {
                try { _owner.Show(); _owner.BringToFront(); } catch { }
            }
        }

        private void lblGalericiTanimlama_Click(object sender, EventArgs e)
        {
            OpenModal(new AdminGalericiTanimlama());
        }

        private void lblSifreBasvurulari_Click(object sender, EventArgs e)
        {
            OpenModal(new AdminBasvurular());
        }

        private void lblRaporlar_Click(object sender, EventArgs e)
        {
            YenidenYukle();
        }

        private void lblCikis_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            LogoutToLogin();
        }

        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            OpenModal(new Hakkimizda(Hakkimizda.HomeRole.Admin, 0));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            OpenModal(new Yardim(Yardim.HomeRole.Admin, 0));
        }

        private void label13_Click(object sender, EventArgs e) { this.Close(); }

        // Designer stub’lar
        private void label17_Click(object sender, EventArgs e) { }
        private void label18_Click(object sender, EventArgs e) { }
        private void chart2_Click(object sender, EventArgs e) { }
        private void chart1_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void label9_Click(object sender, EventArgs e)
        {
            OpenModal(new Hakkimizda(Hakkimizda.HomeRole.Admin, 0));
        }

        private void label4_Click(object sender, EventArgs e) { YenidenYukle(); }
    }
}
