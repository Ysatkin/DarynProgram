using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Daryn
{
    class GenerateTestFiles
    {
        const string folderBaseUsers = "База учеников\\";
        const string folderAnswers = "Ответы\\";
        int[] cnt = new int[] { 40, 60, 75, 0, 0, 95 };

        public void GenerateKeysTest()
        {
            Random rnd = new Random();
            for (int i = 5; i <= 10; i++)
            {
                if (i == 8 | i == 9) continue;
                List<string> rows = new List<string>();
                for (int y = 1; y <= 24; y++)
                {
                    string s = "";
                    for (int x = 1; x <= cnt[i - 5]; x++)
                    {
                        char ch = (char)rnd.Next(65, 65 + i - 2);
                        s += ch;
                    }
                    rows.Add(s);
                }
                File.WriteAllLines("Ключи\\key" + i + ".txt", rows);
            }

        }

        public void GenerateAnswersTest()
        {
            Directory.Delete(folderAnswers, true);
            Directory.CreateDirectory(folderAnswers);
            Random rnd = new Random();
            int q = rnd.Next(200, 1000);
            int fn_num = 1;
            List<string> rows = new List<string>();

            string[] FullIINs = File.ReadAllLines(folderBaseUsers + "daryn_table_users.csv", Encoding.UTF8);

            for (int k = 0; k < FullIINs.Length; k++)
            {
                string[] cols = FullIINs[k].Replace("\"", "").Split(';');
                if (cols[3] == "1")
                {
                    string iin = cols[7];
                    int cls = int.Parse(cols[15]);
                    if (cls == 5 | cls == 6 | cls == 7 | cls == 10)
                    {
                        string s = "";
                        for (int x = 1; x <= cnt[cls - 5]; x++)
                        {
                            char ch = (char)rnd.Next(65, 65 + cls - 2);
                            s += ch;
                        }
                        s = iin + rnd.Next(1, 24).ToString("00") + s;
                        rows.Add(s);
                        if (rows.Count == q)
                        {
                            File.WriteAllLines(folderAnswers + "answers" + fn_num.ToString("000") + ".txt", rows);
                            fn_num++;
                            q = rnd.Next(200, 1000);
                            rows = new List<string>();
                        }
                    }
                }
            }
            if (rows.Count > 0) File.WriteAllLines(folderAnswers + "answers" + fn_num.ToString("000") + ".txt", rows);
        }
    }
}
