using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    public void SetPlayerName(string name)
    {
        Text nameText = Util.FindChild<Text>(gameObject, "nameText");
        nameText.text = name;
    }
}
