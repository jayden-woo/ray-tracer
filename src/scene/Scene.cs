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
            double aspectRatio = outputImage.Width / outputImage.Height;
            double scaleFOV = Math.Tan((FieldOfView / 180 * Math.PI) / 2);

            // Loop through all the pixels in the image
            for (int y = 0; y < outputImage.Height; y++)
            {
                for (int x = 0; x < outputImage.Width; x++)
                {
                    // Find the pixel coordinates on the screen first
                    double x_pos = (x + 0.5) / outputImage.Width;
                    double y_pos = (y + 0.5) / outputImage.Height;
                    double z_pos = 1.0;

                    // Scale the coordinates to a range between -1 and 1
                    x_pos = (x_pos * 2) - 1;
                    y_pos = 1 - (y_pos * 2);

                    // Scale the coordinates by the appropriate factors next
                    x_pos = x_pos * scaleFOV;
                    y_pos = y_pos * scaleFOV / aspectRatio;

                    // Create the ray for this pixel
                    Vector3 origin = new Vector3(0, 0, 0);
                    Vector3 direction = new Vector3(x_pos, y_pos, z_pos);
                    Ray ray = new Ray(origin, direction);

                    // Set each pixel to white colour
                    outputImage.SetPixel(x, y, new Color(1, 1, 1));
                }
            }
        }

    }
}
