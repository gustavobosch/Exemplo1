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

        private FileTab CurrentFileTab;

        public FormNotepad() {
            InitializeComponent();
            this.CurrentFileTab = null;
            this.Text = FormNotepad.FormName("", false);
        }

        private void menu_sair_Click(object sender, EventArgs e) {
            if (this.CurrentFileTab.Modified) {
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
            if (this.CurrentFileTab.Modified) {
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
            this.CurrentFileTab.ContentEditor.Clear();
            this.CurrentFileTab.FileName = "";
            this.CurrentFileTab.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
        }

        private void menu_fonte_Click(object sender, EventArgs e) {
            dlgFonte.ShowDialog();
            this.CurrentFileTab.ContentEditor.Font = dlgFonte.Font;
        }

        private void menu_abrir_Click(object sender, EventArgs e) {
            if (this.CurrentFileTab.Modified) {
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
            this.CurrentFileTab.FileName = dlgAbrir.FileName;
            this.CurrentFileTab.ContentEditor.Text = FormNotepad.CarregaArquivo(this.CurrentFileTab.FileName);
            this.CurrentFileTab.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
        }

        private void menu_salvar_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(this.CurrentFileTab.FileName)) {
                menu_salvarcomo_Click(sender, e);
                return;
            }
            string texto = this.CurrentFileTab.ContentEditor.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileTab.FileName, texto);
            this.CurrentFileTab.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
        }

        private void menu_salvarcomo_Click(object sender, EventArgs e) {
            dlgSalvar.ShowDialog();
            this.CurrentFileTab.FileName = dlgSalvar.FileName;
            string texto = this.CurrentFileTab.ContentEditor.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileTab.FileName, texto);
            this.CurrentFileTab.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
        }

        private void textBox_TextChanged(object sender, EventArgs e) {
            if (!this.CurrentFileTab.Modified) {
                this.CurrentFileTab.Modified = true;
                this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
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

    public class FileTab {
        public string FileName { get; set; }
        public bool Modified { get; set; }
        public TextBox ContentEditor { get; set; }

        public FileTab(string FileName, bool Modified) {
            this.FileName = FileName;
            this.Modified = Modified;
            this.ContentEditor = new TextBox();
        }
    }
}
