using System.Drawing;
using System.Net;
using System.Numerics;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SteamAPI
{

    public partial class Form1 : Form
    {
        private Button[,] buttons;
        private int[,] mines;
        int difficultyMultiplier;
        private Random random = new Random();

        private string userInputUserName;
        private string userInputID;
        public string Id;
        FlowLayoutPanel flowLayoutPanel;
        FlowLayoutPanel flowLayoutPanel2;
        private Middle middle = new Middle();
        public Form1()
        {
            InitializeComponent();

            TabPage tabPage1 = tabControl1.TabPages["tabPage1"];
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.AutoScroll = true;//jos tulee liikaa pelejä sallii srollaamisen
            tabPage1.Controls.Add(flowLayoutPanel);
            flowLayoutPanel.Size = new Size(490, 400);
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;//tuo pelit päällekkäin ja saane nätisti esille
            flowLayoutPanel.WrapContents = false;
            TabPage tabPage2 = tabControl1.TabPages["tabPage2"];
            flowLayoutPanel2 = new FlowLayoutPanel();
            flowLayoutPanel2.AutoScroll = true;
            tabPage2.Controls.Add(flowLayoutPanel2);
            flowLayoutPanel2.Size = new Size(490, 400);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.WrapContents = false;

            comboBox1.Items.Add("1");
            comboBox1.Items.Add("2");
        }
        public void Form1_Load()
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            userInputUserName = textBox1.Text;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void SetUser(JObject player)
        {

            pictureBox1.Load(player["avatarfull"].ToString());//asettaa haetun pelaajan tiedot päähaun näyksi
            textBox1.Text = player["personaname"].ToString();
            textBox2.Text = player["steamid"].ToString();
            label5.Text = player["personastate"].ToString() == "0" ? "Poissa" : "Paikalla";//katsotaan onko pelaaja paikalla
        }

        private void SetGames(JArray gamesArray)
        {


            if (gamesArray == null)
            {
                MessageBox.Show("Tilillä ei ole pelejä tai ne on asetettu yksityiseksi");
            }
            else
            {
                flowLayoutPanel.SuspendLayout();

                foreach (JObject game in gamesArray)
                {
                    string icon = game["img_icon_url"].ToString();
                    if (icon != "")
                    {
                        string name = game["name"].ToString();
                        string appID = game["appid"].ToString();

                        // Luo uusi PictureBox ja lataa siihen pelin ikoni
                        PictureBox gameIcon = middle.GetGameIcon(appID, icon);
                        gameIcon.Size = new Size(50, 50); // Aseta kuvan koko

                        // Luo uusi Label ja aseta siihen pelin nimi
                        Label gameName = new Label();
                        gameName.Text = name;
                        gameName.AutoSize = true;

                        FlowLayoutPanel gamePanel = new FlowLayoutPanel();
                        gamePanel.AutoSize = true; // Aseta Panelin koko mukautumaan sisällön mukaan
                        gamePanel.FlowDirection = FlowDirection.LeftToRight; // Aseta ohjausten järjestys vasemmalta oikealle
                        gamePanel.Controls.Add(gameIcon); // Lisää PictureBox Paneliin
                        gamePanel.Controls.Add(gameName); // Lisää Label Paneliin

                        flowLayoutPanel.Controls.Add(gamePanel);
                    }

                }
                flowLayoutPanel.ResumeLayout();
            }
        }
        private string SetAmountOfGames(string steamID)
        {
            return middle.GetAmountOfGames(steamID);
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(textBox1.Text))   // tarkistetaan onko tekstikenttä tyhjä jos ei suoritetaan haku
            {
                UsernameSearch();
            }
            else if (!string.IsNullOrEmpty(textBox2.Text)) //jos käyttäjänimikenttä on tyhjä koitetaan hakea idllä
            {
                IdSearch();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void SetFriends(JArray friendProfileArray)
        {
            if (friendProfileArray == null)
            {
                MessageBox.Show("Kaverit on asetettu yksityiseksi. Ei lupaa näytää tätä tietoa"); //tämä tulee jos api avain on väärä tai ei ole lupaa nähdä tietoja.
            }
            else
            {
                flowLayoutPanel2.SuspendLayout();
                foreach (JObject friendProfile in friendProfileArray)
                {
                    PictureBox profilePicture = new PictureBox();
                    profilePicture.Load(friendProfile["avatar"].ToString());
                    profilePicture.Size = new Size(50, 50);

                    Label profileName = new Label();
                    profileName.Text = friendProfile["personaname"].ToString();
                    profileName.AutoSize = true;
                    profileName.Click += (sender, e) =>
                    {
                        // Aseta klikatun Label-ohjauksen teksti textBox1:een
                        Label clickedLabel = sender as Label;
                        textBox1.Text = "";
                        textBox2.Text = friendProfile["steamid"].ToString();
                        IdSearch();
                    };

                    FlowLayoutPanel friendPanel = new FlowLayoutPanel();
                    friendPanel.AutoSize = true;
                    friendPanel.FlowDirection = FlowDirection.LeftToRight;
                    friendPanel.Controls.Add(profilePicture);
                    friendPanel.Controls.Add(profileName);

                    flowLayoutPanel2.Controls.Add(friendPanel);
                }
                flowLayoutPanel2.ResumeLayout();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!BigInteger.TryParse(textBox2.Text, out var a)) // tarkistetaan onko syötetty id pelkästään numeroita
            {
                MessageBox.Show("Syötä vain numeroita");
            }
            else
            {
                userInputID = textBox2.Text;
                Id = userInputID;
            }
        }
        private void UsernameSearch()
        {
            ClearFields();
            string steamID = middle.GetID(userInputUserName);
            if (steamID != null) //jos steam id ei ole tyhjä
            {
                SetUser(middle.GetUser(steamID));
                SetGames(middle.GetGames(steamID));
                SetFriends(middle.GetFriends(steamID));
                label7.Text = SetAmountOfGames(steamID);

            }
            else { MessageBox.Show("Käyttäjänimeä ei löytynyt"); }

        }
        private void IdSearch()
        {
            ClearFields();
            string steamID = userInputID.ToString();
            if (steamID != null) //jos steam id ei ole tyhjä
            {
                SetUser(middle.GetUser(steamID));
                SetGames(middle.GetGames(steamID));
                SetFriends(middle.GetFriends(steamID));
                label7.Text = SetAmountOfGames(steamID);
            }
            else { MessageBox.Show("Idtä ei löytynyt"); }
        }
        private void ClearFields()
        {
            flowLayoutPanel.Controls.Clear();// tyhjentää kentät kun haetaan uutta tietpa
            flowLayoutPanel2.Controls.Clear();
            label7.Controls.Clear();

        }
        private void SetGame(int i)
        {

            difficultyMultiplier = 5 * i; // Vaikeuskerroin muuttuu i:n mukaan
            mines = new int[difficultyMultiplier, difficultyMultiplier];
            buttons = new Button[difficultyMultiplier, difficultyMultiplier];

            for (int x = 0; x < difficultyMultiplier; x++)
            {
                for (int y = 0; y < difficultyMultiplier; y++)
                {
                    buttons[x, y] = new Button();
                    buttons[x, y].SetBounds(x * 40, y * 40, 40, 40);
                    buttons[x, y].MouseUp += ButtonMouseUp;
                    tabPage3.Controls.Add(buttons[x, y]);

                    // Satunnaisesti sijoitetut miinat
                    mines[x, y] = random.Next(difficultyMultiplier) == 0 ? 1 : 0;
                }
            }
        }
        private void ButtonMouseUp(object sender, MouseEventArgs e)
        {
            //mitä näappia painettiin
            Button button = (Button)sender;

            //napin sijainti
            int x = button.Location.X / 40;
            int y = button.Location.Y / 40;

            //osuuko miinaan
            if (e.Button == MouseButtons.Left)
            {
                if (mines[x, y] == 1)
                {
                    button.Text = "M";
                    MessageBox.Show("Osuit miinaan");
                    tabPage3.Controls.Clear();
                    SetGame(Int32.Parse(comboBox1.Text));
                }
                else
                {
                    int nearMines = CountMines(x, y);
                    RevealCells(x, y);
                    button.Text = nearMines.ToString();
                   
                }
            }
            //merkitse miina
            else if (e.Button == MouseButtons.Right)
            {
                button.Text = "f";
                CheckWin();
            }
        }

        private void CheckWin()
        {
            for (int x = 0; x < difficultyMultiplier; x++)
            {
                for (int y = 0; y < difficultyMultiplier; y++)
                {
                    if (mines[x, y] == 1 && buttons[x, y].Text != "f")
                    {
                        return; // Peli ei ole vielä ohi, koska kaikkia miinoja ei ole merkitty
                    }
                }
            }
            // Kaikki miinat on merkitty, joten pelaaja voittaa
            MessageBox.Show("Voitit pelin!");
        }

        private void RevealCells(int x, int y)
        {
            // Tarkista, onko solu jo paljastettu
            if (buttons[x, y].Text != "")
                return;

            int nearMines = CountMines(x, y);
            buttons[x, y].Text = nearMines.ToString();

            // Jos solussa ei ole miinoja, paljasta kaikki viereiset solut
            if (nearMines == 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int newI = x + i;
                        int newJ = y + j;
                        if (newI >= 0 && newJ >= 0 && newI < difficultyMultiplier && newJ < difficultyMultiplier)
                        {
                            // Tarkista, onko solu jo paljastettu ennen kuin kutsutaan rekursiivisesti
                            if (buttons[newI, newJ].Text == "")
                            {
                                RevealCells(newI, newJ);
                            }
                        }
                    }
                }
            }
        }
        private int CountMines(int x, int y)
        {
            int count = 0;
            // Käy läpi kaikki solut, jotka ovat yhden solun päässä annetusta solusta `(x, y)`
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Laske uudet indeksit `newI` ja `newJ` lisäämällä `i` ja `j` alkuperäisiin `x` ja `y` indekseihin
                    int newI = x + i;
                    int newJ = y + j;
                    // Tarkista, onko uusi solu `(newI, newJ)` ruudukon sisällä
                    if (newI >= 0 && newJ >= 0 && newI < difficultyMultiplier && newJ < difficultyMultiplier && mines[newI, newJ] == 1)
                    {
                        // Jos solu on ruudukon sisällä ja siinä on miina, kasvata miinojen määrää
                        count++;
                    }
                }
            }
            // Palauta laskettu miinojen määrä
            return count;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabPage3.Controls.Clear();
            SetGame(Int32.Parse(comboBox1.Text));
        }
    }
}
        