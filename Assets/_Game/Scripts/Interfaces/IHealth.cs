namespace DLS.Game.Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }
        bool IsAlive { get; set; }

        void SetHealth(int amount);
        void AddHealth(int amount);
        void RemoveHealth(int amount);
        void Die();
    }
}
