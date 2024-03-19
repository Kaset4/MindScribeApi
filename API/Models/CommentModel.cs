using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CommentModel
    {
        public int ArticleId { get; set; }

        [Required]
        [Display(Name = "Комментарий", Prompt = "Введите текст")]
        public string Content_comment { get; set; } = "";
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public string User_id { get; set; } = "";
    }
}
