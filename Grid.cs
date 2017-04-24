using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Aldyparen
{
    public class Grid
    {
        public Pen axisPen = new Pen(Color.Red, 2);
        public Pen gridPen = Pens.Red;
        public Color annotationColor = Color.Black;
        public Font annotationFont = new Font("Arial", 10); 
        public bool useGrid = false;
        public bool annotate = false;

        public Grid(bool useGrid) {
            this.useGrid = useGrid;
        }
 
    }
}
