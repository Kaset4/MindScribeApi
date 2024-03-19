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
    public class ArticleTagController: ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;

        public ArticleTagController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [Route("ArticleEdit/{id}/CreateTag")]
        [HttpPost]
        public IActionResult CreateTag(int id, string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                // Возвращаем ошибку
                ModelState.AddModelError("", "tagname имеет пустое значение.");
                return BadRequest(ModelState);
            }

            var temp = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (temp.GetArticleById(id) == null)
            {
                ModelState.AddModelError("", "Статья не найдена.");
                return BadRequest(ModelState);
            }

            // Создаем новый объект ArticleTag
            var newTag = new ArticleTagModel
            {
                NameTag = tagName,
                ArticleId = id
            };

            // Сохраняем новый тег в базе данных

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = _mapper.Map<ArticleTag>(newTag);
                repository.Create(model);
                return Ok("Успешное создание нового тега.");
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                ModelState.AddModelError("", "Репозиторий не найден.");
                return BadRequest("Репозиторий не найден.");
            }
        }

        [Authorize]
        [Route("ArticleEdit/{id}/EditTag")]
        [HttpPost]
        public IActionResult EditTag(int tagId, int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                ModelState.AddModelError("", "newName имеет пустое значение");
                return BadRequest(ModelState);
            }
            var temp = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (temp.GetArticleById(id) == null)
            {
                ModelState.AddModelError("", "Статья не найдена.");
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = repository.GetArticleTagById(tagId);
                if (model != null && model.ArticleId == id)
                {
                    var articleTag = _mapper.Map<ArticleTag>(model);
                    articleTag.NameTag = newName;

                    repository.UpdateArticleTag(articleTag);
                    return Ok("Успешное изменение тега."); // Перенаправление на страницу статьи
                }
                else
                {
                    // Обработка случая, когда тег не найден
                    ModelState.AddModelError("", "Тег не найден.");
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
        [Route("ArticleEdit/{id}/DeleteTag")]
        [HttpPost]
        public IActionResult DeleteTag(int tagId, int id)
        {
            var temp = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (temp.GetArticleById(id) == null)
            {
                ModelState.AddModelError("", "Статья не найдена.");
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = repository.GetArticleTagById(tagId);
                if (model != null && model.ArticleId == id)
                {
                    repository.Delete(model);
                    return Ok("Успешное удаление тега."); // Перенаправление на страницу статьи
                }
                else
                {
                    // Обработка случая, когда тег не найден
                    ModelState.AddModelError("", "Тег не найден.");
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
    }
}
