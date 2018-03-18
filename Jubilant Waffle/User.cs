using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jubilant_Waffle {
    class User {
        public string imagePath;
        public string publicName;
        public string ip;
        public string name;
        public string surname;

        public User(string name, string ip, string imagePath = null) {
            this.publicName = name;
            this.ip = ip;
            this.imagePath = imagePath;
        }
    }
}
