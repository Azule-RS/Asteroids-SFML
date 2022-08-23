namespace Engine
{
    public interface ICollider
    {
        /// <summary>
        /// Used to calculate collision.
        /// </summary>
        /// <param name="other"></param>
        public void ResolveCollision(ICollider other);
        /// <summary>
        /// Used to calculate raycast.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="maxDistance"></param>
        /// <param name="hit"></param>
        /// <param name="sqrDistance"></param>
        /// <returns></returns>
        public bool ComputeRaycast(Ray ray, float maxDistance, out Vector2 hit, out float sqrDistance);
    }
}
