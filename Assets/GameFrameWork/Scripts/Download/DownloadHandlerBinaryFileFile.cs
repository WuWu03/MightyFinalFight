using GameFrameWork.Utils;
using System.IO;
using UnityEngine.Networking;

namespace GameFrameWork.Download
{
    public class DownloadHandlerFile : DownloadHandlerScript
    {
        public ulong startDownloadLength { get; private set; }
        public DownloadHandlerFile(string uri, string version) : base()
        {
            InitDownloadFile(uri, version);
        }

        public DownloadHandlerFile(string uri, string version, byte[] buffer) : base(buffer)
        {
            InitDownloadFile(uri, version);
        }

        private void InitDownloadFile(string uri, string version)
        {
            string fileName = Path.GetFileName(uri);
            string downloadFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, fileName);
            string downloadVersionFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, fileName, ".downloadversion");

            if (!string.IsNullOrEmpty(version))
            {
                if (File.Exists(downloadVersionFilePath))
                {
                    string versionTemp = File.ReadAllText(downloadVersionFilePath);

                    if (versionTemp == version)
                    {
                        startDownloadLength = GetStartDownloadLength(downloadFilePath);
                    }
                    else
                    {
                        FileUtil.CreateTextFile(downloadVersionFilePath, version);
                        FileUtil.DeleteFile(downloadFilePath);
                        startDownloadLength = GetStartDownloadLength(downloadFilePath);
                    }
                }
                else
                {
                    FileUtil.CreateTextFile(downloadVersionFilePath, version);
                    FileUtil.DeleteFile(downloadFilePath);
                    startDownloadLength = GetStartDownloadLength(downloadFilePath);
                }
            }
            else
            {
                FileUtil.DeleteFile(downloadVersionFilePath);
                startDownloadLength = GetStartDownloadLength(downloadFilePath);
            }

            m_DownloadVersionFilePath = downloadVersionFilePath;
            m_FileStream = new FileStream(downloadFilePath, FileMode.Append, FileAccess.Write);
        }

        protected override byte[] GetData() { return null; }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
            {
                return false;
            }

            m_ReceivedLength += dataLength;
            m_FileStream.Write(data, 0, dataLength);
            return true;
        }

        protected override float GetProgress()
        {
            if (m_ContentLength <= 0)
            {
                return 0;
            }

            return (float)m_ReceivedLength / m_ContentLength;
        }

        protected override void CompleteContent()
        {
            if (!string.IsNullOrEmpty(m_DownloadVersionFilePath))
            {
                FileUtil.DeleteFile(m_DownloadVersionFilePath);
            }

            Dispose();
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            m_ContentLength = contentLength;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream.Dispose();
                m_FileStream = null;
            }
        }

        private ulong GetStartDownloadLength(string downloadFilePath)
        {
            if (File.Exists(downloadFilePath))
            {
                FileInfo fileInfo = new(downloadFilePath);
                return fileInfo != null ? (ulong)fileInfo.Length : 0;
            }

            FileUtil.CreateBinaryFile(downloadFilePath, null);
            return 0;
        }


        private ulong m_ContentLength = 0;
        private int m_ReceivedLength = 0;
        private string m_DownloadVersionFilePath = string.Empty;
        private FileStream m_FileStream;
    }
}