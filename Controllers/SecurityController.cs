using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Identity;
using MovieApp.Web.Models;
using MovieApp.Web.Models.Account;
using System;
using System.Threading.Tasks;

namespace MovieApp.Web.Controllers
{

    public class SecurityController : Controller
    {

        private UserManager<AppIdentityUser> _userManager;

        private SignInManager<AppIdentityUser> _signInManager;

        public SecurityController(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
       
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInModel model)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user!=null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(String.Empty,"Email doğruşaması yapılmadı");
                    return View(model);
                }

            }
                    
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password,false,false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index","Home");
            }

            ModelState.AddModelError(String.Empty,"Kullanıcı adı veya şifre hatalı");
            return View(model);
            
        }
   
    
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppIdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
               
                
            };

            var result = await _userManager.CreateAsync(user,model.Password);
           
            if (result.Succeeded)
            {
                var confirmationCode = _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callBackUrl = Url.Action("ConfirmEmail", "Security", new { userId = user.Id, code = confirmationCode.Result });

                //Send Email
                

                return RedirectToAction("Index", "Home"); 
            }

            return View(model);

          
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId==null || code==null)
            {
                return RedirectToAction("Index","Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                throw new ApplicationException("Unable to find to user");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }

            return RedirectToAction("Index", "Home");


        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email) 
        {
            // email boş mu
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user==null)
            {
                return View();
            }

            var confirmationCode = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Security", new { userId = user.Id, code = confirmationCode });

            //send callbackurlwithemail

            return RedirectToAction("ForgotPasswordEmailSent");
        }

        public IActionResult ForgotPasswordEmailSent()
        {
            return View();
        }

        public IActionResult ResetPassword(string userId,string code)
        {
            if (User==null || code==null)
            {
                throw new ApplicationException("şifre sıfırlamak için kodu sağlamalısınız");
            }

            var model = new ResetPasswordModel { Code = code };

            return View(model);
        }
        [HttpPost]
        public  async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                throw new ApplicationException("kullanıcı bulunamadı");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirm");
            }
            return View();
        }

        public IActionResult ResetPasswordConfirm()
        {
            return View();
        }

    }

}
