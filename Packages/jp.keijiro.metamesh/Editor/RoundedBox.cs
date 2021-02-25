using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Metamesh
{

[System.Serializable]
public class RoundedBox
{
    public float Width = 1;
    public float Height = 1;
    public float Depth = 1;
    [Range(1, 10)] public int Divisions = 3;
    public float Radius = 0.1f;

    // Parameter distribution function used to calculate points on a edge.
    // It only makes points on rounded corners but not on flat surfaces.
    float GetEdgePoint(int i, float length)
      => i <= Divisions ?
            Radius / length * (i                    ) / Divisions :
        1 + Radius / length * (i - Divisions * 2 - 1) / Divisions;

    // Move a point onto a rounded curve. Used to bend a flat plane into a
    // rounded surface.
    (float3, float3) RoundPoint(float3 v)
    {
        var extent = math.float3(Width, Height, Depth) / 2;
        var anchor = math.sign(v) * (extent - Radius);
        var normal = math.normalize(v - anchor);
        return (anchor + normal * Radius, normal);
    }

    // Single plane construction function.
    List<(float3, float3, float2)> MakePlane(float4 ax, float4 ay, float3 offs)
    {
        var vc_edge = 2 + Divisions * 2;
        var vtx = new List<(float3, float3, float2)>();
        for (var iy = 0; iy < vc_edge; iy++)
        {
            var v = GetEdgePoint(iy, ay.w);
            var y = ay.xyz * (v - 0.5f) * ay.w;
            for (var ix = 0; ix < vc_edge; ix++)
            {
                var u = GetEdgePoint(ix, ax.w);
                var x = ax.xyz * (u - 0.5f) * ax.w;
                var (p, n) = RoundPoint(x + y + offs);
                vtx.Add((p, n, math.float2(u, v)));
            }
        }
        return vtx;
    }

    public void Generate(Mesh mesh)
    {
        var vc_edge = 2 + Divisions * 2;

        // Vertex array construction
        var vtx = new List<(float3 p, float3 n, float2 uv)>();
        vtx.AddRange(MakePlane(math.float4( 1, 0, 0, Width), math.float4(0,  1, 0, Height), math.float3(0, 0, -0.5f * Depth)));
        vtx.AddRange(MakePlane(math.float4(-1, 0, 0, Width), math.float4(0,  1, 0, Height), math.float3(0, 0,  0.5f * Depth)));
        vtx.AddRange(MakePlane(math.float4(0, 0,  1, Depth), math.float4(0, -1, 0, Height), math.float3(-0.5f * Width, 0, 0)));
        vtx.AddRange(MakePlane(math.float4(0, 0, -1, Depth), math.float4(0, -1, 0, Height), math.float3( 0.5f * Width, 0, 0)));
        vtx.AddRange(MakePlane(math.float4( 1, 0, 0, Width), math.float4(0, 0, -1, Depth), math.float3(0, -0.5f * Height, 0)));
        vtx.AddRange(MakePlane(math.float4(-1, 0, 0, Width), math.float4(0, 0, -1, Depth), math.float3(0,  0.5f * Height, 0)));

        // Index array construction
        var idx = new List<int>();
        var i = 0;
        for (var ip = 0; ip < 6; ip++)
        {
            for (var iy = 0; iy < vc_edge - 1; iy++, i++)
            {
                for (var ix = 0; ix < vc_edge - 1; ix++, i++)
                {
                    // Lower triangle
                    idx.Add(i);
                    idx.Add(i + vc_edge);
                    idx.Add(i + 1);
                    // Upper triangle
                    idx.Add(i + 1);
                    idx.Add(i + vc_edge);
                    idx.Add(i + vc_edge + 1);
                }
            }
            i += vc_edge;
        }

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v.p).ToList());
        mesh.SetNormals(vtx.Select(v => (Vector3)v.n).ToList());
        mesh.SetUVs(0, vtx.Select(v => (Vector2)v.uv).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
    }
}

}
