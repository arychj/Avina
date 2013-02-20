using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Listener {
    class Program {
        private static Dictionary<string, Command> _phrases = new Dictionary<string, Command>();
        private static Word _keyword;
        private static string _thisroom;

        public static void Main(string[] args) {
            XmlDocument config = LoadConfig("config.xml");
            List<Room> rooms = LoadRooms(config);
            Grammar grammar = LoadGrammar(rooms);
            
            SpeechRecognitionEngine sr = new SpeechRecognitionEngine();

            sr.LoadGrammar(grammar);

            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);
            sr.SetInputToDefaultAudioDevice();
            sr.RecognizeAsync(RecognizeMode.Multiple);

            Console.WriteLine("listening...");
            Console.Read();
        }

        private static XmlDocument LoadConfig(string filename) {
            XmlDocument config = new XmlDocument();
            config.Load(filename);

            _thisroom = config.DocumentElement.SelectSingleNode("./thisroom").InnerText;
            _keyword = new Word(
                config.DocumentElement.SelectSingleNode("./keyword/name").InnerText,
                config.DocumentElement.SelectSingleNode("./keyword/pheonetic").InnerText
            );

            return config;
        }

        private static Grammar LoadGrammar(List<Room> rooms) {
            foreach (Room room in rooms) {
                foreach (Action action in room.Actions) {
                    if (room.Word.Name == _thisroom) {
                        if (!action.OptionRequired) {
                            AddCommand(_phrases,
                                new Command(
                                    action.DefaultOption.Id,
                                    string.Format("{0}|{1}|{2}|{3}", _keyword.Name, room.Word.Name, action.Word.Name, action.DefaultOption.Word.Name),
                                    string.Format("{0}. {1}", _keyword.Pheonetic, action.Word.Pheonetic)
                                )
                            );

                            AddCommand(_phrases,
                                new Command(
                                    action.DefaultOption.Id,
                                    string.Format("{0}|{1}|{2}|{3}", _keyword.Name, room.Word.Name, action.Word.Name, action.DefaultOption.Word.Name),
                                    string.Format("{0}. {1} {2}", _keyword.Pheonetic, room.Word.Pheonetic, action.Word.Pheonetic)
                                )
                            );
                        }
                        else {
                            foreach (Option option in action.Options) {
                                AddCommand(_phrases,
                                    new Command(
                                        option.Id,
                                        string.Format("{0}|{1}|{2}|{3}", _keyword.Name, room.Word.Name, action.Word.Name, option.Word.Name),
                                        string.Format("{0}. {1} {2}", _keyword.Pheonetic, action.Word.Pheonetic, option.Word.Pheonetic)
                                    )
                                );

                                if (action.SkipKeyword)
                                    AddCommand(_phrases,
                                            new Command(
                                                option.Id,
                                                string.Format("{0}|{1}|{2}|{3}", _keyword.Name, room.Word.Name, action.Word.Name, option.Word.Name),
                                                string.Format("{0}. {1}", action.Word.Pheonetic, option.Word.Pheonetic)
                                            )
                                        );
                            }
                        }
                    }

                    foreach (Option option in action.Options) {
                        AddCommand(_phrases,
                                new Command(
                                    option.Id,
                                    string.Format("{0}|{1}|{2}|{3}", _keyword.Name, room.Word.Name, action.Word.Name, option.Word.Name),
                                    string.Format("{0}. {1} {2} {3}", _keyword.Pheonetic, room.Word.Pheonetic, action.Word.Pheonetic, option.Word.Pheonetic)
                                )
                            );
                    }
                }
            }

            Choices jargon_grammar = new Choices();
            GrammarBuilder gb = new GrammarBuilder();

            jargon_grammar.Add(_phrases.Keys.ToArray());
            gb.Append(jargon_grammar);

            Grammar g = new Grammar(gb);

            return g;
        }

        private static List<Room> LoadRooms(XmlDocument config) {
            List<Room> rooms = new List<Room>();
            List<Action> actions;
            List<Option> options;

            Room room; Action action;
            XmlElement defaultOption;
            foreach (XmlElement xRoom in config.DocumentElement.SelectNodes("./room")) {
                actions = new List<Action>();
                foreach (XmlElement xAction in xRoom.SelectNodes("./actions/action")) {
                    options = new List<Option>();
                    foreach (XmlElement xOption in xAction.SelectNodes("./options/option")) {
                        options.Add(
                            new Option(
                                new Word(
                                    xOption.SelectSingleNode("./name").InnerText,
                                    xOption.SelectSingleNode("./pheonetic").InnerText
                                ),
                                int.Parse(xOption.Attributes["id"].Value)
                            )
                        );
                    }

                    defaultOption = (XmlElement)xAction.SelectSingleNode("./default");
                    if (defaultOption == null) {
                        action = new Action(
                            new Word(
                                xAction.SelectSingleNode("./name").InnerText,
                                xAction.SelectSingleNode("./pheonetic").InnerText
                            ),
                            options
                        );
                    }
                    else {
                        action = new Action(
                            new Word(
                                xAction.SelectSingleNode("./name").InnerText,
                                xAction.SelectSingleNode("./pheonetic").InnerText
                            ),
                            options,
                            defaultOption.InnerText
                        );
                    }

                    if (xAction.Attributes["skipKeyword"] != null && xAction.Attributes["skipKeyword"].Value == "true")
                        action.SkipKeyword = true;

                    actions.Add(action);
                }

                room = new Room(
                    new Word(
                        xRoom.SelectSingleNode("./name").InnerText,
                        xRoom.SelectSingleNode("./pheonetic").InnerText
                    ),
                    actions
                );

                rooms.Add(room);
            }

            return rooms;
        }

        private static void AddCommand(Dictionary<string, Command> phrases, Command command) {
            Console.WriteLine(string.Format("listening for: `{0}`", command.Phrase));
            phrases.Add(command.Phrase, command);
        }

        private static void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            if (_phrases.ContainsKey(e.Result.Text)) {
                Command command = _phrases[e.Result.Text];
                Console.WriteLine(string.Format("recognized: {0}", command));
                SendCommand(command.Id);
            }
            else
                Console.WriteLine(string.Format("not recognized: {0}", e.Result.Text));
        }

        private static void SendCommand(int id) {
            switch (id) {
                case 3:
                    Plex.Manager.GetPlayer().Play();
                    break;
                case 4:
                    Plex.Manager.GetPlayer().Pause();
                    break;
                case 11:
                    Plex.Manager.GetPlayer().Stop();
                    break;
                case 12:
                    Plex.Manager.GetPlayer().Rewind();
                    break;
                case 13:
                    Plex.Manager.GetPlayer().FastForward();
                    break;
                case 14:
                    Plex.Manager.GetPlayer().Select();
                    break;
                case 15:
                    Plex.Manager.GetPlayer().Back();
                    break;
                case 16:
                    Plex.Manager.GetPlayer().Up();
                    break;
                case 17:
                    Plex.Manager.GetPlayer().Down();
                    break;
                case 18:
                    Plex.Manager.GetPlayer().Left();
                    break;
                case 19:
                    Plex.Manager.GetPlayer().Right();
                    break;
                case 20:
                    Plex.Manager.GetPlayer().PageUp();
                    break;
                case 21:
                    Plex.Manager.GetPlayer().PageDown();
                    break;
            }
        }
    }
}