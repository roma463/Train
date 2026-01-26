using System.Collections.Generic;
using UnityEngine;
using _Train.Scripts.Character;
using _Train.Scripts.Train;

public class TrainAttachZone : MonoBehaviour
{
    [SerializeField] private Transform attachRoot;
    [SerializeField] private bool detachOnExit = true;
    
    [SerializeField] private TrainMotion trainMotion;
    
    [SerializeField] private bool onlyLocalPlayer = true;

    private readonly HashSet<Character> passengers = new();

    private void Reset()
    {
        attachRoot = transform.parent;

        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        if (!attachRoot) attachRoot = transform.parent;
        if (!trainMotion) trainMotion = GetComponentInParent<TrainMotion>();

        var col = GetComponent<Collider>();
        if (col && !col.isTrigger) col.isTrigger = true;
    }

    private void FixedUpdate()
    {
        if (!trainMotion || passengers.Count == 0) 
            return;

        foreach (var character in passengers)
        {
            if (!character) continue;

            if (onlyLocalPlayer && !character.isLocalPlayer)
                continue;

            character.SetExternalVelocity(trainMotion.Velocity);
            character.SetAngularVelocity(trainMotion.AngularVelocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var characterRoot = other.attachedRigidbody
            ? other.attachedRigidbody.transform
            : other.transform.root;

        var character = characterRoot.GetComponentInParent<Character>();
        if (!character) return;

        if (onlyLocalPlayer && !character.isLocalPlayer)
            return;
        
        passengers.Add(character);
        character.SetPassengers(true);
        
        Attach(character.transform.root);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!detachOnExit) return;

        if (other.TryGetComponent(out Character character))
        {
            if (onlyLocalPlayer && !character.isLocalPlayer)
                return;

            passengers.Remove(character);
            character.SetPassengers(false);
            
            character.SetExternalVelocity(Vector3.zero);

            Detach(character.transform);
        }
    }

    private void OnDisable()
    {
        foreach (var ch in passengers)
        {
            if (!ch) continue;
            ch.SetExternalVelocity(Vector3.zero);
        }
        passengers.Clear();
    }

    private void Attach(Transform characterRoot)
    {
        if (!attachRoot) return;
        if (characterRoot.parent == attachRoot) return;

        characterRoot.SetParent(attachRoot, true);
    }

    private void Detach(Transform characterRoot)
    {
        if (!attachRoot) return;
        if (characterRoot.parent != attachRoot) return;

        characterRoot.SetParent(null, true);
    }
}
