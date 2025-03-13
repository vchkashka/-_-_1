using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    public class Files
    {
        private TabControl tabControl;
        private Form1 mainForm;
        public Dictionary<RichTextBox, string> FilePaths { get; set; }

        public Files(TabControl tabControl, Form1 mainForm)
        {
            this.tabControl = tabControl;
            this.mainForm = mainForm;
            FilePaths = new Dictionary<RichTextBox, string>();
        }

        [STAThread]
        public void createFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Создать файл",
                Filter = "Текстовый файл (*.txt)|*.txt|Файл CSV (*.csv)|*.csv|Все файлы (*.*)|*.*",
                DefaultExt = "txt",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = "";
                    mainForm.AddNewTab(saveFileDialog.FileName, content);
                    File.WriteAllText(saveFileDialog.FileName, content);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void openFile()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Открыть файл",
                    Filter = "Текстовые файлы (*.txt)|*.txt|Файлы CSV (*.csv)|*.csv|Все файлы (*.*)|*.*"
                })
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        foreach (TabPage tab in tabControl.TabPages)
                        {
                            if (FilePaths.ContainsValue(filePath))
                            {
                                tabControl.SelectedTab = tab;
                                return;
                            }
                        }

                        string content = File.ReadAllText(filePath);
                        mainForm.AddNewTab(filePath, content);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void saveToFile(RichTextBox rtb)
        {
            try
            {
                if (FilePaths.TryGetValue(rtb, out string filePath) && File.Exists(filePath))
                {
                    File.WriteAllText(filePath, rtb.Text);
                    rtb.Modified = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void saveAs(RichTextBox rtb)
        {
            if (rtb == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить файл как",
                Filter = "Текстовый файл (*.txt)|*.txt|Файл CSV (*.csv)|*.csv|Файл docx (*.docx)|*.docx|Все файлы (*.*)|*.*",
                DefaultExt = "txt",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, rtb.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
