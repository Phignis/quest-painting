# Painting Method explanation

This text will clarify how the painting method is implemented.

## How to detect the painting?
> There is various way to detect a painting. In this implmentation, we had use the RayCast system, which project
> a ray from an origin in a certain destination. If this ray collide with a MeshCollider, it is possible to gather
> the UV coordinates of the impact point. From this point, we have to recolor the pixel in a certain radius, by using a
> shader, to avoid doing it over the CPU, which would result in CPU struggling to calculate each pixel to modify.<br/>
> We will use a compute shader to transform the Texture in a painted one. By choosing this, we order the GPU to do it,
> in heavy multi threading way, resulting in a short amount of time spent to change the colors.

## Why using a compute shader
> As said in [the file explaining how a shader is working](./shaders/shader_usage.md), we will only change a small
> amount of pixels among all in the Texture. A pixel shader would launch one thread for every single pixel, to color them.<br />
> But we're only actually only changing a small amount of pixels, and doing it with a pixel shader would result in a lot
> of useless thread. Compute shader will let us create a precise number of thread, to just modify pixels around the one
> from where the raycast hitted.

## How shader is implemented
> For each thread, we'll give the origin pixel, and the number of pixel to change on x and y.
> Using the threadID, seperated in x and y, we can calculate the offset of the origin pixel, by adding the threadID, and
> removing half of the number of pixel to paint, to center the operation on the raycast hit.
> In this way, we will create each thread in order to paint one pixel around the origin point.

## Limitation with this solution
> Some limitations exists with this solution. We are using UV map in order to paint a circle around the raycast hit.
> But UV map can map in neighbouring pixel some pixel that are on different side of the object, du to 2D transformation
> of the 3D model.<br />
> If there is some limited UV unwrapping, which not take in account this problem, it will result in possibly painting 
> at not intended places, because neighbour UV pixel could be at very different location on 3D model.

## How to detect that an object is fully painted
> This verification is pretty simple. The possibility is to count the number of pixels considered as painted, with a
> compute shader. Each thread will increment the valid-pixel counter if the color of described pixel is considered
> as valid.br />
> If we consider as valid only the precise RGB code, it will be too hard to detect some pixels that are not of the exact
> same color. To avoid this, a tolerance on the color have to be implemented.
> When retreiving the valid-pixel counter on CPU side, we will compare it with the total number of pixels in texture.
> If a certain percentage is reached, it will be valided.

## How to notify scene that object is fully painted
> Once object is totally painted, we need to continue the game.<br />
> For this reason, we either need to make a possible game manager checking for the paint, or let it to the painted object
> itself, which will have the charge to notify all interested game object.<br />
> This solution, called Observer, have benefits to be scalable. Even if we don't know right now how many GameObject in
> scene need to change upon object fully-painted event, we can just make them subscribe to the subject.