using System;

namespace dijitalsanatgalerisi
{
    partial class SergiciDashboard
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SergiciDashboard));
            this.labelSergiciAdi = new System.Windows.Forms.Label();
            this.cikisYapButton = new System.Windows.Forms.Label();
            this.eserlerimButton = new System.Windows.Forms.Label();
            this.dashboardButton = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.labelBekleyen = new System.Windows.Forms.Label();
            this.labelSatilan = new System.Windows.Forms.Label();
            this.labelSergilenen = new System.Windows.Forms.Label();
            this.labelToplamSergi = new System.Windows.Forms.Label();
            this.chartSergiler = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelToplamSergi2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chartKazanc = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.hakkımızdaButton = new System.Windows.Forms.Label();
            this.raporButton = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewSergiciler = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartSergiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiciler)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSergiciAdi
            // 
            this.labelSergiciAdi.AutoSize = true;
            this.labelSergiciAdi.BackColor = System.Drawing.Color.SlateGray;
            this.labelSergiciAdi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelSergiciAdi.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSergiciAdi.Location = new System.Drawing.Point(54, 140);
            this.labelSergiciAdi.Name = "labelSergiciAdi";
            this.labelSergiciAdi.Size = new System.Drawing.Size(88, 18);
            this.labelSergiciAdi.TabIndex = 168;
            this.labelSergiciAdi.Text = "Sergici Adı";
            this.labelSergiciAdi.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelSergiciAdi.Click += new System.EventHandler(this.label12_Click);
            // 
            // cikisYapButton
            // 
            this.cikisYapButton.AutoSize = true;
            this.cikisYapButton.BackColor = System.Drawing.Color.SlateGray;
            this.cikisYapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cikisYapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cikisYapButton.Location = new System.Drawing.Point(50, 450);
            this.cikisYapButton.Name = "cikisYapButton";
            this.cikisYapButton.Size = new System.Drawing.Size(79, 18);
            this.cikisYapButton.TabIndex = 166;
            this.cikisYapButton.Text = "Çıkış Yap";
            this.cikisYapButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cikisYapButton.Click += new System.EventHandler(this.cikisYapButton_Click);
            // 
            // eserlerimButton
            // 
            this.eserlerimButton.AutoSize = true;
            this.eserlerimButton.BackColor = System.Drawing.Color.SlateGray;
            this.eserlerimButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.eserlerimButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.eserlerimButton.Location = new System.Drawing.Point(51, 275);
            this.eserlerimButton.Name = "eserlerimButton";
            this.eserlerimButton.Size = new System.Drawing.Size(80, 18);
            this.eserlerimButton.TabIndex = 165;
            this.eserlerimButton.Text = "Eserlerim";
            this.eserlerimButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.eserlerimButton.Click += new System.EventHandler(this.eserlerimButton_Click);
            // 
            // dashboardButton
            // 
            this.dashboardButton.AutoSize = true;
            this.dashboardButton.BackColor = System.Drawing.Color.SlateGray;
            this.dashboardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.dashboardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dashboardButton.Location = new System.Drawing.Point(51, 191);
            this.dashboardButton.Name = "dashboardButton";
            this.dashboardButton.Size = new System.Drawing.Size(90, 18);
            this.dashboardButton.TabIndex = 164;
            this.dashboardButton.Text = "Dashboard";
            this.dashboardButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.dashboardButton.Click += new System.EventHandler(this.dashboardButton_Click);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.SlateGray;
            this.label21.Location = new System.Drawing.Point(-2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(201, 592);
            this.label21.TabIndex = 162;
            // 
            // labelBekleyen
            // 
            this.labelBekleyen.BackColor = System.Drawing.Color.Beige;
            this.labelBekleyen.Location = new System.Drawing.Point(859, 312);
            this.labelBekleyen.Name = "labelBekleyen";
            this.labelBekleyen.Size = new System.Drawing.Size(97, 37);
            this.labelBekleyen.TabIndex = 161;
            this.labelBekleyen.Text = "iki2";
            this.labelBekleyen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSatilan
            // 
            this.labelSatilan.BackColor = System.Drawing.Color.Beige;
            this.labelSatilan.Location = new System.Drawing.Point(670, 312);
            this.labelSatilan.Name = "labelSatilan";
            this.labelSatilan.Size = new System.Drawing.Size(97, 37);
            this.labelSatilan.TabIndex = 160;
            this.labelSatilan.Text = "üç3";
            this.labelSatilan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSergilenen
            // 
            this.labelSergilenen.BackColor = System.Drawing.Color.Beige;
            this.labelSergilenen.Location = new System.Drawing.Point(478, 312);
            this.labelSergilenen.Name = "labelSergilenen";
            this.labelSergilenen.Size = new System.Drawing.Size(97, 37);
            this.labelSergilenen.TabIndex = 159;
            this.labelSergilenen.Text = "beş5";
            this.labelSergilenen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelToplamSergi
            // 
            this.labelToplamSergi.BackColor = System.Drawing.Color.Beige;
            this.labelToplamSergi.Location = new System.Drawing.Point(274, 312);
            this.labelToplamSergi.Name = "labelToplamSergi";
            this.labelToplamSergi.Size = new System.Drawing.Size(97, 37);
            this.labelToplamSergi.TabIndex = 158;
            this.labelToplamSergi.Text = "on10";
            this.labelToplamSergi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chartSergiler
            // 
            chartArea1.Name = "ChartArea1";
            this.chartSergiler.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartSergiler.Legends.Add(legend1);
            this.chartSergiler.Location = new System.Drawing.Point(644, 117);
            this.chartSergiler.Name = "chartSergiler";
            this.chartSergiler.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "Sergiler";
            this.chartSergiler.Series.Add(series1);
            this.chartSergiler.Size = new System.Drawing.Size(370, 146);
            this.chartSergiler.TabIndex = 157;
            this.chartSergiler.Text = "chart3";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Beige;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label8.Location = new System.Drawing.Point(831, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(157, 84);
            this.label8.TabIndex = 156;
            this.label8.Text = "Bekleyen:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Beige;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(640, 279);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(157, 84);
            this.label7.TabIndex = 155;
            this.label7.Text = "Satılan:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Beige;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(447, 279);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 84);
            this.label6.TabIndex = 154;
            this.label6.Text = "Sergilenen:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelToplamSergi2
            // 
            this.labelToplamSergi2.BackColor = System.Drawing.Color.Beige;
            this.labelToplamSergi2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelToplamSergi2.Location = new System.Drawing.Point(246, 279);
            this.labelToplamSergi2.Name = "labelToplamSergi2";
            this.labelToplamSergi2.Size = new System.Drawing.Size(157, 84);
            this.labelToplamSergi2.TabIndex = 153;
            this.labelToplamSergi2.Text = "Toplam Sergi:";
            this.labelToplamSergi2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.SlateGray;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(197, 266);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(817, 104);
            this.label4.TabIndex = 152;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chartKazanc
            // 
            chartArea2.Name = "Toplam Kazanç";
            this.chartKazanc.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartKazanc.Legends.Add(legend2);
            this.chartKazanc.Location = new System.Drawing.Point(201, 117);
            this.chartKazanc.Name = "chartKazanc";
            series2.ChartArea = "Toplam Kazanç";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Toplam Kazanç";
            this.chartKazanc.Series.Add(series2);
            this.chartKazanc.Size = new System.Drawing.Size(448, 146);
            this.chartKazanc.TabIndex = 151;
            this.chartKazanc.Text = "chart2";
            // 
            // hakkımızdaButton
            // 
            this.hakkımızdaButton.BackColor = System.Drawing.SystemColors.Control;
            this.hakkımızdaButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.hakkımızdaButton.Location = new System.Drawing.Point(214, 545);
            this.hakkımızdaButton.Name = "hakkımızdaButton";
            this.hakkımızdaButton.Size = new System.Drawing.Size(800, 41);
            this.hakkımızdaButton.TabIndex = 149;
            this.hakkımızdaButton.Text = "Hakkımızda     ";
            this.hakkımızdaButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.hakkımızdaButton.Click += new System.EventHandler(this.hakkımızdaButton_Click);
            // 
            // raporButton
            // 
            this.raporButton.AutoSize = true;
            this.raporButton.BackColor = System.Drawing.Color.SlateGray;
            this.raporButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.raporButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.raporButton.Location = new System.Drawing.Point(50, 361);
            this.raporButton.Name = "raporButton";
            this.raporButton.Size = new System.Drawing.Size(107, 18);
            this.raporButton.TabIndex = 167;
            this.raporButton.Text = "Rapor Ekranı";
            this.raporButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.raporButton.Click += new System.EventHandler(this.raporButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(350, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 169;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.SlateGray;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(38, 34);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(119, 103);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 170;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(655, 552);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 18);
            this.label1.TabIndex = 171;
            this.label1.Text = "?";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // dataGridViewSergiciler
            // 
            this.dataGridViewSergiciler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSergiciler.Location = new System.Drawing.Point(229, 394);
            this.dataGridViewSergiciler.Name = "dataGridViewSergiciler";
            this.dataGridViewSergiciler.RowHeadersWidth = 51;
            this.dataGridViewSergiciler.RowTemplate.Height = 24;
            this.dataGridViewSergiciler.Size = new System.Drawing.Size(759, 148);
            this.dataGridViewSergiciler.TabIndex = 172;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(237, 371);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 20);
            this.label2.TabIndex = 173;
            this.label2.Text = "Sergilerim";
            // 
            // SergiciDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewSergiciler);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelSergiciAdi);
            this.Controls.Add(this.raporButton);
            this.Controls.Add(this.cikisYapButton);
            this.Controls.Add(this.eserlerimButton);
            this.Controls.Add(this.dashboardButton);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.labelBekleyen);
            this.Controls.Add(this.labelSatilan);
            this.Controls.Add(this.labelSergilenen);
            this.Controls.Add(this.labelToplamSergi);
            this.Controls.Add(this.chartSergiler);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelToplamSergi2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chartKazanc);
            this.Controls.Add(this.hakkımızdaButton);
            this.Name = "SergiciDashboard";
            this.Text = "SergiciDashboard";
            this.Load += new System.EventHandler(this.SergiciDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartSergiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSergiciler)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelSergiciAdi;
        private System.Windows.Forms.Label cikisYapButton;
        private System.Windows.Forms.Label eserlerimButton;
        private System.Windows.Forms.Label dashboardButton;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label labelBekleyen;
        private System.Windows.Forms.Label labelSatilan;
        private System.Windows.Forms.Label labelSergilenen;
        private System.Windows.Forms.Label labelToplamSergi;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSergiler;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelToplamSergi2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartKazanc;
        private System.Windows.Forms.Label hakkımızdaButton;
        private System.Windows.Forms.Label raporButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewSergiciler;
        private System.Windows.Forms.Label label2;
    }
}