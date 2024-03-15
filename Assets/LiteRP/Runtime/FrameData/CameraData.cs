using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    public class CameraData : ContextItem
    {
        public Camera camera;
        public CullingResults cullingResults;

        public override void Reset()
        {
            camera = null;
            cullingResults = default;
        }

        public Color GetBackgroundColor()
        {
            return CoreUtils.ConvertSRGBToActiveColorSpace(camera.backgroundColor);
        }

        internal RTClearFlags GetClearFlags()
        {
            if (camera.clearFlags == CameraClearFlags.Depth)
                return RTClearFlags.Depth;
            else if (camera.clearFlags == CameraClearFlags.Nothing)
                return RTClearFlags.None;
            else
                return RTClearFlags.All;
        }
    }
}
