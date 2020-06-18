using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public class Sphere
{
    public float Radius = 1;
    public uint Columns = 24;
    public uint Rows = 12;
    public Axis Axis = Axis.Y;

    public void Generate(Mesh mesh)
    {
        // Parameter sanitization
        var res = math.max(3, math.int2((int)Columns, (int)Rows));

        // Axis selection
        var v0 = float3.zero;
        var ax = float3.zero;
        var ay = float3.zero;

        var ai = (int)Axis;

        v0[(ai + 2) % 3] = 1;
        ax[(ai + 0) % 3] = 1;
        ay[(ai + 1) % 3] = 1;

        // Vertex array
        var vtx = new List<float3>();
        var nrm = new List<float3>();
        var uv0 = new List<float2>();

        for (var iy = 0; iy < res.y; iy++)
        {
            for (var ix = 0; ix < res.x + 1; ix++)
            {
                var u = (float)ix / res.x;
                var v = (float)iy / (res.y - 1);

                var theta = u * math.PI * 2;
                var phi = (v - 0.5f) * math.PI;

                var rx = quaternion.AxisAngle(-ax, theta);
                var ry = quaternion.AxisAngle(ay, phi);
                var p = math.mul(rx, math.mul(ry, v0));

                vtx.Add(p * Radius);
                nrm.Add(p);
                uv0.Add(math.float2(u, v));
            }
        }

        // Index array
        var idx = new List<int>();
        var i = 0;

        for (var iy = 0; iy < res.y - 1; iy++, i++)
        {
            for (var ix = 0; ix < res.x; ix++, i++)
            {
                if (iy > 0)
                {
                    idx.Add(i);
                    idx.Add(i + res.x + 1);
                    idx.Add(i + 1);
                }

                if (iy < res.y - 1)
                {
                    idx.Add(i + 1);
                    idx.Add(i + res.x + 1);
                    idx.Add(i + res.x + 2);
                }
            }
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
