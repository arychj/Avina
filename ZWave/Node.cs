﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave {
    class Node {
        private string _name;

        public Node(string name) {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }

        public bool SendCommand(string command) {
            return true;
        }
    }
}
