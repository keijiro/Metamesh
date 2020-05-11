using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace MetaMesh
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
            var vx = float3.zero;
            var vy = float3.zero;

            vx[((int)Axis + 1) % 3] = Size.x / 2;
            vy[((int)Axis + 2) % 3] = Size.y / 2;

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
                    idx.Add(i + 1);
                    idx.Add(i + res.x);

                    idx.Add(i + 1);
                    idx.Add(i + res.x + 1);
                    idx.Add(i + res.x);
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
                        idx.Add(i + res.x);
                        idx.Add(i + 1);

                        idx.Add(i + 1);
                        idx.Add(i + res.x);
                        idx.Add(i + res.x + 1);
                    }
                }
            }

            // Mesh object construction
            mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());
            mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
            mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        }
    }
}
