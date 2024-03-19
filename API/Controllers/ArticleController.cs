using API.Data.UoW;
using API.Models;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers
{
    public class ArticleController: ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;

        public ArticleController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("Article/{id}")]
        [HttpGet]
        public IActionResult Index(int id)
        {
            ArticleRepository? repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (repository != null)
            {
                var article = repository.GetArticleById(id);
                if (article != null)
                {
                    var model = _mapper.Map<ArticleModel>(article);
                    return Ok(model);
                }
                else
                {
                    // Обработка случая, когда статья не найдена
                    ModelState.AddModelError("", "Статья не найдена.");
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
        [Route("CreateArticle")]
        [HttpPost]
        public async Task<IActionResult> CreateArticle(ArticleModel model)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                var userModel = new UserModel(result);

                if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content_article))
                {
                    return BadRequest(model);
                }

                model.User_id = userModel.User.Id;
                model.Created_at = DateTime.Now;
                model.Updated_at = model.Created_at;
                model.User_id = userModel.User.Id;

                var article = _mapper.Map<Article>(model);

                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    if (model != null)
                    {
                        repository.CreateArticle(article);
                        return Ok("Статья успешно создана.");
                    }
                    else
                    {
                        // Обработка случая, когда модель не была передана
                        ModelState.AddModelError("", "Модель статьи не была передана.");
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
            else
            {
                // Обработка случая, когда пользователь не найден
                ModelState.AddModelError("", "Пользователь не найден.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [Route("DeleteArticle")]
        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                var userModel = new UserModel(result);

                ArticleRepository? repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    var model = repository.GetArticleById(id);
                    if (model != null)
                    {
                        var article = _mapper.Map<Article>(model);
                        if (userModel.User.Id == article.User_id || User.IsInRole("Admin"))
                        {
                            repository.Delete(article);
                            return Ok("Удачное удаление статьи.");
                        }
                        else
                        {
                            // Обработка случая, когда пользователь не имеет прав на удаление статьи
                            ModelState.AddModelError("", "Ошибка. Пользователь не имеет прав на удаление статьи. Статья не была удалена.");
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        // Обработка случая, когда статья не найдена
                        ModelState.AddModelError("", "Статья не найдена.");
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
            else
            {
                // Обработка случая, когда пользователь не найден
                ModelState.AddModelError("", "Пользователь не найден.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [Route("ArticleUpdate")]
        [HttpPost]
        public async Task<IActionResult> ArticleUpdate(ArticleEditModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByIdAsync(model.id);

                var user = User;

                var tempUser = await _userManager.GetUserAsync(user);

                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    var article = repository.GetArticleById(model.Article_Id);

                    if (article != null && tempUser != null && article.User_id == tempUser.Id)
                    {
                        article.Updated_at = DateTime.Now;
                        article.Title = model.Title;
                        article.Content_article = model.Content_article;

                        repository.UpdateArticle(article);

                        return Ok("Статья была обновлена");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Статья не найдена или вы не являетесь её владельцем.");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Репозиторий статей не найден.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return BadRequest(ModelState);
            }
        }
    }
}
