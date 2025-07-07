using System;
using UnityEngine;
using static DanthoLogic.CharacterController;

namespace DanthoLogic
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "DanthoLogic/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] PlayerSettings playerSettings;
        public PlayerSettings PlayerStng => playerSettings;

        [Serializable]
        public class PlayerSettings
        {
            [Header("Speeds")]

            public SpeedParam speed0;
            public SpeedParam speed1;
            public SpeedParam speedMax;

            [Space]

            public SpeedParam slowmoSpeed;
            public SpeedParam speedBoost;

            [Space]

            public float strafeSpeed;
            public float strafeSpeedDuringSpeedBoost;
            public float transiDurationAfterCollisionDuringSpeedBoost;
            public float rotationDuration;

            [Header("Punch")]
            public float hitRange;
        }
    }
}