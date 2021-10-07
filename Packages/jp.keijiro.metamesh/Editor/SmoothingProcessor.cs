using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metamesh
{

public class SmoothingProcessor<TVertex>
{
    public readonly SmoothingSettings SmoothingSettings;
    private readonly List<TVertex> _vertices;
    private readonly HashSet<int> _usedIndices = new HashSet<int>();

    public SmoothingProcessor(SmoothingSettings smoothingSettings, List<TVertex> vertices)
    {
        SmoothingSettings = smoothingSettings;
        _vertices = vertices;
    }

    public int ProcessVertexIndexForTriangles(int index)
    {
        if (!SmoothingSettings.ConfigureSmoothingAngle)
            return index;
        
        if (!_usedIndices.Contains(index))
        {
            _usedIndices.Add(index);
            return index;
        }

        _vertices.Add(_vertices[index]);
        OnCreatedVertex(index);
        return _vertices.Count - 1;
    }

    public void AddIndex(int index, List<int> indices)
    {
        index = ProcessVertexIndexForTriangles(index);
        indices.Add(index);
    }

    protected virtual void OnCreatedVertex(int originalIndex) {}
}

public class SmoothingProcessorUv<TVertex, TUv> : SmoothingProcessor<TVertex>
{
    private readonly List<TUv> _uvs;

    public SmoothingProcessorUv(SmoothingSettings smoothingSettings, List<TVertex> vertices, List<TUv> uvs) : base(smoothingSettings, vertices)
        => _uvs = uvs;

    protected override void OnCreatedVertex(int originalIndex)
    {
        base.OnCreatedVertex(originalIndex);
        _uvs.Add(_uvs[originalIndex]);
    }
}

}
