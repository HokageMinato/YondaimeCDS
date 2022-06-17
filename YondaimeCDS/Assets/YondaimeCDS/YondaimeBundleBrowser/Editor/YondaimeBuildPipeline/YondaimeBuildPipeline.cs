using System.IO;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace YondaimeCDS
{
    public static class YondaimeBuildPipeline
    {
        public static CompatibilityAssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            AssetBundleBuild[] bundleBuilds = ContentBuildInterface.GenerateAssetBundleBuilds();
            return BuildAssetBundles_Internal(outputPath, new BundleBuildContent(bundleBuilds), assetBundleOptions, targetPlatform);
        }

        public static CompatibilityAssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            return BuildAssetBundles_Internal(outputPath, new BundleBuildContent(builds), assetBundleOptions, targetPlatform);
        }

        internal static CompatibilityAssetBundleManifest BuildAssetBundles_Internal(string outputPath, IBundleBuildContent content, BuildAssetBundleOptions options, BuildTarget targetPlatform)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);
            BundleBuildParameters bundleBuildParameters = new BundleBuildParameters(targetPlatform, buildTargetGroup, outputPath);
            bundleBuildParameters.WriteLinkXML = true;

            if ((options & BuildAssetBundleOptions.ForceRebuildAssetBundle) != 0)
            {
                bundleBuildParameters.UseCache = false;
            }

            if ((options & BuildAssetBundleOptions.AppendHashToAssetBundleName) != 0)
            {
                bundleBuildParameters.AppendHash = true;
            }

            if ((options & BuildAssetBundleOptions.ChunkBasedCompression) != 0)
            {
                bundleBuildParameters.BundleCompression = UnityEngine.BuildCompression.LZ4;
            }
            else if ((options & BuildAssetBundleOptions.UncompressedAssetBundle) != 0)
            {
                bundleBuildParameters.BundleCompression = UnityEngine.BuildCompression.Uncompressed;
            }
            else
            {
                bundleBuildParameters.BundleCompression = UnityEngine.BuildCompression.LZMA;
            }

            if ((options & BuildAssetBundleOptions.DisableWriteTypeTree) != 0)
            {
                bundleBuildParameters.ContentBuildFlags |= ContentBuildFlags.DisableWriteTypeTree;
            }

            if (ContentPipeline.BuildAssetBundles(bundleBuildParameters, content, out IBundleBuildResults result) < ReturnCode.Success)
            {
                return null;
            }

            CompatibilityAssetBundleManifest compatibilityAssetBundleManifest = ScriptableObject.CreateInstance<CompatibilityAssetBundleManifest>();
            compatibilityAssetBundleManifest.SetResults(result.BundleInfos);
            File.WriteAllText(bundleBuildParameters.GetOutputFilePathForIdentifier(Path.GetFileName(outputPath) + ".manifest"), compatibilityAssetBundleManifest.ToString());
            return compatibilityAssetBundleManifest;
        }
    }
}