using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace лаба_компиляторы_1
{
    internal class Parcer
    {
        public Dictionary<int, string> dictionary = new Dictionary<int, string>();
        private enum State { Start, Const, CharType, Identifier, BracketOpen, BracketNum, BracketClose, Equal, Quote, String, End }
        private State state = State.Start;
        private List<string> tokens = new List<string>();
        private string buffer = "";
        DataGridView data = new DataGridView();

        public void Analyze(string text, DataGridView data)
        {
            buffer = "";
            this.data = data;
            // text = text.Trim();
            while (state != State.End)
            {
                for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                    switch (state)
                    {
                        case State.Start:
                            if (Regex.IsMatch(text.Substring(i), "^const\\b")) { tokens.Add("const"); i += 4; state = State.Const; }
                            else Error("Ожидалось 'const'", i);
                            break;
                        case State.Const:
                            if (Regex.IsMatch(text.Substring(i), "^char\\b")) { tokens.Add("char"); i += 3; state = State.CharType; }
                            else Error("Ожидалось 'char'", i);
                            break;
                        case State.CharType:
                            if (char.IsLetter(c) || c == '_') { buffer += c; state = State.Identifier; }
                            else Error("Ожидался идентификатор", i);
                            break;
                        case State.Identifier:
                            if (char.IsLetterOrDigit(c) || c == '_') buffer += c;
                            else if (c == '[') { tokens.Add(buffer); buffer = ""; tokens.Add("["); state = State.BracketOpen; }
                            else if (c == '=') { tokens.Add(buffer); buffer = ""; tokens.Add("="); state = State.Equal; }
                            else Error("Некорректный идентификатор", i);
                            break;
                        case State.BracketOpen:
                            if (char.IsDigit(c)) { buffer += c; state = State.BracketNum; }
                            else if (c == ']') { tokens.Add("]"); state = State.Equal; }
                            else Error("Ожидалось число или ']' внутри скобок", i);
                            break;
                        case State.BracketNum:
                            if (char.IsDigit(c)) buffer += c;
                            else if (c == ']') { tokens.Add(buffer); buffer = ""; tokens.Add("]"); state = State.Equal; }
                            else Error("Некорректный размер массива", i);
                            break;
                        case State.Equal:
                            if (c == '=') { tokens.Add("="); state = State.Quote; }
                            else Error("Ожидался '='", i);
                            break;
                        case State.Quote:
                            if (c == '"') { tokens.Add("\""); state = State.String; }
                            else Error("Ожидалась '\"' для строки", i);
                            break;
                        case State.String:
                            if (c == '"') { tokens.Add(buffer); buffer = ""; tokens.Add("\""); state = State.End; }
                            else buffer += c;
                            break;
                        case State.End:
                            if (c == ';') { tokens.Add(";"); Console.WriteLine("Разбор завершен"); return; }
                            else Error("Ожидался ';'", i);
                            break;
                    }
                }
            }
            Error("Неожиданный конец выражения", 0);
        }

        private void Error(string message, int place)
        {
            this.data.Rows.Add(message, place);
            // Добавить нейтрализацию ошибки (например, вставить ожидаемый символ)
        }

        //public void PrintTokens()
        //{
        //    Console.WriteLine(string.Join(" ", tokens));
        //}
    }
}
