using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public int popupId;
    
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }
}
