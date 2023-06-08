using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class Language
    {
        public Dictionary<int, School> schoolCollection;

        public Language()
        {
            schoolCollection = new Dictionary<int, School>();
        }

        public void AddSchool(int id_school, string name_school, string name_school_kz)
        {
            if (!schoolCollection.ContainsKey(id_school)) schoolCollection.Add(id_school, new School(name_school, name_school_kz));
        }
    }
}
