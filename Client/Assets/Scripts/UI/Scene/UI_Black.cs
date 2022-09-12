using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Black : UI_Scene
{
    enum Images
    {
        BlackImage,
    }

    enum Texts
    {
        InfoText,
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
    }

    public void FadeOutBlack(int time)
    {
        StartCoroutine(CoFadeOutBlack(time));
    }
    
    IEnumerator CoFadeOutBlack(int time)
    {
        int curTime = time;
        int totalTime = time;
        
        while (curTime >= 0)
        {
            float percent = (float) curTime / totalTime;
            Get<Image>((int) Images.BlackImage).color = new Color(0, 0, 0, percent);
            Get<Text>((int) Texts.InfoText).text = $"당신은 탐정입니다\n남은 시간 : {curTime}";
            
            yield return new WaitForSeconds(1f);
            
            curTime--;
            
        }
        
        Managers.UI.CloseUI(UiName.Black);
    }
}
