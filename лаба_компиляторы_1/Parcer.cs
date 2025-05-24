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
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Position { get; set; }

        public Token(string type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }
    }
    public class Parcer
    {
        private string input;
        private int pos;

        DataGridView data = new DataGridView();
        RichTextBox rtb = new RichTextBox();

        string functions;
        string text;
        int countErrors;

        public int CountErrors { get { return countErrors; } }
        public string Functions { get { return functions; } }

        public Parcer(string input, DataGridView data, RichTextBox rtb)
        {
            this.input = input.Replace(" ", "");
            this.data = data;
            this.rtb = rtb;
            this.pos = 0;
            this.functions = "";
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

        private List<Token> tokens;

        private void Lex()
        {
            tokens = new List<Token>();
            countErrors = 0;

            int i = 0;
            while (i < input.Length)
            {
                if (char.IsLetter(input[i]) && ((input[i] >= 'A' && input[i] <= 'Z') || (input[i] >= 'a' && input[i] <= 'z')))
                {
                    tokens.Add(new Token("CHAR", input[i].ToString(), i));
                    i++;
                }
                else if (input[i] == '<')
                {
                    int start = i;
                    while (i < input.Length && input[i] != '>')
                        i++;
                    if (i < input.Length && input[i] == '>')
                    {
                        i++;
                        string tag = input.Substring(start, i - start);
                        tokens.Add(new Token("TAG", tag, start));
                        switch (tag) 
                        {
                            case "<em>": break;
                            case "<p>": break;
                            case "<ol>": break;
                            case "<li>": break;
                            case "</em>": break;
                            case "</p>": break;
                            case "</ol>": break;
                            case "</li>": break;
                            default: 
                                    Error("Недопустимый символ", start+1);
                                    ErrorSelection(start+1, rtb);
                                    countErrors++;
                                    i++;
                                 break;

                        }
                    }
                    else
                    {
                        Error("Незавершённый тег", start);
                        ErrorSelection(start, rtb);
                        countErrors++;
                        i++;
                    }
                }
                else
                {
                    Error($"Недопустимый символ '{input[i]}'", i);
                    ErrorSelection(i, rtb);
                    countErrors++;
                    i++;
                }
            }
        }

        public void Parse()
        {
            countErrors = 0;
            pos = 0;

            Lex();

            if (countErrors > 0)
                return; 

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
            if (pos >= input.Length) 
            {
                if (functions.Length == 0)
                {
                    functions += " Doc - ε";
                }
                else functions += " - Doc- ε";

                return; 
            }
            if (input.Substring(pos).StartsWith("</"))
            {
                if (functions.Length == 0)
                {
                    functions += " Doc - ε";
                }
                else functions += " - Doc - ε";

                return;
            }
            if (functions.Length == 0)
            {
                functions += " Doc";
            }
            else functions += " - Doc";
            Element();
            Doc();
        }

        private void Element()
        {
            int start = pos;
            functions += " - Element";

            if (IsChar())
            {
                Text();
                return;
            }

            if (Match("<em>"))
            {
                functions += " - <em>";
                Doc();
                Require("</em>");
                return;
            }

            if (Match("<p>"))
            {
                functions += " - <p>";
                Doc();
                Require("</p>");
                return;
            }

            if (Match("<ol>"))
            {
                functions += " - <ol>";
                List();
                Require("</ol>");
                return;
            }
            if (!Match("<em>") && !Match("<p>") && !Match("<ol>"))
                functions += " - Text - ε";

            throw new Exception("Ожидался элемент (Text | <em> | <p> | <ol>)");

        }

        private void List()
        {
            functions += " - List";
            if (input.Substring(pos).StartsWith("<li>"))
            {
                ListItem();
                List();
            }
            else functions += " - ε";
        }

        private void ListItem()
        {
            functions += " - ListItem";
            Require("<li>");
            Text();
            Require("</li>");
        }

        private void Text()
        {
            functions += " - Text";
            if (ParseChar())
                Text();
            else functions += " - ε";
        }

        private bool ParseChar()
        {
            if (pos < input.Length && char.IsLetter(input[pos]))
            {
                functions += " - Char";
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
            else
                functions += " - " + token;
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
