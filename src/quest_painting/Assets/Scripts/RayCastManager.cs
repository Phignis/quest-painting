using System;
using UnityEngine;

public class RayCastManager : MonoBehaviour
{
    private RaycastHit rayHit; // represent a raycast collision

    public ComputeShader shaderToApply;

    public Color colorToApply;

    public Vector2Int paintingFormat;

    public float distanceOfRay;

    // Update is called once per frame
    void Update()
    {
        if (distanceOfRay < 0)
            throw new ArgumentException("max distance of raycast should be positive");

        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * distanceOfRay, new Color(255, 0, 0), 1.0f);

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out rayHit, distanceOfRay))
        {
            var hittedRenderer = rayHit.transform.GetComponent<Renderer>();
            Texture toPaint = hittedRenderer.material.mainTexture;
            try
            {
                hittedRenderer.material.mainTexture = ShaderUtility
                        .PaintTextureOf(toPaint, shaderToApply, rayHit.textureCoord, paintingFormat, colorToApply);
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }

        }
    }
}
