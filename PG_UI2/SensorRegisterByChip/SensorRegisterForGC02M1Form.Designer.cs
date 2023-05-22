
namespace PG_UI2.SensorRegisterByChip
{
    partial class SensorRegisterForGC02M1Form
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.GroupBox_Demo = new System.Windows.Forms.GroupBox();
            this.CheckBox_ReadUpdate = new System.Windows.Forms.CheckBox();
            this.Button_I2CRead = new System.Windows.Forms.Button();
            this.Button_I2CWrite = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.CheckBox_I2CBit0 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit1 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit2 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit3 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit4 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit5 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit6 = new System.Windows.Forms.CheckBox();
            this.CheckBox_I2CBit7 = new System.Windows.Forms.CheckBox();
            this.NumericUpDown_Value = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NumericUpDown_Address = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.NumericUpDown_Page = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupBox_Engineer = new System.Windows.Forms.GroupBox();
            this.GroupBox_Setup = new System.Windows.Forms.GroupBox();
            this.Button_SetupSet = new System.Windows.Forms.Button();
            this.Button_SetupGet = new System.Windows.Forms.Button();
            this.NumericUpDown_Intg = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            this.GroupBox_Demo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Address)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Page)).BeginInit();
            this.GroupBox_Engineer.SuspendLayout();
            this.GroupBox_Setup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Intg)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.GroupBox_Demo);
            this.tabPage1.Controls.Add(this.GroupBox_Engineer);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 307);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Normal";
            // 
            // GroupBox_Demo
            // 
            this.GroupBox_Demo.Controls.Add(this.CheckBox_ReadUpdate);
            this.GroupBox_Demo.Controls.Add(this.Button_I2CRead);
            this.GroupBox_Demo.Controls.Add(this.Button_I2CWrite);
            this.GroupBox_Demo.Controls.Add(this.label8);
            this.GroupBox_Demo.Controls.Add(this.label7);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit0);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit1);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit2);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit3);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit4);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit5);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit6);
            this.GroupBox_Demo.Controls.Add(this.CheckBox_I2CBit7);
            this.GroupBox_Demo.Controls.Add(this.NumericUpDown_Value);
            this.GroupBox_Demo.Controls.Add(this.label3);
            this.GroupBox_Demo.Controls.Add(this.NumericUpDown_Address);
            this.GroupBox_Demo.Controls.Add(this.label2);
            this.GroupBox_Demo.Controls.Add(this.NumericUpDown_Page);
            this.GroupBox_Demo.Controls.Add(this.label1);
            this.GroupBox_Demo.Location = new System.Drawing.Point(6, 6);
            this.GroupBox_Demo.Name = "GroupBox_Demo";
            this.GroupBox_Demo.Size = new System.Drawing.Size(776, 86);
            this.GroupBox_Demo.TabIndex = 0;
            this.GroupBox_Demo.TabStop = false;
            this.GroupBox_Demo.Text = "GC02M1 Demo";
            // 
            // CheckBox_ReadUpdate
            // 
            this.CheckBox_ReadUpdate.AutoSize = true;
            this.CheckBox_ReadUpdate.Checked = true;
            this.CheckBox_ReadUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_ReadUpdate.Location = new System.Drawing.Point(283, 57);
            this.CheckBox_ReadUpdate.Name = "CheckBox_ReadUpdate";
            this.CheckBox_ReadUpdate.Size = new System.Drawing.Size(120, 16);
            this.CheckBox_ReadUpdate.TabIndex = 75;
            this.CheckBox_ReadUpdate.Text = "Update when Read";
            this.CheckBox_ReadUpdate.UseVisualStyleBackColor = true;
            // 
            // Button_I2CRead
            // 
            this.Button_I2CRead.Location = new System.Drawing.Point(473, 50);
            this.Button_I2CRead.Name = "Button_I2CRead";
            this.Button_I2CRead.Size = new System.Drawing.Size(75, 23);
            this.Button_I2CRead.TabIndex = 74;
            this.Button_I2CRead.Text = "Read";
            this.Button_I2CRead.UseVisualStyleBackColor = true;
            this.Button_I2CRead.Click += new System.EventHandler(this.Button_I2CRead_Click);
            // 
            // Button_I2CWrite
            // 
            this.Button_I2CWrite.Location = new System.Drawing.Point(473, 21);
            this.Button_I2CWrite.Name = "Button_I2CWrite";
            this.Button_I2CWrite.Size = new System.Drawing.Size(75, 23);
            this.Button_I2CWrite.TabIndex = 73;
            this.Button_I2CWrite.Text = "Write";
            this.Button_I2CWrite.UseVisualStyleBackColor = true;
            this.Button_I2CWrite.Click += new System.EventHandler(this.Button_I2CWrite_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(339, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 72;
            this.label8.Text = "[7]";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(444, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 71;
            this.label7.Text = "[0]";
            // 
            // CheckBox_I2CBit0
            // 
            this.CheckBox_I2CBit0.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit0.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit0.Location = new System.Drawing.Point(448, 33);
            this.CheckBox_I2CBit0.Name = "CheckBox_I2CBit0";
            this.CheckBox_I2CBit0.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit0.TabIndex = 70;
            this.CheckBox_I2CBit0.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit0.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit1
            // 
            this.CheckBox_I2CBit1.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit1.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit1.Location = new System.Drawing.Point(433, 33);
            this.CheckBox_I2CBit1.Name = "CheckBox_I2CBit1";
            this.CheckBox_I2CBit1.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit1.TabIndex = 69;
            this.CheckBox_I2CBit1.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit1.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit2
            // 
            this.CheckBox_I2CBit2.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit2.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit2.Location = new System.Drawing.Point(418, 33);
            this.CheckBox_I2CBit2.Name = "CheckBox_I2CBit2";
            this.CheckBox_I2CBit2.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit2.TabIndex = 68;
            this.CheckBox_I2CBit2.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit2.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit3
            // 
            this.CheckBox_I2CBit3.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit3.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit3.Location = new System.Drawing.Point(403, 33);
            this.CheckBox_I2CBit3.Name = "CheckBox_I2CBit3";
            this.CheckBox_I2CBit3.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit3.TabIndex = 67;
            this.CheckBox_I2CBit3.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit3.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit4
            // 
            this.CheckBox_I2CBit4.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit4.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit4.Location = new System.Drawing.Point(388, 33);
            this.CheckBox_I2CBit4.Name = "CheckBox_I2CBit4";
            this.CheckBox_I2CBit4.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit4.TabIndex = 66;
            this.CheckBox_I2CBit4.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit4.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit5
            // 
            this.CheckBox_I2CBit5.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit5.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit5.Location = new System.Drawing.Point(373, 33);
            this.CheckBox_I2CBit5.Name = "CheckBox_I2CBit5";
            this.CheckBox_I2CBit5.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit5.TabIndex = 65;
            this.CheckBox_I2CBit5.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit5.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit6
            // 
            this.CheckBox_I2CBit6.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit6.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit6.Location = new System.Drawing.Point(358, 33);
            this.CheckBox_I2CBit6.Name = "CheckBox_I2CBit6";
            this.CheckBox_I2CBit6.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit6.TabIndex = 64;
            this.CheckBox_I2CBit6.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit6.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // CheckBox_I2CBit7
            // 
            this.CheckBox_I2CBit7.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_I2CBit7.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_I2CBit7.Location = new System.Drawing.Point(343, 33);
            this.CheckBox_I2CBit7.Name = "CheckBox_I2CBit7";
            this.CheckBox_I2CBit7.Size = new System.Drawing.Size(15, 14);
            this.CheckBox_I2CBit7.TabIndex = 63;
            this.CheckBox_I2CBit7.UseVisualStyleBackColor = false;
            this.CheckBox_I2CBit7.CheckedChanged += new System.EventHandler(this.CheckBox_I2CBit_CheckedChanged);
            // 
            // NumericUpDown_Value
            // 
            this.NumericUpDown_Value.Hexadecimal = true;
            this.NumericUpDown_Value.Location = new System.Drawing.Point(283, 21);
            this.NumericUpDown_Value.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown_Value.Name = "NumericUpDown_Value";
            this.NumericUpDown_Value.Size = new System.Drawing.Size(50, 22);
            this.NumericUpDown_Value.TabIndex = 5;
            this.NumericUpDown_Value.ValueChanged += new System.EventHandler(this.NumericUpDown_Value_ValueChanged);
            this.NumericUpDown_Value.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericUpDown_Value_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(224, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Value 0x";
            // 
            // NumericUpDown_Address
            // 
            this.NumericUpDown_Address.Hexadecimal = true;
            this.NumericUpDown_Address.Location = new System.Drawing.Point(168, 21);
            this.NumericUpDown_Address.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown_Address.Name = "NumericUpDown_Address";
            this.NumericUpDown_Address.Size = new System.Drawing.Size(50, 22);
            this.NumericUpDown_Address.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address 0x";
            // 
            // NumericUpDown_Page
            // 
            this.NumericUpDown_Page.Location = new System.Drawing.Point(41, 21);
            this.NumericUpDown_Page.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NumericUpDown_Page.Name = "NumericUpDown_Page";
            this.NumericUpDown_Page.Size = new System.Drawing.Size(50, 22);
            this.NumericUpDown_Page.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Page";
            // 
            // GroupBox_Engineer
            // 
            this.GroupBox_Engineer.Controls.Add(this.GroupBox_Setup);
            this.GroupBox_Engineer.Location = new System.Drawing.Point(6, 98);
            this.GroupBox_Engineer.Name = "GroupBox_Engineer";
            this.GroupBox_Engineer.Size = new System.Drawing.Size(775, 200);
            this.GroupBox_Engineer.TabIndex = 1;
            this.GroupBox_Engineer.TabStop = false;
            this.GroupBox_Engineer.Text = "GC02M1 Engineer";
            // 
            // GroupBox_Setup
            // 
            this.GroupBox_Setup.Controls.Add(this.Button_SetupSet);
            this.GroupBox_Setup.Controls.Add(this.Button_SetupGet);
            this.GroupBox_Setup.Controls.Add(this.NumericUpDown_Intg);
            this.GroupBox_Setup.Controls.Add(this.label4);
            this.GroupBox_Setup.Location = new System.Drawing.Point(7, 43);
            this.GroupBox_Setup.Name = "GroupBox_Setup";
            this.GroupBox_Setup.Size = new System.Drawing.Size(190, 150);
            this.GroupBox_Setup.TabIndex = 0;
            this.GroupBox_Setup.TabStop = false;
            this.GroupBox_Setup.Text = "Group Setup";
            // 
            // Button_SetupSet
            // 
            this.Button_SetupSet.Location = new System.Drawing.Point(109, 121);
            this.Button_SetupSet.Name = "Button_SetupSet";
            this.Button_SetupSet.Size = new System.Drawing.Size(75, 23);
            this.Button_SetupSet.TabIndex = 8;
            this.Button_SetupSet.Text = "Set";
            this.Button_SetupSet.UseVisualStyleBackColor = true;
            this.Button_SetupSet.Click += new System.EventHandler(this.Button_SetupSet_Click);
            // 
            // Button_SetupGet
            // 
            this.Button_SetupGet.Location = new System.Drawing.Point(6, 121);
            this.Button_SetupGet.Name = "Button_SetupGet";
            this.Button_SetupGet.Size = new System.Drawing.Size(75, 23);
            this.Button_SetupGet.TabIndex = 7;
            this.Button_SetupGet.Text = "Get";
            this.Button_SetupGet.UseVisualStyleBackColor = true;
            this.Button_SetupGet.Click += new System.EventHandler(this.Button_SetupGet_Click);
            // 
            // NumericUpDown_Intg
            // 
            this.NumericUpDown_Intg.Enabled = false;
            this.NumericUpDown_Intg.Hexadecimal = true;
            this.NumericUpDown_Intg.Location = new System.Drawing.Point(59, 21);
            this.NumericUpDown_Intg.Name = "NumericUpDown_Intg";
            this.NumericUpDown_Intg.Size = new System.Drawing.Size(100, 22);
            this.NumericUpDown_Intg.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Intg 0x";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 333);
            this.tabControl1.TabIndex = 2;
            // 
            // SensorRegisterForGC02M1Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(819, 450);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "SensorRegisterForGC02M1Form";
            this.Text = "SensorRegisterForT8820Form";
            this.tabPage1.ResumeLayout(false);
            this.GroupBox_Demo.ResumeLayout(false);
            this.GroupBox_Demo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Address)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Page)).EndInit();
            this.GroupBox_Engineer.ResumeLayout(false);
            this.GroupBox_Setup.ResumeLayout(false);
            this.GroupBox_Setup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Intg)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox GroupBox_Demo;
        private System.Windows.Forms.CheckBox CheckBox_ReadUpdate;
        private System.Windows.Forms.Button Button_I2CRead;
        private System.Windows.Forms.Button Button_I2CWrite;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit0;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit1;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit2;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit3;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit4;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit5;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit6;
        private System.Windows.Forms.CheckBox CheckBox_I2CBit7;
        private System.Windows.Forms.NumericUpDown NumericUpDown_Value;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NumericUpDown_Address;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NumericUpDown_Page;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox GroupBox_Engineer;
        private System.Windows.Forms.GroupBox GroupBox_Setup;
        private System.Windows.Forms.Button Button_SetupSet;
        private System.Windows.Forms.Button Button_SetupGet;
        private System.Windows.Forms.NumericUpDown NumericUpDown_Intg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
    }
}