using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IPortifolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portifolio> CreateAsync(Portifolio portifolio);
        Task<Portifolio> DeletePortfolio(AppUser user, string symbol);
    }
}