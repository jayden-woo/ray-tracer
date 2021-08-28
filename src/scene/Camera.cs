using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent the camera capturing the image of a scene.
    /// Ref: https://raytracing.github.io/books/RayTracingInOneWeekend.html
    /// </summary>
    public class Camera
    {
        // Horizontal field-of-view of the camera in degrees
        public const double FieldOfView = 60.0;
        // Unit for one pixel in the image plane
        public const double PixelUnit = 1.0;

        private SceneOptions options;
        private Image outputImage;
        private Random random;
        private double aspectRatio;
        private double scale;
        private double pixelOffset;
        private Vector3 origin;
        private Vector3 cameraForward;
        private Vector3 cameraUp;
        private Vector3 cameraRight;
        private Vector3 bottomLeft;
        private Vector3 horizontal;
        private Vector3 vertical;

        /// <summary>
        /// Construct a new camera with the provided options and image to
        /// render the output on.
        /// </summary>
        /// <param name="options">Options data</param>
        /// <param name="outputImage">Image to store render output</param>
        public Camera(SceneOptions options, Image outputImage)
        {
            this.options = options;
            this.outputImage = outputImage;
            // Set a global random variable with a seed to ensure future reproducible results
            this.random = new Random(30019);

            // Find the aspect ratio and the FOV scaling factor of the image
            this.aspectRatio = (double) outputImage.Width / outputImage.Height;
            this.scale = Math.Tan((FieldOfView / 180 * Math.PI) / 2);

            // Calculate the offset for each pixel in the image plane
            this.pixelOffset = (PixelUnit / 2) / options.AAMultiplier;

            // Find the half width and height of the image plane
            double halfWidth = PixelUnit * this.scale;
            double halfHeight = halfWidth / this.aspectRatio;

            // Assign the origin and forward vector of the camera
            this.origin = options.CameraPosition;
            this.cameraForward = options.CameraAxis.Normalized();

            // Positive angle indicates counter-clockwise rotations
            double theta = options.CameraAngle / 180.0 * Math.PI;
            // Find the up vector after applying the rotation (initial = (0,1,0))
            //  x' = x cos θ − y sin θ
            //     = 0 cos θ - 1 sin θ
            //     = -sin θ
            //  y' = x sin θ + y cos θ
            //     = 0 sin θ + 1 cos θ
            //     =  cos θ
            this.cameraUp = new Vector3(-Math.Sin(theta), Math.Cos(theta), 0);

            // Find the orthogonal vector point right and up for the camera
            this.cameraRight = this.cameraUp.Cross(this.cameraForward).Normalized();
            this.cameraUp = this.cameraForward.Cross(this.cameraRight).Normalized();

            // Find the most bottom left pixel in the image plane
            this.bottomLeft = (this.origin) - (halfWidth * this.cameraRight) - (halfHeight * this.cameraUp) + this.cameraForward;

            // Find the horizontal and vertical length of the image plane
            this.horizontal = 2 * halfWidth * this.cameraRight;
            this.vertical = 2 * halfHeight * this.cameraUp;

            // TODO: Remove camera settings logging
            Console.WriteLine($"\nCameraSettings :-\n  AspectRatio   : {aspectRatio}\n  Scale         : {scale}");
            Console.WriteLine($"  PixelOffset   : {pixelOffset}\n  Origin        : {origin}");
            Console.WriteLine($"  CameraForward : {cameraForward}\n  CameraUp      : {cameraUp}\n  CameraRight   : {cameraRight}");
            Console.WriteLine($"  BottomLeft    : {bottomLeft}\n  Horizontal    : {horizontal}\n  Vertical      : {vertical}\n");
        }

        /// <summary>
        /// Get a ray from the camera origin to the current pixel/sub-pixel
        /// described by the given coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the pixel</param>
        /// <param name="y">Y-coordinate of the pixel</param>
        /// <param name="i">I-coordinate of the sub-pixel</param>
        /// <param name="j">J-coordinate of the sub-pixel</param>
        /// <returns>A ray from the camera origin to the pixel/sub-pixel</returns>
        public Ray GetRay(double x, double y, double i, double j)
        {
            // Find the pixel coordinate with their given offset with the bottom leftmost corner being (0,0)
            x = (x + this.pixelOffset * (i * 2 + 1)) / this.outputImage.Width;
            y = 1 - (y + this.pixelOffset * (j * 2 + 1)) / this.outputImage.Height;
            // Find the direction of the camera ray by getting the length in horizontal and vertical
            // direction from the bottom leftmost pixel and subtracting the camera origin from it
            Vector3 direction = this.bottomLeft + x * this.horizontal + y * this.vertical - this.origin;
            // Return the ray with the given camera origin and its normalized direction
            return new Ray(this.origin, direction.Normalized());
        }

        /// <summary>
        /// Get a random ray from within the aperture radius of the given
        /// ray origin to the focal point.
        /// </summary>
        /// <param name="rayOrigin">Origin of the camera ray</param>
        /// <param name="focalPoint">Focal point of the camera ray</param>
        /// <returns>A random ray from the camera origin to the focal point</returns>
        public Ray GetRay(Vector3 rayOrigin, Vector3 focalPoint)
        {
            // Find a random vector within the aperture radius of the camera origin
            Vector3 dv = RandomVectorInDisk() * this.options.ApertureRadius;
            // Multiply the x and y value of the random vector with the orthogonal unit vector pointing up and
            // to the right of the camera to get the new camera origin with a offset from the previous origin
            Vector3 offset = this.cameraRight * dv.X + this.cameraUp * dv.Y;
            Vector3 origin = rayOrigin + offset;
            // Return the ray with the new origin and the normalized direction from the origin to the focal point
            return new Ray(origin, (focalPoint - origin).Normalized());
        }

        /// <summary>
        /// Return a random vector within the 2D disk.
        /// </summary>
        /// <returns>A random vector inside the 2D disk</returns>
        private Vector3 RandomVectorInDisk()
        {
            while (true)
            {
                // Get two random values in the range of -1 and 1 for the x and y axis
                double dx = 2 * this.random.NextDouble() - 1;
                double dy = 2 * this.random.NextDouble() - 1;
                // Create a vector with the values and make sure it doesn't
                // intersect with the axis at the edge of the radius
                Vector3 vec = new Vector3(dx, dy, 0);
                if (vec.LengthSq() >= 1) continue;
                return vec;
            }
        }

    }
}
