using System.Linq;
using Logic;
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
        
        public float EffectiveDistance = 0.5f;
        public float Cleavage = 0.5f;
        public float Damage;
        
        private Transform _heroTransform;
        private int _layerMask;
        private float _attackCooldownTimer;
        private bool _attackIsActive;
        private bool _isAttacking;
        private Collider[] _hits = new Collider[1];

        public void Construct(Transform heroTransform)
        {
            _heroTransform = heroTransform;
        }

        private void Awake()
        {
            _layerMask = 1 << LayerMask.NameToLayer("Player");
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
                PhysicsDebug.DrawDebug(StartPoint(), Cleavage, 1f);
                hit.transform.GetComponent<IHealth>().TakeDamage(Damage);
            }
        }

        public void EnableAttack() => _attackIsActive = true;
        public void DisableAttack() => _attackIsActive = false;

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(StartPoint(), Cleavage, _hits, _layerMask);
            
            hit = _hits.FirstOrDefault();
            
            return hitsCount > 0;
        }

        private Vector3 StartPoint() =>
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) + transform.forward * EffectiveDistance;

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
    }
}