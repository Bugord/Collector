using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Authorization;
using Collector.BL.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{
    [ApiController]
    [Route("register")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AccountController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        public string Get(LoginDto model)
        {
            string messageToVisitor = "You are not logged.";
            if (User.Identity.IsAuthenticated)
            {
                messageToVisitor = $"Hello, {User.Claims.First().Value}.";
            }

            return DateTime.Now + "\n" + messageToVisitor;
        }
        // GET api/values
        [HttpPost]
        public async Task Post(LoginDto model)
        {

            var test = new RegisterDto()
            {
                Email = "email",
                FirstName = "first name",
                LastName = "last name",
                Password = "password",
                Username = "username"
            };
            await _tokenService.RegisterAsync(test);
        }

        // GET api/values/5
        [HttpGet("{c}")]
        public ActionResult<string> Get(int a, int b, int c)
        {
           
            return "value " + a + " " + b + " " + c;
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

