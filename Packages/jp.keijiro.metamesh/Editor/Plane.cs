using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Metamesh
{
    [System.Serializable]
    public class Plane
    {
        public float2 Size = math.float2(1, 1);
        public uint2 Subdivisions = math.uint2(2, 2);
        public Axis Axis = Axis.Y;
        public bool DoubleSided = false;

        public void Generate(Mesh mesh)
        {
            // Parameter sanitization
            var res = (int2)math.max(2, Subdivisions);

            // X/Y vectors perpendicular to Axis
            float3 vx, vy;

            if (Axis == Axis.X)
            {
                vx = math.float3(0, 0, 1);
                vy = math.float3(0, 1, 0);
            }
            else if (Axis == Axis.Y)
            {
                vx = math.float3(1, 0, 0);
                vy = math.float3(0, 0, 1);
            }
            else // Axis.Z
            {
                vx = math.float3(-1, 0, 0);
                vy = math.float3(0, 1, 0);
            }

            vx *= Size.x;
            vy *= Size.y;

            // Vertex array
            var vtx = new List<float3>();
            var uv0 = new List<float2>();

            for (var iy = 0; iy < res.y; iy++)
            {
                for (var ix = 0; ix < res.x; ix++)
                {
                    var uv = math.float2((float)ix / (res.x - 1),
                                         (float)iy / (res.y - 1));

                    var p = math.lerp(-vx, vx, uv.x) +
                            math.lerp(-vy, vy, uv.y);

                    vtx.Add(p);
                    uv0.Add(uv);
                }
            }

            if (DoubleSided)
            {
                vtx = vtx.Concat(vtx).ToList();
                uv0 = uv0.Concat(uv0).ToList();
            }

            // Index array
            var idx = new List<int>();
            var i = 0;

            for (var iy = 0; iy < res.y - 1; iy++, i++)
            {
                for (var ix = 0; ix < res.x - 1; ix++, i++)
                {
                    idx.Add(i);
                    idx.Add(i + res.x);
                    idx.Add(i + 1);

                    idx.Add(i + 1);
                    idx.Add(i + res.x);
                    idx.Add(i + res.x + 1);
                }
            }

            if (DoubleSided)
            {
                i += res.x;

                for (var iy = 0; iy < res.y - 1; iy++, i++)
                {
                    for (var ix = 0; ix < res.x - 1; ix++, i++)
                    {
                        idx.Add(i);
                        idx.Add(i + 1);
                        idx.Add(i + res.x);

                        idx.Add(i + 1);
                        idx.Add(i + res.x + 1);
                        idx.Add(i + res.x);
                    }
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
}
