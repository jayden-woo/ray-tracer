# COMP30019 Assignment 1 - Ray Tracer
This is your README.md... you should write anything relevant to your implementation here.

Please ensure your student details are specified below (*exactly* as on UniMelb records):

**Name:** Jun Cheng Woo \
**Student Number:** 1045457 \
**Username:** woojw \
**Email:** woojw@student.unimelb.edu.au

## Completed stages

Tick the stages bellow that you have completed so we know what to mark (by editing README.md). At most **six** marks can be chosen in total for stage three. If you complete more than this many marks, pick your best one(s) to be marked!

<!---
Tip: To tick, place an x between the square brackes [ ], like so: [x]
-->

##### Stage 1

- [x] Stage 1.1 - Familiarise yourself with the template
- [x] Stage 1.2 - Implement vector mathematics
- [x] Stage 1.3 - Fire a ray for each pixel
- [x] Stage 1.4 - Calculate ray-entity intersections
- [x] Stage 1.5 - Output primitives as solid colours

##### Stage 2

- [x] Stage 2.1 - Diffuse materials
- [x] Stage 2.2 - Shadow rays
- [x] Stage 2.3 - Reflective materials
- [x] Stage 2.4 - Refractive materials
- [x] Stage 2.5 - The Fresnel effect
- [x] Stage 2.6 - Anti-aliasing

##### Stage 3

- [ ] Option A - Emissive materials (+6)
- [ ] Option B - Ambient lighting/occlusion (+6)
- [ ] Option C - OBJ models (+6)
- [ ] Option D - Glossy materials (+3)
- [x] Option E - Custom camera orientation (+3)
- [ ] Option F - Beer's law (+3)
- [x] Option G - Depth of field (+3)

*Please summarise your approach(es) to stage 3 here.*

##### Option E

First, all the previous code related to pixel coordinates calculations and camera controls are refactored into a new class of its own in Camera.cs file. The calculations for pixel coordinates and their offset calculations are also rewritten to first calculating the length and height of the image plane and then finding the final position and subsequently direction of each ray by calculating how far from the bottom leftmost pixel it is and multiplying it by the total length.

Next, the position translation of the camera origin is achieved by just simply assigning it from the new position vector specified in the command line arguments. The camera's rotation is achieved by first simply defining the forward vector of the new camera orientation as the normalized camera axis vector from the command line arguments.
The up vector for the camera is calculated according to trigonometry calculations of the new X and Y coordinates. After that, the right vector is achieved by getting the cross product of the up vector and the forward vector and the result is then normalized. The new orthogonal up vector is calculated by getting the normalized cross product of the right and forward vector.

Finally, with the new vectors defined for the three axis of the camera's orientation, any further calculations of the new plane formed by the three vectors can be achieved by just multiplying the X components with the right unit vector, Y components with the up unit vector, and the Z components with the forward unit vector.

##### Option G

First, to create a depth of field effect in the rendering of the final image, a focal point P is calculated by multiplying the ray direction with the focal length and adding the result to the ray origin. This focal point of all pixels in the image will form a spherical focal plane that is equidistant from the camera in any direction. Any objects that intersects with the focal plane or near the focal plane will appear to be in focus. Otherwise, the objects will appear blurry and out of focus.

Next, to create the blur effect to objects that are out of focus, a random sample around the aperture radius from the camera origin is sampled. Shifting the ray origin within the aperture radius is done by generating two random offset dx and dy within the range of [-aperture radius, aperture radius] and using that as the new origin. A new ray direction vector from the new ray origin to the focal point is calculated and normalized. A ray from the new origin is created with the calculated new ray direction and the colour is calculated by firing the ray into the scene. This colour is then averaged between all the samples of the same pixel and the final colour is set for the pixel in the final rendered image.

The sample size for the field of view effect is being defined as a constant at the top of the Scene.cs file and can be adjusted to reduce render time but with a side effect of less accurately rendered final image. The default value of 50 samples is defined after experimenting with different sample sizes to get an ideal tradeoff between rendering time and final image quality.

## Final scene render

Be sure to replace ```/images/final_scene.png``` with your final render so it shows up here:

![My final render](/images/final_scene.png)

This render took **x** minutes and **y** seconds on my PC.

I used the following command to render the image exactly as shown:

```
dotnet run -- (... your command line args)
```

## Sample outputs

We have provided you with some sample tests located at ```/tests/*```. So you have some point of comparison, here are the outputs our ray tracer solution produces for given command line inputs (for the first two stages, left and right respectively):

###### Sample 1
```
dotnet run -- -f tests/sample_scene_1.txt -o images/sample_scene_1.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_1_s1.png" />
  <img src="/images/sample_scene_1_s2.png" />
</p>

###### Sample 2

```
dotnet run -- -f tests/sample_scene_2.txt -o images/sample_scene_2.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_2_s1.png" />
  <img src="/images/sample_scene_2_s2.png" />
</p>

## References

*You must list any references you used!*

To get you started, here is some good reading material:

Working through a ray tracer, from the head of the xbox games studio: https://www.linkedin.com/pulse/writing-simple-ray-tracer-c-matt-booty/

*Ray Tracing in a Weekend*: https://raytracing.github.io/

Great walkthrough of some of the basic maths: https://blog.scottlogic.com/2020/03/10/raytracer-how-to.html

Scratchapixel: Intro to Ray Tracing: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-ray-tracing/how-does-it-work

Scratchapixel: A Minimal Ray-Tracer: Rendering Simple Shapes (Sphere, Cube, Disk, Plane, etc.): https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/parametric-and-implicit-surfaces

Scratchapixel: Ray Tracing: Rendering a Triangle: https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/moller-trumbore-ray-triangle-intersection

Scratchapixel: Introduction to Shading: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-shading/reflection-refraction-fresnel

Depth of Field in Path Tracing: https://medium.com/@elope139/depth-of-field-in-path-tracing-e61180417027

Ray Tracing in One Weekend: https://raytracing.github.io/books/RayTracingInOneWeekend.html
