using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    internal class Parcer
    {
        public List<Tuple<int, int>> tokens = new List<Tuple<int, int>>();
        DataGridView data = new DataGridView();

        //public bool isErrorSelection = true;

        private void ErrorSelection(int index, RichTextBox rtb)
        {
            //isErrorSelection = true;
            rtb.SuspendLayout();
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            if (index <= rtb.Text.Length)
                rtb.Select(index, 1);
            rtb.SelectionBackColor = Color.Red;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
           // isErrorSelection = false;
        }

        public int Analyze(RichTextBox rtb, DataGridView data)
        {
            //foreach (Tuple<int, int> token in tokens)
            //MessageBox.Show($"{token.Item1}, {token.Item2}");
            int countErrors = 0;
            this.data = data;
            int i = 0;
            int state = 1;

            while (state > 0)
            {
                var token = i < tokens.Count ? tokens[i] : null;
                int position = token?.Item2 ?? rtb.Text.Length - 1;
                //MessageBox.Show($"{token.Item1}, {token.Item2}");
                if (tokens.Count == 0)
                {
                    Error("Пустой ввод", 0);
                    return 1;
                }
                switch (state)
                {
                    case 1:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else if (token.Item1 != 1 && token.Item1 != 2)
                        {
                            Error("Недопустимый фрагмент", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                            state = 1;
                        }
                        else if (token.Item1 != 1)
                        {
                            Error("Ожидалось ключевое слово const", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            state = 2;
                        }
                        else
                        {
                            i++;
                            if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 1)
                                while (tokens[i].Item1 == 1)
                                {
                                    Error("Недопустимый фрагмент - const", tokens[i].Item2);
                                    ErrorSelection(tokens[i].Item2 - 1, rtb);
                                    countErrors++;
                                    i++;
                                }
                            state = 2;
                        }


                        break;
                    case 2:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 == 3 && tokens[i + 1]!= null && tokens[i + 1]?.Item1 == 3)
                        {
                            Error("Ожидалось ключевое слово char", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else
                        if (token.Item1 != 2)
                        {
                            Error("Ожидалось ключевое слово char", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else i++;
                        if (i < tokens.Count &&  tokens[i] != null && tokens[i].Item1 == 2)
                            while (tokens[i].Item1 == 2)
                            {
                                Error("Недопустимый фрагмент - char", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 3;
                        break;
                    case 3:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 3)
                        {
                            Error("Ожидался идентификатор", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else
                        {
                            i++;
                        }
                        if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 3)
                            while (i < tokens.Count && tokens[i].Item1 == 3)
                            {
                                Error("Недопустимый фрагмент", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 4;
                        break;
                    case 4:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 5 && token.Item1 != 6 && token.Item1 != 9)
                        {
                            Error("Неожиданный символ, проверьте наличие [", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else if (token.Item1 != 5)
                        {
                            Error("Пропущена [", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else i++;
                        if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 5)
                            while (tokens[i].Item1 == 5)
                            {
                                Error("Недопустимый фрагмент - [", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 5;
                        break;
                    case 5:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 6 && token.Item1 != 9 && token.Item1 != 7)
                        {
                            Error("Пропущено число или ] ", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else if ((token.Item1 != 6 && token.Item1 != 9))
                        {
                            Error("Пропущено число или ] ", position);
                            ErrorSelection(position - 1, rtb);
                            state = 7;
                            countErrors++;
                        }
                        else if (token.Item1 == 9)
                        {
                            state = 6;
                            i++;
                        }
                        else if (token.Item1 == 6)
                        {
                            i++;
                            if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 6)
                                while (tokens[i].Item1 == 6)
                                {
                                    Error("Недопустимый фрагмент - ]", tokens[i].Item2);
                                    ErrorSelection(tokens[i].Item2 - 1, rtb);
                                    countErrors++;
                                    i++;
                                }
                            state = 7;
                        }
                        break;
                    case 6:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 6 && token.Item1 != 7)
                        {
                            Error("Пропущена ]", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else if (token.Item1 != 6)
                        {
                            Error("Пропущена ]", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else i++;
                        if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 6)
                            while (tokens[i].Item1 == 6)
                            {
                                Error("Недопустимый фрагмент - ]", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 7;
                        break;
                    case 7:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 7 && token.Item1 != 8)
                        {
                            Error("Ожидался знак = ", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else if (token.Item1 != 7)
                        {
                            Error("Ожидался знак = ", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else i++;
                        if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 7)
                            while (tokens[i].Item1 == 7)
                            {
                                Error("Недопустимый фрагмент - =", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 8;
                        break;
                    case 8:
                        if (token == null)
                        {
                            Error("Неполный ввод, проверьте наличие всех необходимых символов", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        if (token.Item1 != 8 && token.Item1 != 10)
                        {
                            Error("Ожидалась строка", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            i++;
                        }
                        else if (token.Item1 != 8)
                        {
                            Error("Ожидалась строка", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                        }
                        else i++;
                        if (i < tokens.Count && tokens[i] != null && tokens[i].Item1 == 8)
                            while (tokens[i].Item1 == 8)
                            {
                                Error("Недопустимый фрагмент - строка", tokens[i].Item2);
                                ErrorSelection(tokens[i].Item2 - 1, rtb);
                                countErrors++;
                                i++;
                            }
                        state = 9;
                        break;
                    case 9:
                        if (token == null)
                        {
                            Error("Ожидался символ ;", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            return countErrors;
                        }
                        else if (token.Item1 != 10 && token.Item1 != 1)
                        {
                            Error("Ожидался символ ;", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;

                            if (i + 1 < tokens.Count)
                            {
                                state = 1;
                                i++;
                            }
                            return countErrors;
                        }
                        else if (token.Item1 != 10)
                        {
                            Error("Ожидался символ ;", position);
                            ErrorSelection(position - 1, rtb);
                            countErrors++;
                            if (i + 1 < tokens.Count) state = 1;
                        }
                        else if (i + 1 < tokens.Count)
                        {
                            state = 1;
                            i++;
                        }
                        else return countErrors;
                        break;
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
