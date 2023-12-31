﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace downcore
{
    public class DownTarget
    {
        public string? url;
        public long size;
        public string? fileName;
        public string? errorInfo;
        public long[]? pieceGetedSize;
        public long[]? starts;
        public long[]? ends;

        public double getTotalProgress()
        {
            double sum = 0;
            if (pieceGetedSize != null)
            {
                foreach (long progress in pieceGetedSize)
                {
                    sum += progress / (double)size;
                }
            }
            return sum;
        }

        public double getPieceProgress(int index)
        {
            if (fileName != null)
            {
                return pieceGetedSize[index] / (ends[index] - starts[index] + 1.00);
            }
            return 0;
        }
    }
}
