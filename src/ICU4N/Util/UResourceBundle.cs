﻿using ICU4N.Impl;
using ICU4N.Support.Collections;
using ICU4N.Support.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Collections;

namespace ICU4N.Util
{
    public abstract class UResourceBundle : ResourceBundle, IEnumerable<UResourceBundle> //: ResourceManager
    {
        /**
         * {@icu} Creates a resource bundle using the specified base name and locale.
         * ICU_DATA_CLASS is used as the default root.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param localeName the locale for which a resource bundle is desired
         * @throws MissingResourceException If no resource bundle for the specified base name
         * can be found
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         */
        public static UResourceBundle GetBundleInstance(string baseName, string localeName)
        {
            return GetBundleInstance(baseName, localeName, GetAssembly(baseName),
                                     false);
        }

        // ICU4N TODO: Our main assembly won't be able to load any LanguageData, RegionData, etc.
        // Need to come up with a better way to retrieve these values
        private static Assembly GetAssembly(string baseName)
        {
            if (baseName.EndsWith("/lang", StringComparison.Ordinal) || baseName.Contains("/lang/"))
                return LocaleDisplayNamesImpl.LangDataTables.impl.GetType().GetTypeInfo().Assembly;
            if (baseName.EndsWith("/region", StringComparison.Ordinal) || baseName.Contains("/region/"))
                return LocaleDisplayNamesImpl.RegionDataTables.impl.GetType().GetTypeInfo().Assembly;

            return ICUResourceBundle.ICU_DATA_CLASS_LOADER;
        }


        /**
         * {@icu} Creates a resource bundle using the specified base name, locale, and class root.
         *
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param localeName the locale for which a resource bundle is desired
         * @param root the class object from which to load the resource bundle
         * @throws MissingResourceException If no resource bundle for the specified base name
         * can be found
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         */
        public static UResourceBundle GetBundleInstance(string baseName, string localeName,
                                                        Assembly root)
        {
            return GetBundleInstance(baseName, localeName, root, false);
        }

        /**
         * {@icu} Creates a resource bundle using the specified base name, locale, and class
         * root.
         *
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param localeName the locale for which a resource bundle is desired
         * @param root the class object from which to load the resource bundle
         * @param disableFallback Option to disable locale inheritence.
         *                          If true the fallback chain will not be built.
         * @throws MissingResourceException
         *     if no resource bundle for the specified base name can be found
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         *
         */
        protected static UResourceBundle GetBundleInstance(string baseName, string localeName,
                                                           Assembly root, bool disableFallback)
        {
            return InstantiateBundle(baseName, localeName, root, disableFallback);
        }

        /**
         * {@icu} Sole constructor.  (For invocation by subclass constructors, typically
         * implicit.)  This is public for compatibility with Java, whose compiler
         * will generate public default constructors for an abstract class.
         * @stable ICU 3.0
         */
        public UResourceBundle()
        {
        }

        /**
         * {@icu} Creates a UResourceBundle for the locale specified, from which users can extract
         * resources by using their corresponding keys.
         * @param locale  specifies the locale for which we want to open the resource.
         *                If null the bundle for default locale is opened.
         * @return a resource bundle for the given locale
         * @stable ICU 3.0
         */
        public static UResourceBundle GetBundleInstance(ULocale locale)
        {
            if (locale == null)
            {
                locale = ULocale.GetDefault();
            }
            return GetBundleInstance(ICUData.ICU_BASE_NAME, locale.GetBaseName(),
                                     ICUResourceBundle.ICU_DATA_CLASS_LOADER, false);
        }

        /**
         * {@icu} Creates a UResourceBundle for the default locale and specified base name,
         * from which users can extract resources by using their corresponding keys.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @return a resource bundle for the given base name and default locale
         * @stable ICU 3.0
         */
        public static UResourceBundle GetBundleInstance(string baseName)
        {
            if (baseName == null)
            {
                baseName = ICUData.ICU_BASE_NAME;
            }
            ULocale uloc = ULocale.GetDefault();
            //return GetBundleInstance(baseName, uloc.GetBaseName(), ICUResourceBundle.ICU_DATA_CLASS_LOADER,
            //                         false);
            return GetBundleInstance(baseName, uloc.GetBaseName(), GetAssembly(baseName),
                                     false);
        }

