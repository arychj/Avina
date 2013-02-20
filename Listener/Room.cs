using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener {
    internal class Room {
        private Word _word;
        private List<Action> _actions;

        public Word Word {
            get { return _word; }
        }

        public List<Action> Actions {
            get { return _actions; }
        }

        public Room(Word word, List<Action> actions) {
            _word = word;
            _actions = actions;
        }
    }
}
