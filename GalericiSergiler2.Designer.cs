using System.Windows.Forms;

namespace dijitalsanatgalerisi
{
    partial class GalericiSergiler2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GalericiSergiler2));
            this.btnGeri = new System.Windows.Forms.Button();
            this.btnIleri1 = new System.Windows.Forms.Button();
            this.btnIptal1 = new System.Windows.Forms.Button();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSergiler = new System.Windows.Forms.Label();
            this.lblMusteriBilgileri = new System.Windows.Forms.Label();
            this.lblRaporEkrani = new System.Windows.Forms.Label();
            this.lblCikisYap = new System.Windows.Forms.Label();
            this.lblSergiciTanimlama = new System.Windows.Forms.Label();
            this.lblDashboard = new System.Windows.Forms.Label();
            this.lblGalericiAdi = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.dtpBitis = new System.Windows.Forms.DateTimePicker();
            this.dtpBaslangic = new System.Windows.Forms.DateTimePicker();
            this.lblBaslangic = new System.Windows.Forms.Label();
            this.lblBitis = new System.Windows.Forms.Label();
            this.lblHakkimizda = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblBugun = new System.Windows.Forms.Label();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.lblTakvim = new System.Windows.Forms.Label();
            this.lblYardim = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGeri
            // 
            this.btnGeri.BackColor = System.Drawing.Color.SlateGray;
            this.btnGeri.ForeColor = System.Drawing.SystemColors.Control;
            this.btnGeri.Location = new System.Drawing.Point(317, 472);
            this.btnGeri.Name = "btnGeri";
            this.btnGeri.Size = new System.Drawing.Size(120, 38);
            this.btnGeri.TabIndex = 206;
            this.btnGeri.Text = "Geri";
            this.btnGeri.UseVisualStyleBackColor = false;
            this.btnGeri.Click += new System.EventHandler(this.btnGeri_Click);
            // 
            // btnIleri1
            // 
            this.btnIleri1.BackColor = System.Drawing.Color.SlateGray;
            this.btnIleri1.ForeColor = System.Drawing.SystemColors.Control;
            this.btnIleri1.Location = new System.Drawing.Point(729, 472);
            this.btnIleri1.Name = "btnIleri1";
            this.btnIleri1.Size = new System.Drawing.Size(120, 38);
            this.btnIleri1.TabIndex = 205;
            this.btnIleri1.Text = "İleri";
            this.btnIleri1.UseVisualStyleBackColor = false;
            this.btnIleri1.Click += new System.EventHandler(this.btnIleri1_Click);
            // 
            // btnIptal1
            // 
            this.btnIptal1.BackColor = System.Drawing.Color.SlateGray;
            this.btnIptal1.ForeColor = System.Drawing.SystemColors.Control;
            this.btnIptal1.Location = new System.Drawing.Point(519, 472);
            this.btnIptal1.Name = "btnIptal1";
            this.btnIptal1.Size = new System.Drawing.Size(120, 38);
            this.btnIptal1.TabIndex = 204;
            this.btnIptal1.Text = "İptal";
            this.btnIptal1.UseVisualStyleBackColor = false;
            this.btnIptal1.Click += new System.EventHandler(this.btnIptal1_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(655, 203);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 203;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Beige;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(489, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 29);
            this.label3.TabIndex = 202;
            this.label3.Text = "Sergi Tarihi Planı";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblSergiler
            // 
            this.lblSergiler.AutoSize = true;
            this.lblSergiler.BackColor = System.Drawing.Color.SlateGray;
            this.lblSergiler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSergiler.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSergiler.Location = new System.Drawing.Point(64, 352);
            this.lblSergiler.Name = "lblSergiler";
            this.lblSergiler.Size = new System.Drawing.Size(66, 18);
            this.lblSergiler.TabIndex = 201;
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
            this.lblMusteriBilgileri.Location = new System.Drawing.Point(38, 297);
            this.lblMusteriBilgileri.Name = "lblMusteriBilgileri";
            this.lblMusteriBilgileri.Size = new System.Drawing.Size(124, 18);
            this.lblMusteriBilgileri.TabIndex = 200;
            this.lblMusteriBilgileri.Text = "Müşteri Bilgileri";
            this.lblMusteriBilgileri.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMusteriBilgileri.Click += new System.EventHandler(this.lblMusteriBilgileri_Click);
            // 
            // lblRaporEkrani
            // 
            this.lblRaporEkrani.AutoSize = true;
            this.lblRaporEkrani.BackColor = System.Drawing.Color.SlateGray;
            this.lblRaporEkrani.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblRaporEkrani.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRaporEkrani.Location = new System.Drawing.Point(38, 409);
            this.lblRaporEkrani.Name = "lblRaporEkrani";
            this.lblRaporEkrani.Size = new System.Drawing.Size(107, 18);
            this.lblRaporEkrani.TabIndex = 199;
            this.lblRaporEkrani.Text = "Rapor Ekranı";
            this.lblRaporEkrani.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblRaporEkrani.Click += new System.EventHandler(this.lblRaporEkrani_Click);
            // 
            // lblCikisYap
            // 
            this.lblCikisYap.AutoSize = true;
            this.lblCikisYap.BackColor = System.Drawing.Color.SlateGray;
            this.lblCikisYap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCikisYap.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCikisYap.Location = new System.Drawing.Point(51, 471);
            this.lblCikisYap.Name = "lblCikisYap";
            this.lblCikisYap.Size = new System.Drawing.Size(79, 18);
            this.lblCikisYap.TabIndex = 198;
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
            this.lblSergiciTanimlama.Location = new System.Drawing.Point(25, 243);
            this.lblSergiciTanimlama.Name = "lblSergiciTanimlama";
            this.lblSergiciTanimlama.Size = new System.Drawing.Size(147, 18);
            this.lblSergiciTanimlama.TabIndex = 197;
            this.lblSergiciTanimlama.Text = "Sergici Tanımlama";
            this.lblSergiciTanimlama.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSergiciTanimlama.Click += new System.EventHandler(this.lblSergiciTanimlama_Click);
            // 
            // lblDashboard
            // 
            this.lblDashboard.AutoSize = true;
            this.lblDashboard.BackColor = System.Drawing.Color.SlateGray;
            this.lblDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDashboard.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDashboard.Location = new System.Drawing.Point(51, 191);
            this.lblDashboard.Name = "lblDashboard";
            this.lblDashboard.Size = new System.Drawing.Size(90, 18);
            this.lblDashboard.TabIndex = 196;
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
            this.lblGalericiAdi.Location = new System.Drawing.Point(51, 140);
            this.lblGalericiAdi.Name = "lblGalericiAdi";
            this.lblGalericiAdi.Size = new System.Drawing.Size(66, 18);
            this.lblGalericiAdi.TabIndex = 195;
            this.lblGalericiAdi.Text = "Profilim";
            this.lblGalericiAdi.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblGalericiAdi.Click += new System.EventHandler(this.lblGalericiAdi_Click);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.SlateGray;
            this.label18.Location = new System.Drawing.Point(-2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(201, 589);
            this.label18.TabIndex = 194;
            // 
            // dtpBitis
            // 
            this.dtpBitis.Location = new System.Drawing.Point(380, 333);
            this.dtpBitis.Name = "dtpBitis";
            this.dtpBitis.Size = new System.Drawing.Size(181, 22);
            this.dtpBitis.TabIndex = 193;
            // 
            // dtpBaslangic
            // 
            this.dtpBaslangic.Location = new System.Drawing.Point(380, 289);
            this.dtpBaslangic.Name = "dtpBaslangic";
            this.dtpBaslangic.Size = new System.Drawing.Size(181, 22);
            this.dtpBaslangic.TabIndex = 192;
            // 
            // lblBaslangic
            // 
            this.lblBaslangic.AutoSize = true;
            this.lblBaslangic.BackColor = System.Drawing.Color.Beige;
            this.lblBaslangic.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBaslangic.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblBaslangic.Location = new System.Drawing.Point(228, 289);
            this.lblBaslangic.Name = "lblBaslangic";
            this.lblBaslangic.Size = new System.Drawing.Size(144, 22);
            this.lblBaslangic.TabIndex = 191;
            this.lblBaslangic.Text = "Başlangıç Tarihi:";
            this.lblBaslangic.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBitis
            // 
            this.lblBitis.AutoSize = true;
            this.lblBitis.BackColor = System.Drawing.Color.Beige;
            this.lblBitis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBitis.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblBitis.Location = new System.Drawing.Point(228, 333);
            this.lblBitis.Name = "lblBitis";
            this.lblBitis.Size = new System.Drawing.Size(100, 22);
            this.lblBitis.TabIndex = 190;
            this.lblBitis.Text = "Bitiş Tarihi:";
            this.lblBitis.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblHakkimizda
            // 
            this.lblHakkimizda.BackColor = System.Drawing.SystemColors.Control;
            this.lblHakkimizda.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHakkimizda.Location = new System.Drawing.Point(192, 539);
            this.lblHakkimizda.Name = "lblHakkimizda";
            this.lblHakkimizda.Size = new System.Drawing.Size(808, 42);
            this.lblHakkimizda.TabIndex = 189;
            this.lblHakkimizda.Text = "Hakkımızda     ";
            this.lblHakkimizda.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHakkimizda.Click += new System.EventHandler(this.lblHakkimizda_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Beige;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(203, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(783, 451);
            this.label2.TabIndex = 188;
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(350, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 207;
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
            this.pictureBox2.TabIndex = 208;
            this.pictureBox2.TabStop = false;
            // 
            // lblBugun
            // 
            this.lblBugun.AutoSize = true;
            this.lblBugun.BackColor = System.Drawing.Color.Beige;
            this.lblBugun.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBugun.Location = new System.Drawing.Point(229, 243);
            this.lblBugun.Name = "lblBugun";
            this.lblBugun.Size = new System.Drawing.Size(68, 20);
            this.lblBugun.TabIndex = 209;
            this.lblBugun.Text = "Bugün:";
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.Location = new System.Drawing.Point(380, 245);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(180, 22);
            this.dateTimePicker3.TabIndex = 210;
            // 
            // lblTakvim
            // 
            this.lblTakvim.AutoSize = true;
            this.lblTakvim.BackColor = System.Drawing.Color.Beige;
            this.lblTakvim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTakvim.Location = new System.Drawing.Point(658, 172);
            this.lblTakvim.Name = "lblTakvim";
            this.lblTakvim.Size = new System.Drawing.Size(67, 18);
            this.lblTakvim.TabIndex = 211;
            this.lblTakvim.Text = "Takvim:";
            // 
            // lblYardim
            // 
            this.lblYardim.AutoSize = true;
            this.lblYardim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblYardim.Location = new System.Drawing.Point(655, 552);
            this.lblYardim.Name = "lblYardim";
            this.lblYardim.Size = new System.Drawing.Size(17, 18);
            this.lblYardim.TabIndex = 212;
            this.lblYardim.Text = "?";
            this.lblYardim.Click += new System.EventHandler(this.lblYardim_Click);
            // 
            // GalericiSergiler2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.lblYardim);
            this.Controls.Add(this.lblTakvim);
            this.Controls.Add(this.dateTimePicker3);
            this.Controls.Add(this.lblBugun);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnGeri);
            this.Controls.Add(this.btnIleri1);
            this.Controls.Add(this.btnIptal1);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblSergiler);
            this.Controls.Add(this.lblMusteriBilgileri);
            this.Controls.Add(this.lblRaporEkrani);
            this.Controls.Add(this.lblCikisYap);
            this.Controls.Add(this.lblSergiciTanimlama);
            this.Controls.Add(this.lblDashboard);
            this.Controls.Add(this.lblGalericiAdi);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.dtpBitis);
            this.Controls.Add(this.dtpBaslangic);
            this.Controls.Add(this.lblBaslangic);
            this.Controls.Add(this.lblBitis);
            this.Controls.Add(this.lblHakkimizda);
            this.Controls.Add(this.label2);
            this.Name = "GalericiSergiler2";
            this.Text = "Galerici Sergi Tarihi";
            this.Load += new System.EventHandler(this.GalericiSergiler2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion// Sonradan ekledim
        private DateTimePicker dateBaslangic;
        private DateTimePicker dateBitis;
        private Button btnIleri;
        private Button btnIptal;
        private Label lblBaslangicTarih;
        private Label lblBitisTarih;

        private System.Windows.Forms.Button btnGeri;
        private System.Windows.Forms.Button btnIleri1;
        private System.Windows.Forms.Button btnIptal1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSergiler;
        private System.Windows.Forms.Label lblMusteriBilgileri;
        private System.Windows.Forms.Label lblRaporEkrani;
        private System.Windows.Forms.Label lblCikisYap;
        private System.Windows.Forms.Label lblSergiciTanimlama;
        private System.Windows.Forms.Label lblDashboard;
        private System.Windows.Forms.Label lblGalericiAdi;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DateTimePicker dtpBitis;
        private System.Windows.Forms.DateTimePicker dtpBaslangic;
        private System.Windows.Forms.Label lblBaslangic;
        private System.Windows.Forms.Label lblBitis;
        private System.Windows.Forms.Label lblHakkimizda;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Label lblBugun;
        private DateTimePicker dateTimePicker3;
        private Label lblTakvim;
        private Label lblYardim;
    }
}