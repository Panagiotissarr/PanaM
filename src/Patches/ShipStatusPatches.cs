using HarmonyLib;

namespace PanaM;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ShipStatus_FixedUpdate
{
    public static void Postfix(ShipStatus __instance)
    {
        PanaMSabotageCheats.Process(__instance);
        PanaMCheats.OpenSabotageMapCheat();

        PanaMCheats.CloseMeetingCheat();
        PanaMCheats.SkipMeetingCheat();
        PanaMCheats.CallMeetingCheat();
        PanaMCheats.WalkInVentCheat();
        PanaMCheats.KickVentsCheat();

        PanaMPPMCheats.ReportBodyPPM();
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class FungleShipStatus_FixedUpdate
{
    public static void Postfix(FungleShipStatus __instance)
    {
        PanaMSabotageCheats.ProcessFungle(__instance);
    }
}
