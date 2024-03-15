using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder : IRenderGraphRecorder, IDisposable
    {
        private TextureHandle m_BackBufferRenderHandle = TextureHandle.nullHandle;
        private RTHandle m_RenderTargetHandle = null;

        public void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            CameraData cameraData = frameData.Get<CameraData>();
            CreateRenderGraphCameraRenderTarget(renderGraph, cameraData);
            AddSetupCameraPropertiesPass(renderGraph, cameraData);
            AddClearRenderTargetPass(renderGraph, cameraData);
            AddDrawObjectsPass(renderGraph, cameraData);
        }

        private void CreateRenderGraphCameraRenderTarget(RenderGraph renderGraph, CameraData cameraData)
        {
            if (m_RenderTargetHandle == null)
            {
                RenderTargetIdentifier renderTargetIdentifier = BuiltinRenderTextureType.CurrentActive;
                m_RenderTargetHandle = RTHandles.Alloc(renderTargetIdentifier, "BackBuffer Color");
            }

            Color cameraBackgroundColor = CoreUtils.ConvertSRGBToActiveColorSpace(cameraData.camera.backgroundColor);

            var importResourceParams = new ImportResourceParams();
            importResourceParams.clearOnFirstUse = true;
            importResourceParams.clearColor = cameraBackgroundColor;
            importResourceParams.discardOnLastUse = false;

            RenderTargetInfo renderTargetInfo = new RenderTargetInfo();
            renderTargetInfo.msaaSamples = 1;
            renderTargetInfo.volumeDepth = 1;
            renderTargetInfo.format = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default, QualitySettings.activeColorSpace != ColorSpace.Gamma);
            renderTargetInfo.width = Screen.width;
            renderTargetInfo.height = Screen.height;

            //RenderTargetInfo renderTargetInfoDepth = new RenderTargetInfo();
            //renderTargetInfoDepth.format = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);

            m_BackBufferRenderHandle = renderGraph.ImportTexture(m_RenderTargetHandle, renderTargetInfo, importResourceParams);
        }

        public void Dispose()
        {
            RTHandles.Release(m_RenderTargetHandle);
            GC.SuppressFinalize(this);
        }
    }
}
