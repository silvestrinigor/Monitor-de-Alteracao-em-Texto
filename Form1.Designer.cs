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
            SelectFileButton = new Button();
            FilePathTextBox = new TextBox();
            FileChangeHistoryTextBox = new RichTextBox();
            label1 = new Label();
            TimerToVerifyFile = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            LogsTextBox = new RichTextBox();
            SuspendLayout();
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
            // FilePathTextBox
            // 
            FilePathTextBox.Location = new Point(134, 13);
            FilePathTextBox.Name = "FilePathTextBox";
            FilePathTextBox.ReadOnly = true;
            FilePathTextBox.Size = new Size(654, 23);
            FilePathTextBox.TabIndex = 1;
            // 
            // FileChangeHistory
            // 
            FileChangeHistoryTextBox.Location = new Point(134, 42);
            FileChangeHistoryTextBox.Name = "FileChangeHistory";
            FileChangeHistoryTextBox.ReadOnly = true;
            FileChangeHistoryTextBox.Size = new Size(654, 247);
            FileChangeHistoryTextBox.TabIndex = 2;
            FileChangeHistoryTextBox.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(66, 45);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 3;
            label1.Text = "Alteracoes";
            // 
            // TimerToVerifyFile
            // 
            TimerToVerifyFile.Enabled = true;
            TimerToVerifyFile.Interval = 5000;
            TimerToVerifyFile.Tick += OnTimerToVerifyFileTick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(96, 295);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 4;
            label2.Text = "Logs";
            // 
            // LogsTextBox
            // 
            LogsTextBox.Location = new Point(134, 295);
            LogsTextBox.Name = "LogsTextBox";
            LogsTextBox.ReadOnly = true;
            LogsTextBox.Size = new Size(654, 143);
            LogsTextBox.TabIndex = 5;
            LogsTextBox.Text = "";
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(LogsTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(FileChangeHistoryTextBox);
            Controls.Add(FilePathTextBox);
            Controls.Add(SelectFileButton);
            Name = "Form";
            Text = "Monitor de Alteração em Texto";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SelectFileButton;
        private TextBox FilePathTextBox;
        private RichTextBox FileChangeHistoryTextBox;
        private Label label1;
        private System.Windows.Forms.Timer TimerToVerifyFile;
        private Label label2;
        private RichTextBox LogsTextBox;
    }
}
