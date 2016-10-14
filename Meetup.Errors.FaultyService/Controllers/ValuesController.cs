using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Errors.FaultyService.Controllers
{
    [Route("api/[controller]")]
    public class FaultyController
    {
        [HttpGet]
        public string Get()
        {
            Console.WriteLine("Inside controller");

            throw new Exception("by design");
            //return "Ok this time";
        }
    }

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
