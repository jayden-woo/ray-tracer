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
        // Offset for the ray projection from its original point to avoid premature hit with the surface itself (i.e. 1e4)
        public const double Bias = 0.00001;
        // The maximum recursion depth for a reflective material
        public const int RayDepth = 10;
        // The sample size for depth of field rendering around the given aperture radius
        public const int DepthOfFieldSample = 50;

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
            // Create a new camera with the specified settings for this scene
            Camera camera = new Camera(this.options, outputImage);

            // Loop through all the pixels in the image
            for (int y = 0; y < outputImage.Height; y++)
            {
                for (int x = 0; x < outputImage.Width; x++)
                {
                    // Reset the color value for the pixel
                    Ray ray;
                    Color color = new Color(0, 0, 0);
                    // Loop through all the sub-pixels according to the AAMultiplier
                    for (int i = 0; i < this.options.AAMultiplier; i++)
                    {
                        for (int j = 0; j < this.options.AAMultiplier; j++)
                        {
                            // Get the ray for this pixel/sub-pixel
                            ray = camera.GetRay(x, y, i, j);
                            // Check if the aperture radius and focal length is modified
                            if (options.ApertureRadius == 0 && options.FocalLength == 1)
                            {
                                // Find the color for the pixel and add it to the total color value
                                color += CastRay(ray.Origin, ray.Direction, 1);
                                // Exit early as depth of field sampling is not needed anymore
                                continue;
                            }
                            // Find the focal point of from the origin that is exactly a focal length away from the camera origin
                            Vector3 origin = ray.Origin;
                            Vector3 focalPoint = ray.Origin + ray.Direction * Math.Max(0, options.FocalLength);
                            // Randomly sample around the aperture radius of the origin of the camera
                            Color colorDOF = new Color(0, 0, 0);
                            for (int n = 0; n < DepthOfFieldSample; n++)
                            {
                                // Get a random new ray with a new origin and the direction of the
                                // new origin from the focal point previously calculated
                                ray = camera.GetRay(origin, focalPoint);
                                // Cast a new ray and find the new color for the pixel from this
                                // new origin and direction and add the color up with previous samples
                                colorDOF += CastRay(ray.Origin, ray.Direction, 1);
                            }
                            // Average out the color of all the depth of field samples by dividing the total
                            // sum of colours against the sample size and add it to the color for the pixel
                            color += colorDOF / DepthOfFieldSample;
                        }
                    }
                    // Set the pixel to the final averaged color
                    color /= this.options.AAMultiplier * this.options.AAMultiplier;
                    outputImage.SetPixel(x, y, color);
                }
                // TODO: Remove rendering progress logging
                if (y % 20 == 0) Console.WriteLine($"Finished Rendering Y-Axis ({y:000}) ...");
                if (y == outputImage.Height - 1) Console.WriteLine($"Finished Rendering Image ...\n");
            }
        }

        /// <summary>
        /// Check if a ray is intersecting with any objects in the scene and
        /// the corresponding colour for the pixel where the ray originated.
        /// </summary>
        /// <param name="origin">The start position of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="depth">The current recursion depth</param>
        /// <returns>The colour for the pixel in origin</returns>
        private Color CastRay(Vector3 origin, Vector3 direction, int depth)
        {
            // Set the default color and create a ray from the parameters
            Color color = new Color(0, 0, 0);
            Ray ray = new Ray(origin, direction);

            // Check if the maximum recursion depth is reached
            if (depth > RayDepth) return color;

            // Find the closest entity intersected with the ray
            double minDist = Double.PositiveInfinity;
            foreach (SceneEntity entity in this.entities)
            {
                // Skip this entity if the ray doesn't intersect with it
                RayHit hit = entity.Intersect(ray);
                if (hit == null) continue;

                // Skip this entity if it is back-facing and not of refractive material
                if (hit.Normal.Dot(ray.Direction) > 0 && hit.Material.Type != Material.MaterialType.Refractive) continue;

                // Skip this entity if the intersection point is further than previous intersection points
                double dist = (hit.Position - ray.Origin).LengthSq();
                if (dist > minDist) continue;
                // Update the minimum distance to the distance of current entity
                minDist = dist;

                // Check the type of material of the entity's surface
                switch (hit.Material.Type)
                {
                    case Material.MaterialType.Diffuse:
                        // Find the colour of the entity after illuminated by all the lights in the scene
                        color = new Color(0, 0, 0);
                        foreach (PointLight light in this.lights)
                        {
                            // Find the direction and strength of the light source
                            Vector3 lightDirection = (light.Position - hit.Position).Normalized();

                            // Check if a shadow will be casted onto the position for the current light source
                            double lightStrength = 0;
                            if (!CastShadow(hit, light.Position, lightDirection))
                            {
                                // Truncate the strength if > 90 degrees to prevent colour underflow
                                lightStrength = Math.Max(0, hit.Normal.Dot(lightDirection));
                            }
                            // Scale the colour according to the light strength and colour
                            color += (hit.Material.Color * light.Color * lightStrength);
                        }
                        break;
                    case Material.MaterialType.Reflective:
                        // Find the origin with small offset to prevent premature intersection
                        Vector3 reflectOrigin = hit.Position + (Bias * hit.Normal);
                        // Recursively find the colour with the reflected ray as origin
                        color = CastRay(reflectOrigin, hit.Reflection, depth + 1);
                        break;
                    case Material.MaterialType.Refractive:
                        // Get the ratio of reflected light
                        double kr = hit.Fresnel;

                        // Calculate the direction and offset for each new origin point
                        bool outside = ray.Direction.Dot(hit.Normal) < 0;
                        Vector3 offset = Bias * hit.Normal;

                        // Check if the ray is totally internal reflected
                        Color refractColor = new Color(0, 0, 0);
                        if (kr < 1)
                        {
                            // Find the refraction origin with a small offset
                            Vector3 refractOrigin = outside ? hit.Position - offset : hit.Position + offset;
                            // Recursively find the refraction colour with the refraction ray as origin
                            refractColor = CastRay(refractOrigin, hit.Refraction, depth + 1);
                        }
                        // Find the reflection origin with a small offset
                        reflectOrigin = outside ? hit.Position + offset : hit.Position - offset;
                        // Recursively find the reflection colour with the reflection ray as origin
                        Color reflectColor = CastRay(reflectOrigin, hit.Reflection, depth + 1);

                        // Calculate the final colour according to the ratio of each colour
                        color = reflectColor * kr + refractColor * (1 - kr);
                        break;
                }
            }
            // Return the calculated colour value for the origin of the ray
            return color;
        }

        /// <summary>
        /// Check if a hit point is in fact in a shadow for the current
        /// light source.
        /// </summary>
        /// <param name="point">The hit data of the hit point</param>
        /// <param name="lightPosition">The position of the light source</param>
        /// <param name="lightDirection">This direction of the light source from the hit point</param>
        /// <returns>True if the hit point is a shadow and false otherwise</returns>
        private bool CastShadow(RayHit point, Vector3 lightPosition, Vector3 lightDirection)
        {
            // Create a shadow ray from origin to the light source
            Vector3 origin = point.Position + (Bias * point.Normal);
            Ray ray = new Ray(origin, lightDirection);
            foreach (SceneEntity entity in this.entities)
            {
                // Check if the shadow ray intersects with any entity on the way
                RayHit hit = entity.Intersect(ray);
                if (hit != null)
                {
                    // Check if the intersection occurred behind the light source
                    double zo = point.Position.Z;
                    double zh = hit.Position.Z;
                    double zl = lightPosition.Z;
                    if ((zo <= zh && zh <= zl) || (zo >= zh && zh >= zl)) return true;
                }
            }
            // The point doesn't have any shadow casted on it
            return false;
        }

    }
}
