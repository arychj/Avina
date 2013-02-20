using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener {
    internal class Word {
        private string _name, _pheonetic;

        public string Name {
            get { return _name; }
        }

        public string Pheonetic {
            get { return _pheonetic; }
        }

        public Word(string name, string pheonetic) {
            _name = name;
            _pheonetic = pheonetic;
        }
    }
}
