using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GithubScraper.Domain;
using GithubScraper.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace GithubScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Concat '/user/repo' to end of url and hit enter. Ex: https://localhost:44323/api/values/Deuclerio/scrapertestdata";
        }

        // GET api/values/5
        [HttpGet("{user}/{repo}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult<List<ScrapeResult>> Get(string user, string repo)
        {
            return new Scraper().Execute(user, repo);
        }
    }
}
