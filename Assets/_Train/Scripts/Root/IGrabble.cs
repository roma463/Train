using UnityEngine;

namespace _Train.Scripts.Root
{
    public interface IGrabble
    {
        public string GrabAnimName { get; }
        public string DropAnimName { get; }
        
        public bool CanGrab();
        public bool CanThrow();
        public bool CanAttach();
        public void Grab(Character.Character character);
        public void Drop();
        public void Throw(Vector3 direction, float force);
    }
}