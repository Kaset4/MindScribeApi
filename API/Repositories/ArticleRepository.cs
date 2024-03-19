using API.Data;
using API.Models;

namespace API.Repositories
{
    public class ArticleRepository : Repository<Article>
    {
        private readonly ApplicationDbContext _context;
        public ArticleRepository(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public Article GetArticleById(int articleId)
        {
            var article = Get(articleId);

            // Явная загрузка связанных комментариев
            try
            {
                _context.Entry(article)
                    .Collection(a => a.Comments)
                    .Load();
            }catch (Exception ex) { return article; }

            try
            {
                _context.Entry(article)
                   .Collection(a => a.Tags)
                   .Load();
            }
            catch (Exception ex) { return article; }
           

            return article;
        }

        public List<Article> GetAllArticles()
        {
            return GetAll().ToList();
        }

        public void CreateArticle(Article article)
        {
            Create(article);
        }

        public void UpdateArticle(Article updatedArticle)
        {
            Update(updatedArticle);
        }

        public void DeleteArticle(int articleId)
        {
            Delete(articleId);
        }

        public List<Article> GetArticlesByAuthorId(User user)
        {
            return Set.Where(article => article.User_id == user.Id).ToList();
        }
    }
}
