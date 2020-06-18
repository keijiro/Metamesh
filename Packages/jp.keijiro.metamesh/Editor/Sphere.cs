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
        var res = math.int2((int)Columns, (int)Rows);
        res = math.max(res, math.int2(3, 2));

        // Axis selection
        var va = float3.zero;
        var vx = float3.zero;
        var vy = float3.zero;

        var ai = (int)Axis;

        va[(ai + 2) % 3] = 1;
        vx[(ai + 0) % 3] = 1;
        vy[(ai + 1) % 3] = 1;

        // Vertex array
        var vtx = new List<float3>();
        var nrm = new List<float3>();
        var uv0 = new List<float2>();

        for (var iy = 0; iy < res.y + 1; iy++)
        {
            for (var ix = 0; ix < res.x + 1; ix++)
            {
                var u = (float)ix / res.x;
                var v = (float)iy / res.y;

                var theta = u * math.PI * 2;
                var phi = (v - 0.5f) * math.PI;

                var rx = quaternion.AxisAngle(-vx, theta);
                var ry = quaternion.AxisAngle(vy, phi);
                var p = math.mul(rx, math.mul(ry, va));

                vtx.Add(p * Radius);
                nrm.Add(p);
                uv0.Add(math.float2(u, v));
            }
        }

        // Index array
        var idx = new List<int>();
        var i = 0;

        for (var iy = 0; iy < res.y; iy++, i++)
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
