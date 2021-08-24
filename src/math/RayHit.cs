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

                // Calculate the normalized direction of the reflected ray
                return (this.incident - 2 * this.incident.Dot(this.normal) * this.normal).Normalized();
            }
        }

        /// <summary>
        /// The direction of the refraction (transmitted) ray.
        /// </summary>
        public Vector3 Refraction { get
            {
                // Ref: https://www.scratchapixel.com/lessons/3d-basic-rendering/
                // introduction-to-shading/reflection-refraction-fresnel

                double cosi = Math.Clamp(this.incident.Dot(this.normal), -1, 1);
                double etai = 1, etat = this.material.RefractiveIndex;
                Vector3 normal;

                // Outside of the surface and entering the object
                if (cosi < 0)
                {
                    // Flip the sign of cos(theta) in the equation
                    cosi = -cosi;
                    // Assign the normal vector as it is already in  the right direction
                    normal = this.normal;
                }
                // Inside the surface and exiting the object
                else
                {
                    // Swap the refraction index of the two mediums
                    double temp = etai;
                    etai = etat;
                    etat = temp;
                    // Reverse the direction of normal before assigning
                    normal = -this.normal;
                }

                // Calculate ratio of the two refraction indexes
                double eta = etai / etat;
                double k = 1 - eta * eta * (1 - cosi * cosi);

                // Check if total internal reflection occurs (incident angle >= critical angle)
                if (k < 0)
                {
                    // TODO: update direction for total internal reflection
                    return new Vector3(0, 0, 0);
                }

                // Calculate the normalized direction of the transmitted ray
                return (eta * this.incident + (eta * cosi - Math.Sqrt(k)) * normal).Normalized();
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
