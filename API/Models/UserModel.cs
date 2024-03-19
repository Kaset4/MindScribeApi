namespace API.Models
{
    public class UserModel
    {
        public User User { get; set; }

        public UserModel(User user)
        {
            User = user;
        }

        public List<Article>? Articles { get; set; }
    }
}
