using System.IO;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace Monitor_de_Alteração_em_Texto
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public TextFileInfo? TextFileInfo { get; set; }
        public DatabaseManager DataAccess { get; set; } = new DatabaseManager();

        public Form1()
        {
            InitializeComponent();
        }

        private void OnSelectFileButtonClick(object sender, EventArgs e)
        {
            AddLogInfoInTextBox("Botao de selecionar arquivo precionado", "DEBUG");

            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
            openFileDialog.Title = "Selecione um arquivo de texto";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
                AddLogInfoInTextBox("Arquivo selecionado com sucesso", "INFO");

                var textFileInfoId = DataAccess.GetTextFileInfoIdByPath(openFileDialog.FileName);
                if(textFileInfoId == null)
                {
                    TextFileInfo = DataAccess.CreateTextFileInfo(openFileDialog.FileName);
                    AddLogInfoInTextBox($"Criado e carregado informacoes em banco do arquivo {openFileDialog.FileName}", "DEBUG");
                }
                else
                {
                    TextFileInfo = DataAccess.GetTextFileInfo(textFileInfoId.Value);
                    AddLogInfoInTextBox($"Carregado informacoes em banco do arquivo {openFileDialog.FileName}", "DEBUG");
                }
            }
            else
            {
                AddLogInfoInTextBox("Nenhum arquivo selecionado", "INFO");
            }
        }

        private void OnTimerToVerifyFileTick(object sender, EventArgs e)
        {
            if (TextFileInfo == null)
            {
                return;
            }

            TextFileInfo.LoadLinesFromTextFileInfo();

            string updatedFileContent = File.ReadAllText(FilePathTextBox.Text);

            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(TextFileInfo.GetContent(), updatedFileContent);
            
            FileChangeHistoryTextBox.BeginInvoke(new Action(() =>
            {
                if(!diff.HasDifferences)
                {
                    AddLogInfoInTextBox("Nenhuma alteração encontrada", "INFO");
                    return;
                }

                FileChangeHistoryTextBox.Clear();

                var linesInUse = new HashSet<int>();

                foreach (var line in diff.Lines)
                {
                    if(line.Position != null)
                    {
                        linesInUse.Add(line.Position.Value);
                        AddLogInfoInTextBox($"Linha em uso: {line.Position.Value}", "DEBUG");
                    }

                    string newLineChangeInfo;
                    switch (line.Type)
                    {
                        case ChangeType.Inserted:
                            newLineChangeInfo = $"{line.Position} + {line.Text}\n";
                            FileChangeHistoryTextBox.AppendText(newLineChangeInfo);
                            FileChangeHistoryTextBox.SelectionStart = FileChangeHistoryTextBox.Text.Length - newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionLength = newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionColor = Color.Green;

                            if (line.Position != null)
                            {
                                var textFileLineInfo = TextFileInfo.GetTextLineInfoByLineNumber(line.Position.Value);
                                if(textFileLineInfo == null)
                                {
                                    textFileLineInfo = TextFileInfo.CreateTextLineInfo(line.Position.Value, line.Text);
                                    AddLogInfoInTextBox("Criado uma informacao de linha de texto no banco", "DEBUG");
                                }
                                else
                                {
                                    textFileLineInfo.Content = line.Text;

                                }

                            }
                            break;

                        case ChangeType.Deleted:
                            newLineChangeInfo = $"- {line.Text}\n"; // Add +1 for line numbering
                            FileChangeHistoryTextBox.AppendText(newLineChangeInfo);
                            FileChangeHistoryTextBox.SelectionStart = FileChangeHistoryTextBox.Text.Length - newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionLength = newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionColor = Color.Red;
                            break;

                        default:
                            newLineChangeInfo = $"{line.Position} = {line.Text}\n";
                            FileChangeHistoryTextBox.AppendText(newLineChangeInfo);
                            FileChangeHistoryTextBox.SelectionStart = FileChangeHistoryTextBox.Text.Length - newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionLength = newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionColor = Color.Black;
                            if (line.Position != null)
                            {
                                var textFileLineInfo = TextFileInfo.GetTextLineInfoByLineNumber(line.Position.Value);
                                if (textFileLineInfo == null)
                                {
                                    textFileLineInfo = TextFileInfo.CreateTextLineInfo(line.Position.Value, line.Text);
                                }
                                else
                                {
                                    textFileLineInfo.Content = line.Text;
                                }

                            }
                            break;
                    }

                    foreach(var oldLine in TextFileInfo.Lines)
                    {
                        if (!linesInUse.Contains(oldLine.LineNumber))
                        {
                            //oldLine.DeleteLine();
                            AddLogInfoInTextBox($"Linha deletada do banco: {oldLine.LineNumber}, {oldLine.Content}");
                        }
                        
                    }

                    // Reset the selection to avoid highlighting
                    FileChangeHistoryTextBox.SelectionLength = 0;
                    TextFileInfo.LoadLinesFromTextFileInfo();
                    //FileChangeHistoryTextBox.Text = TextFileInfo.GetContent();
                }
            }));
            AddLogInfoInTextBox("Verificação de alterações realizada com sucesso", "INFO");
        }

        private void AddLogInfoInTextBox(string logInfo, string severity = "INFO")
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedLog = $"[{timestamp}] [{severity}] {logInfo}";

            LogsTextBox.BeginInvoke(new Action(() =>
            {
                LogsTextBox.AppendText(formattedLog + Environment.NewLine);
            }));
        }
    }
}
