using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class User
    {
        public string iin;
        public string name;
        public string surname;
        public string parentName;
        public int mathematic;
        public int reading;
        public int history;
        public int total_ball;
        int sex;
        public int region_school_id;
        public string lang;
        public int cls;
        public int school_id;
        public bool calculate = false;
        public bool noValidVariant = false;

        public void Create(string[] cols)
        {
            name = cols[4];
            surname = cols[5];
            parentName = cols[6];
            iin = cols[7];
            sex = int.Parse(cols[9]);
            region_school_id = int.Parse(cols[13]);
            lang = cols[14];
            cls = int.Parse(cols[15]);
            school_id = int.Parse(cols[16]);
        }

        public void Finalization(int m, int r, int h, int total)
        {
            mathematic = m;
            reading = r;
            history = h;
            total_ball = total;
            calculate = true;
        }
    }
}
