using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using DDDProject.Data;

namespace DDDProject.Utils
{
    public static class ClientLoginService
    {
        public static bool IsLoggedIn { 
            get {
                return LoginToken != "";
            } 
        }

        public static string LoginToken = "";
        public static string Username = "";
        public static string Fullname = "";
        public static string ProfileBio = "";
        public static string ProfileDepartment = "";

        public static async Task FetchUserInfo()
        {
            LoginService loginService = new();

            UserInfo user = await loginService.RequestUserInfo(LoginToken);
            Username = user.Username;
            Fullname = user.Fullname;
            ProfileBio = user.ProfileBio;
            ProfileDepartment = user.ProfileDepartment;
        }
    }
}