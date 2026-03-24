using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Excel
using ClosedXML.Excel;

// PDF (QuestPDF)
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace dijitalsanatgalerisi
{
    public partial class GalericiRaporlar : Form
    {
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public GalericiRaporlar(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            this.Load -= GalericiRaporlar_Load;
            this.Load += GalericiRaporlar_Load;
        }

        public GalericiRaporlar() : this(0) { }

        private void GalericiRaporlar_Load(object sender, EventArgs e)
        {
            BindCriticalEvents();

            ToplamKazancGrafigiYukle();
            ToplamSergiGrafigiYukle();
            SergiciListesiniYukle();

            if (dataGridViewSergiciler != null)
            {
                dataGridViewSergiciler.SelectionChanged -= dataGridViewSergiciler_SelectionChanged;
                dataGridViewSergiciler.SelectionChanged += dataGridViewSergiciler_SelectionChanged;
            }

            // İlk açılışta tüm sergiler
            SergilerTablosunuYukle(null);
        }

        // =========================================================
        // ✅ GALERICI NAV STANDARDI
        // - Menü/Footer: Dialog aç (üst üste sayfa birikmez)
        // - Login gibi akış değişiyorsa: Show + Close
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

            // geri dönünce veriler güncellensin
            RefreshAll();
        }

        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;
            nextForm.Show();
            this.Close();
        }

        private void RefreshAll()
        {
            ToplamKazancGrafigiYukle();
            ToplamSergiGrafigiYukle();
            SergiciListesiniYukle();

            // seçili sergiciye göre alttaki tablo
            string sergiciAd = GetSelectedSergiciAd();
            SergilerTablosunuYukle(string.IsNullOrWhiteSpace(sergiciAd) ? null : sergiciAd);
        }

        // =========================================================
        // EVENTLERİ GARANTİ BAĞLA
        // =========================================================
        private void BindCriticalEvents()
        {
            // Menü
            BindLabelClick("lblDashboard", lblDashboard_Click);
            BindLabelClick("lblSergiciTanimlama", lblSergiciTanimlama_Click);
            BindLabelClick("lblMusteriBilgileri", lblMusteriBilgileri_Click);
            BindLabelClick("lblSergiler", lblSergiler_Click);
            BindLabelClick("lblRaporEkrani", lblRaporEkrani_Click);
            BindLabelClick("lblGalericiAdi", lblGalericiAdi_Click);
            BindLabelClick("lblCikisYap", lblCikisYap_Click);

            // Footer
            BindLabelClick("lblHakkimizda", lblHakkimizda_Click);
            BindLabelClick("lblYardim", lblYardim_Click);

            // Butonlar
            BindButtonClick("btnExcelOlustur", btnExcelOlustur_Click);
            BindButtonClick("btnPdfOlustur", btnPdfOlustur_Click);
            BindButtonClick("btnTumSergiler", btnTumSergiler_Click);
        }

        private void BindButtonClick(string controlName, EventHandler handler)
        {
            var btn = this.Controls.Find(controlName, true).FirstOrDefault() as Button;
            if (btn == null) return;

            btn.Click -= handler;
            btn.Click += handler;
        }

        private void BindLabelClick(string controlName, EventHandler handler)
        {
            var lbl = this.Controls.Find(controlName, true).FirstOrDefault() as Label;
            if (lbl == null) return;

            lbl.Click -= handler;
            lbl.Click += handler;
        }

        // =========================================================
        // 1) Aylık toplam kazanç grafiği
        // =========================================================
        private void ToplamKazancGrafigiYukle()
        {
            try
            {
                string sql = @"
SELECT 
    DATE_FORMAT(SatisTarihi, '%Y-%m') AS Ay,
    COALESCE(SUM(SatisFiyati),0)      AS ToplamKazanc
FROM satislar
WHERE (@GalId = 0 OR GalericiID = @GalId)
GROUP BY Ay
ORDER BY Ay;";

                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(_connStr))
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GalId", _galericiId);
                    da.Fill(dt);
                }

                if (chartKazanc == null) return;
                if (chartKazanc.Series.Count == 0) return;

                chartKazanc.Series[0].Points.Clear();
                chartKazanc.Series[0].XValueMember = "Ay";
                chartKazanc.Series[0].YValueMembers = "ToplamKazanc";
                chartKazanc.DataSource = dt;
                chartKazanc.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam kazanç grafiği yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 2) Aylık sergi sayısı grafiği
        // =========================================================
        private void ToplamSergiGrafigiYukle()
        {
            try
            {
                string sql = @"
SELECT 
    DATE_FORMAT(BaslangicTarihi, '%Y-%m') AS Ay,
    COUNT(*)                              AS SergiSayisi
FROM sergiler
WHERE (@GalId = 0 OR GalericiID = @GalId)
GROUP BY Ay
ORDER BY Ay;";

                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(_connStr))
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GalId", _galericiId);
                    da.Fill(dt);
                }

                if (chartSergi == null) return;
                if (chartSergi.Series.Count == 0) return;

                chartSergi.Series[0].Points.Clear();
                chartSergi.Series[0].XValueMember = "Ay";
                chartSergi.Series[0].YValueMembers = "SergiSayisi";
                chartSergi.DataSource = dt;
                chartSergi.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergi sayısı grafiği yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 3) Sergici listesi (üst grid)
        // =========================================================
        private void SergiciListesiniYukle()
        {
            try
            {
                string sql = @"
SELECT
    sc.SanatciID              AS 'Sergici ID',
    sc.AdSoyad                AS 'Sergici',
    se.SergiID                AS 'Sergi ID',
    se.SergiAdi               AS 'Sergi Adı',
    e.EserID                  AS 'Eser ID',
    e.Baslik                  AS 'Eser Adı',
    e.Fiyat                   AS 'Eser Fiyatı'
FROM sergiciler sc
LEFT JOIN sergiler se 
    ON se.Sergici    = sc.AdSoyad
   AND se.GalericiID = sc.GalericiID
LEFT JOIN eserler e  
    ON e.SergiID = se.SergiID
WHERE (@GalId = 0 OR sc.GalericiID = @GalId)
ORDER BY sc.SanatciID, se.SergiID, e.EserID;";

                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(_connStr))
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GalId", _galericiId);
                    da.Fill(dt);
                }

                if (dataGridViewSergiciler == null) return;

                dataGridViewSergiciler.ReadOnly = true;
                dataGridViewSergiciler.MultiSelect = false;
                dataGridViewSergiciler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewSergiciler.AllowUserToAddRows = false;
                dataGridViewSergiciler.AllowUserToDeleteRows = false;
                dataGridViewSergiciler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridViewSergiciler.DataSource = dt;

                if (dataGridViewSergiciler.Rows.Count > 0)
                {
                    dataGridViewSergiciler.ClearSelection();
                    dataGridViewSergiciler.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergici listesi yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewSergiciler_SelectionChanged(object sender, EventArgs e)
        {
            string sergiciAd = GetSelectedSergiciAd();
            SergilerTablosunuYukle(string.IsNullOrWhiteSpace(sergiciAd) ? null : sergiciAd);
        }

        private string GetSelectedSergiciAd()
        {
            if (dataGridViewSergiciler == null) return null;
            if (dataGridViewSergiciler.CurrentRow == null) return null;

            if (dataGridViewSergiciler.CurrentRow.DataBoundItem is DataRowView rv)
            {
                if (rv.Row.Table.Columns.Contains("Sergici"))
                    return rv["Sergici"]?.ToString();
            }

            if (dataGridViewSergiciler.Columns.Contains("Sergici"))
                return dataGridViewSergiciler.CurrentRow.Cells["Sergici"]?.Value?.ToString();

            return null;
        }

        // =========================================================
        // 4) Sergiler tablosu (alt grid)
        // sergiciAd null => tüm sergiler
        // =========================================================
        private void SergilerTablosunuYukle(string sergiciAd)
        {
            try
            {
                string sql = @"
SELECT
    SergiID,
    SergiAdi,
    SergiTuru,
    SergiTemasi,
    HedefKitle,
    Kapasite,
    COALESCE(EserSayisi,0) AS EserSayisi,
    BaslangicTarihi,
    BitisTarihi,
    Sergici,
    Durum
FROM sergiler
WHERE (@GalId = 0 OR GalericiID = @GalId)
  AND (@Sergici IS NULL OR @Sergici = '' OR Sergici = @Sergici)
ORDER BY BaslangicTarihi DESC;";

                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(_connStr))
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GalId", _galericiId);
                    da.SelectCommand.Parameters.AddWithValue("@Sergici", sergiciAd ?? "");
                    da.Fill(dt);
                }

                if (dataGridViewSergiler == null) return;

                dataGridViewSergiler.ReadOnly = true;
                dataGridViewSergiler.MultiSelect = true;
                dataGridViewSergiler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewSergiler.AllowUserToAddRows = false;
                dataGridViewSergiler.AllowUserToDeleteRows = false;
                dataGridViewSergiler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridViewSergiler.DataSource = dt;

                if (dataGridViewSergiler.Columns["SergiID"] != null)
                    dataGridViewSergiler.Columns["SergiID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergiler tablosu yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTumSergiler_Click(object sender, EventArgs e)
        {
            if (dataGridViewSergiciler != null)
                dataGridViewSergiciler.ClearSelection();

            SergilerTablosunuYukle(null);
        }

        // =========================================================
        // RAPOR TABLOLARI
        // =========================================================
        private DataTable GetSelectedSergicilerForReport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Sergici ID");
            dt.Columns.Add("Sergici");

            if (dataGridViewSergiciler == null) return dt;

            var selected = dataGridViewSergiciler.SelectedRows;
            if (selected == null || selected.Count == 0)
            {
                if (dataGridViewSergiciler.CurrentRow != null)
                {
                    var r = dataGridViewSergiciler.CurrentRow;
                    string id = GetCellSafe(r, "Sergici ID");
                    string ad = GetCellSafe(r, "Sergici");
                    if (!string.IsNullOrWhiteSpace(ad))
                        dt.Rows.Add(id, ad);
                }
                return dt;
            }

            var rows = selected.Cast<DataGridViewRow>().OrderBy(x => x.Index).ToList();
            foreach (var r in rows)
            {
                string id = GetCellSafe(r, "Sergici ID");
                string ad = GetCellSafe(r, "Sergici");
                if (!string.IsNullOrWhiteSpace(ad))
                    dt.Rows.Add(id, ad);
            }

            return dt;
        }

        private DataTable GetSergilerForReport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Sergi Adı");
            dt.Columns.Add("Tür");
            dt.Columns.Add("Tema");
            dt.Columns.Add("Hedef Kitle");
            dt.Columns.Add("Kapasite");
            dt.Columns.Add("Eser Sayısı");
            dt.Columns.Add("Başlangıç");
            dt.Columns.Add("Bitiş");
            dt.Columns.Add("Sergici");
            dt.Columns.Add("Durum");

            if (dataGridViewSergiler == null) return dt;

            var useRows = (dataGridViewSergiler.SelectedRows != null && dataGridViewSergiler.SelectedRows.Count > 0)
                ? dataGridViewSergiler.SelectedRows.Cast<DataGridViewRow>().OrderBy(x => x.Index)
                : dataGridViewSergiler.Rows.Cast<DataGridViewRow>().Where(x => !x.IsNewRow);

            foreach (var r in useRows)
            {
                dt.Rows.Add(
                    GetCellSafe(r, "SergiAdi"),
                    GetCellSafe(r, "SergiTuru"),
                    GetCellSafe(r, "SergiTemasi"),
                    GetCellSafe(r, "HedefKitle"),
                    GetCellSafe(r, "Kapasite"),
                    GetCellSafe(r, "EserSayisi"),
                    ShortDate(GetCellSafe(r, "BaslangicTarihi")),
                    ShortDate(GetCellSafe(r, "BitisTarihi")),
                    GetCellSafe(r, "Sergici"),
                    GetCellSafe(r, "Durum")
                );
            }

            return dt;
        }

        private string GetCellSafe(DataGridViewRow row, string columnName)
        {
            if (row == null || row.DataGridView == null) return "";
            if (!row.DataGridView.Columns.Contains(columnName)) return "";

            object v = row.Cells[columnName]?.Value;
            if (v == null || v == DBNull.Value) return "";
            return v.ToString();
        }

        private string ShortDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (DateTime.TryParse(value, out DateTime dt))
                return dt.ToString("dd.MM.yyyy");
            return value;
        }

        // =========================================================
        // EXCEL OLUŞTUR
        // =========================================================
        private void btnExcelOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                var dtSergiciler = GetSelectedSergicilerForReport();
                var dtSergiler = GetSergilerForReport();

                if (dtSergiciler.Rows.Count == 0 && dtSergiler.Rows.Count == 0)
                {
                    MessageBox.Show("Raporlanacak veri bulunamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
                    sfd.FileName = "Galerici_Rapor.xlsx";
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    using (var wb = new XLWorkbook())
                    {
                        var ws1 = wb.Worksheets.Add("Sergiciler");
                        if (dtSergiciler.Rows.Count > 0)
                        {
                            ws1.Cell(1, 1).InsertTable(dtSergiciler, "SergicilerTablo", true);
                            ws1.Columns().AdjustToContents();
                        }
                        else
                        {
                            ws1.Cell(1, 1).Value = "Seçili sergici bulunamadı.";
                        }

                        var ws2 = wb.Worksheets.Add("Sergiler");
                        if (dtSergiler.Rows.Count > 0)
                        {
                            ws2.Cell(1, 1).InsertTable(dtSergiler, "SergilerTablo", true);
                            ws2.Columns().AdjustToContents();
                        }
                        else
                        {
                            ws2.Cell(1, 1).Value = "Raporlanacak sergi bulunamadı.";
                        }

                        var ws3 = wb.Worksheets.Add("Bilgi");
                        ws3.Cell(1, 1).Value = "Rapor Tarihi";
                        ws3.Cell(1, 2).Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        ws3.Cell(2, 1).Value = "Galerici ID";
                        ws3.Cell(2, 2).Value = _galericiId.ToString();
                        ws3.Columns().AdjustToContents();

                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Excel raporu oluşturuldu.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel oluşturulurken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // PDF OLUŞTUR
        // =========================================================
        private void btnPdfOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                var dtSergiciler = GetSelectedSergicilerForReport();
                var dtSergiler = GetSergilerForReport();

                if (dtSergiciler.Rows.Count == 0 && dtSergiler.Rows.Count == 0)
                {
                    MessageBox.Show("Raporlanacak veri bulunamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Dosyası (*.pdf)|*.pdf";
                    sfd.FileName = "Galerici_Rapor.pdf";
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    QuestPDF.Settings.License = LicenseType.Community;

                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(25);
                            page.DefaultTextStyle(x => x.FontSize(10));

                            page.Header().Column(col =>
                            {
                                col.Item().Text("GALERICİ RAPORU").FontSize(18).Bold().AlignCenter();
                                col.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}")
                                    .FontSize(9).AlignRight();
                                col.Item().Text($"Galerici ID: {_galericiId}")
                                    .FontSize(9).AlignRight();
                            });

                            page.Content().Column(col =>
                            {
                                col.Spacing(12);

                                col.Item().Text("Seçili Sergici(ler)").FontSize(12).Bold();

                                if (dtSergiciler.Rows.Count == 0)
                                {
                                    col.Item().Text("Seçili sergici bulunamadı.");
                                }
                                else
                                {
                                    col.Item().Table(t =>
                                    {
                                        t.ColumnsDefinition(c =>
                                        {
                                            c.ConstantColumn(80);
                                            c.RelativeColumn();
                                        });

                                        t.Header(h =>
                                        {
                                            h.Cell().Padding(4).Border(1).Background(Colors.Grey.Lighten3).Text("ID").Bold();
                                            h.Cell().Padding(4).Border(1).Background(Colors.Grey.Lighten3).Text("Sergici").Bold();
                                        });

                                        foreach (DataRow r in dtSergiciler.Rows)
                                        {
                                            t.Cell().Padding(4).Border(1).Text(r[0]?.ToString() ?? "");
                                            t.Cell().Padding(4).Border(1).Text(r[1]?.ToString() ?? "");
                                        }
                                    });
                                }

                                col.Item().Text("Sergiler").FontSize(12).Bold();

                                if (dtSergiler.Rows.Count == 0)
                                {
                                    col.Item().Text("Raporlanacak sergi bulunamadı.");
                                }
                                else
                                {
                                    col.Item().Table(t =>
                                    {
                                        t.ColumnsDefinition(c =>
                                        {
                                            c.RelativeColumn();     // Sergi
                                            c.ConstantColumn(55);   // Tür
                                            c.RelativeColumn();     // Tema
                                            c.ConstantColumn(70);   // Hedef
                                            c.ConstantColumn(55);   // Kap.
                                            c.ConstantColumn(55);   // Eser
                                            c.ConstantColumn(70);   // Baş.
                                            c.ConstantColumn(70);   // Bitiş
                                            c.RelativeColumn();     // Sergici
                                            c.ConstantColumn(55);   // Durum
                                        });

                                        t.Header(h =>
                                        {
                                            string[] headers = { "Sergi", "Tür", "Tema", "Hedef", "Kap.", "Eser", "Baş.", "Bitiş", "Sergici", "Durum" };
                                            foreach (var head in headers)
                                                h.Cell().Padding(3).Border(1).Background(Colors.Grey.Lighten3).Text(head).Bold();
                                        });

                                        foreach (DataRow r in dtSergiler.Rows)
                                        {
                                            t.Cell().Padding(3).Border(1).Text(r[0]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[1]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[2]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[3]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[4]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[5]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[6]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[7]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[8]?.ToString() ?? "");
                                            t.Cell().Padding(3).Border(1).Text(r[9]?.ToString() ?? "");
                                        }
                                    });
                                }
                            });

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.Span("Art Flow - Dijital Sanat Galerisi | Sayfa ");
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                        });
                    }).GeneratePdf(sfd.FileName);

                    MessageBox.Show("PDF raporu oluşturuldu.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF oluşturulurken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // MENÜ (✅ dialog standardı)
        // =========================================================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            // Bu sayfa dashboard'dan dialog açıldıysa: geri dönmek için sadece kapat
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
            NavigateDialog(new GalericiSergiler(_galericiId));
        }

        private void lblRaporEkrani_Click(object sender, EventArgs e)
        {
            // Zaten buradasın -> yenile
            RefreshAll();
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

            // login ana akış: show + close
            NavigateAndClose(new Form1());
        }

        // =========================================================
        // FOOTER (✅ dialog: kapanınca buraya geri gelir)
        // =========================================================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            NavigateDialog(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            NavigateDialog(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }

        // =========================================================
        // Designer stub’lar
        // =========================================================
        private void label17_Click(object sender, EventArgs e) { }
        private void LabelCikisYap_Click(object sender, EventArgs e) { }
    }
}
