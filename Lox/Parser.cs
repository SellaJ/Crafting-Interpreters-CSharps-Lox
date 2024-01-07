using Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lox
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current;
        private int loopDepth = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Decleration());
            }

            return statements;
        }

        private Stmt Decleration()
        {
            try
            {
                if (Match(TokenType.FUN)) return this.Function("function");
                if (Match(TokenType.VAR)) return VarDecleration();

                return Statement();
            }
            catch (ParseError e)
            {
                Syncronize();
                return null;
            }
        }

        private Function Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
            Consume(TokenType.LEFT_PARN, $"Expect '(' after {kind} name.");
            List<Token> parameters = new List<Token>();
            if (!Check(TokenType.RIGHT_PARN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name"));
                } while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RIGHT_PARN, "Expect ')' after parameters.");

            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            List<Stmt> body = Block();
            return new Function(name, parameters, body);
        }

        private Stmt VarDecleration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Var(name, initializer);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Stmt Statement()
        {
            if (Match(TokenType.BREAK)) return BreakStatement();
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Block(Block());

            return ExpressionStatement();
        }

        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Expr value = null;
            if(!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value");
            return new Statement.Return(keyword, value);
        }

        private Stmt BreakStatement()
        {
            if (loopDepth == 0)
            {
                Error(Previous(), "Must be inside a loop to use 'break'.");
            }
            Consume(TokenType.SEMICOLON, "Expected ';' after break");
            return new Break();
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PARN, "Expect '(' after 'for'.");
            Stmt initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDecleration();
            }
            else initializer = ExpressionStatement();

            Expr condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition");

            Expr increment = null;

            if (!Check(TokenType.RIGHT_PARN))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PARN, "Expect ')' after for clauses.");

            try
            {
                loopDepth++;
                Stmt body = Statement();
                if (increment != null)
                {
                    body = new Block(new List<Stmt> { body, new Statement.Expression(increment) });
                }

                if (condition == null)
                {
                    condition = new Literal(true);
                }

                body = new While(condition, body);

                if (initializer != null)
                {
                    body = new Block(new List<Stmt> { initializer, body });
                }

                return body;
            }
            finally
            {
                loopDepth--;
            }
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PARN, "Expect '(' aftrer 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PARN, "Expect ')' after condition.");
            try
            {
                loopDepth++;
                Stmt body = Statement();

                return new While(condition, body);
            }
            finally
            {
                loopDepth--;
            }
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PARN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PARN, "Expect ')' after if condition");

            Stmt thenBranch = Statement();
            Stmt elseBranch = Statement();
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new If(condition, thenBranch, elseBranch);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Decleration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Statement.Expression(expr);
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

            while (Match(TokenType.COMMA))
            {
                Token Operator = Previous();
                Expr right = Equality();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Comma();

                if (expr is Variable)
                {
                    Token name = ((Variable)expr).Name;
                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();

            while (Match(TokenType.OR))
            {
                Token @operator = Previous();
                Expr right = And();
                expr = new Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Comma();

            while (Match(TokenType.AND))
            {
                Token @operator = Previous();
                Expr right = And();
                expr = new Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr conditional()
        {
            Expr expr = Equality();

            if (Match(TokenType.QUESTION))
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

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
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

        private bool Match(params TokenType[] types)
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

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
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

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token Operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token Operator = Previous();
                Expr right = Unary();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.MINUS, TokenType.BANG))
            {
                Token Operator = Previous();
                Expr right = Unary();
                return new Unary(Operator, right);
            }

            return Call();
        }

        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PARN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr FinishCall(Expr calle)
        {
            List<Expr> arguments = new List<Expr>();
            if (!Check(TokenType.RIGHT_PARN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Cant have more than 255 arguments.");
                    }
                    arguments.Add(Equality());
                } while (Match(TokenType.COMMA));
            }

            Token paren = Consume(TokenType.RIGHT_PARN, "Expect ')' after arguments.");

            return new Call(calle, paren, arguments);
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
            }

            if (Match(TokenType.LEFT_PARN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PARN, "Expect ')' aftrer expression.");
                return new Grouping(expr);
            }

            // Error productions.
            if (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Equality();
                return null;
            }

            if (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Comparison();
                return null;
            }

            if (Match(TokenType.PLUS))
            {
                CsLox.Error(Previous(), "Missing left-hand operand.");
                Term();
                return null;
            }

            if (Match(TokenType.SLASH, TokenType.STAR))
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

        private void Syncronize()
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
