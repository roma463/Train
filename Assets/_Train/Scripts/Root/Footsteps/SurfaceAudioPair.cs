using System;
using UnityEngine;

namespace _Train.Scripts.Root.Footsteps
{
    [Serializable]
    public class SurfaceAudioPair
    {
        public SurfaceType surfaceType;
        public Texture2D surfaceTexture;
        public AudioClip[] audioClips;
    }
}