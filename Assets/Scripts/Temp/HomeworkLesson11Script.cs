#nullable enable

using System;
using System.Collections.Generic;
using RollABall.Test.TempTypes;

// ReSharper disable once CheckNamespace
namespace RollABall.Test
{
    public class HomeworkLesson11Script
    {
        private readonly List<Soldier> _soldiers = new ()
        {
            new Soldier(), new Soldier(), new Soldier()
        };
    
        private readonly List<Panzer> _panzers = new ()
        {
            new Panzer(), new Panzer(), new Panzer()
        };
    
        private delegate Ammunition? HandlerMethod();    
    
        private static Ammunition? AmmunitionHandler() => null;  
    
        private static Bullet? BulletHandler() => null;
    
        public HomeworkLesson11Script()
        {
            foreach (var soldier in _soldiers)
            {
                soldier.SoldierNotify += OnGameUnitNotifyHandler;
            }
        
            foreach (var panzer in _panzers)
            {
                panzer.PanzerNotify += OnGameUnitNotifyHandler;
            }

            HandlerMethod ammunitionHandler = AmmunitionHandler;
            HandlerMethod bulletHandler = BulletHandler;
        }

        private void OnGameUnitNotifyHandler(object sender, EventArgs e)
        {
            // Do something ...
        }
    }
}