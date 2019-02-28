using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;

namespace API.Controllers
{

  [Route("api/[controller]")]
  [ApiController]
  public class StringController : ControllerBase
  {
    public StringController()
    {
    }

    // Get api/string
    [HttpGet("{input}")]
    public IActionResult ConvertString(string input)
    {
      return Ok(StringParanthesisUnwrapper.ParseString(input));
    }

  }
}
