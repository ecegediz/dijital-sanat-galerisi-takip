using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Excel
using ClosedXML.Excel;

// PDF (QuestPDF)
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// Color çakışmasını kesin çözmek için:
using DrawingColor = System.Drawing.Color;

namespace dijitalsanatgalerisi
{
    public partial class SergiciRaporlar : Form
    {
        private readonly int _sergiciId;
        private string _sergiciAdi;

        private const string ConnStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        public SergiciRaporlar(int sergiciId, string sergiciAdi)
        {
            InitializeComponent();
            _sergiciId = sergiciId;
            _sergiciAdi = (sergiciAdi ?? "").Trim();

            this.Load -= SergiciRaporlar_Load;
            this.Load += SergiciRaporlar_Load;
        }

        public SergiciRaporlar() : this(0, "") { }

        private void NavigateTo(Form next)
        {
            next.FormClosed += (s, e) => this.Close();
            next.Show();
            this.Hide();
        }

        private void SergiciRaporlar_Load(object sender, EventArgs e)
        {
            if (_sergiciId <= 0)
            {
                MessageBox.Show("Rapor açılırken SergiciID=0 geldi. ID taşımıyorsun.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(_sergiciAdi))
                _sergiciAdi = SergiciAdiniDbdenGetir();

            if (labelSergiciAdi != null)
                labelSergiciAdi.Text = string.IsNullOrWhiteSpace(_sergiciAdi) ? "—" : _sergiciAdi;

            // grid seçimli export istiyorsan bu önemli:
            if (dataGridViewSergiler != null)
            {
                dataGridViewSergiler.MultiSelect = true;
                dataGridViewSergiler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            IstatistikLabelleriniYukle();
            ToplamKazancGrafigiYukle();
            ToplamSergiGrafigiYukle();
            SergilerimiYukle();
            RaporLabelStilleriUygula();
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

        private void IstatistikLabelleriniYukle()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string ad = _sergiciAdi ?? "";

                    using (var cmd = new MySqlCommand(
                        @"SELECT COUNT(*) FROM sergiler se WHERE (@Ad='' OR se.Sergici=@Ad);", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelToplamSergi.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM sergiler se
WHERE (@Ad = '' OR se.Sergici = @Ad)
  AND se.Durum = 'Aktif'
  AND se.BaslangicTarihi <= CURDATE()
  AND se.BitisTarihi   >= CURDATE();", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelSergilenen.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM satislar s
INNER JOIN eserler  e  ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
  AND s.SatisDurumu = 'Satıldı';", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelSatilan.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    // ✅ HATA DÜZELTİLDİ: se.SergiID = e.EserID değil, e.SergiID olmalı
                    using (var cmd = new MySqlCommand(@"
SELECT COUNT(*)
FROM satislar s
INNER JOIN eserler  e  ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
  AND s.SatisDurumu = 'Beklemede';", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        labelBekleyen.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToplamKazancGrafigiYukle()
        {
            try
            {
                string ad = _sergiciAdi ?? "";
                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    DATE_FORMAT(s.SatisTarihi, '%Y-%m') AS Ay,
    SUM(s.SatisFiyati)                  AS ToplamKazanc
FROM satislar s
INNER JOIN eserler  e  ON e.EserID   = s.EserID
INNER JOIN sergiler se ON se.SergiID = e.SergiID
WHERE (@Ad = '' OR se.Sergici = @Ad)
GROUP BY Ay
ORDER BY Ay;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dt);
                }

                chartKazanc.Series[0].Points.Clear();
                chartKazanc.DataSource = dt;
                chartKazanc.Series[0].XValueMember = "Ay";
                chartKazanc.Series[0].YValueMembers = "ToplamKazanc";
                chartKazanc.ChartAreas[0].AxisY.Title = "TL";
                chartKazanc.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kazanç grafiği yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToplamSergiGrafigiYukle()
        {
            try
            {
                string ad = _sergiciAdi ?? "";
                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    DATE_FORMAT(se.BaslangicTarihi, '%Y-%m') AS Ay,
    COUNT(*)                                 AS SergiSayisi
FROM sergiler se
WHERE (@Ad = '' OR se.Sergici = @Ad)
GROUP BY Ay
ORDER BY Ay;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dt);
                }

                chartSergiler.Series[0].Points.Clear();
                chartSergiler.DataSource = dt;
                chartSergiler.Series[0].XValueMember = "Ay";
                chartSergiler.Series[0].YValueMembers = "SergiSayisi";
                chartSergiler.ChartAreas[0].AxisY.Title = "Adet";
                chartSergiler.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergi grafiği yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SergilerimiYukle()
        {
            try
            {
                string ad = _sergiciAdi ?? "";
                DataTable dt = new DataTable();

                using (var conn = new MySqlConnection(ConnStr))
                using (var da = new MySqlDataAdapter(@"
SELECT 
    se.SergiID         AS 'Sergi ID',
    se.SergiAdi        AS 'Sergi Adı',
    se.SergiTuru       AS 'Sergi Türü',
    se.SergiTemasi     AS 'Sergi Teması',
    se.BaslangicTarihi AS 'Başlangıç Tarihi',
    se.BitisTarihi     AS 'Bitiş Tarihi',
    se.Durum           AS 'Durum',
    se.HedefKitle      AS 'Hedef Kitle',
    se.EserSayisi      AS 'Eser Sayısı',
    se.Kapasite        AS 'Kapasite',
    se.GaleriKirasi    AS 'Galeri Kirası',
    se.EserBasiUcret   AS 'Eser Başı Ücret',
    se.ToplamMaliyet   AS 'Toplam Maliyet'
FROM sergiler se
WHERE (@Ad = '' OR se.Sergici = @Ad)
ORDER BY se.BaslangicTarihi DESC;", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ad", ad);
                    da.Fill(dt);
                }

                dataGridViewSergiler.ReadOnly = true;
                dataGridViewSergiler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewSergiler.AllowUserToAddRows = false;
                dataGridViewSergiler.AllowUserToDeleteRows = false;
                dataGridViewSergiler.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sergiler yüklenirken hata:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RaporLabelStilleriUygula()
        {
            DrawingColor mavi = DrawingColor.FromArgb(52, 152, 219);
            DrawingColor turuncu = DrawingColor.FromArgb(243, 156, 18);
            DrawingColor yesil = DrawingColor.FromArgb(46, 204, 113);
            DrawingColor mor = DrawingColor.FromArgb(155, 89, 182);

            labelToplamSergi.BackColor = mavi;
            labelSergilenen.BackColor = turuncu;
            labelSatilan.BackColor = yesil;
            labelBekleyen.BackColor = mor;

            Label[] icerikler = { labelToplamSergi, labelSergilenen, labelSatilan, labelBekleyen };

            foreach (var lbl in icerikler)
            {
                lbl.ForeColor = DrawingColor.White;
                lbl.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        // ============================
        // ✅ EXPORT: SEÇİLİ SERGİLER (YOKSA TÜMÜ)
        // ============================
        private DataTable GetSelectedOrAllExhibitionsTable()
        {
            var outDt = new DataTable();
            outDt.Columns.Add("Sergi Adı");
            outDt.Columns.Add("Sergi Türü");
            outDt.Columns.Add("Sergi Teması");
            outDt.Columns.Add("Başlangıç");
            outDt.Columns.Add("Bitiş");
            outDt.Columns.Add("Durum");
            outDt.Columns.Add("Hedef Kitle");
            outDt.Columns.Add("Eser Sayısı");
            outDt.Columns.Add("Kapasite");
            outDt.Columns.Add("Galeri Kirası");
            outDt.Columns.Add("Eser Başı Ücret");
            outDt.Columns.Add("Toplam Maliyet");

            if (dataGridViewSergiler == null || dataGridViewSergiler.Rows.Count == 0)
                return outDt;

            bool hasSelection = dataGridViewSergiler.SelectedRows != null && dataGridViewSergiler.SelectedRows.Count > 0;

            // seçili satırlar varsa onları al; yoksa tüm satırlar
            DataGridViewRow[] rows;
            if (hasSelection)
            {
                rows = new DataGridViewRow[dataGridViewSergiler.SelectedRows.Count];
                dataGridViewSergiler.SelectedRows.CopyTo(rows, 0);
                Array.Sort(rows, (a, b) => a.Index.CompareTo(b.Index));
            }
            else
            {
                rows = new DataGridViewRow[dataGridViewSergiler.Rows.Count];
                dataGridViewSergiler.Rows.CopyTo(rows, 0);
            }

            foreach (var r in rows)
            {
                if (r == null || r.IsNewRow) continue;

                string Get(string col) =>
                    dataGridViewSergiler.Columns.Contains(col)
                        ? (r.Cells[col]?.Value?.ToString() ?? "")
                        : "";

                outDt.Rows.Add(
                    Get("Sergi Adı"),
                    Get("Sergi Türü"),
                    Get("Sergi Teması"),
                    Get("Başlangıç Tarihi"),
                    Get("Bitiş Tarihi"),
                    Get("Durum"),
                    Get("Hedef Kitle"),
                    Get("Eser Sayısı"),
                    Get("Kapasite"),
                    Get("Galeri Kirası"),
                    Get("Eser Başı Ücret"),
                    Get("Toplam Maliyet")
                );
            }

            return outDt;
        }

        // ============================
        // ✅ EXCEL (ClosedXML)
        // ============================
        private void btnExcelOlustur_Click(object sender, EventArgs e)
        {
            var dt = GetSelectedOrAllExhibitionsTable();
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Raporlanacak sergi bulunamadı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
                sfd.FileName = $"SergiciRapor_{(_sergiciAdi ?? "Sergici")}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Sergiler");

                        ws.Cell(1, 1).Value = "Sergici";
                        ws.Cell(1, 2).Value = _sergiciAdi ?? "";
                        ws.Cell(2, 1).Value = "Rapor Tarihi";
                        ws.Cell(2, 2).Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                        // tabloyu 4. satırdan başlatalım
                        ws.Cell(4, 1).InsertTable(dt, "SergilerTablo", true);

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Excel başarıyla oluşturuldu.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excel oluşturulurken hata:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================
        // ✅ PDF (QuestPDF)
        // ============================
        private void btnPdfOlustur_Click(object sender, EventArgs e)
        {
            var dt = GetSelectedOrAllExhibitionsTable();
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Raporlanacak sergi bulunamadı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF Dosyası (*.pdf)|*.pdf";
                sfd.FileName = $"SergiciRapor_{(_sergiciAdi ?? "Sergici")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    QuestPDF.Settings.License = LicenseType.Community;

                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(20);
                            page.DefaultTextStyle(x => x.FontSize(10));

                            page.Header().Column(col =>
                            {
                                col.Item().Text("ART FLOW - Sergici Raporu").FontSize(18).SemiBold().AlignCenter();
                                col.Item().Text($"Sergici: {_sergiciAdi}").FontSize(11).AlignCenter();
                                col.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).AlignRight();
                            });

                            page.Content().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(2); // Sergi Adı
                                    cols.RelativeColumn(1); // Tür
                                    cols.RelativeColumn(2); // Tema
                                    cols.RelativeColumn(1); // Başlangıç
                                    cols.RelativeColumn(1); // Bitiş
                                    cols.RelativeColumn(1); // Durum
                                    cols.RelativeColumn(2); // Hedef Kitle
                                    cols.RelativeColumn(1); // Eser Sayısı
                                    cols.RelativeColumn(1); // Kapasite
                                    cols.RelativeColumn(1); // Galeri Kirası
                                    cols.RelativeColumn(1); // Eser Başı
                                    cols.RelativeColumn(1); // Toplam
                                });

                                // ✅ DOĞRU HEADER KULLANIMI
                                table.Header(h =>
                                {
                                    void H(string t) =>
                                        h.Cell().Padding(4).Background(Colors.Grey.Lighten3)
                                            .DefaultTextStyle(x => x.SemiBold()).Text(t);

                                    H("Sergi Adı");
                                    H("Tür");
                                    H("Tema");
                                    H("Başlangıç");
                                    H("Bitiş");
                                    H("Durum");
                                    H("Hedef Kitle");
                                    H("Eser");
                                    H("Kapasite");
                                    H("Kira");
                                    H("Eser Başı");
                                    H("Toplam");
                                });

                                foreach (DataRow r in dt.Rows)
                                {
                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        table.Cell()
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(4)
                                            .Text(r[i]?.ToString() ?? "");
                                    }
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
                    }).GeneratePdf(sfd.FileName);

                    MessageBox.Show("PDF başarıyla oluşturuldu.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PDF oluşturulurken hata:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ======= MENÜ EVENTLERİ (SENDE ZATEN VAR) =======
        private void dashboardButton_Click_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciDashboard(_sergiciId, _sergiciAdi));
        }

        private void eserlerimButton_Click_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciEserlerim(_sergiciId, _sergiciAdi));
        }

        private void raporButton_Click_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Zaten Rapor ekranındasınız.");
        }

        private void cikisYapButton_Click_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkış yapmak istiyor musunuz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NavigateTo(new Form1());
            }
        }

        private void hakkımızdaButton_Click_Click(object sender, EventArgs e)
        {
            new Hakkimizda().Show();
        }

        private void labelSergiciAdi_Click(object sender, EventArgs e)
        {
            NavigateTo(new SergiciProfilim(_sergiciId, _sergiciAdi));
        }

        private void label1_Click(object sender, EventArgs e)
        {
            new Yardim().Show();
        }
    }
}
