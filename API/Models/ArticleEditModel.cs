using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ArticleEditModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Название статьи", Prompt = "Введите название")]
        public string Title { get; set; } = "";

        [DataType(DataType.Text)]
        [Display(Name = "Текст", Prompt = "Введите текст")]
        public string Content_article { get; set; } = "";

        public List<ArticleTag>? Tags { get; set; }
        public List<Comment>? Comments { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public int Article_Id { get; set; }
    }
}
