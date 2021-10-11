using UnityEngine;

namespace NitroxClient.GameLogic.Simulation
{
    public class BuilderToolConstructContext : LockRequestContext
    {
        public Constructable constructable { get; }
        public BuilderTool tool { get; }

        public BuilderToolConstructContext(BuilderTool b, Constructable c)
        {
            this.constructable = c;
            this.tool = b;
        }
    }
}
