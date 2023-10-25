using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// PaintedSubject is the Subject of the
/// <see cref="https://refactoring.guru/design-patterns/observer">Observer design pattern</see> Here design pattern is altered
/// It will check if it's fully painted, and will send a notification if it is, to all observers which will have subscribe
/// </summary>
public class PaintedSubject : Subject
{
    /// <summary>
    /// shader which will check if the object is fully painted
    /// </summary>
    public ComputeShader checkerShader;

    /// <summary>
    /// the color to search, representing the state painted for a pixel
    /// </summary>
    public Color paintingColor;

    /// <summary>
    /// the offset in each RGB channel, to allow multiple taint of paintingColor
    /// </summary>
    public int validColorOffset;

    private bool _isFullyPainted = false;

    private void Start()
    {
        paintingColor.r *= 255;
        paintingColor.g *= 255;
        paintingColor.b *= 255;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isFullyPainted)
        {
            int kernelId = checkerShader.FindKernel("CheckColor"); // throw exception if not founded
            if (validColorOffset < 0) throw new ArgumentException("color offset should always be positive");

            Texture texture = GetComponent<Renderer>().material.mainTexture;

            ComputeBuffer computeBuffer = new ComputeBuffer(1, sizeof(uint)); // we create a buffer with only one uint
            uint[] nbPixelsValid = { 0 };
            computeBuffer.SetData(nbPixelsValid); // we initialize the uint buffer to 0

            checkerShader.SetTexture(kernelId, "TextureToCheck", texture);
            checkerShader.SetVector("ValidColor", paintingColor);
            checkerShader.SetInt("AcceptableOffset", validColorOffset);
            checkerShader.SetBuffer(kernelId, "NbValidPixels", computeBuffer);

            checkerShader.Dispatch(kernelId, texture.width / 8, texture.height / 8, 1);

            computeBuffer.GetData(nbPixelsValid);
            computeBuffer.Dispose();
            Debug.Log(nbPixelsValid[0]);
        }
    }
}
