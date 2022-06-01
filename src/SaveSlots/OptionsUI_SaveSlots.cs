using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MelonLoader;
using TMPro;
using UnityEngine;

namespace SaveSlots
{
    public static class OptionsHelper
    {
        public static GameObject FindOptionsEventBoxAnimation()
        {
            return GameObject
                            .FindGameObjectWithTag("UI Canvas")?
                            .transform
                            .OfType<Transform>()
                            .LastOrDefault(t => t.name.StartsWith("Options", StringComparison.InvariantCultureIgnoreCase))?
                            .OfType<Transform>()
                            .FirstOrDefault()?
                            .gameObject;
        }

        public static void PatchOptionsBox(Transform eventBoxAnimation)
        {
            var existingSaveSlotUI = eventBoxAnimation.transform.OfType<Transform>().FirstOrDefault(d => d.name.StartsWith("Save Slot", StringComparison.InvariantCultureIgnoreCase))?.gameObject;
            if (existingSaveSlotUI != null)
            {
                return;
            }

            var rotateDropdown = eventBoxAnimation.transform.OfType<Transform>().FirstOrDefault(d => d.name.StartsWith("Rotate", StringComparison.InvariantCultureIgnoreCase)).gameObject;

            var go = GameObject.Instantiate(rotateDropdown, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(eventBoxAnimation.transform);
            go.transform.SetAsLastSibling();
            go.name = "Save Slot";
            var txt = go.GetComponent<TextMeshProUGUI>();
            txt.text = "Save Slot:";
            var lt = GameObject.Instantiate(go.GetComponent<ReplacementText>(), Vector3.zero, Quaternion.identity);

            MelonLogger.Warning($"asd;f dsa  {go.GetComponent<ReplacementText>() == rotateDropdown.GetComponent<ReplacementText>()}");


            var languageTerms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string key = "";

            var rect = go.GetComponent<RectTransform>();
            var rect0 = rotateDropdown.GetComponent<RectTransform>();
            rect.offsetMax = rect0.offsetMax;
            rect.offsetMin = rect0.offsetMin;
            rect.anchoredPosition = new Vector2(660, -705);

            var dropdown = go.transform.OfType<Transform>().FirstOrDefault().gameObject;
            var tmp_dropdown = dropdown.GetComponent<TMP_Dropdown>();
            tmp_dropdown.options.RemoveAll(a => true);
            tmp_dropdown.AddOptions(Enumerable.Range(1, PatchSaveManager.MaxSaveSlotNumber).Select(i => $"Record {i}").ToList());

            tmp_dropdown.onValueChanged.RemoveAllListeners();// = new TMP_Dropdown.DropdownEvent();
            MelonLogger.Msg("save slots - patched");
        }

    }
    
    [HarmonyPatch(typeof(LangaugeManager), "LoadLanguageFile")]
    public class PatchUILanguageText
    {

        //static readonly KeyValuePair<string,string>

        public static void Postfix(ref Dictionary<string,string> ___languageTerms, ref int ___chosenLanguageNum, ref List<string> ___discoveredFonts)
        {
            if (___discoveredFonts.Count < ___chosenLanguageNum)
                ;
            //var languageName = ___discoveredFonts?[]

        }

    }



