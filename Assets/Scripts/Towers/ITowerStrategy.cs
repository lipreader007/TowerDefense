using System.Collections.Generic;
using TowerDefense.Entities;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Strategy pattern for tower shot behaviour.
    /// Adding a new tower type = adding a new implementation, not editing Tower.cs.
    /// </summary>
    public interface ITowerStrategy
    {
        void Execute(List<Enemy> enemiesInRange, Tower owner);
    }
}