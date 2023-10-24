# Shader Principle in Unity

This text will clarify how the painting method is implemented.

## How to detect the painting?
> There is various way to detect a painting. In this implmentation, we had use the RayCast system, which project
> a ray from an origin in a certain destination. If this ray collide with a MeshCollider, it is possible to gather
> the UV coordinates of the impact point. From this point, we have to recolor the pixel in a certain radius, by using a
> shader, to avoid doing it over the CPU, which would result in CPU struggling to calculate each pixel to modify.<br/>
> We will use a compute shader to transform the Texture in a painted one. By choosing this, we order the GPU to do it,
> in heavy multi threading way, resulting in a short amount of time spent to change the colors.

## Why using a compute shader
> As said in [the file explaining how a shader is working](./shader_usage.md), we will only change a small amount of
> pixels among all in the Texture. A pixel shader would launch one thread for every single pixel, to color them.<br />
> But we're only actually only changing a small amount of pixels, and doing it with a pixel shader would result in a lot
> of useless thread. Compute shader will let us create a precise number of thread, to just modify pixels around the one
> from where the raycast hitted.

## How shader is implemented
> For each thread, we'll give the origin pixel, and the number of pixel to change on x and y.
> Using the threadID, seperated in x and y, we can calculate the offset of the origin pixel, by adding the threadID, and
> removing half of the number of pixel to paint, to center the operation on the raycast hit.
> In this way, we will create each thread to paint one pixel around the origin point.

## Limitation with this solution
> Some limitations exists with this solution. We are using UV map in order to paint a circle around the raycast hit.
> But UV map can map in neighbouring pixel some pixel that are on different side of the object, du to 2D transformation
> of the 3D model.<br />
> If there is some limited UV unwrapping, which not take in account this problem, it will result in possibly painting 
> at not intended places, because neighbour UV pixel could be at very different location on 3D model.<br />