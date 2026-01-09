using System.Collections.Generic;
using UnityEngine;

namespace _Train.Scripts.Root.Footsteps
{
    [CreateAssetMenu(fileName = "FootstepsSurfacePairsConfig", menuName = "FootstepsSurfacePairsConfig")]
    public class SurfaceAudioPairsConfig : ScriptableObject
    {
        [SerializeField] private List<SurfaceAudioPair> surfaceAudioPairs;
        
        public List<SurfaceAudioPair> SurfaceAudioPairs => surfaceAudioPairs;

        public AudioClip[] GetAudioClips(SurfaceType surfaceType)
        {
            return surfaceAudioPairs[(int)surfaceType].audioClips;
        }
    }
}