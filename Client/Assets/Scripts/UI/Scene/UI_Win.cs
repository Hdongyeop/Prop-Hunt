using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Win : UI_Scene
{
    enum Texts
    {
        WinText,
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Text>(typeof(Texts));
    }

    public void SetText(string text)
    {
        Get<Text>((int) Texts.WinText).text = text;
    }
}
