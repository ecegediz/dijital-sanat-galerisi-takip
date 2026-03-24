using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace dijitalsanatgalerisi
{
    public partial class GalericiMusteriBilgileri : Form
    {
        private readonly int _galericiId;

        private readonly string _connStr =
            "Server=localhost;Database=sanatgalerisidb;Uid=root;Pwd=;Charset=utf8;";

        private DataTable _dt;

        public GalericiMusteriBilgileri(int galericiId)
        {
            InitializeComponent();
            _galericiId = galericiId;

            Load -= GalericiMusteriBilgileri_Load;
            Load += GalericiMusteriBilgileri_Load;
        }

        public GalericiMusteriBilgileri() : this(0) { }

        private void GalericiMusteriBilgileri_Load(object sender, EventArgs e)
        {
            GridAyarla();
            MusteriListesiniYukle();

            if (dataGridView1 != null)
            {
                dataGridView1.CellClick -= dataGridView1_CellClick;
                dataGridView1.CellClick += dataGridView1_CellClick;
            }
        }

        // =========================================================
        // ✅ GALERICI NAV STANDARDI
        // - Menü/Footer: Dialog aç, bu form kapanmasın
        // - Çıkış/Login: Show + Close
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
                    // geri dönünce tabloyu tazele
                    MusteriListesiniYukle();
                }
            }
        }

        private void NavigateAndClose(Form nextForm)
        {
            if (nextForm == null) return;
            nextForm.Show();
            this.Close();
        }

        // =====================
        // GRID
        // =====================
        private void GridAyarla()
        {
            if (dataGridView1 == null) return;

            dataGridView1.ReadOnly = true;

            dataGridView1.MultiSelect = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
        }

        private void EnsureGridButtons()
        {
            if (dataGridView1 == null) return;

            if (dataGridView1.Columns["Duzenle"] == null)
            {
                dataGridView1.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Duzenle",
                    HeaderText = "Düzenle",
                    Text = "Düzenle",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                });
            }

            if (dataGridView1.Columns["Sil"] == null)
            {
                dataGridView1.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Sil",
                    HeaderText = "Sil",
                    Text = "Sil",
                    UseColumnTextForButtonValue = true,
                    Width = 60
                });
            }

            // En sağa it
            if (dataGridView1.Columns["Duzenle"] != null)
                dataGridView1.Columns["Duzenle"].DisplayIndex = dataGridView1.Columns.Count - 2;

            if (dataGridView1.Columns["Sil"] != null)
                dataGridView1.Columns["Sil"].DisplayIndex = dataGridView1.Columns.Count - 1;
        }

        private void MusteriListesiniYukle()
        {
            if (dataGridView1 == null) return;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT 
    m.MusteriID,
    m.AdSoyad  AS MusteriAdi,
    m.Eposta   AS Eposta,
    s.EserID   AS EserID,
    e.Baslik   AS EserAdi,
    e.SanatciAdi AS SanatciAdi,
    COALESCE(s.SatisFiyati, e.Fiyat) AS Fiyat
FROM musteriler m
LEFT JOIN satislar s  ON s.MusteriID = m.MusteriID
LEFT JOIN eserler  e  ON e.EserID    = s.EserID
WHERE (@GalId = 0 OR m.GalericiID = @GalId)
ORDER BY m.MusteriID DESC;";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@GalId", _galericiId);
                        _dt = new DataTable();
                        da.Fill(_dt);
                    }
                }

                dataGridView1.DataSource = null;
                dataGridView1.Columns.Clear();

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = _dt;

                if (dataGridView1.Columns["MusteriID"] != null) dataGridView1.Columns["MusteriID"].Visible = false;
                if (dataGridView1.Columns["EserID"] != null) dataGridView1.Columns["EserID"].Visible = false;

                if (dataGridView1.Columns["MusteriAdi"] != null) dataGridView1.Columns["MusteriAdi"].HeaderText = "Müşteri Adı";
                if (dataGridView1.Columns["Eposta"] != null) dataGridView1.Columns["Eposta"].HeaderText = "E-posta";
                if (dataGridView1.Columns["EserAdi"] != null) dataGridView1.Columns["EserAdi"].HeaderText = "Eser Adı";
                if (dataGridView1.Columns["SanatciAdi"] != null) dataGridView1.Columns["SanatciAdi"].HeaderText = "Sanatçı";
                if (dataGridView1.Columns["Fiyat"] != null) dataGridView1.Columns["Fiyat"].HeaderText = "Fiyat";

                EnsureGridButtons();
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri listesi yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? GetMusteriIdFromRow(int rowIndex)
        {
            if (dataGridView1 == null) return null;
            if (rowIndex < 0) return null;

            var cell = dataGridView1.Rows[rowIndex].Cells["MusteriID"];
            if (cell?.Value == null || cell.Value == DBNull.Value) return null;

            return Convert.ToInt32(cell.Value);
        }

        // =====================
        // GRID BUTONLARI (Düzenle/Sil)
        // =====================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            int? musteriId = GetMusteriIdFromRow(e.RowIndex);
            if (musteriId == null) return;

            // Düzenle
            if (dataGridView1.Columns["Duzenle"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Duzenle"].Index)
            {
                // ✅ dialog aç: kapanınca bu ekrana geri dön
                NavigateDialog(new GalericiMusteriDuzenleme(_galericiId, musteriId.Value));
                return;
            }

            // Sil
            if (dataGridView1.Columns["Sil"] != null &&
                e.ColumnIndex == dataGridView1.Columns["Sil"].Index)
            {
                SilMusteri(musteriId.Value);
                return;
            }
        }

        private void SilMusteri(int musteriId)
        {
            var dr = MessageBox.Show(
                "Bu müşteriyi silmek istiyor musunuz? (Satış kaydı da silinir)",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var c1 = new MySqlCommand("DELETE FROM satislar WHERE MusteriID=@mid;", conn, tx))
                        {
                            c1.Parameters.AddWithValue("@mid", musteriId);
                            c1.ExecuteNonQuery();
                        }

                        using (var c2 = new MySqlCommand(
                            "DELETE FROM musteriler WHERE MusteriID=@mid AND (@gid=0 OR GalericiID=@gid);",
                            conn, tx))
                        {
                            c2.Parameters.AddWithValue("@mid", musteriId);
                            c2.Parameters.AddWithValue("@gid", _galericiId);
                            c2.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MusteriListesiniYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // SEÇİLİ SATIRLAR -> DATATABLE
        // ==========================
        private bool TryGetSelectedCustomersTable(out DataTable dt, string purposeText)
        {
            dt = new DataTable();
            dt.Columns.Add("Müşteri Adı");
            dt.Columns.Add("E-posta");
            dt.Columns.Add("Eser Adı");
            dt.Columns.Add("Sanatçı");
            dt.Columns.Add("Fiyat");

            if (dataGridView1 == null || dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"{purposeText} müşteri bilgilerini seçiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var rows = dataGridView1.SelectedRows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .OrderBy(r => r.Index)
                .ToList();

            foreach (var r in rows)
            {
                dt.Rows.Add(
                    r.Cells["MusteriAdi"]?.Value?.ToString() ?? "",
                    r.Cells["Eposta"]?.Value?.ToString() ?? "",
                    r.Cells["EserAdi"]?.Value?.ToString() ?? "",
                    r.Cells["SanatciAdi"]?.Value?.ToString() ?? "",
                    r.Cells["Fiyat"]?.Value?.ToString() ?? ""
                );
            }

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show($"{purposeText} müşteri bilgilerini seçiniz.",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ==========================
        // EXCEL
        // ==========================
        private void btnExcelOlustur_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedCustomersTable(out var dt, "Excel oluşturmak istediğiniz"))
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
                sfd.FileName = "Musteriler.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Müşteriler");
                        ws.Cell(1, 1).InsertTable(dt, "MusteriTablo", true);
                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excel oluşturma hatası:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================
        // PDF
        // ==========================
        private void btnPdfOlustur_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedCustomersTable(out var dt, "PDF oluşturmak istediğiniz"))
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF Dosyası (*.pdf)|*.pdf";
                sfd.FileName = "Musteriler.pdf";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    QuestPDF.Settings.License = LicenseType.Community;

                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(QuestPDF.Helpers.PageSizes.A4);
                            page.Margin(25);
                            page.DefaultTextStyle(x => x.FontSize(11));

                            page.Header().Column(col =>
                            {
                                col.Item().Text("MÜŞTERİ RAPORU").FontSize(18).Bold().AlignCenter();
                                col.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).AlignRight();
                            });

                            page.Content().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn();
                                    cols.RelativeColumn();
                                    cols.RelativeColumn();
                                    cols.RelativeColumn();
                                    cols.ConstantColumn(70);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Padding(4).Border(1).Text("Müşteri Adı").Bold();
                                    header.Cell().Padding(4).Border(1).Text("E-posta").Bold();
                                    header.Cell().Padding(4).Border(1).Text("Eser Adı").Bold();
                                    header.Cell().Padding(4).Border(1).Text("Sanatçı").Bold();
                                    header.Cell().Padding(4).Border(1).Text("Fiyat").Bold();
                                });

                                foreach (DataRow r in dt.Rows)
                                {
                                    table.Cell().Padding(4).Border(1).Text(r[0]?.ToString() ?? "");
                                    table.Cell().Padding(4).Border(1).Text(r[1]?.ToString() ?? "");
                                    table.Cell().Padding(4).Border(1).Text(r[2]?.ToString() ?? "");
                                    table.Cell().Padding(4).Border(1).Text(r[3]?.ToString() ?? "");
                                    table.Cell().Padding(4).Border(1).Text(r[4]?.ToString() ?? "");
                                }
                            });

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.Span("Art Flow | Sayfa ");
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                        });
                    }).GeneratePdf(sfd.FileName);

                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PDF oluşturma hatası:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================
        // YENİ MÜŞTERİ EKLE
        // ==========================
        private void btnYeniMusteriEkle_Click(object sender, EventArgs e)
        {
            // ✅ dialog aç: kapanınca bu sayfaya geri dön
            NavigateDialog(new GalericiMusteriPDF(_galericiId));
        }

        // ==========================
        // MENU (Galerici standardı: dialog)
        // ==========================
        private void lblDashboard_Click(object sender, EventArgs e)
        {
            NavigateAndClose(new GalericiDashboard(_galericiId));
        }

        private void lblSergiciTanimlama_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiciTanim(_galericiId));
        }

        private void lblMusteriBilgileri_Click(object sender, EventArgs e)
        {
            // Zaten burası -> sadece yenile
            MusteriListesiniYukle();
        }

        private void lblSergiler_Click(object sender, EventArgs e)
        {
            NavigateDialog(new GalericiSergiler(_galericiId));
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

        // ==========================
        // FOOTER (dialog)
        // ==========================
        private void lblHakkimizda_Click(object sender, EventArgs e)
        {
            // ✅ dialog: kapanınca buraya geri döner
            NavigateDialog(new Hakkimizda(Hakkimizda.HomeRole.Galerici, _galericiId));
        }

        private void lblYardim_Click(object sender, EventArgs e)
        {
            // ✅ dialog: kapanınca buraya geri döner
            NavigateDialog(new Yardim(Yardim.HomeRole.Galerici, _galericiId));
        }
    }
}
