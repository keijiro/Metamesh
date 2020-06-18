using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Metamesh {

public sealed class IcosphereBuilder
{
    #region Public properties

    public int VertexCount => _vertices.Count;

    public IEnumerable<float3> Vertices => _vertices;

    public IEnumerable<int> Indices
      => _triangles.SelectMany(it => new [] { it.i1, it.i2, it.i3 });

    #endregion

    #region Constructors

    public IcosphereBuilder() => BuildInitialInstance();

    public IcosphereBuilder(IcosphereBuilder source)
    {
        _vertices.AddRange(source._vertices);

        var midPoints = new MidpointTable(_vertices);

        foreach (var t in source._triangles)
        {
            var m1 = midPoints.GetMidpoint(t.i1, t.i2);
            var m2 = midPoints.GetMidpoint(t.i2, t.i3);
            var m3 = midPoints.GetMidpoint(t.i3, t.i1);

            _triangles.Add((t.i1, m1, m3));
            _triangles.Add((m1, t.i2, m2));
            _triangles.Add((m3, m2, t.i3));
            _triangles.Add((m1, m2, m3));
        }
    }

    #endregion

    #region Data members

    List<float3> _vertices = new List<float3>();
    List<(int i1, int i2, int i3)> _triangles = new List<(int, int, int)>();

    #endregion

    #region Level 0 icosphere

    void BuildInitialInstance()
    {
        var t = (1 + math.sqrt(5)) / 2;

        _vertices.Add(math.normalize(math.float3(-1, +t, 0)));
        _vertices.Add(math.normalize(math.float3(+1, +t, 0)));
        _vertices.Add(math.normalize(math.float3(-1, -t, 0)));
        _vertices.Add(math.normalize(math.float3(+1, -t, 0)));

        _vertices.Add(math.normalize(math.float3(0, -1, +t)));
        _vertices.Add(math.normalize(math.float3(0, +1, +t)));
        _vertices.Add(math.normalize(math.float3(0, -1, -t)));
        _vertices.Add(math.normalize(math.float3(0, +1, -t)));

        _vertices.Add(math.normalize(math.float3(+t, 0, -1)));
        _vertices.Add(math.normalize(math.float3(+t, 0, +1)));
        _vertices.Add(math.normalize(math.float3(-t, 0, -1)));
        _vertices.Add(math.normalize(math.float3(-t, 0, +1)));

        _triangles.Add((0, 11, 5));
        _triangles.Add((0, 5, 1));
        _triangles.Add((0, 1, 7));
        _triangles.Add((0, 7, 10));
        _triangles.Add((0, 10, 11));

        _triangles.Add((1, 5, 9));
        _triangles.Add((5, 11, 4));
        _triangles.Add((11, 10, 2));
        _triangles.Add((10, 7, 6));
        _triangles.Add((7, 1, 8));

        _triangles.Add((3, 9, 4));
        _triangles.Add((3, 4, 2));
        _triangles.Add((3, 2, 6));
        _triangles.Add((3, 6, 8));
        _triangles.Add((3, 8, 9));

        _triangles.Add((4, 9, 5));
        _triangles.Add((2, 4, 11));
        _triangles.Add((6, 2, 10));
        _triangles.Add((8, 6, 7));
        _triangles.Add((9, 8, 1));
    }

    #endregion

    #region Midpoint table class

    class MidpointTable
    {
        List<float3> _vertices;
        Dictionary<int, int> _table;

        // Key for the given pair of indices
        static int IndexPairToKey(int i1, int i2)
          => i1 < i2 ?  i1 | (i2 << 16) : (i1 << 16) | i2;

        // Constructor
        public MidpointTable(List<float3> vertices)
        {
            _vertices = vertices;
            _table = new Dictionary<int, int>();
        }

        // Get the midpoint of the pair of indices.
        public int GetMidpoint(int i1, int i2)
        {
            var key = IndexPairToKey(i1, i2);

            // Look up the table.
            if (_table.ContainsKey(key)) return _table[key];

            // Add a new entry to the table.
            var i = _vertices.Count;
            var mid = (_vertices[i1] + _vertices[i2]) / 2;
            _vertices.Add(math.normalize(mid));
            _table[key] = i;

            return i;
        }
    }

    #endregion
}

}
