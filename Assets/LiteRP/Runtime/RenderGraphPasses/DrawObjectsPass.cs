using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        static readonly ProfilingSampler s_DrawObjectsProfilingSampler = new ProfilingSampler("Draw Objects");

        readonly static ShaderTagId shaderTagId = new ShaderTagId("SRPDefaultUnlit");

        internal class DrawObjectsPassData
        {
            internal TextureHandle backBufferHandle;
            internal RendererListHandle opaqueRendererListHandle;
        }

        private void AddDrawObjectsPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            CameraData cameraData = frameData.Get<CameraData>();

            using (var builder = renderGraph.AddRasterRenderPass<DrawObjectsPassData>("Draw Objects Pass", out var passData, s_DrawObjectsProfilingSampler))
            {
                // 不透明
                RendererListDesc rendererListDesc = new RendererListDesc(shaderTagId, cameraData.cullingResults, cameraData.camera);
                rendererListDesc.sortingCriteria = SortingCriteria.CommonOpaque;
                rendererListDesc.renderQueueRange = RenderQueueRange.opaque;
                passData.opaqueRendererListHandle = renderGraph.CreateRendererList(rendererListDesc);
                builder.UseRendererList(passData.opaqueRendererListHandle);

                // 导入backbuffer
                passData.backBufferHandle = renderGraph.ImportBackbuffer(BuiltinRenderTextureType.CurrentActive);
                builder.SetRenderAttachment(passData.backBufferHandle, 0, AccessFlags.Write);

                // 全局
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((DrawObjectsPassData passData, RasterGraphContext context) =>
                {
                    context.cmd.DrawRendererList(passData.opaqueRendererListHandle);
                });
            }
        }
    }
}
