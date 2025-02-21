using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PrimeVilla_Utility;
using PrimeVilla_Web.Models;
using PrimeVilla_Web.Models.DTO;
using PrimeVilla_Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PrimeVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
            if(response != null && response.IsSuccess)
            {
                LoginResponseDTO model =  JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.Token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim (ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u=>u.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken, model.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.Admin, Value = SD.Admin},
                new SelectListItem { Text = SD.Customer, Value = SD.Customer }
            };
            ViewBag.RoleList = roleList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            if (string.IsNullOrEmpty(obj.Role))
            {
                obj.Role = SD.Customer;
            }
            APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);
            if(result != null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.Admin, Value = SD.Admin},
                new SelectListItem { Text = SD.Customer, Value = SD.Customer }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken,"");
            return RedirectToAction("login","Auth");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }   
}
