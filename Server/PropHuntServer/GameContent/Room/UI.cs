using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace PropHuntServer.GameContent
{
    public class UI
    {
        public static UI Instance { get; } = new UI();

        public int uiInteger = 0;
        public string uiString = String.Empty;

        public void StartBtnOnOff(int objectId, bool onOff)
        {
            if (onOff)
            {
                S_ShowUi showUiPacket = new S_ShowUi();
                showUiPacket.UiName = UiName.Startbtn;
                ObjectManager.Instance.Find(objectId).Session.Send(showUiPacket);
            }
            else
            {
                S_CloseUi closeUiPacket = new S_CloseUi();
                closeUiPacket.UiName = UiName.Startbtn;
                ObjectManager.Instance.Find(objectId).Session.Send(closeUiPacket);
            }
        }

        public void TimerOnOff(int objectId, bool onOff)
        {
            if (onOff)
            {
                S_ShowUi showUiPacket = new S_ShowUi();
                showUiPacket.UiName = UiName.Timer;
                showUiPacket.Integer = uiInteger;
                ObjectManager.Instance.Find(objectId).Session.Send(showUiPacket);
            }
            else
            {
                S_CloseUi closeUiPacket = new S_CloseUi();
                closeUiPacket.UiName = UiName.Timer;
                ObjectManager.Instance.Find(objectId).Session.Send(closeUiPacket);
            }
        }

        public void BlackOnOff(int objectId, bool onOff)
        {
            if (onOff)
            {
                S_ShowUi showUiPacket = new S_ShowUi();
                showUiPacket.UiName = UiName.Black;
                showUiPacket.Integer = uiInteger;
                ObjectManager.Instance.Find(objectId).Session.Send(showUiPacket);
            }
            else
            {
                S_CloseUi closeUiPacket = new S_CloseUi();
                closeUiPacket.UiName = UiName.Black;
                ObjectManager.Instance.Find(objectId).Session.Send(closeUiPacket);
            }
        }

        public void WinOnOff(int objectId, bool onOff)
        {
            if (onOff)
            {
                S_ShowUi showUiPacket = new S_ShowUi();
                showUiPacket.UiName = UiName.Win;
                showUiPacket.Text = uiString;
                ObjectManager.Instance.Find(objectId).Session.Send(showUiPacket);
            }
            else
            {
                S_CloseUi closeUiPacket = new S_CloseUi();
                closeUiPacket.UiName = UiName.Win;
                ObjectManager.Instance.Find(objectId).Session.Send(closeUiPacket);
            }
        }

        public void PlayerNameOnOff(int objectId, bool onOff)
        {
            if (onOff)
            {
                S_ShowUi showUiPacket = new S_ShowUi();
                showUiPacket.UiName = UiName.Username;
                ObjectManager.Instance.Find(objectId).Session.Send(showUiPacket);
            }
            else
            {
                S_CloseUi closeUiPacket = new S_CloseUi();
                closeUiPacket.UiName = UiName.Username;
                ObjectManager.Instance.Find(objectId).Session.Send(closeUiPacket);
            }
        }
    }
}
