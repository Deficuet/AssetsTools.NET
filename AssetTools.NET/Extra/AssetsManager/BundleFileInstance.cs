using System;
using System.Collections.Generic;
using System.IO;

namespace AssetsTools.NET.Extra
{
    public class BundleFileInstance
    {
        public string path;
        public string name;
        public AssetBundleFile file;
        /// <summary>
        /// Original compression type. If this bundle is decompressed, you might
        /// use this value to compress it back to its original compression type.
        /// </summary>
        public AssetBundleCompressionType originalCompression;
        /// <summary>
        /// List of loaded assets files for this bundle.
        /// </summary>
        /// <remarks>
        /// This list does not contain <i>every</i> assets file for the bundle,
        /// instead only the ones that have been loaded so far.
        /// </remarks>
        public List<AssetsFileInstance> loadedAssetsFiles;

        public Stream BundleStream => file.Reader.BaseStream;
        public Stream DataStream => file.DataReader.BaseStream;

        public BundleFileInstance(Stream stream, string filePath, bool unpackIfPacked = true)
        {
            path = Path.GetFullPath(filePath);
            name = Path.GetFileName(path);

            file = new AssetBundleFile();
            file.Read(new AssetsFileReader(stream));

            originalCompression = file.GetCompressionType();
            if (file.Header != null && file.DataIsCompressed && unpackIfPacked)
            {
                file = BundleHelper.UnpackBundle(file);
            }

            loadedAssetsFiles = new List<AssetsFileInstance>();
        }

        public BundleFileInstance(FileStream stream, bool unpackIfPacked = true)
            : this(stream, stream.Name, unpackIfPacked)
        {
        }

        /// <summary>
        /// Wraps an <see cref="AssetBundleFile"/> that was produced by an external
        /// loader (for example a bundle decryption pipeline) so it can be used
        /// exactly like a bundle read by <see cref="AssetBundleFile.Read"/>.
        /// The instance takes ownership of the readers inside <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The already loaded bundle.</param>
        /// <param name="filePath">Path used for manager lookups. It does not need to exist on disk.</param>
        /// <param name="originalCompression">
        /// Compression type to restore when the bundle is packed again.
        /// Use <see cref="AssetBundleFile.GetCompressionType"/> when in doubt.
        /// </param>
        public BundleFileInstance(AssetBundleFile file, string filePath, AssetBundleCompressionType originalCompression)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            path = Path.GetFullPath(filePath);
            name = Path.GetFileName(path);

            this.file = file;
            this.originalCompression = originalCompression;
            loadedAssetsFiles = new List<AssetsFileInstance>();
        }
    }
}
