using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Metamesh
{
    [System.Serializable]
    public class Box
    {
        public float Width = 1;
        public float Height = 1;
        public float Depth = 1;

        public void Generate(Mesh mesh)
        {
            var x = Width  / 2;
            var y = Height / 2;
            var z = Depth  / 2;

            var v0 = new Vector3(-x, -y, -z);
            var v1 = new Vector3( x, -y, -z);
            var v2 = new Vector3(-x, -y,  z);
            var v3 = new Vector3( x, -y,  z);

            var v4 = new Vector3(-x,  y, -z);
            var v5 = new Vector3( x,  y, -z);
            var v6 = new Vector3(-x,  y,  z);
            var v7 = new Vector3( x,  y,  z);

            var t0 = new Vector2(0, 0);
            var t1 = new Vector2(1, 0);
            var t2 = new Vector2(0, 1);
            var t3 = new Vector2(1, 1);

            var vtx = new List<Vector3>();
            var uv0 = new List<Vector2>();

            // Bottom
            vtx.Add(v0); vtx.Add(v1); vtx.Add(v2);
            vtx.Add(v1); vtx.Add(v3); vtx.Add(v2);
            uv0.Add(t1); uv0.Add(t0); uv0.Add(t3); 
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t3); 

            // Top
            vtx.Add(v4); vtx.Add(v6); vtx.Add(v5);
            vtx.Add(v5); vtx.Add(v6); vtx.Add(v7);
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t1); 
            uv0.Add(t1); uv0.Add(t2); uv0.Add(t3); 

            // Side faces
            vtx.Add(v0); vtx.Add(v4); vtx.Add(v1);
            vtx.Add(v1); vtx.Add(v4); vtx.Add(v5);
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t1); 
            uv0.Add(t1); uv0.Add(t2); uv0.Add(t3); 

            vtx.Add(v1); vtx.Add(v5); vtx.Add(v3);
            vtx.Add(v3); vtx.Add(v5); vtx.Add(v7);
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t1); 
            uv0.Add(t1); uv0.Add(t2); uv0.Add(t3); 

            vtx.Add(v2); vtx.Add(v6); vtx.Add(v0);
            vtx.Add(v0); vtx.Add(v6); vtx.Add(v4);
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t1); 
            uv0.Add(t1); uv0.Add(t2); uv0.Add(t3); 

            vtx.Add(v3); vtx.Add(v7); vtx.Add(v2);
            vtx.Add(v2); vtx.Add(v7); vtx.Add(v6);
            uv0.Add(t0); uv0.Add(t2); uv0.Add(t1); 
            uv0.Add(t1); uv0.Add(t2); uv0.Add(t3); 

            var idx = Enumerable.Range(0, vtx.Count).ToList();

            mesh.SetVertices(vtx);
            mesh.SetUVs(0, uv0);
            mesh.SetIndices(idx, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
        }
    }
}
