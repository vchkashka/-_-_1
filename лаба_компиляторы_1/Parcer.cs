using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    internal class Parcer
    {
        public List<Tuple<int, int>> tokens = new List<Tuple<int, int>>();
        DataGridView data = new DataGridView();
        

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

        public int Analyze(RichTextBox rtb, DataGridView data)
        {
            int countErrors = 0;
            this.data = data;
            int i = 0;
            int state = 1;

            while (state > 0)
            {
                var token = i < tokens.Count ? tokens[i] : null;
                int position = token?.Item2 ?? -1;

                switch (state)
                {
                    case 1:
                        if (token == null || token.Item1 != 1)
                        {
                            Error("Ожидалось ключевое слово const", position);
                            ErrorSelection(position-1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 2;
                        break;
                    case 2:
                        if (token == null || token.Item1 != 2)
                        {
                            Error("Ожидалось ключевое слово char", position);
                            ErrorSelection(position-1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 3;
                        break;
                    case 3:
                        if (token == null || token.Item1 != 3)
                        {
                            Error("Ожидался идентификатор", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 4;
                        break;
                    case 4:
                        if (token == null || token.Item1 != 5)
                        {
                            Error("Пропущена [ ", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 5;
                        break;
                    case 5:
                        if (token == null || (token.Item1 != 6 && token.Item1 != 9))
                        {
                            Error("Пропущено число или ] ", position);
                            ErrorSelection(position - 1, rtb);
                            state = 7;
                            countErrors++;
                            return countErrors;
                        }
                        else if (token.Item1 == 9)
                        {
                            state = 6;
                            i++;
                        }
                        else if (token.Item1 == 6)
                        {
                            state = 7;
                            i++;
                        }
                        break;
                    case 6:
                        if (token == null || token.Item1 != 6)
                        {
                            Error("Пропущена ]", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 7;
                        break;
                    case 7:
                        if (token == null || token.Item1 != 7)
                        {
                            Error("Пропущен знак = ", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 8;
                        break;
                    case 8:
                        if (token == null || token.Item1 != 8)
                        {
                            Error("Ожидалась строка", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else i++;
                        state = 9;
                        break;
                    case 9:
                        if (token == null || token.Item1 != 10)
                        {
                            Error("Ожидался символ ;", position);
                            ErrorSelection(position - 1 , rtb);
                            countErrors++;
                        }
                        return countErrors;
                }
            }
            return countErrors;
        }

        private void Error(string message, int place)
        {
            this.data.Rows.Add(message, place);
        }
    }
}
