# Shader Principle in Unity

All this explanation was mainlky made out of two explanations, which are the first and second ressources in Lexicography.

## What is a shader?
> A shader is a mathematical function, which will usually transform an input into an output.<br />
> Shaders are usually used to calculate skin alteration, or realistic lightning.
> It is executed on GPU, which will allows us to do heavy multithreading, and light up the CPU workcharge.

## What's the principle of execution?
> As said before, the purpose of shaders is to do the quickest as possible transformation on a large scale of elements.<br />
> To do so, the CPU will call a certain number of ThreadGroup, which will consist in Threads.<br />
> Each thread in ThreadGroup will be called in parrallel, so will be given to GPU at once by a call of CPU.
> GPU will then call the threads.<br />
> All threads will execute the transformation to one element.

## What's the difference between pixel shader and compute shader?
> pixel shader ends with .shader extension in Unity<br />
> compute shader ends with .compute extension in Unity<br />
> <br />
> This specific part is a summary of the fourth and fifth ressources in lexicography.<br />
> As said before, the threads of shader are executed in parrallels, in wave.<br />
> In compute shader, you can specify the size of a threadgroup, up to 1024, in "Shader Model 5". This allow to have the minimum group, avoiding CPU usage, slower.<br />
> Pixel shader can't specify the size of threadgroup, it's automatic, so the coincident pixel have the guarantee
> to be in the same group. This will have to effect to write data in a precise part of memory during a wave,
> allowing cache optimisation to come into place.<br />
> One specification of pixel are that they are specifically designed to display texturing render, justifying the pixel order in thread.
> Moreover, this specification comes with some downsides: it comes with the graphics pipeline render, which take some time.
> If you don't need to display the changes, it will be a waste of time.<br />
> Compute shader are more general, and don't come as part of the graphic pipeline. The downside is that you'll have to 
> display result by yourself.

## ThreadGroups / WorkGroups, Threading Index
> ThreadGroups will be activated by the CPU, sending it to the GPU, will take care of threads inside.<br />
> As a reminder, in the "Shader Model 5", the threadgroup can countains up to 1024 threads<br />
> When working in 3D, they can be seen as 3D vector, to simplify ; like here : [numthreads(8,8,2)]. There is 8 * 8 * 4 = 256 threads
> in a group.

## Lexicography
> 1.  https://medium.com/@simonalbou/lusage-de-compute-shaders-sur-unity-un-gain-de-performance-violent-54c1b0f72698
> 2.  https://polycoding.net/compute-shaders-unity
> 3.  https://docs.unity3d.com/Manual/class-ComputeShader.html
> 4.  https://computergraphics.stackexchange.com/questions/54/when-is-a-compute-shader-more-efficient-than-a-pixel-shader-for-image-filtering/60#60
> 5.  https://blog.chirag.io/compute-shaders-and-you/
