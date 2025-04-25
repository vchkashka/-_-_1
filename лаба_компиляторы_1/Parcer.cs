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
        private int pos = 0;
        private string currentToken;

        private List<string> poliz = new List<string>();
        public string GetPOLIZ() {
            string result = "";
            foreach (string pol in poliz) 
            {
                result += pol + " ";
            }
            return result;
        }

        DataGridView data = new DataGridView();
        RichTextBox rtb = new RichTextBox();

        int countErrors;

        public int CountErrors {  get { return countErrors; } }

        public Parcer(string input, DataGridView data, RichTextBox rtb)
        {
            this.input = input.Replace(" ", "");
            this.data = data;
            this.rtb = rtb;
            NextToken();
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

        void NextToken()
        {
            if (pos >= input.Length)
            {
                currentToken = null;
                return;
            }

            if (char.IsDigit(input[pos]))
            {
                int start = pos;
                while (pos < input.Length && char.IsDigit(input[pos]))
                    pos++;
                currentToken = input.Substring(start, pos - start);
            }
            else
            {
                currentToken = input[pos].ToString();
                pos++;
            }
        }

        void Match(string expected)
        {
            if (currentToken == expected)
            {
                NextToken();
            }
            else
            {
                Error($"Ожидалось '{expected}'", pos);
                ErrorSelection(pos-1, rtb);
                countErrors++;
            }
        }

        public void Parse()
        {
            E();
            if (currentToken != null)
            {
                Error("Неожиданный символ", pos);
                ErrorSelection(pos-1, rtb);
                countErrors++;
            }
        }

        // E → T A
        void E()
        {
            T();
            A();
        }

        // A → ε | + T A | - T A
        void A()
        {
            if (currentToken == "+" || currentToken == "-")
            {
                string op = currentToken;
                Match(op);
                T();
                poliz.Add(op);
                A();
            }
            // else ε (ничего не делаем)
        }

        // T → O B
        void T()
        {
            O();
            B();
        }

        // B → ε | * O B | / O B
        void B()
        {
            if (currentToken == "*" || currentToken == "/")
            {
                string op = currentToken;
                Match(op);
                O();
                poliz.Add(op);
                B();
            }
            // else ε
        }

        // O → num | ( E )
        void O()
        {
            if (IsNumber(currentToken))
            {
                poliz.Add(currentToken);
                Match(currentToken); // число
            }
            else if (currentToken == "(")
            {
                Match("(");
                E();
                Match(")");
            }
            else
            {
                Error($"Ожидалось число или (", pos);
                ErrorSelection(pos-1, rtb);
                countErrors++;
            }
        }

        bool IsNumber(string token)
        {
            return token != null && Regex.IsMatch(token, @"^\d+$");
        }

        private void Error(string message, int place)
        {
            this.data.Rows.Add(message, place);
        }

        public double EvaluatePOLIZ()
        {
            Stack<double> stack = new Stack<double>();

            foreach (string token in poliz)
            {
                if (Regex.IsMatch(token, @"^\d+$")) // число
                {
                    stack.Push(double.Parse(token));
                }
                else
                {
                    double b = stack.Pop();
                    double a = stack.Pop();
                    double result = 0;
                    switch (token)
                    {
                        case "+":  result = a + b; break;
                        case "-": result = a - b; break;
                        case "*": result = a * b; break;
                        case "/": result = a / b; break;
                        default: throw new Exception($"Неизвестный оператор: {token}");
                    };
                    stack.Push(result);
                }
            }

            if (stack.Count != 1)
                throw new Exception("Ошибка вычисления ПОЛИЗ");

            return stack.Pop();
        }

    }
}
