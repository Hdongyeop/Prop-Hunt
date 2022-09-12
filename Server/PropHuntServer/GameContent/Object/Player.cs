using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace PropHuntServer.GameContent
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }
        public int CurPropNum = -2;
        public string sortingLayerName = "Layer 1";

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public override void OnDamaged(GameObject attacker)
        {
            // 맞았을 떄 추가적인 대응을 할려면 여기

            OnDead(attacker);
        }

        public override void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            PosInfo.State = CreatureState.Dead;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);

            // 살아있는 사람이 없으면 결과 UI 띄우고 게임 다시 시작
            if (Room.CountAlive() == 0)
            {
                Room.RestartRoom("Detective");
            }
        }
    }
}
