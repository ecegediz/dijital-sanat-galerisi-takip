namespace dijitalsanatgalerisi
{
    partial class GalericiDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GalericiDashboard));
            this.lblHakkimizda = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chartKazanc = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSergi = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblToplamSergi = new System.Windows.Forms.Label();
            this.lblSergilenen = new System.Windows.Forms.Label();
            this.lblSatilan = new System.Windows.Forms.Label();
            this.lblBekleyen = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.LabelSergiler = new System.Windows.Forms.Label();
            this.LabelMusteriBilgileri = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.LabelCikisYap = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.LabelGalericiProfilim_Click = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblToplamKazanc = new System.Windows.Forms.Label();
            this.lblYardim = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSergi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHakkimizda
            // 
            this.lblHakkimizda.BackColor = System.Drawing.SystemColors.Control;
            this.lblHakkimizda.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHakkimizda.Location = new System.Drawing.Point(203, 539);
            this.lblHakkimizda.Name = "lblHakkimizda";
            this.lblHakkimizda.Size = new System.Drawing.Size(816, 43);
            this.lblHakkimizda.TabIndex = 117;
            this.lblHakkimizda.Text = "Hakkımızda     ";
            this.lblHakkimizda.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHakkimizda.Click += new System.EventHandler(this.lblHakkimizda_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Beige;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(-2, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1017, 439);
            this.label2.TabIndex = 116;
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Beige;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(525, 347);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 25);
            this.label3.TabIndex = 128;
            this.label3.Text = "Sergiciler";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // chartKazanc
            // 
            chartArea1.Name = "Toplam Kazanç";
            this.chartKazanc.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartKazanc.Legends.Add(legend1);
            this.chartKazanc.Location = new System.Drawing.Point(202, 102);
            this.chartKazanc.Name = "chartKazanc";
            series1.ChartArea = "Toplam Kazanç";
            series1.Legend = "Legend1";
            series1.Name = "Toplam Kazanç";
            this.chartKazanc.Series.Add(series1);
            this.chartKazanc.Size = new System.Drawing.Size(407, 138);
            this.chartKazanc.TabIndex = 127;
            this.chartKazanc.Text = "chart2";
            // 
            // chartSergi
            // 
            chartArea2.Name = "ChartArea1";
            this.chartSergi.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartSergi.Legends.Add(legend2);
            this.chartSergi.Location = new System.Drawing.Point(601, 102);
            this.chartSergi.Name = "chartSergi";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Toplam Sergi Sayısı";
            this.chartSergi.Series.Add(series2);
            this.chartSergi.Size = new System.Drawing.Size(414, 138);
            this.chartSergi.TabIndex = 126;
            this.chartSergi.Text = "chart1";
            this.chartSergi.Click += new System.EventHandler(this.chart1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(234, 375);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(723, 161);
            this.dataGridView1.TabIndex = 125;
            // 
            // lblToplamSergi
            // 
            this.lblToplamSergi.BackColor = System.Drawing.Color.Beige;
            this.lblToplamSergi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblToplamSergi.Location = new System.Drawing.Point(240, 313);
            this.lblToplamSergi.Name = "lblToplamSergi";
            this.lblToplamSergi.Size = new System.Drawing.Size(65, 37);
            this.lblToplamSergi.TabIndex = 131;
            this.lblToplamSergi.Text = "1";
            this.lblToplamSergi.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblSergilenen
            // 
            this.lblSergilenen.BackColor = System.Drawing.Color.Beige;
            this.lblSergilenen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSergilenen.Location = new System.Drawing.Point(409, 313);
            this.lblSergilenen.Name = "lblSergilenen";
            this.lblSergilenen.Size = new System.Drawing.Size(73, 37);
            this.lblSergilenen.TabIndex = 132;
            this.lblSergilenen.Text = "2";
            this.lblSergilenen.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblSatilan
            // 
            this.lblSatilan.BackColor = System.Drawing.Color.Beige;
            this.lblSatilan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSatilan.Location = new System.Drawing.Point(570, 313);
            this.lblSatilan.Name = "lblSatilan";
            this.lblSatilan.Size = new System.Drawing.Size(70, 38);
            this.lblSatilan.TabIndex = 133;
            this.lblSatilan.Text = "3";
            this.lblSatilan.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBekleyen
            // 
            this.lblBekleyen.BackColor = System.Drawing.Color.Beige;
            this.lblBekleyen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBekleyen.Location = new System.Drawing.Point(732, 313);
            this.lblBekleyen.Name = "lblBekleyen";
            this.lblBekleyen.Size = new System.Drawing.Size(59, 43);
            this.lblBekleyen.TabIndex = 134;
            this.lblBekleyen.Text = "4";
            this.lblBekleyen.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Beige;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label10.Location = new System.Drawing.Point(202, 250);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 65);
            this.label10.TabIndex = 136;
            this.label10.Text = "Toplam Sergi:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Beige;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.Location = new System.Drawing.Point(394, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(131, 63);
            this.label11.TabIndex = 137;
            this.label11.Text = "Sergilenen:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.Beige;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label19.Location = new System.Drawing.Point(556, 250);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(97, 63);
            this.label19.TabIndex = 138;
            this.label19.Text = "Satılan:";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Beige;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label20.Location = new System.Drawing.Point(699, 250);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(147, 63);
            this.label20.TabIndex = 139;
            this.label20.Text = "Bekleyen:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelSergiler
            // 
            this.LabelSergiler.AutoSize = true;
            this.LabelSergiler.BackColor = System.Drawing.Color.SlateGray;
            this.LabelSergiler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LabelSergiler.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LabelSergiler.Location = new System.Drawing.Point(64, 352);
            this.LabelSergiler.Name = "LabelSergiler";
            this.LabelSergiler.Size = new System.Drawing.Size(66, 18);
            this.LabelSergiler.TabIndex = 147;
            this.LabelSergiler.Text = "Sergiler";
            this.LabelSergiler.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LabelSergiler.Click += new System.EventHandler(this.LabelSergiler_Click);
            // 
            // LabelMusteriBilgileri
            // 
            this.LabelMusteriBilgileri.AutoSize = true;
            this.LabelMusteriBilgileri.BackColor = System.Drawing.Color.SlateGray;
            this.LabelMusteriBilgileri.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LabelMusteriBilgileri.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LabelMusteriBilgileri.Location = new System.Drawing.Point(38, 297);
            this.LabelMusteriBilgileri.Name = "LabelMusteriBilgileri";
            this.LabelMusteriBilgileri.Size = new System.Drawing.Size(124, 18);
            this.LabelMusteriBilgileri.TabIndex = 146;
            this.LabelMusteriBilgileri.Text = "Müşteri Bilgileri";
            this.LabelMusteriBilgileri.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LabelMusteriBilgileri.Click += new System.EventHandler(this.LabelMusteriBilgileri_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.SlateGray;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label13.Location = new System.Drawing.Point(38, 409);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(107, 18);
            this.label13.TabIndex = 145;
            this.label13.Text = "Rapor Ekranı";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label13.Click += new System.EventHandler(this.LabelRaporEkrani_Click);
            // 
            // LabelCikisYap
            // 
            this.LabelCikisYap.AutoSize = true;
            this.LabelCikisYap.BackColor = System.Drawing.Color.SlateGray;
            this.LabelCikisYap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LabelCikisYap.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LabelCikisYap.Location = new System.Drawing.Point(51, 471);
            this.LabelCikisYap.Name = "LabelCikisYap";
            this.LabelCikisYap.Size = new System.Drawing.Size(79, 18);
            this.LabelCikisYap.TabIndex = 144;
            this.LabelCikisYap.Text = "Çıkış Yap";
            this.LabelCikisYap.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LabelCikisYap.Click += new System.EventHandler(this.LabelCikisYap_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.SlateGray;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label14.Location = new System.Drawing.Point(25, 243);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(147, 18);
            this.label14.TabIndex = 143;
            this.label14.Text = "Sergici Tanımlama";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label14.Click += new System.EventHandler(this.LabelSergiciTanimlama_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.SlateGray;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label17.ForeColor = System.Drawing.SystemColors.Control;
            this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.Location = new System.Drawing.Point(51, 191);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(90, 18);
            this.label17.TabIndex = 142;
            this.label17.Text = "Dashboard";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label17.Click += new System.EventHandler(this.LabelDashboard_Click);
            // 
            // LabelGalericiProfilim_Click
            // 
            this.LabelGalericiProfilim_Click.AutoSize = true;
            this.LabelGalericiProfilim_Click.BackColor = System.Drawing.Color.SlateGray;
            this.LabelGalericiProfilim_Click.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LabelGalericiProfilim_Click.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LabelGalericiProfilim_Click.Location = new System.Drawing.Point(64, 140);
            this.LabelGalericiProfilim_Click.Name = "LabelGalericiProfilim_Click";
            this.LabelGalericiProfilim_Click.Size = new System.Drawing.Size(66, 18);
            this.LabelGalericiProfilim_Click.TabIndex = 141;
            this.LabelGalericiProfilim_Click.Text = "Profilim";
            this.LabelGalericiProfilim_Click.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LabelGalericiProfilim_Click.Click += new System.EventHandler(this.LabelGalericiProfilim_Click_Click);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.SlateGray;
            this.label21.Location = new System.Drawing.Point(-2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(201, 592);
            this.label21.TabIndex = 140;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(350, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 148;
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
            this.pictureBox2.TabIndex = 149;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Beige;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(883, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 57);
            this.label1.TabIndex = 151;
            this.label1.Text = "Toplam Kazanç:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblToplamKazanc
            // 
            this.lblToplamKazanc.BackColor = System.Drawing.Color.Beige;
            this.lblToplamKazanc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblToplamKazanc.Location = new System.Drawing.Point(883, 313);
            this.lblToplamKazanc.Name = "lblToplamKazanc";
            this.lblToplamKazanc.Size = new System.Drawing.Size(81, 38);
            this.lblToplamKazanc.TabIndex = 150;
            this.lblToplamKazanc.Text = "5";
            this.lblToplamKazanc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblYardim
            // 
            this.lblYardim.AutoSize = true;
            this.lblYardim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblYardim.Location = new System.Drawing.Point(655, 552);
            this.lblYardim.Name = "lblYardim";
            this.lblYardim.Size = new System.Drawing.Size(17, 18);
            this.lblYardim.TabIndex = 152;
            this.lblYardim.Text = "?";
            this.lblYardim.Click += new System.EventHandler(this.lblYardim_Click);
            // 
            // GalericiDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.lblYardim);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblToplamKazanc);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LabelSergiler);
            this.Controls.Add(this.LabelMusteriBilgileri);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.LabelCikisYap);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.LabelGalericiProfilim_Click);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblBekleyen);
            this.Controls.Add(this.lblSatilan);
            this.Controls.Add(this.lblSergilenen);
            this.Controls.Add(this.lblToplamSergi);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chartKazanc);
            this.Controls.Add(this.chartSergi);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblHakkimizda);
            this.Controls.Add(this.label2);
            this.Name = "GalericiDashboard";
            this.Text = "Galerici Dashboard";
            this.Load += new System.EventHandler(this.GalericiDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartKazanc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSergi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblHakkimizda;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartKazanc;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSergi;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblToplamSergi;
        private System.Windows.Forms.Label lblSergilenen;
        private System.Windows.Forms.Label lblSatilan;
        private System.Windows.Forms.Label lblBekleyen;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label LabelSergiler;
        private System.Windows.Forms.Label LabelMusteriBilgileri;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label LabelCikisYap;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label LabelGalericiProfilim_Click;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblToplamKazanc;
        private System.Windows.Forms.Label lblYardim;
    }
}