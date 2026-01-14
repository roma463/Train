using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace _Train.Scripts.Character.MovementStateMachine
{
    public class EnergyUI : MonoBehaviour
    {
        [SerializeField] private Slider energySlider;

        private Character _character;

        private void Awake()
        {
            if (energySlider == null)
                energySlider = GetComponentInChildren<Slider>(true);
        }

        private void OnEnable()
        {
            TryBind();
        }

        private void Update()
        {
            if (_character == null)
                TryBind();
        }

        private void OnDisable()
        {
            if (_character != null)
                _character.EnergyNormalizedChanged -= OnEnergyChanged;
            _character = null;
        }

        private void TryBind()
        {
            if (!NetworkClient.active) return;
            if (NetworkClient.localPlayer == null) return;

            var ch = NetworkClient.localPlayer.GetComponent<Character>();
            if (ch == null) return;

            if (_character == ch) return;

            if (_character != null)
                _character.EnergyNormalizedChanged -= OnEnergyChanged;

            _character = ch;
            _character.EnergyNormalizedChanged += OnEnergyChanged;
            
            OnEnergyChanged(_character.EnergyNormalized);
        }

        private void OnEnergyChanged(float norm)
        {
            energySlider.value = Mathf.Clamp01(norm);
        }
    }
}