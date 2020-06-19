using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SwaggerSecurityTrimming
{
    [Route("protected")]
    [Authorize(policy: "can-update")]
    public class ValuesProtectedController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "secret1", "secret2" };
        }
    }
}
