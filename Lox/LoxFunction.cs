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
        private Function decleration;
        private Environment closure;

        public LoxFunction(Function decleration, Environment closure)
        {
            this.decleration = decleration;
            this.closure = closure;
        }

        public int Arity => decleration.Params.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(closure);
            for (int i = 0; i < decleration.Params.Count; i++)
            {
                environment.Define(decleration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(decleration.body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {decleration.name.Lexeme}>";
        }
    }
}
