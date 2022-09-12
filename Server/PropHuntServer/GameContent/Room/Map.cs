using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace PropHuntServer.GameContent
{
    public struct Pos
    {
        public Pos(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float Y;
        public float X;
    }

    public class Map
    {
        public bool ApplyMove(GameObject gameObject, Pos dest)
        {
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.PosInfo;

            // 서버 상 좌표 이동
            posInfo.PosX = dest.X;
            posInfo.PosY = dest.Y;
            return true;
        }
    }
}
