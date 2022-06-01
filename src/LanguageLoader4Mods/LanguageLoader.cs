using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace LanguageLoader4Mods
{
    public class LanguageLoader : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("LanguageLoader4Mods started");
            base.OnApplicationStart();
        }

    }


    [HarmonyPatch(typeof(LangaugeManager), "LoadLanguageFile")]
    public class LanguageManagerPatch
    {
        public class State
        {
            public Dictionary<FileInfo, Dictionary<string, string>> ParsedFile { get; set; } = new Dictionary<FileInfo, Dictionary<string, string>>();
        }

        public static readonly string S_Path_ModsLanguageRootDir = Path.Combine(Application.streamingAssetsPath, "Language/Mods");

        /// may throw IOException if user doesn't have permission
        public static bool Prefix(ref LangaugeManager __instance
                            , ref Dictionary<string, string> ___languageTerms
                            , ref List<string> ___discoveredFonts
                            , ref int ___chosenLanguageNum
                            , out State __state
                            )
        {
            __state = new State();

            if (___chosenLanguageNum < 0 || ___chosenLanguageNum > ___discoveredFonts.Count)
            {
                MelonLogger.Warning($"failed to patch language #{___chosenLanguageNum} : index out of range");
                return true;
            }

            var language = ___discoveredFonts[___chosenLanguageNum];

            if (!Directory.Exists(S_Path_ModsLanguageRootDir))
            {
                MelonLogger.Msg($"Creating directory {S_Path_ModsLanguageRootDir}...");
                Directory.CreateDirectory(S_Path_ModsLanguageRootDir);
            }

            var targets = new DirectoryInfo(S_Path_ModsLanguageRootDir).GetDirectories()
                        .Select(dirInfo => dirInfo.GetFiles($"{language}.csv").FirstOrDefault())
                        .Where(a => a != null)
                        .ToList();

            MelonLogger.Msg($"Loading {targets.Count} custom mod translation files...");

            foreach (var target in targets)
            {
                var text = File.ReadAllText(target.FullName);

                var loadedDict = ProcessCsvText(text);
                __state.ParsedFile[target] = loadedDict;
                MelonLogger.Msg($"Loaded {loadedDict.Count} LT pairs from file {target.Directory.Name}/{target.Name}");
            }
            return true;
        }

        public static void Postfix(ref LangaugeManager __instance
                        , ref Dictionary<string, string> ___languageTerms
                        , State __state)
        {
            MelonLogger.Msg($"Processing {__state?.ParsedFile.Count} language files for mods");

            foreach (var f in __state.ParsedFile.Keys)
            {
                var dict = __state.ParsedFile[f];
                foreach (var k in dict.Keys)
                {
                    if (___languageTerms.ContainsKey(k))
                    {
                        MelonLogger.Msg($"LT skipped - `{k}` of {f.Name} already loaded");
                    }
                    else
                    {
                        ___languageTerms[k] = dict[k];
                    }
                }
            }
            ReplaceRestText();
        }

        private static void ReplaceRestText()
        {
            foreach (var i in UnityEngine.Object.FindObjectsOfType<ReplacementText>())
            {
                i.ReplaceText();
            }
        }

        /// ripped out from LangaugeManager.LoadLanguageFile()
        private static Dictionary<string, string> ProcessCsvText(string text)
        {
            var ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            bool flag = false;
            text = text.Replace("\"\"", "*");
            for (int j = 0; j < text.Length; j++)
            {
                if (text.Substring(j, 1) == "\"")
                {
                    flag = !flag;
                }
                else if (flag && text.Substring(j, 1) == ",")
                {
                    text = text.Remove(j, 1);
                    text = text.Insert(j, ";");
                }
            }
            text = text.Replace("*", "\"");
            string[] array = text.Split(new char[]
            {
            ',',
            '\n'
            });
            int num = 5;
            while (num + 5 < array.Length)
            {
                string text2 = array[num + 1].ToLower().Trim();
                string text3 = array[num + 3].ToLower().Trim();
                if (text2.Length > 0 && text3.Length > 0)
                {
                    text3 = text3.Replace(";", ",");
                    if (text3.Substring(0, 1) == "\"")
                    {
                        text3 = text3.Remove(0, 1);
                    }
                    if (text3.Substring(text3.Length - 1, 1) == "\"")
                    {
                        text3 = text3.Remove(text3.Length - 1, 1);
                    }
                    if (ret.ContainsKey(text2))
                    {
                        Debug.Log("Already contains key" + text2);
                    }
                    else
                    {
                        ret.Add(text2, text3);
                    }
                }
                num += 5;
            }
            return ret;
        }

    }
}
