using System;
using UnityEngine;

namespace Metamesh
{

public static class NormalsUtils
{
    public static void WriteNormals(this Mesh mesh, SmoothingSettings smoothingSettings, Action<Mesh> fallbackWriteNormals)
    {
        if (smoothingSettings.ConfigureSmoothingAngle)
            mesh.RecalculateNormals(smoothingSettings.SmoothingAngle);
        else
            fallbackWriteNormals(mesh);
    }
}

}
