
namespace PG_UI2
{
    partial class SensorRegisterMainForm
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
            this.Panel_User = new System.Windows.Forms.Panel();
            this.Panel_Generic = new System.Windows.Forms.Panel();
            this.CheckBox_ReadUpdate = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CheckBox_I2CBit0 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit1 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit2 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit3 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit4 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit5 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit6 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit7 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit8 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit9 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit10 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit11 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit12 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit13 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit14 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit15 = new System.Windows.Forms.CheckBox();
            this.Button_I2CRead = new System.Windows.Forms.Button();
            this.Button_I2CWrite = new System.Windows.Forms.Button();
            this.ComboBox_I2CFormat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericUpDown_I2CValue = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDown_I2CAddress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.NumericUpDown_SlaveID = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.Panel_ByChip = new System.Windows.Forms.Panel();
            this.Panel_Buttons = new System.Windows.Forms.Panel();
            this.Button_ChipReset = new System.Windows.Forms.Button();
            this.Button_ChipWakeup = new System.Windows.Forms.Button();
            this.Button_ChipPowerDown = new System.Windows.Forms.Button();
            this.TextBox_LogMessage = new System.Windows.Forms.TextBox();
            this.Panel_User.SuspendLayout();
            this.Panel_Generic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_I2CValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_I2CAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_SlaveID)).BeginInit();
            this.Panel_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_User
            // 
            this.Panel_User.Controls.Add(this.Panel_Generic);
            this.Panel_User.Controls.Add(this.Panel_ByChip);
            this.Panel_User.Controls.Add(this.Panel_Buttons);
            this.Panel_User.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_User.Location = new System.Drawing.Point(0, 0);
            this.Panel_User.Name = "Panel_User";
            this.Panel_User.Size = new System.Drawing.Size(800, 413);
            this.Panel_User.TabIndex = 1;
            // 
            // Panel_Generic
            // 
            this.Panel_Generic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_Generic.Controls.Add(this.CheckBox_ReadUpdate);
            this.Panel_Generic.Controls.Add(this.label8);
            this.Panel_Generic.Controls.Add(this.label7);
            this.Panel_Generic.Controls.Add(this.label6);
            this.Panel_Generic.Controls.Add(this.label5);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit0);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit1);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit2);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit3);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit4);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit5);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit6);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit7);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit8);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit9);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit10);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit11);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit12);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit13);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit14);
            this.Panel_Generic.Controls.Add(this.CheckBox_I2CBit15);
            this.Panel_Generic.Controls.Add(this.Button_I2CRead);
            this.Panel_Generic.Controls.Add(this.Button_I2CWrite);
            this.Panel_Generic.Controls.Add(this.ComboBox_I2CFormat);
            this.Panel_Generic.Controls.Add(this.label4);
            this.Panel_Generic.Controls.Add(this.NumericUpDown_I2CValue);
            this.Panel_Generic.Controls.Add(this.label3);
            this.Panel_Generic.Controls.Add(this.NumericUpDown_I2CAddress);
            this.Panel_Generic.Controls.Add(this.label2);
            this.Panel_Generic.Controls.Add(this.NumericUpDown_SlaveID);
            this.Panel_Generic.Controls.Add(this.label1);
            this.Panel_Generic.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel_Generic.Location = new System.Drawing.Point(4, 302);
            this.Panel_Generic.Name = "Panel_Generic";
            this.Panel_Generic.Size = new System.Drawing.Size(650, 104);
            this.Panel_Generic.TabIndex = 2;
            // 
            // CheckBox_ReadUpdate
            // 
            this.CheckBox_ReadUpdate.AutoSize = true;
            this.CheckBox_ReadUpdate.Checked = true;
            this.CheckBox_ReadUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_ReadUpdate.Location = new System.Drawing.Point(239, 71);
            this.CheckBox_ReadUpdate.Name = "CheckBox_ReadUpdate";
            this.CheckBox_ReadUpdate.Size = new System.Drawing.Size(147, 22);
            this.CheckBox_ReadUpdate.TabIndex = 63;
            this.CheckBox_ReadUpdate.Text = "Update when Read";
            this.CheckBox_ReadUpdate.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(502, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 18);
            this.label8.TabIndex = 62;
            this.label8.Text = "[7]";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(607, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 18);
            this.label7.TabIndex = 61;
            this.label7.Text = "[0]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(473, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 18);
            this.label6.TabIndex = 60;
            this.label6.Text = "[8]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(365, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 18);
            this.label5.TabIndex = 59;
            this.label5.Text = "[15]";
            // 
            // CheckBox_I2CBit0
            // 
            this.CheckBox_I2CBit0.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit0.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit0.Location = new System.Drawing.Point(611, 54);
            this.CheckBox_I2CBit0.Name = "CheckBox_I2CBit0";
            this.CheckBox_I2CBit0.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit0.TabIndex = 58;
            this.CheckBox_I2CBit0.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit0.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit1
            // 
            this.CheckBox_I2CBit1.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit1.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit1.Location = new System.Drawing.Point(596, 54);
            this.CheckBox_I2CBit1.Name = "CheckBox_I2CBit1";
            this.CheckBox_I2CBit1.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit1.TabIndex = 57;
            this.CheckBox_I2CBit1.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit1.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit2
            // 
            this.CheckBox_I2CBit2.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit2.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit2.Location = new System.Drawing.Point(581, 54);
            this.CheckBox_I2CBit2.Name = "CheckBox_I2CBit2";
            this.CheckBox_I2CBit2.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit2.TabIndex = 56;
            this.CheckBox_I2CBit2.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit2.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit3
            // 
            this.CheckBox_I2CBit3.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit3.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit3.Location = new System.Drawing.Point(566, 54);
            this.CheckBox_I2CBit3.Name = "CheckBox_I2CBit3";
            this.CheckBox_I2CBit3.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit3.TabIndex = 55;
            this.CheckBox_I2CBit3.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit3.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit4
            // 
            this.CheckBox_I2CBit4.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit4.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit4.Location = new System.Drawing.Point(551, 54);
            this.CheckBox_I2CBit4.Name = "CheckBox_I2CBit4";
            this.CheckBox_I2CBit4.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit4.TabIndex = 54;
            this.CheckBox_I2CBit4.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit4.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit5
            // 
            this.CheckBox_I2CBit5.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit5.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit5.Location = new System.Drawing.Point(536, 54);
            this.CheckBox_I2CBit5.Name = "CheckBox_I2CBit5";
            this.CheckBox_I2CBit5.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit5.TabIndex = 53;
            this.CheckBox_I2CBit5.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit5.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit6
            // 
            this.CheckBox_I2CBit6.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit6.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit6.Location = new System.Drawing.Point(521, 54);
            this.CheckBox_I2CBit6.Name = "CheckBox_I2CBit6";
            this.CheckBox_I2CBit6.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit6.TabIndex = 52;
            this.CheckBox_I2CBit6.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit6.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit7
            // 
            this.CheckBox_I2CBit7.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit7.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit7.Location = new System.Drawing.Point(506, 54);
            this.CheckBox_I2CBit7.Name = "CheckBox_I2CBit7";
            this.CheckBox_I2CBit7.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit7.TabIndex = 51;
            this.CheckBox_I2CBit7.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit7.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit8
            // 
            this.CheckBox_I2CBit8.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit8.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit8.Location = new System.Drawing.Point(477, 54);
            this.CheckBox_I2CBit8.Name = "CheckBox_I2CBit8";
            this.CheckBox_I2CBit8.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit8.TabIndex = 50;
            this.CheckBox_I2CBit8.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit8.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit9
            // 
            this.CheckBox_I2CBit9.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit9.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit9.Location = new System.Drawing.Point(462, 54);
            this.CheckBox_I2CBit9.Name = "CheckBox_I2CBit9";
            this.CheckBox_I2CBit9.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit9.TabIndex = 49;
            this.CheckBox_I2CBit9.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit9.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit10
            // 
            this.CheckBox_I2CBit10.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit10.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit10.Location = new System.Drawing.Point(447, 54);
            this.CheckBox_I2CBit10.Name = "CheckBox_I2CBit10";
            this.CheckBox_I2CBit10.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit10.TabIndex = 48;
            this.CheckBox_I2CBit10.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit10.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit11
            // 
            this.CheckBox_I2CBit11.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit11.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit11.Location = new System.Drawing.Point(432, 54);
            this.CheckBox_I2CBit11.Name = "CheckBox_I2CBit11";
            this.CheckBox_I2CBit11.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit11.TabIndex = 47;
            this.CheckBox_I2CBit11.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit11.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit12
            // 
            this.CheckBox_I2CBit12.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit12.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit12.Location = new System.Drawing.Point(417, 54);
            this.CheckBox_I2CBit12.Name = "CheckBox_I2CBit12";
            this.CheckBox_I2CBit12.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit12.TabIndex = 46;
            this.CheckBox_I2CBit12.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit12.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit13
            // 
            this.CheckBox_I2CBit13.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit13.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit13.Location = new System.Drawing.Point(402, 54);
            this.CheckBox_I2CBit13.Name = "CheckBox_I2CBit13";
            this.CheckBox_I2CBit13.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit13.TabIndex = 45;
            this.CheckBox_I2CBit13.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit13.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit14
            // 
            this.CheckBox_I2CBit14.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit14.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit14.Location = new System.Drawing.Point(387, 54);
            this.CheckBox_I2CBit14.Name = "CheckBox_I2CBit14";
            this.CheckBox_I2CBit14.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit14.TabIndex = 44;
            this.CheckBox_I2CBit14.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit14.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit15
            // 
            this.CheckBox_I2CBit15.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit15.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit15.Location = new System.Drawing.Point(372, 54);
            this.CheckBox_I2CBit15.Name = "CheckBox_I2CBit15";
            this.CheckBox_I2CBit15.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit15.TabIndex = 43;
            this.CheckBox_I2CBit15.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit15.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // Button_I2CRead
            // 
            this.Button_I2CRead.Location = new System.Drawing.Point(555, 15);
            this.Button_I2CRead.Name = "Button_I2CRead";
            this.Button_I2CRead.Size = new System.Drawing.Size(75, 23);
            this.Button_I2CRead.TabIndex = 9;
            this.Button_I2CRead.Text = "Read";
            this.Button_I2CRead.UseVisualStyleBackColor = true;
            this.Button_I2CRead.Click += new System.EventHandler(this.Button_I2CRead_Click);
            // 
            // Button_I2CWrite
            // 
            this.Button_I2CWrite.Location = new System.Drawing.Point(474, 15);
            this.Button_I2CWrite.Name = "Button_I2CWrite";
            this.Button_I2CWrite.Size = new System.Drawing.Size(75, 23);
            this.Button_I2CWrite.TabIndex = 8;
            this.Button_I2CWrite.Text = "Write";
            this.Button_I2CWrite.UseVisualStyleBackColor = true;
            this.Button_I2CWrite.Click += new System.EventHandler(this.Button_I2CWrite_Click);
            // 
            // ComboBox_I2CFormat
            // 
            this.ComboBox_I2CFormat.FormattingEnabled = true;
            this.ComboBox_I2CFormat.Location = new System.Drawing.Point(239, 16);
            this.ComboBox_I2CFormat.Name = "ComboBox_I2CFormat";
            this.ComboBox_I2CFormat.Size = new System.Drawing.Size(120, 26);
            this.ComboBox_I2CFormat.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(192, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Format";
            // 
            // NumericUpDown_I2CValue
            // 
            this.NumericUpDown_I2CValue.Hexadecimal = true;
            this.NumericUpDown_I2CValue.Location = new System.Drawing.Point(239, 43);
            this.NumericUpDown_I2CValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumericUpDown_I2CValue.Name = "NumericUpDown_I2CValue";
            this.NumericUpDown_I2CValue.Size = new System.Drawing.Size(120, 26);
            this.NumericUpDown_I2CValue.TabIndex = 5;
            this.NumericUpDown_I2CValue.ValueChanged += new System.EventHandler(this.NumericUpDown_I2CValue_ValueChanged);
            this.NumericUpDown_I2CValue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericUpDown_I2CValue_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(180, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Value 0x";
            // 
            // NumericUpDown_I2CAddress
            // 
            this.NumericUpDown_I2CAddress.Hexadecimal = true;
            this.NumericUpDown_I2CAddress.Location = new System.Drawing.Point(74, 43);
            this.NumericUpDown_I2CAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumericUpDown_I2CAddress.Name = "NumericUpDown_I2CAddress";
            this.NumericUpDown_I2CAddress.Size = new System.Drawing.Size(100, 26);
            this.NumericUpDown_I2CAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address 0x";
            // 
            // NumericUpDown_SlaveID
            // 
            this.NumericUpDown_SlaveID.Hexadecimal = true;
            this.NumericUpDown_SlaveID.Location = new System.Drawing.Point(74, 15);
            this.NumericUpDown_SlaveID.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown_SlaveID.Name = "NumericUpDown_SlaveID";
            this.NumericUpDown_SlaveID.Size = new System.Drawing.Size(50, 26);
            this.NumericUpDown_SlaveID.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "SlaveID 0x";
            // 
            // Panel_ByChip
            // 
            this.Panel_ByChip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_ByChip.Location = new System.Drawing.Point(4, 4);
            this.Panel_ByChip.Name = "Panel_ByChip";
            this.Panel_ByChip.Size = new System.Drawing.Size(650, 292);
            this.Panel_ByChip.TabIndex = 1;
            // 
            // Panel_Buttons
            // 
            this.Panel_Buttons.Controls.Add(this.Button_ChipReset);
            this.Panel_Buttons.Controls.Add(this.Button_ChipWakeup);
            this.Panel_Buttons.Controls.Add(this.Button_ChipPowerDown);
            this.Panel_Buttons.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel_Buttons.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel_Buttons.Location = new System.Drawing.Point(660, 0);
            this.Panel_Buttons.Name = "Panel_Buttons";
            this.Panel_Buttons.Size = new System.Drawing.Size(140, 413);
            this.Panel_Buttons.TabIndex = 0;
            // 
            // Button_ChipReset
            // 
            this.Button_ChipReset.Location = new System.Drawing.Point(3, 70);
            this.Button_ChipReset.Name = "Button_ChipReset";
            this.Button_ChipReset.Size = new System.Drawing.Size(134, 23);
            this.Button_ChipReset.TabIndex = 2;
            this.Button_ChipReset.Text = "Chip Reset";
            this.Button_ChipReset.UseVisualStyleBackColor = true;
            this.Button_ChipReset.Click += new System.EventHandler(this.Button_ChipReset_Click);
            // 
            // Button_ChipWakeup
            // 
            this.Button_ChipWakeup.Location = new System.Drawing.Point(3, 41);
            this.Button_ChipWakeup.Name = "Button_ChipWakeup";
            this.Button_ChipWakeup.Size = new System.Drawing.Size(134, 23);
            this.Button_ChipWakeup.TabIndex = 1;
            this.Button_ChipWakeup.Text = "Chip Wakeup";
            this.Button_ChipWakeup.UseVisualStyleBackColor = true;
            this.Button_ChipWakeup.Click += new System.EventHandler(this.Button_ChipWakeup_Click);
            // 
            // Button_ChipPowerDown
            // 
            this.Button_ChipPowerDown.Location = new System.Drawing.Point(3, 12);
            this.Button_ChipPowerDown.Name = "Button_ChipPowerDown";
            this.Button_ChipPowerDown.Size = new System.Drawing.Size(134, 23);
            this.Button_ChipPowerDown.TabIndex = 0;
            this.Button_ChipPowerDown.Text = "Chip Power Down";
            this.Button_ChipPowerDown.UseVisualStyleBackColor = true;
            this.Button_ChipPowerDown.Click += new System.EventHandler(this.Button_ChipPowerDown_Click);
            // 
            // TextBox_LogMessage
            // 
            this.TextBox_LogMessage.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TextBox_LogMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextBox_LogMessage.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.TextBox_LogMessage.Location = new System.Drawing.Point(0, 412);
            this.TextBox_LogMessage.Multiline = true;
            this.TextBox_LogMessage.Name = "TextBox_LogMessage";
            this.TextBox_LogMessage.ReadOnly = true;
            this.TextBox_LogMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_LogMessage.Size = new System.Drawing.Size(800, 119);
            this.TextBox_LogMessage.TabIndex = 2;
            // 
            // SensorRegisterMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 531);
            this.Controls.Add(this.TextBox_LogMessage);
            this.Controls.Add(this.Panel_User);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "SensorRegisterMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SensorRegisterMainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SensorRegisterMainForm_FormClosing);
            this.Panel_User.ResumeLayout(false);
            this.Panel_Generic.ResumeLayout(false);
            this.Panel_Generic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_I2CValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_I2CAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_SlaveID)).EndInit();
            this.Panel_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel Panel_User;
        private System.Windows.Forms.Panel Panel_Buttons;
        private System.Windows.Forms.Button Button_ChipReset;
        private System.Windows.Forms.Button Button_ChipWakeup;
        private System.Windows.Forms.Button Button_ChipPowerDown;
        private System.Windows.Forms.Panel Panel_Generic;
        private System.Windows.Forms.Panel Panel_ByChip;
        private System.Windows.Forms.NumericUpDown NumericUpDown_I2CValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NumericUpDown_I2CAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NumericUpDown_SlaveID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBox_I2CFormat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Button_I2CRead;
        private System.Windows.Forms.Button Button_I2CWrite;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit0;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit1;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit2;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit3;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit4;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit5;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit6;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit7;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit8;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit9;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit10;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit11;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit12;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit13;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit14;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit15;
        private System.Windows.Forms.CheckBox CheckBox_ReadUpdate;
        private System.Windows.Forms.TextBox TextBox_LogMessage;
    }
}