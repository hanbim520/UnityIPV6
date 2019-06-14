using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;

public class TestIpv6 : MonoBehaviour {
    private string apkUrl = "http://116.211.25.6:10091/client/checkversion?version=1.0.0.0&areaId=11001&serverId=11011";

    Socket socket = null;
    // Use this for initialization
    void Start()
    {
        //  CheckNetWork();
        StartCoroutine(Download(apkUrl));
    }

    private void CheckNetWork()
    {

        ////IPAddress[] addresses = Dns.GetHostAddresses("www.baidu.com");
        //IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
        //foreach (var info in addresses)
        //{
        //    Debug.Log(string.Format("type is {0},IP is {1}",info.AddressFamily,info));
        //    if (AddressFamily.InterNetworkV6.Equals(info.AddressFamily))
        //    {
        //        Debug.Log("[ socket network ]------ipv6------");
        //        socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        //    }else if (AddressFamily.InterNetwork.Equals(info.AddressFamily))
        //    {
        //        Debug.Log("[ socket network ]------ipv4------");
        //        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    }
        //}
        //socket.Connect("116.211.25.126", 8000);

        SocketClient("116.211.25.126", "8000");
    }
    IEnumerator check(Socket socket)
    {
        while (true)
        {
            yield return null;
            UnityEngine.Debug.Log(socket.Connected);
        }

    }

    public void SocketClient(string serverIp, String serverPorts)
    {
        String newServerIp = "";
        AddressFamily newAddressFamily = AddressFamily.InterNetwork;
        IPV6.GetIPType(serverIp, serverPorts, out newServerIp, out newAddressFamily);
        if (!string.IsNullOrEmpty(newServerIp)) { serverIp = newServerIp; }
        Socket socketClient = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);

        IAsyncResult asyncResult = socketClient.BeginConnect(newServerIp, 8000, null, null);
        bool isFinished = asyncResult.AsyncWaitHandle.WaitOne(30, false);


        Debug.Log("Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + serverIp);
        //socketClient.Connect(serverIp, 8000);
        StartCoroutine(check(socketClient));
    }


    private IEnumerator Download(string _url)
    {
        //设置保存路径
        string path = Application.persistentDataPath + "/wod.apk";
        Debug.Log(path);
        //这个方法可以新建一个线程运行，来提高效率和降低卡顿，这里就不写了
        _url = IPV6.FinalUrl(_url);
        //  Debug.Log("IPV6 URL:" + _url);
        //  _url = "http://[2001:2:0:1baa::74d3:1906]:10091/client/checkversion?version=1.0.0.0&areaId=11001&serverId=11011";
        Uri url = new Uri(_url);
        Debug.Log("Request URL:" + url.AbsoluteUri);
        //创建接受
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        request.ProtocolVersion = HttpVersion.Version10;
        request.Timeout = 5000;
        //以下为接收响应的方法
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        byte[] bytes = new byte[response.ContentLength];
        using (Stream stream = response.GetResponseStream())
        {
            stream.Read(bytes, 0, bytes.Length);
            stream.Flush();
            Debug.Log(System.Text.Encoding.Default.GetString(bytes));
        }
        yield return null;
        Debug.Log("下载完成 finished");

    }

}
