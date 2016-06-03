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

        private static string ApplicationName = "Broco di Nota";

        private string CurrentFileName;
        private bool Modified;

        public FormNotepad() {
            InitializeComponent();
            this.CurrentFileName = "";
            this.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
        }

        private void menu_sair_Click(object sender, EventArgs e) {
            if (this.Modified) {
                switch (FormNotepad.ConfirmaFecharArquivo()) {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        menu_salvar_Click(sender, e);
                        break;
                    default:
                        return;
                }
            }
            Application.Exit();
        }

        private void menu_novo_Click(object sender, EventArgs e) {
            if (this.Modified) {
                switch (FormNotepad.ConfirmaFecharArquivo()) {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        menu_salvar_Click(sender, e);
                        break;
                    default:
                        return;
                }
            }
            edTexto.Clear();
            this.CurrentFileName = "";
            this.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
        }

        private void menu_fonte_Click(object sender, EventArgs e) {
            dlgFonte.ShowDialog();
            edTexto.Font = dlgFonte.Font;
        }

        private void menu_abrir_Click(object sender, EventArgs e) {
            if (this.Modified) {
                switch (FormNotepad.ConfirmaFecharArquivo()) {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        menu_salvar_Click(sender, e);
                        break;
                    default:
                        return;
                }
            }
            dlgAbrir.ShowDialog();
            this.CurrentFileName = dlgAbrir.FileName;
            edTexto.Text = FormNotepad.CarregaArquivo(this.CurrentFileName);
            this.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
        }

        private void menu_salvar_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(this.CurrentFileName)) {
                menu_salvarcomo_Click(sender, e);
                return;
            }
            string texto = edTexto.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileName, texto);
            this.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
        }

        private void menu_salvarcomo_Click(object sender, EventArgs e) {
            dlgSalvar.ShowDialog();
            this.CurrentFileName = dlgSalvar.FileName;
            string texto = edTexto.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileName, texto);
            this.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
        }

        private void edTexto_TextChanged(object sender, EventArgs e) {
            if (!this.Modified) {
                this.Modified = true;
                this.Text = FormNotepad.FormName(this.CurrentFileName, this.Modified);
            }
        }

        private static DialogResult ConfirmaFecharArquivo() {
            return MessageBox.Show("Deseja salvar o arquivo atual?", FormNotepad.ApplicationName, MessageBoxButtons.YesNoCancel);
        }

        private static string CarregaArquivo(string filename) {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fs, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }

        private static void SalvaArquivo(string filename, string texto) {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) {
                writer.Write(texto);
            }
        }

        private static string FormName(string current_filename, bool modified) {
            string str = "";
            if (modified) {
                str += "*";
            }
            if (!String.IsNullOrEmpty(current_filename)) {
                str += current_filename + "  -  ";
            }
            return str + FormNotepad.ApplicationName;
        }
    }
}