        /**
         * {@icu} Creates a UResourceBundle for the specified locale and specified base name,
         * from which users can extract resources by using their corresponding keys.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param locale  specifies the locale for which we want to open the resource.
         *                If null the bundle for default locale is opened.
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         */

        public static UResourceBundle GetBundleInstance(string baseName, CultureInfo locale)
        {
            if (baseName == null)
            {
                baseName = ICUData.ICU_BASE_NAME;
            }
            ULocale uloc = locale == null ? ULocale.GetDefault() : ULocale.ForLocale(locale);

            //return GetBundleInstance(baseName, uloc.GetBaseName(),
            //                         ICUResourceBundle.ICU_DATA_CLASS_LOADER, false);
            return GetBundleInstance(baseName, uloc.GetBaseName(),
                GetAssembly(baseName), false);
        }

        /**
         * {@icu} Creates a UResourceBundle, from which users can extract resources by using
         * their corresponding keys.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param locale  specifies the locale for which we want to open the resource.
         *                If null the bundle for default locale is opened.
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         */
        public static UResourceBundle GetBundleInstance(string baseName, ULocale locale)
        {
            if (baseName == null)
            {
                baseName = ICUData.ICU_BASE_NAME;
            }
            if (locale == null)
            {
                locale = ULocale.GetDefault();
            }
            //return GetBundleInstance(baseName, locale.GetBaseName(),
            //                         ICUResourceBundle.ICU_DATA_CLASS_LOADER, false);
            return GetBundleInstance(baseName, locale.GetBaseName(),
                GetAssembly(baseName), false);
        }

        /**
         * {@icu} Creates a UResourceBundle for the specified locale and specified base name,
         * from which users can extract resources by using their corresponding keys.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param locale  specifies the locale for which we want to open the resource.
         *                If null the bundle for default locale is opened.
         * @param loader  the loader to use
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.8
         */
        public static UResourceBundle GetBundleInstance(string baseName, CultureInfo locale,
                                                        Assembly loader)
        {
            if (baseName == null)
            {
                baseName = ICUData.ICU_BASE_NAME;
            }
            ULocale uloc = locale == null ? ULocale.GetDefault() : ULocale.ForLocale(locale);
            return GetBundleInstance(baseName, uloc.GetBaseName(), loader, false);
        }

        /**
         * {@icu} Creates a UResourceBundle, from which users can extract resources by using
         * their corresponding keys.<br><br>
         * Note: Please use this API for loading non-ICU resources. Java security does not
         * allow loading of resources across jar files. You must provide your class loader
         * to load the resources
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param locale  specifies the locale for which we want to open the resource.
         *                If null the bundle for default locale is opened.
         * @param loader  the loader to use
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.8
         */
        public static UResourceBundle GetBundleInstance(string baseName, ULocale locale,
                                                        Assembly loader)
        {
            if (baseName == null)
            {
                baseName = ICUData.ICU_BASE_NAME;
            }
            if (locale == null)
            {
                locale = ULocale.GetDefault();
            }
            return GetBundleInstance(baseName, locale.GetBaseName(), loader, false);
        }

        /**
         * {@icu} Returns the RFC 3066 conformant locale id of this resource bundle.
         * This method can be used after a call to getBundleInstance() to
         * determine whether the resource bundle returned really
         * corresponds to the requested locale or is a fallback.
         *
         * @return the locale of this resource bundle
         * @stable ICU 3.0
         */
        public abstract ULocale GetULocale();

        /**
         * {@icu} Returns the localeID
         * @return The string representation of the localeID
         * @stable ICU 3.0
         */
        protected abstract string GetLocaleID();

        /**
         * {@icu} Returns the base name of the resource bundle
         * @return The string representation of the base name
         * @stable ICU 3.0
         */
        protected internal abstract string GetBaseName();


        /**
         * {@icu} Returns the parent bundle
         * @return The parent bundle
         * @stable ICU 3.0
         */
        //public override abstract UResourceBundle Parent { get; }
        new public virtual UResourceBundle Parent
        {
            get { return (UResourceBundle)base.parent; }
        }


        /**
         * Returns the locale of this bundle
         * @return the locale of this resource bundle
         * @stable ICU 3.0
         */
        public override CultureInfo GetLocale()
        {
            return GetULocale().ToLocale();
        }

