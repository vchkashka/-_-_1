using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    public partial class Form1 : Form
    {
        private Files fileManager;
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private bool isTextChangedByUndoRedo = false;
        //Lexer lexer = new Lexer();

        public Form1()
        {
            InitializeComponent();
            this.Height = menuStrip1.Height + toolStrip1.Height + splitContainer1.Height;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer() { Interval = 1000 };
            timer.Tick += Timer_Tick;
            timer.Start();

            this.KeyPreview = true;
            splitContainer1.Dock = DockStyle.Fill;

            this.DragEnter += Form_DragEnter;
            this.DragDrop += Form_DragDrop;

            AppSettings.ApplyFontSizeToControls(this.Controls);

            fileManager = new Files(tabControl1, this);

            label1.Hide();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel5.Text = DateTime.Now.ToLongDateString();
            toolStripStatusLabel6.Text = DateTime.Now.ToLongTimeString();
        }

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string filePath = files[0];
                try
                {
                    foreach (TabPage tab in tabControl1.TabPages)
                    {
                        if (fileManager.FilePaths.ContainsValue(filePath))
                        {
                            tabControl1.SelectedTab = tab;
                            return;
                        }
                    }

                    string content = File.ReadAllText(filePath);
                    AddNewTab(filePath, content);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            fileManager.createFile();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileManager.createFile();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            fileManager.openFile();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileManager.openFile();
        }

        public RichTextBox GetActiveRichTextBox()
        {
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Controls.Count > 0)
            {
                return tabControl1.SelectedTab.Controls[0] as RichTextBox;
            }
            return null;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb == null) return;
            fileManager.saveToFile(rtb);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb == null) return;
            fileManager.saveToFile(rtb);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileManager.saveAs(GetActiveRichTextBox());
        }

        private bool ExitFromProgram()
        {
            if (tabControl1.TabPages.Count > 0)
            {
                foreach (TabPage tab in tabControl1.TabPages)
                    if (tab.Controls.Count > 0 && tab.Controls[0] is RichTextBox rtb && rtb.Modified)
                    {
                        DialogResult result = DialogResult.None;
                        if (toolStripMenuItem1.Text == "Английский язык")
                            result = MessageBox.Show($"Вы хотите сохранить изменения в файле {tab.Text}?", "Внимание!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        else
                            result = MessageBox.Show($"Do you want to save the changes to the {tab.Text} file?", "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                        if (result == DialogResult.Cancel)
                            return false;

                        if (result == DialogResult.No)
                            continue;

                        if (result == DialogResult.Yes)
                        {
                            fileManager.saveToFile(rtb);
                        }
                    }
            }
            return true;
        }

        private void ExitFromFile()
        {
            if (tabControl1.SelectedTab != null)
            {
                TabPage tab = tabControl1.SelectedTab;
                if (tab.Controls[0] is RichTextBox rtb)
                    if (rtb.Modified)
                    {
                        DialogResult result = DialogResult.None;
                        if (toolStripMenuItem1.Text == "Английский язык")
                            result = MessageBox.Show($"Вы хотите сохранить изменения в файле {tab.Text}?", "Внимание!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        else
                            result = MessageBox.Show($"Do you want to save the changes to the {tab.Text} file?", "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);



                        if (result == DialogResult.Yes)
                        {
                            fileManager.saveToFile(rtb);
                            fileManager.FilePaths.Remove(rtb);
                            tabControl1.TabPages.Remove(tab);
                        }
                        else if (result == DialogResult.No)
                        {
                            fileManager.FilePaths.Remove(rtb);
                            tabControl1.TabPages.Remove(tab);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        DialogResult result = DialogResult.None;
                        if (toolStripMenuItem1.Text == "Английский язык")
                            result = MessageBox.Show($"Вы хотите закрыть файл {tab.Text}?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        else
                            result = MessageBox.Show($"Do you want to close the {tab.Text} file?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            fileManager.FilePaths.Remove(rtb);
                            tabControl1.TabPages.Remove(tab);
                        }
                        else
                        {
                            return;
                        }
                    }
                dataGridView1.Rows.Clear();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitFromFile();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool canClose = ExitFromProgram();
            if (!canClose)
                e.Cancel = true;
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetActiveRichTextBox().SelectedText.Length > 0)
            {
                //undoStack.Push(GetActiveRichTextBox().Text);
                GetActiveRichTextBox().SelectedText = "";
                
                //redoStack.Clear();
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Cut();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Copy();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Copy();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Paste();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.Paste();
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetActiveRichTextBox()?.SelectAll();
        }

        private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                toolStripButton18_Click(sender, e);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                toolStripButton17_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb == null) return;

            if (e.Control && e.KeyCode == Keys.C) rtb.Copy();
            if (e.Control && e.KeyCode == Keys.V) rtb.Paste();
            if (e.Control && e.KeyCode == Keys.X) rtb.Cut();
            if (e.Control && e.KeyCode == Keys.A) rtb.SelectAll();
            if (e.KeyCode == Keys.F12) fileManager.saveAs(GetActiveRichTextBox());


            if (e.Control && e.KeyCode == Keys.S)
                fileManager.saveToFile(rtb);
        }

        private void HighlightSyntax(RichTextBox rtb)
        {
            //isTextChangedByUndoRedo = true;
            string[] keywords = { "const ", "char " };
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            rtb.SuspendLayout();
            int scrollPos = rtb.GetPositionFromCharIndex(rtb.SelectionStart).Y;

            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;


            foreach (string word in keywords)
            {
                MatchCollection matches = Regex.Matches(rtb.Text, $@"\b{word}\b");
                foreach (Match match in matches)
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.Blue;
                }
            }

            foreach (char c in rtb.Text)
            {
                if (char.IsDigit(c))
                {
                    MatchCollection matches = Regex.Matches(rtb.Text, $@"{c}");
                    foreach (Match match in matches)
                    {
                        rtb.Select(match.Index, match.Length);
                        rtb.SelectionColor = Color.DarkOliveGreen;
                    }
                }
            }

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;
            rtb.SelectionColor = Color.Black;
            rtb.ScrollToCaret();
            rtb.ResumeLayout();
            //isTextChangedByUndoRedo = false;
        }
        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is RichTextBox rtb)
            {
                if (!isTextChangedByUndoRedo)
                {
                    undoStack.Push(rtb.Text);
                    redoStack.Clear();
                }

                isTextChangedByUndoRedo = true;
                HighlightSyntax(rtb);
                isTextChangedByUndoRedo = false;
            }

        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb != null && undoStack.Count > 0)
            {
                isTextChangedByUndoRedo = true; // Блокируем ненужные вызовы TextChanged

                redoStack.Push(rtb.Text); // Сохраняем текущее состояние перед откатом
                rtb.Text = undoStack.Pop();

                rtb.SelectionStart = rtb.Text.Length;
                rtb.ScrollToCaret();
                isTextChangedByUndoRedo = false;
                
            }
        }
        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb != null && undoStack.Count > 0)
            {
                isTextChangedByUndoRedo = true; // Блокируем ненужные вызовы TextChanged

                redoStack.Push(rtb.Text); // Сохраняем текущее состояние перед откатом
                rtb.Text = undoStack.Pop();

                rtb.SelectionStart = rtb.Text.Length;
                rtb.ScrollToCaret();
                isTextChangedByUndoRedo = false;

            }
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb != null && redoStack.Count > 0)
            {
                isTextChangedByUndoRedo = true;

                undoStack.Push(rtb.Text);
                rtb.Text = redoStack.Pop();

                rtb.SelectionStart = rtb.Text.Length;
                rtb.ScrollToCaret();
                isTextChangedByUndoRedo = false;
            }
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = GetActiveRichTextBox();
            if (rtb != null && redoStack.Count > 0)
            {
                isTextChangedByUndoRedo = true;

                undoStack.Push(rtb.Text);
                rtb.Text = redoStack.Pop();

                rtb.SelectionStart = rtb.Text.Length;
                rtb.ScrollToCaret();
                isTextChangedByUndoRedo = false;
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            About aboutProgram = new About();
            aboutProgram.Show();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutProgram = new About();
            aboutProgram.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string newCulture = Thread.CurrentThread.CurrentUICulture.Name == "ru-RU" ? "en-US" : "ru-RU";

            Properties.Settings.Default.AppLanguage = newCulture;
            Properties.Settings.Default.Save();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(newCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(newCulture);

            foreach (Form form in Application.OpenForms)
            {
                AppSettings.UpdateFormLanguage(form);
            }

        }

        private void DrawLineNumbers(Graphics g, RichTextBox richTextBox, PictureBox lineNumberBox)
        {
            g.Clear(lineNumberBox.BackColor);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int firstIndex = richTextBox.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = richTextBox.GetLineFromCharIndex(firstIndex);
            int totalLines = richTextBox.Lines.Length;

            int maxWidth = TextRenderer.MeasureText(totalLines.ToString(), richTextBox.Font).Width + 10;
            if (lineNumberBox.Width != maxWidth)
            {
                lineNumberBox.Width = maxWidth;
            }

            int y = 0;
            for (int i = firstLine; i < totalLines; i++)
            {
                Point pos = richTextBox.GetPositionFromCharIndex(richTextBox.GetFirstCharIndexFromLine(i));
                y = pos.Y;

                if (y >= richTextBox.Height)
                    break;

                g.DrawString((i + 1).ToString(), richTextBox.Font, Brushes.Black, 5, y);
            }
        }

        public void AddNewTab(string fileName, string content)
        {
            TabPage newTab = new TabPage(Path.GetFileName(fileName));
            RichTextBox richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Text = content
            };

            PictureBox lineNumberBox = new PictureBox
            {
                Width = 35,
                Dock = DockStyle.Left,
                BackColor = richTextBox.BackColor,
            };

            richTextBox.KeyDown += RichTextBox_KeyDown;
            richTextBox.TextChanged += RichTextBox_TextChanged;
            richTextBox.TextChanged += (s, e) => lineNumberBox.Invalidate();
            richTextBox.VScroll += (s, e) => lineNumberBox.Invalidate();
            richTextBox.Resize += (s, e) => lineNumberBox.Invalidate();
            richTextBox.FontChanged += (s, e) => lineNumberBox.Invalidate();

            lineNumberBox.Paint += (s, e) => DrawLineNumbers(e.Graphics, richTextBox, lineNumberBox);

            newTab.Controls.Add(richTextBox);
            newTab.Controls.Add(lineNumberBox);
            tabControl1.TabPages.Add(newTab);
            tabControl1.SelectedTab = newTab;

            isTextChangedByUndoRedo = true;
            HighlightSyntax(richTextBox);
            isTextChangedByUndoRedo= false;

            fileManager.FilePaths.Add(richTextBox, fileName);

            richTextBox.Modified = false;
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(toolStripComboBox2.Text, out int size))
            {
                Properties.Settings.Default.FontSize = size;
                Properties.Settings.Default.Save();

                AppSettings.ApplyFontSizeToControls(this.Controls);

                foreach (Form form in Application.OpenForms)
                {
                    AppSettings.ApplyFontSizeToControls(form.Controls);
                }
            }
        }

        private void ClearErrorSelection(RichTextBox rtb)
        {
            //isTextChangedByUndoRedo = true;
            rtb.SuspendLayout();
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            rtb.SelectAll();
            rtb.SelectionBackColor = Color.White;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;
            rtb.ResumeLayout();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            isTextChangedByUndoRedo = true;
            dataGridView1.Rows.Clear();
            foreach (TabPage tab in tabControl1.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is RichTextBox rtb)
                    {
                        //isTextChangedByUndoRedo = true;
                        //undoStack.Push(rtb.Text);

                        ClearErrorSelection(rtb);
                        HighlightSyntax(rtb);
                        try
                        {
                            var parcer = new Parcer(rtb.Text, dataGridView1, rtb);
                            parcer.Parse();
                            int countErrors = parcer.CountErrors;
                            if (countErrors > 0)
                            {
                                tabControl2.TabPages[0].Text = $"Обнаружено ошибок: {countErrors}";
                                dataGridView1.Show();
                                label1.Hide();
                            }
                            else
                            {
                                tabControl2.TabPages[0].Text = "Результат";
                                dataGridView1.Hide();
                                label1.Text = "ПОЛИЗ: " + parcer.GetPOLIZ() + "\nРезультат вычислений: " + parcer.EvaluatePOLIZ();
                                label1.Show();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                   
                    }
                }
            }
            isTextChangedByUndoRedo = false;
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isTextChangedByUndoRedo = true;
            dataGridView1.Rows.Clear();
            foreach (TabPage tab in tabControl1.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is RichTextBox rtb)
                    {
                        //isTextChangedByUndoRedo = true;

                        //undoStack.Push(rtb.Text);

                        ClearErrorSelection(rtb);
                        HighlightSyntax(rtb);
                        try
                        {
                            var parcer = new Parcer(rtb.Text, dataGridView1, rtb);
                            parcer.Parse();
                            int countErrors = parcer.CountErrors;
                            if (countErrors > 0)
                            {
                                tabControl2.TabPages[0].Text = $"Обнаружено ошибок: {countErrors}";
                                dataGridView1.Show();
                                label1.Hide();
                            }
                            else
                            {
                                tabControl2.TabPages[0].Text = "Результат";
                                dataGridView1.Hide();
                                label1.Text = "ПОЛИЗ: " + parcer.GetPOLIZ() + "\nРезультат вычислений: " + parcer.EvaluatePOLIZ();
                                label1.Show();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(AppSettings.ErrorMessage() + ex.Message, AppSettings.ErrorMessage(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            isTextChangedByUndoRedo = false;
        }
    }
}
