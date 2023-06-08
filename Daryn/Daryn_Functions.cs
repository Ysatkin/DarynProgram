using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Daryn
{
    class Daryn_Functions
    {
        const string folderBaseUsers = "База учеников\\";
        const string folderKeys = "Ключи\\";
        const string folderAnswers = "Ответы\\";
        List<int>[] normAnswers;

        public string[] filesAnswers;

        List<string>[] validKeys;
        public List<string> usersAnswers;
        public List<User> users;
        public List<string> IINs = new List<string>();
        public List<string> notFoundKeys;
        public List<string> notValidCountAnswers;

        public Daryn_Functions()
        {
            normAnswers = new List<int>[6];
            normAnswers[0] = new List<int> { 30, 10, 0 };
            normAnswers[1] = new List<int> { 35, 15, 10 };
            normAnswers[2] = new List<int> { 55, 10, 10 };
            normAnswers[3] = new List<int> { 0, 0, 0 };
            normAnswers[4] = new List<int> { 0, 0, 0 };
            normAnswers[5] = new List<int> { 60, 10, 25 };
            ReadKeys();
            ReadAnswers();
            ReadBaseUsers();
        }

        //Чтение ключей
        public void ReadKeys()
        {
            validKeys = new List<string>[6];
            for (int i = 5; i <= 10; i++)
            {
                if (i == 8 | i == 9)
                {
                    validKeys[i - 5] = new List<string>();
                    continue;
                }
                int normCount = normAnswers[i - 5].Sum();
                validKeys[i - 5] = new List<string>();
                string fn = folderKeys + "key" + i + ".txt";
                if (!File.Exists(fn))
                {
                    MessageBox.Show("Нет файла ключа " + fn, "Отсутствует файл ключа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string[] lines = File.ReadAllLines(fn, Encoding.Default);
                for (int y = 0; y < lines.Length; y++)
                {
                    if (lines[y] == "")
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Найдена пустая строка", "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (lines[y].Trim().Length != normCount)
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Неверное кол-во ключей в варианте", "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (lines[y].ToUpper() != lines[y])
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Все ключи должны быть заглавными буквами", "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int letter_a=lines[y].IndexOf("А");
                    if (letter_a != -1)
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Встретилась русская буква \"A\". Позиция " + (letter_a + 1), "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int letter_b = lines[y].IndexOf("В");
                    if (letter_b != -1)
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Встретилась русская буква \"B\". Позиция " + (letter_b + 1), "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int letter_c = lines[y].IndexOf("С");
                    if (letter_c != -1)
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Встретилась русская буква \"C\". Позиция " + (letter_c + 1), "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int letter_e = lines[y].IndexOf("Е");
                    if (letter_e != -1)
                    {
                        MessageBox.Show("Файл " + fn + ". Строка " + (y + 1) + ". Встретилась русская буква \"E\". Позиция " + (letter_e + 1), "Ошибка чтения файла ключей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    validKeys[i - 5].Add(lines[y]);
                }
            }
        }

        //Чтение ответов
        public void ReadAnswers()
        {
            usersAnswers = new List<string>();
            filesAnswers = Directory.GetFiles(folderAnswers, "*.*");
            foreach (var x in filesAnswers) usersAnswers.AddRange(File.ReadAllLines(x, Encoding.UTF8));
        }

        //Чтение базы данных
        public void ReadBaseUsers()
        {
            users = new List<User>();
            string[] FullIINs = File.ReadAllLines(folderBaseUsers + "daryn_table_users.csv", Encoding.UTF8);
            Dictionary<string, bool> temp = new Dictionary<string, bool>();

            for (int k = 1; k < FullIINs.Length; k++)
            {
                string[] cols = FullIINs[k].Replace("\"", "").Split(';');
                if (cols[3] == "1")
                {
                    string iin = cols[7];
                    User u = new User();
                    u.Create(cols);
                    if (!temp.ContainsKey(iin))
                    {
                        IINs.Add(iin);
                        users.Add(u);
                        temp.Add(iin, true);
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с ИИН \"" + iin + "\" в базе встретился более 1 раза", "Дубликат ИИН", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
        }

        public bool Check_IIN(string iin)
        {
            if (!IINs.Contains(iin))
            {
                notFoundKeys.Add(iin + " не найден. Возможные варианты:");
                Dictionary<string, string> variantsIIN = new Dictionary<string, string>();
                for (int aa = 0; aa < IINs.Count; aa++)
                {
                    int cc = 0;
                    for (int bb = 0; bb < IINs[aa].Length; bb++)
                    {
                        if (IINs[aa][bb] == iin[bb]) cc++;
                        if (cc >= 10)
                        {
                            if (!variantsIIN.ContainsKey(IINs[aa])) variantsIIN.Add(IINs[aa], users[aa].surname + " " + users[aa].name);
                        }
                    }
                }
                foreach (var vv in variantsIIN) notFoundKeys.Add(vv.Key + " " + vv.Value);
                notFoundKeys.Add("");
                return false;
            }
            return true;
        }

        public void Calc(string iin, int variant, string answers)
        {
            int mathematic = 0;
            int reading = 0;
            int history = 0;
            int total_ball = 0;

            int iinIndex = IINs.IndexOf(iin);
            int cls = users[iinIndex].cls;

            if (answers.Length != normAnswers[cls - 5].Sum())
            {
                User uTemp = users[iinIndex];
                notValidCountAnswers.Add(uTemp.surname + " " + uTemp.name + " (" + uTemp.iin + ") содержит " + answers.Length + " ответов вместо " + normAnswers[cls - 5].Sum());
                return;
            }
            if (variant > validKeys[cls - 5].Count)
            {
                User uTemp = users[iinIndex];
                notValidCountAnswers.Add(uTemp.surname + " " + uTemp.name + " (" + uTemp.iin + ") указал вариант " + variant + " из " + validKeys[cls - 5].Count + " возможных");
                return;
            }

            string key = validKeys[cls - 5][variant - 1];

            int n1 = normAnswers[cls - 5][0];
            int n2 = normAnswers[cls - 5][1];

            for (int i = 1; i <= answers.Length; i++)
            {
                char ch_key = key[i - 1];
                char ch_ans = answers[i - 1];
                if (ch_ans == 'O') continue;
                if (i <= n1)
                {
                    if (ch_key == ch_ans) mathematic += 4; else mathematic--;
                }
                if (i > n1 & i <= n1 + n2)
                {
                    if (ch_key == ch_ans) reading += 4; else reading--;
                }
                if (cls > 5)
                {
                    if (i > n1 + n2)
                    {
                        if (ch_key == ch_ans) history += 4; else history--;
                    }
                }
            }
            total_ball += mathematic + reading + history;
            users[iinIndex].Finalization(mathematic, reading, history, total_ball);
        }
    }
}