        private enum RootType { MISSING, ICU, JAVA } // ICU4N TODO: API - rename from JAVA

        private static IDictionary<string, RootType> ROOT_CACHE = new ConcurrentDictionary<string, RootType>();

        private static RootType GetRootType(string baseName, Assembly root)
        {
            RootType rootType;

            if (!ROOT_CACHE.TryGetValue(baseName, out rootType))
            {
                string rootLocale = (baseName.IndexOf('.') == -1) ? "root" : "";
                try
                {
                    ICUResourceBundle.GetBundleInstance(baseName, rootLocale, root, true);
                    rootType = RootType.ICU;
                }
                catch (MissingManifestResourceException ex)
                {
                    try
                    {
                        ResourceBundleWrapper.GetBundleInstance(baseName, rootLocale, root, true);
                        rootType = RootType.JAVA;
                    }
                    catch (MissingManifestResourceException e)
                    {
                        //throw away the exception
                        rootType = RootType.MISSING;
                    }
                }

                ROOT_CACHE[baseName] = rootType;
            }

            return rootType;
        }

        private static void SetRootType(string baseName, RootType rootType)
        {
            ROOT_CACHE[baseName] = rootType;
        }

        /**
         * {@icu} Loads a new resource bundle for the given base name, locale and class loader.
         * Optionally will disable loading of fallback bundles.
         * @param baseName string containing the name of the data package.
         *                    If null the default ICU package name is used.
         * @param localeName the locale for which a resource bundle is desired
         * @param root the class object from which to load the resource bundle
         * @param disableFallback disables loading of fallback lookup chain
         * @throws MissingResourceException If no resource bundle for the specified base name
         * can be found
         * @return a resource bundle for the given base name and locale
         * @stable ICU 3.0
         */
        protected static UResourceBundle InstantiateBundle(string baseName, string localeName,
                                                           Assembly root, bool disableFallback)
        {
            RootType rootType = GetRootType(baseName, root);

            switch (rootType)
            {
                case RootType.ICU:
                    return ICUResourceBundle.GetBundleInstance(baseName, localeName, root, disableFallback);

                case RootType.JAVA:
                    return ResourceBundleWrapper.GetBundleInstance(baseName, localeName, root,
                                                                   disableFallback);

                case RootType.MISSING:
                default:
                    UResourceBundle b;
                    try
                    {
                        b = ICUResourceBundle.GetBundleInstance(baseName, localeName, root,
                                                                disableFallback);
                        SetRootType(baseName, RootType.ICU);
                    }
                    catch (MissingManifestResourceException ex)
                    {
                        b = ResourceBundleWrapper.GetBundleInstance(baseName, localeName, root,
                                                                    disableFallback);
                        SetRootType(baseName, RootType.JAVA);
                    }
                    return b;
            }
        }

