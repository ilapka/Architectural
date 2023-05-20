﻿using System;
using Logic;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyHealth : MonoBehaviour, IHealth
    {
        [SerializeField]
        private EnemyAnimator _animator;
        [SerializeField]
        private float _current;
        [SerializeField]
        private float _max;
        
        public event Action HealthChanged;

        public float Current
        {
            get => _current;
            set => _current = value;
        }

        public float Max
        {
            get => _max;
            set => _max = value;
        }

        public void TakeDamage(float damage)
        {
            Current -= damage;

            _animator.PlayHit();

            HealthChanged?.Invoke();
        }
    }
}