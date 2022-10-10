using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Models
{
    [Serializable]
    public struct Point
    {
        #region Properties

        public float PosX { get; }
        public float PosY { get; }
        public float PosZ { get; }

        #endregion

        #region Constructors

        public Point(float posX, float posY, float posZ)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
        }

        public Point(Vector3 position)
        {
            PosX = position.x;
            PosY = position.y;
            PosZ = position.z;
        }

        #endregion
    }
}