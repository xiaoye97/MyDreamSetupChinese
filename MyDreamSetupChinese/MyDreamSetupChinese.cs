using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Localization;
using HarmonyLib;
using Saving.SaveLoadSystems;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace MyDreamSetupChinese
{
    [BepInPlugin("me.xiaoye97.MyDreamSetup.MyDreamSetupChinese", "MyDreamSetupChinese", "1.0.0")]
    public class MyDreamSetupChinese : BaseUnityPlugin
    {
        private static string locFilePath;
        private static string needLocFilePath;
        private static Dictionary<string, string> locDict = new Dictionary<string, string>();
        private static Dictionary<string, string> needLocDict = new Dictionary<string, string>();

        void Start()
        {
            locFilePath = $"{Paths.PluginPath}/Chinese.json";
            needLocFilePath = $"{Paths.PluginPath}/NeedTranslate.json";
            LoadLoc();
            Harmony.CreateAndPatchAll(typeof(MyDreamSetupChinese));
        }

        public void LoadLoc()
        {
            if (File.Exists(locFilePath))
            {
                string json = File.ReadAllText(locFilePath);
                locDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public static void SaveNeedLoc()
        {
            if (needLocDict.Count > 0)
            {
                string json = JsonConvert.SerializeObject(needLocDict, Formatting.Indented);
                File.WriteAllText(needLocFilePath, json);
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LanguageSaveLoad), "Load")]
        public static void OnLanguageLoad()
        {
            Translate(GameObject.FindObjectOfType<BlueprintLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<BlueprintLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<InformationalMessagesLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<MainMenuLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<RecommendationLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<RoomsLoadingLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<SettingsLocalization>()._strings);
            Translate(GameObject.FindObjectOfType<TwitterLocalization>()._strings);
            SaveNeedLoc();
        }

        public static void Translate<E>(Dictionary<E, Dictionary<Language, string>> dict) where E : Enum
        {
            E e = default;
            string typeName = e.GetType().Name;
            foreach (var kv in dict)
            {
                string key = $"{typeName}.{kv.Key}";
                if (locDict.ContainsKey(key))
                {
                    kv.Value[Language.English] = locDict[key];
                }
                else
                {
                    Debug.Log($"有需要翻译的内容 Key:{key} Value:{kv.Value[Language.English]}");
                    needLocDict[key] = kv.Value[Language.English];
                }
            }
        }
    }
}
