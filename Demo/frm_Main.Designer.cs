namespace Demo
{
    partial class frm_Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.信息 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_rev = new System.Windows.Forms.Label();
            this.lbl_pjName = new System.Windows.Forms.Label();
            this.btn_start = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtb_debug = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.信息.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(5, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(604, 391);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图像";
            // 
            // 信息
            // 
            this.信息.Controls.Add(this.groupBox2);
            this.信息.Controls.Add(this.btn_Stop);
            this.信息.Controls.Add(this.btn_start);
            this.信息.Location = new System.Drawing.Point(650, 40);
            this.信息.Name = "信息";
            this.信息.Size = new System.Drawing.Size(194, 391);
            this.信息.TabIndex = 1;
            this.信息.TabStop = false;
            this.信息.Text = "信息";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbl_rev);
            this.groupBox2.Controls.Add(this.lbl_pjName);
            this.groupBox2.Location = new System.Drawing.Point(7, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 108);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // lbl_rev
            // 
            this.lbl_rev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_rev.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_rev.Location = new System.Drawing.Point(18, 52);
            this.lbl_rev.Name = "lbl_rev";
            this.lbl_rev.Size = new System.Drawing.Size(141, 34);
            this.lbl_rev.TabIndex = 0;
            this.lbl_rev.Text = "版本信息";
            this.lbl_rev.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_pjName
            // 
            this.lbl_pjName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_pjName.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_pjName.Location = new System.Drawing.Point(18, 18);
            this.lbl_pjName.Name = "lbl_pjName";
            this.lbl_pjName.Size = new System.Drawing.Size(141, 34);
            this.lbl_pjName.TabIndex = 0;
            this.lbl_pjName.Text = "项目名称";
            this.lbl_pjName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_start
            // 
            this.btn_start.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_start.Location = new System.Drawing.Point(46, 240);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(120, 42);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "开始";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rtb_debug);
            this.groupBox3.Location = new System.Drawing.Point(5, 455);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(839, 192);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "调试信息：";
            // 
            // rtb_debug
            // 
            this.rtb_debug.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.rtb_debug.ForeColor = System.Drawing.Color.Lime;
            this.rtb_debug.Location = new System.Drawing.Point(9, 20);
            this.rtb_debug.Name = "rtb_debug";
            this.rtb_debug.Size = new System.Drawing.Size(824, 166);
            this.rtb_debug.TabIndex = 0;
            this.rtb_debug.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(867, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            this.设置ToolStripMenuItem.Click += new System.EventHandler(this.设置ToolStripMenuItem_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Stop.Location = new System.Drawing.Point(46, 313);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(120, 42);
            this.btn_Stop.TabIndex = 1;
            this.btn_Stop.Text = "结束";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 659);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.信息);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frm_Main";
            this.Text = "示例程序";
            this.Load += new System.EventHandler(this.frm_Main_Load);
            this.信息.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox 信息;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_rev;
        private System.Windows.Forms.Label lbl_pjName;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtb_debug;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.Button btn_Stop;
    }
}

