using System.Collections.Generic;

namespace Metamesh.Smoothing {

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

    public void AddIndex(List<int> indices, int index)
    {
        index = ProcessVertexIndexForTriangles(index);
        indices.Add(index);
    }

    protected virtual void OnCreatedVertex(int originalIndex) {}
}

}
