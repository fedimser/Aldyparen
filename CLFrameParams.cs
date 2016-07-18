using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aldyparen
{
    public class CLFrameParams
    {
        public double[] k;
        public int N;
        public int maxUsedColors;

        public CLFrameParams(int kCount, int colorsCount)
        {
            N = kCount;
            maxUsedColors = colorsCount;
            k = new double[N];
        }

        //step=0.0 - the same
        //step=1.0 - totally new
        //linear
        public CLFrameParams getMove(CLFrameParams newParams, double step)
        {
            CLFrameParams ret = new CLFrameParams(this.N, this.maxUsedColors);
            for (int i = 0; i < N; i++)
            {
                ret.k[i] = this.k[i] + (newParams.k[i] - this.k[i]) * step;
            }

            return ret;
        }

        public CLFrameParams clone()
        {
            CLFrameParams ret = new CLFrameParams(this.N, this.maxUsedColors);
            for (int i = 0; i < N; i++)
            {
                ret.k[i] = this.k[i];
            }

            return ret;
        }

    }
}
