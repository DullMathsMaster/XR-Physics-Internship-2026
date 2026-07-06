using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundary : MonoBehaviour
{
    public float maxRadius = 500f;

    // Update is called once per frame
    void LateUpdate() {
        Vector3 centreToPlayer = transform.position;

        if (centreToPlayer.magnitude > maxRadius) {
            transform.position = centreToPlayer.normalized * maxRadius;
        }
    }
}
