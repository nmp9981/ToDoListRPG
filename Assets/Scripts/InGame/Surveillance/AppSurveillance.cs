using System.Runtime.InteropServices;
using System.Text;
using System;
using UnityEngine;
using System.Diagnostics;


public class AppSurveillance : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pid);

    private static readonly string[] _browsers = { "chrome", "msedge", "whale", "firefox", "opera", "brave" };

    private void Start()
    {
        InvokeRepeating(nameof(InspectUrl), 3f, 5f);
    }
    /// <summary>
    /// URL 감시
    /// </summary>
    void InspectUrl()
    {
        if(GameManager.Instance.PlayMode == PlayMode.Concentration)
        {
            string url = GetCurrentUrl(true);
            string prog = GetCurrentUrl(false);
            var ins = GameManager.Instance._player;

            if (url != string.Empty)
            {
                if (!WhitelistManager.Instance.IsContainAllowUrl(url))
                {
                    ins.DecreaseHP(7);
                    ins.TodayLossHP += 7;
                    ins.CountOtherAction += 1;
                    ins.ConsumeConcentrateTime = Mathf.Max(0, ins.ConsumeConcentrateTime - 5);
                    ins.TotalConcentrateTime = Mathf.Max(0, ins.TotalConcentrateTime - 5);
                }
            }
            if (prog != string.Empty)
            {
                if (!WhitelistManager.Instance.IsContainAllowUProcess(prog))
                {
                    ins.DecreaseHP(7);
                    ins.TodayLossHP += 7;
                    ins.CountOtherAction += 1;
                    ins.ConsumeConcentrateTime = Mathf.Max(0, ins.ConsumeConcentrateTime - 5);
                    ins.TotalConcentrateTime = Mathf.Max(0, ins.TotalConcentrateTime - 5);
                }
            }
        }
    }
   
    /// <summary>
    /// URL or 프로그램 가져오기
    /// </summary>
    /// <param name="isURL"></param>
    /// <returns></returns>
    public string GetCurrentUrl(bool isURL)
    {
        try
        {
            // C#에서 직접 포커스 창 PID 읽기
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);

            var proc = Process.GetProcessById((int)pid);
            string procName = proc.ProcessName.ToLower();

            // 브라우저인지 확인
            bool isBrowser = Array.Exists(_browsers, b => b == procName);

            if (isURL)//URL 반환해야함
            {
                if (!isBrowser)//브라우저가 아님
                {
                    return string.Empty;
                }
            }
            else//프로그램 반환해야함
            {
                if (isBrowser)//브라우저
                {
                    return string.Empty;
                }
                else
                {
                    return procName;
                }
            }

            // PID를 PowerShell에 넘겨서 URL만 읽어오기
            string script = $@"
                Add-Type -AssemblyName UIAutomationClient
                Add-Type -AssemblyName UIAutomationTypes
                $hwnd = (Get-Process -Id {pid}).MainWindowHandle
                $element = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
                $condition = New-Object System.Windows.Automation.PropertyCondition(
                    [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                    [System.Windows.Automation.ControlType]::Edit)
                $edit = $element.FindFirst('Descendants', $condition)
                if ($edit) {{
                    $val = $edit.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
                    Write-Output $val.Current.Value
                }} else {{
                    Write-Output 'EDIT_NOT_FOUND'
                }}";

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"{script}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string result = process.StandardOutput.ReadToEnd().Trim();
            string error = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error)) UnityEngine.Debug.LogWarning($"[PS 에러] {error}");

            return result == "EDIT_NOT_FOUND" ? null : result;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[예외] {e.Message}");
            return null;
        }
    }
}