using Altseed2;

namespace LifeGame
{
    class Block : SpriteNode
    {
        private static readonly Color AliveColor = new Color(255, 255, 255);
        private static readonly Color DeadColor = new Color(100, 100, 100);
        private static readonly Texture2D texture = Texture2D.LoadStrict("Resources/Block.png");
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
        }
        private int CountLives()
        {
            var result = 0;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, -1), out var block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(0, -1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, -1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, 0), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(0, 0), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, 0), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(-1, 1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(0, 1), out block) && block.IsAlive) result++;
            if (scene.Blocks.TryGetValue(Location + new Vector2I(1, 1), out block) && block.IsAlive) result++;
            return result;
        }
        protected override void OnUpdate()
        {
            var count = CountLives();
            IsAlive = MainScene.Result[new Entry(count, IsAlive)];
        }
    }
}
