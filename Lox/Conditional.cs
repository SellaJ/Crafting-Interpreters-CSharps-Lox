namespace Lox
{
    internal class Conditional : Expr
    {
        private Expr expr;
        private Expr thenBranch;
        private Expr elseBranch;

        public Conditional(Expr expr, Expr thenBranch, Expr elseBranch)
        {
            this.expr = expr;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        public override R Accept<R>(Visitor<R> visitor)
        {
            throw new NotImplementedException();
        }
    }
}