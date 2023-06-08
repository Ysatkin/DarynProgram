using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class RegionCollection
    {
        public Dictionary<int, Region> regionCollection;

        public RegionCollection()
        {
            regionCollection = new Dictionary<int, Region>();
        }

        public void AddRegion(int id_region, string name_region)
        {
            if (!regionCollection.ContainsKey(id_region)) regionCollection.Add(id_region, new Region(name_region));
        }
    }
}
