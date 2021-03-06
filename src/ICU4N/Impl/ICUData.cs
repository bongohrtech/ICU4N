﻿using ICU4N.Logging;
using ICU4N.Support;
using ICU4N.Support.Globalization;
using ICU4N.Util;
using J2N;
using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace ICU4N.Impl
{
    /// <summary>
    /// Provides access to ICU data files as <see cref="Stream"/>s.  Implements security checking.
    /// </summary>
    public sealed class ICUData
    {
        /// <summary>
        /// The data path to be used with <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, OpenType)"/> API
        /// </summary>
        internal const string IcuDataPath = "Impl/";

        /// <summary>
        /// The ICU data package name.
        /// This is normally the name of the .dat package, and the prefix (plus '/')
        /// of the package entry names.
        /// </summary>
#pragma warning disable 612, 618
        internal const string PackageName = "icudt" + VersionInfo.IcuDataVersionPath;
#pragma warning restore 612, 618
        /// <summary>
        /// The data path to be used with <see cref="Assembly.GetManifestResourceStream(string)"/>.
        /// </summary>
        public const string IcuBundle = "Data/" + PackageName;

        /// <summary>
        /// The base name of ICU data to be used with <see cref="Assembly.GetManifestResourceStream(string)"/>,
        /// <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, bool)"/> etc.
        /// </summary>
        public const string IcuBaseName = IcuDataPath + IcuBundle;

        /// <summary>
        /// The base name of collation data to be used with <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, OpenType)"/> API
        /// </summary>
        public const string IcuCollationBaseName = IcuBaseName + "/coll";

        /// <summary>
        /// The base name of rbbi data to be used with <see cref="ICUBinary.GetData(Assembly, string, string, bool)"/> API
        /// </summary>
        public const string IcuBreakIteratorName = "brkitr";

        /// <summary>
        /// The base name of rbbi data to be used with <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, OpenType)"/> API
        /// </summary>
        public const string IcuBreakIteratorBaseName = IcuBaseName + "/" + IcuBreakIteratorName;

        /// <summary>
        /// The base name of rbnf data to be used with <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, OpenType)"/> API
        /// </summary>
        public const string IcuRuleBasedNumberFormatBaseName = IcuBaseName + "/rbnf";

        /// <summary>
        /// The base name of transliterator data to be used with <see cref="ICUResourceBundle.GetBundleInstance(string, string, Assembly, OpenType)"/> API
        /// </summary>
        public const string IcuTransliteratorBaseName = IcuBaseName + "/translit";

        public const string IcuLanguageBaseName = IcuBaseName + "/lang";
        public const string IcuCurrencyBaseName = IcuBaseName + "/curr";
        public const string IcuRegionBaseName = IcuBaseName + "/region";
        public const string IcuZoneBaseName = IcuBaseName + "/zone";
        public const string IcuUnitBaseName = IcuBaseName + "/unit";

        /// <summary>
        /// For testing (otherwise false): When reading a <see cref="Stream"/> from an <see cref="Assembly"/>
        /// (that is, not from a file), log when the stream contains ICU binary data.
        /// <para/>
        /// This cannot be <see cref="ICUConfig"/>'ured because <see cref="ICUConfig"/> calls <see cref="ICUData.GetStream(string)"/>
        /// to read the properties file, so we would get a circular dependency
        /// in the class initialization.
        /// </summary>
        private static readonly bool logBinaryDataFromInputStream = SystemProperties.GetPropertyAsBoolean("ICU4N.LogBinaryDataFromInputStream", false);
        private static readonly ILog logger = logBinaryDataFromInputStream ? LogProvider.For<ICUData>() : null;

        public static bool Exists(string resourceName)
        {
            return typeof(ICUData).FindAndGetManifestResourceStream(ResourceUtil.ConvertResourceName(resourceName)) != null;
        }

        private static Stream GetStream(Type root, string resourceName, bool required)
        {
            Stream i;
            i = root.FindAndGetManifestResourceStream(ResourceUtil.ConvertResourceName(resourceName));
            if (i == null && required)
            {
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
                Assembly assembly = root.GetTypeInfo().Assembly;
#else
                Assembly assembly = root.Assembly;
#endif
                throw new MissingManifestResourceException("could not locate data " + resourceName +
                    " Assembly: " + assembly.FullName + " Resource: " + resourceName);
            }
            CheckStreamForBinaryData(i, resourceName);
            return i;
        }

        /// <summary>
        /// Should be called only from <see cref="ICUBinary.GetData(Assembly, string, string, bool)"/> or from convenience overloads here.
        /// </summary>
        internal static Stream GetStream(Assembly loader, string resourceName, bool required)
        {
            Stream i = null;
            i = loader.FindAndGetManifestResourceStream(ResourceUtil.ConvertResourceName(resourceName));
            if (i == null && required)
            {
                throw new MissingManifestResourceException("could not locate data " + loader.ToString() + " Resource: " + resourceName);
            }
            CheckStreamForBinaryData(i, resourceName);
            return i;
        }

        private static void CheckStreamForBinaryData(Stream input, string resourceName)
        {
            // ICU4N specific - only check the data if the stream is seekable
            if (logBinaryDataFromInputStream && input != null && input.CanSeek && resourceName.IndexOf(PackageName, StringComparison.Ordinal) >= 0)
            {
                try
                {
                    byte[] b = new byte[32];
                    int len = input.Read(b, 0, b.Length);
                    if (len == 32 && b[2] == (byte)0xda && b[3] == 0x27)
                    {
                        string msg = string.Format(
                                "ICU binary data file loaded from Assembly as Stream " +
                                "from {0}: MappedData {1:x2}{2:x2}{3:x2}{4:x2}  dataFormat {5:x2}{6:x2}{7:x2}{8:x2}",
                                resourceName,
                                b[0], b[1], b[2], b[3],
                                b[12], b[13], b[14], b[15]);
                        logger.Info(msg);
                    }
                    input.Seek(b.Length, SeekOrigin.Current);
                }
                catch (IOException)
                {
                    // ignored
                }
            }
        }

        public static Stream GetStream(Assembly assembly, string resourceName)
        {
            return GetStream(assembly, resourceName, false);
        }

        public static Stream GetRequiredStream(Assembly assembly, string resourceName)
        {
            return GetStream(assembly, resourceName, true);
        }

        /// <summary>
        /// Convenience override that calls <c>GetStream(typeof(ICUData), resourceName, false)</c>.
        /// </summary>
        /// <returns>Returns null if the resource could not be found.</returns>
        public static Stream GetStream(string resourceName)
        {
            return GetStream(typeof(ICUData), resourceName, false);
        }

        /// <summary>
        /// Convenience method that calls <c>GetStream(typeof(ICUData), resourceName, true)</c>
        /// </summary>
        /// <exception cref="System.Resources.MissingManifestResourceException">If the resource could not be found.</exception>
        public static Stream GetRequiredStream(string resourceName)
        {
            return GetStream(typeof(ICUData), resourceName, true);
        }

        /// <summary>
        /// Convenience override that calls <c>GetStream(root, resourceName, false)</c>.
        /// </summary>
        /// <returns>Returns null if the resource could not be found.</returns>
        public static Stream GetStream(Type root, string resourceName)
        {
            return GetStream(root, resourceName, false);
        }

        /// <summary>
        /// Convenience method that calls <c>GetStream(root, resourceName, true)</c>.
        /// </summary>
        /// <exception cref="System.Resources.MissingManifestResourceException">If the resource could not be found.</exception>
        public static Stream GetRequiredStream(Type root, string resourceName)
        {
            return GetStream(root, resourceName, true);
        }
    }
}
