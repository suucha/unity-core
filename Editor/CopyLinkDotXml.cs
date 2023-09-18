using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuuchaStudio.Unity.Core.Editor
{
    [InitializeOnLoad]
    public class CopyLinkDotXml
    {
        static CopyLinkDotXml()
        {
            var sourcePath = Path.GetFullPath(Path.Combine("Packages", "com.suucha.unity.core"));
            var destPath = Path.Combine(Application.dataPath, "Suucha", "Unity", "Core");
            var sourceFile = Path.Combine(sourcePath, "link.xml");
            var destFile = Path.Combine(destPath, "link.xml");
            if (File.Exists(sourceFile))
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                File.Copy(sourceFile, destFile, true);
            }
        }
    }
}
