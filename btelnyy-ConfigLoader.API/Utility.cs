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
    }
}
