using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CoordinateRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float CalculateAngle (Vector3 point1, Vector3 point2)
    {
        Vector2 direction1 = new Vector2(point1.x, point1.z);
        Vector2 direction2 = new Vector2(point2.x, point2.z);

        float angle = Vector2.SignedAngle(direction1, direction2);
        return angle;
    }

}
