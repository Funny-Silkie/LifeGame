using Altseed2;

namespace LifeGame
{
    class Block : SpriteNode
    {
        private static readonly Color AliveColor = new Color(255, 255, 255);
        private static readonly Color DeadColor = new Color(100, 100, 100);
        private static readonly Texture2D texture = Texture2D.LoadStrict("Resources/Block.png");
        private bool? nextLife = null;
        private readonly MainScene scene;
        public bool IsAlive
        {
            get => _isAlive;
            set
            {
                if (_isAlive == value) return;
                _isAlive = value;
                Color = value ? AliveColor : DeadColor;
            }
        }
        private bool _isAlive;
        public Vector2I Location { get; }
        public Block(Vector2I location, MainScene scene)
        {
            this.scene = scene;
            Texture = texture;
            Location = location;
            Position = location * texture.Size;
            Color = DeadColor;
            AdjustSize();
        }
        public void ChangeAlive()
        {
            if (nextLife.HasValue) IsAlive = nextLife.Value;
        }
        public bool CheckCount()
        {
            var result = DataBase.LiveDeadTable[new Entry(CountLives(), IsAlive)];
            nextLife = result;
            return result;
        }
        private int CountLives()
        {
            var result = 0;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, -1), out var block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(0, -1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, -1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, 0), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, 0), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, 1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(0, 1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, 1), out block) && block.IsAlive) result++;
            return result;
        }
        private bool IsMouseEnter()
        {
            var pos = Engine.Mouse.Position - Position;
            return 0 <= pos.X && pos.X <= Size.X && 0 <= pos.Y && pos.Y <= Size.Y;
        }
        protected override void OnUpdate()
        {
            if (IsMouseEnter() && Engine.Mouse.GetMouseButtonState(MouseButtons.ButtonLeft) == ButtonState.Hold) IsAlive = true;
        }
    }
}
