using Lox;

namespace Statement;
public abstract class Stmt
{

    public abstract R Accept<R>(Visitor<R> visitor);
}
public interface Visitor<R>
{
    R VisitBlockStmt(Block stmt);
    R VisitExpresionStmt(Expresion stmt);
    R VisitPrintStmt(Print stmt);
    R VisitVarStmt(Var stmt);
}
public class Block : Stmt
{
    public Block(List<Stmt> statements)
    {
        this.statements = statements;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitBlockStmt(this); }
    internal List<Stmt> statements { get; }
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
public class Var : Stmt
{
    public Var(Token name, Expr initializer)
    {
        this.name = name;
        this.initializer = initializer;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitVarStmt(this); }
    internal Token name { get; }
    internal Expr initializer { get; }
}
