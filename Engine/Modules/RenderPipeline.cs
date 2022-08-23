using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace Engine
{
    internal class RenderPipeline
    {
        #region Fields

        public RenderWindow Window { private set; get; }
        public GameEngine Engine { private set; get; }

        public List<IUpdate> Members { private set; get; } = new List<IUpdate>();

        #endregion

        internal RenderPipeline(GameEngine engine, string title)
        {
            Engine = engine;

            Thread thread = new Thread(() =>
            {
                #region Initialization window

                Window = new RenderWindow(new VideoMode((uint)Screen.Width, (uint)Screen.Height), title, Styles.Default);
                Window.SetFramerateLimit(144);
                Window.SetVerticalSyncEnabled(true);

                Window.Closed += (a, b) =>
                {
                    Window.Close();
                    Environment.Exit(0);
                };

                #endregion

                Main();
            });
            thread.Start();
        }

        private void Main()
        {
            Stopwatch deltaTime = new Stopwatch();

            while (Window.IsOpen)
            {
                deltaTime.Start();

                #region Main

                Window.DispatchEvents();
                Window.Clear(Color.Black);

                for (int i = 0; i < Members.Count; i++)
                    Members[i].Update(Window);

                Window.Display();

                #endregion

                Time.UnscaleDeltaTime = (float)deltaTime.Elapsed.TotalSeconds;
                Time.Elapsed += Time.DeltaTime;
                deltaTime.Restart();
            }
        }
    }

    internal interface IUpdate
    {
        public void Update(RenderWindow window);
    }
}
