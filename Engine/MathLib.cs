namespace Engine
{
    public static class MathLib
    {
        public const float Deg2Rad = MathF.PI * 2 / 360;
        public const float Rad2Deg = 360 / (MathF.PI * 2);

        public static float Clamp01(float value) => value < 0F ? 0F : (value > 1F ? 1F : value);
        public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);
        public static float LerpUnclamped(float a, float b, float t) => a + (b - a) * t;
    }
}
