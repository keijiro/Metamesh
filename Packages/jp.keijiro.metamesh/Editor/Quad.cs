using UnityEngine;
using System.Collections.Generic;

namespace MetaMesh
{
    [System.Serializable]
    public class Quad
    {
        public float Width = 1;
        public float Height = 1;
        public Axis Axis = Axis.Z;
        public bool DoubleSided = true;

        public void Generate(Mesh mesh)
        {
            var x = Vector3.zero;
            var y = Vector3.zero;

            x[((int)Axis + 1) % 3] = Width  / 2;
            y[((int)Axis + 2) % 3] = Height / 2;

            var v1 = -x -y;
            var v2 =  x -y;
            var v3 = -x +y;
            var v4 =  x +y;

            var t1 = new Vector2(0, 0);
            var t2 = new Vector2(1, 0);
            var t3 = new Vector2(0, 1);
            var t4 = new Vector2(1, 1);

            var vtx = new List<Vector3>();
            var uv0 = new List<Vector2>();

            vtx.Add(v1); uv0.Add(t1);
            vtx.Add(v2); uv0.Add(t2);
            vtx.Add(v3); uv0.Add(t3);
            vtx.Add(v4); uv0.Add(t4);

            if (DoubleSided)
            {
                vtx.Add(v1); uv0.Add(t1);
                vtx.Add(v2); uv0.Add(t2);
                vtx.Add(v3); uv0.Add(t3);
                vtx.Add(v4); uv0.Add(t4);
            }

            var idx = new List<int>();

            idx.Add(0); idx.Add(1); idx.Add(2);
            idx.Add(1); idx.Add(3); idx.Add(2);

            if (DoubleSided)
            {
                idx.Add(4); idx.Add(6); idx.Add(5);
                idx.Add(5); idx.Add(6); idx.Add(7);
            }

            mesh.SetVertices(vtx);
            mesh.SetUVs(0, uv0);
            mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        }
    }
}
