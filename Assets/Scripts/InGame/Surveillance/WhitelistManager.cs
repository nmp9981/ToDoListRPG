using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WhitelistManager : MonoBehaviour
{
    public static WhitelistManager Instance { get; private set; }

    private Whitelistdata _data = new Whitelistdata();
    private string _savePath;

    public List<string> GetProcessList() => _data.allowProcessList;
    public List<string> GetUrlList() => _data.allowURLList;

    [SerializeField] private GameObject _listItemPrefab;
   
    [Header("프로세스 섹션")]
    [SerializeField] private TMP_InputField _processInput;
    [SerializeField] private Transform _processContent;

    [Header("URL 섹션")]
    [SerializeField] private TMP_InputField _urlInput;
    [SerializeField] private Transform _urlContent;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "whitelist.json");
        Load();
        _data.allowURLList.Clear();
        _data.allowProcessList.Clear();
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
    public void AddProcess()
    {
        string input = _processInput.text;
        if (input == string.Empty) return;

        string key = Normalize(input);
        _data.allowProcessList.Add(key);
        SpawnItem(_processContent, key);
        Save();
    }
    /// <summary>
    /// 프로그램 제거
    /// </summary>
    /// <param name="processName"></param>
    public void RemoveProcess(string processName, GameObject gm)
    {
        _data.allowProcessList.Remove(Normalize(processName));
        Save();
        Destroy(gm);
    }
    #endregion

    #region url 관리
    /// <summary>
    /// 프로그램 추가
    /// </summary>
    /// <param name="url"></param>
    public void AddUrl()
    {
        string input = _urlInput.text;
        if (input == string.Empty) return;

        string key = NormalizeUrl(input);
        _data.allowURLList.Add(key);
        SpawnItem(_urlContent, key);
        Save();
    }
    /// <summary>
    /// 프로그램 제거
    /// </summary>
    /// <param name="url"></param>
    public void RemoveUrl(string url, GameObject gm)
    {
        _data.allowURLList.Remove(NormalizeUrl(url));
        Save();
        Destroy(gm);
    }
    #endregion

    #region 검사
    /// <summary>
    /// 허용된 프로그램인가?
    /// </summary>
    /// <returns></returns>
    public bool IsContainAllowUProcess(string program)
    {
        if (_data.allowProcessList.Count == 0) return false;//허용 프로그램이 없음

        foreach (string factor in _data.allowProcessList)
        {
            if (factor.Contains(program)) return true;
        }
        return false;
    }

    /// <summary>
    /// 허용된 url인가?
    /// </summary>
    /// <returns></returns>
    public bool IsContainAllowUrl(string url)
    {
        if (_data.allowURLList.Count == 0) return false;//허용 url이 없음

        foreach(string factor in _data.allowURLList)
        {
            if (url.Contains(factor)) return true;//하용 url에 있음
        }
        return false;
    }
    #endregion

    #region 오브젝트 생성
    private void SpawnItem(Transform parent, string label)
    {
        var go = Instantiate(_listItemPrefab, parent);

        // 텍스트 설정
        var text = go.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = label;

        // 삭제 버튼 설정
        var btn = go.GetComponentInChildren<Button>();
        if (btn != null) btn.onClick.AddListener(delegate { RemoveUrl(label, go); });
    }
    #endregion

    #region 유틸 함수

    string Normalize(string s) => s.ToLower().Replace(".exe", "").Trim();
    string NormalizeUrl(string s) => s.ToLower().Replace("www.", "")
        .Replace("https://","").Replace("http://", "").Trim();

    #endregion
}
