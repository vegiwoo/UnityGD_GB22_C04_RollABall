using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [Serializable]
    public class BonusManagerStateItems
    {
        private IEnumerable<BonusManagerStateItem> Bonuses { get; set; }

        public BonusManagerStateItems(IEnumerable<BonusManagerStateItem> bonuses)
        {
            Bonuses = bonuses;
        }
    }
}