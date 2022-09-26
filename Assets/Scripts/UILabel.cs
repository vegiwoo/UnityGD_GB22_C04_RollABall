using RollABall.Args;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.UI
{
    [RequireComponent(typeof(Text))]
    public class UILabel : MonoBehaviour
    {
        #region Fields
        
        private Text _label;
        
        #endregion
        
        #region Properties

        [field: SerializeField] private string Prefix { get; set; }
        [field: SerializeField] private string Suffix { get; set; }
        
        #endregion
        
        #region MonoBehaviour methods
        
        private void Awake()
        {
            _label = GetComponent<Text>();
        }

        private void Start()
        {
            UpdateLabel(new UILabelArgs(Prefix, Suffix, Color.white));
        }

        #endregion
        
        #region Functionality

        public void UpdateLabel(UILabelArgs args)
        {
            _label.color = args.Color;
            _label.text = $"{args.Prefix}: {args.Suffix}".ToUpper();
        }
        
        #endregion
    }
}