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
       // public bool isErrorSelection = false;

        private void ErrorSelection(int index, RichTextBox rtb)
        {
            //isErrorSelection = true;
            rtb.SuspendLayout();
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            //rtb.SelectAll();
            //rtb.SelectionBackColor = ;

            if (index <= rtb.Text.Length)
                rtb.Select(index, 1);
            rtb.SelectionBackColor = Color.Red;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
            //isErrorSelection = false;
        }

        private void ExtractInvalidFragment(string row, ref int index, DataGridView dataGrid, ref int countErrors, RichTextBox rtb)
        {
            int startError = index;
            while (index < row.Length && !(row[index] >= 'A' && row[index] <= 'Z') && !(row[index] >= 'a' && row[index] <= 'z') && !char.IsDigit(row[index]) && !"[]=;\" ".Contains(row[index]))
            {
                index++;
            }
            string wordError = row.Substring(startError, index - startError);
            dataGrid.Rows.Add($"Недопустимый фрагмент: {wordError}", $"{startError + 1}");
            countErrors++;
            ErrorSelection(startError, rtb);
        }


        public int Analyze(string text, RichTextBox rtb, DataGridView dataGrid)
        {
            int index = 0;
            int countErrors = 0;
            string row = text;
            Parcer parcer = new Parcer();
            
            while (index < row.Length)
            {

                char current = row[index];

                if (char.IsWhiteSpace(current))
                {
                    index++;
                    continue;
                }

                if (!(current >= 'A' && current <= 'Z') && !(current >= 'a' && current <= 'z') && !char.IsDigit(current) && !"[]=;\" ".Contains(current))
                    {
                        ExtractInvalidFragment(row, ref index, dataGrid, ref countErrors, rtb);
                    if (index + 1 < row.Length && !char.IsLetter(row[index + 1]))
                        index++;
                }

                if (char.IsLetter(current))
                {
                    int start = index;
                    string word = null;
                    while (index < row.Length)
                    {
                        if (!(row[index] >= 'A' && row[index] <= 'Z') && !(row[index] >= 'a' && row[index] <= 'z') && !char.IsDigit(row[index]) && !"[]=;\" ".Contains(row[index]))
                        {
                            ExtractInvalidFragment(row, ref index, dataGrid, ref countErrors, rtb);
                        }
                        else if (char.IsDigit(row[index]) || char.IsLetter(row[index]))
                        {
                            if (word == null)
                                start = index;
                            word = word + row[index];
                            index++;
                        }
                        else
                        {
                            //index++;
                            break;
                        }
                        //MessageBox.Show($"{word}, {start + 1}");
                    }
                    
                    //MessageBox.Show($"{word}, {}");
                    switch (word)
                    {
                        case "const":
                            parcer.tokens.Add(Tuple.Create(1, start + 1));
                            break;
                        case "char":
                            parcer.tokens.Add(Tuple.Create(2, start + 1));
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
                        break;
                        //if (index+1 <= row.Length)
                        //Analyze(text, rtb, dataGrid, index+1);
                        //return countErrors;
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
                            parcer.tokens.Add(Tuple.Create(8, start));
                        }
                        else
                        if (!closingBracket)
                        {
                            index++;
                            string strValue = row.Substring(start, index - start - 1);
                            parcer.tokens.Add(Tuple.Create(8, start));
                            //ErrorSelection(start, rtb);
                            //dataGrid.Rows.Add($"Недопустимый символ: {row.Substring(start, 1)}", $"{start + 1}");
                            dataGrid.Rows.Add($"Не хватает \" в конце строки", $"{index - 2}");
                            countErrors++;
                        }
                        continue;
                    //default:
                    //    ErrorSelection(index, rtb);
                    //    dataGrid.Rows.Add($"Недопустимый символ: {current.ToString()}", $"{index + 1}");
                    //    countErrors++;
                    //    continue;
                }
                index++;
            }
            if (row.Length > 0 ) 
            countErrors += parcer.Analyze(rtb, dataGrid);
            return countErrors;
        }
    }
}
