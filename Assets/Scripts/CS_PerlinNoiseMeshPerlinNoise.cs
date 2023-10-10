using UnityEngine;
using System.Collections;

public class PerlinNoiseMesh : MonoBehaviour
{
    [Range(1, 255)]
    public int size;
    public float vertexDistance = 1f;
    public Material material;
    public PhysicMaterial physicMaterial;

    public PerlinNoiseProperty[] perlinNoiseProperty = new PerlinNoiseProperty[1];
    [System.Serializable]
    public class PerlinNoiseProperty
    {
        public float heightMultiplier = 1f;
        public float scale = 1f;
        public Vector2 offset;
    }

    void Start()
    {
        CreateMesh();
    }

    void CreateMesh()
    {
        Vector3[] vertices = new Vector3[size * size];
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {

                float sampleX;
                float sampleZ;
                float y = 0;
                foreach (PerlinNoiseProperty p in perlinNoiseProperty)
                {
                    p.scale = Mathf.Max(0.0001f, p.scale);
                    sampleX = (x + p.offset.x) / p.scale;
                    sampleZ = (z + p.offset.y) / p.scale;
                    y += Mathf.PerlinNoise(sampleX, sampleZ) * p.heightMultiplier;
                }

                vertices[z * size + x] = new Vector3(x * vertexDistance, y, -z * vertexDistance);
            }
        }

        int triangleIndex = 0;
        int[] triangles = new int[(size - 1) * (size - 1) * 6];
        for (int z = 0; z < size - 1; z++)
        {
            for (int x = 0; x < size - 1; x++)
            {

                int a = z * size + x;
                int b = a + 1;
                int c = a + size;
                int d = c + 1;

                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;

                triangles[triangleIndex + 3] = c;
                triangles[triangleIndex + 4] = b;
                triangles[triangleIndex + 5] = d;

                triangleIndex += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (!meshCollider) meshCollider = gameObject.AddComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshCollider.sharedMesh = mesh;
        meshCollider.sharedMaterial = physicMaterial;
    }

    void OnValidate()
    {
        CreateMesh();
    }
}