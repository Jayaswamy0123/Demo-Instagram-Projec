namespace Instagram.Repository.Iservices
{
    public interface IUserServices
    {
        public int UserID { get; }

        public string Email { get; }

        public string Name { get; }
    }
}