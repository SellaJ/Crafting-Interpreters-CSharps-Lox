using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class Environment
    {
        private Environment? enclosing;
        private Dictionary<string, object> values = new();

        public Environment() {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                var value = values[name.Lexeme];
                if (value == null)
                {
                    throw new RuntimeError(name, $"Variable '{name.Lexeme}' is nil");
                }
                return value;
            }

            if(enclosing != null) return enclosing.Get(name);

            throw new RuntimeError(name, $"Undefined varible '{name.Lexeme}'");
        }

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public void Assign(Token name, object value)
        {
            if(values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if(enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

    }
}
