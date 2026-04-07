using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WhitelistManager : MonoBehaviour
{
    public static WhitelistManager Instance { get; private set; }

    private Whitelistdata _data = new Whitelistdata();
    private string _savePath;

    public List<string> GetProcessList() => _data.allowProcessList;
    public List<string> GetUrlList() => _data.allowURLList;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "whitelist.json");
        Load();
    }

    #region 로드와 저장
    /// <summary>
    /// 로드
    /// </summary>
    void Load()
    {
        if (File.Exists(_savePath))
        {
            _data = JsonUtility.FromJson<Whitelistdata>(File.ReadAllText(_savePath));
        }
    }
    /// <summary>
    /// 저장
    /// </summary>
    void Save()
    {
        File.WriteAllText(_savePath, JsonUtility.ToJson(_data,true));
    }
    #endregion

    #region 프로세스 관리
    /// <summary>
    /// 프로그램 추가
    /// </summary>
    /// <param name="processName"></param>
    public void AddProcess(string processName)
    {
        string key = Normalize(processName);
        if (!_data.allowProcessList.Contains(key))
        {
            _data.allowProcessList.Add(key);
            Save();
        }
    }
    /// <summary>
    /// 프로그램 제거
    /// </summary>
    /// <param name="processName"></param>
    public void RemoveProcess(string processName)
    {
        _data.allowProcessList.Remove(Normalize(processName));
        Save();
    }
    #endregion

    #region url 관리
    /// <summary>
    /// 프로그램 추가
    /// </summary>
    /// <param name="url"></param>
    public void AddUrl(string url)
    {
        string key = NormalizeUrl(url);
        if (!_data.allowURLList.Contains(key))
        {
            _data.allowURLList.Add(key);
            Save();
        }
    }
    /// <summary>
    /// 프로그램 제거
    /// </summary>
    /// <param name="url"></param>
    public void RemoveUrl(string url)
    {
        _data.allowURLList.Remove(NormalizeUrl(url));
        Save();
    }
    #endregion

    #region 유틸 함수

    string Normalize(string s) => s.ToLower().Replace(".exe", "").Trim();
    string NormalizeUrl(string s) => s.ToLower().Replace("www.", "")
        .Replace("https://","").Replace("http://", "").Trim();

    #endregion
}
