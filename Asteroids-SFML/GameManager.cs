using Engine;
using SFML.Graphics;

namespace Asteroids
{
    public class GameManager : Component
    {
        public const float c_SafeZone = 100;

        public static GameManager active { get; private set; }

        #region Fields

        public int Score = 0;

        public Player Player;
        public List<Node> LoopNodes = new List<Node>();
        public List<Asteroid> Asteroids = new List<Asteroid>();

        #region UI

        private Font m_Font;

        private Text MainStatsText = new Text();
        private Text ScoreText = new Text();
        private Text OverheatText = new Text();

        #endregion

        #endregion

        public GameManager(Node node) : base(node)
        {
            active = this;

            CreateUI();
        }
        private void CreateUI()
        {
            m_Font = new Font("Assets/Font.ttf");

            #region Main stats

            MainStatsText.Font = m_Font;
            MainStatsText.FillColor = Color.White;
            MainStatsText.CharacterSize = 20;

            #endregion

            #region Score

            ScoreText.Font = m_Font;
            ScoreText.FillColor = Color.White;
            ScoreText.CharacterSize = 20;

            #endregion

            #region Overheat

            OverheatText.Font = m_Font;
            OverheatText.FillColor = Color.White;
            OverheatText.CharacterSize = 100;
            OverheatText.DisplayedString = "Overheat";
            OverheatText.Style = Text.Styles.Italic;
            OverheatText.Position = new SFML.System.Vector2f(Screen.Width / 2 - OverheatText.FindCharacterPos((uint)(OverheatText.DisplayedString.Length / 2)).X, Screen.Height - 200);

            #endregion
        }

        public override void Update(RenderWindow window)
        {
            #region Draw main stats

            MainStatsText.DisplayedString = $"Health: {Player.Health.ToString("N2")}%\n" +
                $"Beam temperature: {Player.BeamTemperature.ToString("N2")}°C" +
                $"\n\nTarget health: {(Player.LastAsteroid != null ? Player.LastAsteroid.Health : "Unknown")}";
            window.Draw(MainStatsText);

            #endregion

            #region Draw score

            ScoreText.DisplayedString = $"Score: {Score.ToString("000")}";
            ScoreText.Position = new SFML.System.Vector2f(Screen.Width / 2 - ScoreText.FindCharacterPos((uint)(ScoreText.DisplayedString.Length / 2)).X, 0);
            window.Draw(ScoreText);

            #endregion

            #region Draw overheat

            if (Player.BeamTemperature > 1000)
            {
                Color color = OverheatText.FillColor;
                color.A = (byte)(Math.Abs(Math.Cos(Time.Elapsed * 10)) * 255);
                OverheatText.FillColor = color;
                window.Draw(OverheatText);
            }

            #endregion
        }
        public override void PhysicsUpdate()
        {
            foreach (Node node in LoopNodes)
            {
                if (node.Position.x > Screen.Width / 2 + c_SafeZone)
                    node.Position = new Vector2(-node.Position.x + 10, node.Position.y);
                else
                    if (node.Position.x < -Screen.Width / 2 - c_SafeZone)
                    node.Position = new Vector2(-node.Position.x - 10, node.Position.y);

                if (node.Position.y > Screen.Height / 2 + c_SafeZone)
                    node.Position = new Vector2(node.Position.x, -node.Position.y + 10);
                else
                    if (node.Position.y < -Screen.Height / 2 - c_SafeZone)
                    node.Position = new Vector2(node.Position.x, -node.Position.y - 10);
            }
        }

        public void GameOver()
        {
            Debug.Log("Game over! Score: " + Score);

            Score = 0;

            foreach (Asteroid asteroid in Asteroids)
                asteroid.Regenerate();
        }
    }
}