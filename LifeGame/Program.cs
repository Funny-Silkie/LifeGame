using Altseed2;

namespace LifeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Engine.Initialize("Life Game", 960, 720)) return;
            Engine.ClearColor = default;
            Engine.AddNode(new MainScene());
            while (Engine.DoEvents()) Engine.Update();
            Engine.Terminate();
        }
    }
}
