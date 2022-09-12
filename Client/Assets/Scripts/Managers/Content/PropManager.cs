using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropManager
{
    private Dictionary<int, Sprite> Props = new Dictionary<int, Sprite>();

    public void Init()
    {
        Sprite[] propSprites = Resources.LoadAll<Sprite>("Art/Texture/TX Props");
        for (int i = 0; i < propSprites.Length; i++)
            Props.Add(i, propSprites[i]);
    }

    // 임시로 쓰는 함수임
    public int GetRandomProp()
    {
        return Random.Range(0, Props.Count);
    }

    public Sprite GetPropSprite(int idx)
    {
        return Props[idx];
    }
}
