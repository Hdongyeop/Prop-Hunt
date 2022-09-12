using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace PropHuntServer.GameContent
{
	public class GameObject
	{
		public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
		public int Id
		{
			get { return Info.ObjectId; }
			set { Info.ObjectId = value; }
		}

		public GameRoom Room { get; set; }

		public ObjectInfo Info { get; set; } = new ObjectInfo();
		public PositionInfo PosInfo { get; private set; } = new PositionInfo();
		public StatInfo Stat { get; set; } = new StatInfo();

        public MoveDir Dir
		{
			get { return PosInfo.MoveDir; }
			set { PosInfo.MoveDir = value; }
		}

		public CreatureState State
		{
			get { return PosInfo.State; }
			set { PosInfo.State = value; }
		}

		public GameObject()
		{
			Info.PosInfo = PosInfo;
			Info.StatInfo = Stat;
		}

		public virtual void Update()
		{

		}

		public Pos CellPos
		{
			get
			{
				return new Pos(PosInfo.PosX, PosInfo.PosY);
			}

			set
			{
				PosInfo.PosX = value.X;
				PosInfo.PosY = value.Y;
			}
		}

		public virtual void OnDamaged(GameObject attacker)
		{
			if (Room == null)
				return;
        }

		public virtual void OnDead(GameObject attacker)
		{
			if (Room == null)
				return;
        }
	}
}
