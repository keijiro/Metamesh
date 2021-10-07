using System.Collections.Generic;

namespace Metamesh.Smoothing {

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
