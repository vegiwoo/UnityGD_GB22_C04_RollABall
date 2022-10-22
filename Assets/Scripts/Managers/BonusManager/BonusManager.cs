using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [RequireComponent(typeof(AudioIsPlaying))]
    public partial class BonusManager : BaseManager
    {
        #region Links
        
        [SerializeField] private EffectStats stats;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;
        [field: SerializeField] private BonusRepository BonusRepository { get; set; }
        [field: SerializeField] private EffectRepository EffectRepository { get; set; }
        [field: SerializeField] private ApplyEffectEvent ApplyEffectEvent { get; set; }
        [field: SerializeField] private BonusManagerEvent BonusManagerEvent { get; set; }

        #endregion
        
        #region Properties

        #endregion
        
        #region Fields

        private AudioIsPlaying _audioIsPlaying;
        
        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _audioIsPlaying = GetComponent<AudioIsPlaying>();
        }

        #endregion
        
        #region Functionality
        
        protected override void InitManager(InitItemMode mode)
        {
            StopAllCoroutines();

            switch (mode)
            {
                case InitItemMode.NewGame:
                    
                    NewGameProcedure();

                    break;
                case InitItemMode.RestartGame:
                    
                    RestartGameProcedure();

                    break;
                case InitItemMode.LoadGame:
                    
                    LoadGameProcedure();

                    break;
            }
            
            // Placement notify
            BonusManagerEvent.Notify(new BonusManagerArgs(bonusPoints,null,null,false));
        }

        private void LoadGameProcedure()
        {
            // Clear repo
            BonusRepository.UpdateAllWithAction(UnsubscribeAndDestroyAction);
            BonusRepository.RemoveAll();

            // Cleanup notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null, null, null, true));

            // Make items from loading file
            const float pointTolerance = 0.05f;

            foreach (var bonusPoint in bonusPoints)
            {
                var entities = State.OfType<BonusSaveArgs>().Where(it =>
                {
                    Vector3 position;
                    return Math.Abs(it.Point.PosX - (position = bonusPoint.position).x) < pointTolerance &&
                           Math.Abs(it.Point.PosZ - position.z) < pointTolerance;
                }).ToList();

                var pair = new KeyValuePair<Transform, BonusItem[]>(bonusPoint, new BonusItem[2]);

                for (var i = 0; i < entities.Count(); i++)
                {
                    var bonusItem = CreateBonusAndObject(null, entities[i].Effect, bonusPoint, entities[i].IsActive);
                    pair.Value[i] = bonusItem;
                }

                try
                {
                    BonusRepository.Insert(pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }

        private void RestartGameProcedure()
        {
            if (BonusRepository.Count > 0)
            {
                BonusRepository.UpdateAllWithAction(UnsubscribeAndUnactivateAction);
            }

            BonusRepository.UpdateAllWithAction(RandomActivateAndSubscribeAction);
        }

        private void NewGameProcedure()
        {
            // Manager state initialization.
            State = new List<ISavableArgs>(new List<BonusSaveArgs>());

            var counter = 0;
            while (counter < bonusPoints.Length)
            {
                var point = bonusPoints[counter];

                var buffBonusItem = CreateBonusAndObject(EffectType.Buff, null, point);
                var debuffBonusItem = CreateBonusAndObject(EffectType.Debuff, null, point);

                try
                {
                    BonusRepository.Insert(point, new[] { buffBonusItem, debuffBonusItem });
                }
                catch (Exception e)
                {
                    LogException(e);
                }

                ++counter;
            }

            BonusRepository.UpdateAllWithAction(RandomActivateAndSubscribeAction);
        }

        // Event handler for CurrentGameEvent
        
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Restart game
            if (args.IsRestartGame)
            {
                InitManager(InitItemMode.RestartGame);
            }

            // Save game 
            if (args.IsSaveGame)
            {
                var items =
                    from bonusPair in BonusRepository.FindAllValues()
                    from bonus in bonusPair
                    select new BonusSaveArgs(bonus.Bonus.Point.position, bonus.Bonus.Effect as Effect,
                        bonus.BonusGo.activeInHierarchy);

                State = new List<ISavableArgs>(items.ToList());
                
                PassStateEvent.Notify(State);
            }

            // Load game
            if (args.SaveGameArgs != null)
            {
                State = new List<ISavableArgs>(args.SaveGameArgs.BonusSaveArgs);
                InitManager(InitItemMode.LoadGame);
            }
        }   
        
        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        #endregion
    }
}