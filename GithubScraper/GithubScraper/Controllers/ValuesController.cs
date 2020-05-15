using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GithubScraper.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GithubScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Concat '/user/repo' to end of url and hit enter. Ex: https://localhost:44323/api/values/Deuclerio/scrapertestdata";
        }

        // GET api/values/5
        [HttpGet("{user}/{repo}")]
        public ActionResult<List<ScrapeResult>> Get(string user, string repo)
        {
            return new Scraper().Execute(user, repo);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
