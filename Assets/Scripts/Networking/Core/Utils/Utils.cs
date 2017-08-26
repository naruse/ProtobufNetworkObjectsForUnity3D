/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
 */

using System;
using System.Net;
using System.Net.Sockets;

public static class Utils {
    public static string GetIP() {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    public static int ByteArrToInt(byte[] bytes) {
        return ByteArrToInt(bytes, 0);
    }
    public static int ByteArrToInt(byte[] bytes, int offset) {
        byte[] intBytes = new byte[4];
        Array.Copy(bytes, offset, intBytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(intBytes);
        return BitConverter.ToInt32(intBytes, 0);
    }

    public static byte[] IntToBytes(int intValue) {
        byte[] intBytes = BitConverter.GetBytes(intValue);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(intBytes);
        return intBytes;
    }
}
