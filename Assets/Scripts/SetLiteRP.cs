using UnityEngine;
using UnityEngine.Rendering;

public class SetLiteRP : MonoBehaviour
{
    public RenderPipelineAsset asset;

    private void OnEnable()
    {
        GraphicsSettings.renderPipelineAsset = asset;
    }

    private void OnValidate()
    {
        GraphicsSettings.renderPipelineAsset = asset;
    }
}
