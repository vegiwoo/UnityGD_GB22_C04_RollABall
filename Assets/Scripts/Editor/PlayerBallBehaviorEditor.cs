using System;
using RollABall.Player;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Editor
{
    [CustomEditor(typeof(PlayerBall))]
    public class PlayerBallBehaviorEditor : UnityEditor.Editor
    {
        private PlayerBall _playerBallTarget;
        private float _playerBallScale;

        private void OnEnable()
        {
            _playerBallTarget =  (PlayerBall)target;
            _playerBallScale = _playerBallTarget.transform.localScale.x;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            
            EditorGUILayout.LabelField("Editor Extension", EditorStyles.boldLabel);
            
            _playerBallScale = EditorGUILayout.Slider("Ball scale", _playerBallScale, 0.3f, 1.3f);
            _playerBallTarget.transform.localScale = new Vector3(_playerBallScale, _playerBallScale, _playerBallScale);
        }
    }
}