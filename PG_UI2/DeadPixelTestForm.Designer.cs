namespace PG_UI2
{
    partial class DeadPixelTestForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.darkpixmin = new System.Windows.Forms.TextBox();
            this.darkpixmax = new System.Windows.Forms.TextBox();
            this.darkrowmax = new System.Windows.Forms.TextBox();
            this.darkrowmin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.darkcolmax = new System.Windows.Forms.TextBox();
            this.darkcolmin = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.darkmeanmax = new System.Windows.Forms.TextBox();
            this.darkmeanmin = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.darkvarmax = new System.Windows.Forms.TextBox();
            this.darkvarmin = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.satcol = new System.Windows.Forms.TextBox();
            this.satrow = new System.Windows.Forms.TextBox();
            this.satpix = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.satvarmax = new System.Windows.Forms.TextBox();
            this.satvarmin = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.satmeanmax = new System.Windows.Forms.TextBox();
            this.satmeanmin = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.satsnrmax = new System.Windows.Forms.TextBox();
            this.satsnrmin = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lightness = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.imagenumber = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.LEDToggle = new System.Windows.Forms.CheckBox();
            this.LEDonTest = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.darkStartButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SaturationStartButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Dead Pixel Limit";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(150, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "Min";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(150, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "Max";
            // 
            // darkpixmin
            // 
            this.darkpixmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkpixmin.Location = new System.Drawing.Point(189, 15);
            this.darkpixmin.Name = "darkpixmin";
            this.darkpixmin.Size = new System.Drawing.Size(54, 22);
            this.darkpixmin.TabIndex = 5;
            // 
            // darkpixmax
            // 
            this.darkpixmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkpixmax.Location = new System.Drawing.Point(189, 44);
            this.darkpixmax.Name = "darkpixmax";
            this.darkpixmax.Size = new System.Drawing.Size(54, 22);
            this.darkpixmax.TabIndex = 6;
            // 
            // darkrowmax
            // 
            this.darkrowmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkrowmax.Location = new System.Drawing.Point(189, 103);
            this.darkrowmax.Name = "darkrowmax";
            this.darkrowmax.Size = new System.Drawing.Size(54, 22);
            this.darkrowmax.TabIndex = 11;
            // 
            // darkrowmin
            // 
            this.darkrowmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkrowmin.Location = new System.Drawing.Point(189, 74);
            this.darkrowmin.Name = "darkrowmin";
            this.darkrowmin.Size = new System.Drawing.Size(54, 22);
            this.darkrowmin.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(150, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "Max";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(150, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 14);
            this.label5.TabIndex = 8;
            this.label5.Text = "Min";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 14);
            this.label6.TabIndex = 7;
            this.label6.Text = "Dead Row Limit";
            // 
            // darkcolmax
            // 
            this.darkcolmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkcolmax.Location = new System.Drawing.Point(189, 162);
            this.darkcolmax.Name = "darkcolmax";
            this.darkcolmax.Size = new System.Drawing.Size(54, 22);
            this.darkcolmax.TabIndex = 16;
            // 
            // darkcolmin
            // 
            this.darkcolmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkcolmin.Location = new System.Drawing.Point(189, 133);
            this.darkcolmin.Name = "darkcolmin";
            this.darkcolmin.Size = new System.Drawing.Size(54, 22);
            this.darkcolmin.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(150, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 14);
            this.label7.TabIndex = 14;
            this.label7.Text = "Max";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(150, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 14);
            this.label8.TabIndex = 13;
            this.label8.Text = "Min";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 136);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 14);
            this.label9.TabIndex = 12;
            this.label9.Text = "Dead Column Limit";
            // 
            // darkmeanmax
            // 
            this.darkmeanmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkmeanmax.Location = new System.Drawing.Point(189, 222);
            this.darkmeanmax.Name = "darkmeanmax";
            this.darkmeanmax.Size = new System.Drawing.Size(54, 22);
            this.darkmeanmax.TabIndex = 21;
            // 
            // darkmeanmin
            // 
            this.darkmeanmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkmeanmin.Location = new System.Drawing.Point(189, 193);
            this.darkmeanmin.Name = "darkmeanmin";
            this.darkmeanmin.Size = new System.Drawing.Size(54, 22);
            this.darkmeanmin.TabIndex = 20;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(150, 225);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 14);
            this.label10.TabIndex = 19;
            this.label10.Text = "Max";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(150, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 14);
            this.label11.TabIndex = 18;
            this.label11.Text = "Min";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(13, 196);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 14);
            this.label12.TabIndex = 17;
            this.label12.Text = "Dead Mean Limit";
            // 
            // darkvarmax
            // 
            this.darkvarmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkvarmax.Location = new System.Drawing.Point(189, 281);
            this.darkvarmax.Name = "darkvarmax";
            this.darkvarmax.Size = new System.Drawing.Size(54, 22);
            this.darkvarmax.TabIndex = 26;
            // 
            // darkvarmin
            // 
            this.darkvarmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkvarmin.Location = new System.Drawing.Point(189, 252);
            this.darkvarmin.Name = "darkvarmin";
            this.darkvarmin.Size = new System.Drawing.Size(54, 22);
            this.darkvarmin.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(150, 284);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 14);
            this.label13.TabIndex = 24;
            this.label13.Text = "Max";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(150, 255);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 14);
            this.label14.TabIndex = 23;
            this.label14.Text = "Min";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(13, 255);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(116, 14);
            this.label15.TabIndex = 22;
            this.label15.Text = "Dead Variance Limit";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(6, 255);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(116, 14);
            this.label16.TabIndex = 31;
            this.label16.Text = "Dead Variance Limit";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(6, 196);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 14);
            this.label17.TabIndex = 30;
            this.label17.Text = "Dead Mean Limit";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(6, 136);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(155, 14);
            this.label18.TabIndex = 29;
            this.label18.Text = "Dead Column Limit            ± ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(6, 77);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(152, 14);
            this.label19.TabIndex = 28;
            this.label19.Text = "Dead Row Limit                 ± ";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(6, 18);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(149, 14);
            this.label20.TabIndex = 27;
            this.label20.Text = "Dead Pixel Limit                ±";
            // 
            // satcol
            // 
            this.satcol.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satcol.Location = new System.Drawing.Point(175, 133);
            this.satcol.Name = "satcol";
            this.satcol.Size = new System.Drawing.Size(54, 22);
            this.satcol.TabIndex = 34;
            // 
            // satrow
            // 
            this.satrow.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satrow.Location = new System.Drawing.Point(175, 74);
            this.satrow.Name = "satrow";
            this.satrow.Size = new System.Drawing.Size(54, 22);
            this.satrow.TabIndex = 35;
            // 
            // satpix
            // 
            this.satpix.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satpix.Location = new System.Drawing.Point(175, 15);
            this.satpix.Name = "satpix";
            this.satpix.Size = new System.Drawing.Size(54, 22);
            this.satpix.TabIndex = 36;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(235, 18);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(16, 14);
            this.label21.TabIndex = 37;
            this.label21.Text = "%";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(235, 136);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(16, 14);
            this.label22.TabIndex = 38;
            this.label22.Text = "%";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(235, 77);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(16, 14);
            this.label23.TabIndex = 39;
            this.label23.Text = "%";
            // 
            // satvarmax
            // 
            this.satvarmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satvarmax.Location = new System.Drawing.Point(175, 281);
            this.satvarmax.Name = "satvarmax";
            this.satvarmax.Size = new System.Drawing.Size(54, 22);
            this.satvarmax.TabIndex = 47;
            // 
            // satvarmin
            // 
            this.satvarmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satvarmin.Location = new System.Drawing.Point(175, 252);
            this.satvarmin.Name = "satvarmin";
            this.satvarmin.Size = new System.Drawing.Size(54, 22);
            this.satvarmin.TabIndex = 46;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(136, 284);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 14);
            this.label24.TabIndex = 45;
            this.label24.Text = "Max";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(136, 255);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(28, 14);
            this.label25.TabIndex = 44;
            this.label25.Text = "Min";
            // 
            // satmeanmax
            // 
            this.satmeanmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satmeanmax.Location = new System.Drawing.Point(175, 222);
            this.satmeanmax.Name = "satmeanmax";
            this.satmeanmax.Size = new System.Drawing.Size(54, 22);
            this.satmeanmax.TabIndex = 43;
            // 
            // satmeanmin
            // 
            this.satmeanmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satmeanmin.Location = new System.Drawing.Point(175, 193);
            this.satmeanmin.Name = "satmeanmin";
            this.satmeanmin.Size = new System.Drawing.Size(54, 22);
            this.satmeanmin.TabIndex = 42;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(136, 225);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(29, 14);
            this.label26.TabIndex = 41;
            this.label26.Text = "Max";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(136, 196);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(28, 14);
            this.label27.TabIndex = 40;
            this.label27.Text = "Min";
            // 
            // satsnrmax
            // 
            this.satsnrmax.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satsnrmax.Location = new System.Drawing.Point(175, 340);
            this.satsnrmax.Name = "satsnrmax";
            this.satsnrmax.Size = new System.Drawing.Size(54, 22);
            this.satsnrmax.TabIndex = 52;
            // 
            // satsnrmin
            // 
            this.satsnrmin.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.satsnrmin.Location = new System.Drawing.Point(175, 311);
            this.satsnrmin.Name = "satsnrmin";
            this.satsnrmin.Size = new System.Drawing.Size(54, 22);
            this.satsnrmin.TabIndex = 51;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(136, 343);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(29, 14);
            this.label28.TabIndex = 50;
            this.label28.Text = "Max";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(136, 314);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(28, 14);
            this.label29.TabIndex = 49;
            this.label29.Text = "Min";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(6, 314);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(60, 14);
            this.label30.TabIndex = 48;
            this.label30.Text = "Dead SNR";
            // 
            // lightness
            // 
            this.lightness.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lightness.Location = new System.Drawing.Point(104, 47);
            this.lightness.Name = "lightness";
            this.lightness.Size = new System.Drawing.Size(54, 22);
            this.lightness.TabIndex = 53;
            this.lightness.Text = "10";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(11, 52);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(59, 14);
            this.label31.TabIndex = 54;
            this.label31.Text = "Lightness";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(11, 19);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(87, 14);
            this.label32.TabIndex = 55;
            this.label32.Text = "Image Number";
            // 
            // imagenumber
            // 
            this.imagenumber.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imagenumber.Location = new System.Drawing.Point(104, 15);
            this.imagenumber.Name = "imagenumber";
            this.imagenumber.Size = new System.Drawing.Size(54, 22);
            this.imagenumber.TabIndex = 56;
            this.imagenumber.Text = "8";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(77, 50);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(18, 14);
            this.label33.TabIndex = 57;
            this.label33.Text = "0x";
            // 
            // LEDToggle
            // 
            this.LEDToggle.AutoSize = true;
            this.LEDToggle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LEDToggle.Location = new System.Drawing.Point(12, 78);
            this.LEDToggle.Name = "LEDToggle";
            this.LEDToggle.Size = new System.Drawing.Size(83, 18);
            this.LEDToggle.TabIndex = 58;
            this.LEDToggle.Text = "LED Toggle";
            this.LEDToggle.UseVisualStyleBackColor = true;
            this.LEDToggle.CheckedChanged += new System.EventHandler(this.LEDToggle_CheckedChanged);
            // 
            // LEDonTest
            // 
            this.LEDonTest.AutoSize = true;
            this.LEDonTest.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LEDonTest.Location = new System.Drawing.Point(12, 107);
            this.LEDonTest.Name = "LEDonTest";
            this.LEDonTest.Size = new System.Drawing.Size(63, 18);
            this.LEDonTest.TabIndex = 59;
            this.LEDonTest.Text = "LED On";
            this.LEDonTest.UseVisualStyleBackColor = true;
            this.LEDonTest.CheckedChanged += new System.EventHandler(this.LEDonTest_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.darkStartButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.darkpixmin);
            this.groupBox1.Controls.Add(this.darkpixmax);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.darkrowmin);
            this.groupBox1.Controls.Add(this.darkrowmax);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.darkcolmin);
            this.groupBox1.Controls.Add(this.darkcolmax);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.darkmeanmin);
            this.groupBox1.Controls.Add(this.darkmeanmax);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.darkvarmin);
            this.groupBox1.Controls.Add(this.darkvarmax);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 418);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dark Test";
            // 
            // darkStartButton
            // 
            this.darkStartButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkStartButton.Location = new System.Drawing.Point(16, 314);
            this.darkStartButton.Name = "darkStartButton";
            this.darkStartButton.Size = new System.Drawing.Size(75, 28);
            this.darkStartButton.TabIndex = 58;
            this.darkStartButton.Text = "Start";
            this.darkStartButton.UseVisualStyleBackColor = true;
            this.darkStartButton.Click += new System.EventHandler(this.darkStartButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SaturationStartButton);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.satcol);
            this.groupBox2.Controls.Add(this.satrow);
            this.groupBox2.Controls.Add(this.satpix);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.satsnrmax);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.satsnrmin);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label28);
            this.groupBox2.Controls.Add(this.label27);
            this.groupBox2.Controls.Add(this.label29);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.satmeanmin);
            this.groupBox2.Controls.Add(this.satvarmax);
            this.groupBox2.Controls.Add(this.satmeanmax);
            this.groupBox2.Controls.Add(this.satvarmin);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(286, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(261, 418);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Saturation Test";
            // 
            // SaturationStartButton
            // 
            this.SaturationStartButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaturationStartButton.Location = new System.Drawing.Point(20, 370);
            this.SaturationStartButton.Name = "SaturationStartButton";
            this.SaturationStartButton.Size = new System.Drawing.Size(75, 28);
            this.SaturationStartButton.TabIndex = 62;
            this.SaturationStartButton.Text = "Start";
            this.SaturationStartButton.UseVisualStyleBackColor = true;
            this.SaturationStartButton.Click += new System.EventHandler(this.SaturationStartButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.imagenumber);
            this.groupBox3.Controls.Add(this.label32);
            this.groupBox3.Controls.Add(this.label31);
            this.groupBox3.Controls.Add(this.LEDonTest);
            this.groupBox3.Controls.Add(this.lightness);
            this.groupBox3.Controls.Add(this.LEDToggle);
            this.groupBox3.Controls.Add(this.label33);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(553, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(172, 418);
            this.groupBox3.TabIndex = 62;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // DeadPixelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 451);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "DeadPixelTestForm";
            this.ShowIcon = false;
            this.Text = "Dead Pixel Test";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox darkpixmin;
        private System.Windows.Forms.TextBox darkpixmax;
        private System.Windows.Forms.TextBox darkrowmax;
        private System.Windows.Forms.TextBox darkrowmin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox darkcolmax;
        private System.Windows.Forms.TextBox darkcolmin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox darkmeanmax;
        private System.Windows.Forms.TextBox darkmeanmin;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox darkvarmax;
        private System.Windows.Forms.TextBox darkvarmin;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox satcol;
        private System.Windows.Forms.TextBox satrow;
        private System.Windows.Forms.TextBox satpix;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox satvarmax;
        private System.Windows.Forms.TextBox satvarmin;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox satmeanmax;
        private System.Windows.Forms.TextBox satmeanmin;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox satsnrmax;
        private System.Windows.Forms.TextBox satsnrmin;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox lightness;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox imagenumber;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox LEDToggle;
        private System.Windows.Forms.CheckBox LEDonTest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button darkStartButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button SaturationStartButton;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}