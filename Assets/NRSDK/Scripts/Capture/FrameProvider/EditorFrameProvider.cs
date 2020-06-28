namespace NRKernal.Record
{
    using NRKernal;
    using UnityEngine;
    using System.Collections;

    public class EditorFrameProvider : AbstractFrameProvider
    {
        private Texture2D m_DefaultTexture;
        private RGBTextureFrame m_DefaultFrame;
        private bool isPlay = false;

        public EditorFrameProvider()
        {
            m_DefaultTexture = Resources.Load<Texture2D>("Record/Textures/captureDefault");
            m_DefaultFrame = new RGBTextureFrame();
            m_DefaultFrame.texture = m_DefaultTexture;

            NRKernalUpdater.Instance.StartCoroutine(UpdateFrame());
        }

        public IEnumerator UpdateFrame()
        {
            while (true)
            {
                if (isPlay)
                {
                    m_DefaultFrame.timeStamp = NRTools.GetTimeStamp();
                    OnUpdate?.Invoke(m_DefaultFrame);
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public override Resolution GetFrameInfo()
        {
            Resolution resolution = new Resolution();
            resolution.width = m_DefaultTexture.width;
            resolution.height = m_DefaultTexture.height;
            return resolution;
        }

        public override void Play()
        {
            isPlay = true;
        }

        public override void Stop()
        {
            isPlay = false;
        }

        public override void Release()
        {
            isPlay = false;
            NRKernalUpdater.Instance.StopCoroutine(UpdateFrame());
        }
    }
}
