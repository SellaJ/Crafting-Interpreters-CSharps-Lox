using Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox;

public class Interpreter : Visitor<object>, Statement.Visitor<LoxVoid>
{
    internal Lox.Environment globals = new();
    private Lox.Environment environment;

    public Interpreter()
    {
        environment = globals;
        globals.Define("clock", new ClockFunction());
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            CsLox.RuntimeError(error);
        }
    }
    public object VisitBinaryExpr(Binary expr)
    {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                {
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string || right is string)
                    {
                        return left.ToString() + right.ToString();
                    }

                    throw new RuntimeError(expr.Operator,
                        "Opeands must be two numbers or two strings");
                }
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                if ((double)right == 0) throw new RuntimeError(expr.Operator, "Cannot divide by zero");
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
        }

        // Unrechable
        return null;
    }
    public object VisitGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }
    public object VisitLiteralExpr(Literal expr)
    {
        return expr.Value;
    }
    public object VisitUnaryExpr(Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
        }

        return null;
    }
    public object VisitAssignExpr(Assign expr)
    {
        object value = Evaluate(expr.Value);
        environment.Assign(expr.Name, value);
        return value;
    }
    public object VisitVariableExpr(Variable expr)
    {
        return environment.Get(expr.Name);
    }
    public object VisitLogicalExpr(Logical expr)
    {
        object left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
    }
    public object VisitCallExpr(Call expr)
    {
        object callee = Evaluate(expr.Callee);

        List<object> arguments = new List<object>();

        foreach (Expr argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }

        if (!(callee is LoxCallable))
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }

        LoxCallable function = (LoxCallable)callee;
        if (arguments.Count != function.Arity)
        {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
        }
        return function.Call(this, arguments);
    }
    public LoxVoid VisitPrintStmt(Statement.Print stmt)
    {
        object value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return null;
    }
    public LoxVoid VisitVarStmt(Var stmt)
    {
        object value = null;
        if (stmt.initializer != null)
        {
            value = Evaluate(stmt.initializer);
        }

        environment.Define(stmt.name.Lexeme, value);
        return null;
    }
    public LoxVoid VisitBlockStmt(Block stmt)
    {
        ExecuteBlock(stmt.statements, new Environment(environment));
        return null;
    }
    public LoxVoid VisitIfStmt(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.thenBranch);
        }
        else
        {
            Execute(stmt.elseBranch);
        }

        return null;
    }
    public LoxVoid VisitWhileStmt(While stmt)
    {
        try
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
        }
        catch (BreakException)
        {
        }

        return null;
    }
    public LoxVoid VisitExpressionStmt(Expression stmt)
    {
        Evaluate(stmt.expression);
        return null;
    }
    public LoxVoid VisitBreakStmt(Break stmt)
    {
        throw new BreakException();
    }
    public LoxVoid VisitFunctionStmt(Function stmt)
    {
        LoxFunction function = new LoxFunction(stmt, environment);
        environment.Define(stmt.name.Lexeme, function);
        return null;
    }
    public LoxVoid VisitReturnStmt(Statement.Return stmt)
    {
        object value = null;
        if(stmt.value != null) value = Evaluate(stmt.value);

        throw new Return(value);
    }

    private string Stringify(object obj)
    {
        if (obj == null)
        {
            return "nil";
        }

        if (obj is double)
        {
            string text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        if (obj is string) return obj.ToString().Replace("\"", "");
        return obj.ToString();
    }
    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
    internal void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = this.environment;
        try
        {
            this.environment = environment;
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }
    private void CheckNumberOperands(Token @operator, object left, object right)
    {
        if (left is double && right is double)
        {
            return;
        }

        throw new RuntimeError(@operator, "Operands must be numbers.");
    }
    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }
    private void CheckNumberOperand(Token @operator, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(@operator, "Operand must be a numbewr.");
    }
    private bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }
    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

}
