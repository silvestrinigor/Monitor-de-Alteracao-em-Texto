namespace Monitor_de_Alteração_em_Texto
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
            FileChangeHistoryTextBox = new RichTextBox();
            TimerToVerifyFile = new System.Windows.Forms.Timer(components);
            LogsTextBox = new RichTextBox();
            button1 = new Button();
            label2 = new Label();
            label1 = new Label();
            SelectFileButton = new Button();
            panel1 = new Panel();
            FilePathTextBox = new TextBox();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // FileChangeHistoryTextBox
            // 
            FileChangeHistoryTextBox.Location = new Point(134, 56);
            FileChangeHistoryTextBox.Name = "FileChangeHistoryTextBox";
            FileChangeHistoryTextBox.ReadOnly = true;
            FileChangeHistoryTextBox.Size = new Size(654, 211);
            FileChangeHistoryTextBox.TabIndex = 2;
            FileChangeHistoryTextBox.Text = "";
            // 
            // TimerToVerifyFile
            // 
            TimerToVerifyFile.Enabled = true;
            TimerToVerifyFile.Interval = 30000;
            TimerToVerifyFile.Tick += OnTimerToVerifyFileTick;
            // 
            // LogsTextBox
            // 
            LogsTextBox.Location = new Point(134, 276);
            LogsTextBox.Name = "LogsTextBox";
            LogsTextBox.ReadOnly = true;
            LogsTextBox.Size = new Size(651, 128);
            LogsTextBox.TabIndex = 5;
            LogsTextBox.Text = "";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(687, 6);
            button1.Name = "button1";
            button1.Size = new Size(98, 23);
            button1.TabIndex = 6;
            button1.Text = "Deletar tabela";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnCleanTableButtonClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(96, 276);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 4;
            label2.Text = "Logs";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(66, 3);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 3;
            label1.Text = "Alteracoes";
            label1.Click += label1_Click;
            // 
            // SelectFileButton
            // 
            SelectFileButton.Location = new Point(12, 12);
            SelectFileButton.Name = "SelectFileButton";
            SelectFileButton.Size = new Size(116, 23);
            SelectFileButton.TabIndex = 0;
            SelectFileButton.Text = "Selecione arquivo";
            SelectFileButton.UseVisualStyleBackColor = true;
            SelectFileButton.Click += OnSelectFileButtonClick;
            // 
            // panel1
            // 
            panel1.Controls.Add(FilePathTextBox);
            panel1.Controls.Add(SelectFileButton);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 47);
            panel1.TabIndex = 11;
            // 
            // FilePathTextBox
            // 
            FilePathTextBox.Location = new Point(134, 12);
            FilePathTextBox.Name = "FilePathTextBox";
            FilePathTextBox.ReadOnly = true;
            FilePathTextBox.Size = new Size(654, 23);
            FilePathTextBox.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Location = new Point(0, 53);
            panel2.Name = "panel2";
            panel2.Size = new Size(800, 217);
            panel2.TabIndex = 12;
            // 
            // panel3
            // 
            panel3.Location = new Point(0, 273);
            panel3.Name = "panel3";
            panel3.Size = new Size(800, 137);
            panel3.TabIndex = 13;
            // 
            // panel4
            // 
            panel4.Controls.Add(button1);
            panel4.Location = new Point(0, 410);
            panel4.Name = "panel4";
            panel4.Size = new Size(800, 41);
            panel4.TabIndex = 14;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(FileChangeHistoryTextBox);
            Controls.Add(LogsTextBox);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(panel3);
            Controls.Add(panel4);
            MinimumSize = new Size(816, 489);
            Name = "Form1";
            Text = "Monitor de Alteração em Texto";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox FileChangeHistoryTextBox;
        private System.Windows.Forms.Timer TimerToVerifyFile;
        private RichTextBox LogsTextBox;
        private Button button1;
        private Label label2;
        private Label label1;
        private Button SelectFileButton;
        private Panel panel1;
        private TextBox FilePathTextBox;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
    }
}
