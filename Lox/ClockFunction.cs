using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class ClockFunction : LoxCallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)(DateTime.Now - DateTime.MinValue).TotalSeconds;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
