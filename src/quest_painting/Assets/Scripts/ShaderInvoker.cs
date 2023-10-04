using UnityEngine;

public class ShaderInvoker : MonoBehaviour
{

    public Renderer toRender; // Rendered holding the texture to modify

    public ComputeShader shader; // the shader which will process the above texture

    // Start is called before the first frame update
    void Start()
    {

        RenderTexture renderedTexture = new RenderTexture(256, 256, 24);
        renderedTexture.enableRandomWrite = true;

        renderedTexture.Create();

        int kernelIndex = shader.FindKernel("CSMain");

        shader.SetTexture(kernelIndex, "Result", renderedTexture);
        shader.SetFloat("Resolution", renderedTexture.width);
        shader.Dispatch(kernelIndex, renderedTexture.width / 8, renderedTexture.height / 8, 1);

        toRender.material.mainTexture = renderedTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
