using GameFrameWork.Download;
using GameFrameWork.Utils;
using GameFrameWork.WebRequest;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.Version
{
    public enum VersionProcessState
    {
        DontCheckVersion,
        CheckVersion,
        CheckVersionUriError,
        CheckVersionFileError,
        CheckVersionComplete,
        VersionAnalyze,
        VersionAnalyzeError,
        VersionAnalyzeComplete,
        DownloadFiles,
        DownloadFilesError,
        DownloadFilesComplete,
        Success,
        Error,
    }

    public class VersionMgr : BaseMgr<VersionMgr>
    {
        public event GameFrameWorkAction<VersionProcessState, string, ulong, ulong> onVersionProcessStateChangedEvent
        {
            add
            {
                m_OnVersionProcessStateChangedEvent += value;
            }
            remove
            {
                m_OnVersionProcessStateChangedEvent -= value;
            }
        }

        public void SetCheckVersionUri(string uri)
        {
            m_CheckUri = uri;
            Check();
        }

        private void Check()
        {
            if (!GameFrameWorkEntry.config.isCheckVersion)
            {
                Log.LogInfo("框架未开启本版更新功能，不进行版本验证");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.DontCheckVersion, string.Empty, 0, 0);
                return;
            }

            if (string.IsNullOrEmpty(m_CheckUri))
            {
                Log.LogError("版本验证地址错误，请检查");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.CheckVersionUriError, string.Empty, 0, 0);
                return;
            }

            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.CheckVersion, string.Empty, 0, 0);
            WebRequestMgr.instance.AddWebRequest(PathUtil.FormatPath(m_CheckUri, GameFrameWorkEntry.config.versionFileName), "CheckVersionFile", OnRequestVersionFileComplete, OnRequestVersionFileProgress, OnRequestVersionFileError);
        }

        private void OnRequestVersionFileComplete(UnityWebRequest unityWebRequest)
        {
            StartCoroutine(ReadyToDownload(unityWebRequest));
        }

        private IEnumerator ReadyToDownload(UnityWebRequest unityWebRequest)
        {
            if (unityWebRequest == null || unityWebRequest.downloadHandler == null || string.IsNullOrEmpty(unityWebRequest.downloadHandler.text))
            {
                Log.LogError("版本文件为空");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.CheckVersionFileError, string.Empty, 0, 0);
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.CheckVersionComplete, string.Empty, 0, 0);
            yield return new WaitForSeconds(0.5f);
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.VersionAnalyze, string.Empty, 0, 0);
            m_VersionFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, GameFrameWorkEntry.config.versionFileName);
            m_VersionFileContent = unityWebRequest.downloadHandler.text.Trim();
            m_DownloadTempFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, "VersionMgrDownloadTemp.downloadtemp");

            VersionInfo[] newVersionInfos = GetVersionInfos(m_VersionFileContent);

            if (newVersionInfos == null || newVersionInfos.Length < 1)
            {
                Log.LogError("版本文件解析失败，请检查");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.VersionAnalyzeError, string.Empty, 0, 0);
                yield break;
            }

            VersionInfo[] versionInfos = null;

            if (!File.Exists(m_VersionFilePath))
            {
                Log.LogInfo("当前版本需要进行全量更新");
            }
            else
            {
                string versionFile = File.ReadAllText(m_VersionFilePath);
                versionInfos = GetVersionInfos(versionFile);

                if (versionInfos == null || newVersionInfos == null)
                {
                    Log.LogError("版本文件解析失败，请检查");
                    m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.VersionAnalyzeError, string.Empty, 0, 0);
                    yield break;
                }
            }

            VersionInfo[] downloadTempVersionInfos = null;

            if (File.Exists(m_DownloadTempFilePath))
            {
                string downlaodTempFile = File.ReadAllText(m_DownloadTempFilePath);
                downloadTempVersionInfos = GetVersionInfos(downlaodTempFile);
            }

            List<VersionInfo> downloadInfos = new();

            for (int i = 0; i < newVersionInfos.Length; i++)
            {
                if (TryFindVersionInfo(versionInfos, newVersionInfos[i].fileName, out VersionInfo versionInfo))
                {
                    if (versionInfo.fileMd5 == newVersionInfos[i].fileMd5)
                    {
                        Log.LogInfo("文件 [", newVersionInfos[i].fileName, "] 已是最新版本，无需更新");
                        continue;
                    }
                }

                if (TryFindVersionInfo(downloadTempVersionInfos, newVersionInfos[i].fileName, out VersionInfo tempVersionInfo))
                {
                    if (tempVersionInfo.fileMd5 == newVersionInfos[i].fileMd5)
                    {
                        Log.LogInfo("文件 [", newVersionInfos[i].fileName, "] 已存在于临时下载文件中，跳过下载");
                        continue;
                    }
                }

                Log.LogInfo("文件 [", newVersionInfos[i].fileName, "] 需要更新，MD5: [", newVersionInfos[i].fileMd5, "] Size: [", newVersionInfos[i].fileSize.ToString(), "]");
                downloadInfos.Add(newVersionInfos[i]);
            }

            yield return new WaitForSeconds(0.5f);
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.VersionAnalyzeComplete, string.Empty, 0, 0);
            yield return new WaitForSeconds(0.5f);
            DownloadFiles(downloadInfos);
        }

        private void DownloadFiles(List<VersionInfo> downloadInfos)
        {
            if (downloadInfos == null || downloadInfos.Count == 0)
            {
                Log.LogInfo("没有需要更新的文件");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.Success, string.Empty, 0, 0);
                m_CurrDownloadSize = 0;
                m_DownloadFullSize = 0;
                m_VersionFilePath = string.Empty;
                m_VersionFileContent = string.Empty;
                m_OnVersionProcessStateChangedEvent = null;
                return;
            }

            m_CurrDownloadCount = 0;
            m_DownloadFullCount = 0;
            m_CurrDownloadSize = 0;
            m_DownloadFullSize = 0;

            foreach (VersionInfo versionInfo in downloadInfos)
            {
                m_DownloadFullCount++;
                m_DownloadFullSize += versionInfo.fileSize;
            }

            Log.LogInfo("开始下载需要更新的文件，文件数量：", downloadInfos.Count.ToString(), " 总下载字节数：", m_DownloadFullSize.ToString());

            foreach (VersionInfo versionInfo in downloadInfos)
            {
                string fileUri = PathUtil.FormatPath(m_CheckUri, versionInfo.fileName);
                DownloadMgr.instance.AddDownloadBinaryFile(fileUri, versionInfo.fileName, versionInfo.fileMd5, versionInfo.fileSize, OnDownloadComplete, OnDownLoadProgress, OnDownloadError);
            }
        }

        private VersionInfo[] GetVersionInfos(string versionFile)
        {
            if (string.IsNullOrEmpty(versionFile))
            {
                return null;
            }

            string[] versionContents = versionFile.Split('\n');
            List<VersionInfo> versionInfos = new();

            for (int i = 0; i < versionContents.Length; i++)
            {
                if (string.IsNullOrEmpty(versionContents[i]))
                {
                    continue;
                }

                string[] versionInfoParts = versionContents[i].Split('|');

                if (versionInfoParts == null || versionInfoParts.Length != 3)
                {
                    continue;
                }

                versionInfos.Add(new()
                {
                    fileName = versionInfoParts[0].Trim(),
                    fileMd5 = versionInfoParts[1].Trim(),
                    fileSize = ulong.Parse(versionInfoParts[2].Trim())
                });
            }

            return versionInfos.ToArray();
        }

        private bool TryFindVersionInfo(VersionInfo[] versionInfos, string fileName, out VersionInfo versionInfo)
        {
            versionInfo = default;
            if (versionInfos == null || versionInfos.Length == 0 || string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            for (int i = 0; i < versionInfos.Length; i++)
            {
                if (versionInfos[i].fileName == fileName)
                {
                    versionInfo = versionInfos[i];
                    return true;
                }
            }

            return false;
        }

        private void OnRequestVersionFileProgress(float progress)
        {

        }

        private void OnRequestVersionFileError(string errorMsg)
        {
            Log.LogError("版本验证请求失败，错误信息：", errorMsg);
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.Error, errorMsg, 0, 0);
        }

        private void OnDownloadComplete(string uri, string tag, string version, byte[] data, ulong downloadSize)
        {
            m_CurrDownloadSize += downloadSize;
            m_CurrDownloadCount++;
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.DownloadFiles, uri, m_CurrDownloadSize, m_DownloadFullSize);
            string filePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, tag);
            FileUtil.CreateBinaryFile(filePath, data);
            FileUtil.AppendText(m_DownloadTempFilePath, StringUtil.Append(m_CurrDownloadCount > 1 ? "\n" : string.Empty, tag, "|", version, "|", downloadSize.ToString()));
            Log.LogInfo(m_CurrDownloadCount.ToString(), "[", Path.GetFileName(uri), "]下载完成");

            if (m_CurrDownloadCount >= m_DownloadFullCount)
            {
                Log.LogInfo("所有文件下载完成，总下载字节数：", m_CurrDownloadSize.ToString());
                FileUtil.CreateTextFile(m_VersionFilePath, m_VersionFileContent);
                FileUtil.DeleteFile(m_DownloadTempFilePath);
                Log.LogInfo("版本文件已保存到：", m_VersionFilePath);
                Log.LogInfo("更新完毕");
                m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.Success, string.Empty, 0, 0);
                m_CurrDownloadSize = 0;
                m_DownloadFullSize = 0;
                m_VersionFilePath = string.Empty;
                m_VersionFileContent = string.Empty;
                m_DownloadTempFilePath = string.Empty;
                m_OnVersionProcessStateChangedEvent = null;
                DownloadMgr.instance.RemoveAllDownload();
            }
        }

        private void OnDownLoadProgress(string uri, string tag, string version, byte[] data, ulong currDownloadSize, ulong downloadFullSize)
        {
            Log.LogInfo("开始下载文件 [", Path.GetFileName(uri), "] 进度：", currDownloadSize.ToString(), "/", downloadFullSize.ToString(), " / ", data.Length.ToString());
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.DownloadFiles, uri, m_CurrDownloadSize + currDownloadSize, m_DownloadFullSize);
        }

        private void OnDownloadError(string uri, string tag, string version, string errorMsg)
        {
            Log.LogError("下载文件 [", uri, "] 失败，错误信息：", errorMsg);
            m_OnVersionProcessStateChangedEvent?.Invoke(VersionProcessState.DownloadFilesError, errorMsg, 0, 0);
        }

        private ulong m_CurrDownloadSize = 0;
        private ulong m_DownloadFullSize = 0;
        private int m_CurrDownloadCount = 0;
        private int m_DownloadFullCount = 0;
        private string m_VersionFilePath = string.Empty;
        private string m_VersionFileContent = string.Empty;
        private string m_DownloadTempFilePath = string.Empty;
        private string m_CheckUri = string.Empty;
        private event GameFrameWorkAction<VersionProcessState, string, ulong, ulong> m_OnVersionProcessStateChangedEvent = null;
    }
}
