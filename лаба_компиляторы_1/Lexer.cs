﻿using System;
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

        public void Analyze(string text, RichTextBox rtb, DataGridView dataGrid)
        {
            //string[] rows = text.Split('\n');

            //for (int i = 0; i < rows.Length; i++)
            //{
            //string row = rows[i];
            string row = text;
            int index = 0;
            bool lastWasKeyword = false;

            while (index < row.Length)
            {
                char current = row[index];

                if (char.IsWhiteSpace(current))
                {
                    if (lastWasKeyword)
                    {
                        dataGrid.Rows.Add("4", "разделитель", "пробел", $"{index + 1} символ");
                        lastWasKeyword = false;
                    }
                    index++;
                    continue;
                }

                if (current >= 'А' && current <= 'Я' || current >= 'а' && current <= 'я')
                {
                    //int start = index;
                    //while (index < row.Length && (((row[index] >= 'А' && row[index] <= 'Я') ||
                    //      (row[index] >= 'а' && row[index] <= 'я'))))
                    //    index++;

                    //string word = row.Substring(start, index - start);

                    //dataGridView1.Rows.Add("ERROR", "недопустимый фрагмент", word, $"{i + 1} строка, {start + 1} символ");
                    dataGrid.Rows.Add("ERROR", "недопустимый символ", current, $"{index + 1} символ");

                    ErrorSelection(index, rtb);

                    break;

                    //continue;
                }
                //index++;
                if (char.IsLetter(current))
                {
                    int start = index;
                    bool error = false;
                    //((row[index] >= 'A' && row[index] <= 'Z') ||
                    //      (row[index] >= 'a' && row[index] <= 'z') ||
                    //      (row[index] >= '0' && row[index] <= '9'))
                    while (index < row.Length && (char.IsDigit(row[index]) | char.IsLetter(row[index])))
                    {
                        if (row[index] >= 'А' && row[index] <= 'Я' || row[index] >= 'а' && row[index] <= 'я')
                        {
                           error = true;
                           break;
                        }
                        index++;
                    }

                    if (error == true)
                    {
                        dataGrid.Rows.Add("ERROR", "недопустимый символ", row[index], $"{index + 1} символ");
                        ErrorSelection(index, rtb);
                        break;
                    }

                    string word = row.Substring(start, index - start);
                    string location = $"{LocationDetection(word, row, start)}";

                    switch (word)
                    {
                        case "const":
                            dataGrid.Rows.Add("1", "ключевое слово", "const", location);
                            //parcer.dictionary.Add(1, "const");
                            lastWasKeyword = true;
                            break;
                        case "char":
                            //parcer.dictionary.Add(2, "char");
                            dataGrid.Rows.Add("2", "ключевое слово", "char", location);
                            lastWasKeyword = true;
                            break;
                        default:
                            //parcer.dictionary.Add(3, word);
                            dataGrid.Rows.Add("3", "идентификатор", word, location);
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
                    //parcer.dictionary.Add(3, word);
                    dataGrid.Rows.Add("9", "целое без знака", number, location);
                    continue;
                }

                switch (current)
                {
                    case '[':
                        dataGrid.Rows.Add("5", "открывающаяся скобка", "[", $"{index + 1} символ");
                        break;
                    case ']':
                        dataGrid.Rows.Add("6", "закрывающаяся скобка", "]", $"{index + 1}  символ  ");
                        break;
                    case '=':
                        dataGrid.Rows.Add("7", "оператор присваивания", "=", $"{index + 1}  символ");
                        break;
                    case ';':
                        dataGrid.Rows.Add("10", "конец оператора", ";", $"{index + 1} символ");
                        break;
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

                        if (index < row.Length && closingBracket == true)
                        {
                            index++;
                            string strValue = row.Substring(start, index - start);
                            string location = $"{LocationDetection(strValue, row, start)}";
                            dataGrid.Rows.Add("8", "строка", strValue, location);
                        }
                        else if (closingBracket == false)
                        {
                            ErrorSelection(start, rtb);
                            dataGrid.Rows.Add("ERROR", "недопустимый символ", row.Substring(start, 1), $"{start + 1}");
                            break;
                        }
                        continue;
                    default:
                        ErrorSelection(index, rtb);
                        dataGrid.Rows.Add("ERROR", "недопустимый символ", current.ToString(), $"{index + 1}");
                        return;
                }
                index++;
            }
            //}
        }
    }
}
