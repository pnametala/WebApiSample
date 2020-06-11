using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplicationSample.Models;

namespace WebApplicationSample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {

        [HttpPost]
        public IActionResult Post(MatchEntity match)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var occurrences = match.FindOccurrences();
            
                var result = new MatchResult(match);
                result.Occurrences.AddRange(occurrences.Select(occurence => new MatchOccurrence()
                {
                    Index = occurence.Index,
                    Length = occurence.Length,
                    Value = occurence.Value
                }));
            
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}