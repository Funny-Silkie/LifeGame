using Altseed2;

namespace LifeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Engine.Initialize("Life Game", 1260, 720, new Configuration()
            {
                ToolEnabled = true
            })) return;
            Engine.ClearColor = default;
            DataBase.Initialize();
            while (Engine.DoEvents())
            {
                ToolHelper.Update();
                Engine.Update();
            }
            Engine.Terminate();
        }
    }
}
