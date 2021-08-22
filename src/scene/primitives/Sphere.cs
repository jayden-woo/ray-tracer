using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a sphere in a scene represented by its center point and radius.
    /// </summary>
    public class Sphere : SceneEntity
    {
        private Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Ref: https://www.scratchapixel.com/lessons/3d-basic-rendering/
            // minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection

            // Check if the ray intersects the sphere behind the ray's origin
            Vector3 L = this.center - ray.Origin;
            double tc = L.Dot(ray.Direction);
            if (tc < 0) return null;

            // Check if the ray intersects with the sphere surface
            double d2 = (L.Dot(L)) - (tc * tc);
            double radius2 = this.radius * this.radius;
            if (d2 > radius2) return null;

            // Find the two intersection points and make sure they're in order
            double thc = Math.Sqrt(radius2 - d2);
            double[] ts = new double[] {tc - thc, tc + thc};
            Array.Sort(ts);

            // Return only the first positive intersection point
            foreach (double t in ts)
            {
                if (t > 0)
                {
                    // Find and return the hit data for the intersection
                    Vector3 position = ray.Origin + t * ray.Direction;
                    Vector3 normal = position - this.center;
                    return new RayHit(position, normal, ray.Direction, this.material);
                }
            }
            // None of the intersection points are infront of the camera
            return null;
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
