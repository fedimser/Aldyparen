﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Aldyparen
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormMain f = new FormMain();
            f.arguments = args;
            Application.Run(f);
        }
    }
}
