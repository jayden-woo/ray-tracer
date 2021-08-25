using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Plane : SceneEntity
    {
        private Vector3 center;
        private Vector3 normal;
        private Material material;

        /// <summary>
        /// Construct an infinite plane object.
        /// </summary>
        /// <param name="center">Position of the center of the plane</param>
        /// <param name="normal">Direction that the plane faces</param>
        /// <param name="material">Material assigned to the plane</param>
        public Plane(Vector3 center, Vector3 normal, Material material)
        {
            this.center = center;
            this.normal = normal.Normalized();
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the plane, and if so, return hit data.
        /// Ref: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Check if the plane is close to being parallel to the ray
            double denominator = ray.Direction.Dot(this.normal);
            if (Math.Abs(denominator) > Double.Epsilon)
            {
                // Check if the ray intersects the plane behind the camera's origin
                double t = (this.center - ray.Origin).Dot(this.normal) / denominator;
                if (t >= 0) {
                    // Find and return the hit data for the intersection
                    Vector3 position = ray.Origin + (t * ray.Direction);
                    return new RayHit(position, this.normal, ray.Direction, this.material);
                }
            }
            // The plane is parallel to the ray or the intersection point is behind the camera
            return null;
        }

        /// <summary>
        /// The material of the plane.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
