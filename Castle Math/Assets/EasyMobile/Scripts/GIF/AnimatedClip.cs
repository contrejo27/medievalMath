using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    public sealed class AnimatedClip : IDisposable
    {
        /// <summary>
        /// The width of this clip in pixels.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// The height of this clip in pixels.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        /// The FPS of this clip.
        /// </summary>
        /// <value>The frame per second.</value>
        public int FramePerSecond { get; private set; }

        /// <summary>
        /// The length of this clip in seconds.
        /// </summary>
        /// <value>The length.</value>
        public float Length { get; private set; }

        /// <summary>
        /// The frames of this clip.
        /// </summary>
        /// <value>The frames.</value>
        public RenderTexture[] Frames { get; private set; }

        // Whether this object is disposed.
        private bool isDisposed = false;

        public AnimatedClip(int width, int height, int fps, RenderTexture[] frames)
        {
            this.Width = width;
            this.Height = height;
            this.FramePerSecond = fps;
            this.Frames = frames;
            this.Length = (float)frames.Length / fps;
        }

        // Would love to do cleanup in the finalizer as a fallback, but unfortunately we can't
        // access the Release method of RenderTexture from the background
        // thread of the GC. Better warn the user to call Dispose explicitly.
        //        ~AnimatedClip()
        //        {
        //            Cleanup();
        //        }

        public bool IsDisposed()
        {
            return isDisposed;
        }

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (!isDisposed)
            {
                if (this.Frames != null)
                {
                    foreach (var rt in Frames)
                    {
                        rt.Release();
                        UnityEngine.Object.Destroy(rt);
                    }
                    this.Frames = null;
                }

                isDisposed = true;
            }
        }
    }
}
