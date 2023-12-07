using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Analytics;

namespace FloopyLessRogueLikeMoreRogueLite
{
    [BepInPlugin("pop.mods.creditrollover", "CreditRollover", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"CreditRollover has loaded :)");
            Harmony.CreateAndPatchAll(typeof(PatchCreditsToRollOver));
            Logger.LogInfo($"CreditRollover is finished patching :D");

        }
    }
}

class PatchCreditsToRollOver
{


    static private int creds = 0;
    [HarmonyPatch(typeof(GameNetworkManager), "ResetSavedGameValues")]
    [HarmonyPrefix]   
    static void GrabCreditAmountBeforeWipe(ref GameNetworkManager __instance)
    {
        
        // __state = ES3.Load<int>("GroupCredits", __instance.currentSaveFileName);
        creds = GameObject.FindObjectOfType<Terminal>().groupCredits;
        // UnityEngine.Debug.Log($"We have {creds} creds");

    }
    
    // I don't really think this does anything
    [HarmonyPatch(typeof(GameNetworkManager), "ResetSavedGameValues")]
    [HarmonyPostfix]
    static void SetCredit(ref GameNetworkManager __instance)
    {
        // UnityEngine.Debug.Log($"We have {creds} creds");
        ES3.Save<int>("GroupCredits", creds, __instance.currentSaveFileName);
    }
    [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
    [HarmonyPostfix]
    static void SetCreditTerminal(ref StartOfRound __instance)
    {
        // UnityEngine.Debug.Log($"We have {creds} creds");
        GameObject.FindObjectOfType<Terminal>().groupCredits = creds;
    }


}