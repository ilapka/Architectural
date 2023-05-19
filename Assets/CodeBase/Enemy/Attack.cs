using System;
using System.Linq;
using Infrastructure.Factory;
using Infrastructure.Services;
using UnityEngine;

namespace Enemy
{   
    [RequireComponent(typeof(EnemyAnimator))]
    public class Attack : MonoBehaviour
    {
        [SerializeField]
        private EnemyAnimator _animator;
        [SerializeField]
        private float _attackCooldown = 3f;
        [SerializeField]
        private float _cleavage = 0.5f;
        [SerializeField]
        private float _effectiveDistance = 0.5f;

        private IGameFactory _factory;
        
        private Transform _heroTransform;
        private int _layerMask;
        private float _attackCooldownTimer;
        private bool _attackIsActive;
        private bool _isAttacking;
        private Collider[] _hits = new Collider[1];

        private void Awake()
        {
            _factory = AllServices.Container.Single<IGameFactory>();
            
            _layerMask = 1 << LayerMask.NameToLayer("Player");
            
            _factory.HeroCreated += OnHeroCreated;
        }

        private void Update()
        {
            UpdateCooldown();

            if(CanAttack())
                StartAttack();
        }

        private void UpdateCooldown()
        {
            if(!CooldownIsUp()) 
                _attackCooldown -= Time.deltaTime;
        }

        private void OnAttack()
        {
            if (Hit(out Collider hit))
            {
                PhysicsDebug.DrawDebug(StartPoint(), _cleavage, 1f);
            }
        }

        public void EnableAttack() => _attackIsActive = true;
        public void DisableAttack() => _attackIsActive = false;

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(StartPoint(), _cleavage, _hits, _layerMask);
            
            hit = _hits.FirstOrDefault();
            
            return hitsCount > 0;
        }

        private Vector3 StartPoint() =>
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) + transform.forward * _effectiveDistance;

        private void OnAttackEnded()
        {
            _attackCooldownTimer = _attackCooldown;
            _isAttacking = false;
        }

        private bool CooldownIsUp()
        {
            return _attackCooldown <= 0f;
        }

        private void StartAttack()
        {
            transform.LookAt(_heroTransform);
            _animator.PlayAttack();

            _isAttacking = true;
        }

        private bool CanAttack() => _attackIsActive && !_isAttacking && CooldownIsUp();
        private void OnHeroCreated() => _heroTransform = _factory.HeroGameObject.transform;

        private void OnDestroy()
        {
            _factory.HeroCreated -= OnHeroCreated;
        }
    }
}