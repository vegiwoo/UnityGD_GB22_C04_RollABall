using System;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Models;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    //UIManager + EventHandlers    
    public partial class UIManager
    {
        private void OnRestartButtonClick() => GameEvent.Notify(new CurrentGameArgs(CurrentGameState.Restart, null));
        
        private void OnSaveButtonClick() => GameEvent.Notify(new CurrentGameArgs(CurrentGameState.Save, null));
        
        private void OnLoadButtonClick() => GameEvent.Notify(new CurrentGameArgs(CurrentGameState.Load, null));
        
        // Event handler for PlayerEvent
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            SetValues(args);
        }
        
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Lost game
            if (args.IsLostGame is { isLost: true })
            {
                Log(args.IsLostGame.Value.message);
                IsLostGame = args.IsLostGame.Value;
            }
            else if (args.IsWinGame is { isWin: true })
            {
                Log(args.IsWinGame.Value.message);
                IsWinGame = args.IsWinGame.Value;
            }
        }
        
        public void OnEventRaised(ISubject<BonusManagerArgs> subject, BonusManagerArgs args)
        {
            if (args.ActivatePoints is not null)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();

                foreach (var point in args.ActivatePoints)
                {
                    var newItem = Instantiate(BonusPointImage);

                    try
                    {
                        newItem.transform.parent = RadarMapImage.transform;
                        RadarObjectsRepository.Insert(point, newItem);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                }
            }
            
            if (args.ActivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.ActivatePoint, ActivateRadarPointAction);
            }
            
            if (args.UnactivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.UnactivatePoint, UnactivateRadarPointAction);
            }
            
            if (args.RemoveAll)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();
            }
        }
    }
}

