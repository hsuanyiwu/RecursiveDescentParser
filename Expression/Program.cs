using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace RecursiveDescentParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var expr = new Expression();
            expr.Eval("((1-50)-2)*31");
        }
    }

    public class Expression
    {
        enum Token
        {
            NONE, EOF,
            PLUS, MINUS, MULT, DIV,
            LBRACKET, RBRACKET,
            NUM,
        }

        private char _c;
        private char[] _input;
        private int _i;

        private Token _t;
        private int _ival;
        private string _text;

        private Dictionary<char, Token> _symbol = new Dictionary<char, Token>();

        public Expression()
        {
            _symbol.Add('(', Token.LBRACKET);
            _symbol.Add(')', Token.RBRACKET);
            _symbol.Add('+', Token.PLUS);
            _symbol.Add('-', Token.MINUS);
            _symbol.Add('*', Token.MULT);
            _symbol.Add('/', Token.DIV);

        }

        private Token lex()
        {
            if (_c == '\0')
            {
                _c = getch();
            }

            if (_c == '\0')
            {
                return Token.NONE;
            }

            // whitespce
            if (char.IsWhiteSpace(_c))
            {
                do
                {
                    _c = getch();
                }
                while (char.IsWhiteSpace(_c));
            }

            // number
            if (char.IsDigit(_c))
            {
                var sb = new StringBuilder();
                do
                {
                    sb.Append(_c);
                    _c = getch();
                }
                while (char.IsDigit(_c));

                _text = sb.ToString();
                _ival = int.Parse(_text);
                return Token.NUM;
            }

            // symbol
            if (_symbol.TryGetValue(_c, out Token t))
            {
                _c = '\0';
                return t;
            }

            throw new Exception("out of input buffer");
        }

        private char getch()
        {
            if (_i >= _input.Length)
                return '\0';
            return _input[_i++];
        }

        private void advance()
        {
            _t = lex();
        }

        private void eat(Token t)
        {
            if (_t != t)
                throw new Exception("syntex error");
            advance();
        }

        public void Eval(string expr)
        {
            _input = expr.ToCharArray();
            _c = '\0';
            _i = 0;

            Console.WriteLine("{0}:", expr);
            advance();
            E();
        }


        /*
         * S->E$
         * 
         * E->TE`
         * 
         * E`->+TE`
         * E`->-TE`
         * E`->
         * 
         * T->FT`
         * 
         * T`->*FT`
         * T`->\FT`
         * T`->
         * 
         * F->NUM
         * F->(E)
         */


        private void E()
        {
            T(); E_();
        }

        private void E_()
        {
            switch (_t)
            {
            case Token.PLUS:
                advance(); T();
                Console.WriteLine("+");
                E_();
                break;

            case Token.MINUS:
                advance(); T();
                Console.WriteLine("-");
                E_();
                break;

            default:
                break;
            }
        }

        private void T()
        {
            F(); T_();
        }

        private void F()
        {
            switch (_t)
            {
            case Token.NUM:
                advance();
                Console.WriteLine("NUM({0})", _ival);
                break;

            case Token.LBRACKET:
                advance(); E(); eat(Token.RBRACKET);
                break;

            default:
                throw new Exception("syntex error");
            }
        }

        private void T_()
        {
            switch (_t)
            {
            case Token.MULT:
                advance(); F();
                Console.WriteLine("*");
                T_();
                break;

            case Token.DIV:
                advance(); F();
                Console.WriteLine("/");
                T_();
                break;
            }
        }
    }
}
