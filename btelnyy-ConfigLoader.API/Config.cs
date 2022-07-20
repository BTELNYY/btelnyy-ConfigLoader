using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    public class Config
    {
        public string Data = "";
        public List<Tags> DataTags = new();
        public int OriginLine = -1;
        public bool Readonly = false;
    }
    public enum Tags
    {
        READONLY,
        HIDDEN,
        NOTIFY_ON_CHANGE,
    }
}
