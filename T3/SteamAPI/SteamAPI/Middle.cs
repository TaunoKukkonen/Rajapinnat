using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{
    public class Middle
    {
        Backend backend=new Backend();
       
        public JObject GetUser(string steamID)
        {
            return backend.GetUser(steamID);
        }
        public JArray GetGames(string steamID)
        {
            return backend.GetGames(steamID);
        }
        public JArray GetFriends(string steamID)
        {
           return backend.GetFriends(steamID);
        }
        public PictureBox GetGameIcon(string appID, string iconUrl)
        {
            return backend.GetGameIcon(appID, iconUrl);
        }
        public string GetID(string userInputUsername)
        {
             return backend.GetID(userInputUsername);
        }
        public string GetAmountOfGames(string steamID)
        {
            return backend.GetAmontOfGames(steamID);
        }
       
    }
}
