using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jubilant_Waffle {
    class User {
        public string imagePath;
        public string name;
        public string ip;

        public User(string name, string ip, string imagePath = null) {
            this.name = name;
            this.ip = ip;
            this.imagePath = imagePath;
        }
    }
}
