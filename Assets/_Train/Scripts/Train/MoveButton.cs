using System;
using _Train.Scripts.Character;
using _Train.Scripts.Root;
using UnityEngine;

public class MoveButton : MonoBehaviour, IInteractable
{
    [SerializeField] private TrainMotion train;
    [SerializeField] private float targetSpeed = 10f;
    
    //private bool isInteracting;
    
    private bool isMoving;
    
    public Transform RootTransform => transform;
    
    
    private void Awake()
    {
        if (!train) 
            GetComponentInParent<TrainMotion>();
        // trainRb.isKinematic = true; // тут что то для того, что бы физика не толкала поезд. Не шарю. Но на всякий сохранил
        // trainRb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public string GetPromt(Character character)
    {
        return isMoving? "Stop" : "Start";   
    }

    public bool CanInteract(Character character)
    {
        return true;
    }

    public void Interact(Character character)
    {
        isMoving = !isMoving;
        if (isMoving)
            train.SetTargetSpeed(targetSpeed);
        else
            train.SetTargetSpeed(0f);
    }
}
