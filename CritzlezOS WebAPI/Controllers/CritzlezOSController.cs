using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using CritzlezOS;
using System.Net.WebSockets;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CritzlezOS_WebAPI.Controllers
{
    public class Startup
    {
        public string botStart { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CritzlezOSController : ControllerBase
    {
        [HttpPut("botstart")]
        public async Task<IActionResult> BotStart(Startup x)
        {
            if (x.botStart == "true") await MainBot.Main();
            else await MainBot.StopAsync();

            return Ok();
        }

        [HttpGet("botstartstatus")]
        public async Task<IActionResult> BotStartStatus()
        {
            Startup s = new();
            string isBotOn = MainBot.IsBotOn.ToString();

            if (isBotOn == "True")
                s.botStart = "started";
            else
                s.botStart = "stopped";



            return Ok(s);
        }
    }
}
