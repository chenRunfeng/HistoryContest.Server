using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HistoryContest.Server.Services;
using HistoryContest.Server.Models.ViewModels;
using System.Security.Claims;

namespace HistoryContest.Server.Controllers.APIs
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService accountService;

        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Body JSON content invalid");
            }

            var userContext = await accountService.ValidateUser(model.UserName, model.Password);
            if (userContext.User != null)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(userContext.Claims, accountService.GetType().Name));
#if NETCOREAPP2_0
                await HttpContext.SignInAsync(principal);
#else
                await HttpContext.Authentication.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal);
#endif
                HttpContext.Session.SetString("id", userContext.User.UserName);
                return Json(new { isSuccessful = true, userContext.User });
            }
            else
            {
                return Json(new { isSuccessful = false });
            }
        }

        //[AllowAnonymous]
        //[HttpPost("[action]")]
        //public IActionResult Register(RegistrationViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    UserViewModel user = accountService.CreateUser(model.UserName, model.Password, model.role?? "Student");
        //    if (user != null)
        //    {
        //        return Json(new { isSuccessful = true, user });
        //    }
        //    else
        //    {
        //        return Json(new { isSuccessful = false });
        //    }
        //}

        [HttpPost("[action]")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return SignOut();
        }
    }
}