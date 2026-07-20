using UnityEngine;

public class InvertSphere : MonoBehaviour
{
    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            Mesh mesh = filter.mesh;
            Vector3[] normals = mesh.normals;
            
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i];
                    triangles[i] = triangles[i + 2];
                    triangles[i + 2] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }
}