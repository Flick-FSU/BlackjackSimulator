using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackjackSimulator.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlackjackWeb.Controllers
{
    public class BlackjackSimController : Controller
    {
        public IActionResult BlackjackSimSetup()
        {
            var simulationProperties =
                new SimulationProperties {PlayerPropertiesCollection = new List<PlayerProperties>()};
            return View(simulationProperties);
        }
    }
}