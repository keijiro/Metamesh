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
    public SmoothingSettings SmoothingSettings;

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
        var usedIdx = new HashSet<int>();
        var i = 0;

        for (var iy = 0; iy < res.y; iy++, i++)
        {
            for (var ix = 0; ix < res.x; ix++, i++)
            {
                if (iy > 0)
                {
                    AddIndex(i, idx, usedIdx, vtx, uv0);
                    AddIndex(i + res.x + 1, idx, usedIdx, vtx, uv0);
                    AddIndex(i + 1, idx, usedIdx, vtx, uv0);
                }

                if (iy < res.y - 1)
                {
                    AddIndex(i + 1, idx, usedIdx, vtx, uv0);
                    AddIndex(i + res.x + 1, idx, usedIdx, vtx, uv0);
                    AddIndex(i + res.x + 2, idx, usedIdx, vtx, uv0);
                }
            }
        }

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());
        mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        
        if (SmoothingSettings.ConfigureSmoothingAngle)
            mesh.RecalculateNormals(SmoothingSettings.SmoothingAngle);
        else
            mesh.SetNormals(nrm.Select(v => (Vector3)v).ToList());
    }

    void AddIndex(int index, List<int> indices, HashSet<int> usedIdx, List<float3> vtx, List<float2> uv0)
    {
        index = ProcessVertexIndexForTriangles(index, usedIdx, vtx, uv0);
        indices.Add(index);
    }
    
    int ProcessVertexIndexForTriangles(int index, HashSet<int> usedIdx, List<float3> vtx, List<float2> uv0)
    {
        if (!SmoothingSettings.ConfigureSmoothingAngle)
            return index;
        
        if (!usedIdx.Contains(index))
        {
            usedIdx.Add(index);
            return index;
        }

        vtx.Add(vtx[index]);
        uv0.Add(uv0[index]);
        return vtx.Count - 1;
    }
}

} // namespace Metamesh
