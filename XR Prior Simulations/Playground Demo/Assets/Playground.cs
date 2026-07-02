using InteractionTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playground : MonoBehaviour
{
    public ITKInteractable[] interactables;
    public Rigidbody[] bodies;

    private Vector3[] positions;
    private Quaternion[] rotations;

    public bool itk = true;
    public float gravity = -9.81f;

    private void Start()
    {
        interactables = GetComponentsInChildren<ITKInteractable>();
        bodies = GetComponentsInChildren<Rigidbody>();

        positions = new Vector3[bodies.Length];
        rotations = new Quaternion[bodies.Length];

        for (int i = 0; i < bodies.Length; ++i)
        {
            positions[i] = bodies[i].transform.localPosition;
            rotations[i] = bodies[i].transform.localRotation;
        }
    }

    public void ResetTransforms()
    {
        for (int i = 0; i < bodies.Length; ++i)
        {
            bodies[i].transform.localPosition = positions[i];
            bodies[i].transform.localRotation = rotations[i];
            bodies[i].velocity = Vector3.zero;
            bodies[i].angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < bodies.Length; ++i)
        {
            bodies[i].velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
        }

        for (int i = 0; i < interactables.Length; ++i)
        {
            interactables[i].pinch = itk;
            interactables[i].grasp = itk;
        }
    }
}
