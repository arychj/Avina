using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener {
    internal class Option {
        private Word _word;
        private int _id;

        public Word Word {
            get { return _word; }
        }

        public int Id {
            get { return _id; }
        }

        public Option(Word word, int id) {
            _word = word;
            _id = id;
        }
    }
}
