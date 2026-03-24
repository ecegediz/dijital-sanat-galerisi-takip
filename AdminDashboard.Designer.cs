using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;  // BU SATIRI EKLEDİM

namespace dijitalsanatgalerisi
{
    partial class AdminDashboard
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminDashboard));
            this.raporlarlabel = new System.Windows.Forms.Label();
            this.cikisyaplabel = new System.Windows.Forms.Label();
            this.şifrebasvurularılabel = new System.Windows.Forms.Label();
            this.galericitanimlamalabel = new System.Windows.Forms.Label();
            this.dashboardlabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // raporlarlabel
            // 
            this.raporlarlabel.AutoSize = true;
            this.raporlarlabel.BackColor = System.Drawing.Color.SlateGray;
            this.raporlarlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.raporlarlabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.raporlarlabel.Location = new System.Drawing.Point(61, 395);
            this.raporlarlabel.Name = "raporlarlabel";
            this.raporlarlabel.Size = new System.Drawing.Size(73, 18);
            this.raporlarlabel.TabIndex = 116;
            this.raporlarlabel.Text = "Raporlar";
            this.raporlarlabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.raporlarlabel.Click += new System.EventHandler(this.raporlarlabel_Click);
            // 
            // cikisyaplabel
            // 
            this.cikisyaplabel.AutoSize = true;
            this.cikisyaplabel.BackColor = System.Drawing.Color.SlateGray;
            this.cikisyaplabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cikisyaplabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cikisyaplabel.Location = new System.Drawing.Point(53, 459);
            this.cikisyaplabel.Name = "cikisyaplabel";
            this.cikisyaplabel.Size = new System.Drawing.Size(79, 18);
            this.cikisyaplabel.TabIndex = 115;
            this.cikisyaplabel.Text = "Çıkış Yap";
            this.cikisyaplabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cikisyaplabel.Click += new System.EventHandler(this.cikisyaplabel_Click_1);
            // 
            // şifrebasvurularılabel
            // 
            this.şifrebasvurularılabel.AutoSize = true;
            this.şifrebasvurularılabel.BackColor = System.Drawing.Color.SlateGray;
            this.şifrebasvurularılabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.şifrebasvurularılabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.şifrebasvurularılabel.Location = new System.Drawing.Point(35, 329);
            this.şifrebasvurularılabel.Name = "şifrebasvurularılabel";
            this.şifrebasvurularılabel.Size = new System.Drawing.Size(132, 18);
            this.şifrebasvurularılabel.TabIndex = 114;
            this.şifrebasvurularılabel.Text = "Şifre Başvuruları";
            this.şifrebasvurularılabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.şifrebasvurularılabel.Click += new System.EventHandler(this.şifrebasvurularılabel_Click);
            // 
            // galericitanimlamalabel
            // 
            this.galericitanimlamalabel.AutoSize = true;
            this.galericitanimlamalabel.BackColor = System.Drawing.Color.SlateGray;
            this.galericitanimlamalabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.galericitanimlamalabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.galericitanimlamalabel.Location = new System.Drawing.Point(14, 265);
            this.galericitanimlamalabel.Name = "galericitanimlamalabel";
            this.galericitanimlamalabel.Size = new System.Drawing.Size(153, 18);
            this.galericitanimlamalabel.TabIndex = 113;
            this.galericitanimlamalabel.Text = "Galerici Tanımlama";
            this.galericitanimlamalabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.galericitanimlamalabel.Click += new System.EventHandler(this.galericitanimlamalabel_Click);
            // 
            // dashboardlabel
            // 
            this.dashboardlabel.AutoSize = true;
            this.dashboardlabel.BackColor = System.Drawing.Color.SlateGray;
            this.dashboardlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.dashboardlabel.ForeColor = System.Drawing.SystemColors.Control;
            this.dashboardlabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dashboardlabel.Location = new System.Drawing.Point(53, 199);
            this.dashboardlabel.Name = "dashboardlabel";
            this.dashboardlabel.Size = new System.Drawing.Size(90, 18);
            this.dashboardlabel.TabIndex = 112;
            this.dashboardlabel.Text = "Dashboard";
            this.dashboardlabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.SlateGray;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.Location = new System.Drawing.Point(61, 140);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(54, 18);
            this.label17.TabIndex = 111;
            this.label17.Text = "Admin";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.SlateGray;
            this.label18.Location = new System.Drawing.Point(-2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(201, 582);
            this.label18.TabIndex = 110;
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // chart2
            // 
            chartArea1.Name = "Toplam Kazanç";
            this.chart2.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart2.Legends.Add(legend1);
            this.chart2.Location = new System.Drawing.Point(200, 151);
            this.chart2.Name = "chart2";
            series1.ChartArea = "Toplam Kazanç";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Toplam Kazanç";
            this.chart2.Series.Add(series1);
            this.chart2.Size = new System.Drawing.Size(410, 162);
            this.chart2.TabIndex = 108;
            this.chart2.Text = "chart2";
            this.chart2.Click += new System.EventHandler(this.chart2_Click);
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(604, 151);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Toplam Galerici";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(419, 162);
            this.chart1.TabIndex = 107;
            this.chart1.Text = "chart1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(227, 343);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(723, 154);
            this.dataGridView1.TabIndex = 106;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label10.Location = new System.Drawing.Point(203, 539);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(816, 43);
            this.label10.TabIndex = 118;
            this.label10.Text = "Hakkımızda     ";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label10.Click += new System.EventHandler(this.lblHakkimizda_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(350, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 119;
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
            this.pictureBox2.TabIndex = 159;
            this.pictureBox2.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.Location = new System.Drawing.Point(655, 552);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 18);
            this.label11.TabIndex = 161;
            this.label11.Text = "?";
            this.label11.Click += new System.EventHandler(this.lblYardim_Click);
            // 
            // AdminDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.raporlarlabel);
            this.Controls.Add(this.cikisyaplabel);
            this.Controls.Add(this.şifrebasvurularılabel);
            this.Controls.Add(this.galericitanimlamalabel);
            this.Controls.Add(this.dashboardlabel);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "AdminDashboard";
            this.Text = "AdminDashboard";
            this.Load += new System.EventHandler(this.AdminDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label raporlarlabel;
        private System.Windows.Forms.Label cikisyaplabel;
        private System.Windows.Forms.Label şifrebasvurularılabel;
        private System.Windows.Forms.Label galericitanimlamalabel;
        private System.Windows.Forms.Label dashboardlabel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label10;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Label label11;
    }
    #endregion
}