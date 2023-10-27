using System;
using UnityEngine;

public class RayCastManager : MonoBehaviour
{
    private RaycastHit rayHit; // represent a raycast collision

    public ComputeShader shaderToApply;

    /// <summary>
    /// color in which the texture hitted will be colored in
    /// </summary>
    public Color colorToApply;

    /// <summary>
    /// the scale and shape of the resulting painting of a raycast hit
    /// </summary>
    public Vector2Int paintingFormat;

    /// <summary>
    /// in meters, the distance the ray can progress before fading
    /// </summary>
    public float distanceOfRay;

    /// <summary>
    /// layer on which raycast will be colliding
    /// </summary>
    public byte paintingLayer;

    // Update is called once per frame
    void Update()
    {
        if (distanceOfRay < 0)
            throw new ArgumentException("max distance of raycast should be positive");
        if(paintingLayer > 31) {
            throw new ArgumentException("unity layers are between 0 and 31 included");
        }

        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * distanceOfRay, new Color(255, 0, 0), 1.0f);

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out rayHit, distanceOfRay, 1 << paintingLayer))
        {
            var hittedRenderer = rayHit.transform.GetComponent<Renderer>();
            if(hittedRenderer != null )
            {
                Texture toPaint = hittedRenderer.material.mainTexture;
                try {                
                    hittedRenderer.material.mainTexture = ShaderUtility
                        .PaintTextureOf(toPaint, shaderToApply, rayHit.textureCoord, paintingFormat, colorToApply);
                }
                catch (Exception e) when (e is ArgumentException || e is NullReferenceException)
                {
                    if (toPaint is null)
                        Debug.LogError("Non Fatal : texture is null, being changed by another call to PaintTextureOf");
                    else
                        Debug.LogError(e.Message); // not excepted error
                }
            }
        }
    }
}
