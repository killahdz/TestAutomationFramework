using System;
using System.IO;
using Core.Library.Extensions;
using NUnit.Framework;

namespace Core.Library
{
    /// <summary>
    ///     Store paths stuff in here
    /// </summary>
    public class Paths
    {
        public static string WorkingDir => TestContext.CurrentContext.WorkDirectory;

        //public static string BuildDir
        //{
        //    get { return $"{WorkingDir}\\{AcceptanceTestConfig.BuildNumber}"; }
        //}

        public static string ArtifactDir => $"{WorkingDir}\\Artifacts";

        public static string VideoDir => $"{ArtifactDir}\\Video";

        public static string ImagesDir => $"{ArtifactDir}\\Images";

        public static string FramesDir => $"{ImagesDir}\\Frames";

        public static string LogsDir
        {
            get
            {
                var dir = $"{ArtifactDir}\\Logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string ResourcesDir
        {
            get
            {
                var dir = $"{WorkingDir}\\Resources";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string AttachmentPath
        {
            get
            {
                var path = $"{ResourcesDir}\\attachment.txt";
                if (!File.Exists(path))
                    File.Create(path);
                return path;
            }
        }

        public static void CleanUpArtifacts()
        {
            try
            {
                if (Directory.Exists(VideoDir))
                    Directory.Delete(VideoDir, true);

                if (Directory.Exists(ImagesDir))
                    Directory.Delete(ImagesDir, true);
            }
            catch (Exception ex)
            {
                //Log something
                var x = ex.Message;
            }
        }

        public class Browsers
        {
            public static string Chrome => ConfigManager.ChromeExe;

            /// <summary>
            ///     Not implemented
            /// </summary>
            public static string Ie => null;
        }

        public class Media
        {
            public enum ArtifactType
            {
                Video,
                Image
            }

            public static string GetVideoPath(string filename, string ext)
            {
                var videoFile = $"{VideoDir}\\{filename.SanitizeFileName(200)}.{ext}";

                return videoFile;
            }

            public static string GetImagePath(string featureName, string testName, string filename)
            {
                //create a directory for the feature
                var featureDir = $"{ImagesDir}\\{featureName}\\{testName.SanitizeFileName(50)}";
                if (!Directory.Exists(featureDir))
                    Directory.CreateDirectory(featureDir);

                var imageFile = $"{featureDir}\\{filename.SanitizeFileName(200)}.jpg";

                return imageFile;
            }

            public static string SetPassFail(string filename, bool pass = true, bool deleteOnPass = true)
            {
                if (filename == null)
                    return null;

                var result = pass ? "PASS" : "FAIL";
                var newFilename = $"{Path.GetDirectoryName(filename)}\\({result}){Path.GetFileName(filename)}";

                if (pass && deleteOnPass)
                    File.Delete(filename);
                else
                    File.Move(filename, newFilename);
                return newFilename;
            }

            /// <summary>
            ///     Makes a file link for display in TeamCity logs
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static string EncodeFileLink(string filename)
            {
                var encodedFilename = filename
                    .Replace("\\", "/");
                //.Replace(" ", "_");

                //encodedFilename = HttpUtility.HtmlEncode(encodedFilename);

                return encodedFilename;
            }
        }
    }
}