using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace FuelRegen
{
    public class FuelRegenPatch : ModBase
    {
        internal static Harmony harmony;
        internal const float scale = 2.0f;

        public override void DeInit()
        {
            harmony.UnpatchAll("com.flsoz.ttmods.fuelregenpatch");

            Singleton.camera.farClipPlane /= scale;

            CameraManager manCamera = Singleton.Manager<CameraManager>.inst;
            manCamera.SetDetailDist01(manCamera.DetailDist01 / scale);
            manCamera.SetDrawDist01(manCamera.DrawDist01 / scale);
        }

        public override void Init()
        {
            harmony = new Harmony("com.flsoz.ttmods.fuelregenpatch");
            harmony.PatchAll();

            Singleton.camera.farClipPlane *= scale;

            CameraManager manCamera = Singleton.Manager<CameraManager>.inst;
            manCamera.SetDetailDist01(manCamera.DetailDist01);
            manCamera.SetDrawDist01(manCamera.DrawDist01);
        }

        [HarmonyPatch(typeof(CameraManager), "SetDrawDist01")]
        public static class PatchDrawDist
        {
            public static void Prefix(ref float drawDist01)
            {
                drawDist01 = drawDist01 * scale;
            }
        }
        [HarmonyPatch(typeof(CameraManager), "SetDetailDist01")]
        public static class PatchDetailDist
        {
            public static void Prefix(ref float detailDist01)
            {
                detailDist01 = detailDist01 * scale;
            }
        }

        [HarmonyPatch(typeof(TechBooster), "FixedUpdate")]
        public static class Patch
        {
            private static readonly MethodInfo get_Boosters = typeof(TechBooster).GetProperty("BoostersFiring", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod();

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int status = 0;
                foreach (var instruction in instructions)
                {
                    if (status == 0 && instruction.Calls(get_Boosters))
                    {
                        status = 1;
                    }

                    if (status == 0 || status > 3)
                    {
                        yield return instruction;
                    }
                    
                    if (status != 0 && status < 4) {
                        status++;
                    }
                }
            }
        }
    }
}
