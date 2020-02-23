using System;
using Prime31;
using UnityEngine;

namespace Mercury
{
    [RequireComponent(typeof(CharacterController2D))]
    public class PlayerInterface : MonoBehaviour
    {
        private EntityPlayer _player;

        public EntityPlayer Player
        {
            get => _player;
            set
            {
                if (_player != null)
                {
                    throw new InvalidOperationException("已经设置过player了");
                }

                _player = value;
            }
        }

        public CharacterController2D CharacterController { get; private set; }

        private void Awake() { CharacterController = GetComponent<CharacterController2D>(); }

        private void Update() { _player?.OnUpdate(); }
    }
}