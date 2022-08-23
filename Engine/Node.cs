using SFML.Graphics;

namespace Engine
{
    public class Node : IUpdate, IPhysics
    {
        #region Static

        internal static List<Node> s_Nodes = new List<Node>();

        #endregion

        #region Fields

        public string Name { get; set; }
        public string[] Tags { get; set; } = new string[8];

        #endregion

        public Node(string name)
        {
            Name = name;
            s_Nodes.Add(this);

            GameEngine.active.Physics.Members.Add(this);
            GameEngine.active.RenderPipeline.Members.Add(this);
        }
        public void Destroy()
        {
            GameEngine.active.Physics.Members.Remove(this);
            GameEngine.active.RenderPipeline.Members.Remove(this);
            s_Nodes.Clear();

            int componentsCount = Components.Count;
            for (int i = 0; i < componentsCount; i++)
                Components[0].Destroy();

            Components.Clear();
        }

        #region Elements

        #region Transform

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Vector2 TransformPoint(Vector2 localPoint)
        {
            return Position + Vector2.Rotate(localPoint, Rotation);
        }
        public Vector2 TranformVector(Vector2 vector)
        {
            return Vector2.Rotate(vector, Rotation);
        }

        public Vector2 Up
        {
            get
            {
                return TranformVector(Vector2.up);
            }
        }
        public Vector2 Right
        {
            get
            {
                return TranformVector(Vector2.right);
            }
        }

        #endregion

        #region Rigidbody

        public float Mass { get; set; } = 1;
        public Vector2 Velocity { get; set; } = Vector2.zero;
        public float AngularVelocity { get; set; } = 0;

        public void AddForce(Vector2 force)
        {
            Velocity += force / Mass;
        }

        #endregion

        #region Components
        internal List<Component> Components { get; private set; } = new List<Component>();

        public T AddComponent<T>() where T : Component
        {
            T? t = Activator.CreateInstance(typeof(T), this) as T;

            if (t == null)
                throw new Exception();

            Components.Add(t);

            return t;
        }
        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    return (T)Components[i];

            return null;
        }

        #endregion


        #endregion

        public void Update(RenderWindow window)
        {
            Position += Velocity * Time.DeltaTime;
            Rotation += AngularVelocity * Time.DeltaTime;
        }
        public void PhysicsUpdate()
        {

        }

        public override string ToString()
        {
            return $"Node({Name})";
        }
    }
}
