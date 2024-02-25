using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
  
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;
using UnityEditor.Callbacks;

public class PostBuildTasks
{
    [PostProcessBuild]
    public static void PostBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            // Get root
            PlistElementDict rootDict = plist.root;
            // Tenjin IDFA
            rootDict.SetString("NSUserTrackingUsageDescription", "This request has popped up due to a 3rd party framework used by the app to deliver personalized ads to you");
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            #if tenjin_mopub_enabled
            AddSKAdNetworks(rootDict);
            #endif

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());

            var xcodeProjectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj");
            var pbxPath = Path.Combine(xcodeProjectPath, "project.pbxproj");
            var xcodeProjectLines = File.ReadAllLines(pbxPath);
            var sb = new StringBuilder();
            foreach (var line in xcodeProjectLines)
            {
                if (line.Contains("GCC_ENABLE_OBJC_EXCEPTIONS") ||
                    line.Contains("CLANG_ENABLE_MODULES"))
                {
                    var newLine = line.Replace("NO", "YES");
                    sb.AppendLine(newLine);
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            File.WriteAllText(pbxPath, sb.ToString());
        }
    }

    private static void AddSKAdNetworks(PlistElementDict rootDict)
    {
        const string AddSKAdNetworksDirectory = "Assets/Mamboo/Internal/iOS/SKAdNetworks/";
        const string AddSKAdNetworksPath = AddSKAdNetworksDirectory + "GlobalNetworks.xml";

        XDocument doc;

        if (!File.Exists(AddSKAdNetworksPath))
        {
            Debug.Log("SKAdNetworks not found " + AddSKAdNetworksPath + " file");
            return;
        }

        Debug.LogFormat("SKAdNetworks loading existing file {0}", AddSKAdNetworksPath);
        doc = XDocument.Load(AddSKAdNetworksPath);

        rootDict.CreateArray("SKAdNetworkItems");

        var arraySKAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");

        var paths = doc.Descendants("string").ToList();
        foreach (var item in paths)
        {
            var dictSKAdNetworkIdentifier = arraySKAdNetworkItems.AddDict();
            dictSKAdNetworkIdentifier.SetString("SKAdNetworkIdentifier", item.Value);
        }
    }

}
#endif