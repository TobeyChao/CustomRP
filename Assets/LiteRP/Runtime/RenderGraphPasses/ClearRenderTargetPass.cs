using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        static readonly ProfilingSampler s_ClearRenderTargetProfilingSampler = new ProfilingSampler("ClearRenderTarget");

        internal class ClearRenderTargetPassData
        {
            internal CameraData cameraData;
        }

        // 添加一个设置相机属性的Pass
        private void AddClearRenderTargetPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<ClearRenderTargetPassData>("ClearRenderTarget Pass", out var passData, s_ClearRenderTargetProfilingSampler))
            {
                RTClearFlags clearDepth = cameraData.GetClearFlags();
                Color backgroundColor = cameraData.GetBackgroundColor();

                builder.SetRenderAttachment(m_BackBufferRenderHandle, 0, AccessFlags.Write);

                builder.SetRenderFunc((ClearRenderTargetPassData passData, RasterGraphContext rasterGraphContext) =>
                {
                    rasterGraphContext.cmd.ClearRenderTarget(clearDepth, backgroundColor, 1, 0);
                });
            }
        }
    }
}
