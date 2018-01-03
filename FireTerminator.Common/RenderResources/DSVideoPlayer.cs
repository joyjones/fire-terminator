using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectShowLib;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace FireTerminator.Common
{
    /// <summary>
    /// Describes the state of a video player
    /// </summary>
    public enum VideoState
    {
        Playing,
        Paused,
        Stopped
    }

    public class DSVideoInfo
    {
        #region Media Type GUIDs
        public static readonly Guid MEDIATYPE_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        public static readonly Guid MEDIASUBTYPE_RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);
        public static readonly Guid FORMAT_VideoInfo = new Guid(0x05589f80, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);
        #endregion

        /// <summary>
        /// Currently Loaded Video File
        /// </summary>
        public string FileName = "";
        /// <summary>
        /// Average Time per Frame in milliseconds
        /// </summary>
        public long AvgTimePerFrame = 0;
        /// <summary>
        /// BitRate of the currently loaded video
        /// </summary>
        public int BitRate = 0;
        /// <summary>
        /// Video duration
        /// </summary>
        public long Duration = 0;
        /// <summary>
        /// Width of the loaded video
        /// </summary>
        public int Width = 0;
        /// <summary>
        /// Height of the loaded video
        /// </summary>
        public int Height = 0;
    }

    /// <summary>
    /// Enables Video Playback in Microsoft XNA
    /// </summary>
    public class DSVideoPlayer : ISampleGrabberCB, IDisposable
    {
        public DSVideoPlayer(string filename, GraphicsDevice graphicsDevice)
        {
            try
            {
                // Open DirectShow Interfaces
                InitInterfaces();

                Info = new DSVideoInfo();
                // Create a SampleGrabber Filter and add it to the FilterGraph
                SampleGrabber sg = new SampleGrabber();
                ISampleGrabber sampleGrabber = (ISampleGrabber)sg;
                DsError.ThrowExceptionForHR(FG_GraphBuilder.AddFilter((IBaseFilter)sg, "Grabber"));

                // Setup Media type info for the SampleGrabber
                AMMediaType mt = new AMMediaType();
                mt.majorType = DSVideoInfo.MEDIATYPE_Video;     // Video
                mt.subType = DSVideoInfo.MEDIASUBTYPE_RGB24;    // RGB24
                mt.formatType = DSVideoInfo.FORMAT_VideoInfo;   // VideoInfo
                DsError.ThrowExceptionForHR(sampleGrabber.SetMediaType(mt));

                //// Construct the rest of the FilterGraph
                DsError.ThrowExceptionForHR(FG_GraphBuilder.RenderFile(filename, null));
                Info.FileName = filename;

                //// Set SampleGrabber Properties
                DsError.ThrowExceptionForHR(sampleGrabber.SetBufferSamples(true));
                DsError.ThrowExceptionForHR(sampleGrabber.SetOneShot(false));
                DsError.ThrowExceptionForHR(sampleGrabber.SetCallback((ISampleGrabberCB)this, 1));

                // Hide Default Video Window
                IVideoWindow pVideoWindow = (IVideoWindow)FG_GraphBuilder;
                DsError.ThrowExceptionForHR(pVideoWindow.put_AutoShow(OABool.False));

                //// Create AMMediaType to capture video information
                AMMediaType MediaType = new AMMediaType();
                DsError.ThrowExceptionForHR(sampleGrabber.GetConnectedMediaType(MediaType));
                VideoInfoHeader pVideoHeader = new VideoInfoHeader();
                Marshal.PtrToStructure(MediaType.formatPtr, pVideoHeader);

                // Store video information
                Info.Height = pVideoHeader.BmiHeader.Height;
                Info.Width = pVideoHeader.BmiHeader.Width;
                Info.AvgTimePerFrame = pVideoHeader.AvgTimePerFrame;
                Info.BitRate = pVideoHeader.BitRate;
                DsError.ThrowExceptionForHR(FG_MediaSeeking.GetDuration(out Info.Duration));

                // Create byte arrays to hold video data
                videoFrameBytes = new byte[(Info.Height * Info.Width) * 4]; // RGBA format (4 bytes per pixel)
                bgrData = new byte[(Info.Height * Info.Width) * 3];         // BGR24 format (3 bytes per pixel)

                // Create Output Frame Texture2D with the height and width of the video
                outputFrame = new Texture2D(graphicsDevice, Info.Width, Info.Height, 1, TextureUsage.None, SurfaceFormat.Color);
            }
            catch(Exception ex)
            {
                throw new Exception("不能加载或播放该视频: " + ex.Message);
            }
        }

        private void InitInterfaces()
        {
            FG = new FilterGraph();
            FG_GraphBuilder = (IGraphBuilder)FG;
            FG_MediaControl = (IMediaControl)FG;
            FG_MediaEventEx = (IMediaEventEx)FG;
            FG_MediaSeeking = (IMediaSeeking)FG;
            FG_MediaPosition = (IMediaPosition)FG;
        }
        private void ReleaseInterface()
        {
            if (FG_MediaEventEx != null)
            {
                DsError.ThrowExceptionForHR(FG_MediaControl.Stop());
                //0x00008001 = WM_GRAPHNOTIFY
                DsError.ThrowExceptionForHR(FG_MediaEventEx.SetNotifyWindow(IntPtr.Zero, 0x00008001, IntPtr.Zero));
            }
            FG_MediaControl = null;
            FG_MediaEventEx = null;
            FG_GraphBuilder = null;
            FG_MediaSeeking = null;
            FG_MediaPosition = null;
            if (FG != null)
                Marshal.ReleaseComObject(FG);
            FG = null;
        }
        
        public DSVideoInfo Info
        {
            get;
            private set;
        }
        /// <summary>
        /// The Main FilterGraph Com Object
        /// </summary> 
        private FilterGraph FG = null;

        /// <summary>
        /// The GraphBuilder interface ref
        /// </summary>
        public IGraphBuilder FG_GraphBuilder
        {
            get;
            private set;
        }

        /// <summary>
        /// The MediaControl interface ref
        /// </summary>
        public IMediaControl FG_MediaControl
        {
            get;
            private set;
        }
        /// <summary>
        /// The MediaEvent interface ref
        /// </summary>
        public IMediaEventEx FG_MediaEventEx
        {
            get;
            private set;
        }
        /// <summary>
        /// The MediaPosition interface ref
        /// </summary>
        public IMediaPosition FG_MediaPosition
        {
            get;
            private set;
        }
        /// <summary>
        /// The MediaSeeking interface ref
        /// </summary>
        public IMediaSeeking FG_MediaSeeking
        {
            get;
            private set;
        }
        #region Private Fields
        
        /// <summary>
        /// Is a new frame avaliable to update?
        /// </summary>
        private bool frameAvailable = false;

        /// <summary>
        /// Array to hold the raw data from the DirectShow video stream.
        /// </summary>
        private byte[] bgrData;

        /// <summary>
        /// The RGBA frame bytes used to set the data in the Texture2D Output Frame
        /// </summary>
        private byte[] videoFrameBytes;

        /// <summary>
        /// Private Texture2D to render video to. Created in the Video Player Constructor.
        /// </summary> 
        private Texture2D outputFrame;

        /// <summary>
        /// Current state of the video player
        /// </summary>
        private VideoState currentState = VideoState.Stopped;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// Current time position
        /// </summary>
        private long currentPosition;

        /// <summary>
        /// How transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        private byte alphaTransparency = 255;
        #endregion

        #region Public Properties
        /// <summary>
        /// Automatically updated video frame. Render this to the screen using a SpriteBatch.
        /// </summary>
        public Texture2D OutputFrame
        {
            get
            {
                return outputFrame;
            }
        }

        /// <summary>
        /// Gets or Sets the current position of playback in seconds
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                return (double)currentPosition / 10000000;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > Duration)
                    value = Duration;

                DsError.ThrowExceptionForHR(FG_MediaPosition.put_CurrentPosition(value));
                currentPosition = (long)value * 10000000;
            }
        }

        /// <summary>
        /// Returns the current position of playback, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string CurrentPositionAsTimeString
        {
            get
            {
                double seconds = (double)currentPosition / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Total duration in seconds
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)Info.Duration / 10000000;
            }
        }

        /// <summary>
        /// Returns the duration of the video, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string DurationAsTimeString
        {
            get
            {
                double seconds = (double)Info.Duration / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Gets or Sets the current state of the video player
        /// </summary>
        public VideoState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                switch (value)
                {
                    case VideoState.Playing:
                        Play();
                        break;
                    case VideoState.Paused:
                        Pause();
                        break;
                    case VideoState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        /// <summary>
        /// Event which occurs when the video stops playing once it has reached the end of the file
        /// </summary>
        public event EventHandler OnVideoComplete;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// Number of Frames Per Second in the video file.
        /// Returns -1 if this cannot be calculated.
        /// </summary>
        public int FramesPerSecond
        {
            get
            {
                if (Info.AvgTimePerFrame == 0)
                    return -1;
                float frameTime = (float)Info.AvgTimePerFrame / 10000000.0f;
                float framesPS = 1.0f / frameTime;
                return (int)Math.Round(framesPS, 0, MidpointRounding.ToEven);
            }
        }

        /// <summary>
        /// The number of milliseconds between each frame
        /// Returns -1 if this cannot be calculated
        /// </summary>
        public float MillisecondsPerFrame
        {
            get
            {
                if (Info.AvgTimePerFrame == 0)
                    return -1;
                return (float)Info.AvgTimePerFrame / 10000.0f;
            }
        }

        /// <summary>
        /// Gets or sets how transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        public byte AlphaTransparency
        {
            get
            {
                return alphaTransparency;
            }
            set
            {
                alphaTransparency = value;
            }
        }
        #endregion

        #region Update and Media Control
        /// <summary>
        /// Updates the Output Frame data using data from the video stream. Call this in Game.Update().
        /// </summary>
        public void Update()
        {
            // Remove the OutputFrame from the GraphicsDevice to prevent an InvalidOperationException on the SetData line.
            if (outputFrame.GraphicsDevice.Textures[0] == outputFrame)
            {
                outputFrame.GraphicsDevice.Textures[0] = null;
            }

            // Set video data into the Output Frame
            outputFrame.SetData<byte>(videoFrameBytes);
            
            // Update current position read-out
            DsError.ThrowExceptionForHR(FG_MediaSeeking.GetCurrentPosition(out currentPosition));
            if (currentPosition >= Duration * 10000000)
            {
                DsError.ThrowExceptionForHR(FG_MediaSeeking.SetPositions(0, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning));
            }
        }
        private ManualResetEvent StoppingEvent = new ManualResetEvent(false);

        /// <summary>
        /// Starts playing the video
        /// </summary>
        public void Play()
        {
            if (currentState != VideoState.Playing)
            {
                StoppingEvent.Reset();

                // Create video threads
                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateBuffer));
                ThreadPool.QueueUserWorkItem(new WaitCallback(WaitForCompletion));

                // Start the FilterGraph
                DsError.ThrowExceptionForHR(FG_MediaControl.Run());

                // Update VideoState
                currentState = VideoState.Playing;
            }
        }

        /// <summary>
        /// Pauses the video
        /// </summary>
        public void Pause()
        {
            // End threads
            StoppingEvent.Set();

            // Stop the FilterGraph (but remembers the current position)
            DsError.ThrowExceptionForHR(FG_MediaControl.Stop());

            // Update VideoState
            currentState = VideoState.Paused;
        }

        /// <summary>
        /// Stops playing the video
        /// </summary>
        public void Stop()
        {
            StoppingEvent.Set();
            
            // Stop the FilterGraph
            DsError.ThrowExceptionForHR(FG_MediaControl.Stop());

            // Reset the current position
            DsError.ThrowExceptionForHR(FG_MediaSeeking.SetPositions(0, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning));

            // Update VideoState
            currentState = VideoState.Stopped;
        }

        /// <summary>
        /// Rewinds the video to the start and plays it again
        /// </summary>
        public void Rewind()
        {
            Stop();
            Play();
        }
        #endregion

        #region ISampleGrabberCB Members and Helpers
        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (BufferLen > bgrData.Length)
                BufferLen = bgrData.Length;
            // Copy raw data into bgrData byte array
            Marshal.Copy(pBuffer, bgrData, 0, BufferLen);

            // Flag the new frame as available
            frameAvailable = true;

            // Return S_OK
            return 0;
        }

        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            // Return S_OK
            return 0;
        }

        /// <summary>
        /// Worker to copy the BGR data from the video stream into the RGBA byte array for the Output Frame.
        /// </summary>
        private void UpdateBuffer(object state)
        {
            int waitTime = Info.AvgTimePerFrame != 0 ? (int)((float)Info.AvgTimePerFrame / 10000) : 20;

            int samplePosRGBA = 0;
            int samplePosRGB24 = 0;

            while (!StoppingEvent.WaitOne(0))
            {
                for (int y = 0, y2 = Info.Height - 1; y < Info.Height; y++, y2--)
                {
                    for (int x = 0; x < Info.Width; x++)
                    {
                        samplePosRGBA = (((y2 * Info.Width) + x) * 4);
                        samplePosRGB24 = ((y * Info.Width) + x) * 3;

                        videoFrameBytes[samplePosRGBA + 0] = bgrData[samplePosRGB24 + 0];
                        videoFrameBytes[samplePosRGBA + 1] = bgrData[samplePosRGB24 + 1];
                        videoFrameBytes[samplePosRGBA + 2] = bgrData[samplePosRGB24 + 2];
                        videoFrameBytes[samplePosRGBA + 3] = alphaTransparency;
                    }
                }

                frameAvailable = false;
                while (!frameAvailable && !StoppingEvent.WaitOne(0))
                    Thread.Sleep(waitTime);
            }
        }

        /// <summary>
        /// Waits for the video to finish, then calls the OnVideoComplete event
        /// </summary>
        private void WaitForCompletion(object state)
        {
            int waitTime = Info.AvgTimePerFrame != 0 ? (int)((float)Info.AvgTimePerFrame / 10000) : 20;

            try
            {
                while (Info.Duration > currentPosition)
                {
                    if (StoppingEvent.WaitOne(0))
                        return;
                    Thread.Sleep(waitTime);
                }

                if (OnVideoComplete != null)
                    OnVideoComplete.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Cleans up the Video Player. Must be called when finished with the player.
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;

            Stop();
            ReleaseInterface();

            outputFrame.Dispose();
            outputFrame = null;
        }
        #endregion
    }
}
