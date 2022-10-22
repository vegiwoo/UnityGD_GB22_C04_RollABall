using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{



    // Форматы хранения данных - Json 
    // Количество хранимых сохранений
    
    
    public class GameManager : BaseManager, GameDevLib.Interfaces.IObserver<PlayerArgs>, GameDevLib.Interfaces.IObserver<List<ISavableArgs>>
    {
        #region Links
        
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }

        #endregion

        #region MonoBehaviour methods
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            PassStateEvent.Attach(this);
            PlayerEvent.Attach(this);
        }
        
        #endregion
        
        #region Functionality

        protected override void InitManager(InitItemMode mode)
        {
            State = new List<ISavableArgs>(new List<SaveGameArgs>()) { new SaveGameArgs() };
        }

        // Event handlers
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            if (args.CurrentHp <= 0)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false, false,  false, (true, "You have spent all your hit points :("), null));
            } else if (args.GamePoints >= GameStats.GameHighScore)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false, false,  false, (true, "You have reached required number of points :)")));
            }
        }
        
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Do something...
        }
        
        public void OnEventRaised(ISubject<List<ISavableArgs>> subject, List<ISavableArgs> args)
        {
            try
            {
                var playerSaveArgs = args.Cast<PlayerArgs>().First();
                ((SaveGameArgs)State[0]).PlayerArgs = playerSaveArgs;
                Log("Player save");
            }
            catch (Exception e)
            {
                // ignored
            } 
            
            try
            {
                var effectSaveArgs = args.Cast<EffectSaveArgs>().ToList();
                ((SaveGameArgs)State[0]).EffectSaveArgs = effectSaveArgs;
                Log("Effects save");
            }
            catch (Exception e)
            {
                // ignored
            }
            
            try
            {
                var bonusSaveArgs = args.Cast<BonusSaveArgs>().ToList();
                ((SaveGameArgs)State[0]).BonusSaveArgs = bonusSaveArgs;
                Log("Bonuses save");
            }
            catch (Exception e)
            {
                // ignored
            }
            
            // Точка сохранения
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            PassStateEvent.Detach(this);
            PlayerEvent.Detach(this);
        }

        #endregion
    }
}