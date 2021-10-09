using System;
using System.Reflection;
using HarmonyLib;
using NitroxClient.GameLogic;
using NitroxClient.GameLogic.HUD;
using NitroxClient.GameLogic.Simulation;
using NitroxClient.MonoBehaviours;
using NitroxModel.Core;
using NitroxModel.DataStructures;
using NitroxModel.Logger;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic
{
    public class BuilderTool_Construct_Patch_Copy : NitroxPatch, IDynamicPatch
    {
        public static readonly Type TARGET_CLASS = typeof(BuilderTool);
        public static readonly MethodInfo TARGET_METHOD = TARGET_CLASS.GetMethod("Construct", BindingFlags.NonPublic | BindingFlags.Instance);

        private static SimulationOwnership simulationOwnership = NitroxServiceLocator.LocateService<SimulationOwnership>();

        private static bool lastState = true;
        private static BuilderTool lastTool;
        private static bool skipPrefixPatch = false;

        public static bool Prefix(BuilderTool __instance, Constructable target, bool state)
        {
            if (skipPrefixPatch)
            {
                return true;
            }
            
            NitroxId id = NitroxEntity.GetId(target.gameObject);

            if (simulationOwnership.HasExclusiveLock(id))
            {
                Log.Debug($"Already have an exclusive lock on the targetted constructable: {id}");
                return true;
            }

            lastState = state;
            lastTool = __instance;

            BuilderToolConstructContext context = new BuilderToolConstructContext(target);
            LockRequest<BuilderToolConstructContext> lockRequest = new LockRequest<BuilderToolConstructContext>(id, SimulationLockType.EXCLUSIVE, ReceivedSimulationLockResponse, context);

            simulationOwnership.RequestSimulationLock(lockRequest);

            return false;
        }

        private static void ReceivedSimulationLockResponse(NitroxId id, bool lockAquired, BuilderToolConstructContext context)
        {
            if (lockAquired)
            {
                skipPrefixPatch = true;
                TARGET_METHOD.Invoke(lastTool, new object[] { context.constructable, lastState });
                skipPrefixPatch = false;
            }
            else
            {
                ErrorMessage.AddMessage((lastState) ? $"Cannot construct." : $"Cannot deconstruct.");
                ErrorMessage.AddDebug($"lockAcquired failed");
            }
        }

        public override void Patch(Harmony harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
