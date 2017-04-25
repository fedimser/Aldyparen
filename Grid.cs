using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Aldyparen
{
    public class Grid
    {
        public Color annotationColor = Color.Black;
        public Font annotationFont = new Font("Arial", 10);
        public bool annotate = false;

        public Pen axesPen = new Pen(Color.Red, 2);
        public Pen gridPen = Pens.Red;
        public bool useAxes = false;
        public bool useGrid = false;
        public bool useTicks = false;
       

        public Grid(){}
 
    }
}
