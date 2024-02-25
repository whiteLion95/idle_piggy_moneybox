// #if UNITY_IOS
// using System.IO;
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;
// using UnityEngine;
// #endif
//
// namespace IDFATracking
// {
//   
// public static class IDFATrackingBuildPostprocess
// {
// #if UNITY_IOS
//   [PostProcessBuild(998)]
//   public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
//   {
//     if(buildTarget != BuildTarget.iOS)
//       return;
//
//     Debug.Log("iOS IDFA Tracking - IDFAPlatformIOSImpl - postprocess");
//     
//     PBXProject project = new PBXProject();
//     path = PBXProject.GetPBXProjectPath(path);
//     project.ReadFromString(File.ReadAllText(path));
//
// #if UNITY_2019_3_OR_NEWER
//     string projectTarget = project.GetUnityFrameworkTargetGuid();
// #else
//     string targetName = PBXProject.GetUnityTargetName();
//     string projectTarget = project.TargetGuidByName(targetName);
// #endif  
//
//     // Required Frameworks
//     project.AddFrameworkToProject(projectTarget, "AppTrackingTransparency.framework", false);
//
//     File.WriteAllText(path, project.WriteToString());
//   }
// #endif
// }
//
// }
