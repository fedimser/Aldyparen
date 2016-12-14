using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;

namespace Aldyparen
{
    public class FrameParams
    {
        public double[] k;
        public int N;
        public int maxUsedColors = Frame.MAX_COLORS;


        public Function genFunc;    //function used for generation.
        public double genInfty;      //Limit for magnitude to consider that sequence diverges.
        public int genSteps;        //Limit for iterations to consider that sequence converges.
        public Complex genInit;       //First point of sequence
         

        public FrameParams(int kCount,int _maxUsedColors)
        {
            N = kCount; 
            k = new double[N];
            maxUsedColors = _maxUsedColors;

        }

        //step=0.0 - the same
        //step=1.0 - totally new
        //linear
        public FrameParams getMove(FrameParams newParams, double step)
        {
            FrameParams ret = this.clone();
            ret.genFunc.move(newParams.genFunc,step);
            for (int i = 0; i < N; i++)
            {
                ret.k[i] = this.k[i] + (newParams.k[i] - this.k[i]) * step;
            }

            return ret;
        }
         


        public FrameParams clone()
        {
            FrameParams ret = new FrameParams(this.N ,this.maxUsedColors);
            for (int i = 0; i < N; i++)
            {
                ret.k[i] = this.k[i];
            }

            if(this.genFunc!=null)ret.genFunc = this.genFunc.clone();
            ret.genInfty = this.genInfty;
            ret.genInit = this.genInit;
            ret.genSteps = this.genSteps;  

            return ret;
        }

    }
}
