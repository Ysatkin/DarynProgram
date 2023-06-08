using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Daryn
{
    public partial class Form1 : Form
    {
        const string folderBaseUsers = "База учеников\\";
        const string folderResult = "ved\\";

        double[] duration = new double[5];
        Daryn_Functions df;
        GenerateTestFiles gtf = new GenerateTestFiles();
        RegionCollection rc;

        public Form1()
        {
            InitializeComponent();
            Directory.CreateDirectory("Ключи");
            Directory.CreateDirectory("Ответы");
            Directory.CreateDirectory(folderResult);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!Directory.Exists("База учеников"))
            {
                MessageBox.Show("Нет папки \"База учеников\". Данные: экспорт csv-таблица базы Daryn", "Отсутствует база",  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime dt = DateTime.Now;
            Application.DoEvents();
            df = new Daryn_Functions();
            duration[0] = (DateTime.Now - dt).TotalSeconds;
            label5.Text = duration[0].ToString("0.000") + " сек";
            label16.Text = duration[0].ToString("0.000") + " сек";

            label2.Text = df.filesAnswers.Length.ToString();
            label4.Text = df.usersAnswers.Count.ToString();
            if (df.usersAnswers.Count > 0) button1.Enabled = true;
        }

        string rus_to_lat(string rusString)
        {
            string latString = "";
            string rus = " абвгдеё жзийклмнопрстуфхц ч ш щ ыэю я ";
            string lat = "_abvgdeyojziyklmnoprstufhtschshshyeyuya";

            rusString = rusString.ToLower();
            rusString = rusString.Replace("«", "").Replace("»", "");
            rusString = rusString.ToLower().Replace("ь", "").Replace("ъ", "");
            rusString = rusString.Replace("№ ", "").Replace("№", "");
            if (rusString.Length > 70) rusString = rusString.Substring(0, 70);

            for (int p = 0; p < rusString.Length; p++)
            {
                char ch = rusString[p];
                int w = rus.IndexOf(ch);
                if (w == -1) latString += rusString[p];
                else
                {
                    latString += lat[w];
                    if (rus[w + 1] == ' ') latString += lat[w + 1];
                }
            }
            return latString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Application.DoEvents();
            df.notFoundKeys = new List<string>();
            df.notValidCountAnswers = new List<string>();
            int xxx = 0;
            rc = new RegionCollection();

            Process[] ListProccess = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in ListProccess) proc.Kill();

            //Подсчёт баллов
            DateTime dt = DateTime.Now;
            for (int y = 0; y < df.usersAnswers.Count; y++)
            {
                string s = df.usersAnswers[y].Replace("\t", "").Replace(" ", "");
                string iin = s.Substring(0, 12);
                int iinIndex = df.IINs.IndexOf(iin);

                if (df.Check_IIN(iin))
                {
                    int variant = 0;
                    if (!int.TryParse(s.Substring(12, 2), out variant))
                    {
                        User uTemp = df.users[iinIndex];
                        df.notValidCountAnswers.Add(uTemp.surname + " " + uTemp.name + " (" + uTemp.iin + ") указал некорректный вариант " + s.Substring(12, 2));
                        df.users[iinIndex].noValidVariant = true;
                    }
                    else
                    {
                        string answers = s.Substring(14);
                        df.Calc(iin, variant, answers);
                    }
                }
            }
            duration[1] = (DateTime.Now - dt).TotalSeconds;
            label6.Text = duration[1].ToString("0.000") + " сек";

            //Разложение на группы
            dt = DateTime.Now;
            Dictionary<int, string> rg = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(folderBaseUsers + "daryn_table_region.csv");
            for (int y = 1; y < lines.Length; y++)
            {
                string[] s = lines[y].Replace("\"", "").Split(';');
                rg.Add(int.Parse(s[0]), s[1]);
            }
            Dictionary<int, string> sch = new Dictionary<int, string>();
            Dictionary<int, string> sch_kz = new Dictionary<int, string>();
            lines = File.ReadAllLines(folderBaseUsers + "daryn_table_schools.csv");
            for (int y = 1; y < lines.Length; y++)
            {
                string[] s = lines[y].Replace("\"", "").Split(';');
                sch.Add(int.Parse(s[0]), s[1]);
                sch_kz.Add(int.Parse(s[0]), s[2]);
            }
            foreach (var user in df.users)
            {
                if (user.calculate | user.noValidVariant)
                {
                    int region_id = user.region_school_id;
                    string language = user.lang;
                    int school_id = user.school_id;
                    int cls = user.cls;
                    rc.AddRegion(region_id, rg[region_id]);
                    rc.regionCollection[region_id].AddLanguage(language);
                    rc.regionCollection[region_id].languageCollection[language].AddSchool(school_id, sch[school_id], sch_kz[school_id]);
                    rc.regionCollection[region_id].languageCollection[language].schoolCollection[school_id].AddClass(cls);
                    rc.regionCollection[region_id].languageCollection[language].schoolCollection[school_id].classCollection[cls].AddUser(user);
                }
            }
            duration[2] = (DateTime.Now - dt).TotalSeconds;
            label12.Text = duration[2].ToString("0.000") + " сек";

            //Сортировка по убыванию
            dt = DateTime.Now;
            foreach (var r in rc.regionCollection)
            {
                foreach (var l in r.Value.languageCollection)
                {
                    foreach (var s in l.Value.schoolCollection)
                    {
                        foreach (var c in s.Value.classCollection)
                        {
                            c.Value.Sort();
                        }
                    }
                }
            }
            duration[3] = (DateTime.Now - dt).TotalSeconds;
            label13.Text = duration[3].ToString("0.000") + " сек";

            //Экспорт в Excel
            string dir = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            dt = DateTime.Now;
            var ex = new Excel.Application();
            var book = ex.Workbooks.Open(dir + "Ведомость_шаблон.xlsx", false);
            var sheet = book.Worksheets[1];
            List<string> user_answers = new List<string>();

            foreach (var r in rc.regionCollection)
            {
                foreach (var l in r.Value.languageCollection)
                {
                    foreach (var s in l.Value.schoolCollection)
                    {
                        foreach (var c in s.Value.classCollection)
                        {
                            string schName = rus_to_lat(s.Value.schoolName);
                            string latRegion = rus_to_lat(r.Value.regionName);
                            Directory.CreateDirectory(folderResult + latRegion + "\\" + l.Key);

                            string xls_file_name = dir + string.Format("{0}{1}\\{2}\\{3}-{4}_-_{5}.xlsx",
                                                                        folderResult,
                                                                        latRegion,
                                                                        l.Key,
                                                                        c.Key.ToString(),
                                                                        l.Key,
                                                                        schName);
                            string txt_file_name = xls_file_name.Replace(".xlsx", ".txt");

                            sheet.Cells[1, 1] = s.Value.schoolNameKz + "\r\nоқуға түсу үшін конкурстық іріктеу үміткерлерінің\r\nқорытынды нәтижелерінің ведомосы";
                            sheet.Cells[1, 5] = "Ведомость итоговых результатов претендентов\r\nконкурсного отбора\r\nдля поступления в " + s.Value.schoolName;
                            object[,] v = new object[c.Value.users.Count, 8];
                            List<string> rTxt = new List<string>();
                            int w = 0;
                            for (int y = 0; y < c.Value.users.Count; y++)
                            {
                                if (!c.Value.users[y].noValidVariant)
                                {
                                    string x = "";
                                    v[y, 0] = y + 1;
                                    v[y, 1] = c.Value.users[y].surname;
                                    v[y, 2] = c.Value.users[y].name;
                                    v[y, 3] = c.Value.users[y].parentName;
                                    v[y, 4] = c.Value.users[y].total_ball;
                                    v[y, 5] = c.Value.users[y].mathematic;
                                    v[y, 6] = c.Value.users[y].reading;
                                    v[y, 7] = c.Value.users[y].history;
                                    for (int j = 0; j <= 7; j++) x += ";" + v[y, j];
                                    rTxt.Add(x.Substring(1));
                                    User us = c.Value.users[y];
                                    user_answers.Add(us.iin + ";" + us.mathematic + ";" + us.reading + ";" + us.history);
                                    w++;
                                }
                            }
                            Excel.Range rrr = ex.get_Range("A3", "H" + (w + 2));
                            rrr.Value2 = v;

                            rrr = ex.get_Range("A3", "H" + (w + 2));
                            rrr.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            if (File.Exists(xls_file_name)) File.Delete(xls_file_name);
                            book.SaveAs(xls_file_name);
                            File.WriteAllLines(txt_file_name, rTxt, Encoding.UTF8);
                            xxx += c.Value.users.Count;
                            sheet.Range("A3:H" + c.Value.users.Count + 2).Delete();
                            progressBar1.Value = xxx * 100 / df.usersAnswers.Count;
                            Application.DoEvents();
                        }
                    }
                }
            }
            book.Close(false);
            ex.Quit();
            File.WriteAllLines("ved\\user_answers.txt", user_answers);
            ListProccess = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in ListProccess) proc.Kill();
            duration[4] = (DateTime.Now - dt).TotalSeconds;
            label14.Text = duration[4].ToString("0.000") + " сек";
            progressBar1.Value = 100;

            //
            File.WriteAllLines("Этих ИИН нет в базе.txt", df.notFoundKeys,Encoding.UTF8);
            File.WriteAllLines("Эти ученики имеют несоответствующее кол-во ответов.txt", df.notValidCountAnswers, Encoding.UTF8);
            // ------- //
            label16.Text = duration.ToList().Sum().ToString("0.000") + " сек";
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gtf.GenerateKeysTest();
            gtf.GenerateAnswersTest();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            gtf.GenerateKeysTest();
            gtf.GenerateAnswersTest();
        }
    }
}
