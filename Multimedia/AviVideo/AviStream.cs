using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.AviVideo
{
	public abstract class AviStream
	{
		protected AviStream(int filePtr, IntPtr streamPtr)
		{
			FilePointer = filePtr;
			StreamPointer = streamPtr;
		}

		public int FilePointer { get; private set; }
		public IntPtr StreamPointer { get; private set; }

		protected AviInterop.StreamInfo GetStreamInfo()
		{
			var streamInfo = new AviInterop.StreamInfo();
			int result = AviInterop.AVIStreamInfo(StreamPointer, ref streamInfo,
				Marshal.SizeOf(streamInfo));
			if (result != 0)
				throw new Exception("Exception in VideoStreamInfo: " + AviErrors.GetError(result));

			return streamInfo;
		}

		public virtual void Close()
		{
			AviInterop.AVIStreamRelease(StreamPointer);
		}
	}
}