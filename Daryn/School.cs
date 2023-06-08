using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class School
    {
        public string schoolName;
        public string schoolNameKz;
        public Dictionary<int, ClassRoom> classCollection;

        public School(string nameSchool, string nameSchoolKz)
        {
            schoolName = nameSchool;
            schoolNameKz = nameSchoolKz;
            classCollection = new Dictionary<int, ClassRoom>();
        }

        public void AddClass(int n)
        {
            if (!classCollection.ContainsKey(n)) classCollection.Add(n, new ClassRoom());
        }
    }
}
