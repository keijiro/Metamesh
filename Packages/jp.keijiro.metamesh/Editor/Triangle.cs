using UnityEngine;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public sealed class Triangle
{
    public float3 Vertex1 = math.float3(0, 0, 0);
    public float3 Vertex2 = math.float3(0, 1, 0);
    public float3 Vertex3 = math.float3(1, 0, 0);
    public bool DoubleSided = false;

    public void Generate(Mesh mesh)
    {
        var (v1, v2, v3) = (Vertex1, Vertex2, Vertex3);
        var uv1 = Vector3.right;
        var uv2 = Vector3.up;
        var uv3 = Vector3.forward;

        var vtx = DoubleSided ?
          new Vector3 [] { v1, v2, v3, v1, v3, v2 } :
          new Vector3 [] { v1, v2, v3 };
        
        var uvs = DoubleSided ?
          new Vector3 [] { uv1, uv2, uv3, uv1, uv3, uv2 } :
          new Vector3 [] { uv1, uv2, uv3 };

        var idx = DoubleSided ?
          new [] { 0, 1, 2, 3, 4, 5 } :
          new [] { 0, 1, 2 };

        mesh.SetVertices(vtx);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
    }
}

} // namespace Metamesh
