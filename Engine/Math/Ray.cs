using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Ray
    {
        public Vector2 Origin;
        public Vector2 Direction;

        public Ray(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = direction.Normalized;
        }

        public Vector2 GetPoint(float distance)
        {
            return Origin + (Direction * distance);
        }
    }
}
