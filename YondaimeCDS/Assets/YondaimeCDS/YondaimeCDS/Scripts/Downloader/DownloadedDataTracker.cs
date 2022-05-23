
namespace YondaimeCDS
{
    internal static class DownloadedDataTracker
    {
        internal static double RequestBundleSize(string bundleName)
        {
            double downloadedDataSize = IOUtils.GetOnDiskDataSize(bundleName);
            double assetSize = ManifestTracker.LocalAssetManifest.GetBundleSize(bundleName);

            return assetSize - downloadedDataSize;
        }

        internal static double GetPendingDownloadSizeInMB(string bundleName)
        {
            return GetPendingDownloadSizeInKB(bundleName) / 1000;
        }

        internal static double GetPendingDownloadSizeInKB(string bundleName)
        {
            return RequestBundleSize(bundleName) / 1000;
        }
    }
}