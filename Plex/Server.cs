using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Plex {
    public class Server {
        private string _name;

        public string Name {
            get { return _name; }
        }

        public Server(string name) {
            _name = name;
        }

        public Player GetPlayer() {
            Dictionary<string, string> players = GetPlayers();
            return new Player(_name, players.First().Value);
        }

        public Player GetPlayer(string player) {
            string host;
            if ((host = CheckPlayer(player)) != null)
                return new Player(_name, player);
            else
                return null;
        }

        public string CheckPlayer(string playerName) {
            Dictionary<string, string> players = GetPlayers();
            if (players.ContainsKey(playerName))
                return players[playerName];
            else
                return null;
        }

        public Dictionary<string, string> GetPlayers() {
            Uri uri = new Uri(string.Format("http://{0}:32400/clients", _name));
            WebClient client = new WebClient();

            string sResponse = client.DownloadString(uri);
            XmlDocument response = new XmlDocument();

            Dictionary<string, string> players = new Dictionary<string, string>();
            foreach (XmlElement player in response.DocumentElement.SelectNodes("//Server")) {
                players.Add(player.Attributes["name"].Value, player.Attributes["host"].Value);
            }

            return players;
        }
    }
}
