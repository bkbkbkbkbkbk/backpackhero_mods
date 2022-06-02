using HarmonyLib;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SaveSlots
{
    public static class PatchSaveManager
    {
        public static readonly string SaveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveSlots");

        public static int SaveSlotNumber = 0;
        public static int MaxSaveSlotNumber = 10;

        public static string GetCurrentSlotFilename() => $"slot{SaveSlotNumber}.es3";
        public static string GetCurrentSlotFilePath() => Path.Combine(SaveDir, GetCurrentSlotFilename());
        public static void MakesureSaveDirExists()
        {
            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);
        }
    }


    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.Save))]
    public class PatchSaveManager_Save
    {
        public static void Postfix(ref SaveManager __instance, ref IEnumerator __result, ref ES3Settings ___settings)
        {
            var opath = ___settings.path;
            var npath = PatchSaveManager.GetCurrentSlotFilePath();

            __result = __result.ToIEnumerable().Append(() =>
            {
                ES3.StoreCachedFile();
                PatchSaveManager.MakesureSaveDirExists();
                ES3.CopyFile(opath, npath);
                return new WaitForEndOfFrame();
            }).Append(() => new WaitForSeconds(0.3f))
            .Append(() =>
            {
                var gm = UnityEngine.GameObject.FindObjectOfType<GameManager>();
                MelonLogger.Msg($"exported ES3File to {npath}");
                gm?.CreatePopUp($"{PatchSaveManager.GetCurrentSlotFilename()} saved");
                return new WaitForEndOfFrame();
            }).GetEnumerator();

        }
    }

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.Load))]
    public class PatchSaveManager_Load
    {
        public static bool Prefix(ref SaveManager __instance, ref ES3Settings ___settings)
        {
            var opath = ___settings.path;
            var npath = PatchSaveManager.GetCurrentSlotFilePath();
            if (!File.Exists(npath))
            {
                MelonLogger.Msg($"ES3File not found: {npath}");
                return true;
            }

            try
            {
                ES3.CopyFile(npath, opath);
                MelonLogger.Msg($"imported ES3File from {npath}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load ES3File {npath}: \r\n{ex.StackTrace}");
            }

            return true;
        }
    }



}
