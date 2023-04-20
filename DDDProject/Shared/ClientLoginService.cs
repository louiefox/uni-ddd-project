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

        public static async void FetchUserInfo()
        {
            LoginService loginService = new();
            Fullname = await loginService.RequestUserInfo(LoginToken);
        }
    }
}