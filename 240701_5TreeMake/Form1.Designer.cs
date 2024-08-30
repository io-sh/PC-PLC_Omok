namespace _240701_5TreeMake
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            panel2 = new Panel();
            btnReady = new Button();
            button3 = new Button();
            tbTalk = new TextBox();
            lbChat = new ListBox();
            label2 = new Label();
            label1 = new Label();
            tbID = new TextBox();
            tbIP = new TextBox();
            btnConnect = new Button();
            menuStrip1 = new MenuStrip();
            파일ToolStripMenuItem = new ToolStripMenuItem();
            다시시작ToolStripMenuItem = new ToolStripMenuItem();
            복기ToolStripMenuItem = new ToolStripMenuItem();
            모드변경ToolStripMenuItem = new ToolStripMenuItem();
            싱글모드ToolStripMenuItem = new ToolStripMenuItem();
            멀티모드ToolStripMenuItem = new ToolStripMenuItem();
            서버생성ToolStripMenuItem = new ToolStripMenuItem();
            pLC연결ToolStripMenuItem = new ToolStripMenuItem();
            pLC연결해제ToolStripMenuItem = new ToolStripMenuItem();
            보기ToolStripMenuItem = new ToolStripMenuItem();
            이미지ToolStripMenuItem = new ToolStripMenuItem();
            그리기ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            수순표시ToolStripMenuItem = new ToolStripMenuItem();
            수순표시안함ToolStripMenuItem = new ToolStripMenuItem();
            tmrUpdate = new System.Windows.Forms.Timer(components);
            에러리셋ToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 620F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 600F));
            tableLayoutPanel1.Controls.Add(panel1, 1, 1);
            tableLayoutPanel1.Controls.Add(panel2, 2, 1);
            tableLayoutPanel1.Controls.Add(menuStrip1, 1, 0);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Margin = new Padding(4, 5, 4, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 620F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new Size(1213, 693);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(34, 35);
            panel1.Margin = new Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(612, 610);
            panel1.TabIndex = 0;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnReady);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(tbTalk);
            panel2.Controls.Add(lbChat);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(tbID);
            panel2.Controls.Add(tbIP);
            panel2.Controls.Add(btnConnect);
            panel2.Location = new Point(654, 35);
            panel2.Margin = new Padding(4, 5, 4, 5);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(0, 15, 0, 0);
            panel2.Size = new Size(559, 610);
            panel2.TabIndex = 1;
            // 
            // btnReady
            // 
            btnReady.Location = new Point(425, 35);
            btnReady.Name = "btnReady";
            btnReady.Size = new Size(112, 72);
            btnReady.TabIndex = 10;
            btnReady.Text = "준비";
            btnReady.UseVisualStyleBackColor = true;
            btnReady.Click += btnReady_Click;
            // 
            // button3
            // 
            button3.Location = new Point(485, 544);
            button3.Margin = new Padding(4, 5, 4, 5);
            button3.Name = "button3";
            button3.Size = new Size(70, 38);
            button3.TabIndex = 9;
            button3.Text = "전송";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // tbTalk
            // 
            tbTalk.Location = new Point(4, 551);
            tbTalk.Margin = new Padding(4, 5, 4, 5);
            tbTalk.Name = "tbTalk";
            tbTalk.Size = new Size(468, 31);
            tbTalk.TabIndex = 8;
            // 
            // lbChat
            // 
            lbChat.FormattingEnabled = true;
            lbChat.ItemHeight = 25;
            lbChat.Location = new Point(4, 164);
            lbChat.Margin = new Padding(4, 5, 4, 5);
            lbChat.Name = "lbChat";
            lbChat.Size = new Size(547, 354);
            lbChat.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 82);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(66, 25);
            label2.TabIndex = 2;
            label2.Text = "아이디";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 35);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(27, 25);
            label1.TabIndex = 2;
            label1.Text = "IP";
            // 
            // tbID
            // 
            tbID.Location = new Point(90, 76);
            tbID.Margin = new Padding(4, 5, 4, 5);
            tbID.Name = "tbID";
            tbID.Size = new Size(208, 31);
            tbID.TabIndex = 1;
            tbID.Text = "춘식이";
            // 
            // tbIP
            // 
            tbIP.Location = new Point(90, 35);
            tbIP.Margin = new Padding(4, 5, 4, 5);
            tbIP.Name = "tbIP";
            tbIP.Size = new Size(208, 31);
            tbIP.TabIndex = 1;
            tbIP.Text = "127.0.0.1";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(306, 35);
            btnConnect.Margin = new Padding(4, 5, 4, 5);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(112, 72);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "연결";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 파일ToolStripMenuItem, 보기ToolStripMenuItem });
            menuStrip1.Location = new Point(30, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(620, 30);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            파일ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 다시시작ToolStripMenuItem, 복기ToolStripMenuItem, 모드변경ToolStripMenuItem, 서버생성ToolStripMenuItem, pLC연결ToolStripMenuItem, pLC연결해제ToolStripMenuItem, 에러리셋ToolStripMenuItem });
            파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            파일ToolStripMenuItem.Size = new Size(64, 26);
            파일ToolStripMenuItem.Text = "파일";
            // 
            // 다시시작ToolStripMenuItem
            // 
            다시시작ToolStripMenuItem.Name = "다시시작ToolStripMenuItem";
            다시시작ToolStripMenuItem.Size = new Size(270, 34);
            다시시작ToolStripMenuItem.Text = "다시시작";
            다시시작ToolStripMenuItem.Click += 다시시작ToolStripMenuItem_Click;
            // 
            // 복기ToolStripMenuItem
            // 
            복기ToolStripMenuItem.Name = "복기ToolStripMenuItem";
            복기ToolStripMenuItem.Size = new Size(270, 34);
            복기ToolStripMenuItem.Text = "복기";
            복기ToolStripMenuItem.Click += 복기ToolStripMenuItem_Click;
            // 
            // 모드변경ToolStripMenuItem
            // 
            모드변경ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 싱글모드ToolStripMenuItem, 멀티모드ToolStripMenuItem });
            모드변경ToolStripMenuItem.Name = "모드변경ToolStripMenuItem";
            모드변경ToolStripMenuItem.Size = new Size(270, 34);
            모드변경ToolStripMenuItem.Text = "모드 변경";
            // 
            // 싱글모드ToolStripMenuItem
            // 
            싱글모드ToolStripMenuItem.Checked = true;
            싱글모드ToolStripMenuItem.CheckState = CheckState.Checked;
            싱글모드ToolStripMenuItem.Name = "싱글모드ToolStripMenuItem";
            싱글모드ToolStripMenuItem.Size = new Size(192, 34);
            싱글모드ToolStripMenuItem.Text = "싱글 모드";
            싱글모드ToolStripMenuItem.Click += 싱글모드ToolStripMenuItem_Click;
            // 
            // 멀티모드ToolStripMenuItem
            // 
            멀티모드ToolStripMenuItem.Name = "멀티모드ToolStripMenuItem";
            멀티모드ToolStripMenuItem.Size = new Size(192, 34);
            멀티모드ToolStripMenuItem.Text = "멀티 모드";
            멀티모드ToolStripMenuItem.Click += 멀티모드ToolStripMenuItem_Click;
            // 
            // 서버생성ToolStripMenuItem
            // 
            서버생성ToolStripMenuItem.Name = "서버생성ToolStripMenuItem";
            서버생성ToolStripMenuItem.Size = new Size(270, 34);
            서버생성ToolStripMenuItem.Text = "서버 생성";
            서버생성ToolStripMenuItem.Click += 서버생성ToolStripMenuItem_Click;
            // 
            // pLC연결ToolStripMenuItem
            // 
            pLC연결ToolStripMenuItem.Name = "pLC연결ToolStripMenuItem";
            pLC연결ToolStripMenuItem.Size = new Size(270, 34);
            pLC연결ToolStripMenuItem.Text = "PLC연결";
            pLC연결ToolStripMenuItem.Click += pLC연결ToolStripMenuItem_Click;
            // 
            // pLC연결해제ToolStripMenuItem
            // 
            pLC연결해제ToolStripMenuItem.Name = "pLC연결해제ToolStripMenuItem";
            pLC연결해제ToolStripMenuItem.Size = new Size(270, 34);
            pLC연결해제ToolStripMenuItem.Text = "PLC연결 해제";
            pLC연결해제ToolStripMenuItem.Click += pLC연결해제ToolStripMenuItem_Click;
            // 
            // 보기ToolStripMenuItem
            // 
            보기ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 이미지ToolStripMenuItem, 그리기ToolStripMenuItem, toolStripMenuItem1 });
            보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            보기ToolStripMenuItem.Size = new Size(64, 26);
            보기ToolStripMenuItem.Text = "보기";
            // 
            // 이미지ToolStripMenuItem
            // 
            이미지ToolStripMenuItem.Checked = true;
            이미지ToolStripMenuItem.CheckState = CheckState.Checked;
            이미지ToolStripMenuItem.Name = "이미지ToolStripMenuItem";
            이미지ToolStripMenuItem.Size = new Size(168, 34);
            이미지ToolStripMenuItem.Text = "이미지";
            이미지ToolStripMenuItem.Click += 이미지ToolStripMenuItem_Click;
            // 
            // 그리기ToolStripMenuItem
            // 
            그리기ToolStripMenuItem.Name = "그리기ToolStripMenuItem";
            그리기ToolStripMenuItem.Size = new Size(168, 34);
            그리기ToolStripMenuItem.Text = "그리기";
            그리기ToolStripMenuItem.Click += 그리기ToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { 수순표시ToolStripMenuItem, 수순표시안함ToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(168, 34);
            toolStripMenuItem1.Text = "수순";
            // 
            // 수순표시ToolStripMenuItem
            // 
            수순표시ToolStripMenuItem.Checked = true;
            수순표시ToolStripMenuItem.CheckState = CheckState.Checked;
            수순표시ToolStripMenuItem.Name = "수순표시ToolStripMenuItem";
            수순표시ToolStripMenuItem.Size = new Size(228, 34);
            수순표시ToolStripMenuItem.Text = "수순 표시";
            수순표시ToolStripMenuItem.Click += 수순표시ToolStripMenuItem_Click;
            // 
            // 수순표시안함ToolStripMenuItem
            // 
            수순표시안함ToolStripMenuItem.Name = "수순표시안함ToolStripMenuItem";
            수순표시안함ToolStripMenuItem.Size = new Size(228, 34);
            수순표시안함ToolStripMenuItem.Text = "수순 표시안함";
            수순표시안함ToolStripMenuItem.Click += 수순표시안함ToolStripMenuItem_Click;
            // 
            // tmrUpdate
            // 
            tmrUpdate.Tick += tmrUpdate_Tick;
            // 
            // 에러리셋ToolStripMenuItem
            // 
            에러리셋ToolStripMenuItem.Name = "에러리셋ToolStripMenuItem";
            에러리셋ToolStripMenuItem.Size = new Size(270, 34);
            에러리셋ToolStripMenuItem.Text = "에러리셋";
            에러리셋ToolStripMenuItem.Click += 에러리셋ToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1220, 703);
            Controls.Add(tableLayoutPanel1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "오목";
            FormClosing += Form1_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private TextBox tbIP;
        private Button btnConnect;
        private Button button3;
        private TextBox tbTalk;
        private ListBox lbChat;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 파일ToolStripMenuItem;
        private ToolStripMenuItem 보기ToolStripMenuItem;
        private ToolStripMenuItem 이미지ToolStripMenuItem;
        private ToolStripMenuItem 그리기ToolStripMenuItem;
        private Label label2;
        private TextBox tbID;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem 수순표시ToolStripMenuItem;
        private ToolStripMenuItem 수순표시안함ToolStripMenuItem;
        private ToolStripMenuItem 복기ToolStripMenuItem;
        private ToolStripMenuItem 다시시작ToolStripMenuItem;
        private ToolStripMenuItem 모드변경ToolStripMenuItem;
        private ToolStripMenuItem 싱글모드ToolStripMenuItem;
        private ToolStripMenuItem 멀티모드ToolStripMenuItem;
        private ToolStripMenuItem 서버생성ToolStripMenuItem;
        private Button btnReady;
        private ToolStripMenuItem pLC연결ToolStripMenuItem;
        private System.Windows.Forms.Timer tmrUpdate;
        private ToolStripMenuItem pLC연결해제ToolStripMenuItem;
        private ToolStripMenuItem 에러리셋ToolStripMenuItem;
    }
}
