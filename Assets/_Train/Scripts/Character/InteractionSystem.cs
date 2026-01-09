using System;
using _Train.Scripts.Root;
using _Train.Scripts.UI;
using Mirror;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class InteractionSystem : NetworkBehaviour
    {
        public event Action<GameObject> OnInteractionStart;
        public event Action OnInteractionStop;
        
        [SerializeField] private float interactionDistance;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private Camera camera;
        
        private GameObject _lastInteractableObject;
        private GameObject _currentInteractableObject;

        private void FixedUpdate()
        {
            if (!isLocalPlayer) 
                return;
        
            RaycastHit hit;
        
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                if (_lastInteractableObject == null)
                {
                    _lastInteractableObject = hit.collider.gameObject;

                    OnInteractionStart?.Invoke(hit.collider.gameObject);

                    return;
                }

                if (hit.collider.gameObject != _lastInteractableObject)
                {
                    _lastInteractableObject = null;

                    OnInteractionStop?.Invoke();
                }
                
            }
            else if (_lastInteractableObject != null)
            {
                _lastInteractableObject = null;
                InteractableView.Instance.Hide();
                
                OnInteractionStop?.Invoke();
            }
        }
    }
}
