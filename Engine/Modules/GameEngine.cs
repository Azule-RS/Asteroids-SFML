namespace Engine
{
    public class GameEngine
    {
        internal static GameEngine? active { private set; get; }

        internal RenderPipeline RenderPipeline { private set; get; }
        public Input Input { private set; get; }
        public Debug Debug { private set; get; }
        public Physics Physics { private set; get; }

        public GameEngine(string title)
        {
            active = this;

            Debug = new Debug();

            RenderPipeline = new RenderPipeline(this, title);
            Input = new Input(this);
            Physics = new Physics();
        }
    }
}