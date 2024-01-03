using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class RuntimeError : Exception
    {
        public Token token;

        public RuntimeError(Token token, string messages) : base(messages)
        {
            this.token = token;
        }
    }
}
