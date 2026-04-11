using System.Runtime.InteropServices;
using System.Text;
using System;
using UnityEngine;
using System.Diagnostics;
using Unity.VisualScripting;

public class AppSurveillance : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pid);

    private static readonly string[] _browsers = { "chrome", "msedge", "whale", "firefox", "opera", "brave" };

    private void Start()
    {
        InvokeRepeating(nameof(InspectUrl), 1f, 3f);
    }

    void InspectUrl()
    {
        if(GameManager.Instance.PlayMode == PlayMode.Concentration)
        {
            string url = GetCurrentUrl();
            if (url == string.Empty) return;

            if (!WhitelistManager.Instance.IsContainAllowUrl(url))
            {
                GameManager.Instance._player.DecreaseHP(1);
            }
        }
    }

    public string GetCurrentUrl()
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
            if (!isBrowser)
            {
                return string.Empty;
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