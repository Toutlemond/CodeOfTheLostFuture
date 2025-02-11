using JetBrains.Annotations;

namespace Al.States
{
    public class DeactivatedState : IEnemyState
    {
        private Enemy _enemy;
        private System.Action _onUpdate;
        private System.Action _onStart;
        
        public DeactivatedState(Enemy enemy, System.Action onUpdate, [CanBeNull] System.Action onStart)
        {
            _enemy = enemy;
            _onUpdate = onUpdate;
            _onStart = onStart;
        }
        public void Enter(Enemy enemy)
        {
            // Логика входа в состояние
            _onStart?.Invoke();
        }

        public void Update()
        {
            // Логика обновления состояния
            _onUpdate?.Invoke();
        }

        public void Exit()
        {
            // Логика выхода из состояния
        }
    }
}