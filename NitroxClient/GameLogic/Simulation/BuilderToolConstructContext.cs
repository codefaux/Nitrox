using UnityEngine;

namespace NitroxClient.GameLogic.Simulation
{
    public class BuilderToolConstructContext : LockRequestContext
    {
        public Constructable constructable { get; }

        public BuilderToolConstructContext(Constructable c)
        {
            this.constructable = c;
        }
    }
}
