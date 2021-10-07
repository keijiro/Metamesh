using System;
using UnityEngine;

namespace Metamesh.Smoothing {

[Serializable]
public class SmoothingSettings
{
    public bool ConfigureSmoothingAngle;
    [Min(0)]
    public float SmoothingAngle;
}

}
