namespace Lox;
public abstract class Expr
{

    public abstract R Accept<R>(Visitor<R> visitor);
}
public interface Visitor<R>
{
    R VisitAssignExpr(Assign expr);
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
    R VisitVariableExpr(Variable expr);
}
public class Assign : Expr
{
    public Assign(Token name, Expr value)
    {
        this.name = name;
        this.value = value;

    }
    public override R Accept<R>(Visitor<R> visitor) { return visitor.VisitAssignExpr(this); }
    internal Token name { get; }
    internal Expr value { get; }
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
