using HarmonyLib;
using RimWorld;
using Verse;

namespace MPCanSleepFix;

[StaticConstructorOnStartup]
public static class MPCanSleepFix
{
    private const int BufferTicks = 1_000;

    static MPCanSleepFix()
    {
        Harmony harmony = new("NachoToast.MPCanSleepFix");

        harmony.Patch(
            original: AccessTools.Method(
                type: typeof(JobGiver_GetRest),
                name: nameof(JobGiver_GetRest.GetPriority)),
            prefix: new HarmonyMethod(
                methodType: typeof(MPCanSleepFix),
                methodName: nameof(GetRestPriority_Prefix)));
    }

    private static void GetRestPriority_Prefix(Pawn pawn)
    {
        int? sleepTick = pawn?.mindState?.canSleepTick;

        if (!sleepTick.HasValue)
        {
            return;
        }

        if (Find.TickManager.TicksGame + BufferTicks < sleepTick)
        {
            Log.Message($"[MPCanSleepFix] Fixing pawn {pawn} (game = {Find.TickManager.TicksGame}, sleep = {sleepTick})");
            pawn.mindState.canSleepTick = Find.TickManager.TicksGame;
        }
    }
}
