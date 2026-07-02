using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    private Vector3 initialVelocity;
    private float gravity;
    private Vector3 launchPoint;
    private float time = 0;
    ProjectileLauncher launcherScript;
    public bool targetHit = false;

    public ParticleSystem explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {

        GameObject launcherObject = GameObject.FindGameObjectWithTag("launcher");

        if (launcherObject != null)
        {
            launcherScript = launcherObject.GetComponent<ProjectileLauncher>();

            if (launcherScript != null)
            {
                initialVelocity = launcherScript.startVelocity;
                gravity = ProjectileLauncher.gravity;
                launchPoint = launcherScript.origin;
                transform.position = launchPoint;
            }
        }
    }

    void Update()
    {
        //Calculate the position using the parabolic motion equation
        //Vector3 newPosition = launchPoint + initialVelocity * time + 0.5f * Vector3.down * gravity * time * time;

        if (!targetHit)
        {
            Vector3 AdjustedLaunchPoint = launcherScript.AdjustedOrigin;

            var x = (initialVelocity.x * time);
            var y = (initialVelocity.y * time) - (gravity / 2 * time * time);
            Vector3 point = new Vector3(x, y, 0);
            Vector3 newPlace = AdjustedLaunchPoint + point;
            transform.position = launcherScript.UntransformCoords(newPlace);
            time += Time.deltaTime*0.5f;
        }
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            targetHit = true;
            Explode();
            Destroy(gameObject);
            UnityEngine.Debug.Log("COLLISION HIT");
            
        }
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}

