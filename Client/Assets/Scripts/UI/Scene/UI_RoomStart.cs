using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RoomStart : UI_Scene
{
    enum Buttons
    {
        StartBtn,
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.StartBtn).gameObject.BindEvent(OnClickButton);
    }

    void OnClickButton(PointerEventData evt)
    {
        C_GameStart gameStartPacket = new C_GameStart();
        Managers.Network.Send(gameStartPacket);
    }
}
