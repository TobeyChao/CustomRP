using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline
    {
        RenderGraph m_RenderGraph;
        LiteRenderGraphRecorder m_Recorder;
        ContextContainer m_ContextContainer;

        public LiteRenderPipeline()
        {
            InitRenderGraph();
        }

        private void InitRenderGraph()
        {
            m_RenderGraph = new RenderGraph("LiteRP RenderGraph");
            m_Recorder = new LiteRenderGraphRecorder();
            m_ContextContainer = new ContextContainer();
        }

        protected override void Dispose(bool disposing)
        {
            CleanupRenderGraph();
            base.Dispose(disposing);
        }

        private void CleanupRenderGraph()
        {
            m_ContextContainer?.Dispose();
            m_ContextContainer = null;
            m_Recorder = null;
            m_RenderGraph?.Cleanup();
            m_RenderGraph = null;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            BeginContextRendering(context, cameras);

            for (int i = 0; i < cameras.Count; i++)
            {
                RenderCamera(context, cameras[i]);
            }

            m_RenderGraph.EndFrame();

            EndContextRendering(context, cameras);
        }

        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context, camera);

            if (!PrepareFrameData(context, camera))
                return;

            CommandBuffer cmd = CommandBufferPool.Get(camera.name);

            context.SetupCameraProperties(camera);

            RenderAndExcuteRenderGraph(context, camera, cmd);

            context.ExecuteCommandBuffer(cmd);

            cmd.Clear();
            CommandBufferPool.Release(cmd);

            context.Submit();

            EndCameraRendering(context, camera);
        }

        private bool PrepareFrameData(ScriptableRenderContext context, Camera camera)
        {
            ScriptableCullingParameters scriptableCullingParameters;
            if (!camera.TryGetCullingParameters(out scriptableCullingParameters))
                return false;
            CullingResults cullingResults = context.Cull(ref scriptableCullingParameters);

            CameraData cameraData = m_ContextContainer.GetOrCreate<CameraData>();
            cameraData.camera = camera;
            cameraData.cullingResults = cullingResults;
            return true;
        }

        private void RenderAndExcuteRenderGraph(ScriptableRenderContext context, Camera camera, CommandBuffer cmd)
        {
            RenderGraphParameters renderGraphParameters = new RenderGraphParameters
            {
                executionName = camera.name,
                commandBuffer = cmd,
                scriptableRenderContext = context,
                currentFrameIndex = Time.frameCount,
            };

            m_RenderGraph.BeginRecording(renderGraphParameters);
            m_Recorder.RecordRenderGraph(m_RenderGraph, m_ContextContainer);
            m_RenderGraph.EndRecordingAndExecute();
        }
    }
}
