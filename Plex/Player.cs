using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Plex {
    public class Player {
        private const string COMMAND_FORMAT = "http://{0}:32400/system/players/{1}/{2}/{3}";
        private string _server, _name;

        public string Server {
            get { return _server; }
        }

        public string Name {
            get { return _name; }
        }

        public Player(string server, string name) {
            _server = server;
            _name = name;
        }

        public void Play() {
            SendCommand("playback", "play");
        }

        public void Pause() {
            SendCommand("playback", "pause");
        }

        public void Stop() {
            SendCommand("playback", "stop");
        }

        public void FastForward() {
            SendCommand("playback", "fastForward");
        }

        public void Rewind() {
            SendCommand("playback", "rewind");
        }

        public void Back() {
            SendCommand("navigation", "back");
        }

        public void Select() {
            SendCommand("navigation", "select");
        }

        public void Up() {
            SendCommand("navigation", "moveUp");
        }

        public void Down() {
            SendCommand("navigation", "moveDown");
        }

        public void Left() {
            SendCommand("navigation", "moveLeft");
        }

        public void Right() {
            SendCommand("navigation", "moveRight");
        }

        public void PageUp() {
            SendCommand("navigation", "pageUp");
        }

        public void PageDown() {
            SendCommand("navigation", "pageDown");
        }

        public void Menu() {
            SendCommand("navigation", "contextMenu");
        }

        private void SendCommand(string controller, string command) {
            Uri uri = new Uri(string.Format(COMMAND_FORMAT, _server, _name, controller, command));

            WebClient client = new WebClient();
            string sResponse = client.DownloadString(uri);

            if (sResponse.Length > 0)
                throw new Exception(sResponse);
        }
    }
}
