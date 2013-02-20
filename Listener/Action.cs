using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener {
    internal class Action {
        private Word _word;
        private Option _defaultOption;
        private List<Option> _options;
        private bool _optionRequired, _skipKeyword = false;

        public Word Word {
            get { return _word; }
        }

        public List<Option> Options {
            get { return _options; }
        }

        public bool OptionRequired {
            get { return _optionRequired; }
        }

        public bool SkipKeyword {
            get { return _skipKeyword; }
            set { _skipKeyword = value; }
        }

        public Option DefaultOption {
            get { return _defaultOption; }
        }

        public Action(Word word, List<Option> options) {
            _word = word;
            _options = options;
            _optionRequired = true;
        }

        public Action(Word word, List<Option> options, string defaultOption) {
            _word = word;
            _options = options;
            _optionRequired = false;

            foreach (Option option in _options) {
                if (option.Word.Name == defaultOption) {
                    _defaultOption = option;
                    break;
                }
            }

            if (_defaultOption == null)
                throw new Exception(string.Format("default option '{0}' not found", defaultOption));
        }
    }
}
