/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com

  Simple test class that sends a SampleObject protobuf object
 */


using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class ClientTest : MonoBehaviour {

    private string ip = Utils.GetIP();
    private string port = "19861";


    string objName = "This is an obj name";
    string sampleStr = "This is a sample string";
    int sampleInt = -1;
    float sampleFloat = -0.5f;

    private void SendMessage(Message m) {
        try {
            byte[] data = m.ToByteArr();

            //create socket
            Debug.Log("Connecting to socket:" + ip + " " + port);

            Socket socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));

            socket.Connect(endpoint);
            socket.Send(data);
            socket.Close();

            Debug.Log("Sending (" + data.Length + ") bytes to: " + ip);
        } catch (System.Exception e) {
            throw e;
        }
    }






    private string log = "";
    void OnGUI() {
        float windowWidth = 290;
        float windowHeight = 270;
        Rect area = new Rect(Screen.width/2-windowWidth/2, Screen.height/2 - windowHeight/2,
                             windowWidth, windowHeight);
        GUILayout.BeginArea(area);
        GUI.Box(new Rect(0, 0, area.width, area.height), "Client");
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
            GUILayout.Label(" IP:");
            ip = GUILayout.TextField(ip);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label(" Port:");
            port = GUILayout.TextField(port);
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        GUILayout.Label(" === Sample Object fields: ===");
        GUILayout.BeginHorizontal();
            GUILayout.Label(" string objName:");
            objName = GUILayout.TextField(objName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            GUILayout.Label(" another string:");
            sampleStr = GUILayout.TextField(sampleStr);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            GUILayout.Label(" sample int:" + sampleInt);
            if(GUILayout.Button("-", GUILayout.Width(20))) sampleInt--;
            if(GUILayout.Button("+", GUILayout.Width(20))) sampleInt++;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            GUILayout.Label(" sample float:" + sampleFloat);
            if(GUILayout.Button("-", GUILayout.Width(20))) sampleFloat -= 0.1f;
            if(GUILayout.Button("+", GUILayout.Width(20))) sampleFloat += 0.1f;
        GUILayout.EndHorizontal();


        if(GUILayout.Button("Send Sample Object!")) {
            Message messageToSend = MessageCreator.CreateSampleObjectMessage(objName, sampleStr, sampleInt, sampleFloat);
            log = "Sending message to: " + ip + ":" + port;
            SendMessage(messageToSend);
        }
        GUILayout.EndArea();
        GUILayout.Label(log);
    }
}
