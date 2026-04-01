namespace TowerDefense.Interfaces
{
    /// <summary>
    /// Anything that can receive damage implements this.
    /// Keeps targeting logic decoupled from concrete types.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
        bool IsDead { get; }
    }
}
