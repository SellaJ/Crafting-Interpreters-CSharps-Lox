using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class AstPrinter : Visitor<string>
    {
        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{").Append(name);
            foreach (Expr e in exprs)
            {
                sb.Append(" ");
                sb.Append(e.Accept(this));
            }
            sb.Append("}");
            return sb.ToString();
        }
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
    }

    public class RpnAstPrinter : Visitor<string>
    {
        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (Expr e in exprs)
            {
                sb.Append(" ");
                sb.Append(e.Accept(this));
            }
            sb.Append(" "+name);
            sb.Append(" }");
            return sb.ToString();
        }
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
    }
}
