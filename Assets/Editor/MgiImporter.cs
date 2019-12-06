using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.Collections.Generic;
using System.Linq;

namespace Mgi
{
    [ScriptedImporter(1, "mgi")]
    class MgiImporter : ScriptedImporter
    {
        #region ScriptedImporter implementation

        public enum Model { Quad, Box }

        public enum Axis { X, Y, Z }

        [SerializeField] Model _model = Model.Box;
        [SerializeField] float _width = 1;
        [SerializeField] float _height = 1;
        [SerializeField] float _depth = 1;
        [SerializeField] Axis _axis = Axis.Z;
        [SerializeField] bool _doubleSided = true;

        public override void OnImportAsset(AssetImportContext context)
        {
            var gameObject = new GameObject();
            var mesh = ImportAsMesh(context.assetPath);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial =
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

            context.AddObjectToAsset("prefab", gameObject);
            if (mesh != null) context.AddObjectToAsset("mesh", mesh);

            context.SetMainObject(gameObject);
        }

        #endregion

        #region Reader implementation

        Mesh ImportAsMesh(string path)
        {
            var mesh = new Mesh();
            mesh.name = "Mesh";

            switch (_model)
            {
            case Model.Quad:
                GenerateQuad(mesh, _width, _height, _axis, _doubleSided);
                break;
            case Model.Box:
                GenerateBox(mesh, _width, _height, _depth);
                break;
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(true);

            return mesh;
        }

        static void GenerateQuad(
            Mesh mesh,
            float width, float height,
            Axis axis, bool doubleSided
        )
        {
            var x = Vector3.zero;
            var y = Vector3.zero;

            x[((int)axis + 1) % 3] = width / 2;
            y[((int)axis + 2) % 3] = height / 2;

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

            if (doubleSided)
            {
                vtx.Add(v1); uv0.Add(t1);
                vtx.Add(v2); uv0.Add(t2);
                vtx.Add(v3); uv0.Add(t3);
                vtx.Add(v4); uv0.Add(t4);
            }

            var idx = new List<int>();

            idx.Add(0); idx.Add(1); idx.Add(2);
            idx.Add(1); idx.Add(3); idx.Add(2);

            if (doubleSided)
            {
                idx.Add(4); idx.Add(6); idx.Add(5);
                idx.Add(5); idx.Add(6); idx.Add(7);
            }

            mesh.SetVertices(vtx);
            mesh.SetUVs(0, uv0);
            mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        }

        static void GenerateBox(Mesh mesh, float width, float height, float depth)
        {
            var x = width / 2;
            var y = height / 2;
            var z = depth / 2;

            var v0 = new Vector3(-x, -y, -z);
            var v1 = new Vector3( x, -y, -z);
            var v2 = new Vector3(-x, -y,  z);
            var v3 = new Vector3( x, -y,  z);

            var v4 = new Vector3(-x,  y, -z);
            var v5 = new Vector3( x,  y, -z);
            var v6 = new Vector3(-x,  y,  z);
            var v7 = new Vector3( x,  y,  z);

            var vtx = new List<Vector3>();
            var uv0 = new List<Vector2>();

            vtx.Add(v0); vtx.Add(v1); vtx.Add(v2);
            vtx.Add(v1); vtx.Add(v3); vtx.Add(v2);

            vtx.Add(v4); vtx.Add(v6); vtx.Add(v5);
            vtx.Add(v5); vtx.Add(v6); vtx.Add(v7);

            vtx.Add(v0); vtx.Add(v4); vtx.Add(v1);
            vtx.Add(v1); vtx.Add(v4); vtx.Add(v5);

            vtx.Add(v1); vtx.Add(v5); vtx.Add(v3);
            vtx.Add(v3); vtx.Add(v5); vtx.Add(v7);

            vtx.Add(v2); vtx.Add(v6); vtx.Add(v0);
            vtx.Add(v0); vtx.Add(v6); vtx.Add(v4);

            vtx.Add(v3); vtx.Add(v7); vtx.Add(v2);
            vtx.Add(v2); vtx.Add(v7); vtx.Add(v6);

            var idx = Enumerable.Range(0, vtx.Count).ToList();

            mesh.SetVertices(vtx);
            mesh.SetIndices(idx, MeshTopology.Triangles, 0);
        }

        #endregion
    }
}
