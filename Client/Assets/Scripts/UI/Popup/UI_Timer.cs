using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : UI_Popup
{
    enum Texts
    {
        TimerText,
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Text>(typeof(Texts));
    }

    public void TimerStart(int time)
    {
        StartCoroutine(CoTimerStart(time));
    }
    
    IEnumerator CoTimerStart(int time) // 초
    {
        int _time = time;
        while (_time >= 0)
        {
            int minute = _time / 60;
            int second = _time % 60;
            
            Get<Text>((int) Texts.TimerText).text = $"{minute.ToString("00")} : {second.ToString("00")}";
            
            yield return new WaitForSeconds(1f);
            
            _time--;
        }

        C_TimerFinish finishPacket = new C_TimerFinish();
        Managers.Network.Send(finishPacket);
        
        Managers.UI.CloseUI(UiName.Timer);
    }
}
