namespace PG_UI2
{
    partial class FingerID
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
            this.EnrollTimeTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.EnrollButton = new System.Windows.Forms.Button();
            this.MatchButton = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.ImportButton = new System.Windows.Forms.Button();
            this.RunDatabaseButton = new System.Windows.Forms.Button();
            this.calibration_but = new System.Windows.Forms.Button();
            this.capture_but = new System.Windows.Forms.Button();
            this.Delete_but = new System.Windows.Forms.Button();
            this.touch_mode_but = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.verify_but = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.condition_combobox = new System.Windows.Forms.ComboBox();
            this.fingernum_textbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBox_original = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.set_but = new System.Windows.Forms.Button();
            this.Load_but = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Offset_textbox = new System.Windows.Forms.TextBox();
            this.Gain_textbox = new System.Windows.Forms.TextBox();
            this.Intg_textbox = new System.Windows.Forms.TextBox();
            this.IspSettingGroupBox = new System.Windows.Forms.GroupBox();
            this.PbTest_Button = new System.Windows.Forms.Button();
            this.Normalize_checkbox = new System.Windows.Forms.CheckBox();
            this.denoise_checkbox = new System.Windows.Forms.CheckBox();
            this.Gain_error_checkbox = new System.Windows.Forms.CheckBox();
            this.richconsole = new System.Windows.Forms.RichTextBox();
            this.CaptureButton = new System.Windows.Forms.Button();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.delayCheckBox = new System.Windows.Forms.CheckBox();
            this.delayTextBox = new System.Windows.Forms.TextBox();
            this.captureNumTextBox = new System.Windows.Forms.TextBox();
            this.CaptureNumCheckBox = new System.Windows.Forms.CheckBox();
            this.summingStartTextBox = new System.Windows.Forms.TextBox();
            this.summingStartCheckBox = new System.Windows.Forms.CheckBox();
            this.summingEndTextBox = new System.Windows.Forms.TextBox();
            this.summingEndCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_original)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.IspSettingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // EnrollTimeTextBox
            // 
            this.EnrollTimeTextBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnrollTimeTextBox.Location = new System.Drawing.Point(142, 18);
            this.EnrollTimeTextBox.Name = "EnrollTimeTextBox";
            this.EnrollTimeTextBox.Size = new System.Drawing.Size(38, 23);
            this.EnrollTimeTextBox.TabIndex = 0;
            this.EnrollTimeTextBox.Text = "15";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enroll/Verify Time :";
            // 
            // EnrollButton
            // 
            this.EnrollButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnrollButton.Location = new System.Drawing.Point(6, 26);
            this.EnrollButton.Name = "EnrollButton";
            this.EnrollButton.Size = new System.Drawing.Size(75, 23);
            this.EnrollButton.TabIndex = 3;
            this.EnrollButton.Text = "Enroll";
            this.EnrollButton.UseVisualStyleBackColor = true;
            this.EnrollButton.Click += new System.EventHandler(this.EnrollButton_Click);
            // 
            // MatchButton
            // 
            this.MatchButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MatchButton.Location = new System.Drawing.Point(205, 26);
            this.MatchButton.Name = "MatchButton";
            this.MatchButton.Size = new System.Drawing.Size(90, 23);
            this.MatchButton.TabIndex = 4;
            this.MatchButton.Text = "Match";
            this.MatchButton.UseVisualStyleBackColor = true;
            this.MatchButton.Visible = false;
            this.MatchButton.Click += new System.EventHandler(this.MatchButton_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(754, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(190, 220);
            this.listBox1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(514, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(234, 278);
            this.panel1.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(243, 272);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 19);
            this.label2.TabIndex = 30;
            this.label2.Text = "User Name :";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTextBox.Location = new System.Drawing.Point(96, 58);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(84, 23);
            this.NameTextBox.TabIndex = 29;
            this.NameTextBox.Text = "User01";
            // 
            // ImportButton
            // 
            this.ImportButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportButton.Location = new System.Drawing.Point(205, 56);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(90, 23);
            this.ImportButton.TabIndex = 31;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Visible = false;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // RunDatabaseButton
            // 
            this.RunDatabaseButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunDatabaseButton.Location = new System.Drawing.Point(97, 56);
            this.RunDatabaseButton.Name = "RunDatabaseButton";
            this.RunDatabaseButton.Size = new System.Drawing.Size(100, 23);
            this.RunDatabaseButton.TabIndex = 32;
            this.RunDatabaseButton.Text = "Run Database";
            this.RunDatabaseButton.UseVisualStyleBackColor = true;
            this.RunDatabaseButton.Visible = false;
            this.RunDatabaseButton.Click += new System.EventHandler(this.RunDatabaseButton_Click);
            // 
            // calibration_but
            // 
            this.calibration_but.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calibration_but.Location = new System.Drawing.Point(121, 579);
            this.calibration_but.Name = "calibration_but";
            this.calibration_but.Size = new System.Drawing.Size(88, 23);
            this.calibration_but.TabIndex = 33;
            this.calibration_but.Text = "Calibraiton";
            this.calibration_but.UseVisualStyleBackColor = true;
            this.calibration_but.Visible = false;
            this.calibration_but.Click += new System.EventHandler(this.calibration_but_Click);
            // 
            // capture_but
            // 
            this.capture_but.Enabled = false;
            this.capture_but.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.capture_but.ForeColor = System.Drawing.Color.Red;
            this.capture_but.Location = new System.Drawing.Point(97, 26);
            this.capture_but.Name = "capture_but";
            this.capture_but.Size = new System.Drawing.Size(75, 23);
            this.capture_but.TabIndex = 34;
            this.capture_but.Text = "Capture";
            this.capture_but.UseVisualStyleBackColor = true;
            this.capture_but.Click += new System.EventHandler(this.capture_but_Click);
            // 
            // Delete_but
            // 
            this.Delete_but.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Delete_but.Location = new System.Drawing.Point(6, 56);
            this.Delete_but.Name = "Delete_but";
            this.Delete_but.Size = new System.Drawing.Size(75, 23);
            this.Delete_but.TabIndex = 35;
            this.Delete_but.Text = "Delete";
            this.Delete_but.UseVisualStyleBackColor = true;
            this.Delete_but.Click += new System.EventHandler(this.Delete_but_Click);
            // 
            // touch_mode_but
            // 
            this.touch_mode_but.Enabled = false;
            this.touch_mode_but.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.touch_mode_but.Location = new System.Drawing.Point(6, 26);
            this.touch_mode_but.Name = "touch_mode_but";
            this.touch_mode_but.Size = new System.Drawing.Size(87, 55);
            this.touch_mode_but.TabIndex = 36;
            this.touch_mode_but.Text = "Touch Enroll";
            this.touch_mode_but.UseVisualStyleBackColor = true;
            this.touch_mode_but.Click += new System.EventHandler(this.touch_mode_but_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EnrollButton);
            this.groupBox1.Controls.Add(this.MatchButton);
            this.groupBox1.Controls.Add(this.ImportButton);
            this.groupBox1.Controls.Add(this.Delete_but);
            this.groupBox1.Controls.Add(this.capture_but);
            this.groupBox1.Controls.Add(this.RunDatabaseButton);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 452);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 85);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Capture Area";
            this.groupBox1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.verify_but);
            this.groupBox2.Controls.Add(this.touch_mode_but);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(509, 320);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 85);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PB Reference Test";
            // 
            // verify_but
            // 
            this.verify_but.Enabled = false;
            this.verify_but.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verify_but.Location = new System.Drawing.Point(100, 26);
            this.verify_but.Name = "verify_but";
            this.verify_but.Size = new System.Drawing.Size(84, 55);
            this.verify_but.TabIndex = 37;
            this.verify_but.Text = "Verify";
            this.verify_but.UseVisualStyleBackColor = true;
            this.verify_but.Click += new System.EventHandler(this.verify_but_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.condition_combobox);
            this.panel2.Controls.Add(this.fingernum_textbox);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.EnrollTimeTextBox);
            this.panel2.Controls.Add(this.NameTextBox);
            this.panel2.Location = new System.Drawing.Point(754, 245);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(190, 187);
            this.panel2.TabIndex = 39;
            // 
            // condition_combobox
            // 
            this.condition_combobox.FormattingEnabled = true;
            this.condition_combobox.Items.AddRange(new object[] {
            "st",
            "45d",
            "90d"});
            this.condition_combobox.Location = new System.Drawing.Point(98, 139);
            this.condition_combobox.Name = "condition_combobox";
            this.condition_combobox.Size = new System.Drawing.Size(84, 20);
            this.condition_combobox.TabIndex = 34;
            // 
            // fingernum_textbox
            // 
            this.fingernum_textbox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fingernum_textbox.Location = new System.Drawing.Point(132, 97);
            this.fingernum_textbox.Name = "fingernum_textbox";
            this.fingernum_textbox.Size = new System.Drawing.Size(51, 22);
            this.fingernum_textbox.TabIndex = 33;
            this.fingernum_textbox.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(4, 137);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 19);
            this.label8.TabIndex = 32;
            this.label8.Text = "Condition :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 19);
            this.label7.TabIndex = 31;
            this.label7.Text = "Finger Number :";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pictureBox_original);
            this.panel3.Location = new System.Drawing.Point(272, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(236, 279);
            this.panel3.TabIndex = 40;
            // 
            // pictureBox_original
            // 
            this.pictureBox_original.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_original.Location = new System.Drawing.Point(3, 4);
            this.pictureBox_original.Name = "pictureBox_original";
            this.pictureBox_original.Size = new System.Drawing.Size(230, 272);
            this.pictureBox_original.TabIndex = 0;
            this.pictureBox_original.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.set_but);
            this.groupBox3.Controls.Add(this.Load_but);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Offset_textbox);
            this.groupBox3.Controls.Add(this.Gain_textbox);
            this.groupBox3.Controls.Add(this.Intg_textbox);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 304);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(254, 132);
            this.groupBox3.TabIndex = 41;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sensor Register";
            this.groupBox3.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(102, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 19);
            this.label6.TabIndex = 8;
            this.label6.Text = "16 /";
            // 
            // set_but
            // 
            this.set_but.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.set_but.Location = new System.Drawing.Point(199, 96);
            this.set_but.Name = "set_but";
            this.set_but.Size = new System.Drawing.Size(49, 23);
            this.set_but.TabIndex = 7;
            this.set_but.Text = "Set";
            this.set_but.UseVisualStyleBackColor = true;
            this.set_but.Click += new System.EventHandler(this.set_but_Click);
            // 
            // Load_but
            // 
            this.Load_but.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Load_but.Location = new System.Drawing.Point(198, 26);
            this.Load_but.Name = "Load_but";
            this.Load_but.Size = new System.Drawing.Size(50, 23);
            this.Load_but.TabIndex = 6;
            this.Load_but.Text = "Load";
            this.Load_but.UseVisualStyleBackColor = true;
            this.Load_but.Click += new System.EventHandler(this.Load_but_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label5.Location = new System.Drawing.Point(10, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 19);
            this.label5.TabIndex = 5;
            this.label5.Text = "Offset_12:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label4.Location = new System.Drawing.Point(10, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 19);
            this.label4.TabIndex = 4;
            this.label4.Text = "Gain:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(10, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "Intg:";
            // 
            // Offset_textbox
            // 
            this.Offset_textbox.Location = new System.Drawing.Point(92, 95);
            this.Offset_textbox.Name = "Offset_textbox";
            this.Offset_textbox.Size = new System.Drawing.Size(100, 27);
            this.Offset_textbox.TabIndex = 2;
            // 
            // Gain_textbox
            // 
            this.Gain_textbox.Location = new System.Drawing.Point(144, 60);
            this.Gain_textbox.Name = "Gain_textbox";
            this.Gain_textbox.Size = new System.Drawing.Size(48, 27);
            this.Gain_textbox.TabIndex = 1;
            // 
            // Intg_textbox
            // 
            this.Intg_textbox.Location = new System.Drawing.Point(92, 26);
            this.Intg_textbox.Name = "Intg_textbox";
            this.Intg_textbox.Size = new System.Drawing.Size(100, 27);
            this.Intg_textbox.TabIndex = 0;
            // 
            // IspSettingGroupBox
            // 
            this.IspSettingGroupBox.Controls.Add(this.summingEndTextBox);
            this.IspSettingGroupBox.Controls.Add(this.summingEndCheckBox);
            this.IspSettingGroupBox.Controls.Add(this.summingStartTextBox);
            this.IspSettingGroupBox.Controls.Add(this.summingStartCheckBox);
            this.IspSettingGroupBox.Controls.Add(this.captureNumTextBox);
            this.IspSettingGroupBox.Controls.Add(this.CaptureNumCheckBox);
            this.IspSettingGroupBox.Controls.Add(this.delayTextBox);
            this.IspSettingGroupBox.Controls.Add(this.delayCheckBox);
            this.IspSettingGroupBox.Controls.Add(this.Normalize_checkbox);
            this.IspSettingGroupBox.Controls.Add(this.denoise_checkbox);
            this.IspSettingGroupBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IspSettingGroupBox.Location = new System.Drawing.Point(344, 438);
            this.IspSettingGroupBox.Name = "IspSettingGroupBox";
            this.IspSettingGroupBox.Size = new System.Drawing.Size(355, 164);
            this.IspSettingGroupBox.TabIndex = 42;
            this.IspSettingGroupBox.TabStop = false;
            this.IspSettingGroupBox.Text = "ISP Setting";
            // 
            // PbTest_Button
            // 
            this.PbTest_Button.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PbTest_Button.Location = new System.Drawing.Point(18, 579);
            this.PbTest_Button.Name = "PbTest_Button";
            this.PbTest_Button.Size = new System.Drawing.Size(88, 23);
            this.PbTest_Button.TabIndex = 35;
            this.PbTest_Button.Text = "PbTest";
            this.PbTest_Button.UseVisualStyleBackColor = true;
            this.PbTest_Button.Visible = false;
            this.PbTest_Button.Click += new System.EventHandler(this.PbTest_Button_Click);
            // 
            // Normalize_checkbox
            // 
            this.Normalize_checkbox.AutoSize = true;
            this.Normalize_checkbox.Checked = true;
            this.Normalize_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Normalize_checkbox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Normalize_checkbox.ForeColor = System.Drawing.Color.Green;
            this.Normalize_checkbox.Location = new System.Drawing.Point(256, 24);
            this.Normalize_checkbox.Name = "Normalize_checkbox";
            this.Normalize_checkbox.Size = new System.Drawing.Size(93, 23);
            this.Normalize_checkbox.TabIndex = 3;
            this.Normalize_checkbox.Text = "Normalize";
            this.Normalize_checkbox.UseVisualStyleBackColor = true;
            // 
            // denoise_checkbox
            // 
            this.denoise_checkbox.AutoSize = true;
            this.denoise_checkbox.Checked = true;
            this.denoise_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.denoise_checkbox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.denoise_checkbox.ForeColor = System.Drawing.Color.Green;
            this.denoise_checkbox.Location = new System.Drawing.Point(256, 53);
            this.denoise_checkbox.Name = "denoise_checkbox";
            this.denoise_checkbox.Size = new System.Drawing.Size(86, 23);
            this.denoise_checkbox.TabIndex = 2;
            this.denoise_checkbox.Text = "De-noise";
            this.denoise_checkbox.UseVisualStyleBackColor = true;
            // 
            // Gain_error_checkbox
            // 
            this.Gain_error_checkbox.AutoSize = true;
            this.Gain_error_checkbox.Checked = true;
            this.Gain_error_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Gain_error_checkbox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Gain_error_checkbox.ForeColor = System.Drawing.Color.Green;
            this.Gain_error_checkbox.Location = new System.Drawing.Point(18, 550);
            this.Gain_error_checkbox.Name = "Gain_error_checkbox";
            this.Gain_error_checkbox.Size = new System.Drawing.Size(93, 23);
            this.Gain_error_checkbox.TabIndex = 1;
            this.Gain_error_checkbox.Text = "Gain Error";
            this.Gain_error_checkbox.UseVisualStyleBackColor = true;
            this.Gain_error_checkbox.Visible = false;
            // 
            // richconsole
            // 
            this.richconsole.BackColor = System.Drawing.SystemColors.InfoText;
            this.richconsole.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richconsole.ForeColor = System.Drawing.Color.Lime;
            this.richconsole.Location = new System.Drawing.Point(12, 20);
            this.richconsole.Name = "richconsole";
            this.richconsole.Size = new System.Drawing.Size(246, 278);
            this.richconsole.TabIndex = 43;
            this.richconsole.Text = "";
            this.richconsole.TextChanged += new System.EventHandler(this.richconsole_TextChanged);
            // 
            // CaptureButton
            // 
            this.CaptureButton.Enabled = false;
            this.CaptureButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CaptureButton.ForeColor = System.Drawing.Color.Red;
            this.CaptureButton.Location = new System.Drawing.Point(425, 345);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(75, 55);
            this.CaptureButton.TabIndex = 36;
            this.CaptureButton.Text = "Capture";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.capture_but_Click);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ConnectButton.Location = new System.Drawing.Point(344, 345);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 55);
            this.ConnectButton.TabIndex = 44;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // delayCheckBox
            // 
            this.delayCheckBox.AutoSize = true;
            this.delayCheckBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delayCheckBox.ForeColor = System.Drawing.Color.Green;
            this.delayCheckBox.Location = new System.Drawing.Point(6, 26);
            this.delayCheckBox.Name = "delayCheckBox";
            this.delayCheckBox.Size = new System.Drawing.Size(65, 23);
            this.delayCheckBox.TabIndex = 35;
            this.delayCheckBox.Text = "Delay";
            this.delayCheckBox.UseVisualStyleBackColor = true;
            // 
            // delayTextBox
            // 
            this.delayTextBox.Location = new System.Drawing.Point(77, 24);
            this.delayTextBox.Name = "delayTextBox";
            this.delayTextBox.Size = new System.Drawing.Size(86, 27);
            this.delayTextBox.TabIndex = 36;
            this.delayTextBox.Text = "0";
            // 
            // captureNumTextBox
            // 
            this.captureNumTextBox.Location = new System.Drawing.Point(125, 53);
            this.captureNumTextBox.Name = "captureNumTextBox";
            this.captureNumTextBox.Size = new System.Drawing.Size(86, 27);
            this.captureNumTextBox.TabIndex = 38;
            this.captureNumTextBox.Text = "8";
            // 
            // CaptureNumCheckBox
            // 
            this.CaptureNumCheckBox.AutoSize = true;
            this.CaptureNumCheckBox.Checked = true;
            this.CaptureNumCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CaptureNumCheckBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CaptureNumCheckBox.ForeColor = System.Drawing.Color.Green;
            this.CaptureNumCheckBox.Location = new System.Drawing.Point(6, 57);
            this.CaptureNumCheckBox.Name = "CaptureNumCheckBox";
            this.CaptureNumCheckBox.Size = new System.Drawing.Size(113, 23);
            this.CaptureNumCheckBox.TabIndex = 37;
            this.CaptureNumCheckBox.Text = "Capture Num";
            this.CaptureNumCheckBox.UseVisualStyleBackColor = true;
            // 
            // summingStartTextBox
            // 
            this.summingStartTextBox.Location = new System.Drawing.Point(133, 82);
            this.summingStartTextBox.Name = "summingStartTextBox";
            this.summingStartTextBox.Size = new System.Drawing.Size(86, 27);
            this.summingStartTextBox.TabIndex = 40;
            this.summingStartTextBox.Text = "4";
            // 
            // summingStartCheckBox
            // 
            this.summingStartCheckBox.AutoSize = true;
            this.summingStartCheckBox.Checked = true;
            this.summingStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.summingStartCheckBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.summingStartCheckBox.ForeColor = System.Drawing.Color.Green;
            this.summingStartCheckBox.Location = new System.Drawing.Point(6, 86);
            this.summingStartCheckBox.Name = "summingStartCheckBox";
            this.summingStartCheckBox.Size = new System.Drawing.Size(121, 23);
            this.summingStartCheckBox.TabIndex = 39;
            this.summingStartCheckBox.Text = "Summing Start";
            this.summingStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // summingEndTextBox
            // 
            this.summingEndTextBox.Location = new System.Drawing.Point(133, 111);
            this.summingEndTextBox.Name = "summingEndTextBox";
            this.summingEndTextBox.Size = new System.Drawing.Size(86, 27);
            this.summingEndTextBox.TabIndex = 42;
            this.summingEndTextBox.Text = "7";
            // 
            // summingEndCheckBox
            // 
            this.summingEndCheckBox.AutoSize = true;
            this.summingEndCheckBox.Checked = true;
            this.summingEndCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.summingEndCheckBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.summingEndCheckBox.ForeColor = System.Drawing.Color.Green;
            this.summingEndCheckBox.Location = new System.Drawing.Point(6, 115);
            this.summingEndCheckBox.Name = "summingEndCheckBox";
            this.summingEndCheckBox.Size = new System.Drawing.Size(115, 23);
            this.summingEndCheckBox.TabIndex = 41;
            this.summingEndCheckBox.Text = "Summing End";
            this.summingEndCheckBox.UseVisualStyleBackColor = true;
            // 
            // FingerID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(958, 614);
            this.Controls.Add(this.PbTest_Button);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.CaptureButton);
            this.Controls.Add(this.calibration_but);
            this.Controls.Add(this.richconsole);
            this.Controls.Add(this.Gain_error_checkbox);
            this.Controls.Add(this.IspSettingGroupBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listBox1);
            this.KeyPreview = true;
            this.Name = "FingerID";
            this.Text = "FingerID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FingerID_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FingerID_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_original)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.IspSettingGroupBox.ResumeLayout(false);
            this.IspSettingGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox EnrollTimeTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EnrollButton;
        private System.Windows.Forms.Button MatchButton;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Button RunDatabaseButton;
        private System.Windows.Forms.Button calibration_but;
        private System.Windows.Forms.Button capture_but;
        private System.Windows.Forms.Button Delete_but;
        private System.Windows.Forms.Button touch_mode_but;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button verify_but;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox_original;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button set_but;
        private System.Windows.Forms.Button Load_but;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Offset_textbox;
        private System.Windows.Forms.TextBox Gain_textbox;
        private System.Windows.Forms.TextBox Intg_textbox;
        private System.Windows.Forms.GroupBox IspSettingGroupBox;
        private System.Windows.Forms.CheckBox Normalize_checkbox;
        private System.Windows.Forms.CheckBox denoise_checkbox;
        private System.Windows.Forms.CheckBox Gain_error_checkbox;
        private System.Windows.Forms.RichTextBox richconsole;
        private System.Windows.Forms.Button PbTest_Button;
        private System.Windows.Forms.Button CaptureButton;
        private System.Windows.Forms.TextBox fingernum_textbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox condition_combobox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox summingEndTextBox;
        private System.Windows.Forms.CheckBox summingEndCheckBox;
        private System.Windows.Forms.TextBox summingStartTextBox;
        private System.Windows.Forms.CheckBox summingStartCheckBox;
        private System.Windows.Forms.TextBox captureNumTextBox;
        private System.Windows.Forms.CheckBox CaptureNumCheckBox;
        private System.Windows.Forms.TextBox delayTextBox;
        private System.Windows.Forms.CheckBox delayCheckBox;
    }
}