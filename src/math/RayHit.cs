using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent ray hit data, including the position and
    /// normal of a hit (and optionally other computed vectors).
    /// </summary>
    public class RayHit
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector3 incident;
        private Material material;

        /// <summary>
        /// Construct a new ray hit data.
        /// </summary>
        /// <param name="position">The position of the intersection point</param>
        /// <param name="normal">The normal of the intersected surface</param>
        /// <param name="incident">The direction of the incident ray</param>
        /// <param name="material">The material of the intersected surface</param>
        public RayHit(Vector3 position, Vector3 normal, Vector3 incident, Material material)
        {
            this.position = position;
            this.normal = normal;
            this.incident = incident;
            this.material = material;
        }

        // You may wish to write methods to compute other vectors,
        // e.g. reflection, transmission, etc

        /// <summary>
        /// The direction of the reflection ray.
        /// </summary>
        public Vector3 Reflection { get
            {
                // Ref: https://www.scratchapixel.com/lessons/3d-basic-rendering/
                // introduction-to-shading/reflection-refraction-fresnel

                // Calculate the direction of the reflected ray
                return (this.incident - 2 * this.incident.Dot(this.normal) * this.normal).Normalized();
            }
        }

        /// <summary>
        /// The position of the intersection point.
        /// </summary>
        public Vector3 Position { get { return this.position; } }

        /// <summary>
        /// The normal of the intersected surface.
        /// </summary>
        public Vector3 Normal { get { return this.normal; } }

        /// <summary>
        /// The direction of the incident ray.
        /// </summary>
        public Vector3 Incident { get { return this.incident; } }

        /// <summary>
        /// The material of the intersected surface.
        /// </summary>
        public Material Material { get { return this.material; } }
    }
}
