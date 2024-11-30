using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SteamAPI
{
    public class Backend
    {
        private string apiKey = "APIKEYHERE"; //Rajapinnan avain
        public string GetID(string username)//haetaan steam käyttäjän id jolla saadaan halutut tiedot
        {
            string url = $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={apiKey}&vanityurl={username}";//käyttäen minun rajapintaavainta katsotaan käyttäjänimi ja asetetaan se url arvoksi
            using (WebClient client = new WebClient())
            {
                string Json = client.DownloadString(url);//ladataan tiedot käyttäjästä ja samalla nähdään onko profiili julkinen että sieltä saa tietoa
                JObject data = JObject.Parse(Json);//muuttaa json datan JObject olioksi että sen dataa voidaan käsitellä
                if (data["response"]["success"].ToString() == "1")//katsoo onko ResolveVanityURL onnistunut eli 1
                {

                    return data["response"]["steamid"].ToString();//jos onnistui niin palauttaa käyttäjän steamID:n

                }
                else
                {
                    return null;

                }
            }
        }
        public JObject GetUser(string steamID)
        {
            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamID}";//url joka hakee pelaajan tiedot
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                JObject data = JObject.Parse(json);
                JObject player = data["response"]["players"][0] as JObject;
                return player;
            }
        }

        public JArray GetGames(string steamID)
        {
            //pyytää rajapintaavaimen käyttäjän idn bool vastauksen tuleeko pelien infoa kuten nimi ja ikoni mistäkin pelistä ja tuleeko ilmaiset pelatut pelit mukaan
            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={apiKey}&steamid={steamID}&include_appinfo=true&include_played_free_games=true&format=json";

            using (WebClient client = new WebClient())
            {
                try //yritetään saada käyttäjän omistamat pelit
                {
                    string json = client.DownloadString(url);
                    JObject games = JObject.Parse(json);
                    JArray gamesArray = (JArray)games["response"]["games"];//cast´ätään jobject JArrayksi käsittelyä varten
                    return gamesArray;


                }
                catch (WebException ex) // Jos tapahtuu WebException-virhe, käsitellään se tässä
                {
                    ErrorFinder(ex);
                    return null;
                }

            }
        }
        public PictureBox GetGameIcon(string appID, string iconUrl)//haetaan pelin ikoni kuva
        {
            string url = $"http://media.steampowered.com/steamcommunity/public/images/apps/{appID}/{iconUrl}.jpg";//url osoite pelin ikonin hakuun
            PictureBox gameIcon = new PictureBox();
            gameIcon.Load(url);
            gameIcon.Size = new Size(50, 50);//asetetaan kooksi 50x50 pikseliä koska se on hyvä koko
            return gameIcon;
        }
        public JArray GetFriends(string steamID)
        {

            string url = $"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={apiKey}&steamid={steamID}&relationship=friend";

            using (WebClient client = new WebClient())
            {
                try //yritetään saada käyttäjän omistamat pelit
                {

                    string json = client.DownloadString(url);
                    JObject steamFriend = JObject.Parse(json);
                    JArray friendArray = (JArray)steamFriend["friendslist"]["friends"];
                    if (friendArray == null)
                    {
                        MessageBox.Show("Tilillä ei ole kavereita");// jos palauttaa tyhjänä kavereita ei ole
                        return null;
                    }
                    else
                    {
                        JArray friendProfileArray = new JArray();
                        foreach (JObject friend in friendArray)
                        {
                            string friendID = friend["steamid"].ToString();
                            friendProfileArray.Add(GetUser(friendID));
                        }
                        return friendProfileArray;
                    }
                }
                catch (WebException ex) //jos kavereita ei saada katsotaan miksi
                {
                    ErrorFinder(ex);
                    return null;
                }
            }

        }
        public string GetAmontOfGames(string steamID)
        {
            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={apiKey}&steamid={steamID}&include_appinfo=true&include_played_free_games=true&format=json";

            using (WebClient client = new WebClient())
            {
                try //yritetään saada käyttäjän omistamat pelit
                {
                    string json = client.DownloadString(url);
                    JObject games = JObject.Parse(json);
                    return games["response"]["game_count"].ToString();
                }
                catch (WebException ex) //jos kavereita ei saada katsotaan miksi
                {
                    ErrorFinder(ex);
                    return null;
                }

            }
        }
        private WebException ErrorFinder(WebException ex)
        {


            // Tarkistetaan, onko virheen tila ProtocolError ja onko vastaus HttpWebResponse-tyyppiä
            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse response)
            {
                // Jos HTTP-vastauksen tilakoodi on Unauthorized (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Näytetään virheilmoitus käyttäjälle ja palautetaan null
                    MessageBox.Show("Ei lupaa näyttää pelejä."); // Tämä tulee jos API-avain on väärä tai ei ole lupaa nähdä tietoja.
                    return null;
                }
                else
                {
                    // Jos HTTP-vastauksen tilakoodi on jokin muu, näytetään virheilmoitus käyttäjälle ja palautetaan null
                    MessageBox.Show($"HTTP-virhe: {response.StatusCode}");
                    return null;
                }
            }
            else
            {
                // Jos virheen tila ei ole ProtocolError, näytetään virheilmoitus käyttäjälle ja palautetaan null
                MessageBox.Show($"Verkkovirhe: {ex.Status}");
                return null;
            }
        }
    }
 }

