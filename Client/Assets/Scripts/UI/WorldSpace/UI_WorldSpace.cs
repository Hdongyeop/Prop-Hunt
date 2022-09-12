using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WorldSpace : UI_Base
{
    public override void Init()
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
    }
}
