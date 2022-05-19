
namespace YondaimeCDS
{
    public static class DownloadedDataTracker
    {
        public static double RequestBundleSize(string bundleName)
        {
            double downloadedDataSize = IOUtils.GetOnDiskDataSize(bundleName);
            double assetSize = ManifestTracker.LocalAssetManifest.GetBundleSize(bundleName);

            return assetSize - downloadedDataSize;
        }

        public static double GetPendingDownloadSizeInMB(string bundleName)
        {
            return GetPendingDownloadSizeInKB(bundleName) / 1000;
        }

        public static double GetPendingDownloadSizeInKB(string bundleName)
        {
            return RequestBundleSize(bundleName) / 1000;
        }
    }
}