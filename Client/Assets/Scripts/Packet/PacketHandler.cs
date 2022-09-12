using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		
		// MyPlayer instantiate
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		
		// Other player instantiate
		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
			
			GameObject go = Managers.Object.FindById(obj.ObjectId);
			PlayerController pc = go.GetComponent<PlayerController>();
			if (pc == null) return;
			
			pc.MoveToNextPos();
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null) return;

		// MyPlayer는 상태만 (좌표는 이미 클라에서 움직였음 나머지는 자율)
		if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
		{
			MyPlayerController mpc = go.GetComponent<MyPlayerController>();
			if (mpc == null) return;
			
			mpc.PosInfo.State = movePacket.PosInfo.State;
			
			return;
		}

		// 다른 플레이어들은 어차피 강제 이동
		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null) return;

		pc.PosInfo = movePacket.PosInfo;
		// pc.PosInfo.PosX = movePacket.PosInfo.PosX;
		// pc.PosInfo.PosY = movePacket.PosInfo.PosY;
		
		pc.MoveToNextPos();
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.UseSkill(skillPacket.Info.SkillId);
		}
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
			cc.OnDead();
	}

	public static void S_ChangeSpriteHandler(PacketSession session, IMessage packet)
	{
		S_ChangeSprite changePacket = packet as S_ChangeSprite;
		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
		{
			pc.ChangeToProp(changePacket.PropId);
			// pc.AnimatorOnOff(true);
		}
	}
	
	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat changeStatPacket = packet as S_ChangeStat;

		GameObject go = Managers.Object.FindById(changeStatPacket.ObjectId);
		if (go == null) return;

		BaseController bc = go.GetComponent<BaseController>();
		bc.Stat = changeStatPacket.StatInfo;
	}

	public static void S_ChangeSortLayerHandler(PacketSession session, IMessage packet)
	{
		S_ChangeSortLayer sortLayerChangePacket = packet as S_ChangeSortLayer;

		GameObject go = Managers.Object.FindById(sortLayerChangePacket.ObjectId);
		if (go == null) return;

		BaseController bc = go.GetComponent<BaseController>();
		bc.SortLayerChange(sortLayerChangePacket.LayerName);
	}

	public static void S_ShowUiHandler(PacketSession session, IMessage packet)
	{
		S_ShowUi showUiPacket = packet as S_ShowUi;
		
		Managers.UI.ShowUI(showUiPacket.UiName, showUiPacket.Integer, showUiPacket.Text);
	}
	
	public static void S_CloseUiHandler(PacketSession session, IMessage packet)
	{
		S_CloseUi closeUiPacket = packet as S_CloseUi;

		Managers.UI.CloseUI(closeUiPacket.UiName);
	}

	public static void S_GameStartDetectiveHandler(PacketSession session, IMessage packet)
	{
		S_GameStartDetective detectivePacket = packet as S_GameStartDetective;
		
		GameObject go = Managers.Object.FindById(detectivePacket.ObjectId);
		if (go == null) return;
		
		BaseController bc = go.GetComponent<BaseController>();
		
		// Speed를 0으로 했다가 10초 후에는 원상태로 복귀
		bc.SpeedChangeAndRollback(0f, bc.Speed);
	}
	
	public static void S_GameStartPropmanHandler(PacketSession session, IMessage packet)
	{
		
	}
	
	public static void S_GameStartFailedHandler(PacketSession session, IMessage packet)
	{
		S_GameStartFailed failedPacket = packet as S_GameStartFailed;
		
		string failReason = Enum.GetName(typeof(GameStartFailReason), failedPacket.Reason); 
		Debug.Log($"GAME START FAILED : {failReason}");
	}
}
