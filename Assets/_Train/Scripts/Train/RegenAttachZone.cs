using UnityEngine;

namespace _Train.Scripts.Train
{
    public class RegenAttachZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var energy = other.GetComponentInParent<_Train.Scripts.Character.Energy>();
            if (energy == null) ;
            
            energy.SetRegenZoneState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            var energy = other.GetComponentInParent<_Train.Scripts.Character.Energy>();
            if (energy == null) return;
            
            energy.SetRegenZoneState(false);
        }
    }
}
