using Accord.Video.FFMPEG;

namespace Core.Library.Media
{
    internal class EncoderSettings
    {
        internal int BitRate = 1000000;
        internal VideoCodec Codec = VideoCodec.MPEG4;
        internal string FileExt = "wmv";
        internal int FrameRate = 25;
        internal int Interval = 40;

        public EncoderSettings()
        {
            if (Method == EncoderMethod.WebDriver)
            {
                FrameRate = 1;
                Interval = 50;
                BitRate = 100000;
            }
        }

        internal EncoderMethod Method
        {
            get
            {
                if (!ConfigManager.SupportsVideo)
                    return EncoderMethod.None;

                if (ConfigManager.UseWebDriverVideoCapture && ConfigManager.SupportsVideo)
                    return EncoderMethod.WebDriver;
                return EncoderMethod.Default;
            }
        }

        internal enum EncoderMethod
        {
            Default,
            WebDriver,
            None
        }
    }
}