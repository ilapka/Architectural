using Infrastructure.Services;
using Infrastructure.States;
using UI;
using UI.Elements;

namespace Infrastructure
{
    public class Game
    {
        public GameStateMachine StateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain curtain)
        {
            SceneLoader sceneLoader = new SceneLoader(coroutineRunner);
            
            StateMachine = new GameStateMachine(sceneLoader, curtain, AllServices.Container);
        }
    }
}