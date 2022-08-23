using SFML.Graphics;
using SFML.Window;

namespace Engine
{
    public class Input : IUpdate
    {
        public enum KeyState
        {
            Down,
            Pressed,
            Up,
            Unpressed
        }

        internal static Input active { private set; get; }

        private Dictionary<Keyboard.Key, KeyState> Keys;

        internal Input(GameEngine engine)
        {
            active = this;

            Keys = new Dictionary<Keyboard.Key, KeyState>();
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
                Keys.TryAdd(key, KeyState.Unpressed);

            engine.RenderPipeline.Members.Add(this);
        }
        public void Update(RenderWindow window)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                Keyboard.Key key = Keys.Keys.ElementAt(i);
                bool isPressed = Keyboard.IsKeyPressed(key);

                if (isPressed)
                {
                    switch (Keys[key])
                    {
                        case KeyState.Unpressed | KeyState.Up:
                            Keys[key] = KeyState.Down;
                            break;
                        case KeyState.Down:
                            Keys[key] = KeyState.Pressed;
                            break;
                    }
                }
                else
                {
                    switch (Keys[key])
                    {
                        case KeyState.Pressed | KeyState.Down:
                            Keys[key] = KeyState.Up;
                            break;
                        case KeyState.Up:
                            Keys[key] = KeyState.Unpressed;
                            break;
                    }
                }
            }

            MousePosition = (Vector2)Mouse.GetPosition(window);

            Vector2 v = MousePosition - new Vector2(Screen.Width / 2f, Screen.Height / 2f);
            v.y = -v.y;
            MouseWorldPosition = v;
        }

        #region Static

        #region Functions

        public static bool GetKeyDown(Keyboard.Key key) => active.Keys[key] == KeyState.Down;
        public static bool GetKey(Keyboard.Key key) => active.Keys[key] == KeyState.Pressed;
        public static bool GetKeyUp(Keyboard.Key key) => active.Keys[key] == KeyState.Up;

        #endregion

        #region Fields

        public static Vector2 MousePosition { get; private set; }
        public static Vector2 MouseWorldPosition { get; private set; }

        #endregion

        #endregion
    }
}
