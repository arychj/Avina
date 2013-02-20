using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Plex {
    public class Manager {
        private const string DEFAULT_SERVER = "quora";
        private const string DEFAULT_PLAYER = "192.168.1.200";

        public static Player GetPlayer() {
            return new Player(DEFAULT_SERVER, DEFAULT_PLAYER);
        }

        public static Player GetPlayer(string server, string player) {
            string host;
            if ((host = CheckPlayer(server, player)) != null)
                return new Player(server, player);
            else
                return null;
        }

        public static string CheckPlayer(string serverName, string playerName) {
            Uri uri = new Uri(string.Format("http://{0}:32400/clients", serverName));
            WebClient client = new WebClient();

            string sResponse = client.DownloadString(uri);
            XmlDocument response = new XmlDocument();
            response.LoadXml(sResponse);

            XmlElement player = (XmlElement)response.DocumentElement.SelectSingleNode(string.Format("//Server[name='{0}']", playerName));
            if (player != null)
                return player.Attributes["host"].Value;
            else
                return null;
        }
    }
}
