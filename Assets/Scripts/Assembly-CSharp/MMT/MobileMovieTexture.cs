using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MMT
{
    public class MobileMovieTexture : MonoBehaviour
    {
        public delegate void OnFinished(MobileMovieTexture sender);

        [SerializeField]
        private string m_path;

        [SerializeField]
        private bool m_absolutePath;

        [SerializeField]
        private Material[] m_movieMaterials;

        [SerializeField]
        private bool m_playAutomatically = true;

        [SerializeField]
        private int m_loopCount = -1;

        [SerializeField]
        private float m_playSpeed = 1f;

        [SerializeField]
        private bool m_scanDuration = true;

        [SerializeField]
        private bool m_seekKeyFrame;

        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        public bool AbsolutePath
        {
            get { return m_absolutePath; }
            set { m_absolutePath = value; }
        }

        public Material[] MovieMaterial
        {
            get { return m_movieMaterials; }
        }

        public bool PlayAutomatically
        {
            set { m_playAutomatically = value; }
        }

        public int LoopCount
        {
            get { return m_loopCount; }
            set { m_loopCount = value; }
        }

        public float PlaySpeed
        {
            get { return m_playSpeed; }
            set { m_playSpeed = value; }
        }

        public bool ScanDuration
        {
            get { return m_scanDuration; }
            set { m_scanDuration = value; }
        }

        public bool SeekKeyFrame
        {
            get { return m_seekKeyFrame; }
            set { m_seekKeyFrame = value; }
        }

        // Stub properties
        public int Width { get { return 0; } }
        public int Height { get { return 0; } }
        public float AspectRatio { get { return 1f; } }
        public double FPS { get { return 1.0; } }
        public bool isPlaying
{
    get
    {
        Debug.Log("MMT isPlaying checked!!");
        return false;
    }
}
        public double playPosition { get { return 0.0; } set { } }
        public double duration { get { return 0.0; } }

        public bool pause
        {
            get { return false; }
            set { }
        }

        public event OnFinished onFinished;

        private void Awake()
{
    if (m_playAutomatically)
        StartCoroutine(PlayDelayed());
}

        private void OnDestroy()
        {
            // stub - nothing to do!!
        }

        private void Update()
{
    return;
}
		
		private System.Collections.IEnumerator PlayDelayed()
{
    // wait one frame!!
    yield return null;
    Play();
}

        [ContextMenu("Play")]
public void Play()
{
    Debug.Log("MMT Play() called - path: " + m_path);
    if (onFinished != null)
    {
        Debug.Log("MMT firing onFinished!!");
        onFinished(this);
        Debug.Log("MMT onFinished fired!!");
    }
    else
    {
        Debug.Log("MMT onFinished is NULL - nobody listening!!");
    }
}

        [ContextMenu("Stop")]
        public void Stop()
        {
            // stub - do nothing!!
        }

        public void SetTextures(Material material) { }
        public void RemoveTextures(Material material) { }
    }
}