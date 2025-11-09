using Core.Domain.Common;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces
{
    public interface IExchangeService
    {
        Task<GenericResponse<ExchangeResults?>> GetBestRateAsync(string from, string to, decimal amount);
        Task<GenericResponse<List<ExchangeResults?>>> GetRatesAsync(string from, string to, decimal amount);
    }
}
