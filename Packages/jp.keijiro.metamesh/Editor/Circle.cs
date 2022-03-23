using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public sealed class Ring
{
    public float Radius = 1;
    public float Width = 0.1f;
    [Range(0, 1)] public float Angle = 1;
    public uint Segments = 32;
    public Axis Axis = Axis.Z;
    public bool DoubleSided = false;

    public void Generate(Mesh mesh)
    {
        // Parameter sanitization
        var ext = math.min(Radius, Width / 2);

        // Axis selection
        var X = float3.zero;
        var Y = float3.zero;
        var ai = (int)Axis;
        X[(ai + 1) % 3] = 1;
        Y[(ai + 2) % 3] = 1;

        // Vertex array
        var vtx = new List<float3>();
        var uv0 = new List<float2>();

        var i_div_o = (Radius - Width / 2) / (Radius + Width / 2);

        for (var i = 0; i < Segments; i++)
        {
            var phi = 2 * math.PI * Angle * ((float)i / (Segments - 1) - 0.5f);
            var (x, y) = (math.cos(phi), math.sin(phi));

            var v = x * X + y * Y;
            vtx.Add(v * (Radius - ext));
            vtx.Add(v * (Radius + ext));
            uv0.Add(math.float2(-x, y) / 2 * i_div_o + 0.5f);
            uv0.Add(math.float2(-x, y) / 2 + 0.5f);
        }

        if (DoubleSided)
        {
            vtx = vtx.Concat(vtx).ToList();
            uv0 = uv0.Concat(uv0).ToList();
        }

        // Index array
        var idx = new List<int>();
        var n = (int)Segments;

        for (var i = 0; i < n - 1; i++)
        {
            idx.Add(i * 2);
            idx.Add(i * 2 + 1);
            idx.Add(i * 2 + 2);

            idx.Add(i * 2 + 1);
            idx.Add(i * 2 + 3);
            idx.Add(i * 2 + 2);
        }

        if (DoubleSided)
        {
            for (var i = 0; i < n - 1; i++)
            {
                idx.Add((n + i) * 2);
                idx.Add((n + i) * 2 + 2);
                idx.Add((n + i) * 2 + 1);

                idx.Add((n + i) * 2 + 1);
                idx.Add((n + i) * 2 + 2);
                idx.Add((n + i) * 2 + 3);
            }
        }

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());
        mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
    }
}

} // namespace Metamesh
