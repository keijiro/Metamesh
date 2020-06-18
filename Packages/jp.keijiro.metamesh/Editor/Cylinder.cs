using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public sealed class Cylinder
{
    public float Radius = 1;
    public float Height = 1;
    public uint Columns = 24;
    public uint Rows = 12;
    public Axis Axis = Axis.Y;
    public bool Caps = true;

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

                var rot = quaternion.AxisAngle(va, u * math.PI * -2);
                var n = math.mul(rot, vx);
                var p = n * Radius + va * (v - 0.5f) * Height;

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
                var p = math.mul(rot, vx) * Radius;

                vtx.Add(p + va * Height / -2);
                vtx.Add(p + va * Height / +2);

                nrm.Add(-va);
                nrm.Add(+va);

                uv0.Add(math.float2(math.cos(-u), math.sin(-u)) / 2 + 0.5f);
                uv0.Add(math.float2(math.cos(+u), math.sin(+u)) / 2 + 0.5f);
            }
        }

        // Index array
        var idx = new List<int>();
        var i = 0;

        // (Body indices)
        for (var iy = 0; iy < res.y; iy++, i++)
        {
            for (var ix = 0; ix < res.x; ix++, i++)
            {
                idx.Add(i);
                idx.Add(i + res.x + 1);
                idx.Add(i + 1);

                idx.Add(i + 1);
                idx.Add(i + res.x + 1);
                idx.Add(i + res.x + 2);
            }
        }

        // (End cap indices)
        if (Caps)
        {
            i += res.x + 1;

            for (var ix = 0; ix < (res.x - 1) * 2; ix += 2)
            {
                idx.Add(i);
                idx.Add(i + ix + 2);
                idx.Add(i + ix + 4);

                idx.Add(i + 1);
                idx.Add(i + ix + 5);
                idx.Add(i + ix + 3);
            }

            idx.Add(i);
            idx.Add(i + res.x * 2);
            idx.Add(i + 2);

            idx.Add(i + 1);
            idx.Add(i + 3);
            idx.Add(i + 1 + res.x * 2);
        }

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());
        mesh.SetNormals(nrm.Select(v => (Vector3)v).ToList());
        mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
    }
}

}
