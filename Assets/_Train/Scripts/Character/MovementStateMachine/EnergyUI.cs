using System;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Train.Scripts.Character.MovementStateMachine
{
    public class EnergyUI : MonoBehaviour
    {
        [SerializeField] private Slider energySlider;
        [SerializeField] private Energy energy;

        private void OnEnable()
        {
            energy.OnEnergyNormalizedChanged += OnEnergyChanged;
        }

        private void OnDestroy()
        {
            energy.OnEnergyNormalizedChanged -= OnEnergyChanged;
        }

        private void OnEnergyChanged(float norm)
        {
            energySlider.value = Mathf.Clamp01(norm);
        }
    }
}