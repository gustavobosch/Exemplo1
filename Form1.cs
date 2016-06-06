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

        public static string ApplicationName = "Broco di Nota";
        public static AnchorStyles AnchorAll = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        private List<FileTab> FileTabList;
        private FileTab CurrentFileTab;
        private int CurrentTabIndex;
        private int NewFileCount;

        public FormNotepad() {
            InitializeComponent();
            this.FileTabList = new List<FileTab>();
            this.CurrentFileTab = null;
            this.CurrentTabIndex = -1;
            this.NewFileCount = 0;
            this.Text = FormNotepad.FormName("", false);
            this.fecharToolStripMenuItem.Enabled = false;
        }

        private void menu_fonte_Click(object sender, EventArgs e) {
            dlgFonte.ShowDialog();
            this.CurrentFileTab.ContentEditor.Font = dlgFonte.Font;
        }

        private void menu_novo_Click(object sender, EventArgs e) {
            this.CurrentFileTab = NovaAba("", false);
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
        }

        private void menu_abrir_Click(object sender, EventArgs e) {
            dlgAbrir.ShowDialog();
            this.CurrentFileTab = NovaAba(dlgAbrir.FileName, false);
            this.CurrentFileTab.ContentEditor.Text = FormNotepad.CarregaArquivo(this.CurrentFileTab.FileName);
            this.CurrentFileTab.Modified = false;
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
            this.abas.SelectedTab.Text = FormNotepad.TabName(this.CurrentFileTab.DisplayName, this.CurrentFileTab.Modified);
        }

        private void menu_salvar_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(this.CurrentFileTab.FileName)) {
                menu_salvarcomo_Click(sender, e);
                return;
            }
            string texto = this.CurrentFileTab.ContentEditor.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileTab.FileName, texto);
            this.CurrentFileTab.Modified = false;
            this.CurrentFileTab.DisplayName = FormNotepad.DisplayName(this.CurrentFileTab.FileName);
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
            this.abas.SelectedTab.Text = FormNotepad.TabName(this.CurrentFileTab.DisplayName, this.CurrentFileTab.Modified);
        }

        private void menu_salvarcomo_Click(object sender, EventArgs e) {
            dlgSalvar.ShowDialog();
            this.CurrentFileTab.FileName = dlgSalvar.FileName;
            string texto = this.CurrentFileTab.ContentEditor.Text;
            FormNotepad.SalvaArquivo(this.CurrentFileTab.FileName, texto);
            this.CurrentFileTab.Modified = false;
            this.CurrentFileTab.DisplayName = FormNotepad.DisplayName(this.CurrentFileTab.FileName);
            this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
            this.abas.SelectedTab.Text = FormNotepad.TabName(this.CurrentFileTab.DisplayName, this.CurrentFileTab.Modified);
        }

        private void menu_fechar_Click(object sender, EventArgs e) {
            if (this.CurrentFileTab != null && this.CurrentFileTab.Modified) {
                switch (FormNotepad.ConfirmaFecharArquivo(this.CurrentFileTab.DisplayName)) {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        menu_salvar_Click(sender, e);
                        break;
                    default:
                        return;
                }
            }

            this.fecharToolStripMenuItem.Enabled = (this.abas.TabCount - 1 > 0);
            if (this.abas.TabCount <= 0) {
                this.CurrentTabIndex = -1;
                this.CurrentFileTab = null;
                return;
            }
            this.FileTabList.RemoveAt(this.abas.SelectedIndex);
            this.abas.TabPages.RemoveAt(this.abas.SelectedIndex);
            this.abas_SelectedIndexChanged(sender, e);
        }

        private void textBox_TextChanged(object sender, EventArgs e) {
            if (!this.CurrentFileTab.Modified) {
                this.CurrentFileTab.Modified = true;
                this.Text = FormNotepad.FormName(this.CurrentFileTab.FileName, this.CurrentFileTab.Modified);
                this.abas.SelectedTab.Text = FormNotepad.TabName(this.CurrentFileTab.DisplayName, this.CurrentFileTab.Modified);
            }
        }

        private void abas_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.abas.TabCount == 0) {
                this.CurrentTabIndex = -1;
                this.CurrentFileTab = null;
                return;
            }
            this.CurrentTabIndex = this.abas.SelectedIndex;
            this.CurrentFileTab = this.FileTabList[this.CurrentTabIndex];
        }

        private void menu_sair_Click(object sender, EventArgs e) {
            foreach (FileTab arquivo in this.FileTabList) {
                if (arquivo.Modified && DialogResult.Yes.Equals(FormNotepad.ConfirmaFecharArquivo(arquivo.DisplayName))) {
                    menu_salvar_Click(sender, e);
                }
            }
            Application.Exit();
        }

        private FileTab NovaAba(string filename, bool modified) {
            FileTab novo_arquivo = null;
            if (String.IsNullOrEmpty(filename)) {
                novo_arquivo = new FileTab(filename, "New File " + (++this.NewFileCount), modified);
            } else {
                novo_arquivo = new FileTab(filename, FormNotepad.DisplayName(filename), modified);
            }
            this.FileTabList.Add(novo_arquivo);

            TabPage tab_page = new TabPage(novo_arquivo.DisplayName);
            tab_page.Controls.Add(novo_arquivo.ContentEditor);

            this.abas.TabPages.Add(tab_page);
            novo_arquivo.ContentEditor.Size = this.abas.Size - new Size(8, 24);
            novo_arquivo.ContentEditor.TextChanged += this.textBox_TextChanged;

            this.abas.SelectedIndex = Math.Max(this.abas.TabCount - 1, 0);
            this.fecharToolStripMenuItem.Enabled = true;
            return novo_arquivo;
        }

        private static DialogResult ConfirmaFecharArquivo(string filename) {
            return MessageBox.Show("Deseja salvar o arquivo " + filename + "?", FormNotepad.ApplicationName, MessageBoxButtons.YesNoCancel);
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
            if (!String.IsNullOrEmpty(current_filename)) {
                str += current_filename + "  -  ";
            }
            return str + FormNotepad.ApplicationName;
        }

        private static string DisplayName(string filename) {
            return filename.Substring(filename.LastIndexOf("\\") + 1);
        }

        private static string TabName(string current_displayname, bool modified) {
            string str = "";
            if (!String.IsNullOrEmpty(current_displayname)) {
                str += current_displayname;
            }
            if (modified) {
                str += "*";
            }
            return str;
        }
    }

    public class FileTab {
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public bool Modified { get; set; }
        public TextBox ContentEditor { get; set; }

        public FileTab(string FileName, string DisplayName, bool Modified) {
            this.FileName = FileName;
            this.DisplayName = DisplayName;
            this.Modified = Modified;
            this.ContentEditor = new TextBox();
            this.ContentEditor.Anchor = FormNotepad.AnchorAll;
            this.ContentEditor.Multiline = true;
            this.ContentEditor.ScrollBars = ScrollBars.Vertical;
        }
    }
}
