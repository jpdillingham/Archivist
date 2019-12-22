namespace Archivist
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    public static class Extensions
    {
        public static string RelativeName(this string path, string root)
        {
            Uri pathUri = new Uri(path);

            if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                root += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(root);
            
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string MD5Checksum(this FileInfo file)
        {
            try
            {
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(file.FullName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch
            {
                return "ERROR";
            }
        }

        public static string ToJson(this object obj)
        {
            return System.Text.Encoding.UTF8.GetString(Utf8Json.JsonSerializer.Serialize(obj));
        }
    }
}
