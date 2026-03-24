namespace dijitalsanatgalerisi
{
    partial class GalericiRaporlar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GalericiRaporlar));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chartKazanc = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSergi = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblSergiler = new System.Windows.Forms.Label();
            this.lblMusteriBilgileri = new System.Windows.Forms.Label();
            this.lblRaporEkrani = new System.Windows.Forms.Label();
            this.lblCikisYap = new System.Windows.Forms.Label();
            this.lblSergiciTanimlama = new System.Windows.Forms.Label();
            this.lblDashboard = new System.Windows.Forms.Label();
            this.lblGalericiAdi = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblYardim = new System.Windows.Forms.Label();
            this.lblHakkimizda = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridViewSergiciler = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridViewSergiler = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSergi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiciler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiler)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(350, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 149;
            this.pictureBox1.TabStop = false;
            // 
            // chartKazanc
            // 
            chartArea1.Name = "Toplam Kazanç";
            this.chartKazanc.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartKazanc.Legends.Add(legend1);
            this.chartKazanc.Location = new System.Drawing.Point(172, 102);
            this.chartKazanc.Name = "chartKazanc";
            series1.ChartArea = "Toplam Kazanç";
            series1.Legend = "Legend1";
            series1.Name = "Toplam Kazanç";
            this.chartKazanc.Series.Add(series1);
            this.chartKazanc.Size = new System.Drawing.Size(409, 138);
            this.chartKazanc.TabIndex = 151;
            this.chartKazanc.Text = "chart2";
            // 
            // chartSergi
            // 
            chartArea2.Name = "ChartArea1";
            this.chartSergi.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartSergi.Legends.Add(legend2);
            this.chartSergi.Location = new System.Drawing.Point(579, 102);
            this.chartSergi.Name = "chartSergi";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Toplam Sergi Sayısı";
            this.chartSergi.Series.Add(series2);
            this.chartSergi.Size = new System.Drawing.Size(436, 138);
            this.chartSergi.TabIndex = 150;
            this.chartSergi.Text = "chart1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.SlateGray;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(40, 33);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(119, 103);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 160;
            this.pictureBox2.TabStop = false;
            // 
            // lblSergiler
            // 
            this.lblSergiler.AutoSize = true;
            this.lblSergiler.BackColor = System.Drawing.Color.SlateGray;
            this.lblSergiler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSergiler.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSergiler.Location = new System.Drawing.Point(66, 351);
            this.lblSergiler.Name = "lblSergiler";
            this.lblSergiler.Size = new System.Drawing.Size(66, 18);
            this.lblSergiler.TabIndex = 159;
            this.lblSergiler.Text = "Sergiler";
            this.lblSergiler.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSergiler.Click += new System.EventHandler(this.lblSergiler_Click);
            // 
            // lblMusteriBilgileri
            // 
            this.lblMusteriBilgileri.AutoSize = true;
            this.lblMusteriBilgileri.BackColor = System.Drawing.Color.SlateGray;
            this.lblMusteriBilgileri.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblMusteriBilgileri.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMusteriBilgileri.Location = new System.Drawing.Point(40, 296);
            this.lblMusteriBilgileri.Name = "lblMusteriBilgileri";
            this.lblMusteriBilgileri.Size = new System.Drawing.Size(124, 18);
            this.lblMusteriBilgileri.TabIndex = 158;
            this.lblMusteriBilgileri.Text = "Müşteri Bilgileri";
            this.lblMusteriBilgileri.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMusteriBilgileri.Click += new System.EventHandler(this.lblMusteriBilgileri_Click);
            // 
            // lblRaporEkrani
            // 
            this.lblRaporEkrani.AutoSize = true;
            this.lblRaporEkrani.BackColor = System.Drawing.Color.SlateGray;
            this.lblRaporEkrani.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblRaporEkrani.ForeColor = System.Drawing.SystemColors.Control;
            this.lblRaporEkrani.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRaporEkrani.Location = new System.Drawing.Point(40, 408);
            this.lblRaporEkrani.Name = "lblRaporEkrani";
            this.lblRaporEkrani.Size = new System.Drawing.Size(107, 18);
            this.lblRaporEkrani.TabIndex = 157;
            this.lblRaporEkrani.Text = "Rapor Ekranı";
            this.lblRaporEkrani.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblCikisYap
            // 
            this.lblCikisYap.AutoSize = true;
            this.lblCikisYap.BackColor = System.Drawing.Color.SlateGray;
            this.lblCikisYap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCikisYap.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCikisYap.Location = new System.Drawing.Point(53, 470);
            this.lblCikisYap.Name = "lblCikisYap";
            this.lblCikisYap.Size = new System.Drawing.Size(79, 18);
            this.lblCikisYap.TabIndex = 156;
            this.lblCikisYap.Text = "Çıkış Yap";
            this.lblCikisYap.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblCikisYap.Click += new System.EventHandler(this.lblCikisYap_Click);
            // 
            // lblSergiciTanimlama
            // 
            this.lblSergiciTanimlama.AutoSize = true;
            this.lblSergiciTanimlama.BackColor = System.Drawing.Color.SlateGray;
            this.lblSergiciTanimlama.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSergiciTanimlama.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSergiciTanimlama.Location = new System.Drawing.Point(27, 242);
            this.lblSergiciTanimlama.Name = "lblSergiciTanimlama";
            this.lblSergiciTanimlama.Size = new System.Drawing.Size(147, 18);
            this.lblSergiciTanimlama.TabIndex = 155;
            this.lblSergiciTanimlama.Text = "Sergici Tanımlama";
            this.lblSergiciTanimlama.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSergiciTanimlama.Click += new System.EventHandler(this.lblSergiciTanimlama_Click);
            // 
            // lblDashboard
            // 
            this.lblDashboard.AutoSize = true;
            this.lblDashboard.BackColor = System.Drawing.Color.SlateGray;
            this.lblDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDashboard.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblDashboard.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDashboard.Location = new System.Drawing.Point(53, 190);
            this.lblDashboard.Name = "lblDashboard";
            this.lblDashboard.Size = new System.Drawing.Size(90, 18);
            this.lblDashboard.TabIndex = 154;
            this.lblDashboard.Text = "Dashboard";
            this.lblDashboard.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDashboard.Click += new System.EventHandler(this.lblDashboard_Click);
            // 
            // lblGalericiAdi
            // 
            this.lblGalericiAdi.AutoSize = true;
            this.lblGalericiAdi.BackColor = System.Drawing.Color.SlateGray;
            this.lblGalericiAdi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblGalericiAdi.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblGalericiAdi.Location = new System.Drawing.Point(53, 139);
            this.lblGalericiAdi.Name = "lblGalericiAdi";
            this.lblGalericiAdi.Size = new System.Drawing.Size(66, 18);
            this.lblGalericiAdi.TabIndex = 153;
            this.lblGalericiAdi.Text = "Profilim";
            this.lblGalericiAdi.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblGalericiAdi.Click += new System.EventHandler(this.lblGalericiAdi_Click);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.SlateGray;
            this.label21.Location = new System.Drawing.Point(0, -1);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(201, 592);
            this.label21.TabIndex = 152;
            // 
            // lblYardim
            // 
            this.lblYardim.AutoSize = true;
            this.lblYardim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblYardim.Location = new System.Drawing.Point(655, 552);
            this.lblYardim.Name = "lblYardim";
            this.lblYardim.Size = new System.Drawing.Size(17, 18);
            this.lblYardim.TabIndex = 162;
            this.lblYardim.Text = "?";
            this.lblYardim.Click += new System.EventHandler(this.lblYardim_Click);
            // 
            // lblHakkimizda
            // 
            this.lblHakkimizda.BackColor = System.Drawing.SystemColors.Control;
            this.lblHakkimizda.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHakkimizda.Location = new System.Drawing.Point(203, 539);
            this.lblHakkimizda.Name = "lblHakkimizda";
            this.lblHakkimizda.Size = new System.Drawing.Size(816, 43);
            this.lblHakkimizda.TabIndex = 161;
            this.lblHakkimizda.Text = "Hakkımızda     ";
            this.lblHakkimizda.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHakkimizda.Click += new System.EventHandler(this.lblHakkimizda_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(216, 296);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 25);
            this.label3.TabIndex = 164;
            this.label3.Text = "Sergiciler";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dataGridViewSergiciler
            // 
            this.dataGridViewSergiciler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSergiciler.Location = new System.Drawing.Point(325, 246);
            this.dataGridViewSergiciler.Name = "dataGridViewSergiciler";
            this.dataGridViewSergiciler.RowHeadersWidth = 51;
            this.dataGridViewSergiciler.RowTemplate.Height = 24;
            this.dataGridViewSergiciler.Size = new System.Drawing.Size(645, 109);
            this.dataGridViewSergiciler.TabIndex = 163;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.SlateGray;
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(645, 498);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 38);
            this.button2.TabIndex = 165;
            this.button2.Text = "PDF Oluştur";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnPdfOlustur_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.SlateGray;
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(481, 498);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 38);
            this.button1.TabIndex = 166;
            this.button1.Text = "Excel Oluştur";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnExcelOlustur_Click);
            // 
            // dataGridViewSergiler
            // 
            this.dataGridViewSergiler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSergiler.Location = new System.Drawing.Point(325, 368);
            this.dataGridViewSergiler.Name = "dataGridViewSergiler";
            this.dataGridViewSergiler.RowHeadersWidth = 51;
            this.dataGridViewSergiler.RowTemplate.Height = 24;
            this.dataGridViewSergiler.Size = new System.Drawing.Size(645, 109);
            this.dataGridViewSergiler.TabIndex = 167;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label1.Location = new System.Drawing.Point(233, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 25);
            this.label1.TabIndex = 168;
            this.label1.Text = "Sergiler";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.SlateGray;
            this.btnReset.ForeColor = System.Drawing.SystemColors.Control;
            this.btnReset.Location = new System.Drawing.Point(239, 408);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(81, 43);
            this.btnReset.TabIndex = 169;
            this.btnReset.Text = "Tüm Sergiler";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnTumSergiler_Click);
            // 
            // GalericiRaporlar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewSergiler);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridViewSergiciler);
            this.Controls.Add(this.lblYardim);
            this.Controls.Add(this.lblHakkimizda);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblSergiler);
            this.Controls.Add(this.lblMusteriBilgileri);
            this.Controls.Add(this.lblRaporEkrani);
            this.Controls.Add(this.lblCikisYap);
            this.Controls.Add(this.lblSergiciTanimlama);
            this.Controls.Add(this.lblDashboard);
            this.Controls.Add(this.lblGalericiAdi);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.chartKazanc);
            this.Controls.Add(this.chartSergi);
            this.Controls.Add(this.pictureBox1);
            this.Name = "GalericiRaporlar";
            this.Text = "GalericiRaporlar";
            this.Load += new System.EventHandler(this.GalericiRaporlar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSergi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiciler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiler)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartKazanc;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSergi;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblSergiler;
        private System.Windows.Forms.Label lblMusteriBilgileri;
        private System.Windows.Forms.Label lblRaporEkrani;
        private System.Windows.Forms.Label lblCikisYap;
        private System.Windows.Forms.Label lblSergiciTanimlama;
        private System.Windows.Forms.Label lblDashboard;
        private System.Windows.Forms.Label lblGalericiAdi;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblYardim;
        private System.Windows.Forms.Label lblHakkimizda;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridViewSergiciler;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridViewSergiler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReset;
    }
}