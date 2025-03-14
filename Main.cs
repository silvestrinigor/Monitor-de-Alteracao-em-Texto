namespace Monitor_de_Alteração_em_Texto
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt"; // Filtra apenas arquivos .txt
            openFileDialog.Title = "Selecione um arquivo de texto"; // Título da janela
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Diretório inicial

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                textBox1.Text = filePath;
            }
        }
    }
}
