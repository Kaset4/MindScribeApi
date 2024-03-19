using API.Data.UoW;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RoleController: ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Route("CreateRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        // Роль успешно создана
                        return Ok("Успешное создание роли.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль с таким именем уже существует");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        [Route("DeleteRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        // Роль успешно удалена
                        return Ok("Успешное удаление роли.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль не найдена");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        [Route("EditRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(string roleName, string newRoleName)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    role.Name = newRoleName; // Обновляем имя роли
                    var result = await _roleManager.UpdateAsync(role); // Обновляем роль в базе данных
                    if (result.Succeeded)
                    {
                        // Роль успешно отредактирована
                        return Ok("Успешное изменение роли.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль не найдена");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
