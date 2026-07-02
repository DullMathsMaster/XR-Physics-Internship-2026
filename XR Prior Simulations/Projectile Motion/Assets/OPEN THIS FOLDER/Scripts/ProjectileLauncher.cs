using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Diagnostics;


using Unity.VisualScripting;
using System.Linq.Expressions;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public static float launchSpeed = 3f;
    public static float gravity = 9.81f;

    [Header("****Trajectory Display****")]
    public LineRenderer lineRenderer;
    public float timeIntervalInPoints = 0.01f;
    public int linePoints;
    //public BoxCollider boxCollider; //added
    public MeshCollider meshCollider;
    //public float CanonXSize = 0.14f; // added, in m
    //public float CanonYSize = 0.03f;

    public Vector3 origin;
    public Vector3 AdjustedOrigin;
    public Vector3 startVelocity;
    public GameObject visualScriptsObject;

    public Vector3[] startingLineRendererPoints = null;


    //public int linePoints = (2 * launchSpeed) / (gravity * timeIntervalInPoints);

    private void Start()
    {

        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            // Use the meshCollider variable to access size properties
            Bounds colliderBounds = meshCollider.bounds;
            Vector3 colliderSize = colliderBounds.size;

            UnityEngine.Debug.Log("Cylinder Size: " + colliderSize);
        }
        else
        {
            // Handle the case where the Mesh Collider component is not found
            UnityEngine.Debug.Log("Mesh Collider component not found on this GameObject.");
        }



        // Initialize linePoints
        linePoints = Mathf.CeilToInt((2 * launchSpeed) / (gravity * timeIntervalInPoints));
        lineRenderer.positionCount = linePoints;
        startingLineRendererPoints = new Vector3[linePoints];
        //visualScriptsObject = GameObject.FindWithTag("VisualScripts");
        visualScriptsObject = GameObject.Find("VisualScripts");       
        //ScaleFactor = boxCollider.size.y / CanonSize

    }

    public void SetG(float value)
    {
        gravity = value;
    }

    public void Setv(float value2)
    {
        launchSpeed = value2;
    }

    public void LaunchProjectile()
    {
        var _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
        //_projectile.GetComponent<Rigidbody>().velocity = launchSpeed * launchPoint.up;
    }

    public void DrawTrajectoryOld()
    { 
        origin = launchPoint.position;

        //Vector3 translatedorigin = TranslateCoords(origin);
        //Vector3 rotatedorigin = RotateCoords(translatedorigin);

        startVelocity = launchSpeed * launchPoint.up;       
        float time = 0;
        Vector3 point = new Vector3(999999, 999999, 999999);
        for (int i = 0; i<linePoints; i++)
        {
            var x = (startVelocity.x * time);
            var y = (startVelocity.y * time) - (gravity / 2 * time * time);

            // d = s*t
            //var x = (startVelocity.x * time);
            //var y = (startVelocity.y * time);

            // s = u*t + 1/2*g*t*t
            point = new Vector3(x, y, 0);

            lineRenderer.SetPosition(i, origin + point);
            time += timeIntervalInPoints;
        }
      
    }

    public void DrawTrajectory()
    {

        AdjustedOrigin = TransformCoords(launchPoint.position);
        startVelocity = launchSpeed * launchPoint.up;
        lineRenderer.positionCount = linePoints;
        float time = 0;
        Vector3 point = new Vector3(999999, 999999, 999999);
        Vector3 nextPlace = new Vector3(999999, 999999, 999999);
        for (int i = 0; i < linePoints; i++)
        {
            var x = (startVelocity.x * time);
            var y = (startVelocity.y * time) - (gravity / 2 * time * time);

            // d = s*t
            //var x = (startVelocity.x * time);
            //var y = (startVelocity.y * time);

            // s = u*t + 1/2*g*t*t
            point = new Vector3(x, y, 0);
            nextPlace = AdjustedOrigin + point;

            //lineRenderer.SetPosition(i, detranslatedplace);
            lineRenderer.SetPosition(i, UntransformCoords(nextPlace));
            //lineRenderer.SetPosition(i, origin + point);
            time += timeIntervalInPoints;
        }
        lineRenderer.GetPositions(startingLineRendererPoints);
        DisplayEquations();
    }

    public void LimitLineRenderer()
    {
        bool hitSomething = false;

        if (lineRenderer)
        {
            RaycastHit hitInfo;
            Vector3[] newPointsInLine = null;
            for (int i = 0; i < startingLineRendererPoints.Length-1; i++)
            {
                if (Physics.Linecast(startingLineRendererPoints[i], startingLineRendererPoints[i+1], out hitInfo))
                {
                    UnityEngine.Debug.Log("Line cast between "+i+" "+ startingLineRendererPoints[i]+" and "+ (i+1)+" "+ startingLineRendererPoints[i+1]);
                    newPointsInLine = new Vector3[(i + 1) + 1];
                    for (int j =0; j < newPointsInLine.Length; j++)
                    {
                        newPointsInLine[j] = startingLineRendererPoints[j];
                    }

                    newPointsInLine[i + 1] = hitInfo.point;
                    hitSomething = true;
                    break;
                }
            }
            if (hitSomething)
            {
                lineRenderer.positionCount = newPointsInLine.Length;
                lineRenderer.SetPositions(newPointsInLine);
            }
            else
            {
                lineRenderer.positionCount = startingLineRendererPoints.Length;
                lineRenderer.SetPositions(startingLineRendererPoints);
            }
        }
    }


    public Vector3 TranslateCoords(Vector3 Googlevector)
    {
        Vector3 googleLaunchPosition = (Vector3)Variables.Object(visualScriptsObject).Get("launcherPosition");
        Vector3 TranslatedVector = Googlevector - googleLaunchPosition;
        
        return TranslatedVector;
    }

    public Vector3 RotateCoords (Vector3 Translatedvector)
    {
        Vector3 Googlelaunch = (Vector3)Variables.Object(visualScriptsObject).Get("launcherPosition"); //change to launch POINT position
        Vector3 Googletarget = (Vector3)Variables.Object(visualScriptsObject).Get("targetPosition");
        Vector3 Translatedtarget = Googletarget - Googlelaunch;
        //float rotationAngle = CalculateAngle(Googlelaunch, Googletarget);
        float rotationAngle = CalculateAngle(Translatedtarget, new Vector3 (1f,0,0));
        Variables.Object(visualScriptsObject).Set("Angle", rotationAngle);

        Vector3 rotatedVector = Quaternion.Euler(0, -rotationAngle, 0) * Translatedvector;
        // rotatedVector = Quaternion.AngleAxis(rotationAngle, Vector3.up) * Translatedvector;//CHECK SYNTAX

        //Vector3 rotatedVector = Translatedvector + new Vector3 (1f,1f,1f); //CHECK SYNTAX
        //Vector3 rotatedVector = GooglelaunchPosition + GoogletargetPosition; //CHECK SYNTAX

        //return new Vector3 (Googlelaunch.x, Googletarget.x, rotationAngle) ;
        return rotatedVector;
    }

    public Vector3 TransformCoords(Vector3 VectortoTransform)
    {
        //Vector3 Googlelaunch = (Vector3)Variables.Object(visualScriptsObject).Get("launcherPosition"); //change to launch POINT position
        Vector3 Googlelaunch = launchPoint.position;
        Vector3 Googletarget = (Vector3)Variables.Object(visualScriptsObject).Get("targetPosition");
        Vector3 TranslatedTarget = Googletarget - Googlelaunch;

        Vector3 TranslatedVector = VectortoTransform - Googlelaunch; 
        float rotationAngle = CalculateAngle(TranslatedTarget, new Vector3(1f, 0, 0));
        Variables.Object(visualScriptsObject).Set("Angle", rotationAngle);

        Vector3 TransformedVector = Quaternion.Euler(0, -rotationAngle, 0) * TranslatedVector;
        return TransformedVector;
    }

    public Vector3 UntransformCoords(Vector3 VectortoUnTransform)
    {
        //Vector3 Googlelaunch = (Vector3)Variables.Object(visualScriptsObject).Get("launcherPosition"); //change to launch POINT position
        Vector3 Googlelaunch = launchPoint.position;
        Vector3 Googletarget = (Vector3)Variables.Object(visualScriptsObject).Get("targetPosition");
        Vector3 TranslatedTarget = Googletarget - Googlelaunch;

        float rotationAngle = CalculateAngle(TranslatedTarget, new Vector3(1f, 0, 0));
        Variables.Object(visualScriptsObject).Set("Angle", rotationAngle);
        Vector3 TranslatedVector = Quaternion.Euler(0, rotationAngle, 0) * VectortoUnTransform;

        Vector3 GoogleVector = TranslatedVector + Googlelaunch;

        return GoogleVector;
    }

    public void DebugTranslate()
    {
        visualScriptsObject = GameObject.FindWithTag("VisualScripts");


        Vector3 TargetPosition = (Vector3)Variables.Object(visualScriptsObject).Get("targetPosition");
        Vector3 TransformedTargetPosition = TransformCoords(TargetPosition);
        Variables.Object(visualScriptsObject).Set("translatedTargetPosition", TransformedTargetPosition);

        Vector3 LauncherPosition = (Vector3)Variables.Object(visualScriptsObject).Get("launcherPosition");
        Vector3 TransformedLauncherPosition = TransformCoords(LauncherPosition);
        Vector3 UnTransformedLauncherPosition = UntransformCoords(TransformedLauncherPosition);
        Variables.Object(visualScriptsObject).Set("translatedLauncherPosition", UnTransformedLauncherPosition);

    }

    public float CalculateAngle(Vector3 point1, Vector3 point2)
    {
        Vector2 direction1 = new Vector2(point1.x, point1.z);
        Vector2 direction2 = new Vector2(point2.x, point2.z);

        float angle = Vector2.SignedAngle(direction1, direction2);
        return angle;
    }

    public void DisplayEquations()
    {
        //string Equation = "s = " + launchSpeed + "t - " + gravity + "t<sup>2</sup>";
        //string Equation = $"s = {startVelocity.y}t - {gravity / 2f}t\u00B2";
        Vector3 rotationAngles = launchPoint.rotation.eulerAngles;
        float angle = (450 - rotationAngles.z) % 360;
        string YEquation = $"y = {launchSpeed:F1}sin({angle:F0})t - \u00BD({gravity:F1})t\u00B2";
        string XEquation = $"x = {launchSpeed:F1}cos({angle:F0})t";
        Variables.Object(visualScriptsObject).Set("GlobalYEquation", YEquation);
        Variables.Object(visualScriptsObject).Set("GlobalXEquation", XEquation);
    }
}
