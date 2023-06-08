using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class ClassRoom
    {
        public List<User> users;

        public ClassRoom()
        {
            users = new List<User>();
        }

        public void AddUser(User u)
        {
            users.Add(u);
        }

        public void Sort()
        {
            users.Sort(new Comparison<User>((a, b) => b.total_ball - a.total_ball));
        }
    }
}
