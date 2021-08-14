using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor;

namespace AVR.UEditor.Core {
    public class PackageBuilder
    {
        static List<string> PackageQueue = new List<string>();

        static PackRequest Request;

        [MenuItem("AVR/Build Tarballs", false, -10)]
        public static void BuildPackager()
        {
    #if AVR_CORE
            PackageQueue.Add("com.avr.core");
    #endif
    #if AVR_AVATAR
            PackageQueue.Add("com.avr.avatar");
    #endif
    #if AVR_MOTION
            PackageQueue.Add("com.avr.motion");
    #endif
    #if AVR_PHYS
            PackageQueue.Add("com.avr.phys");
    #endif
    #if AVR_UI
            PackageQueue.Add("com.avr.ui");
    #endif
    #if AVR_NET
            PackageQueue.Add("com.avr.net");
    #endif

            Debug.Log("Started building "+PackageQueue.Count+" packages.");

            EditorApplication.update += UpdateBuildProcess;
        }

        static void UpdateBuildProcess() {
            if(PackageQueue.Count<1) {
                EditorApplication.update -= UpdateBuildProcess;
            }
            else if(Request==null || Request.IsCompleted) {
                BuildTarball(PackageQueue[0]);
                PackageQueue.RemoveAt(0);
            }
        }

        static void BuildTarball(string source) {
            source = "Packages/"+source;
            string destination = Application.dataPath+"/../../tarballs";

            Debug.Log("Started building " + source + "...");
            Request = Client.Pack(source, destination);
            EditorApplication.update += Progress;
        }

        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Tarball created: " + Request.Result.tarballPath);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}
