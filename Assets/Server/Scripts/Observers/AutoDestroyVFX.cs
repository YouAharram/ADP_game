using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f;
    
    void Start()
    {
        Debug.Log("AutoDestroyVFX: Destroying VFX after " + lifetime + " seconds.");
        Destroy(gameObject, lifetime);
    }
}