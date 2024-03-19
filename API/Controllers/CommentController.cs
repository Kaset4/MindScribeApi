using API.Data.UoW;
using API.Models;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class CommentController: ControllerBase
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        [Authorize]
        [Route("Article/{Id}/SendComment")]
        [HttpPost]
        public async Task<IActionResult> SendComment(int Id, CommentModel model)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result == null)
            {
                // Обработка случая, когда пользователь не найден
                ModelState.AddModelError("", "Ошибка. Пользователь не найден.");
                return BadRequest(ModelState);
            }

            var userModel = new UserModel(result);

            model.User_id = userModel.User.Id;
            model.Created_at = DateTime.Now;
            model.Updated_at = DateTime.Now;
            model.ArticleId = Id;
            model.User_id = result.Id;



            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            if (repository != null)
            {
                var commentModel = _mapper.Map<Comment>(model);

                repository.CreateComment(commentModel);
                return Ok("Успешное cоздание комментария.");
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                ModelState.AddModelError("", "Ошибка. Репозиторий комментариев не найден.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [Route("Article/{Id}/DeleteComment")]
        [HttpPost]
        public async Task<IActionResult>  DeleteComment(int Id, int commentId)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);


            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            if (repository != null)
            {
                var comment = repository.GetCommentById(commentId);


                if ((comment != null && result.Id == comment.User_id) ||
                    (comment != null && user.IsInRole("Admin")) ||
                    (comment != null && user.IsInRole("Moderator")))
                {
                    repository.Delete(comment);
                    return Ok("Успешное удаление комментария.");
                }
                else
                {
                    // Обработка случая, когда комментарий не найден
                    ModelState.AddModelError("", "Комментарий не найден.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                ModelState.AddModelError("", "Репозиторий не найден.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [Route("Article/{Id}/EditComment")]
        [HttpPost]
        public async Task<IActionResult> EditComment(int Id, int commentId, string content)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("", "content имеет пустое значение.");
                return BadRequest(ModelState);
            }

            CommentRepository? repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            if (repository != null)
            {
                var model = repository.GetCommentById(commentId);

                if (model.ArticleId != Id)
                {
                    ModelState.AddModelError("", "Комментарий не найден.");
                    return BadRequest(ModelState);
                }

                if ((model != null && result.Id == model.User_id) ||
                    (model != null && user.IsInRole("Admin")) ||
                    (model != null && user.IsInRole("Moderator")))
                {
                    var comment = _mapper.Map<Comment>(model);
                    comment.Content_comment = content;
                    comment.Updated_at = DateTime.Now;
                    repository.UpdateComment(comment);
                    return Ok("Успешное изменение комментария.");
                }
                else
                {
                    // Обработка случая, когда комментарий не найден
                    ModelState.AddModelError("", "Комментарий не найден.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                ModelState.AddModelError("", "Репозиторий комментариев не найден.");
                return BadRequest(ModelState);
            }
        }
    }
}
