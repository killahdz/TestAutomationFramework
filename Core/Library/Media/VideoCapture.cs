using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video.FFMPEG;
using Core.Library.Extensions;
using Core.Library.Helpers;
using Core.Library.Specflow;
using Core.Library.WebDriver;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Core.Library.Media
{
    public class VideoCapture : IDisposable
    {
        private readonly CancellationTokenSource _ctSource;
        private readonly ProxiedWebDriver _driver;
        private DateTime _endTime;
        private string _filename;
        private int _frameCount = 0;
        private Rectangle _screenBounds;
        private DateTime _startTime;
        //private readonly VideoFileWriter _videoWriter;
        private string _frameDir;
        private EncoderSettings _encoderSettings = new EncoderSettings();


        public VideoCapture(ProxiedWebDriver driver)
        {
            _driver = driver;
            //init our video writer
            //_videoWriter = new VideoFileWriter();
            //token to cancel frame capture
            _ctSource = new CancellationTokenSource();
            //remember the screen dimensions - we are recording the entire primary display
            _screenBounds = Screen.PrimaryScreen.Bounds;
            //create the output dir if it doesnt exist
            if (!Directory.Exists(Paths.VideoDir))
                Directory.CreateDirectory(Paths.VideoDir);
        }

        public void Dispose()
        {
            //_videoWriter.Dispose();
            _ctSource.Dispose();
            CleanUpFrames();
        }

        /// <summary>
        ///     Starts Video recording
        /// </summary>
        /// <param name="filename"></param>
        public void Start(string filename)
        {
            
            //check settings
            if (_encoderSettings.Method == EncoderSettings.EncoderMethod.None)
            {
                Console.WriteLine("Video logging not supported for this configuration");
                return;
            }

            //get destination file path
            _filename = Paths.Media.GetVideoPath(filename, _encoderSettings.FileExt);
            
            try
            {
                //set up our thread to record frames and kick it off
                Task.Run(
                    () => PeriodicFrameCaptureAsync(TimeSpan.FromMilliseconds(_encoderSettings.Interval),
                        _ctSource.Token, _encoderSettings.Method), _ctSource.Token);
            }
            catch (Exception ex)
            {
                _driver.RecordLog($"Error starting capture task {ex}");
            }

            //record start time
            _startTime = DateTime.Now;

        }

        /// <summary>
        ///     Captures a frame every interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="encoderMethod"></param>
        /// <returns></returns>
        private async Task PeriodicFrameCaptureAsync(TimeSpan interval, CancellationToken cancellationToken,
            EncoderSettings.EncoderMethod encoderMethod)
        {
            while (true)
            {
                RecordFrameWebDriver();
                await Task.Delay(interval, cancellationToken);
            }
        }


        /// <summary>
        ///     Records a frame via webdriver screenshot functionality
        /// </summary>
        private void RecordFrameWebDriver()
        {
            //check for cancel
            if (_ctSource.IsCancellationRequested) return;

            var outputPath = Paths.Media.GetImagePath("frames", TestContext.CurrentContext.Test.Name.GetTestName(), _frameCount++.ToString());
            _driver.SaveScreenshot(outputPath);
            _frameDir = Path.GetDirectoryName(outputPath);
        }

        /// <summary>
        ///     Ends the recording session and cleans up the instance
        /// </summary>
        /// <returns></returns>
        public void End(ProxiedWebDriver driver, bool success = true)
        {
        
            _ctSource?.Cancel();
            _endTime = DateTime.Now;

            if (string.IsNullOrEmpty(_filename) ||
                string.IsNullOrEmpty(_frameDir) ||
                !Directory.Exists(_frameDir))
            {
                driver.RecordLog($"FrameStitch: No Frames dir or params not valid: _filename {_filename}, _frameDir {_frameDir}");
                return;
            }

            if (!success)
            {
                //stitch the captured frames together
                Stitch(_filename, _frameDir);
            }

        }

        private void CleanUpFrames()
        {
            //always delete frames directory
            try
            {
                //anything holding ref to images?
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Directory.Delete(Paths.FramesDir, true);
            }
            catch (Exception e)
            {
                _driver?.RecordLog("An error occurrred when deleting the frames directory." + e.ToString());
            }
        }

        /// <summary>
        /// Stitches all the frames together into a mp4 video
        /// Also deletes all the recorded frames
        /// </summary>
        /// <param name="targetFilename">destination video filename</param>
        /// <param name="sourceDir">directory where images are stored</param>
        public void Stitch(string targetFilename, string sourceDir)
        {

            //get the first frame in the directory to determine dimensions
            var frames = Directory.GetFiles(sourceDir).ToList();
            if (!frames.Any())
            {
                throw new NotImplementedException($"No frames to process in {sourceDir}");
            }

            var width = _screenBounds.Width;
            var height = _screenBounds.Height;
            var framRate = _encoderSettings.FrameRate;

            //prepare the frames for use
            var seq = 0;
            var orderedFrames = frames
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))) //ordered
                .Select(f => new
                {
                    Filename = f,
                    Index = seq++
                });

            Console.WriteLine($"Stitching {orderedFrames.Count()} frames to '{targetFilename}'");

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(targetFilename, width, height, framRate, VideoCodec.MPEG4);
                
                //loop throught all images in the collection
                foreach (var imageEntity in orderedFrames.OrderBy(i => i.Index))
                {
                    //get the raw data and resize to target dimensions
                    var rawFrame = Image.FromFile(imageEntity.Filename);
                    
                    var resized = ResizeImage(rawFrame, width, height);

                    try
                    {
                        //dont console log the frame writing
                        using (new ConsoleLogSink())
                        {
                            //FFPEG is a bit noisy
                            vFWriter.WriteVideoFrame(resized);
                        }
                    }
                    catch (Exception e)
                    {
                        _driver?.RecordLog($"Failed to write frame '{imageEntity.Filename}', width: {resized.Width}, Height: {resized.Height}, PixelFormat: {resized.PixelFormat}. {e}");
                    }

                }
                vFWriter.Close();
            }
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            GC.Collect();

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, image.PixelFormat);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Low;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}