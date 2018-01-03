// SinkWriterSample.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "SinkWriter.h"

#include <Windows.h>
#include <mfapi.h>
#include <mfidl.h>
#include <Mfreadwrite.h>
#include <mferror.h>
#include <wchar.h>

#pragma comment(lib, "mfreadwrite")
#pragma comment(lib, "mfplat")
#pragma comment(lib, "mfuuid")
namespace SinkWriter
{
	template <class T> void SafeRelease(T **ppT)
	{
		if (*ppT)
		{
			(*ppT)->Release();
			*ppT = NULL;
		}
	}

	// Format constants
	UINT32 VIDEO_WIDTH = 640;
	UINT32 VIDEO_HEIGHT = 480;
	UINT32 VIDEO_FPS = 25;
	UINT32 VIDEO_BIT_RATE = 800000;

	UINT32 VIDEO_PELS = VIDEO_WIDTH * VIDEO_HEIGHT;
	UINT32 VIDEO_FRAME_COUNT = 10 * VIDEO_FPS;

	const GUID   VIDEO_ENCODING_FORMAT = MFVideoFormat_WMV3;
	const GUID   VIDEO_INPUT_FORMAT = MFVideoFormat_RGB32;

	// Buffer to hold the video frame data.
	DWORD* videoFrameBuffer = NULL;

	HRESULT InitializeSinkWriter(IMFSinkWriter **ppWriter, DWORD *pStreamIndex)
	{
		*ppWriter = NULL;
		*pStreamIndex = NULL;

		IMFSinkWriter   *pSinkWriter = NULL;
		IMFMediaType    *pMediaTypeOut = NULL;   
		IMFMediaType    *pMediaTypeIn = NULL;   
		DWORD           streamIndex;     

		TCHAR cPath[MAX_PATH];
		GetCurrentDirectory(MAX_PATH, cPath);
		wcscat_s(cPath, L"\\output.wmv");
		HRESULT hr = MFCreateSinkWriterFromURL(cPath, NULL, NULL, &pSinkWriter);

		// Set the output media type.
		if (SUCCEEDED(hr))
		{
			hr = MFCreateMediaType(&pMediaTypeOut);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeOut->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);     
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeOut->SetGUID(MF_MT_SUBTYPE, VIDEO_ENCODING_FORMAT);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeOut->SetUINT32(MF_MT_AVG_BITRATE, VIDEO_BIT_RATE);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeOut->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeSize(pMediaTypeOut, MF_MT_FRAME_SIZE, VIDEO_WIDTH, VIDEO_HEIGHT);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeRatio(pMediaTypeOut, MF_MT_FRAME_RATE, VIDEO_FPS, 1);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeRatio(pMediaTypeOut, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pSinkWriter->AddStream(pMediaTypeOut, &streamIndex);   
		}

		// Set the input media type.
		if (SUCCEEDED(hr))
		{
			hr = MFCreateMediaType(&pMediaTypeIn);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeIn->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeIn->SetGUID(MF_MT_SUBTYPE, VIDEO_INPUT_FORMAT);     
		}
		if (SUCCEEDED(hr))
		{
			hr = pMediaTypeIn->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeSize(pMediaTypeIn, MF_MT_FRAME_SIZE, VIDEO_WIDTH, VIDEO_HEIGHT);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeRatio(pMediaTypeIn, MF_MT_FRAME_RATE, VIDEO_FPS, 1);   
		}
		if (SUCCEEDED(hr))
		{
			hr = MFSetAttributeRatio(pMediaTypeIn, MF_MT_PIXEL_ASPECT_RATIO, 1, 1);   
		}
		if (SUCCEEDED(hr))
		{
			hr = pSinkWriter->SetInputMediaType(streamIndex, pMediaTypeIn, NULL);   
		}

		// Tell the sink writer to start accepting data.
		if (SUCCEEDED(hr))
		{
			hr = pSinkWriter->BeginWriting();
		}

		// Return the pointer to the caller.
		if (SUCCEEDED(hr))
		{
			*ppWriter = pSinkWriter;
			(*ppWriter)->AddRef();
			*pStreamIndex = streamIndex;
		}

		SafeRelease(&pSinkWriter);
		SafeRelease(&pMediaTypeOut);
		SafeRelease(&pMediaTypeIn);
		return hr;
	}

