using API.Models;
using AutoMapper;

namespace API.Extentions
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModel, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login))
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg));

            CreateMap<ArticleModel, Article>();
            CreateMap<Article, ArticleModel>();

            CreateMap<Article, ArticleEditModel>();
            CreateMap<ArticleEditModel, Article>();

            CreateMap<CommentModel, Comment>();

            CreateMap<ArticleTagModel, ArticleTag>();
        }
    }
}
