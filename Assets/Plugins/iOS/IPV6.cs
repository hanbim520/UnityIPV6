using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;

public class IPV6  {

   

    [DllImport("__Internal")]
    private static extern string getIPv6OnlyHost(string host);
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);

    public static string GetIPv6(string mHost, string mPort)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        string mIPv6 = getIPv6(mHost, mPort);
        return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    public static void GetIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily mIPType)
    {
        mIPType = AddressFamily.InterNetwork;
        newServerIp = serverIp;
        try
        {
            string mIPv6 = GetIPv6(serverIp, serverPorts);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newServerIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception e)
        {
           Debug.LogError("GetIPv6 error:" + e);
        }

    }

    /// <summary>
    /// 拿当前的ip地址或者域名来获取对应ipv6的地址，，如果当前环境不支持ipv6，返回 当前的ipv4地址或者对应域名
    /// </summary>
    /// <param name="hostOrHostName">Host or host name.</param>
    public static string GetIPv6OnlyHost(string hostOrHostName)
    {

        string ip = hostOrHostName;
#if UNITY_IPHONE && !UNITY_EDITOR
        if (IsIPAdress (hostOrHostName)) {
            try{
                ip = getIPv6OnlyHost(hostOrHostName);
                if (!string.IsNullOrEmpty (ip)) {
                    string[] tmp = System.Text.RegularExpressions.Regex.Split (ip, "&&");
                    if (tmp != null && tmp.Length >= 2) {
                        string type = tmp[1];
                        if (type == "ipv6") {
                            ip = tmp[0];
                            Debug.Log ("---ipv6--- " + ip);
                        }else if(type == "ipv4") {
                            ip = tmp[0];
                            Debug.Log ("---ipv4--- " + ip);
                        }
                    }
                }
            }catch(Exception e){
                Debug.LogErrorFormat ("GetIPv6 error: {0}", e.Message);
            }

        } else {
            ip = GetIPV6Adress (hostOrHostName);
        }
#else
        return hostOrHostName;
#endif
        Debug.Log("hostOrHostName: -----" + hostOrHostName + "  -------- ip " + ip);
        return ip;
    }
    //判断str是域名还是ip
    private static bool IsIPAdress(string str)
    {
        Match match = Regex.Match(str, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        return match.Success;
    }
    /// <summary>
    /// 获取域名对应ipv6地址
    /// </summary>
    /// <returns>The IP v6 adress.</returns>
    /// <param name="hostName">Host name.</param>
    private static string GetIPV6Adress(string hostName)
    {
        //基础操作系统和网络适配器是否支持 Internet 协议版本 6 (IPv6)。 ,,并且域名不为null
        if (!System.Net.Sockets.Socket.OSSupportsIPv6 || string.IsNullOrEmpty(hostName))
            return null;
        System.Net.IPHostEntry host;
        string connectIP = "";
        try
        {
            host = System.Net.Dns.GetHostEntry(hostName);
            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    connectIP = ip.ToString();
                }
                else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    connectIP = ip.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("GetIPAddress error: {0}", e.Message);
        }
        Debug.Log("---connectIP--- " + connectIP);
        return connectIP;
    }

    //传一个url转换成ipv6 的地址
    public static string FinalUrl(string url)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        string[] strs = url.Split ('/');
        if (strs.Length < 2)
            return url;
        string hostOrName = strs [2];
        string finalIp = "";
                //如果有端口去掉端口
        if (hostOrName.Contains (":")) {
            hostOrName= hostOrName.Split (':') [0];
        }

        finalIp =GetIPv6OnlyHost (hostOrName);
        //解析后的域名，通过是否包含冒号来判断是ipv6还是ipv4如果是ipv6格式的加上[] 不是ivp6格式不需要加
        if (finalIp.Contains (":")) {
            finalIp = string.Format ("[{0}]", finalIp);
        }
        string finalUrl = url.Replace (hostOrName, finalIp);
        return finalUrl;
#endif
        //只有在苹果真机上才会处理IP 其他情况直接返回 url
        return url;
    }
}
