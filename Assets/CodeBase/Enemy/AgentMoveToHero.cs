using Infrastructure.Factory;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class AgentMoveToHero : Follow
    {
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private float _minimalDestance = 1f;

        private IGameFactory _gameFactory;
        private Transform _heroTransform;

        private void Start()
        {
            _gameFactory = AllServices.Container.Single<IGameFactory>();

            if (IsHeroExists())
            {
                InitializeHeroTransform();
            }
            else
            {
                _gameFactory.HeroCreated += HeroCreated;
            }
        }

        private void Update()
        {
            if(IsInitialized() && IsHeroNotReached())
                _agent.destination = _heroTransform.position;
        }

        private void HeroCreated()
        {
            InitializeHeroTransform();
        }

        private void InitializeHeroTransform()
        {
            _heroTransform = _gameFactory.HeroGameObject.transform;
        }

        private bool IsInitialized() => _heroTransform != null;

        private bool IsHeroNotReached()
        {
            return Vector3.Distance(_agent.transform.position, _heroTransform.position) >= _minimalDestance;
        }

        private bool IsHeroExists()
        {
            return _gameFactory.HeroGameObject != null;
        }
        
        private void OnDestroy()
        {
            if (_gameFactory != null)
                _gameFactory.HeroCreated -= HeroCreated;
        }
    }
}