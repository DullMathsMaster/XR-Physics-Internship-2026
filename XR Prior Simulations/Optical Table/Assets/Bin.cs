using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    public static Bin instance;

    public HashSet<Collider> colliders = new HashSet<Collider>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
}