    [HarmonyPatch(typeof(MenuManager), "Start")]
    public class OptionsBox_Menu
    {
        public static void Postfix(ref MenuManager __instance, ref GameObject ___optionsPrefab)
        {
            var eventBoxAnimation = ___optionsPrefab.transform.OfType<Transform>().FirstOrDefault();
            OptionsHelper.PatchOptionsBox(eventBoxAnimation);
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    public class OptionsBox_inGmae
    {
        public static void Postfix(ref GameManager __instance, ref GameObject ___options)
        {
            var eventBoxAnimation = ___options.transform.OfType<Transform>().FirstOrDefault();
            OptionsHelper.PatchOptionsBox(eventBoxAnimation);
        }
    }

    //[HarmonyPatch(typeof(Options), nameof(Options.EndEvent))]
    public class OptionsBox_Done
    {
        public static bool Prefix(ref Options __instance)
        {
            var eventBoxAnimation = __instance.transform.OfType<Transform>().FirstOrDefault();
            var existingSaveSlotUI = eventBoxAnimation.transform.OfType<Transform>().FirstOrDefault(d => d.name.StartsWith("Save Slot", StringComparison.InvariantCultureIgnoreCase))?.gameObject;
            if (existingSaveSlotUI == null)
            {
                MelonLogger.Error("save slots - not yet patched");
                return true;
            }

            var dropdown = existingSaveSlotUI.transform.OfType<Transform>().FirstOrDefault().gameObject;
            var tmp_dropdown = dropdown.GetComponent<TMP_Dropdown>();

            if (PatchSaveManager.SaveSlotNumber != tmp_dropdown.value)
            {
                PatchSaveManager.SaveSlotNumber = tmp_dropdown.value;
                MelonLogger.Msg("save slots - changed slot to #" + PatchSaveManager.SaveSlotNumber);
            }

            return true;
        }
    }

    //[HarmonyPatch(typeof(Options), nameof(Options.QuitGame))]
    public class OptionsBox_Quit
    {
        public static bool Prefix(ref Options __instance)
        {
            var eventBoxAnimation = __instance.transform.OfType<Transform>().FirstOrDefault();
            var existingSaveSlotUI = eventBoxAnimation.transform.OfType<Transform>().FirstOrDefault(d => d.name.StartsWith("Save Slot", StringComparison.InvariantCultureIgnoreCase))?.gameObject;
            if (existingSaveSlotUI == null)
            {
                MelonLogger.Error("save slots - not yet patched");
                return true;
            }

            var dropdown = existingSaveSlotUI.transform.OfType<Transform>().FirstOrDefault().gameObject;
            var tmp_dropdown = dropdown.GetComponent<TMP_Dropdown>();

            if (PatchSaveManager.SaveSlotNumber != tmp_dropdown.value)
            {
                PatchSaveManager.SaveSlotNumber = tmp_dropdown.value;
                MelonLogger.Msg("save slots - changed slot to #" + PatchSaveManager.SaveSlotNumber);
            }

            return true;
        }
    }

    //[HarmonyPatch(typeof(Options), "Start")]
    public class OptionsBox_Start
    {
        public static bool Prefix(ref Options __instance)
        {
            var eventBoxAnimation = __instance.transform.OfType<Transform>().FirstOrDefault();
            var existingSaveSlotUI = eventBoxAnimation.transform.OfType<Transform>().FirstOrDefault(d => d.name.StartsWith("Save Slot", StringComparison.InvariantCultureIgnoreCase))?.gameObject;
            if (existingSaveSlotUI == null)
            {
                MelonLogger.Msg("save slots - not yet patched");
                return true;
            }

            var dropdown = existingSaveSlotUI.transform.OfType<Transform>().FirstOrDefault().gameObject;
            var tmp_dropdown = dropdown.GetComponent<TMP_Dropdown>();

            if (PatchSaveManager.SaveSlotNumber != tmp_dropdown.value)
            {
                tmp_dropdown.value = PatchSaveManager.SaveSlotNumber;
                MelonLogger.Msg("save slots - loaded slot #" + PatchSaveManager.SaveSlotNumber);
            }

            return true;
        }
    }


    //[HarmonyPatch(typeof(OptionsSaveManager), nameof(OptionsSaveManager.SaveOptions))]
    public class OptionsSaveManager_Save
    {
        public static bool Prefix(ref ES3Settings ___settings)
        {
            ES3.Save("Save Slot", PatchSaveManager.SaveSlotNumber, ___settings);
            return true;
        }

    }

    //[HarmonyPatch(typeof(OptionsSaveManager), nameof(OptionsSaveManager.LoadOptions))]
    public class OptionsSaveManager_Load
    {
        public static void Postfix(ref ES3Settings ___settings)
        {
            PatchSaveManager.SaveSlotNumber = ES3.Load("Save Slot", 0, ___settings);
        }
    }
}
