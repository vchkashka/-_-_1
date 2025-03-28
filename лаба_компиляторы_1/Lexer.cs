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

            rtb.SelectAll();
            rtb.SelectionBackColor = rtb.BackColor;

            rtb.Select(index, 1);
            rtb.SelectionBackColor = Color.Red;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
        }

        public int Analyze(string text, RichTextBox rtb, DataGridView dataGrid, int index = 0)
        {
            int countErrors = 0;
            string row = text;
            bool lastWasKeyword = false;
            Parcer parcer = new Parcer();

            //MessageBox.Show($"{index}");
            while (index < row.Length)
            {
                
                char current = row[index];

                if (char.IsWhiteSpace(current))
                {
                    if (lastWasKeyword)
                    {
                        lastWasKeyword = false;
                    }
                    index++;
                    continue;
                }

                if (current >= 'А' && current <= 'Я' || current >= 'а' && current <= 'я')
                {
                    dataGrid.Rows.Add($"Недопустимый символ: {current}", $"{index + 1}");
                    countErrors++;
                    ErrorSelection(index, rtb);
                    break;
                }

                if (char.IsLetter(current))
                {
                    int start = index;
                    bool error = false;
                    while (index < row.Length && (char.IsDigit(row[index]) | char.IsLetter(row[index])))
                    {
                        if (row[index] >= 'А' && row[index] <= 'Я' || row[index] >= 'а' && row[index] <= 'я')
                        {
                            error = true;
                            break;
                        }
                        index++;
                    }

                    if (error)
                    {
                        dataGrid.Rows.Add($"Недопустимый символ: {row[index]}", $"{index + 1}");
                        countErrors++;
                        ErrorSelection(index, rtb);
                        break;
                    }

                    string word = row.Substring(start, index - start);
                    string location = $"{LocationDetection(word, row, start)}";

                    switch (word)
                    {
                        case "const":
                            parcer.tokens.Add(Tuple.Create(1, start + 1));
                            lastWasKeyword = true;
                            break;
                        case "char":
                            parcer.tokens.Add(Tuple.Create(2, start + 1));
                            lastWasKeyword = true;
                            break;
                        default:
                            parcer.tokens.Add(Tuple.Create(3, start + 1));
                            break;
                    }
                    continue;
                }

                if (char.IsDigit(current))
                {
                    int start = index;
                    while (index < row.Length && char.IsDigit(row[index]))
                        index++;

                    string number = row.Substring(start, index - start);
                    string location = $"{LocationDetection(number, row, start)}";
                    parcer.tokens.Add(Tuple.Create(9, start + 1));
                    continue;
                }

                switch (current)
                {
                    case '[':
                        parcer.tokens.Add(Tuple.Create(5, index + 1));
                        break;
                    case ']':
                        parcer.tokens.Add(Tuple.Create(6, index + 1));
                        break;
                    case '=':
                        parcer.tokens.Add(Tuple.Create(7, index + 1));
                        break;
                    case ';':
                        parcer.tokens.Add(Tuple.Create(10, index + 1));
                        countErrors += parcer.Analyze(rtb, dataGrid);
                        Analyze(text, rtb, dataGrid, index+1);
                        return countErrors;
                    case '"':
                        int start = index;
                        bool closingBracket = false;
                        index++;

                        while (index < row.Length)
                        {
                            if (row[index] == '"')
                            {
                                closingBracket = true;
                                break;
                            }
                            index++;
                        }

                        if (index < row.Length && closingBracket)
                        {
                            index++;
                            string strValue = row.Substring(start, index - start);
                            string location = $"{LocationDetection(strValue, row, start)}";
                            parcer.tokens.Add(Tuple.Create(8, start));
                        }
                        else
                        {
                            ErrorSelection(start, rtb);
                            dataGrid.Rows.Add($"Недопустимый символ: {row.Substring(start, 1)}", $"{start + 1}");
                            countErrors++;
                            break;
                        }
                        continue;
                    default:
                        ErrorSelection(index, rtb);
                        dataGrid.Rows.Add($"Недопустимый символ: {current.ToString()}", $"{index + 1}");
                        countErrors++;
                        continue;
                }
                index++;
            }
            return countErrors;
        }
    }
}
