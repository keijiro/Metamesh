using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using Path = System.IO.Path;

namespace Mgi
{
    [ScriptedImporter(1, "mgi")]
    class MgiImporter : ScriptedImporter
    {
        #region ScriptedImporter implementation

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
            mesh.name = Path.GetFileNameWithoutExtension(path);
            mesh.SetVertices(new [] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0) });
            mesh.SetIndices(new [] { 0, 1, 2 }, MeshTopology.Triangles, 0);
            mesh.UploadMeshData(true);
            return mesh;
        }

        #endregion
    }
}
