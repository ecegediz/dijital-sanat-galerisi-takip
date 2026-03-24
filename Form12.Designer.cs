namespace dijitalsanatgalerisi
{
    partial class Form12
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
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Galerici = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eposta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Şifre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Durum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fiyat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.SteelBlue;
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(501, 491);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 38);
            this.button2.TabIndex = 64;
            this.button2.Text = "PDF/Excel Oluştur";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Beige;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(527, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 25);
            this.label3.TabIndex = 63;
            this.label3.Text = "Müşteri Bilgileri";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
            this.dataGridView1.Location = new System.Drawing.Point(246, 161);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(723, 314);
            this.dataGridView1.TabIndex = 62;
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
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label9.Location = new System.Drawing.Point(206, 541);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(808, 45);
            this.label9.TabIndex = 61;
            this.label9.Text = "Hakkımızda     ?";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(206, -6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(808, 108);
            this.label1.TabIndex = 60;
            this.label1.Text = "ART FLOW Dijital Sanat Galerisi ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.SlateGray;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label15.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label15.Location = new System.Drawing.Point(54, 344);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 18);
            this.label15.TabIndex = 141;
            this.label15.Text = "Sergiler";
            this.label15.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.SlateGray;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label11.Location = new System.Drawing.Point(37, 304);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(124, 18);
            this.label11.TabIndex = 140;
            this.label11.Text = "Müşteri Bilgileri";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.SlateGray;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label12.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label12.Location = new System.Drawing.Point(41, 385);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(107, 18);
            this.label12.TabIndex = 139;
            this.label12.Text = "Rapor Ekranı";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.SlateGray;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label16.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label16.Location = new System.Drawing.Point(54, 424);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 18);
            this.label16.TabIndex = 138;
            this.label16.Text = "Çıkış Yap";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.SlateGray;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label14.Location = new System.Drawing.Point(25, 262);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(147, 18);
            this.label14.TabIndex = 137;
            this.label14.Text = "Sergici Tanımlama";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.SlateGray;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label13.Location = new System.Drawing.Point(54, 221);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 18);
            this.label13.TabIndex = 136;
            this.label13.Text = "Dashboard";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.SlateGray;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.Location = new System.Drawing.Point(54, 180);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(94, 18);
            this.label17.TabIndex = 135;
            this.label17.Text = "Galerici Adı";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.SlateGray;
            this.label18.Location = new System.Drawing.Point(-1, -6);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(201, 592);
            this.label18.TabIndex = 134;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Beige;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(210, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(790, 439);
            this.label2.TabIndex = 142;
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form12
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 580);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "Form12";
            this.Text = "Form12";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Galerici;
        private System.Windows.Forms.DataGridViewTextBoxColumn eposta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Şifre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Durum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fiyat;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label2;
    }
}