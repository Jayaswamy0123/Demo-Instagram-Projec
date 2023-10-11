namespace Instagram.GlobalVariables
{
    public class ResponseConstents
    {
        public static class TemplateName
        {
            public const string ForgotPassword = "ForgotPassword.html";
            public const string AccoutConfirmationMail = "ConfirmationMail.html";
            public const string Bosi = "Instagram.html";
        }


        public const string Mediapath = "Instagram/media";
        public static string GetMediapath(long userid)
        {
            return Mediapath;
        }


        public static class EmailSubject
        {
          
            public const string ForgotPassword = "Instagram - Reset Password";
            public const string AccountCreation = "Instagram - AccountCreation";
            public const string TeamAdminCreation = "Instagram admin has created your company login";
        }





        public static class GetMediaTypes
        {
            public static string GetTypeName(int typeId)
            {
                return typeId switch
                {
                    (int)MediaTypes.ProfilePicture => MediaTypeConsts.ProfilePicture,
                    (int)MediaTypes.UserPosts => MediaTypeConsts.UserPosts,
                    _ => null,
                };
            }
        }

        public static class MediaTypeConsts
        {
            public const string ProfilePicture = "ProfilePicture";
            public const string UserPosts  = "UserPosts";
        }


        public static class Constants
        {
            public static string CapturePayload { get; set; }


            #region Smtp Configuration
            public const string SmtpServer = "EmailConfiguration:SmtpServer";
            public const string SmtpPort = "EmailConfiguration:Port";
            public const string SmtpUserName = "EmailConfiguration:Username";
            public const string SmtpPassword = "EmailConfiguration:Password";
            public const string SmtpFrom = "EmailConfiguration:From";
            #endregion

            public const string MailSentSuccesfully = "MailSentSuccessfully";
            public const string SuccessResponse = "Success";
            public const string CheckEmail = "Invalid Email";
            public const string Invalid = "Invalid";
            public const string Login = "login using a token";
            public const string Registration = "Registration Successful, Please Verify your Email";


            public const string PaymentSuccessful = "Payment Successful";
            public const string PaymentFailed = "Payment Failed";
            public const string Initiated = "Payment Initiated";
            public const string orderefund = "Payment orderefund";




            public const string InvalidInput = "please enter valid input";


            public const string UserExists = "This email or phone number already used please try new email or phone number or login";
            public const string InValidRoleId = "invalid role id";
            public const string RegisterSuccess = "Registration successful, Please Login";
            public const string OTPSent = "Otp has been send to successfully";
            public const string OTPCountExceeded = "Maximum no of OTP requests excceded,please try again later";
            public const string InvalidOtp = "The OTP is expired or wrong";
            public const string LoginSuccess = "SuccessFully Logged in";
            public const string LoginFail = "Invalid Credentials";
            public const string NotAllowed = "You are not allowed to login here";
            public const string ResetLinkSent = "Password reset link sent to your email";
            public const string InvalidEmail = "please enter a valid email or Phone number";
            public const string PasswordResetSuccess = "Password reset successful";
            public const string WrongLink = "Wrong Password Reset Link";
            public const string InvalidRefreshToken = "Invalid rest token";
            public const string RefreshTokenMandatory = "refrestToken parameter is mandatory";



        };
    }
}
