using System.Collections.Generic;

namespace TowerDefense.Interfaces
{
    /// <summary>
    /// Strategy pattern for tower shot behaviour.
    /// Adding a new tower type = adding a new implementation, not editing Tower.cs.
    /// </summary>
    public interface ITowerStrategy
    {
        /// <summary>
        /// Execute the tower's attack given a list of enemies currently in range.
        /// The strategy decides how many to target and what projectile to fire.
        /// </summary>
        void Execute(List<Enemy> enemiesInRange, Tower owner);
    }
}
