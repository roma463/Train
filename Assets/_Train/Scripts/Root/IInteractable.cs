using UnityEngine;

namespace _Train.Scripts.Root
{
    public interface IInteractable
    {
        public Transform RootTransform { get; }

        public string GetPromt(Character.Character character);

        public bool CanInteract(Character.Character character);

        public void Interact(Character.Character character);
    }
}
