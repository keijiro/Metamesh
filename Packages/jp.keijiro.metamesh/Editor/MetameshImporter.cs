using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Metamesh {

[ScriptedImporter(1, "metamesh")]
public sealed class MetameshImporter : ScriptedImporter
{
    #region ScriptedImporter implementation

    [SerializeField] Shape _shape = Shape.Box;
    [SerializeField] Plane _plane = null;
    [SerializeField] Box _box = new Box();
    [SerializeField] Sphere _sphere = new Sphere();
    [SerializeField] Icosphere _icosphere = new Icosphere();
    [SerializeField] Cylinder _cylinder = new Cylinder();
    [SerializeField] RoundedBox _roundedBox = new RoundedBox();
    [SerializeField] Ring _ring = new Ring();
    [SerializeField] Disc _disc = new Disc();
    [SerializeField] bool _generateLightmapUVs = false;

    public override void OnImportAsset(AssetImportContext context)
    {
        var gameObject = new GameObject();
        var mesh = ImportAsMesh(context.assetPath);

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var pipelineAsset = GraphicsSettings.currentRenderPipeline;
        var baseMaterial = pipelineAsset ? pipelineAsset.defaultMaterial : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
        
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = baseMaterial;

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

        switch (_shape)
        {
            case Shape.Plane      : _plane     .Generate(mesh); break;
            case Shape.Box        : _box       .Generate(mesh); break;
            case Shape.Sphere     : _sphere    .Generate(mesh); break;
            case Shape.Icosphere  : _icosphere .Generate(mesh); break;
            case Shape.Cylinder   : _cylinder  .Generate(mesh); break;
            case Shape.RoundedBox : _roundedBox.Generate(mesh); break;
            case Shape.Ring       : _ring      .Generate(mesh); break;
            case Shape.Disc       : _disc      .Generate(mesh); break;
        }

        mesh.RecalculateBounds();
        if(_generateLightmapUVs) Unwrapping.GenerateSecondaryUVSet(mesh);
        mesh.UploadMeshData(true);

        return mesh;
    }

    #endregion
}

} // namespace Metamesh
