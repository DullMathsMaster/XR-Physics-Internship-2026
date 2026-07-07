using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace FieldsToolkit
{
    public class FTKPole : MonoBehaviour
    {
        public static List<FTKPole> poles = new List<FTKPole>();
        private static Queue<LineRenderer> renderers = new Queue<LineRenderer>();

        public float strength = 1;
        public bool renderNeg = false;
        public bool renderPos = false;

        public bool simulate = false;

        public Rigidbody body;
        private bool set = false;

        private Vector3 prev;
        public Vector3 position { get => prev; }

        private class Line
        {
            public bool completed = false;
            public bool bufferEnd = false;
            public int count = 0;
            public int current = 1;
            public float step = FTK.settings.step;
            public Vector3 last;
        }

        private List<LineRenderer> lines = new List<LineRenderer>();
        private List<LineRenderer> linebuffer = new List<LineRenderer>();
        private List<Line> lineData = new List<Line>();

        private LineRenderer GetLineRenderer()
        {
            LineRenderer r;
            if (renderers.Count > 0) r = renderers.Dequeue();
            else
            {
                GameObject l = new GameObject();
                l.name = "Field line";
                r = l.AddComponent<LineRenderer>();
                r.material = FieldsToolkit.instance.fieldLineMaterial;
            }

            r.transform.parent = transform;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0, 0.003f);
            r.widthCurve = curve;
            
            return r;
        }

        public void OnEnable()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].enabled = true;
                linebuffer[i].enabled = false;
            }
        }

        public void OnDisable()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].enabled = false;
                linebuffer[i].enabled = false;
            }
        }

        public void Start()
        {
            poles.Add(this);

            prev = transform.position;

            _ResetLines();
        }

        public void OnDestroy()
        {
            poles.Remove(this);
        }

        private static void ResetLine(FTKPole p)
        {
            p.prev = p.transform.position;

            while (p.lines.Count < FTK.settings.numLinesPerRing * FTK.settings.numRings)
            {
                p.lines.Add(p.GetLineRenderer());
                p.linebuffer.Add(p.GetLineRenderer());
                p.lineData.Add(new Line());
            }
            while (p.lines.Count > FTK.settings.numLinesPerRing * FTK.settings.numRings)
            {
                p.lines[0].gameObject.SetActive(false);
                p.linebuffer[0].gameObject.SetActive(false);
                renderers.Enqueue(p.lines[0]);
                renderers.Enqueue(p.linebuffer[0]);
                p.lines.RemoveAt(0);
                p.linebuffer.RemoveAt(0);
                p.lineData.RemoveAt(0);
            }

            p.marked = false;

            int idx = 0;
            for (int i = 0; i < FTK.settings.numRings; ++i)
            {
                for (int j = 0; j < FTK.settings.numLinesPerRing; ++j, ++idx)
                {
                    p.lineData[idx] = new Line();
                    p.lineData[idx].last = p.transform.position + p.transform.rotation * Quaternion.Euler(j * 360f / FTK.settings.numLinesPerRing, i * 360f / FTK.settings.numRings, 0) * Vector3.forward * 0.025f;
                }
            }
        }

        public static void _ResetLines()
        {
            totalCompleted = 0;
            for (int i = 0; i < poles.Count; ++i)
            {
                ResetLine(poles[i]);
            }
        }
        private void ResetLines()
        {
            if (!triggered)
            {
                triggered = true;
                totalCompleted = 0;
                _ResetLines();
            }
        }

        private static bool triggered = false;
        private static int totalCompleted = 0;
        private bool marked = false;

        private Vector3[] buffer = new Vector3[50];
        public void FixedUpdate()
        {
            triggered = false;

            float tolerance = 0.001f;
            if (body.velocity.x < tolerance && body.velocity.x > -tolerance) body.velocity = new Vector3(0, body.velocity.y, body.velocity.z);
            if (body.velocity.y < tolerance && body.velocity.y > -tolerance) body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            if (body.velocity.z < tolerance && body.velocity.z > -tolerance) body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);

            bool render = strength < 0 && renderNeg || strength >= 1 && renderPos;

            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].enabled = render;
                linebuffer[i].enabled = false;

                if (strength < 0)
                    lines[i].material.color = Color.red;
                else
                    lines[i].material.color = Color.blue;
            }

            /*if (renderNeg || renderPos)
            {
                if (body != null)
                {
                    set = true;
                    body.isKinematic = true;
                }
            }*/

            if (render)
            {
                if (totalCompleted == poles.Where(l => l.strength < 0 && l.renderNeg || l.strength >= 1 && l.renderPos).Count() && poles.Any(l => l.prev != l.transform.position)) 
                    ResetLines();

                if (!lineData.Any(l => !l.completed && !l.bufferEnd) && !marked)
                {
                    marked = true;
                    ++totalCompleted;

                    List<LineRenderer> temp = lines;
                    lines = linebuffer;
                    linebuffer = temp;
                }

                if (lineData.Any(l => !l.completed && !l.bufferEnd))
                {
                    int idx = 0;
                    int count;
                    for (int i = 0; i < FTK.settings.numRings; ++i)
                    {
                        for (int j = 0; j < FTK.settings.numLinesPerRing; ++j, ++idx)
                        {
                            if (lineData[idx].completed || lineData[idx].bufferEnd)
                            {
                                linebuffer[idx].positionCount = lineData[idx].count;
                                continue;
                            }

                            buffer[lineData[idx].current - 1] = lineData[idx].last;
                            lineData[idx].completed = FTK.Solve(buffer, Mathf.Sign(strength), lineData[idx].step, FTK.settings.tolerance, poles, out lineData[idx].step, out count, lineData[idx].current, FTK.settings.iterationsPerFrame);
                            lineData[idx].count = count;
                            lineData[idx].bufferEnd = lineData[idx].count == buffer.Length;
                            if (linebuffer[idx].positionCount < count) linebuffer[idx].positionCount = count;
                            for (--lineData[idx].current; lineData[idx].current < count; ++lineData[idx].current)
                                linebuffer[idx].SetPosition(lineData[idx].current, buffer[lineData[idx].current]);
                            lineData[idx].last = buffer[lineData[idx].current - 1];
                        }
                    }
                }
            }
            /*else
            {
                if (body != null)
                {
                    if (set)
                    {
                        set = false;
                        body.isKinematic = false;
                    }

                    if (simulate) body.AddForceAtPosition(FTK.ForceOnPole(this, poles), transform.position);
                }
            }*/

            if (simulate) body.AddForceAtPosition(FTK.ForceOnPole(this, poles), transform.position);
        }
    }
}
