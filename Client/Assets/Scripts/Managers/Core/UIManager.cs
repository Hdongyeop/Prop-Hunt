using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class UIManager
{
    int _order = 10;
    
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    List<UI_WorldSpace> _worldSpaceList = new List<UI_WorldSpace>();
    UI_Black ui_black = null;
    UI_RoomStart ui_roomstart = null;
    UI_Win ui_win = null;

    public GameObject Root
    {
        get
        {
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
            return root;
		}
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public void ShowUI(UiName uiName, int integer, string text)
    {
	    switch (uiName)
	    {
		    case UiName.Username:
		    {
			    foreach (var go in Managers.Object.Objects.Values)
			    {
				    if(go.GetComponentInChildren<UI_PlayerName>() != null) continue;
				    
				    UI_PlayerName playerName = Managers.UI.MakeWorldSpaceUI<UI_PlayerName>(go.transform);
				    playerName.SetPlayerName(go.GetComponent<PlayerController>().Name);
				    playerName.gameObject.transform.localPosition = new Vector3(0f, 1.9f, 0);
			    }
		    }
			    break;
		    case UiName.Startbtn:
		    {
			    Managers.UI.MakeSceneUI<UI_RoomStart>();
		    }
			    break;
		    case UiName.Timer:
		    {
			    var timer = Managers.UI.MakePopupUI<UI_Timer>();
			    timer.TimerStart(integer);
		    }
			    break;
		    case UiName.Black:
		    {
			    var black = Managers.UI.MakeSceneUI<UI_Black>();
			    black.FadeOutBlack(integer);
		    }
			    break;
		    case UiName.Win:
		    {
			    var win = Managers.UI.MakeSceneUI<UI_Win>();
			    win.SetText(text);
		    }
			    break;
	    }
    }

    public void CloseUI(UiName uiName)
    {
	    switch (uiName)
	    {
		    case UiName.Username:
		    {
			    foreach (var go in Managers.Object.Objects.Values)
			    {
				    var ui = go.GetComponentInChildren<UI_WorldSpace>();
				    CloseWorldSpaceUI(ui);
			    }
		    }
			    break;
		    case UiName.Startbtn:
		    {
			    CloseSceneUI<UI_RoomStart>();
		    }
			    break;
		    case UiName.Timer:
		    {
			    ClosePopupUI();
		    }
			    break;
		    case UiName.Black:
		    {
			    CloseSceneUI<UI_Black>();
		    }
			    break;
		    case UiName.Win:
		    {
			    CloseSceneUI<UI_Win>();
		    }
			    break;
	    }
    }
    
	private T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_WorldSpace
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		T worldSpace = Util.GetOrAddComponent<T>(go);
		_worldSpaceList.Add(worldSpace);

		return Util.GetOrAddComponent<T>(go);
	}

	private T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

	private T MakePopupUI<T>(string name = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
		T popup = Util.GetOrAddComponent<T>(go);
		_popupStack.Push(popup);

		go.transform.SetParent(Root.transform);

		return popup;
	}
	
	private T MakeSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Util.GetOrAddComponent<T>(go);
		
		switch (typeof(T).Name)
		{
			case "UI_Black":
				ui_black = sceneUI as UI_Black;
				break;
			case "UI_RoomStart":
				ui_roomstart = sceneUI as UI_RoomStart;
				break;
			case "UI_Win":
				ui_win = sceneUI as UI_Win;
				break;
		}

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	private void CloseWorldSpaceUI(UI_WorldSpace worldSpaceUi)
	{
		if (worldSpaceUi == null) return;
		Managers.Resource.Destroy(worldSpaceUi.gameObject);
	}

	private void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

	private void CloseSceneUI<T>()
	{
		switch (typeof(T).Name)
	    {
		    case "UI_Black":
			    if (ui_black == null) return;
			    Managers.Resource.Destroy(ui_black.gameObject);
			    ui_black = null;
			    break;
		    case "UI_RoomStart":
			    if (ui_roomstart == null) return;
			    Managers.Resource.Destroy(ui_roomstart.gameObject);
			    ui_roomstart = null;
			    break;
		    case "UI_Win":
			    if (ui_win == null) return;
			    Managers.Resource.Destroy(ui_win.gameObject);
			    ui_win = null;
			    break;
	    }
    }
    
	private void CloseAllPopupUI()
    {
	    while (_popupStack.Count > 0)
		    ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        ui_black = null;
        ui_roomstart = null;
        ui_win = null;
    }
}