	HRESULT WriteFrame(IMFSinkWriter *pWriter, 
					   DWORD streamIndex, 
					   const LONGLONG& rtStart,        // Time stamp.
					   const LONGLONG& rtDuration      // Frame duration.
					   )
	{
		IMFSample *pSample = NULL;
		IMFMediaBuffer *pBuffer = NULL;

		const LONG cbWidth = 4 * VIDEO_WIDTH;
		const DWORD cbBuffer = cbWidth * VIDEO_HEIGHT;

		BYTE *pData = NULL;

		// Create a new memory buffer.
		HRESULT hr = MFCreateMemoryBuffer(cbBuffer, &pBuffer);

		// Lock the buffer and copy the video frame to the buffer.
		if (SUCCEEDED(hr))
		{
			hr = pBuffer->Lock(&pData, NULL, NULL);
		}
		if (SUCCEEDED(hr))
		{
			hr = MFCopyImage(
				pData,                      // Destination buffer.
				cbWidth,                    // Destination stride.
				(BYTE*)videoFrameBuffer,    // First row in source image.
				cbWidth,                    // Source stride.
				cbWidth,                    // Image width in bytes.
				VIDEO_HEIGHT                // Image height in pixels.
				);
		}
		if (pBuffer)
		{
			pBuffer->Unlock();
		}

		// Set the data length of the buffer.
		if (SUCCEEDED(hr))
		{
			hr = pBuffer->SetCurrentLength(cbBuffer);
		}

		// Create a media sample and add the buffer to the sample.
		if (SUCCEEDED(hr))
		{
			hr = MFCreateSample(&pSample);
		}
		if (SUCCEEDED(hr))
		{
			hr = pSample->AddBuffer(pBuffer);
		}

		// Set the time stamp and the duration.
		if (SUCCEEDED(hr))
		{
			hr = pSample->SetSampleTime(rtStart);
		}
		if (SUCCEEDED(hr))
		{
			hr = pSample->SetSampleDuration(rtDuration);
		}

		// Send the sample to the Sink Writer.
		if (SUCCEEDED(hr))
		{
			hr = pWriter->WriteSample(streamIndex, pSample);
		}

		SafeRelease(&pSample);
		SafeRelease(&pBuffer);
		return hr;
	}

	IMFSinkWriter *g_CurSinkWriter = NULL;
	UINT64 g_rtDuration = 0;
	UINT64 g_rtPosStart = 0;
	DWORD g_dwStreamIndex = -1;

	VOID InitializeSinkWriter(UINT32 width, UINT32 height, UINT32 fps, UINT32 bps)
	{
		VIDEO_WIDTH = width;
		VIDEO_HEIGHT = height;
		VIDEO_FPS = fps;
		VIDEO_BIT_RATE = bps;
		VIDEO_PELS = VIDEO_WIDTH * VIDEO_HEIGHT;
		if (videoFrameBuffer != NULL)
			delete[] videoFrameBuffer;
		videoFrameBuffer = new DWORD[VIDEO_PELS];
		// Set all pixels to green
		for (DWORD i = 0; i < VIDEO_PELS; ++i)
		{
			videoFrameBuffer[i] = 0x000000FF;
		}
	}

	HRESULT StartSinkWriter()
	{
		HRESULT hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
		if (!SUCCEEDED(hr))
			return hr;
		hr = MFStartup(MF_VERSION);
		if (!SUCCEEDED(hr))
		{
			CoUninitialize();
			return hr;
		}				
		hr = InitializeSinkWriter(&g_CurSinkWriter, &g_dwStreamIndex);
		if (!SUCCEEDED(hr))
		{
			SafeRelease(&g_CurSinkWriter);
			MFShutdown();
			return hr;
		}

		MFFrameRateToAverageTimePerFrame(VIDEO_FPS, 1, &g_rtDuration);
		UINT64 du = g_rtDuration;
		return S_OK;
	}

	HRESULT WriteFrame(const DWORD buff[])
	{
		memcpy(videoFrameBuffer, buff, VIDEO_PELS * 4);

		HRESULT hr = WriteFrame(g_CurSinkWriter, g_dwStreamIndex, g_rtPosStart, g_rtDuration);
		if (FAILED(hr))
			return hr;
		g_rtPosStart += g_rtDuration;
		return S_OK;
	}

	VOID FinishSinkWriter()
	{
		if (g_CurSinkWriter != NULL)
		{
			g_CurSinkWriter->Finalize();
			SafeRelease(&g_CurSinkWriter);

			MFShutdown();
			CoUninitialize();
		}
		g_rtDuration = 0;
		g_rtPosStart = 0;
		g_dwStreamIndex = -1;
	}
}
