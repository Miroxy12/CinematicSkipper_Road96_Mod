using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BlueEyes.FlowCanvasUtils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

// WARNING: This mod can break NPCs animation for some reasons, do not use on your main run if you want a good playing experience

namespace CinematicSkipper_Road96_Mod
{
    [BepInEx.BepInPlugin(mod_guid, "Cinematic Skipper", version)]
    [BepInEx.BepInProcess("Road 96.exe")]
    public class CinematicSkipperMod : BasePlugin
    {
        private const string mod_guid = "miroxy12.cinematicskipper";
        private const string version = "1.0";
        private readonly Harmony harmony = new Harmony(mod_guid);
        internal static new ManualLogSource Log;
        public static GameObject cinemanager = null;
        public static bool hascinemanager = false;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo(mod_guid + " started, version: " + version);
            harmony.PatchAll(typeof(TriggerEventHook));
            harmony.PatchAll(typeof(LoadSceneHook));
            AddComponent<ModMain>();
        }
    }
    public class ModMain : MonoBehaviour
    {
        void Awake()
        {
            CinematicSkipperMod.Log.LogInfo("loading Cinematic Skipper");
        }
        void OnEnable()
        {
            CinematicSkipperMod.Log.LogInfo("enabled Cinematic Skipper");
        }
        void Update()
        {
            if (!CinematicSkipperMod.hascinemanager) {
                if (SceneManager.GetActiveScene().name.Equals("Core")) {
                    GameObject cinemanager = GameObject.Find("CineManager");
                    if (cinemanager != null) {
                        CinematicSkipperMod.cinemanager = cinemanager;
                        CinematicSkipperMod.hascinemanager = true;
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(FlowEventNode), "TriggerEvent", new System.Type[] { typeof(string) })]
    public class TriggerEventHook
    {
        static void Prefix(FlowEventNode __instance, string eventName)
        {
            if (eventName.Equals("FADE_IN")) {
                __instance.graph.Stop();
                CinematicSkipperMod.cinemanager.SetActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(SceneManager), "LoadSceneAsync", new System.Type[] { typeof(string), typeof(LoadSceneMode) })]
    public class LoadSceneHook
    {
        static void Postfix(string sceneName, LoadSceneMode mode)
        {
            CinematicSkipperMod.cinemanager?.SetActive(true);
        }
    }
}
