using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections.Generic;

public static class PostBuild
{
    // Copy over settings files on build.
    [PostProcessBuild]
    static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/..", "*.avr", SearchOption.AllDirectories);

        foreach(string f in files) {
            string dest_path = Path.GetDirectoryName(pathToBuiltProject) + "/" + Path.GetFileName(f);
            FileUtil.DeleteFileOrDirectory(dest_path);
            FileUtil.CopyFileOrDirectory(f, dest_path);
        }
    }
}

