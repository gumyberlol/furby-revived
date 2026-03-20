using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
    public static class AssetBundleHelpers
    {
        public class AssetBundleLoad
        {
            public bool isLoaded;
            public UnityEngine.Object m_object;
        }

        // kept for IsLoading() compatibility!!
        private static Dictionary<string, object> m_concurrentlyLoadedAssetBundles = new Dictionary<string, object>();

        public static string GetStreamingAssetsPath()
        {
            // kept for compatibility but unused now!!
            return Application.streamingAssetsPath + "/Generated/StandaloneWindows/";
        }

        public static bool IsLoading()
        {
            return m_concurrentlyLoadedAssetBundles.Count != 0;
        }

        public static string LowerCaseAfterFinalSlash(string inputString)
        {
            int num = inputString.LastIndexOf("/");
            if (num >= 0)
            {
                return inputString.Substring(0, num) + inputString.Substring(num).ToLower();
            }
            return inputString.ToLower();
        }

        public static IEnumerator Load(string path, bool compressed, AssetBundleLoad prefabResult, GameObject activeObject, Type expectedType, bool forceLowerCase)
        {
            if (path.Contains(" "))
            {
                Logging.LogError("Path has space - may cause issues: " + path);
            }

            // build Resources path!!
            // path comes in like "FurbyBabyColoring/1_cube_coloring"!!
            // we load from "bundles/FurbyBabyColoring/1_cube_coloring"!!
            string resourcePath = "bundles/" + path;

            if (forceLowerCase)
            {
                resourcePath = LowerCaseAfterFinalSlash(resourcePath);
            }

            Logging.Log("Loading asset via Resources.Load from: " + resourcePath);

            // mark as loading!!
            m_concurrentlyLoadedAssetBundles[path] = new object();

            UnityEngine.Object obj = Resources.Load(resourcePath, expectedType);

            if (obj == null)
            {
                Logging.LogError("Failed to load asset: " + resourcePath);
            }

            prefabResult.m_object = obj;
            prefabResult.isLoaded = true;

            // mark as done!!
            m_concurrentlyLoadedAssetBundles.Remove(path);

            yield break;
        }
    }
}