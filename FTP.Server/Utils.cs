using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using FTP.Server.Model;

namespace FTP.Server
{
    public class Utils
    {
        private static readonly List<User> UsersCredential = GetUsersCredential();
        
        public static List<User> GetUsersCredential()
        {
            using var r = new StreamReader("Users.json");
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        public static string GetClientDefaultDirectory(string username, string password)
        {
            foreach (var user in UsersCredential)
            {
                if (username.Equals(user.Username) && password.Equals(user.Password))
                {
                    return user.DefaultRepository;
                }
            }

            return string.Empty;
        }

        public static bool ValidateUser(string username, string password)
        {
            foreach (var user in UsersCredential)
            {
                if (username.Equals(user.Username) && password.Equals(user.Password))
                {
                    return true;
                }
            }

            return false;
        }
    }
}