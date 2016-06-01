using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exemplo1 {
    public partial class FormNotepad : Form {
        public FormNotepad() {
            InitializeComponent();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void novoToolStripMenuItem_Click(object sender, EventArgs e) {
            edTexto.Clear();
        }

        private void fonteToolStripMenuItem_Click(object sender, EventArgs e) {
            dlgFonte.ShowDialog();
            edTexto.Font = dlgFonte.Font;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e) {
            dlgAbrir.ShowDialog();
            string filename = dlgAbrir.FileName;

            edTexto.Text = FormNotepad.CarregaArquivo(filename);
        }

        private static string CarregaArquivo(string filename) {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fs, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
    }
}
