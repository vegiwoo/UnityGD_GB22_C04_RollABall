using GameDevLib.Audio;
using RollABall.Events;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;

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

        [field: Header("Memento")] 
        [field: SerializeField] private SaveGameEvent SaveGameEvent { get; set; }
        
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

        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        #endregion
    }
}