// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the SINKWRITERSAMPLE_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// SINKWRITERSAMPLE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef SINKWRITERSAMPLE_EXPORTS
#define SINKWRITERSAMPLE_API __declspec(dllexport)
#else
#define SINKWRITERSAMPLE_API __declspec(dllimport)
#endif
struct IMFSinkWriter;

extern "C"
{
	namespace SinkWriter
	{
		VOID SINKWRITERSAMPLE_API InitializeSinkWriter(UINT32 width, UINT32 height, UINT32 fps, UINT32 bps);
		HRESULT SINKWRITERSAMPLE_API WriteFrame(const DWORD buff[]);
		HRESULT SINKWRITERSAMPLE_API StartSinkWriter();
		VOID SINKWRITERSAMPLE_API FinishSinkWriter();
	}
}
