using InteractionTK.HandTracking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VirtualRealityTK;

namespace InteractionTK.Menus
{
    public class MTKPlaygroundConsole : MonoBehaviour
    {
        public GameObject center;

        public Playground playground;

        public MTKButton toggleITK;
        public MTKButton reset;
        public MTKButton earth;
        public MTKButton moon;
        public MTKButton jupiter;
        public MTKButton none;

        private void Start()
        {
            none.OnClick.AddListener(() => playground.gravity = 0);
            moon.OnClick.AddListener(() => playground.gravity = -1.62f);
            earth.OnClick.AddListener(() => playground.gravity = -9.81f);
            jupiter.OnClick.AddListener(() => playground.gravity = -24.79f);

            toggleITK.OnClick.AddListener(() =>
            {
                playground.itk = !playground.itk;
                if (playground.itk) { toggleITK.tmp.text = "Disable ITK"; }
                else { toggleITK.tmp.text = "Enable ITK"; }
            });

            reset.OnClick.AddListener(() => { playground.transform.rotation = Quaternion.Euler(0, VRTK.master.main.transform.rotation.eulerAngles.y, 0);  playground.transform.position = new Vector3(VRTK.master.main.transform.position.x, 0, VRTK.master.main.transform.position.z); playground.ResetTransforms(); });
        }

        private float distance = 0.5f;
        private Vector3 dir = Vector3.forward;
        private bool update = true;
        private void FixedUpdate()
        {
            if (!update) return;
            if (dir == Vector3.zero) dir = Vector3.forward;
            transform.position = VRTK.master.main.transform.position + dir.normalized * distance;
            transform.rotation = Quaternion.LookRotation(center.transform.position - VRTK.master.main.transform.position);
        }

        public void OnMove(ITKInteractable interactable, ITKHandController controller)
        {
            update = false;

            dir = transform.position - VRTK.master.main.transform.position;
            if (dir == Vector3.zero) dir = Vector3.forward;
            distance = Mathf.Clamp(dir.magnitude, 0.05f, 1f);

            transform.position = VRTK.master.main.transform.position + dir.normalized * distance;
            transform.rotation = Quaternion.LookRotation(center.transform.position - VRTK.master.main.transform.position);
        }

        public void OnMove(ITKPinchInteractable interactable, ITKPinchController controller)
        {
            update = false;

            dir = transform.position - VRTK.master.main.transform.position;
            if (dir == Vector3.zero) dir = Vector3.forward;
            distance = Mathf.Clamp(dir.magnitude, 0.1f, 1f);

            transform.position = VRTK.master.main.transform.position + dir.normalized * distance;
            transform.rotation = Quaternion.LookRotation(center.transform.position - VRTK.master.main.transform.position);
        }

        public void OnStopMove(ITKInteractable interactable, ITKHandController controller)
        {
            update = true;
        }

        public void OnStopMove(ITKPinchInteractable interactable, ITKPinchController controller)
        {
            update = true;
        }
    }
}