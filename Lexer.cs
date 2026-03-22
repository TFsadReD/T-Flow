using System;
using System.Collections.Generic;

namespace TFlow.Core
{
    public enum TokenType
    {
        // Ключевые слова для взаимодействия между файлами
        Use,

        // Типы данных
        Int, Float, Char, Str, Bool, Flow,

        // Значения
        True, False, Empty,

        // Литералы и названия переменных
        Identifier, Number, StringLiteral, CharLiteral,

        // Операторы и пунктуация
        Colon, Assign, Plus, Minus, Multiply, Divide,
        OpenBrace, CloseBrace, OpenParenthesis, CloseParenthesis,

        // Окончание файла
        EOF
    }

    public record Token(TokenType Type, string Value, int Line);

    public class Lexer
    {
        private readonly string _code;
        private int _index = 0;
        private int _line = 1;

        public Lexer(string code)
        {
            _code = code;
        }

        public List<Token> Scan()
        {
            var tokens = new List<Token>();

            while (!IsAtEnd())
            {
                char current = Peek();

                // Пропуск пробелов
                if (char.IsWhiteSpace(current))
                {
                    if (current == '\n') _line++;
                    Advance();
                    continue;
                }

                // Обработка комментариев
                if (current == '#')
                {
                    while (!IsAtEnd() && Peek() != '\n') Advance();
                    continue;
                }

                // Обработка букв, то есть ключевые слова и названия переменных
                if (char.IsLetter(current) || current == '_')
                {
                    string word = ReadWord();
                    tokens.Add(GetKeywordToken(word));
                    continue;
                }

                // Обработка чисел, а именно целые и с плавающей точкой
                if (char.IsDigit(current))
                {
                    tokens.Add(new Token(TokenType.Number, ReadNumber(), _line));
                    continue;
                }

                // Обработка строк
                if (current == '"')
                {
                    tokens.Add(new Token(TokenType.StringLiteral, ReadString(), _line));
                    continue;
                }

                // Обработка символов
                if (current == '\'')
                {
                    tokens.Add(new Token(TokenType.CharLiteral, ReadChar(), _line));
                    continue;
                }

                // Обработка одиночных символов (операторы и скобки)
                switch (current)
                {
                    case ':': tokens.Add(new Token(TokenType.Colon, ":", _line)); Advance(); break;
                    case '=': tokens.Add(new Token(TokenType.Assign, "=", _line)); Advance(); break;
                    case '+': tokens.Add(new Token(TokenType.Plus, "+", _line)); Advance(); break;
                    case '-': tokens.Add(new Token(TokenType.Minus, "-", _line)); Advance(); break;
                    case '*': tokens.Add(new Token(TokenType.Multiply, "*", _line)); Advance(); break;
                    case '/': tokens.Add(new Token(TokenType.Divide, "/", _line)); Advance(); break;
                    case '{': tokens.Add(new Token(TokenType.OpenBrace, "{", _line)); Advance(); break;
                    case '}': tokens.Add(new Token(TokenType.CloseBrace, "}", _line)); Advance(); break;
                    case '(': tokens.Add(new Token(TokenType.OpenParenthesis, "(", _line)); Advance(); break;
                    case ')': tokens.Add(new Token(TokenType.CloseParenthesis, ")", _line)); Advance(); break;
                    default:
                        // Твоя фишка: вызываем особую ошибку, если символ незнаком
                        throw new InvalidOperationException(); // ВЕРНУТЬСЯ!!!!!!!!!!!!!!!!!!!!!ноунейм символ!!!!!!!!!!!!!!!!!!!!!!!!
                        Advance();
                        break;
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", _line));
            return tokens;
        }

        private bool IsAtEnd() => _index >= _code.Length;
        private char Peek() => IsAtEnd() ? '\0' : _code[_index];
        private void Advance() => _index++;
        private string ReadWord()
        {
            int start = _index;
            while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
            {
                Advance();
            }
            return _code.Substring(start, _index - start);
        }
        private string ReadNumber()
        {
            int start = _index;
            while (!IsAtEnd() && (char.IsDigit(Peek()) || Peek() == '.'))
            {
                Advance();
            }
            return _code.Substring(start, _index - start);
        }
        private string ReadString()
        {
            Advance(); // Чтобы пропустить открывающуюся кавычку
            int start = _index;

            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\n') _line++; // если строка переносится на новую строку
                Advance();
            }

            if (IsAtEnd())
            {
                throw new InvalidOperationException(); // ВЕРНУТЬСЯ!!!!!!!!!!!!!!!!!!!!!забыта кавычка!!!!!!!!!!!!!!!!!!!!!!!!
            }

            string result = _code.Substring(start, _index - start);
            Advance(); // Чтобы пропустить закрывающуюся кавычку
            return result;
        }
        private string ReadChar()
        {
            Advance(); // Чтобы пропустить открывающуюся кавычку
            int start = _index;

            while (!IsAtEnd() && Peek() != '\'')
            {
                Advance();
            }

            if (IsAtEnd())
            {
                throw new InvalidOperationException(); // ВЕРНУТЬСЯ!!!!!!!!!!!!!!!!!!!!!забыта кавычка!!!!!!!!!!!!!!!!!!!!!!!!
            }

            string result = _code.Substring(start, _index - start);
            if (result.Length > 1)
            {
                throw new InvalidOperationException(); // ВЕРНУТЬСЯ!!!!!!!!!!!!!!!!!!!!!!только один символ в char!!!!!!!!!!!!!!!!!!!!!
            }
            Advance(); // Чтобы пропустить закрывающуюся кавычку
            return result;
        }

        private Token GetKeywordToken(string word) => word switch
        {
            "use" => new Token(TokenType.Use, word, _line),
            "int" => new Token(TokenType.Int, word, _line),
            "char" => new Token(TokenType.Char, word, _line),
            "str" => new Token(TokenType.Str, word, _line),
            "bool" => new Token(TokenType.Bool, word, _line),
            "float" => new Token(TokenType.Float, word, _line),
            "flow" => new Token(TokenType.Flow, word, _line),
            "Empty" => new Token(TokenType.Empty, word, _line),
            "True" => new Token(TokenType.True, word, _line),
            "False" => new Token(TokenType.False, word, _line),
            _ => new Token(TokenType.Identifier, word, _line),
        };
    }
}
