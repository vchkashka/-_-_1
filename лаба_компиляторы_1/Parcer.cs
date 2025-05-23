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
    public class Parcer
    {
        private string input;
        private int pos;

        DataGridView data = new DataGridView();
        RichTextBox rtb = new RichTextBox();

        int countErrors;

        public int CountErrors { get { return countErrors; } }

        public Parcer(string input, DataGridView data, RichTextBox rtb)
        {
            this.input = input.Replace(" ", "");
            this.data = data;
            this.rtb = rtb;
            this.pos = 0;
        }

        private void ErrorSelection(int index, RichTextBox rtb)
        {
            rtb.SuspendLayout();
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            if (index <= rtb.Text.Length)
                rtb.Select(index, 1);
            rtb.SelectionBackColor = Color.Red;

            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
        }

        public void Parse()
        {
            countErrors = 0;
            pos = 0;

            try
            {
                Doc();
                if (pos != input.Length)
                {
                    Error("Неожиданный символ", pos);
                    ErrorSelection(pos+1, rtb);
                    countErrors++;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message, pos);
                ErrorSelection(Math.Max(0, pos), rtb);
                countErrors++;
            }
        }

        private void Doc()
        {
            if (pos >= input.Length) return;
            if (input.Substring(pos).StartsWith("</"))
            {
                return;
            }

            Element();
            Doc();
        }

        private void Element()
        {
            int start = pos;

            if (IsChar())
            {
                Text();
                return;
            }

            if (Match("<em>"))
            {
                Doc();
                Require("</em>");
                return;
            }

            if (Match("<p>"))
            {
                Doc();
                Require("</p>");
                return;
            }

            if (Match("<ol>"))
            {
                List();
                Require("</ol>");
                return;
            }

            throw new Exception("Ожидался элемент (Text | <em> | <p> | <ol>)");
        }

        private void List()
        {
            if (input.Substring(pos).StartsWith("<li>"))
            {
                ListItem();
                List();
            }
        }

        private void ListItem()
        {
            Require("<li>");
            Text();
            Require("</li>");
        }

        private void Text()
        {
            if (ParseChar())
                Text();
        }

        private bool ParseChar()
        {
            if (pos < input.Length && char.IsLetter(input[pos]))
            {
                pos++;
                return true;
            }
            return false;
        }

        private bool Match(string token)
        {
            if (input.Substring(pos).StartsWith(token))
            {
                pos += token.Length;
                return true;
            }
            return false;
        }

        private void Require(string token)
        {
            if (!Match(token))
                throw new Exception($"Ожидалось '{token}'");
        }

        private bool IsChar()
        {
            return pos < input.Length && char.IsLetter(input[pos]);
        }

        private void Error(string message, int place)
        {
            this.data.Rows.Add(message, place);
        }

    }
}
