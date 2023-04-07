using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    internal class Utility
    {
        public static void LineChange(string FileName, int LineToEdit, string NewText)
        {
            string[] arrLine = File.ReadAllLines(FileName);
            if(arrLine.Length < LineToEdit)
            {
                arrLine.Append(NewText);
            }
            else
            {
                arrLine[LineToEdit] = NewText;
            }
            File.WriteAllLines(FileName, arrLine);
        }
        public static int GetLength(string FilePath)
        {
            return File.ReadAllLines(FilePath).Length;
        }
        public static string LineBuilder(string key, string value, string seperator)
        {
            return key + seperator + value;
        }
        public static string TagsLineBuilder(IEnumerable<Tags> tags)
        {
            string str = "[";
            foreach(Tags tag in tags)
            {
                if(tag == tags.Last())
                {
                    str += tag.ToString();
                }
                else
                {
                    str += tag.ToString() + ",";
                }
            }
            str += "]";
            return str;
        }

        public static List<string> ListParse(string str)
        {
            string[] strings = str.Split(",");
            return strings.ToList();
        }

        public static string ListParse(List<string> list) 
        {
            string str = "";
            foreach(string s in list)
            {
                if(list.Last() == s)
                {
                    str += s;
                }
                else
                {
                    str += s + ",";
                }
            }
            return str;
        }

        public static string DictParse(Dictionary<string, string> dict)
        {
            string str = "";
            List<string> pairs = new List<string>();
            foreach(string s in dict.Keys)
            {
                string pair = s + "|" + dict[s];
                pairs.Add(pair);
            }
            foreach(string s in pairs)
            {
                if(pairs.Last() == s)
                {
                    str += s;
                }
                else
                {
                    str += s + ",";
                }
            }
            return str;
        }

        public static Dictionary<string, string> DictParse(string str)
        {
            Dictionary<string, string> dict = new();
            string[] pairs = str.Split(",");
            foreach(string s in pairs)
            {
                Log.WriteDebug(s);
                string[] pair = s.Split("|");
                if (dict.ContainsKey(pairs[0]))
                {
                    Log.WriteWarning("Dict duplicate detected.");
                }
                Log.WriteDebug(pair[0]);
                Log.WriteDebug(pair[1]);
                dict.Add(pair[0], pair[1]);
            }
            return dict;
        }
    }
}
