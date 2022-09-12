using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using PropHuntServer;
using PropHuntServer.GameContent;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom room = player.Room;
        if (room == null) return;

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom room = player.Room;
        if (room == null) return;

        Console.WriteLine($"[Recv] {clientSession.SessionId.ToString("0000")} | {skillPacket}");

        room.Push(room.HandleSkill, player, skillPacket);
    }

    public static void C_ChangeSortLayerHandler(PacketSession session, IMessage packet)
    {
        C_ChangeSortLayer sortLayerChangePacket = packet as C_ChangeSortLayer;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom room = player.Room;
        if (room == null) return;

        Console.WriteLine($"[Recv] {clientSession.SessionId.ToString("0000")} | {sortLayerChangePacket}");

        room.Push(room.HandleChangeSortLayer, player, sortLayerChangePacket);
    }

    public static void C_GameStartHandler(PacketSession session, IMessage packet)
    {
        C_GameStart gameStartPacket = packet as C_GameStart;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom room = player.Room;
        if (room == null) return;

        Console.WriteLine($"[Recv] {clientSession.SessionId.ToString("0000")} | {gameStartPacket}");

        room.Push(room.HandleGameStart, player);
    }

    public static void C_TimerFinishHandler(PacketSession session, IMessage packet)
    {
        C_TimerFinish timerFinishPacket = packet as C_TimerFinish;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom room = player.Room;
        if (room == null) return;

        Console.WriteLine($"[Recv] {clientSession.SessionId.ToString("0000")} | {timerFinishPacket}");

        // 방장만 유효 (1번만 호출하게)
        if (player.Id == room.DetectiveId)
            room.Push(room.RestartRoom, "Prop Man");
    }
}
