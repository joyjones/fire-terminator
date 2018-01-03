using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Runtime.Serialization;

namespace FireTerminator.Common.Services
{
    public enum TransFileKind
    {
        ProjectFile,
        ResourceFile,
    }

    // 文件接收服务
    [ServiceContract]
    public interface IFileTransmissionService
    {
        // 向接收端查询传输许可性
        [OperationContract]
        bool BeginTransferFiles(TransferFileInfo[] files);
        // 向接收端传输文件
        [OperationContract]
        void TransferFile(FileTransferMessage request);
    }

    // 文件传输请求服务
    [ServiceContract]
    public interface ITransmissionRequestService
    {
        // 向发送端请求传输
        [OperationContract(IsOneWay = true)]
        void QueryTransmission(int transPort, TransFileKind kind, string[] files);
    }

    [DataContract]
    public class TransferFileInfo
    {
        public TransferFileInfo(Guid guid, TransFileKind kind, string file, long length, string md5)
        {
            GUID = guid;
            Kind = kind;
            FileName = file;
            FileLength = length;
            MD5Code = md5;
        }
        [DataMember]
        public Guid GUID = Guid.NewGuid();
        [DataMember]
        public TransFileKind Kind = TransFileKind.ResourceFile;
        [DataMember]
        public string FileName = "";
        [DataMember]
        public long FileLength = 0;
        [DataMember]
        public string MD5Code = "";
        [DataMember]
        public DateTime TransferTime = DateTime.Now;
    }

    [MessageContract]
    public class FileTransferMessage
    {
        // GUID
        [MessageHeader(MustUnderstand = true)]
        public Guid GUID;
        // 文件编号
        [MessageHeader(MustUnderstand = true)]
        public int Index = 0;
        // 文件流
        [MessageBodyMember(Order = 1)]
        public Stream Content = null;

        public string SavingFileName
        {
            get;
            set;
        }
    }
    public class FileTransState
    {
        public FileTransState(string name, long size, int count)
        {
            fileName = name;
            maxSize = size;
            curFileCount = count;
        }
        public string fileName = "";
        public long maxSize = 0;
        public long curSize = 0;
        public int curFileCount = 0;
    }
}
