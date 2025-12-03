using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Errors
{
    public class ErrorAlCliente:Exception
    {
        public int ErrorCode;
        public ErrorAlCliente(string message) : base(message) {
            this.ErrorCode = -1;
        }
        public ErrorAlCliente(string message, int CodigoError) : base(message) { 
            this.ErrorCode = CodigoError;
        }
    }
}
