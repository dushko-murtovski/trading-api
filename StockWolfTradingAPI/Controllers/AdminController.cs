using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockwolfTrading.ViewModel;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWolfTradingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITickerService _tickerService;
        private readonly ITradeService _tradeService;
        private readonly IDailyAnalysisService _dailyAnalysisService;
        private readonly ISettingsService _settingsService;
        public AdminController(IUserService userService, ITickerService tickerService, ITradeService tradeService
            , IDailyAnalysisService dailyAnalysisService, ISettingsService settingsService)
        {
            _userService = userService;
            _tickerService = tickerService;
            _tradeService = tradeService;
            _dailyAnalysisService = dailyAnalysisService;
            _settingsService = settingsService;
        }

        [HttpGet("getsettings")]
        public IActionResult GetSettings()
        {
            SettingsViewModel svm = new SettingsViewModel();
            svm.DaysBack = _settingsService.GetByName("DaysBack").SettingValue;
            return Ok(svm);
        }

        [HttpPost("savesettings")]
        public IActionResult SaveSettings(JObject data)
        {
            var setting = JsonConvert.DeserializeObject<SettingsViewModel>(data.ToString());
            try
            {
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}
