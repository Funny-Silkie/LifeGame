﻿using Altseed2;
using Altseed2.ToolAuxiliary;
using System;

namespace LifeGame
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!Engine.Initialize("Life Game", 1260, 720, new Configuration()
            {
                ToolEnabled = true
            })) return;
            Engine.ClearColor = default;
            DataBase.Initialize(args);
            while (Engine.DoEvents())
            {
                Engine.WindowTitle = $"Life Game FPS:{Engine.CurrentFPS}";
                ToolHelper.Update();
                Engine.Update();
            }
            Engine.Terminate();
        }
    }
}
