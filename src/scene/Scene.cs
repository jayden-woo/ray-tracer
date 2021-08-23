using System;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        // Horizontal field-of-view of the camera in degrees
        public const double FieldOfView = 60.0;

        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            // Find the aspect ratio and the FOV scaling factor of the image
            double aspectRatio = (double) outputImage.Width / outputImage.Height;
            double scale = Math.Tan((FieldOfView / 180 * Math.PI) / 2);

            // Loop through all the pixels in the image
            for (int y = 0; y < outputImage.Height; y++)
            {
                for (int x = 0; x < outputImage.Width; x++)
                {
                    // Find the pixel coordinates on the screen first
                    double x_pos = (x + 0.5) / outputImage.Width;
                    double y_pos = (y + 0.5) / outputImage.Height;
                    double z_pos = 1.0;

                    // Scale the x and y coordinates by the appropriate factors
                    x_pos = ((x_pos * 2) - 1) * scale;
                    y_pos = (1 - (y_pos * 2)) * scale / aspectRatio;

                    // Create a ray for this pixel
                    Vector3 origin = new Vector3(0, 0, 0);
                    Vector3 direction = new Vector3(x_pos, y_pos, z_pos) - origin;
                    Ray ray = new Ray(origin, direction.Normalized());

                    // Find the closest entity intersected with the ray
                    double minZ = Double.PositiveInfinity;
                    foreach (SceneEntity entity in this.entities)
                    {
                        // Check if the ray intersects with this entity
                        RayHit hit = entity.Intersect(ray);
                        if (hit != null)
                        {
                            // Check if the Z value of the intersection is closer
                            if (hit.Position.Z < minZ)
                            {
                                // Update the Z value to the closest intersected entity
                                minZ = hit.Position.Z;
                                // Find the colour of the entity after illuminated by all the lights in the scene
                                Color color = new Color(0, 0, 0);
                                foreach (PointLight light in this.lights)
                                {
                                    // Find the direction and strength of the light source
                                    Vector3 lightDirection = (light.Position - hit.Position).Normalized();
                                    // Truncate the strength if > 90 degrees to prevent colour underflow
                                    double strength = Math.Max(0, hit.Normal.Dot(lightDirection));
                                    // Scale the colour according to the light strength and colour
                                    color = color + (hit.Material.Color * light.Color * strength);
                                }
                                // Set the pixel to the final calculated colour
                                outputImage.SetPixel(x, y, color);

                            }
                        }
                    }
                }
            }
        }

    }
}