        /**
         * {@icu} Returns a binary data item from a binary resource, as a read-only ByteBuffer.
         *
         * @return a pointer to a chunk of unsigned bytes which live in a memory mapped/DLL
         * file.
         * @see #getIntVector
         * @see #getInt
         * @throws MissingResourceException If no resource bundle can be found.
         * @throws UResourceTypeMismatchException If the resource has a type mismatch.
         * @stable ICU 3.8
         */
        public virtual ByteBuffer GetBinary()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * Returns a string from a string resource type
         *
         * @return a string
         * @see #getBinary()
         * @see #getIntVector
         * @see #getInt
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle has a type mismatch.
         * @stable ICU 3.8
         */
        public virtual string GetString()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * Returns a string array from a array resource type
         *
         * @return a string
         * @see #getString()
         * @see #getIntVector
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle has a type mismatch.
         * @stable ICU 3.8
         */
        public virtual string[] GetStringArray()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns a binary data from a binary resource, as a byte array with a copy
         * of the bytes from the resource bundle.
         *
         * @param ba  The byte array to write the bytes to. A null variable is OK.
         * @return an array of bytes containing the binary data from the resource.
         * @see #getIntVector
         * @see #getInt
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle has a type mismatch.
         * @stable ICU 3.8
         */
        public virtual byte[] GetBinary(byte[] ba)
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns a 32 bit integer array from a resource.
         *
         * @return a pointer to a chunk of unsigned bytes which live in a memory mapped/DLL file.
         * @see #getBinary()
         * @see #getInt
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle has a type mismatch.
         * @stable ICU 3.8
         */
        public virtual int[] GetInt32Vector()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns a signed integer from a resource.
         *
         * @return an integer value
         * @see #getIntVector
         * @see #getBinary()
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle type mismatch.
         * @stable ICU 3.8
         */
        public virtual int GetInt32()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns a unsigned integer from a resource.
         * This integer is originally 28 bit and the sign gets propagated.
         *
         * @return an integer value
         * @see #getIntVector
         * @see #getBinary()
         * @throws MissingResourceException If resource bundle is missing.
         * @throws UResourceTypeMismatchException If resource bundle type mismatch.
         * @stable ICU 3.8
         */
        public virtual int GetUInt32()
        {
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns a resource in a given resource that has a given key.
         *
         * @param aKey               a key associated with the wanted resource
         * @return                  a resource bundle object representing the resource
         * @throws MissingResourceException If resource bundle is missing.
         * @stable ICU 3.8
         */
        public virtual UResourceBundle Get(string aKey) // ICU4N TODO: API make into indexer property
        {
#pragma warning disable 612, 618
            UResourceBundle obj = FindTopLevel(aKey);
#pragma warning restore 612, 618
            if (obj == null)
            {
                string fullName = ICUResourceBundleReader.GetFullName(GetBaseName(), GetLocaleID());
                throw new MissingManifestResourceException(
                        "Can't find resource for bundle " + fullName + ", key "
                        + aKey + "Type: " + this.GetType().FullName + " Key: " + aKey);
            }
            return obj;
        }

        /**
         * Returns a resource in a given resource that has a given key, or null if the
         * resource is not found.
         *
         * @param aKey the key associated with the wanted resource
         * @return the resource, or null
         * @see #get(String)
         * @internal
         * @deprecated This API is ICU internal only.
         */
        [Obsolete("This API is ICU internal only.")]
        protected virtual UResourceBundle FindTopLevel(string aKey)
        {
            // NOTE: this only works for top-level resources.  For resources at lower
            // levels, it fails when you fall back to the parent, since you're now
            // looking at root resources, not at the corresponding nested resource.
            for (UResourceBundle res = this; res != null; res = res.Parent)
            {
                UResourceBundle obj = res.HandleGet(aKey, null, this);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        /**
         * Returns the string in a given resource at the specified index.
         *
         * @param index an index to the wanted string.
         * @return a string which lives in the resource.
         * @throws IndexOutOfBoundsException If the index value is out of bounds of accepted values.
         * @throws UResourceTypeMismatchException If resource bundle type mismatch.
         * @stable ICU 3.8
         */
        public virtual string GetString(int index)
        {
            ICUResourceBundle temp = (ICUResourceBundle)Get(index);
            if (temp.Type == STRING)
            {
                return temp.GetString();
            }
            throw new UResourceTypeMismatchException("");
        }

        /**
         * {@icu} Returns the resource in a given resource at the specified index.
         *
         * @param index an index to the wanted resource.
         * @return the sub resource UResourceBundle object
         * @throws IndexOutOfBoundsException If the index value is out of bounds of accepted values.
         * @throws MissingResourceException If the resource bundle is missing.
         * @stable ICU 3.8
         */
        public virtual UResourceBundle Get(int index) // ICU4N TODO: API make into indexer property
        {
            UResourceBundle obj = HandleGet(index, null, this);
            if (obj == null)
            {
                obj = Parent;
                if (obj != null)
                {
                    obj = obj.Get(index);
                }
                if (obj == null)
                    throw new MissingManifestResourceException(
                            "Can't find resource for bundle "
                                    + this.GetType().FullName + ", key "
                                    + Key);
            }
            return obj;
        }

        /**
         * Returns a resource in a given resource that has a given index, or null if the
         * resource is not found.
         *
         * @param index the index of the resource
         * @return the resource, or null
         * @see #get(int)
         * @internal
         * @deprecated This API is ICU internal only.
         */
        [Obsolete("This API is ICU internal only.")]
        protected virtual UResourceBundle FindTopLevel(int index)
        {
            // NOTE: this _barely_ works for top-level resources.  For resources at lower
            // levels, it fails when you fall back to the parent, since you're now
            // looking at root resources, not at the corresponding nested resource.
            // Not only that, but unless the indices correspond 1-to-1, the index will
            // lose meaning.  Essentially this only works if the child resource arrays
            // are prefixes of their parent arrays.
            for (UResourceBundle res = this; res != null; res = res.Parent)
            {
                UResourceBundle obj = res.HandleGet(index, null, this);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        /**
         * Returns the keys in this bundle as an enumeration
         * @return an enumeration containing key strings,
         *         which is empty if this is not a bundle or a table resource
         * @stable ICU 3.8
         */
        public override IEnumerable<string> GetKeys() // ICU4N TODO: API - change to Keys property
        {
            return KeySet();
        }

        /**
         * Returns a Set of all keys contained in this ResourceBundle and its parent bundles.
         * @return a Set of all keys contained in this ResourceBundle and its parent bundles,
         *         which is empty if this is not a bundle or a table resource
         * @internal
         * @deprecated This API is ICU internal only.
         */
        [Obsolete("This API is ICU internal only.")]
#pragma warning disable 809
        public override ISet<string> KeySet() // ICU4N TODO: API - change to KeySet property
#pragma warning disable 809
        {
            // TODO: Java 6 ResourceBundle has keySet() which calls handleKeySet()
            // and caches the results.
            // When we upgrade to Java 6, we still need to check for isTopLevelResource().
            // Keep the else branch as is. The if body should just return super.keySet().
            // Remove then-redundant caching of the keys.
            ISet<string> keys = null;
            ICUResourceBundle icurb = null;
            if (IsTopLevelResource && this is ICUResourceBundle)
            {
                // We do not cache the top-level keys in this base class so that
                // not every string/int/binary... resource has to have a keys cache field.
                icurb = (ICUResourceBundle)this;
                keys = icurb.TopLevelKeySet;
            }
            if (keys == null)
            {
                if (IsTopLevelResource)
                {
                    SortedSet<string> newKeySet;
                    if (parent == null)
                    {
                        newKeySet = new SortedSet<string>();
                    }
                    else if (parent is UResourceBundle)
                    {
                        newKeySet = new SortedSet<string>(((UResourceBundle)parent).KeySet());
                    }
                    else
                    {
                        // TODO: Java 6 ResourceBundle has keySet(); use it when we upgrade to Java 6
                        // and remove this else branch.
                        newKeySet = new SortedSet<string>();
                        using (var parentKeys = Parent.GetKeys().GetEnumerator())
                        {
                            while (parentKeys.MoveNext())
                            {
                                newKeySet.Add(parentKeys.Current);
                            }
                        }
                    }
                    newKeySet.UnionWith(HandleKeySet());
                    keys = (newKeySet).ToUnmodifiableSet();
                    if (icurb != null)
                    {
                        icurb.TopLevelKeySet = keys;
                    }
                }
                else
                {
                    return HandleKeySet();
                }
            }
            return keys;
        }

        /**
         * Returns a Set of the keys contained <i>only</i> in this ResourceBundle.
         * This does not include further keys from parent bundles.
         * @return a Set of the keys contained only in this ResourceBundle,
         *         which is empty if this is not a bundle or a table resource
         * @internal
         * @deprecated This API is ICU internal only.
         */
        [Obsolete("This API is ICU internal only.")]
#pragma warning disable 809
        protected override ISet<string> HandleKeySet()
#pragma warning disable 809
        {
            return new HashSet<string>();
        }

        /**
         * {@icu} Returns the size of a resource. Size for scalar types is always 1, and for
         * vector/table types is the number of child resources.
         *
         * <br><b>Note:</b> Integer array is treated as a scalar type. There are no APIs to
         * access individual members of an integer array. It is always returned as a whole.
         * @return number of resources in a given resource.
         * @stable ICU 3.8
         */
        public virtual int Length
        {
            get { return 1; }
        }

        /**
         * {@icu} Returns the type of a resource.
         * Available types are {@link #INT INT}, {@link #ARRAY ARRAY},
         * {@link #BINARY BINARY}, {@link #INT_VECTOR INT_VECTOR},
         * {@link #STRING STRING}, {@link #TABLE TABLE}.
         *
         * @return type of the given resource.
         * @stable ICU 3.8
         */
        public virtual int Type // ICU4N TODO: API - make into an enum
        {
            get { return NONE; }
        }

        /**
         * {@icu} Return the version number associated with this UResourceBundle as an
         * VersionInfo object.
         * @return VersionInfo object containing the version of the bundle
         * @stable ICU 3.8
         */
        public VersionInfo Version
        {
            get { return null; }
        }


        /**
         * {@icu} Returns the iterator which iterates over this
         * resource bundle
         * @return UResourceBundleIterator that iterates over the resources in the bundle
         * @stable ICU 3.8
         */
        public UResourceBundleEnumerator GetEnumerator()
        {
            return new UResourceBundleEnumerator(this);
        }

        #region .NET Compatibility
        IEnumerator<UResourceBundle> IEnumerable<UResourceBundle>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        /**
         * {@icu} Returns the key associated with a given resource. Not all the resources have
         * a key - only those that are members of a table.
         * @return a key associated to this resource, or null if it doesn't have a key
         * @stable ICU 3.8
         */
        public virtual string Key
        {
            get { return null; }
        }

        /**
         * {@icu} Resource type constant for "no resource".
         * @stable ICU 3.8
         */
        public const int NONE = -1; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * {@icu} Resource type constant for strings.
         * @stable ICU 3.8
         */
        public const int STRING = 0; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * {@icu} Resource type constant for binary data.
         * @stable ICU 3.8
         */
        public const int BINARY = 1; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * {@icu} Resource type constant for tables of key-value pairs.
         * @stable ICU 3.8
         */
        public const int TABLE = 2; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * {@icu} Resource type constant for a single 28-bit integer, interpreted as
         * signed or unsigned by the getInt() function.
         * @see #getInt
         * @stable ICU 3.8
         */
        public const int INT32 = 7; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * {@icu} Resource type constant for arrays of resources.
         * @stable ICU 3.8
         */
        public const int ARRAY = 8; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        /**
         * Resource type constant for vectors of 32-bit integers.
         * @see #getIntVector
         * @stable ICU 3.8
         */
        public const int INT32_VECTOR = 14; // ICU4N TODO: API - make into enum named UResourceType (returns from Type property)

        //====== protected members ==============

        /**
         * {@icu} Actual worker method for fetching a resource based on the given key.
         * Sub classes must override this method if they support resources with keys.
         * @param aKey the key string of the resource to be fetched
         * @param aliasesVisited hashtable object to hold references of resources already seen
         * @param requested the original resource bundle object on which the get method was invoked.
         *                  The requested bundle and the bundle on which this method is invoked
         *                  are the same, except in the cases where aliases are involved.
         * @return UResourceBundle a resource associated with the key
         * @stable ICU 3.8
         */
        protected virtual UResourceBundle HandleGet(string aKey, IDictionary<string, string> aliasesVisited,
                                            UResourceBundle requested)
        {
            return null;
        }

        /**
         * {@icu} Actual worker method for fetching a resource based on the given index.
         * Sub classes must override this method if they support arrays of resources.
         * @param index the index of the resource to be fetched
         * @param aliasesVisited hashtable object to hold references of resources already seen
         * @param requested the original resource bundle object on which the get method was invoked.
         *                  The requested bundle and the bundle on which this method is invoked
         *                  are the same, except in the cases where aliases are involved.
         * @return UResourceBundle a resource associated with the index
         * @stable ICU 3.8
         */
        protected virtual UResourceBundle HandleGet(int index, IDictionary<string, string> aliasesVisited,
                                            UResourceBundle requested)
        {
            return null;
        }

        /**
         * {@icu} Actual worker method for fetching the array of strings in a resource.
         * Sub classes must override this method if they support arrays of strings.
         * @return String[] An array of strings containing strings
         * @stable ICU 3.8
         */
        protected virtual string[] HandleGetStringArray()
        {
            return null;
        }

        /**
         * {@icu} Actual worker method for fetching the keys of resources contained in the resource.
         * Sub classes must override this method if they support keys and associated resources.
         *
         * @return Enumeration An enumeration of all the keys in this resource.
         * @stable ICU 3.8
         */
        protected virtual IEnumerable<string> HandleGetKeys()
        {
            return null;
        }

        /**
         * {@inheritDoc}
         * @stable ICU 3.8
         */
        // this method is declared in ResourceBundle class
        // so cannot change the signature
        // Override this method
        protected override object HandleGetObject(string aKey)
        {
            return HandleGetObjectImpl(aKey, this);
        }

        /**
         * Override the superclass method
         */
        // To facilitate XPath style aliases we need a way to pass the reference
        // to requested locale. The only way I could figure out is to implement
        // the look up logic here. This has a disadvantage that if the client
        // loads an ICUResourceBundle, calls ResourceBundle.getObject method
        // with a key that does not exist in the bundle then the lookup is
        // done twice before throwing a MissingResourceExpection.
        private object HandleGetObjectImpl(string aKey, UResourceBundle requested)
        {
            object obj = ResolveObject(aKey, requested);
            if (obj == null)
            {
                UResourceBundle parentBundle = Parent;
                if (parentBundle != null)
                {
                    obj = parentBundle.HandleGetObjectImpl(aKey, requested);
                }
                if (obj == null)
                    throw new MissingManifestResourceException(
                        "Can't find resource for bundle "
                        + this.GetType().FullName + ", key " + aKey);
            }
            return obj;
        }

        // Routine for figuring out the type of object to be returned
        // string or string array
        private object ResolveObject(string aKey, UResourceBundle requested)
        {
            if (Type == STRING)
            {
                return GetString();
            }
            UResourceBundle obj = HandleGet(aKey, null, requested);
            if (obj != null)
            {
                if (obj.Type == STRING)
                {
                    return obj.GetString();
                }
                try
                {
                    if (obj.Type == ARRAY)
                    {
                        return obj.HandleGetStringArray();
                    }
                }
                catch (UResourceTypeMismatchException)
                {
                    return obj;
                }
            }
            return obj;
        }

        /**
         * Is this a top-level resource, that is, a whole bundle?
         * @return true if this is a top-level resource
         * @internal
         * @deprecated This API is ICU internal only.
         */
        [Obsolete("This API is ICU internal only.")]
        protected virtual bool IsTopLevelResource
        {
            get { return true; }
        }
    }

    // ICU4N: temporary stub until we work out how to implement ResourceBundle
    public abstract class ResourceBundle
    {
        /**
         * The parent bundle of this bundle.
         * The parent bundle is searched by {@link #getObject getObject}
         * when this bundle does not contain a particular resource.
         */
        protected internal ResourceBundle parent = null;

        public virtual ResourceBundle Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual void SetParent(ResourceBundle parent)
        {
            this.parent = parent;
        }

        public abstract CultureInfo GetLocale();


        public object GetObject(string key)
        {
            object obj = HandleGetObject(key);
            if (obj == null)
            {
                if (parent != null)
                {
                    obj = parent.GetObject(key);
                }
                if (obj == null)
                {
                    throw new MissingManifestResourceException("Can't find resource for bundle " +
                        this.GetType().Name + ", key " + key);
                }
            }
            return obj;
        }

        public string GetString(string key)
        {
            return (string)GetObject(key);
        }

        public string[] GetStringArray(string key)
        {
            return (string[])GetObject(key);
        }

        public abstract IEnumerable<string> GetKeys();

        public virtual ISet<string> KeySet()
        {
            ISet<string> keys = new HashSet<string>();
            for (ResourceBundle rb = this; rb != null; rb = rb.parent)
            {
                keys.UnionWith(rb.HandleKeySet());
            }
            return keys;
        }

        protected ISet<string> keySet = null;

        protected virtual ISet<string> HandleKeySet()
        {
            if (keySet == null)
            {
                lock(this) {
                    if (keySet == null)
                    {
                        ISet<string> keys = new HashSet<string>();
                        using (var enumKeys = GetKeys().GetEnumerator())
                        {
                            while (enumKeys.MoveNext())
                            {
                                string key = enumKeys.Current;
                                if (HandleGetObject(key) != null)
                                {
                                    keys.Add(key);
                                }
                            }
                        }
                        keySet = keys;
                    }
                }
            }
            return keySet;
        }

        protected abstract object HandleGetObject(string key);
    }
}