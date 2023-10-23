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
    public static RenderTexture PaintTextureOf(Texture initialTexture, ComputeShader shaderForPaiting, Vector2 contactPointOnUvMap)
    {
        ShaderToApply = shaderForPaiting; // check if there is a shader to apply
        RenderTexture renderedTexture = new RenderTexture(initialTexture.width, initialTexture.height, 24);
        renderedTexture.enableRandomWrite = true;

        Graphics.Blit(initialTexture, renderedTexture);
        

        renderedTexture.Create();

        _shaderToApply.SetTexture(ShaderId, "Result", renderedTexture);
        _shaderToApply.SetFloat("Resolution", renderedTexture.width);
        _shaderToApply.Dispatch(ShaderId, renderedTexture.width / 8, renderedTexture.height / 8, 1);

        return renderedTexture;
    }
}
