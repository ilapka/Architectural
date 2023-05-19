using Infrastructure.Factory;
using Infrastructure.Services;
using UnityEngine;

namespace Enemy
{
    public class AgentRotateToHero : Follow
    { 
        [SerializeField]
        private float _speed = 1f;

        private IGameFactory _gameFactory;
        private Transform _heroTransform;
        private Vector3 _positionToLook;
        
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
            if(IsInitialized())
                RotateTowardsHero();
        }

        private void RotateTowardsHero()
        {
            UpdatePositionToLookAt();
            transform.rotation = SmoothedRotation(transform.rotation, _positionToLook);
        }

        private void UpdatePositionToLookAt()
        {
            Vector3 positionDiff = _heroTransform.position - transform.position;
            _positionToLook = new Vector3(positionDiff.x, transform.position.y, positionDiff.z);
        }

        private Quaternion SmoothedRotation(Quaternion rotation, Vector3 positionToLook)
        {
            return Quaternion.Lerp(rotation, TargetRotation(positionToLook), SpeedFactor());
        }

        private Quaternion TargetRotation(Vector3 position) =>
            Quaternion.LookRotation(position);
        
        private float SpeedFactor() =>
            _speed * Time.deltaTime;

        private void HeroCreated()
        {
            InitializeHeroTransform();
        }

        private void InitializeHeroTransform()
        {
            _heroTransform = _gameFactory.HeroGameObject.transform;
        }

        private bool IsInitialized() => _heroTransform != null;
        
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