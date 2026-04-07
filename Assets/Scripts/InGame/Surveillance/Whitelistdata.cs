using System;
using System.Collections.Generic;

public class Whitelistdata
{
    public List<string> allowProcessList = new List<string>();//허용 프로그램
    public List<string> allowURLList = new List<string>();//허용 url
    //체크가 필요한 브라우저
    public List<string> browserProcessList = new List<string>()
    {
        "naver", "google", "chrome", "whale", "namu"
    };
}
