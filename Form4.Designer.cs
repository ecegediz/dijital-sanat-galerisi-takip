using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;  // BU SATIRI EKLEDİM

namespace dijitalsanatgalerisi
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Galerici = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eposta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Şifre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Durum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fiyat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.SlateGray;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label12.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label12.Location = new System.Drawing.Point(48, 422);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 18);
            this.label12.TabIndex = 116;
            this.label12.Text = "Raporlar";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.SlateGray;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label16.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label16.Location = new System.Drawing.Point(48, 474);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 18);
            this.label16.TabIndex = 115;
            this.label16.Text = "Çıkış Yap";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.SlateGray;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label15.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label15.Location = new System.Drawing.Point(30, 375);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(132, 18);
            this.label15.TabIndex = 114;
            this.label15.Text = "Şifre Başvuruları";
            this.label15.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.SlateGray;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label14.Location = new System.Drawing.Point(18, 322);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(153, 18);
            this.label14.TabIndex = 113;
            this.label14.Text = "Galerici Tanımlama";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.SlateGray;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label13.Location = new System.Drawing.Point(48, 274);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 18);
            this.label13.TabIndex = 112;
            this.label13.Text = "Dashboard";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.SlateGray;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.Location = new System.Drawing.Point(48, 222);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 18);
            this.label17.TabIndex = 111;
            this.label17.Text = "Admin Adı";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.SlateGray;
            this.label18.Location = new System.Drawing.Point(-7, -8);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(201, 589);
            this.label18.TabIndex = 110;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.SteelBlue;
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(489, 500);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 38);
            this.button2.TabIndex = 109;
            this.button2.Text = "Galerici Detaylarına Git";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // chart2
            // 
            chartArea3.Name = "Toplam Kazanç";
            this.chart2.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart2.Legends.Add(legend3);
            this.chart2.Location = new System.Drawing.Point(200, 151);
            this.chart2.Name = "chart2";
            series3.ChartArea = "Toplam Kazanç";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Toplam Kazanç";
            this.chart2.Series.Add(series3);
            this.chart2.Size = new System.Drawing.Size(410, 162);
            this.chart2.TabIndex = 108;
            this.chart2.Text = "chart2";
            // 
            // chart1
            // 
            chartArea4.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chart1.Legends.Add(legend4);
            this.chart1.Location = new System.Drawing.Point(604, 151);
            this.chart1.Name = "chart1";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Toplam Galerici";
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(419, 162);
            this.chart1.TabIndex = 107;
            this.chart1.Text = "chart1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Galerici,
            this.eposta,
            this.Şifre,
            this.Durum,
            this.Fiyat});
            this.dataGridView1.Location = new System.Drawing.Point(227, 343);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(723, 154);
            this.dataGridView1.TabIndex = 106;
            // 
            // Galerici
            // 
            this.Galerici.HeaderText = "Galerici";
            this.Galerici.MinimumWidth = 6;
            this.Galerici.Name = "Galerici";
            this.Galerici.Width = 130;
            // 
            // eposta
            // 
            this.eposta.HeaderText = "E-posta";
            this.eposta.MinimumWidth = 6;
            this.eposta.Name = "eposta";
            this.eposta.Width = 150;
            // 
            // Şifre
            // 
            this.Şifre.HeaderText = "Şifre";
            this.Şifre.MinimumWidth = 6;
            this.Şifre.Name = "Şifre";
            this.Şifre.Width = 130;
            // 
            // Durum
            // 
            this.Durum.HeaderText = "Durum";
            this.Durum.MinimumWidth = 6;
            this.Durum.Name = "Durum";
            this.Durum.Width = 130;
            // 
            // Fiyat
            // 
            this.Fiyat.HeaderText = "Fiyat";
            this.Fiyat.MinimumWidth = 6;
            this.Fiyat.Name = "Fiyat";
            this.Fiyat.Width = 130;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label8.Location = new System.Drawing.Point(200, -1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(812, 103);
            this.label8.TabIndex = 117;
            this.label8.Text = "ART FLOW Dijital Sanat Galerisi";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label10.Location = new System.Drawing.Point(200, 537);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(811, 44);
            this.label10.TabIndex = 118;
            this.label10.Text = "Hakkımızda ?";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form4
            // 
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form4";
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Galerici;
        private System.Windows.Forms.DataGridViewTextBoxColumn eposta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Şifre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Durum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fiyat;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label10;
    }
    #endregion
}