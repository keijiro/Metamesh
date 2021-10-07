using System.Collections.Generic;

namespace Metamesh
{

public class SmoothingVertexProcessor<TVertex>
{
    public readonly SmoothingSettings SmoothingSettings;
    private readonly List<TVertex> _vertices;
    private readonly HashSet<int> _usedIndices = new HashSet<int>();

    public SmoothingVertexProcessor(SmoothingSettings smoothingSettings, List<TVertex> vertices)
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

    protected virtual void OnCreatedVertex(int originalIndex) {}
}

public class SmoothingVertexProcessorUv<TVertex, TUv> : SmoothingVertexProcessor<TVertex>
{
    private readonly List<TUv> _uvs;

    public SmoothingVertexProcessorUv(SmoothingSettings smoothingSettings, List<TVertex> vertices, List<TUv> uvs) : base(smoothingSettings, vertices)
        => _uvs = uvs;

    protected override void OnCreatedVertex(int originalIndex)
    {
        base.OnCreatedVertex(originalIndex);
        _uvs.Add(_uvs[originalIndex]);
    }
}

}
