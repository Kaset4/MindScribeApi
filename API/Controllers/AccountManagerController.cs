using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using API.Data.UoW;
using API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Models;

namespace API.Controllers
{
    /// <summary>
    /// Контроллер для управления учетными записями пользователей
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AccountManagerController: ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [Route("Register")]
        [SwaggerOperation(Summary = "Регистрация пользователя", Description = "Регистрирует нового пользователя.")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByNameAsync(model.Login);
                if (userExists != null)
                {
                    ModelState.AddModelError("", "Логин уже занят");
                    return BadRequest(ModelState);
                }

                var emailExists = await _userManager.FindByEmailAsync(model.EmailReg);
                if (emailExists != null)
                {
                    ModelState.AddModelError("", "Пользователь с такой почтой уже зарегистрирован");
                    return BadRequest(ModelState);
                }

                if (!IsValidEmail(model.EmailReg))
                {
                    ModelState.AddModelError("", "Неправильный формат электронной почты");
                    return BadRequest(ModelState);
                }


                var user = _mapper.Map<User>(model);

                var result = await _userManager.CreateAsync(user, model.PasswordReg);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, false);
                    return Ok(new { message = "Регистрация прошла успешно" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var loggedInUser = await _userManager.FindByNameAsync(model.UserName);

                    // Получаем роли пользователя
                    if (loggedInUser != null)
                    {
                        return Ok(new { message = "Авторизация прошла успешно" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    user.Convert(model);

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok("Успешное обновление данных пользователя.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Данные не были обновлены. Ошибка введенных данных.");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    // Обработка случая, когда пользователь не найден
                    ModelState.AddModelError("", "Пользователь не найден.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return BadRequest(ModelState);
            }
        }


        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var repository = _unitOfWork.GetRepository<User>() as UserRepository;

                if (repository != null)
                {
                    repository.DeleteUser(user);
                    return Ok("Пользователь успешно удалён из базы.");
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    ModelState.AddModelError("", "Ошибка. Репозиторий пользователей не найден.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                ModelState.AddModelError("", "Пользователь не найден.");
                return BadRequest("Пользователь не найден.");
            }

        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Выход из аккаунта.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
