using Engine;
using SFML.Graphics;

namespace Asteroids
{
    public class Player : Component
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
                if (m_Health < 0)
                    Died();
            }
        }
        private float m_Health = 100;

        public float BeamTemperature;

        private Vector2 Input = Vector2.zero;

        private ICollider? m_LastCollider;
        public Asteroid? LastAsteroid;

        private float m_AimAngle;
        private float m_DamageCooldown;

        private Random m_Random = new Random();

        #endregion

        public Player(Node node) : base(node)
        {
            var collider = Node.AddComponent<PolygonCollider>();
            collider.Shape.Points = new[] { new Vector2(-25, -35), new Vector2(0, 25), new Vector2(25, -35), new Vector2(0, -25) };
        }

        public override void Update(RenderWindow window)
        {
            #region Input

            Input.x = Engine.Input.GetKey(SFML.Window.Keyboard.Key.A) ? -1 : (Engine.Input.GetKey(SFML.Window.Keyboard.Key.D) ? 1 : 0);
            Input.y = Engine.Input.GetKey(SFML.Window.Keyboard.Key.S) ? -1 : (Engine.Input.GetKey(SFML.Window.Keyboard.Key.W) ? 1 : 0);

            #endregion

            if (m_DamageCooldown > 0)
                m_DamageCooldown -= Time.DeltaTime;

            #region Beam

            if (Engine.Input.GetKeyUp(SFML.Window.Keyboard.Key.Space))
            {
                LastAsteroid = null;
                m_LastCollider = null;
            }

            if (Engine.Input.GetKey(SFML.Window.Keyboard.Key.Space))
            {
                #region Beam overheat

                BeamTemperature += Math.Max(Time.DeltaTime * BeamTemperature, 0.1f);

                if (BeamTemperature > 1000f)
                    Health -= BeamTemperature / 100f * Time.DeltaTime;

                #endregion

                #region Beam Raycast & Graphic

                Ray ray = new Ray(Node.Position + Node.Up * 25.5f, Node.Up);
                if (Physics.Raycast(ray, 200, out ICollider collider, out Vector2 hit, out float distance))
                {
                    window.Draw(new[] { new Vertex(ray.Origin.toWindow(), Color.White), new Vertex(hit.toWindow(), Color.White) }, PrimitiveType.Lines);
                    Node node = ((Component)collider).Node;

                    if (m_LastCollider != collider)
                    {
                        if (node.Tags[0] == "Asteroid")
                            LastAsteroid = node.GetComponent<Asteroid>();
                    }
                    m_LastCollider = collider;
                }
                else
                {
                    m_LastCollider = null;
                    LastAsteroid = null;
                    window.Draw(new[] { new Vertex(ray.Origin.toWindow(), Color.White), new Vertex(ray.GetPoint(200).toWindow(), Color.Black) }, PrimitiveType.Lines);
                }

                #endregion

                if (LastAsteroid != null)
                {
                    LastAsteroid.Health -= Time.DeltaTime * 30 * (distance / 200);

                    #region Aim

                    m_AimAngle += Time.DeltaTime * 30;

                    Vector2 leftUpper = Vector2.Rotate(new Vector2(-0.5f, 0.5f) * LastAsteroid.Radius * 3f, m_AimAngle) + LastAsteroid.Node.Position;
                    Vector2 rightUpper = Vector2.Rotate(new Vector2(0.5f, 0.5f) * LastAsteroid.Radius * 3f, m_AimAngle) + LastAsteroid.Node.Position;
                    Vector2 leftLower = Vector2.Rotate(new Vector2(-0.5f, -0.5f) * LastAsteroid.Radius * 3f, m_AimAngle) + LastAsteroid.Node.Position;
                    Vector2 rightLower = Vector2.Rotate(new Vector2(0.5f, -0.5f) * LastAsteroid.Radius * 3f, m_AimAngle) + LastAsteroid.Node.Position;

                    window.Draw(new[] { new Vertex(leftUpper.toWindow(), Color.White), new Vertex(rightUpper.toWindow(), Color.White), new Vertex(rightLower.toWindow(), Color.White), new Vertex(leftLower.toWindow(), Color.White), new Vertex(leftUpper.toWindow(), Color.White) }, PrimitiveType.LineStrip);

                    #endregion
                }
            }
            else
            {
                //Beam cooling
                BeamTemperature = MathLib.Lerp(BeamTemperature, 0, Time.DeltaTime);
            }

            #endregion
        }
        public override void PhysicsUpdate()
        {
            #region Control

            Node.AngularVelocity += Input.x * -100 * Time.PhysicsDeltaTime;
            Node.AddForce(Node.Up * Input.y * 150 * Time.PhysicsDeltaTime);

            #endregion

            if (LastAsteroid != null)
                LastAsteroid.Node.Velocity += Vector2.Rotate(Vector2.up * m_Random.Next(1, 20), m_Random.Next(0, 360));
        }

        public override void OnCollisionEnter(ICollider other, Vector2 reaction)
        {
            float n = reaction.Length;

            //Collision damage
            if (n > 10 && m_DamageCooldown <= 0)
            {
                Health -= n / 50;
                m_DamageCooldown = Math.Clamp(0.1f * n, 0, 2f);
            }
        }

        public void Died()
        {
            GameManager.active.GameOver();
            Health = 100;
            BeamTemperature = 0;

            Node.Velocity = Vector2.zero;
            Node.AngularVelocity = 0;

            Node.Position = Vector2.zero;
            Node.Rotation = 0;
        }
    }
}