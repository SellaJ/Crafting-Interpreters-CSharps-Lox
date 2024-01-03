using Lox;
namespace Statement;
public abstract class Stmt
{

    public abstract R Accept<R>(Visitor<R> visitor);
}
public interface Visitor<R>
{
    R VisitExpresionStmt(Expresion stmt);
    R VisitPrintStmt(Print stmt);
}
public class Expresion : Stmt
{
    public Expresion(Expr expression)
    {
        this.expression = expression;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitExpresionStmt(this); }
    internal Expr expression { get; }
}
public class Print : Stmt
{
    public Print(Expr expression)
    {
        this.expression = expression;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitPrintStmt(this); }
    internal Expr expression { get; }
}
