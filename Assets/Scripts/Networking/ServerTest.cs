/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;

public class ServerTest : MonoBehaviour {
    private SocketProtobufListener connector;
    private string ip = Utils.GetIP();
    private string port = "19861";
    private string log = "";

    private ConcurrentQueue<ProtobufMessage> messageQueue;//protobuf messages will get stored here

    void Start() {
        messageQueue = new ConcurrentQueue<ProtobufMessage>();
        connector = new SocketProtobufListener(ip, port, messageQueue);//<- tell the socket listener which messageQueue to use
        connector.Connect();
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(Screen.width/2-Screen.width/4, Screen.height/2-Screen.height/4,
                                     Screen.width/2, Screen.height/2));
        GUILayout.Box("Server");
        GUILayout.Label("Received Object:");
        GUILayout.Label(log);
        GUILayout.EndArea();
    }

    void OnApplicationQuit() {
        connector.Disconnect();
    }

    void Update() {
        if(messageQueue.Count > 0) {//when there's a protobuf message in the queue, we print it
            ProtobufMessage pm;
            messageQueue.TryDequeue(out pm);

            switch(pm.MessageType) {
                case ProtobufMessageTypes.SAMPLE_OBJECT:
                    SampleObject sampleObject = (SampleObject) pm.Protobuf;
                    PrintSampleObjectFields(sampleObject);
                    break;
            }
        }
    }

    void PrintSampleObjectFields(SampleObject s) {
        log = "Received SampleObject: \n";
        log += "ObjName: " + s.ObjectName + "\n";
        log += "another string" + s.SampleString + "\n";
        log += "Sample int" + s.SampleInt + "\n";
        log += "Sample float: " + s.SampleFloat;
    }
}