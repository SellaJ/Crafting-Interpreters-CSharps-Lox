using Lox;

namespace Statement;
                                  public abstract class Stmt
     {
        
 public abstract R Accept<R>(Visitor<R> visitor);
}   public interface Visitor<R> {
 R VisitBlockStmt(Block stmt);
 R VisitBreakStmt(Break stmt);
 R VisitExpressionStmt(Expression stmt);
 R VisitFunctionStmt(Function stmt);
 R VisitIfStmt(If stmt);
 R VisitPrintStmt(Print stmt);
 R VisitReturnStmt(Return stmt);
 R VisitVarStmt(Var stmt);
 R VisitWhileStmt(While stmt);
}
public class Block : Stmt
    {   public  Block (List<Stmt> statements) 
     {
     this.statements = statements;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitBlockStmt(this);}    internal List<Stmt> statements { get; }
        }
public class Break : Stmt
    {   public  Break () 
     {

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitBreakStmt(this);}
        }
public class Expression : Stmt
    {   public  Expression (Expr expression) 
     {
     this.expression = expression;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitExpressionStmt(this);}    internal Expr expression { get; }
        }
public class Function : Stmt
{
    public Function(Token name, Lox.Function function)
    {
     this.name = name;
     this.function = function;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitFunctionStmt(this);}    internal Token name { get; }    internal Lox.Function function { get; }
        }
public class If : Stmt
    {   public  If (Expr condition, Stmt thenBranch, Stmt elseBranch) 
     {
     this.condition = condition;
     this.thenBranch = thenBranch;
     this.elseBranch = elseBranch;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitIfStmt(this);}    internal Expr condition { get; }    internal Stmt thenBranch { get; }    internal Stmt elseBranch { get; }
        }
public class Print : Stmt
    {   public  Print (Expr expression) 
     {
     this.expression = expression;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitPrintStmt(this);}    internal Expr expression { get; }
        }
public class Return : Stmt
    {   public  Return (Token name, Expr value) 
     {
     this.name = name;
     this.value = value;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitReturnStmt(this);}    internal Token name { get; }    internal Expr value { get; }
        }
public class Var : Stmt
    {   public  Var (Token name, Expr initializer) 
     {
     this.name = name;
     this.initializer = initializer;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitVarStmt(this);}    internal Token name { get; }    internal Expr initializer { get; }
        }
public class While : Stmt
    {   public  While (Expr condition, Stmt body) 
     {
     this.condition = condition;
     this.body = body;

        }
public override R Accept<R>(Visitor<R> visitor) {return visitor.VisitWhileStmt(this);}    internal Expr condition { get; }    internal Stmt body { get; }
        }
