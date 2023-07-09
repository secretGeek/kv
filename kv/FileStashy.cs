namespace StashyLib
{
    //hat-tip to: http://stashy.codeplex.com
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public partial class FileStashy : IStashy<string>
    {
        public string GetNewId<T1>() where T1 : new()
        => Guid.NewGuid().ToString();

        public void Save<T1>(T1 t1, string id)
        {
            var xmlFileName = GetFileName<T1>(id);
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            var xsZer = new XmlSerializer(typeof(T1));
            using (var writer = new StreamWriter(xmlFileName))
            {
                xsZer.Serialize(writer, t1);
            }
        }

        public T1 Load<T1>(string id) where T1 : new()
        {
            var xmlFileName = GetFileName<T1>(id);
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            return LoadFile<T1>(xmlFileName);
        }

        public IEnumerable<string> ListKeys<T1>() where T1 : new()
        {
            var xmlFilePath = GetObjectPath<T1>();
            EnsurePathExists(Path.GetDirectoryName(xmlFilePath));
            foreach (var f in Directory.EnumerateFiles(xmlFilePath))
            {
                yield return Path.GetFileName(f).DecodeFileNameString();
            }
        }

        public IEnumerable<string> ListKeys<T1>(string searchPattern) where T1 : new()
        {
            //encode the key so it will resemble the encoded file names
            searchPattern = searchPattern.EncodeFileNameString();
            //but decode any wildcards, so they can broaden the search.
            searchPattern = searchPattern.DecodeFileNameChar('*').DecodeFileNameChar('?');
            //note that ? wildcards won't be accurate, if attempting to match encoded chars, as they consume 3+, not 1, chars
            //searchPattern = searchPattern.DecodeFileNameChar('?');

            var xmlFilePath = GetObjectPath<T1>();
            EnsurePathExists(Path.GetDirectoryName(xmlFilePath));
            foreach (var f in Directory.EnumerateFiles(xmlFilePath, searchPattern))
            {
                yield return Path.GetFileName(f).DecodeFileNameString();
            }
        }

        public IEnumerable<T1> LoadAll<T1>() where T1 : new()
        {
            var xmlFilePath = GetObjectPath<T1>();
            EnsurePathExists(Path.GetDirectoryName(xmlFilePath));
            var all = new List<T1>();

            foreach (var xmlFileName in Directory.EnumerateFiles(xmlFilePath))
            {
                all.Add(LoadFile<T1>(xmlFileName));
            }

            return all;
        }

        public void Delete<T1>(string id)
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), id.EncodeFileNameString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            File.Delete(xmlFileName);
        }

        private static T1 LoadFile<T1>(string xmlFileName) where T1 : new()
        {
            if (!File.Exists(xmlFileName))
            {
                throw new FileNotFoundException("File cannot be loaded for serialization", xmlFileName);
            }

            var xsZer = new XmlSerializer(typeof(T1));
            using (var xreader = new XmlTextReader(File.OpenRead(xmlFileName)))
            {
                T1 result = (T1)xsZer.Deserialize(xreader);
                return result;
            }
        }

        public static string BasePath()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "kv");

        private static string GetObjectPath<T1>()
        => Path.Combine(BasePath(), typeof(T1).ToString().ToFolderNameString());
        private static string GetFileName<T1>(string id)
        => Path.Combine(GetObjectPath<T1>(), id.EncodeFileNameString());

        private static void EnsurePathExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DirectoryNotFoundException("Specified path was empty");
            }

            if (!Directory.Exists(path))
            {
                EnsurePathExists(Directory.GetParent(path).FullName);
                Directory.CreateDirectory(path);
            }
        }
    }

    public static class Extensions
    {
        public static string Replace(this string self, char[] invalidChars, char newChar)
        {
            foreach (var c in invalidChars)
            {
                self = self.Replace(c, newChar);
            }

            return self;
        }

        public static string ToFolderNameString(this string self)
        => self.Replace(Path.GetInvalidPathChars(), '_');

        public static string EncodeFileNameChar(this string self, char c)
        => self.Replace(c.ToString(), "[" + ((int)c).ToString() + "]");

        public static string DecodeFileNameChar(this string self, char c)
        => self.Replace("[" + ((int)c).ToString() + "]", c.ToString());

        public static string EncodeFileNameString(this string self)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            self = self.EncodeFileNameChar('[');

            foreach (var c in invalidChars)
            {
                self = self.EncodeFileNameChar(c);
            }

            return self;
        }

        public static string DecodeFileNameString(this string self)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                self = self.DecodeFileNameChar(c);
            }

            return self.DecodeFileNameChar('[');
        }

        public static string ToFileNameString(this string self)
        => self.Replace(Path.GetInvalidFileNameChars(), '_');
    }

    public interface IStashy<K>
    {
        void Save<T1>(T1 t1, K id);
        T1 Load<T1>(K id) where T1 : new();
        IEnumerable<T1> LoadAll<T1>() where T1 : new();
        void Delete<T1>(K id);
        K GetNewId<T1>() where T1 : new();
    }
}