using Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class LoxFunction : LoxCallable
    {
        private readonly string name;
        private readonly Lox.Function decleration;
        private readonly Environment closure;

        public LoxFunction(string name, Lox.Function decleration, Environment closure)
        {
            this.name = name;
            this.decleration = decleration;
            this.closure = closure;
        }

        public int Arity => decleration.Parameters.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(closure);
            for (int i = 0; i < decleration.Parameters.Count; i++)
            {
                environment.Define(decleration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(decleration.Body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {name}>";
        }
    }
}
