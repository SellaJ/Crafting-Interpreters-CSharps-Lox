using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class Return : Exception
    {
        internal object value;

        public Return(object value) : base(null, null)
        {
            this.value = value;
        }
    }
}
