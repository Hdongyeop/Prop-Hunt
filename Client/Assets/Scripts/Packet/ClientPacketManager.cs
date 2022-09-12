using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSprite, MakePacket<S_ChangeSprite>);
		_handler.Add((ushort)MsgId.SChangeSprite, PacketHandler.S_ChangeSpriteHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSortLayer, MakePacket<S_ChangeSortLayer>);
		_handler.Add((ushort)MsgId.SChangeSortLayer, PacketHandler.S_ChangeSortLayerHandler);		
		_onRecv.Add((ushort)MsgId.SShowUi, MakePacket<S_ShowUi>);
		_handler.Add((ushort)MsgId.SShowUi, PacketHandler.S_ShowUiHandler);		
		_onRecv.Add((ushort)MsgId.SCloseUi, MakePacket<S_CloseUi>);
		_handler.Add((ushort)MsgId.SCloseUi, PacketHandler.S_CloseUiHandler);		
		_onRecv.Add((ushort)MsgId.SGameStartDetective, MakePacket<S_GameStartDetective>);
		_handler.Add((ushort)MsgId.SGameStartDetective, PacketHandler.S_GameStartDetectiveHandler);		
		_onRecv.Add((ushort)MsgId.SGameStartPropman, MakePacket<S_GameStartPropman>);
		_handler.Add((ushort)MsgId.SGameStartPropman, PacketHandler.S_GameStartPropmanHandler);		
		_onRecv.Add((ushort)MsgId.SGameStartFailed, MakePacket<S_GameStartFailed>);
		_handler.Add((ushort)MsgId.SGameStartFailed, PacketHandler.S_GameStartFailedHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}