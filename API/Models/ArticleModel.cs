using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ArticleModel
    {
        [Required(ErrorMessage = "Поле Заголовок обязательно к заполнению")]
        [Display(Name = "Название статьи", Prompt = "Введите название статьи")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Поле Контент обязательно к заполнению")]
        [Display(Name = "Контент", Prompt = "Введите текст")]
        public string Content_article { get; set; } = "";

        public string User_id { get; set; } = "";
        public List<ArticleTag>? Tags { get; set; }
        public List<Comment>? Comments { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }


        public async Task<string> GetFullNameAuthor(UserManager<User> userManager)
        {
            var user = await userManager.FindByIdAsync(User_id);
            return user != null ? $"{user.LastName} {user.FirstName}" : "Unknown Author";
        }
    }
}
