namespace Engine
{
    public class Physics
    {
        /// <summary>
        /// Physics ticks per second
        /// </summary>
        public const int TPS = 50;
        public static Physics active { get; private set; }

        #region Fields

        public List<IPhysics> Members { get; private set; } = new List<IPhysics>();
        public List<ICollider> Colliders { get; private set; } = new List<ICollider>();

        #endregion

        internal Physics()
        {
            active = this;

            Thread thread = new Thread(() => { Main(); });
            thread.Start();
        }

        /// <summary>
        /// Main physics process
        /// </summary>
        private void Main()
        {
            while (true)
            {
                for (int i = 0; i < Members.Count; i++)
                {
                    if (Members[i] is ICollider)
                        for (int j = 0; j < Colliders.Count; j++)
                            if (Members[i] != Colliders[j])
                                ((ICollider)Members[i]).ResolveCollision(Colliders[j]);

                    Members[i].PhysicsUpdate();
                }

                Thread.Sleep(1000 / TPS);
            }
        }

        #region Functions

        public static bool Raycast(Ray ray,
                                   float maxDistance,
                                   out ICollider collider,
                                   out Vector2 hit,
                                   out float distance)
        {
            collider = null;
            hit = Vector2.zero;
            distance = float.MaxValue;

            for (int i = 0; i < active.Colliders.Count; i++)
            {
                if (active.Colliders[i].ComputeRaycast(ray, maxDistance, out Vector2 _hit, out float sqrDistance))
                {
                    if (sqrDistance < distance)
                    {
                        hit = _hit;
                        distance = sqrDistance;
                        collider = active.Colliders[i];
                    }
                }
            }

            distance = MathF.Sqrt(distance);
            return collider != null;
        }

        #endregion
    }

    public interface IPhysics
    {
        public void PhysicsUpdate();
    }
}
