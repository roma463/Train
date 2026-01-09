using System;
using UnityEngine;

public class ArmAnimationEvents : MonoBehaviour
{
    public event Action OnDropItem; 
    
    [SerializeField] private SkinnedMeshRenderer localBagMeshRenderer;
    
    public void OnDropItemEvent()
    {
        OnDropItem?.Invoke();
    }
    
    public void ShowBag()
    {
        ChangeLocalMeshActivity(true);
    }

    public void HideBag()
    {
        ChangeLocalMeshActivity(false);
    }

    private void ChangeLocalMeshActivity(bool state)
    {
        localBagMeshRenderer.gameObject.SetActive(state);
    }
}
