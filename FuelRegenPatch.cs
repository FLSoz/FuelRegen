using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace FuelRegen
{
    public class FuelRegenPatch : ModBase
    {
        internal static Harmony harmony;

        public override void DeInit()
        {
            harmony.UnpatchAll("com.flsoz.ttmods.fuelregenpatch");
        }

        public override void Init()
        {
            harmony = new Harmony("com.flsoz.ttmods.fuelregenpatch");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(TechBooster), "FixedUpdate")]
        public static class Patch
        {
            private static MethodInfo get_Boosters = typeof(TechBooster).GetProperty("BoostersFiring", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod();

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
