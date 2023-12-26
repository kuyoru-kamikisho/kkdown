using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace downlist
{
    public class FileRef
    {
        public string name;
        public string filename;
        public string url;
        public string outPath;
        public long? size;

        public FileRef Clone()
        {

            return new FileRef
            {
                name = name,
                filename = filename,
                url = url,
                outPath = outPath,
                size = size
            };
        }
    }
}
