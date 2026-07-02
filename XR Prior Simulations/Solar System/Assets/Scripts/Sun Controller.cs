using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SunController : MonoBehaviour
{
    public GameObject sunObject;
    public float mass = 1.989e+30f;
    public float radius = 6.9634e+08f;
    public float colliderRadius;

    public static float sunScaleFactor = 1e-8f;

    private SphereCollider sphereCollider;

    private float scaledRadius;
    private float scale;


    // Start is called before the first frame update
    void Start()
    {
        CalculateScaledVariables();
        transform.position = Vector3.zero;
        sunObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Calculate Scaled Variables
    private void CalculateScaledVariables()
    {
        scaledRadius = radius * sunScaleFactor;
        colliderRadius = sphereCollider.radius;
    }
}
