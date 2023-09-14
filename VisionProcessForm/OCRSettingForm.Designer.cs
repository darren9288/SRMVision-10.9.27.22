namespace VisionProcessForm
{
    partial class OCRSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OCRSettingForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.lbl_CharWidth = new System.Windows.Forms.Label();
            this.lbl_CharHeight = new System.Windows.Forms.Label();
            this.txt_CharMaxWidth = new System.Windows.Forms.TextBox();
            this.txt_CharHeight = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_lineCount = new System.Windows.Forms.Label();
            this.txt_WordCount = new System.Windows.Forms.TextBox();
            this.lbl_WordCount = new System.Windows.Forms.Label();
            this.txt_LineCount = new System.Windows.Forms.TextBox();
            this.cbo_WordNo = new System.Windows.Forms.ComboBox();
            this.cbo_LineNo = new System.Windows.Forms.ComboBox();
            this.txt_CharCount = new System.Windows.Forms.TextBox();
            this.lbl_LineNo = new System.Windows.Forms.Label();
            this.lbl_CharCount = new System.Windows.Forms.Label();
            this.lbl_WordNo = new System.Windows.Forms.Label();
            this.pnl_pic = new System.Windows.Forms.Panel();
            this.contextMenuStrip_ModifyChar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Any = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Letter = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_LetterUppercase = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_LetterLowercase = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Digit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Punctuation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Symbol = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_CharMinWidth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip_ModifyChar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // lbl_CharWidth
            // 
            resources.ApplyResources(this.lbl_CharWidth, "lbl_CharWidth");
            this.lbl_CharWidth.Name = "lbl_CharWidth";
            // 
            // lbl_CharHeight
            // 
            resources.ApplyResources(this.lbl_CharHeight, "lbl_CharHeight");
            this.lbl_CharHeight.Name = "lbl_CharHeight";
            // 
            // txt_CharMaxWidth
            // 
            resources.ApplyResources(this.txt_CharMaxWidth, "txt_CharMaxWidth");
            this.txt_CharMaxWidth.Name = "txt_CharMaxWidth";
            this.txt_CharMaxWidth.TextChanged += new System.EventHandler(this.txt_CharMaxWidth_TextChanged);
            // 
            // txt_CharHeight
            // 
            resources.ApplyResources(this.txt_CharHeight, "txt_CharHeight");
            this.txt_CharHeight.Name = "txt_CharHeight";
            this.txt_CharHeight.TextChanged += new System.EventHandler(this.txt_CharHeight_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_lineCount);
            this.groupBox1.Controls.Add(this.txt_WordCount);
            this.groupBox1.Controls.Add(this.lbl_WordCount);
            this.groupBox1.Controls.Add(this.txt_LineCount);
            this.groupBox1.Controls.Add(this.cbo_WordNo);
            this.groupBox1.Controls.Add(this.cbo_LineNo);
            this.groupBox1.Controls.Add(this.txt_CharCount);
            this.groupBox1.Controls.Add(this.lbl_LineNo);
            this.groupBox1.Controls.Add(this.lbl_CharCount);
            this.groupBox1.Controls.Add(this.lbl_WordNo);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lbl_lineCount
            // 
            resources.ApplyResources(this.lbl_lineCount, "lbl_lineCount");
            this.lbl_lineCount.Name = "lbl_lineCount";
            // 
            // txt_WordCount
            // 
            resources.ApplyResources(this.txt_WordCount, "txt_WordCount");
            this.txt_WordCount.Name = "txt_WordCount";
            this.txt_WordCount.TextChanged += new System.EventHandler(this.txt_WordCount_TextChanged);
            // 
            // lbl_WordCount
            // 
            resources.ApplyResources(this.lbl_WordCount, "lbl_WordCount");
            this.lbl_WordCount.Name = "lbl_WordCount";
            // 
            // txt_LineCount
            // 
            resources.ApplyResources(this.txt_LineCount, "txt_LineCount");
            this.txt_LineCount.Name = "txt_LineCount";
            this.txt_LineCount.TextChanged += new System.EventHandler(this.txt_LineCount_TextChanged);
            // 
            // cbo_WordNo
            // 
            this.cbo_WordNo.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_WordNo, "cbo_WordNo");
            this.cbo_WordNo.Name = "cbo_WordNo";
            this.cbo_WordNo.SelectedIndexChanged += new System.EventHandler(this.cbo_WordNo_SelectedIndexChanged);
            // 
            // cbo_LineNo
            // 
            this.cbo_LineNo.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_LineNo, "cbo_LineNo");
            this.cbo_LineNo.Name = "cbo_LineNo";
            this.cbo_LineNo.SelectedIndexChanged += new System.EventHandler(this.cbo_LineNo_SelectedIndexChanged);
            // 
            // txt_CharCount
            // 
            resources.ApplyResources(this.txt_CharCount, "txt_CharCount");
            this.txt_CharCount.Name = "txt_CharCount";
            this.txt_CharCount.TextChanged += new System.EventHandler(this.txt_CharCount_TextChanged);
            // 
            // lbl_LineNo
            // 
            resources.ApplyResources(this.lbl_LineNo, "lbl_LineNo");
            this.lbl_LineNo.Name = "lbl_LineNo";
            // 
            // lbl_CharCount
            // 
            resources.ApplyResources(this.lbl_CharCount, "lbl_CharCount");
            this.lbl_CharCount.Name = "lbl_CharCount";
            // 
            // lbl_WordNo
            // 
            resources.ApplyResources(this.lbl_WordNo, "lbl_WordNo");
            this.lbl_WordNo.Name = "lbl_WordNo";
            // 
            // pnl_pic
            // 
            resources.ApplyResources(this.pnl_pic, "pnl_pic");
            this.pnl_pic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pnl_pic.ContextMenuStrip = this.contextMenuStrip_ModifyChar;
            this.pnl_pic.Name = "pnl_pic";
            this.pnl_pic.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_pic_Paint);
            this.pnl_pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnl_pic_MouseDown);
            // 
            // contextMenuStrip_ModifyChar
            // 
            resources.ApplyResources(this.contextMenuStrip_ModifyChar, "contextMenuStrip_ModifyChar");
            this.contextMenuStrip_ModifyChar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Any,
            this.ToolStripMenuItem_Letter,
            this.ToolStripMenuItem_LetterUppercase,
            this.ToolStripMenuItem_LetterLowercase,
            this.ToolStripMenuItem_Digit,
            this.ToolStripMenuItem_Punctuation,
            this.ToolStripMenuItem_Symbol});
            this.contextMenuStrip_ModifyChar.Name = "contextMenuStrip_Setting";
            this.contextMenuStrip_ModifyChar.ShowCheckMargin = true;
            this.contextMenuStrip_ModifyChar.ShowImageMargin = false;
            this.contextMenuStrip_ModifyChar.ShowItemToolTips = false;
            this.contextMenuStrip_ModifyChar.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_ModifyChar_Opening);
            this.contextMenuStrip_ModifyChar.Opened += new System.EventHandler(this.contextMenuStrip_ModifyChar_Opened);
            // 
            // ToolStripMenuItem_Any
            // 
            this.ToolStripMenuItem_Any.Name = "ToolStripMenuItem_Any";
            resources.ApplyResources(this.ToolStripMenuItem_Any, "ToolStripMenuItem_Any");
            this.ToolStripMenuItem_Any.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_Letter
            // 
            this.ToolStripMenuItem_Letter.Name = "ToolStripMenuItem_Letter";
            resources.ApplyResources(this.ToolStripMenuItem_Letter, "ToolStripMenuItem_Letter");
            this.ToolStripMenuItem_Letter.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_LetterUppercase
            // 
            this.ToolStripMenuItem_LetterUppercase.Name = "ToolStripMenuItem_LetterUppercase";
            resources.ApplyResources(this.ToolStripMenuItem_LetterUppercase, "ToolStripMenuItem_LetterUppercase");
            this.ToolStripMenuItem_LetterUppercase.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_LetterLowercase
            // 
            this.ToolStripMenuItem_LetterLowercase.Name = "ToolStripMenuItem_LetterLowercase";
            resources.ApplyResources(this.ToolStripMenuItem_LetterLowercase, "ToolStripMenuItem_LetterLowercase");
            this.ToolStripMenuItem_LetterLowercase.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_Digit
            // 
            this.ToolStripMenuItem_Digit.Name = "ToolStripMenuItem_Digit";
            resources.ApplyResources(this.ToolStripMenuItem_Digit, "ToolStripMenuItem_Digit");
            this.ToolStripMenuItem_Digit.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_Punctuation
            // 
            this.ToolStripMenuItem_Punctuation.Name = "ToolStripMenuItem_Punctuation";
            resources.ApplyResources(this.ToolStripMenuItem_Punctuation, "ToolStripMenuItem_Punctuation");
            this.ToolStripMenuItem_Punctuation.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // ToolStripMenuItem_Symbol
            // 
            this.ToolStripMenuItem_Symbol.Name = "ToolStripMenuItem_Symbol";
            resources.ApplyResources(this.ToolStripMenuItem_Symbol, "ToolStripMenuItem_Symbol");
            this.ToolStripMenuItem_Symbol.Click += new System.EventHandler(this.ToolStripMenuItem_CharType_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.ForeColor = System.Drawing.Color.Aquamarine;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.ForeColor = System.Drawing.Color.Red;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.Black;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox3, "textBox3");
            this.textBox3.ForeColor = System.Drawing.Color.LightGreen;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.Color.Black;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox4, "textBox4");
            this.textBox4.ForeColor = System.Drawing.Color.Yellow;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.Color.Black;
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox5, "textBox5");
            this.textBox5.ForeColor = System.Drawing.Color.Fuchsia;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.pnl_pic);
            this.panel1.Name = "panel1";
            // 
            // txt_CharMinWidth
            // 
            resources.ApplyResources(this.txt_CharMinWidth, "txt_CharMinWidth");
            this.txt_CharMinWidth.Name = "txt_CharMinWidth";
            this.txt_CharMinWidth.TextChanged += new System.EventHandler(this.txt_CharMinWidth_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // OCRSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_CharMinWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txt_CharHeight);
            this.Controls.Add(this.txt_CharMaxWidth);
            this.Controls.Add(this.lbl_CharHeight);
            this.Controls.Add(this.lbl_CharWidth);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Cancel);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OCRSettingForm";
            this.ShowIcon = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip_ModifyChar.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private System.Windows.Forms.Label lbl_CharWidth;
        private System.Windows.Forms.Label lbl_CharHeight;
        private System.Windows.Forms.TextBox txt_CharMaxWidth;
        private System.Windows.Forms.TextBox txt_CharHeight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_WordCount;
        private System.Windows.Forms.Label lbl_lineCount;
        private System.Windows.Forms.Label lbl_WordCount;
        private System.Windows.Forms.TextBox txt_LineCount;
        private System.Windows.Forms.ComboBox cbo_WordNo;
        private System.Windows.Forms.ComboBox cbo_LineNo;
        private System.Windows.Forms.TextBox txt_CharCount;
        private System.Windows.Forms.Label lbl_LineNo;
        private System.Windows.Forms.Label lbl_CharCount;
        private System.Windows.Forms.Label lbl_WordNo;
        private System.Windows.Forms.Panel pnl_pic;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_CharMinWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ModifyChar;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Any;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Letter;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_LetterUppercase;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_LetterLowercase;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Digit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Punctuation;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Symbol;
    }
}