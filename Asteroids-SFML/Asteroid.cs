using Engine;

namespace Asteroids
{
    public class Asteroid : Component
    {
        #region Fields

        public float Health
        {
            get
            {
                return m_Health;
            }
            set
            {
                m_Health = value;
                if (m_Health <= 0)
                    OnDied();
            }
        }
        private float m_Health;

        public float Radius;

        private Random m_Random = new Random();
        private PolygonCollider m_Collider;

        #endregion

        public Asteroid(Node node) : base(node)
        {
            m_Collider = Node.AddComponent<PolygonCollider>();
            Node.Tags[0] = "Asteroid";
            Regenerate();
        }
        public void Regenerate()
        {
            int count = m_Random.Next(8, 16);
            Vector2[] points = new Vector2[count];
            Radius = m_Random.Next(30, 100);
            for (int i = 0; i < count; i++)
            {
                float angle = 0;

                while (angle >= 355 || angle <= 0)
                    angle = i / (float)(count - 1) * 360 + m_Random.Next(-10, 10);

                points[i] = Vector2.Rotate(Vector2.up * (Radius + m_Random.Next(10, 20)), angle);
            }
            m_Collider.Shape.Points = points;

            // Select position
            switch (m_Random.Next(0, 3))
            {
                case 0:
                    Node.Position = new Vector2(-Screen.Width / 2 + GameManager.c_SafeZone / 2, m_Random.Next(-Screen.Height / 2, Screen.Height / 2));
                    break;
                case 1:
                    Node.Position = new Vector2(Screen.Width / 2 - GameManager.c_SafeZone / 2, m_Random.Next(-Screen.Height / 2, Screen.Height / 2));
                    break;
                case 2:
                    Node.Position = new Vector2(m_Random.Next(-Screen.Width / 2, Screen.Width / 2), Screen.Height / 2 - GameManager.c_SafeZone / 2);
                    break;
                case 3:
                    Node.Position = new Vector2(m_Random.Next(-Screen.Width / 2, Screen.Width / 2), -Screen.Height / 2 + GameManager.c_SafeZone / 2);
                    break;
            }

            Node.Velocity = Vector2.Rotate(Vector2.up * m_Random.Next(50, 200), m_Random.Next(0, 360));
            Node.Rotation = m_Random.Next(0, 360);

            Node.Mass = Radius * 10;
            Health = Radius;
        }
        private void OnDied()
        {
            Regenerate();
            GameManager.active.Score++;
        }
    }
}