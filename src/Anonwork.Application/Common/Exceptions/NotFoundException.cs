using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Domain.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"{name} with id '{key}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
