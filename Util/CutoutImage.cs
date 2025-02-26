using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutImage : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material mat = new Material(base.materialForRendering);
            mat.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            return mat;
        }
    }
}