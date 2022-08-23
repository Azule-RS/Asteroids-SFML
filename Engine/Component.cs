namespace Engine
{
    public abstract class Component : IUpdate, IPhysics
    {
        public readonly Node Node;

        public Component(Node node)
        {
            Node = node;
            GameEngine.active.RenderPipeline.Members.Add(this);

            GameEngine.active.Physics.Members.Add(this);
            if (this is ICollider)
                GameEngine.active.Physics.Colliders.Add((ICollider)this);
        }
        public void Destroy()
        {
            GameEngine.active.RenderPipeline.Members.Remove(this);

            GameEngine.active.Physics.Members.Remove(this);
            if (this is ICollider)
                GameEngine.active.Physics.Colliders.Remove((ICollider)this);

            Node.Components.Remove(this);
        }

        public virtual void Update(SFML.Graphics.RenderWindow window) { }
        public virtual void PhysicsUpdate() { }
        public virtual void OnCollisionEnter(ICollider other, Vector2 reaction) { }
        public virtual void OnDestroy() { }
    }
}
