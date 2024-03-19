namespace API.Models
{
    public static class UserFromModel
    {
        public static User Convert(this User user, UserEditModel usereditvm)
        {
            user.Image = usereditvm.Image;
            user.LastName = usereditvm.LastName;
            user.FirstName = usereditvm.FirstName;
            user.Email = usereditvm.Email;
            user.About = usereditvm.About;

            return user;
        }
    }
}
