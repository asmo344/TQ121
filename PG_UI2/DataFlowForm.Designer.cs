
namespace PG_UI2
{
    partial class DataFlowForm
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PictureBox_FlowChar = new System.Windows.Forms.PictureBox();
            this.RemoveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_FlowChar)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddProcessToolStripMenuItem,
            this.ParameterToolStripMenuItem,
            this.MoveLeftToolStripMenuItem,
            this.MoveRightToolStripMenuItem,
            this.RemoveToolStripMenuItem,
            this.RemoveAllToolStripMenuItem,
            this.DisplayToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 180);
            // 
            // AddProcessToolStripMenuItem
            // 
            this.AddProcessToolStripMenuItem.Name = "AddProcessToolStripMenuItem";
            this.AddProcessToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.AddProcessToolStripMenuItem.Text = "Add Process";
            this.AddProcessToolStripMenuItem.Click += new System.EventHandler(this.AddProcessToolStripMenuItem_Click);
            // 
            // ParameterToolStripMenuItem
            // 
            this.ParameterToolStripMenuItem.Name = "ParameterToolStripMenuItem";
            this.ParameterToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ParameterToolStripMenuItem.Text = "Parameter";
            this.ParameterToolStripMenuItem.Click += new System.EventHandler(this.ParameterToolStripMenuItem_Click);
            // 
            // MoveLeftToolStripMenuItem
            // 
            this.MoveLeftToolStripMenuItem.Name = "MoveLeftToolStripMenuItem";
            this.MoveLeftToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.MoveLeftToolStripMenuItem.Text = "Move Left";
            this.MoveLeftToolStripMenuItem.Click += new System.EventHandler(this.MoveLeftToolStripMenuItem_Click);
            // 
            // MoveRightToolStripMenuItem
            // 
            this.MoveRightToolStripMenuItem.Name = "MoveRightToolStripMenuItem";
            this.MoveRightToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.MoveRightToolStripMenuItem.Text = "Move Right";
            this.MoveRightToolStripMenuItem.Click += new System.EventHandler(this.MoveRightToolStripMenuItem_Click);
            // 
            // RemoveToolStripMenuItem
            // 
            this.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem";
            this.RemoveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RemoveToolStripMenuItem.Text = "Remove";
            this.RemoveToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // DisplayToolStripMenuItem
            // 
            this.DisplayToolStripMenuItem.Name = "DisplayToolStripMenuItem";
            this.DisplayToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.DisplayToolStripMenuItem.Text = "Display";
            this.DisplayToolStripMenuItem.Click += new System.EventHandler(this.DisplayToolStripMenuItem_Click);
            // 
            // PictureBox_FlowChar
            // 
            this.PictureBox_FlowChar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureBox_FlowChar.Location = new System.Drawing.Point(0, 0);
            this.PictureBox_FlowChar.Name = "PictureBox_FlowChar";
            this.PictureBox_FlowChar.Size = new System.Drawing.Size(800, 111);
            this.PictureBox_FlowChar.TabIndex = 1;
            this.PictureBox_FlowChar.TabStop = false;
            // 
            // RemoveAllToolStripMenuItem
            // 
            this.RemoveAllToolStripMenuItem.Name = "RemoveAllToolStripMenuItem";
            this.RemoveAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RemoveAllToolStripMenuItem.Text = "RemoveAll";
            this.RemoveAllToolStripMenuItem.Click += new System.EventHandler(this.RemoveAllToolStripMenuItem_Click);
            // 
            // DataFlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 111);
            this.Controls.Add(this.PictureBox_FlowChar);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "DataFlowForm";
            this.Text = "DataFlowForm";
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_FlowChar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.PictureBox PictureBox_FlowChar;
        private System.Windows.Forms.ToolStripMenuItem AddProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveAllToolStripMenuItem;
    }
}