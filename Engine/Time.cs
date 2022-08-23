namespace Engine
{
    public static class Time
    {
        /// <summary>
        /// The interval in seconds from the last frame to the current one.
        /// </summary>
        public static float DeltaTime => UnscaleDeltaTime * TimeScale;
        public static float UnscaleDeltaTime { get; internal set; } = 1f;
        public static float TimeScale { get; set; } = 1f;
        public static float UnscalePhysicsDeltaTime { get; set; } = 1f / Physics.TPS;
        public static float PhysicsDeltaTime { get; set; } = 1f / Physics.TPS * TimeScale;
        /// <summary>
        /// The number of frames per second based on the delay between frames.
        /// </summary>
        public static int FPS => (int)(1f / UnscaleDeltaTime);
        /// <summary>
        /// The total elapsed time since the application was launched, in seconds.
        /// </summary>
        public static float Elapsed { get; internal set; } = 0f;
    }
}
