using System.Collections.Generic;
using System.Linq;
using Metamesh.Smoothing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Unity.Mathematics;

namespace Metamesh
{

[System.Serializable]
public sealed class Cylinder
{
    [FormerlySerializedAs("Radius")] public float TopRadius = 1;
    [FormerlySerializedAs("Radius")] public float BottomRadius = 1;
    public float Height = 1;
    public uint Columns = 24;
    public uint Rows = 12;
    public Axis Axis = Axis.Y;
    public bool Caps = true;
    public SmoothingSettings SmoothingSettings;

    public void Generate(Mesh mesh)
    {
        // Parameter sanitization
        var res = math.int2((int)Columns, (int)Rows);
        res = math.max(res, math.int2(3, 1));

        // Axis selection
        var va = float3.zero;
        var vx = float3.zero;

        var ai = (int)Axis;

        va[(ai + 0) % 3] = 1;
        vx[(ai + 1) % 3] = 1;

        // Normal vector for the first vertex
        var edge = (TopRadius - BottomRadius) * vx + Height * va;
        var n0 = math.normalize(math.cross(math.cross(va, vx), edge));

        // Vertex array
        var vtx = new List<float3>();
        var nrm = new List<float3>();
        var uv0 = new List<float2>();

        // (Body vertices)
        for (var iy = 0; iy < res.y + 1; iy++)
        {
            for (var ix = 0; ix < res.x + 1; ix++)
            {
                var u = (float)ix / res.x;
                var v = (float)iy / res.y;

                var r = math.lerp(BottomRadius, TopRadius, v);
                var rot = quaternion.AxisAngle(va, u * math.PI * -2);
                var n = math.mul(rot, n0);
                var p = math.mul(rot, vx) * r + va * (v - 0.5f) * Height;

                vtx.Add(p);
                nrm.Add(n);
                uv0.Add(math.float2(u, v));
            }
        }

        // (End cap vertices)
        if (Caps)
        {
            vtx.Add(va * Height / -2);
            vtx.Add(va * Height / +2);

            nrm.Add(-va);
            nrm.Add(+va);

            uv0.Add(math.float2(0.5f, 0.5f));
            uv0.Add(math.float2(0.5f, 0.5f));

            for (var ix = 0; ix < res.x; ix++)
            {
                var u = (float)ix / res.x * math.PI * 2;

                var rot = quaternion.AxisAngle(va, -u);
                var p = math.mul(rot, vx);

                vtx.Add(p * BottomRadius + va * Height / -2);
                vtx.Add(p * TopRadius + va * Height / +2);

                nrm.Add(-va);
                nrm.Add(+va);

                uv0.Add(math.float2(math.cos(-u), math.sin(-u)) / 2 + 0.5f);
                uv0.Add(math.float2(math.cos(+u), math.sin(+u)) / 2 + 0.5f);
            }
        }

        // Index array
        var idx = new List<int>();
        var i = 0;
        var smoothingProcessor = new SmoothingProcessorUv<float3, float2>(SmoothingSettings, vtx, uv0);

        // (Body indices)
        for (var iy = 0; iy < res.y; iy++, i++)
        {
            for (var ix = 0; ix < res.x; ix++, i++)
            {
                smoothingProcessor.AddIndex(idx, i);
                smoothingProcessor.AddIndex(idx, i + res.x + 1);
                smoothingProcessor.AddIndex(idx, i + 1);

                smoothingProcessor.AddIndex(idx, i + 1);
                smoothingProcessor.AddIndex(idx, i + res.x + 1);
                smoothingProcessor.AddIndex(idx, i + res.x + 2);
            }
        }

        // (End cap indices)
        if (Caps)
        {
            i += res.x + 1;

            for (var ix = 0; ix < (res.x - 1) * 2; ix += 2)
            {
                smoothingProcessor.AddIndex(idx, i);
                smoothingProcessor.AddIndex(idx, i + ix + 2);
                smoothingProcessor.AddIndex(idx, i + ix + 4);

                smoothingProcessor.AddIndex(idx, i + 1);
                smoothingProcessor.AddIndex(idx, i + ix + 5);
                smoothingProcessor.AddIndex(idx, i + ix + 3);
            }

            smoothingProcessor.AddIndex(idx, i);
            smoothingProcessor.AddIndex(idx, i + res.x * 2);
            smoothingProcessor.AddIndex(idx, i + 2);

            smoothingProcessor.AddIndex(idx, i + 1);
            smoothingProcessor.AddIndex(idx, i + 3);
            smoothingProcessor.AddIndex(idx, i + 1 + res.x * 2);
        }

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());

        mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        mesh.WriteNormals(SmoothingSettings, m => m.SetNormals(nrm.Select(v => (Vector3)v).ToList()));
    }
}

} // namespace Metamesh
