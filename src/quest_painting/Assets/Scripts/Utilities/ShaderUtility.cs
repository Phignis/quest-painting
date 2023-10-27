using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderUtility : MonoBehaviour
{
    ///<summary>
    /// ComputeShader which will be applied when calling PaintTextureOf or searching for id of the kernel named PaintTexture
    /// </summary>
    public static ComputeShader ShaderToApply {
        /// <summary>
        /// Test if a kernel called PaintTexture exists in given shader, and if it exists, will set the ShaderToApply according
        /// </summary>
        /// 
        /// <param name="value">ComputeShader to apply to property</param>
        /// 
        /// <exception cref="ArgumentException">
        /// if no kernel called PaintTexture where found in the given ComputeShader
        /// </exception>
        set
        {
            // will throw ArgumentException if the excepted kernel is not available, preventing to apply the shader
            value.FindKernel("PaintTexture");
            _shaderToApply = value;
        }
    }
    private static ComputeShader _shaderToApply;

    private static RenderTexture _renderedTexture = null;

    /// <summary>
    /// Give back the ID of kernel named PaintTexture from the last shader used in last call of PaintTextureOf,
    /// or of last set of ShaderToApply
    /// </summary>
    public static int ShaderId => _shaderToApply.FindKernel("PaintTexture"); // calculated property to add some meaning for searching the kernel

    /// <summary>
    /// Will calculate and return the painted texture, from an initial texture
    /// </summary>
    /// 
    /// <param name="initialTexture">the texture which will be used as a base for the paint part</param>
    /// <param name="shaderForPaiting">shader which will apply the painting effect</param>
    /// <param name="contactPointOnUvMap">the uv coordinates of contact point from where color will be spreaded</param>
    /// 
    /// <returns>the painted texture</returns>
    /// 
    /// <exception cref="ArgumentException">
    /// if no kernel called PaintTexture where found in shaderForPaiting
    /// </exception>
    public static RenderTexture PaintTextureOf(Texture initialTexture, ComputeShader shaderForPaiting, Vector2 contactPointOnUvMap,
                                                    Vector2Int scaleOfSpray, float paintFade, Color colorToApply)
    {
        ShaderToApply = shaderForPaiting; // check if there is a valid name in kernels, and set it to update ShaderId

        // create a RenderTexture with resolution of the given texture
        if(_renderedTexture is null || initialTexture.height != _renderedTexture.height || initialTexture.width != _renderedTexture.width)
        {
            _renderedTexture = new RenderTexture(initialTexture.width, initialTexture.height, 24);
            _renderedTexture.enableRandomWrite = true;
        }

        Graphics.Blit(initialTexture, _renderedTexture); // copy into the renderTexture the pixel of initialTexture
        //_renderedTexture.Create();

        // contactPointOnUvMap get coord on UvMap (from range from [0, 0] to [1, 1]. Multiply by resolution to get the pixel coords in texture
        int[] coords = { (int)(contactPointOnUvMap.x * initialTexture.width),
                            (int)(contactPointOnUvMap.y * initialTexture.height)};
        _shaderToApply.GetKernelThreadGroupSizes(ShaderId, out uint nbThreadX, out uint nbThreadY, out _);

        // determine the number of pixels which will be changed in each direction, to give it to shader, to center the application
        int[] nbPixelsToChange = { (int)nbThreadX * scaleOfSpray.x, (int)nbThreadY * scaleOfSpray.y };

        #region Set Shader variables
        _shaderToApply.SetTexture(ShaderId, "TextureToTransform", _renderedTexture);
        _shaderToApply.SetVector("ColorToPaint", colorToApply);
        _shaderToApply.SetInts("PixelCoordApplyPoint", coords);
        _shaderToApply.SetInts("NumberPixelToChange", nbPixelsToChange);
        _shaderToApply.SetFloat("FadingPaint", paintFade);
        #endregion


        _shaderToApply.Dispatch(ShaderId, scaleOfSpray.x, scaleOfSpray.y, 1);

        return _renderedTexture;
    }
}
