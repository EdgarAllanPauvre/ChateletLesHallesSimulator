using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder : MonoBehaviour
{
    //private const string DEFINE_DEBUG = "DEBUG";
    private const string DEFINE_SYMBOL_NAME_FINAL_GAME = "FINAL_GAME_VERSION";

    //[MenuItem("File/Build Demo version")]
    //static void PerformWin64Build()
    //{
    //    PerformWin64Build(BuildOptions.None, DEFINE_SYMBOL_NAME, "DemoVersion");
    //}
    //[MenuItem("File/Build Demo version DEV")]
    //static void PerformWin64BuildDev()
    //{
    //    PerformWin64Build(BuildOptions.Development, DEFINE_SYMBOL_NAME, "DemoVersionDEV");
    //}

    [MenuItem("File/Build game")]
    static void PerformWin64BuildFinalGame()
    {
        PerformWin64Build(BuildOptions.None, DEFINE_SYMBOL_NAME_FINAL_GAME, "ChateletLesHallesSimulator");
    }
    [MenuItem("File/Build game DEBUG")]
    static void PerformWin64BuildFinalGameDev()
    {
        PerformWin64Build(BuildOptions.Development, DEFINE_SYMBOL_NAME_FINAL_GAME, "ChateletLesHallesSimulatorDEBUG");
    }

    static void PerformWin64Build(BuildOptions options, string symbolName, string folder)
    {
        try
        {
            var bpOptions = new BuildPlayerOptions()
            {
                locationPathName = "Builds/" + folder + "/" + Application.productName + ".exe",
                targetGroup = BuildTargetGroup.Standalone,
                target = BuildTarget.StandaloneWindows64,
                options = options,
                extraScriptingDefines = new[] { symbolName },
                scenes = EditorBuildSettings.scenes.ToList().Select(x => x.path).ToArray()
            };
            BuildReport report = BuildPipeline.BuildPlayer(bpOptions);
            Debug.Log("Build ok");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
