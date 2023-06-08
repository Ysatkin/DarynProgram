using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryn
{
    class Region
    {
        public string regionName;
        public Dictionary<string,Language> languageCollection;

        public Region(string regName)
        {
            regionName = regName;
            languageCollection = new Dictionary<string, Language>();
        }

        public void AddLanguage(string lang)
        {
            if (languageCollection.Count < 2)
            {
                if (!languageCollection.ContainsKey(lang)) languageCollection.Add(lang, new Language());
            }
        }
    }
}
