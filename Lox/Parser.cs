using Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lox
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();

            while(!IsAtEnd()) {
                statements.Add(Statement());
            }

            return statements;
        }


        private Expr Expression()
        {
            return Comma();
        }

        private Stmt Statement()
        {
            if (match(TokenType.PRINT)) return PrintStatement();

            return ExpressionStatement();
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Statement.Expresion(expr);
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Statement.Print(value);
        }

        private Expr Comma()
        {
            Expr expr = Equality();

            while (match(TokenType.COMMA))
            {
                Token Operator = Previous();
                Expr right = Equality();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr conditional()
        {
            Expr expr = Equality();

            if (match(TokenType.QUESTION))
            {
                Expr thenBranch = Expression();
                Consume(TokenType.COLON,
                    "Expect ':' after then branch of conditional expression.");
                Expr elseBranch = conditional();
                expr = new Conditional(expr, thenBranch, elseBranch);
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token Operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private bool match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token Operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (match(TokenType.MINUS, TokenType.PLUS))
            {
                Token Operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = unary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token Operator = Previous();
                Expr right = unary();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr unary()
        {
            if (match(TokenType.MINUS, TokenType.BANG))
            {
                Token Operator = Previous();
                Expr right = unary();
                return new Unary(Operator, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (match(TokenType.FALSE)) return new Literal(false);
            if (match(TokenType.TRUE)) return new Literal(true);
            if (match(TokenType.NIL)) return new Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (match(TokenType.LEFT_PARN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PARN, "Expect ')' aftrer expression.");
                return new Grouping(expr);
            }

            // Error productions.
            if (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Equality();
                return null;
            }

            if (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Comparison();
                return null;
            }

            if (match(TokenType.PLUS))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Term();
                return null;
            }

            if (match(TokenType.SLASH, TokenType.STAR))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Factor();
                return null;
            }


            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            CsLox.Error(token, message);
            return new ParseError();
        }

        private void Synchtonize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }

    public class ParseError : Exception { }
}
