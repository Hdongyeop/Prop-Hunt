using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerName : UI_WorldSpace
{
    enum Texts
    {
        NameText,
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Text>(typeof(Texts));
    }

    public void SetPlayerName(string name)
    {
        Get<Text>((int) Texts.NameText).text = name;
    }
}
