using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Ref: https://www.scratchapixel.com/lessons/3d-basic-rendering/
            // ray-tracing-rendering-a-triangle/moller-trumbore-ray-triangle-intersection

            // Find the values used in the Moller-Trumbore algorithm
            Vector3 v0v1 = this.v1 - this.v0;
            Vector3 v0v2 = this.v2 - this.v0;
            Vector3 pvec = ray.Direction.Cross(v0v2);
            double det = v0v1.Dot(pvec);

            // With back-face culling
            // Check if the triangle is front-facing (det > 0) or back-facing (det < 0)
            // or almost parallel to the ray (det close to 0)
            // if (det < Double.Epsilon) return null;

            // Without back-face culling
            // Check if the triangle is almost parallel to the ray
            if (Math.Abs(det) < Double.Epsilon) return null;

            // Find the inverse det value
            double invDet = 1 / det;

            // Check if u is within the range of [0,1]
            Vector3 tvec = ray.Origin - this.v0;
            double u = tvec.Dot(pvec) * invDet;
            if (u < 0 || u > 1) return null;

            // Check if v is within the range of [0,1]
            Vector3 qvec = tvec.Cross(v0v1);
            double v = ray.Direction.Dot(qvec) * invDet;
            if (v < 0 || (u + v) > 1) return null;

            // Find the position of the intersection point with the triangle
            double t = v0v2.Dot(qvec) * invDet;

            // Check if the ray intersects the triangle behind the camera's origin
            if (t >= 0) {
                // Find and return the hit data for the intersection
                Vector3 position = ray.Origin + (t * ray.Direction);
                Vector3 normal = v0v1.Cross(v0v2);
                return new RayHit(position, normal.Normalized(), ray.Direction, this.material);
            }
            // The intersection point is behind the camera
            return null;
        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
