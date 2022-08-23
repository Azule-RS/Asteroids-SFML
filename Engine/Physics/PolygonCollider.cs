using SFML.Graphics;

namespace Engine
{
    public class PolygonCollider : Component, ICollider
    {
        public Polygon Shape { private set; get; } = new Polygon(new[] { new Vector2(50, -50), new Vector2(0, 35), new Vector2(-50, -50) });

        public PolygonCollider(Node node) : base(node) { }

        public override void Update(RenderWindow window)
        {
            Vector2[] points = new Vector2[Shape.Points.Length + 1];
            Array.Copy(Shape.Points, 0, points, 0, Shape.Points.Length);
            points[points.Length - 1] = Shape.Points[0];
            window.Draw(points.Select(a => new Vertex(Node.TransformPoint(a).toWindow(), Color.White)).ToArray(), PrimitiveType.LineStrip);
        }

        #region ICollider

        #region Collision
        public void ResolveCollision(ICollider other)
        {
            if (other is PolygonCollider)
            {
                var _other = (PolygonCollider)other;

                var result = PolygonCollision(this, _other, Node.Velocity * Time.PhysicsDeltaTime);

                if (result.WillIntersect)
                {
                    Node.Velocity += result.MinimumTranslationVector;
                    Node.Position += result.MinimumTranslationVector * Math.Clamp(_other.Node.Mass / Node.Mass, 0, 1);

                    foreach (Component component in Node.Components)
                        component.OnCollisionEnter(_other, result.MinimumTranslationVector * _other.Node.Mass);

                    foreach (Component component in _other.Node.Components)
                        component.OnCollisionEnter(this, -result.MinimumTranslationVector * Node.Mass);
                }
            }
        }
        private static PolygonCollisionResult PolygonCollision(PolygonCollider polygonA, PolygonCollider polygonB, Vector2 velocity)
        {
            PolygonCollisionResult result = new PolygonCollisionResult();
            result.Intersect = true;
            result.WillIntersect = true;

            int edgeCountA = polygonA.Shape.Edges.Length;
            int edgeCountB = polygonB.Shape.Edges.Length;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;

            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                    edge = polygonA.Shape.Edges[edgeIndex];
                else
                    edge = polygonB.Shape.Edges[edgeIndex - edgeCountA];

                Vector2 axis = new Vector2(-edge.y, edge.x);
                axis.Normalize();

                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                if (IntervalDistance(minA, maxA, minB, maxB) > 0) result.Intersect = false;

                float velocityProjection = DotProduct(axis, velocity);

                if (velocityProjection < 0)
                    minA += velocityProjection;
                else
                    maxA += velocityProjection;

                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.WillIntersect = false;

                if (!result.Intersect && !result.WillIntersect) break;

                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = (polygonA.Shape.Center + polygonA.Node.Position) - (polygonB.Shape.Center + polygonB.Node.Position);
                    if (DotProduct(d, translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            if (result.WillIntersect) result.MinimumTranslationVector = translationAxis * minIntervalDistance;

            return result;


            float DotProduct(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

            void ProjectPolygon(Vector2 axis, PolygonCollider polygon, ref float min, ref float max)
            {
                float d = DotProduct(axis, polygon.Node.TransformPoint(polygon.Shape.Points[0]));
                min = d;
                max = d;
                for (int i = 0; i < polygon.Shape.Points.Length; i++)
                {
                    d = DotProduct(polygon.Node.TransformPoint(polygon.Shape.Points[i]), axis);
                    if (d < min)
                        min = d;
                    else
                        if (d > max)
                        max = d;
                }
            }

            float IntervalDistance(float minA, float maxA, float minB, float maxB) => minA < minB ? minB - maxA : minA - maxB;
        }
        #endregion

        #region Raycast

        public bool ComputeRaycast(Ray ray, float maxDistance, out Vector2 hit, out float sqrDistance)
        {
            sqrDistance = maxDistance * maxDistance;
            bool flag = false;
            hit = Vector2.zero;

            for (int i = 0; i < Shape.Points.Length; i++)
            {
                Vector2 a = Node.TransformPoint(Shape.Points[i]);
                Vector2 b = Node.TransformPoint(Shape.Points[i + 1 == Shape.Points.Length ? 0 : i + 1]);


                if (SegmentIntersectRay((a, b), ray, maxDistance, out Vector2 _hit))
                {
                    float _sqrDistance = (ray.Origin - _hit).sqrLength;
                    if (_sqrDistance < sqrDistance)
                    {
                        hit = _hit;
                        sqrDistance = _sqrDistance;
                        flag = true;
                    }
                }
            }

            return flag;
        }
        private bool SegmentIntersectRay((Vector2, Vector2) segment, Ray ray, float maxDistance, out Vector2 hit)
        {
            Vector2 A = segment.Item1;
            Vector2 B = segment.Item2;

            Vector2 C = ray.Origin;
            Vector2 D = ray.GetPoint(maxDistance);

            hit = new Vector2();

            float denominator = (D.x - C.x) * (B.y - A.y) - (B.x - A.x) * (D.y - C.y);

            float s = ((A.x - C.x) * (D.y - C.y) - (D.x - C.x) * (A.y - C.y)) / denominator;
            if (s < 0 || s > 1) return false;

            float r = ((B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y)) / denominator;
            if (r < 0) return false;

            hit = new Vector2(s * (B.x - A.x) + A.x, s * (B.y - A.y) + A.y);
            return true;

        }

        #endregion

        #endregion

        public class Polygon
        {
            public Vector2[] Points
            {
                set
                {
                    points = value;
                    ComputeEdges();
                }
                get
                {
                    return points;
                }
            }
            private Vector2[] points;
            public Vector2[] Edges { private set; get; }

            public Polygon(Vector2[] poins)
            {
                this.points = poins;
                ComputeEdges();
            }
            public void ComputeEdges()
            {
                Vector2 p1;
                Vector2 p2;

                Edges = new Vector2[Points.Length];

                for (int i = 0; i < Points.Length; i++)
                {
                    p1 = Points[i];
                    if (i + 1 >= Points.Length)
                        p2 = Points[0];
                    else
                        p2 = Points[i + 1];

                    Edges[i] = p2 - p1;
                }
            }

            public Vector2 Center
            {
                get
                {
                    float totalX = 0;
                    float totalY = 0;
                    for (int i = 0; i < Points.Length; i++)
                    {
                        totalX += Points[i].x;
                        totalY += Points[i].y;
                    }

                    return new Vector2(totalX / (float)Points.Length, totalY / (float)Points.Length);
                }
            }
        }
        public struct PolygonCollisionResult
        {
            // Are the polygons going to intersect forward in time?
            public bool WillIntersect;
            // Are the polygons currently intersecting?
            public bool Intersect;
            // The translation to apply to the first polygon to push the polygons apart.
            public Vector2 MinimumTranslationVector;
        }
    }
}
