using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace Monitor_de_Alteração_em_Texto
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public TextFileInfo? TextFileInfo { get; set; }
        public DataManager DataAccess { get; set; } = new DataManager();

        public Form1()
        {
            InitializeComponent();
        }

        private void OnSelectFileButtonClick(object sender, EventArgs e)
        {
            AddLogInfoInTextBox("Botao de selecionar arquivo pressionado", "DEBUG");

            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
            openFileDialog.Title = "Selecione um arquivo de texto";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddLogInfoInTextBox("Arquivo selecionado com sucesso", "INFO");

                FilePathTextBox.Text = openFileDialog.FileName;

                var textFileInfoId = DataAccess.GetTextFileInfoIdByPath(openFileDialog.FileName);
                if (textFileInfoId == null)
                {
                    TextFileInfo = DataAccess.CreateTextFileInfo(openFileDialog.FileName);
                    AddLogInfoInTextBox($"Criado e carregado informacoes em banco do arquivo {openFileDialog.FileName}", "DEBUG");
                }
                else
                {
                    TextFileInfo = DataAccess.GetTextFileInfo(textFileInfoId.Value);
                    AddLogInfoInTextBox($"Carregado informacoes em banco do arquivo {openFileDialog.FileName}", "DEBUG");
                }
                UpdateFileContentInTextBoxAndData();
            }
            else
            {
                AddLogInfoInTextBox("Nenhum arquivo selecionado", "INFO");
            }
        }

        private void OnTimerToVerifyFileTick(object sender, EventArgs e)
        {
            UpdateFileContentInTextBoxAndData();
        }

        private void UpdateFileContentInTextBoxAndData()
        {
            if (TextFileInfo == null)
            {
                return;
            }

            string updatedFileContent = File.ReadAllText(FilePathTextBox.Text);

            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(TextFileInfo.GetContent(), updatedFileContent, true);

            FileChangeHistoryTextBox.BeginInvoke(new Action(() =>
            {
                if (!diff.HasDifferences)
                {
                    AddLogInfoInTextBox("Nenhuma alteração encontrada", "INFO");
                    return;
                }

                FileChangeHistoryTextBox.Clear();

                var linesInUse = new HashSet<int>();
                int lastPosition = 0;
                foreach (var line in diff.Lines)
                {
                    if (line.Position != null)
                    {
                        linesInUse.Add(line.Position.Value);
                    }
                    TextFileLineInfo? textFileLineInfo;
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
                                lastPosition = line.Position.Value;
                                textFileLineInfo = TextFileInfo.GetTextLineInfoByLineNumber(line.Position.Value);
                                if (textFileLineInfo == null)
                                {
                                    textFileLineInfo = TextFileInfo.CreateTextLineInfo(line.Position.Value, line.Text);
                                    AddLogInfoInTextBox("Criado uma informacao de linha de texto no banco", "DEBUG");
                                }
                                else
                                {
                                    textFileLineInfo.Content = line.Text;
                                    AddLogInfoInTextBox("Setado uma informacao de linha de texto no banco", "DEBUG");

                                }

                            }
                            else
                            {
                                throw new Exception("Posição da linha não pode ser nula");
                            }
                            break;

                        case ChangeType.Deleted:
                            newLineChangeInfo = $" - {line.Text}\n"; // Add +1 for line numbering
                            FileChangeHistoryTextBox.AppendText(newLineChangeInfo);
                            FileChangeHistoryTextBox.SelectionStart = FileChangeHistoryTextBox.Text.Length - newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionLength = newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionColor = Color.Red;
                            textFileLineInfo = TextFileInfo.GetTextLineInfoByLineNumber(lastPosition + 1);
                            lastPosition++;
                            if (textFileLineInfo != null)
                            {
                                AddLogInfoInTextBox($"Deletando linha {textFileLineInfo.LineNumber}", "DEBUG");
                                textFileLineInfo.DeleteLine();
                            }
                            break;

                        default:
                            newLineChangeInfo = $"{line.Position} {line.Text}\n";
                            FileChangeHistoryTextBox.AppendText(newLineChangeInfo);
                            FileChangeHistoryTextBox.SelectionStart = FileChangeHistoryTextBox.Text.Length - newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionLength = newLineChangeInfo.Length;
                            FileChangeHistoryTextBox.SelectionColor = Color.Black;
                            if (line.Position != null)
                            {
                                lastPosition = line.Position.Value;
                                textFileLineInfo = TextFileInfo.GetTextLineInfoByLineNumber(line.Position.Value);
                                if (textFileLineInfo == null)
                                {
                                    textFileLineInfo = TextFileInfo.CreateTextLineInfo(line.Position.Value, line.Text);
                                    AddLogInfoInTextBox("Criado uma informacao de linha de texto no banco", "DEBUG");
                                }
                                else
                                {
                                    if (textFileLineInfo.Content != line.Text)
                                    {
                                        textFileLineInfo.Content = line.Text;
                                        AddLogInfoInTextBox("Setado uma informacao de linha de texto no banco", "DEBUG");
                                    }
                                }

                            }
                            else
                            {
                                throw new Exception("Posição da linha não pode ser nula");
                            }
                            break;
                    }
                    // Reset the selection to avoid highlighting
                    FileChangeHistoryTextBox.SelectionLength = 0;
                }
                TextFileInfo.LoadInfos();
                if (TextFileInfo.Lines != null)
                {
                    foreach (var lineInfo in TextFileInfo.Lines)
                    {
                        if (!linesInUse.Contains(lineInfo.LineNumber))
                        {
                            // If, for some reasong, the line was not deleted, force it
                            AddLogInfoInTextBox($"Deletando linha {lineInfo.LineNumber}", "DEBUG");
                            lineInfo.DeleteLine();
                        }
                    }
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

        private void OnCleanTableButtonClick(object sender, EventArgs e)
        {
            if (TextFileInfo != null)
            {
                FileChangeHistoryTextBox.Clear();
                TextFileInfo.DeleteTextFileInfo();
                FilePathTextBox.Text = "";
                TextFileInfo = null;
                AddLogInfoInTextBox("Todas as tabelas referente ao arquivo foram deletadas", "DEBUG");
            }
        }
    }
}
