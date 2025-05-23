using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    internal class Lexer
    {
        private string LocationDetection(string word, string text, int startIndex)
        {
            int start = text.IndexOf(word, startIndex) + 1;
            int end = start + word.Length - 1;
            return $"c {start} по {end} символ";
        }

        private void ErrorSelection(int index, RichTextBox rtb)
        {
            rtb.SuspendLayout();
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            rtb.Select(index, 1);
            rtb.SelectionBackColor = Color.Red;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
        }

        public void Analyze(string text, RichTextBox rtb, DataGridView dataGrid)
        {
            string row = text;
            int index = 0;

            while (index < row.Length)
            {
                char current = row[index];

                if (char.IsWhiteSpace(current))
                {
                    index++;
                    continue;
                }

                //if (current >= 'А' && current <= 'Я' || current >= 'а' && current <= 'я')
                //{
                //    dataGrid.Rows.Add("ERROR", "недопустимый символ", current, $"{index + 1} символ");
                //    ErrorSelection(index, rtb);
                //    index++;
                //    while (row[index] >= 'А' && row[index] <= 'Я' || row[index] >= 'а' && row[index] <= 'я')
                //    {
                //        index++;
                //    }
                //    continue;
                //}
                if (char.IsLetter(current) && ((row[index] >= 'A' && row[index] <= 'Z') || (row[index] >= 'a' && row[index] <= 'z')))
                {
                    int start = index;
                    int end = 0;
                    bool error = false;
                    int errorIndex = 0;
                    while (index < row.Length && (char.IsLetter(row[index])))
                    {
                        if (!((row[index] >= 'A' && row[index] <= 'Z') || (row[index] >= 'a' && row[index] <= 'z')))
                        {
                            end = index;
                            error = true;
                            errorIndex = index;
                            while (!((row[index] >= 'A' && row[index] <= 'Z') || (row[index] >= 'a' && row[index] <= 'z')))
                            {
                                index++;
                            }
                        }
                        index++;
                    }

                    if (error == false) end = index;

                    string word = row.Substring(start, end - start);
                    string location = $"{LocationDetection(word, row, start)}";

                    dataGrid.Rows.Add("1", "Текст", word, location);

                    if (error == true)
                    {
                        dataGrid.Rows.Add("ERROR", "недопустимый символ", row[errorIndex], $"{errorIndex + 1} символ");
                        ErrorSelection(errorIndex, rtb);
                    }
                    continue;
                }

                if (row[index] == '<') 
                {
                    int start = index;
                    index++;

                    while (index < row.Length)
                    {
                        if (row[index] == '>')
                        {
                            break;
                        }
                        index++;
                    }

                    if (index <= row.Length)
                    {
                        if (index < row.Length) index++;
                        string strValue = row.Substring(start, index - start);
                        string location = $"{LocationDetection(strValue, row, start)}";
                        dataGrid.Rows.Add("8", "строка", strValue, location);
                    }
                    //else if (closingBracket == false)
                    //{
                    //    ErrorSelection(start, rtb);
                    //    dataGrid.Rows.Add("ERROR", "недопустимый символ", row.Substring(start, 1), $"{start + 1}");
                    //    break;
                    //}
                    continue;
                }
                index++;
            }
        }
    }
}
