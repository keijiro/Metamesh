using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Metamesh {

[System.Serializable]
public sealed class Icosphere
{
    public float Radius = 1;
    public uint Subdivision = 2;

public void Generate(Mesh mesh, Vector3 pivot)
    {
        var builder = new IcosphereBuilder();
        for (var i = 1; i < Subdivision; i++)
            builder = new IcosphereBuilder(builder);

        var vtx = builder.Vertices.Select(v => (Vector3)(v * Radius));
        var nrm = builder.Vertices.Select(v => (Vector3)v);
        var idx = builder.Indices;

        List<Vector3> vertices = vtx.ToList();
        for (int v = 0; v < vertices.Count; v++)
            vertices[v] -= pivot;
        
        if (builder.VertexCount > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        mesh.SetNormals(nrm.ToList());
        mesh.SetIndices(idx.ToList(), MeshTopology.Triangles, 0);
    }
}

} // namespace Metamesh
