namespace Lox;
public abstract class Expr
{

    public abstract R Accept<R>(Visitor<R> visitor);
}
public interface Visitor<R>
{
    R VisitAssignExpr(Assign expr);
    R VisitBinaryExpr(Binary expr);
    R VisitCallExpr(Call expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitLogicalExpr(Logical expr);
    R VisitUnaryExpr(Unary expr);
    R VisitVariableExpr(Variable expr);
}
public class Assign : Expr
{
    public Assign(Token Name, Expr Value)
    {
        this.Name = Name;
        this.Value = Value;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitAssignExpr(this); }
    internal Token Name { get; }
    internal Expr Value { get; }
}
public class Binary : Expr
{
    public Binary(Expr Left, Token Operator, Expr Right)
    {
        this.Left = Left;
        this.Operator = Operator;
        this.Right = Right;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitBinaryExpr(this); }
    internal Expr Left { get; }
    internal Token Operator { get; }
    internal Expr Right { get; }
}
public class Call : Expr
{
    public Call(Expr Callee, Token Paren, List<Expr> Arguments)
    {
        this.Callee = Callee;
        this.Paren = Paren;
        this.Arguments = Arguments;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitCallExpr(this); }
    internal Expr Callee { get; }
    internal Token Paren { get; }
    internal List<Expr> Arguments { get; }
}
public class Grouping : Expr
{
    public Grouping(Expr Expression)
    {
        this.Expression = Expression;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitGroupingExpr(this); }
    internal Expr Expression { get; }
}
public class Literal : Expr
{
    public Literal(Object Value)
    {
        this.Value = Value;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitLiteralExpr(this); }
    internal Object Value { get; }
}
public class Logical : Expr
{
    public Logical(Expr Left, Token Operator, Expr Right)
    {
        this.Left = Left;
        this.Operator = Operator;
        this.Right = Right;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitLogicalExpr(this); }
    internal Expr Left { get; }
    internal Token Operator { get; }
    internal Expr Right { get; }
}
public class Unary : Expr
{
    public Unary(Token Operator, Expr Right)
    {
        this.Operator = Operator;
        this.Right = Right;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitUnaryExpr(this); }
    internal Token Operator { get; }
    internal Expr Right { get; }
}
public class Variable : Expr
{
    public Variable(Token Name)
    {
        this.Name = Name;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitVariableExpr(this); }
    internal Token Name { get; }
}
