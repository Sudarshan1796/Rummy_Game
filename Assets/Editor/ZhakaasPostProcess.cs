#if UNITY_IOS || UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System;


public class ZhakaasPostProcess
{
    [PostProcessBuild(1000)]
    public static void PostProcessZhakaas(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string plistPath = path + "/Info.plist";
            var plist = new UnityEditor.iOS.Xcode.PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            var rootDict = plist.root;

            var buildKey2 = "ITSAppUsesNonExemptEncryption";
            rootDict.SetString(buildKey2, "false");

            string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
            if (rootDict.values.ContainsKey(exitsOnSuspendKey))
            {
                rootDict.values.Remove(exitsOnSuspendKey);
            }

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());

            //////////////////////////////
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));

            string target = proj.TargetGuidByName("Unity-iPhone");

            proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");
            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            proj.AddFrameworkToProject(target, "AdSupport.framework", false);
            proj.AddFrameworkToProject(target, "iAd.framework", false);
            proj.AddFrameworkToProject(target, "CoreData.framework", false);
            proj.AddFrameworkToProject(target, "StoreKit.framework", false);

            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}
#endif
