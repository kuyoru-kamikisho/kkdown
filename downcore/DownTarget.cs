using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace downcore
{
    public class DownTarget
    {
        public string url;
        public long size;
        public string fileName;
        public List<long>? pieceProgress;

        public double totalProgress
        {
            get
            {
                double sum = 0;
                if (pieceProgress != null)
                {
                    foreach (long progress in pieceProgress)
                    {
                        sum += progress / size;
                    }
                }
                return sum;
            }
        }
    }
}
