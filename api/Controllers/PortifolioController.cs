using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portifolio")]
    [ApiController]
    [Authorize]
    public class PortifolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortifolioRepository _portifolioRepository;
        public PortifolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortifolioRepository portifolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portifolioRepository = portifolioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portifolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepository.GetStockBySymbol(symbol);

            if (stock == null) return BadRequest("Stock not found");

            var userPortfolio = await _portifolioRepository.GetUserPortfolio(appUser);

            if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Connot add same stock to portifolio");

            var portifolioModel = new Portifolio
            {
                AppUserId = appUser.Id,
                StockId = stock.Id
            };

            var portifolio = await _portifolioRepository.CreateAsync(portifolioModel);

            if (portifolioModel == null) return StatusCode(500, "Could not create");

            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portifolioRepository.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if (filteredStock.Count == 0) return BadRequest("Stock not found");

            await _portifolioRepository.DeletePortfolio(appUser, symbol);

            return Ok();
        }
    }
}