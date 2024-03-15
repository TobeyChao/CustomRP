using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        static readonly ProfilingSampler s_SetupCameraPropertiesProfilingSampler = new ProfilingSampler("Setup Camera Properties");

        internal class SetupCameraPropertiesPassData
        {
            internal CameraData cameraData;
        }

        // ���һ������������Ե�Pass
        private void AddSetupCameraPropertiesPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<SetupCameraPropertiesPassData>("Setup Camera Properties Pass", out var passData, s_SetupCameraPropertiesProfilingSampler))
            {
                passData.cameraData = cameraData;
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.SetRenderFunc((SetupCameraPropertiesPassData data, RasterGraphContext context) =>
                {
                    context.cmd.SetupCameraProperties(data.cameraData.camera);
                });
            }
        }
    }
}
