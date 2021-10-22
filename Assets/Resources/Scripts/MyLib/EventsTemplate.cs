using System;
using UnityEngine;

namespace EventsTemplate
{
    public class PlayerEvTemplate
    {
        public delegate bool boolAction();
        private DamageCLS damage = new DamageCLS();
        public DamageCLS Damage => damage;
        public class DamageCLS {
            public delegate BaseEffect[] EffectArrayAction();
            public delegate StrenghtPushState[] PushesArrayAction();
            public delegate StrengthExplotionState[] ExplotionsArrayAction();

            // Pushes
            public Action<StrenghtPushState> pushRecieved;
            /// <summary> When an enemy player pushed me with any state </summary>
            public Action onPushRecieved;
            public PushesArrayAction GetActivePushesStates;
            public Action hasNoPushesAffecting;

            // Explotion
            public Action<StrengthExplotionState> recieveExplotion;
            /// <summary> When a bomb explotion reached me </summary>
            public Action onExplotionRecieved;
            public ExplotionsArrayAction GetActiveExplotionStates;
            public Action hasNoExplotionsAffecting;
            public Action pushStateFinished;

            // Effect
            public Action<BaseEffect> applyEffect;
            public EffectArrayAction GetActiveEffects;

            // Freeze
            public Action<float> onFreeze;

            // SpeedUp
            public Action<float, float> onSpeedUp;
        }

        private PushCLS push = new PushCLS();
        public PushCLS Push => push;
        public class PushCLS {
            public delegate StrenghtPushState ActionPushState();

            public ActionPushState GetCurrentPushState;
            public Action onPush;
            public Action onStopPush;
            public Action<PlayerPush.ExhaustState> onReachedExhaustState;
            public Action OnImpactedOne;
        }

        private AimBotCLS aimBot = new AimBotCLS();
        public AimBotCLS AimBot => aimBot;
        public class AimBotCLS {
            public delegate GameObject ActionGameObj();
            public ActionGameObj GetGameObjectOfTarget;
        }

        private SoundTrigger mySound = new SoundTrigger();
        public SoundTrigger Sound => mySound;
        public class SoundTrigger {
            public Action footSteps;
            public Action push;
        }

        // Movement
        public Action onMove;
        public Action onNotMoving;

        // GroundCheck
        public Action onGrounded;
        public Action onNotGrounded;

        //ExtraLife
        public Action onExtraLife;
        public boolAction getExtraLife;
        public Action onLastLife;
    }

    public class BombEvTemplate
    {
        public Action instantExplodeSignal;
        public Action startExplodeTimer;
        public Action onReachPatrolPoint;
        public Action onPatrol;
        public Action onChase;
    }

    public class MapEvTemplate
    {
        public delegate GameObject[] GameObjArrayAction();
        public delegate MapBound MapBoundAction();

        public Action hasDropped;
        public Action hasDestroyed;
        public GameObjArrayAction GetActiveMapParts;
        /// <summary> Obtener limites de la plataforma actual </summary>
        public MapBoundAction GetCurrentMapBound;
    }

    public class GameEvTemplate 
    {
        public Action onGameReady;
    }

}
