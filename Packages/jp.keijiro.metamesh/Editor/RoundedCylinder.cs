using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Metamesh {

[System.Serializable]
public sealed class RoundedCylinder
{
    public float TopRadius = 1;
    public float BottomRadius = 1;
    public float Height = 1;
    public uint Columns = 24;
    public uint Rows = 12;
    [Range(1, 10)] public int Divisions = 3;
    [Min(0)] public float RoundingRadius = 0.1f;
    public Axis Axis = Axis.Y;

    struct ProfilePoint
    {
        public float2 Position;
        public float2 Normal;

        public ProfilePoint(float2 position, float2 normal)
        {
            Position = position;
            Normal = normal;
        }
    }

    public void Generate(Mesh mesh)
    {
        // Parameter sanitization
        var res = math.int2((int)Columns, (int)Rows);
        res = math.max(res, math.int2(3, 1));

        var divisions = math.max(Divisions, 1);
        var height = math.max(math.abs(Height), math.EPSILON);
        var topRadius = math.max(TopRadius, 0);
        var bottomRadius = math.max(BottomRadius, 0);

        // The fillet is tangent to both the potentially sloped body and the
        // horizontal end cap. Clamp it before the two fillets or either cap
        // collapse.
        var slope = (topRadius - bottomRadius) / height;
        var slopeLength = math.sqrt(1 + slope * slope);
        var maxRounding = math.min
        (
            height / 2,
            math.min
            (
                topRadius / (slopeLength + slope),
                bottomRadius / (slopeLength - slope)
            )
        );
        var rounding = math.clamp(RoundingRadius, 0, maxRounding);

        if (rounding <= math.EPSILON)
        {
            new Cylinder
            {
                TopRadius = topRadius,
                BottomRadius = bottomRadius,
                Height = height,
                Columns = (uint)res.x,
                Rows = (uint)res.y,
                Axis = Axis,
                Caps = true
            }.Generate(mesh);
            return;
        }

        // Axis selection
        var va = float3.zero;
        var vx = float3.zero;
        var ai = (int)Axis;
        va[(ai + 0) % 3] = 1;
        vx[(ai + 1) % 3] = 1;

        // Build the radial/axial cross-section from bottom to top. Revolving
        // this profile creates the complete rounded cylinder surface.
        var sideAngle = -math.atan(slope);
        var sideNormal = math.float2(math.cos(sideAngle), math.sin(sideAngle));
        var bottomCenter = math.float2
        (
            math.max(bottomRadius - rounding * (slopeLength - slope), 0),
            -height / 2 + rounding
        );
        var topCenter = math.float2
        (
            math.max(topRadius - rounding * (slopeLength + slope), 0),
            height / 2 - rounding
        );

        var profile = new List<ProfilePoint>();

        for (var i = 0; i <= divisions; i++)
        {
            var t = (float)i / divisions;
            var angle = math.lerp(-math.PI / 2, sideAngle, t);
            var normal = math.float2(math.cos(angle), math.sin(angle));
            profile.Add(new ProfilePoint(bottomCenter + normal * rounding, normal));
        }

        var bottomTangent = bottomCenter + sideNormal * rounding;
        var topTangent = topCenter + sideNormal * rounding;
        if (math.distancesq(bottomTangent, topTangent) > math.EPSILON * math.EPSILON)
        {
            for (var i = 1; i <= res.y; i++)
            {
                var t = (float)i / res.y;
                profile.Add(new ProfilePoint(math.lerp(bottomTangent, topTangent, t), sideNormal));
            }
        }

        for (var i = 1; i <= divisions; i++)
        {
            var t = (float)i / divisions;
            var angle = math.lerp(sideAngle, math.PI / 2, t);
            var normal = math.float2(math.cos(angle), math.sin(angle));
            profile.Add(new ProfilePoint(topCenter + normal * rounding, normal));
        }

        // Arc-length parameterization keeps the side UV density even across
        // both straight and rounded sections.
        var profileV = new List<float> { 0 };
        var profileLength = 0f;
        for (var i = 1; i < profile.Count; i++)
        {
            profileLength += math.distance(profile[i - 1].Position, profile[i].Position);
            profileV.Add(profileLength);
        }
        for (var i = 0; i < profileV.Count; i++) profileV[i] /= profileLength;

        // Surface vertices
        var vtx = new List<float3>();
        var nrm = new List<float3>();
        var uv0 = new List<float2>();

        for (var iy = 0; iy < profile.Count; iy++)
        {
            var point = profile[iy];
            for (var ix = 0; ix <= res.x; ix++)
            {
                var u = (float)ix / res.x;
                var rot = quaternion.AxisAngle(va, u * math.PI * -2);
                var radial = math.mul(rot, vx);

                vtx.Add(radial * point.Position.x + va * point.Position.y);
                nrm.Add(radial * point.Normal.x + va * point.Normal.y);
                uv0.Add(math.float2(u, profileV[iy]));
            }
        }

        // Surface indices
        var idx = new List<int>();
        var stride = res.x + 1;
        for (var iy = 0; iy < profile.Count - 1; iy++)
        {
            for (var ix = 0; ix < res.x; ix++)
            {
                var i = iy * stride + ix;
                idx.Add(i);
                idx.Add(i + stride);
                idx.Add(i + 1);

                idx.Add(i + 1);
                idx.Add(i + stride);
                idx.Add(i + stride + 1);
            }
        }

        // Separate cap vertices allow radial cap UVs. The normals match the
        // last fillet rings, so the duplicated boundary remains visually smooth.
        AddCap(false, bottomCenter.x, -height / 2, va, vx, res.x, vtx, nrm, uv0, idx);
        AddCap(true, topCenter.x, height / 2, va, vx, res.x, vtx, nrm, uv0, idx);

        // Mesh object construction
        if (vtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vtx.Select(v => (Vector3)v).ToList());
        mesh.SetNormals(nrm.Select(v => (Vector3)v).ToList());
        mesh.SetUVs(0, uv0.Select(v => (Vector2)v).ToList());
        mesh.SetIndices(idx, MeshTopology.Triangles, 0);
    }

    static void AddCap
    (
        bool top, float radius, float height, float3 axis, float3 radialAxis,
        int columns, List<float3> vtx, List<float3> nrm, List<float2> uv0,
        List<int> idx
    )
    {
        if (radius <= math.EPSILON) return;

        var center = vtx.Count;
        var normal = top ? axis : -axis;

        vtx.Add(axis * height);
        nrm.Add(normal);
        uv0.Add(math.float2(0.5f));

        for (var ix = 0; ix <= columns; ix++)
        {
            var angle = (float)ix / columns * math.PI * -2;
            var radial = math.mul(quaternion.AxisAngle(axis, angle), radialAxis);
            vtx.Add(radial * radius + axis * height);
            nrm.Add(normal);
            uv0.Add(math.float2(math.cos(angle), math.sin(angle)) / 2 + 0.5f);
        }

        for (var ix = 0; ix < columns; ix++)
        {
            if (top)
            {
                idx.Add(center);
                idx.Add(center + ix + 2);
                idx.Add(center + ix + 1);
            }
            else
            {
                idx.Add(center);
                idx.Add(center + ix + 1);
                idx.Add(center + ix + 2);
            }
        }
    }
}

} // namespace Metamesh
