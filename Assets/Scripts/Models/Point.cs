using System;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Models
{
    [Serializable]
    public class Point
    {
        #region Properties

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        #endregion

        #region Constructors

        [JsonConstructor]
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

        public override string ToString()
        {
            return $"\"PosX\":{PosX}, \"PosY\":{PosY}, \"PosZ\":{PosZ}";
        }

        #endregion
    }
}