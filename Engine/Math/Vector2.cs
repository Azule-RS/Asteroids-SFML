using SFML.System;

namespace Engine
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2(Vector2f position)
        {
            x = position.X;
            y = position.Y;
        }

        public float Length
        {
            get
            {
                return MathF.Sqrt(x * x + y * y);
            }
        }
        public float sqrLength
        {
            get
            {
                return x * x + y * y;
            }
        }

        public Vector2 Normalized
        {
            get
            {
                float length = Length;
                if (length == 0)
                    return zero;
                return new Vector2(x / length, y / length);
            }
        }
        public void Normalize()
        {
            float length = Length;

            if (length == 0)
            {
                x = 0;
                y = 0;
            }

            x = x / length;
            y = y / length;
        }

        public Vector2f toWindow()
        {
            return new Vector2f(x + Screen.Width / 2f, -y + Screen.Height / 2f);
        }

        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            if (lhs.Length * rhs.Length == 0 || float.IsNaN(lhs.Length * rhs.Length))
                return 0;

            return (lhs.x * rhs.x + lhs.y * rhs.y) / (lhs.Length * rhs.Length);
        }
        public static Vector2 Lerp(Vector2 from, Vector2 to, float Time)
        {
            return from + Time * (to - from);
        }

        #region Angle

        public static float Angle(Vector2 from, Vector2 to)
        {
            float denominator = (float)Math.Sqrt(from.sqrLength * to.sqrLength);
            if (denominator < 1e-15f)
                return 0f;

            float dot = Math.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return (float)Math.Acos(dot) * MathLib.Rad2Deg;
        }
        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            float unsigned_angle = Angle(from, to);
            float sign = Math.Sign(from.x * to.y - from.y * to.x);
            return unsigned_angle * sign;
        }

        #endregion

        #region Rotate

        public static Vector2 Rotate(Vector2 vector, float angle)
        {
            float sin = MathF.Sin(angle * MathLib.Deg2Rad);
            float cos = MathF.Cos(angle * MathLib.Deg2Rad);

            return new Vector2(
            cos * vector.x - sin * vector.y,
            sin * vector.x + cos * vector.y
            );
        }
        public static Vector2 Rotate(Vector2 vector, Vector2 origin, float angle)
        {
            float sin = MathF.Sin(angle * MathLib.Deg2Rad);
            float cos = MathF.Cos(angle * MathLib.Deg2Rad);

            return new Vector2(
                cos * (vector.x - origin.x) - sin * (vector.y - origin.y) + origin.x,
                sin * (vector.x - origin.x) + cos * (vector.y - origin.y) + origin.y
                );
        }

        #endregion

        #region Presets

        public static Vector2 one
        {
            get
            {
                return new Vector2(1, 1);
            }
        }
        public static Vector2 zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }
        public static Vector2 up
        {
            get
            {
                return new Vector2(0, 1);
            }
        }
        public static Vector2 right
        {
            get
            {
                return new Vector2(1, 0);
            }
        }

        #endregion

        public static implicit operator Vector2f(Vector2 v)
        {
            return new Vector2f(v.x, v.y);
        }

        public static explicit operator Vector2(Vector2i v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static explicit operator Vector2(Vector2f v)
        {
            return new Vector2(v.X, v.Y);
        }

        #region Operations
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }
        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }
        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x * b, a.y * b);
        }
        public static Vector2 operator *(float b, Vector2 a)
        {
            return new Vector2(a.x * b, a.y * b);
        }
        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        #endregion
        public override string ToString() => $"({x}, {y})";
    }
}
