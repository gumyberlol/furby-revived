using UnityEngine;
using UnityEditor;

public class BuildBundles
{
    [MenuItem("Tools/Build Furby Bundles")]
    static void Build()
    {
        string outputPath = 
            Application.streamingAssetsPath + 
            "/Generated/StandaloneWindows/";
            
        // Make sure folder exists!!
        System.IO.Directory
            .CreateDirectory(outputPath);
            
        BuildPipeline.BuildAssetBundleExplicitAssetNames(
            new UnityEngine.Object[] {},
            new string[] {},
            outputPath + "furbybabycoloring.unity3d",
            BuildAssetBundleOptions.CollectDependencies,
            BuildTarget.StandaloneWindows);
            
        Debug.Log("Bundles built!!");
    }
}