using System;
using UnityEngine;

public class RayCastManager : MonoBehaviour
{
    private RaycastHit rayHit; // detect raycast collision

    public ComputeShader shaderToApply;


    // Start is called before the first frame update
    void Start()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHit, 20.0f))
        {
            Renderer rend = rayHit.transform.GetComponent<Renderer>();
            Debug.Log(rayHit.textureCoord);
            Texture toPaint = rend.material.mainTexture;
            try
            {
                rend.material.mainTexture = ShaderUtility.PaintTextureOf(toPaint, shaderToApply, rayHit.textureCoord);
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20.0f, Color.red, 4.0f);
    }
}
