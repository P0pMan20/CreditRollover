using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using FloopyLessRogueLikeMoreRogueLite;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Analytics;

namespace FloopyLessRogueLikeMoreRogueLite
{
    // Thanks floop for giving me the idea
    [BepInPlugin("pop.mods.creditrollover", "CreditRollover", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public ConfigEntry<float> percentageRollover;
        private void Awake()
        {
            percentageRollover = Config.Bind("General", "percentageRollover", 1f, "The percentage of credits that rollover to the next round. Ideally within 0 to 1 as rolloverCredits = previousCreds * percentageRollover.");
            
            // Plugin startup logic
            Logger.LogInfo($"CreditRollover has loaded :)");
            Harmony.CreateAndPatchAll(typeof(PatchCreditsToRollOver));
            Logger.LogInfo($"CreditRollover is finished patching :D");
            ImNotReallySureHowToDoThis.valueOfRolloverPercentage = percentageRollover.Value;
            Logger.LogInfo($"Loaded config");


        }
    }
}

// My C# knowledge sucks so I'm really not sure how to pass the rolloverPercentage to the patch class
// https://csharpindepth.com/articles/singleton
public sealed class ImNotReallySureHowToDoThis
{
    private static readonly ImNotReallySureHowToDoThis instance = new ImNotReallySureHowToDoThis();

    public static ImNotReallySureHowToDoThis _instance
    {
        get
        {
            return instance;
        }
    }
    

    static ImNotReallySureHowToDoThis()
    {
        
    }
    private ImNotReallySureHowToDoThis()
    {
        
    }

    public static float valueOfRolloverPercentage;
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

        GameObject.FindObjectOfType<Terminal>().groupCredits = (int)Math.Floor((float)creds * ImNotReallySureHowToDoThis.valueOfRolloverPercentage);
    }


}