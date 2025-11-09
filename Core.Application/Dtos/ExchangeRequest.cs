using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Dtos
{
    public class ExchangeRequest
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required decimal Amount { get; set; }
    }
}
