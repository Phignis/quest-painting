using System;
using UnityEngine;

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
    public Color paintingColor; // getted color is in float representation (from 0 to 1)

    /// <summary>
    /// the offset in each RGB channel, to allow multiple taint of paintingColor
    /// </summary>
    public int validColorOffset;

    /// <summary>
    /// the percentage needed for the object to consider to be fully painted.
    /// Value should always be between 0 and 1 (included)
    /// </summary>
    public float percentNeededForFullPaint;

    private bool _isFullyPainted = false;

    // Update is called once per frame
    void Update()
    {
        if(!_isFullyPainted)
        {
            var kernelId = checkerShader.FindKernel("CheckColor"); // throw exception if not founded

            if (validColorOffset < 0 || validColorOffset > 255)
                throw new ArgumentException("color offset should always be between 0 and 255");
            if (percentNeededForFullPaint < 0f || percentNeededForFullPaint > 1f)
                throw new ArgumentException("percent should always be between 0 and 1");
            
            Texture texture = GetComponent<Renderer>().material.mainTexture;

            var computeBuffer = new ComputeBuffer(1, sizeof(uint)); // we create a buffer with only one uint
            uint[] nbPixelsValid = { 0 };
            computeBuffer.SetData(nbPixelsValid); // we initialize the uint buffer to 0
            
            #region Set shader variables
            checkerShader.SetTexture(kernelId, "TextureToCheck", texture);
            checkerShader.SetVector("ValidColor", paintingColor);
            checkerShader.SetFloat("AcceptableOffsetPercent", (float)validColorOffset / 255);
            checkerShader.SetBuffer(kernelId, "NbValidPixels", computeBuffer);
            #endregion

            checkerShader.GetKernelThreadGroupSizes(kernelId, out uint nbThreadX, out uint nbThreadY, out _);
            checkerShader.Dispatch(kernelId, texture.width / (int)nbThreadX, texture.height / (int)nbThreadY, 1);

            computeBuffer.GetData(nbPixelsValid);
            computeBuffer.Dispose();

            var nbPixelToBeValid = (uint)(texture.width * texture.height * percentNeededForFullPaint);

            if (nbPixelsValid[0] > nbPixelToBeValid)
            {
                _isFullyPainted = true;
                NotifySubscribers();
                Debug.Log("painted!");
            } else
            {
                Debug.Log("not fully painted: at " + nbPixelsValid[0] / (float)nbPixelToBeValid + "% ");
            }
        }
    }
}
