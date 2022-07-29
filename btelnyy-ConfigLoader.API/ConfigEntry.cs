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
                if (DataTags.Contains(Tags.HIDE_TAGS))
                {
                    break;
                }
                str += t.ToString();
                if(t != DataTags.Last())
                {
                    str += ",";
                    continue;
                }
            }
            str += "\n";
            str += "Readonly: " + Readonly + "\n";
            str += "Origin Line: " + OriginLine;
            return str;
        }
        public string GetTags()
        {
            string tags = "";
            if (DataTags.Contains(Tags.HIDE_TAGS))
            {
                Log.WriteDebug("Tags are hidden, Actual amount: " + DataTags.Count);
                return "Total Tags: 0";
            }
            tags += "Total Tags: " + DataTags.Count;
            tags += "\n";
            foreach(Tags t in DataTags)
            {
                tags += "Tag: " + t.ToString() + "\n";
                tags += "Tag Index: " + (int) t + "\n";
                tags += "Tag Hex: " + ((int) t).ToString("X") + "\n";
            }
            return tags;
        }
    }
    public enum Tags
    {
        READONLY,
        HIDDEN,
        NOTIFY_ON_CHANGE,
        LOCK_TAGS,
        HIDE_TAGS,
    }
}
