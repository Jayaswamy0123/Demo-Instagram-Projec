namespace Instagram.Repository.Iservices
{
    public interface IEmailServices
    {
        Task ForgotPasswordEmail(string email, string url);

        Task ConfirmationMail(string email, string url);

        Task<string> ReadTemplate(string templateName);

        Task SendMultipleEmail(string subject, List<string> email, string content, List<string> attachments = null);
    }
}
