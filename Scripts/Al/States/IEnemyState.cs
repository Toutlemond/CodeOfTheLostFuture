namespace Al.States
{
    public interface IEnemyState
    {
        void Enter(Enemy enemy);
        void Update();
        void Exit();
    }
}