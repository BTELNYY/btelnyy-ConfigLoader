using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    public class ConfigEntry
    {
        public string Data = "";
        public List<Tags> DataTags = new();
        public int OriginLine = -1;
        public bool Readonly = false;

        public override string ToString()
        {
            string str = "";
            str = "Data: " + Data + "\n";
            str += "Tags: ";
            foreach(Tags t in DataTags)
            {
                str += t.ToString();
                if(t != DataTags.Last())
                {
                    str += ",";
                    continue;
                }
                str += "\n";
            }
            str += "Readonly: " + Readonly.ToString() + "\n";
            str += "Origin Line: " + OriginLine.ToString() + "\n";
            return str;
        }
    }
    public enum Tags
    {
        READONLY,
        HIDDEN,
        NOTIFY_ON_CHANGE,
        LOCK_TAGS,
    }
}
