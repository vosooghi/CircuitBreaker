using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Model
{
    public class ClientMessage:Message
    {
        public string ClientText { get; set; } = "This is Client Message";
    }
}
