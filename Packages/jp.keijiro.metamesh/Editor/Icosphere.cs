using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public sealed class Icosphere
{
    public float Radius = 1;
    public uint Subdivision = 2;

    public void Generate(Mesh mesh)
    {
        var builder = new IcosphereBuilder();
        for (var i = 1; i < Subdivision; i++)
            builder = new IcosphereBuilder(builder);

        var vtx = builder.Vertices.Select(v => (Vector3)(v * Radius));
        var nrm = builder.Vertices.Select(v => (Vector3)v);
        var idx = builder.Indices;

        if (builder.VertexCount > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.ToList());
        mesh.SetNormals(nrm.ToList());
        mesh.SetIndices(idx.ToList(), MeshTopology.Triangles, 0);
    }
}

}
