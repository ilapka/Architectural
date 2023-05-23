using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyHealth), typeof(EnemyAnimator))]
    public class EnemyDeath : MonoBehaviour
    {
        [SerializeField]
        private EnemyHealth _health;
        [SerializeField]
        private EnemyAnimator _animator;
        [SerializeField]
        private Follow _follow;
        [SerializeField]
        private Aggro _aggro;
        [SerializeField]
        private GameObject _deathFx;
        public event Action Happened;

        private void Start()
        {
            _health.HealthChanged += HealthChanged;
        }

        private void HealthChanged()
        {
            if (_health.Current <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _health.HealthChanged -= HealthChanged;

            _aggro.enabled = false;
            _follow.SetEnable(false);
            _animator.PlayDeath();

            SpawnDeathFx();
            StartCoroutine(DestroyTimer());

            Happened?.Invoke();
        }

        private void SpawnDeathFx() =>
            Instantiate(_deathFx, transform.position, Quaternion.identity);

        private IEnumerator DestroyTimer()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _health.HealthChanged -= HealthChanged;
        }
    }
}