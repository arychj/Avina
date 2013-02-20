using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener {
    internal class Command {
        private int _id;
        private string _command, _phrase;

        public int Id {
            get { return _id; }
        }

        public string Phrase {
            get { return _phrase; }
        }

        public override string ToString() {
            return _command;
        }

        public Command(int id, string command, string phrase) {
            _id = id;
            _command = command;
            _phrase = phrase;
        }
    }
}
