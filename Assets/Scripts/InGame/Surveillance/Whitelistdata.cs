using System;
using System.Collections.Generic;

[Serializable]
public class Whitelistdata
{
    public List<string> allowProcessList = new List<string>();//허용 프로그램
    public List<string> allowURLList = new List<string>();//허용 url
}
