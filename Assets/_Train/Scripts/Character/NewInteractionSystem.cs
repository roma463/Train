using _Train.Scripts.Root;
using _Train.Scripts.UI;
using Mirror;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class NewInteractionSystem : NetworkBehaviour
    {
        [SerializeField] private Character character;
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private InteractionSystem interactionSystem;
        
        private INPUTE _input;
        
        private IInteractable _currentInteractable;
        private INotifyStateChanged _currentNotifyStateChangedObject;

        public override void OnStartClient()
        {
            if (!isLocalPlayer)
                return;
        
            _input = INPUTE.instance;
            interactionSystem.OnInteractionStart += OnInteractionStarted;
            interactionSystem.OnInteractionStop += OnInteractionStopped;
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer)
                return;
            
            interactionSystem.OnInteractionStart -= OnInteractionStarted;
            interactionSystem.OnInteractionStop -= OnInteractionStopped;

            if (_currentInteractable!= null)
            {
                _input.OnPerformedGrab -= OnPerformedGrabButton;
            }
        }

        private void OnInteractionStarted(GameObject detectedObject)
        {
            if (detectedObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable is INotifyStateChanged interactableChange)
                {
                     _currentNotifyStateChangedObject = interactableChange;
                     _currentNotifyStateChangedObject.OnChange += OnChangeCurrentObject;
                }
                
                InteractableView.Instance.Show(interactable.GetPromt(character));
                
                if (interactable.CanInteract(character))
                {
                    _currentInteractable = interactable; 
                    _input.OnPerformedGrab += OnPerformedGrabButton;
                }
            }
        }

        private void OnChangeCurrentObject()
        {
            InteractableView.Instance.Show(_currentInteractable.GetPromt(character));
        }

        private void OnInteractionStopped()
        {
            _currentInteractable = null;
            
            InteractableView.Instance.Hide();

            if (_currentNotifyStateChangedObject != null)
            {
                _currentNotifyStateChangedObject.OnChange -= OnChangeCurrentObject;
                _currentNotifyStateChangedObject = null;
            }
            
            _input.OnPerformedGrab -= OnPerformedGrabButton;
        }
        
        private void OnPerformedGrabButton()
        {
            if (_currentInteractable.CanInteract(character))
                _currentInteractable.Interact(character);
        }
    }
}