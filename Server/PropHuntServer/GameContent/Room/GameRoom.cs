using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using PropHuntServer.Data;

namespace PropHuntServer.GameContent
{
    public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }

		Dictionary<int, Player> _players = new Dictionary<int, Player>();
        public Map Map { get; private set; } = new Map();
        private int _detectiveId = 0;
        public int DetectiveId => _detectiveId;
        private int _roomMasterId = 0;
        private bool _gameStart = false;

		public void Init(int mapId)
		{

		}

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			Flush();
		}

		public void EnterGame(GameObject gameObject)
		{
			if (gameObject == null)
				return;

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = gameObject as Player;
				_players.Add(gameObject.Id, player);
				player.Room = this;

				Map.ApplyMove(player, new Pos(player.CellPos.X, player.CellPos.Y));

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = player.Info;
					player.Session.Send(enterPacket);

					// 다른 플레이어들 정보 전달
                    S_Spawn spawnPacket = new S_Spawn();
					foreach (Player p in _players.Values)
					{
						if (player.Id != p.Id)
                        {
                            spawnPacket.Objects.Add(p.Info);
						}
                    }
                    player.Session.Send(spawnPacket);

					// 다른 플레이어들 변신 정보 전달
					S_ChangeSprite changeSpritePacket = new S_ChangeSprite();
                    // 다른 플레이어들 sortLayer 정보 전달
                    S_ChangeSortLayer sortLayerChangePacket = new S_ChangeSortLayer();

					foreach (Player p in _players.Values)
                    {
                        if (player != p)
                        {
                            if(p.Id != _detectiveId)
                            {
                                changeSpritePacket.ObjectId = p.Id;
                                changeSpritePacket.PropId = p.CurPropNum;
                                player.Session.Send(changeSpritePacket);
                            }
                            
                            sortLayerChangePacket.ObjectId = p.Id;
                            sortLayerChangePacket.LayerName = p.sortingLayerName;
                            player.Session.Send(sortLayerChangePacket);
                        }
                    }

                    // PlayerName 켜기
                    UI.Instance.PlayerNameOnOff(player.Id, true);

					// 만약 Room에 접속했는데 혼자라면 StartUI on
                    if(_players.Count == 1)
                    {
                        UI.Instance.StartBtnOnOff(player.Id, true);
                        _roomMasterId = player.Id;
                    }
                }
			}

			// 타인한테 정보 전송
			{
				S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);

                foreach (Player p in _players.Values)
				{
                    if (p.Id != gameObject.Id)
                    {
                        p.Session.Send(spawnPacket);

                        // 게임이 시작했으면 탐정에게는 name ui 안보냄
                        if(_gameStart && p.Id == _detectiveId) continue;
                        UI.Instance.PlayerNameOnOff(p.Id, true);
                    }
                }
			}
        }

		public void LeaveGame(int objectId)
		{
			GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

			if (type == GameObjectType.Player)
			{
                // GameRoom에서 정리
				Player player = null;
				if (_players.Remove(objectId, out player) == false)
					return;

                // ObjectManager에서 정리
                ObjectManager.Instance.Remove(player.Id);

				player.Room = null;

				// 본인한테 정보 전송
				{
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}

                // 나가는 사람이 방장이였으면 다른 사람을 방장으로
				if (player.Id == _roomMasterId && _players.Count > 0)
                {
                    Player newRoomMaster = _players.ElementAt(0).Value;
                    if (newRoomMaster != null)
                    {
                        _roomMasterId = newRoomMaster.Id;
                    }
                }

                // 나가는 사람이 탐정이였으면 처음 상태로
                if (player.Id == _detectiveId && _players.Count > 0)
                {
                    // 이 Room 게임 종료
                    _gameStart = false;
                    _detectiveId = 0;

                    // 방장에게 startBtn UI 켜기
                    UI.Instance.StartBtnOnOff(_roomMasterId, true);
                    // 모두 타이머 끄기
                    foreach (var p in _players.Values)
                        UI.Instance.TimerOnOff(p.Id, false);
                    
                }
            }

			// 타인한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.ObjectIds.Add(objectId);
				foreach (Player p in _players.Values)
				{
					if (p.Id != objectId)
						p.Session.Send(despawnPacket);
				}
			}
        }

		public void HandleMove(Player player, C_Move movePacket)
		{
			if (player == null)
				return;
			
			PositionInfo movePosInfo = movePacket.PosInfo;
			ObjectInfo info = player.Info;

            info.PosInfo.State = movePosInfo.State;
			info.PosInfo.MoveDir = movePosInfo.MoveDir;
			Map.ApplyMove(player, new Pos(movePosInfo.PosX, movePosInfo.PosY));
            
			// 다른 플레이어한테도 알려준다
			S_Move resMovePacket = new S_Move();
			resMovePacket.ObjectId = player.Info.ObjectId;
			resMovePacket.PosInfo = movePacket.PosInfo;

			Broadcast(resMovePacket);
		}

		public void HandleSkill(Player player, C_Skill skillPacket)
		{
			if (player == null)
				return;

			ObjectInfo info = player.Info;

            // 이동 중이여도 스킬 가능
            //if (info.PosInfo.State == CreatureState.Skill)
            //    return;

			// 탐정 / 숨는사람에 따라서 다르게 broadcast
			info.PosInfo.State = CreatureState.Skill;

			if (player.Id == _detectiveId)
            {
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = info.ObjectId;
				// TODO 일단 평타로 쓰도록 함
                skill.Info.SkillId = 1;
				Broadcast(skill);

                Skill skillData = null;
				// TODO 일단 평타
                if (DataManager.SkillDict.TryGetValue(1, out skillData) == false)
                    return;

                switch (skillData.id)
                {
					// 평타
                    case 1:
                    {
                        List<Player> list = PlayersInRect(player, skillData.area);
                        foreach (var p in list)
                        {
                            // 나 자신이면 PASS
							if(p.Id == player.Id) continue;

                            // 같은 층 아니면 PASS
                            if(p.sortingLayerName[^1] != player.sortingLayerName[^1]) continue;

                            Console.WriteLine($"{p.Info.Name} HAS DAMAGED");
                            p.OnDamaged(player);
                        }
                    }
                        break;
					// 몽둥이
                    case 2:
                    {

                    }
                        break;
                }
			}
            else
            {
				// 물건 -> 사람
                if (player.CurPropNum >= 0)
                {
					// -2 : 사람, -1 : 탐정, 0 ~ N : 물건 번호
                    player.CurPropNum = -2;

                    S_ChangeSprite changePacket = new S_ChangeSprite();
                    changePacket.ObjectId = info.ObjectId;
                    changePacket.PropId = -2;
                    Broadcast(changePacket);
				}
				// 사람 -> 물건
                else
                {
                    // 탐정이 아니면 물건 번호로 쓰임
                    player.CurPropNum = skillPacket.Info.SkillId;

                    S_ChangeSprite changePacket = new S_ChangeSprite();
                    changePacket.ObjectId = info.ObjectId;
                    changePacket.PropId = skillPacket.Info.SkillId; 
                    Broadcast(changePacket);
				}
            }
        }

        public void HandleGameStart(Player roomMaster)
        {
            if (_players.Count >= 2)
            {
                // 이 Room 게임 시작
                _gameStart = true;

                // 결과창이 있다면 끄기
                foreach (var p in _players.Values)
                {
                    UI.Instance.WinOnOff(p.Id, false);
                }

                // 이전 탐정 초기화
                if (_detectiveId != 0)
                {
                    if (_players.TryGetValue(_detectiveId, out var oldDetective))
                    {
                        DataManager.StatDict.TryGetValue(1, out var statinfo);
                        oldDetective.Stat.Speed = statinfo.Speed;

                        S_ChangeStat changeStatPacket = new S_ChangeStat();
                        changeStatPacket.ObjectId = _detectiveId;
                        changeStatPacket.StatInfo = oldDetective.Stat;
                        oldDetective.Session.Send(changeStatPacket);
                    }
                }

                // 새로운 탐정을 선정하고 Id 저장
                Random rand = new Random();
                _detectiveId = _players.ElementAt(rand.Next(_players.Count)).Value.Id;

                // StartBtn 끄기
                UI.Instance.StartBtnOnOff(_roomMasterId, false);

                // 탐정
                StartAsDetective();

                // 프롭맨
                StartAsPropman();

                // TImer 켜기
                foreach (var p in _players.Values)
                {
                    UI.Instance.uiInteger = 60;
                    UI.Instance.TimerOnOff(p.Id, true);
                }
            }
            else
            {
                S_GameStartFailed failedPacket = new S_GameStartFailed();
                failedPacket.Reason = GameStartFailReason.Notenoughtpeople;
                roomMaster.Session.Send(failedPacket);
            }
		}

        private void StartAsDetective()
        {
            Player detective = _players[_detectiveId];
            detective.CurPropNum = -1;

            {
                // 탐정이 된 사람에게 변경된 스탯(스피드)를 보냄
                DataManager.StatDict.TryGetValue(3, out var statinfo);

                S_ChangeStat changeStatPacket = new S_ChangeStat();
                changeStatPacket.ObjectId = _detectiveId;
                changeStatPacket.StatInfo = statinfo;
                detective.Session.Send(changeStatPacket);
            }

            {
                // 탐정이 만약 변신 중 이라면 해제
                S_ChangeSprite changePacket = new S_ChangeSprite();
                changePacket.ObjectId = _detectiveId;
                changePacket.PropId = -2;
                Broadcast(changePacket);
            }

            // Black 켜기
            UI.Instance.uiInteger = 10;
            UI.Instance.BlackOnOff(_detectiveId, true);

            // PlayerName 끄기
            UI.Instance.PlayerNameOnOff(_detectiveId, false);

            S_GameStartDetective detectivePacket = new S_GameStartDetective();
            detectivePacket.ObjectId = _detectiveId;
            _players[_detectiveId].Session.Send(detectivePacket);
        }

        private void StartAsPropman()
        {
            S_GameStartPropman propmanPacket = new S_GameStartPropman();

            foreach (Player p in _players.Values)
            {
                if (p.Id == _detectiveId) continue;

                // 딱히 아직 뭐 하는거 없음 이 패킷
                p.Session.Send(propmanPacket);

                // 결과창 끄기
                UI.Instance.WinOnOff(p.Id, false);
            }
        }

        public void HandleChangeSortLayer(Player player, C_ChangeSortLayer sortLayerChangePacket)
        {
            // 누가 어떤 정렬 레이어로 바뀌었대
            player.sortingLayerName = sortLayerChangePacket.LayerName;

            // 오케이 다른애들에게 전달
            C_ChangeSortLayer resPacket = new C_ChangeSortLayer();
            resPacket.ObjectId = player.Id;
            resPacket.LayerName = player.sortingLayerName;
            Broadcast(resPacket);
        }

		public Player FindPlayer(Func<GameObject, bool> condition)
		{
			foreach (Player player in _players.Values)
			{
				if (condition.Invoke(player))
					return player;
			}

			return null;
		}

		public void Broadcast(IMessage packet)
		{
			foreach (Player p in _players.Values)
			{
				p.Session.Send(packet);
			}
		}

        private List<Player> PlayersInRect(Player player, float rectSize)
        {
            List<Player> list = new List<Player>();
            foreach (var p in _players.Values)
            {
                Vector2 a = new Vector2(p.PosInfo.PosX, p.PosInfo.PosY);
                Vector2 b = new Vector2(player.PosInfo.PosX, player.PosInfo.PosY);
                if ((a - b).Length() < rectSize)
                    list.Add(p);
            }

            return list;
        }

        public int CountAlive()
        {
            int res = 0;
            foreach (var p in _players.Values)
            {
                if (p.State == CreatureState.Dead || _detectiveId == p.Id) continue;
                res++;
            }
            return res;
        }

        public void RestartRoom(string winner)
        {
            _gameStart = false;

            // 탐정 : 플레이어들 이름 on
            UI.Instance.PlayerNameOnOff(_detectiveId, true);
            // 방장 : start 버튼 on
            UI.Instance.StartBtnOnOff(_roomMasterId, true);

            // 모든 플레이어 : Win UI on, Timer off, Dead 상태 해제, 변신 해제
            foreach (var p in _players.Values)
            {
                UI.Instance.uiString = $"WINNER\n{winner}";
                UI.Instance.WinOnOff(p.Id, true);

                UI.Instance.TimerOnOff(p.Id, false);

                if (p.State == CreatureState.Dead)
                {
                    p.State = CreatureState.Idle;

                    S_Move movePacket = new S_Move();
                    movePacket.ObjectId = p.Id;
                    movePacket.PosInfo = p.PosInfo;

                    Broadcast(movePacket);
                }

                if (p.CurPropNum >= 0)
                {
                    S_ChangeSprite changeSpritePacket = new S_ChangeSprite();
                    changeSpritePacket.ObjectId = p.Id;
                    changeSpritePacket.PropId = -2;

                    Broadcast(changeSpritePacket);
                }
            }
        }
    }
}
