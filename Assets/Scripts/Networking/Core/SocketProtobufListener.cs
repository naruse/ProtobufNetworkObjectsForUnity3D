/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class SocketProtobufListener {

    System.Threading.Thread SocketThread;
    Socket listener;
    Socket handler;
    private string host;
    private string port;
    private bool requestDisconnect = false;
    private static readonly object threadLock = new object();

    ConcurrentQueue<ProtobufMessage> messageQueue;

    public SocketProtobufListener(string _host, string _port, ConcurrentQueue<ProtobufMessage> mq) {
        host = _host;
        port = _port;
        messageQueue = mq;
    }

    public void Connect() {
        lock (threadLock) {
            Debug.Log("Creating a socket at " + host + ":" + port);
            requestDisconnect = false;
            SocketThread = new System.Threading.Thread(Listen);
            SocketThread.IsBackground = false;
            SocketThread.Start();
        }
    }


    public void Disconnect() {
        lock (threadLock) {
            Debug.Log("Closing socket...");
            requestDisconnect = true;

            try {
                if (listener != null)
                    listener.Close();
            } catch (Exception e) {
                Debug.Log(e);
            }

            try {
                if (handler != null)
                    handler.Close();
            } catch (Exception e) {
                Debug.Log(e);
            }

            if(SocketThread != null)
                SocketThread.Join();
        }
    }

    void Listen() {
        IPAddress ip = IPAddress.Parse(host);
        IPEndPoint localEndPoint = new IPEndPoint(ip, Convert.ToInt32(port));

        listener = new Socket(ip.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        listener.LingerState = new LingerOption(true, 0);

        try {// Bind the socket to the local endpoint and listen for incoming connections.
            Debug.Log("Binding socket to: " + localEndPoint);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (!requestDisconnect) {//Start listening for connections.
                handler = listener.Accept();// Program suspended while waiting for an incoming connection.

                if (requestDisconnect)
                    return;

                //read header
                byte[] headerBytes = new byte[MessageHeader.HEADER_SIZE];
                int bytesReceived = handler.Receive(headerBytes);
                MessageHeader header = MessageHeader.ParseFromBytes(headerBytes);

                //if 0 bytes received, return. No valid message.
                if (bytesReceived == 0) {
                    if (requestDisconnect)
                        return;
                    continue;
                }

                Debug.Log("Got message type <" + header.MessageType + ">, Parsing: " + header.BodySize + "bytes");

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                //read body
                int bodySize = header.BodySize;
                byte[] bodyBytes = new byte[bodySize];

                int messageChunkSize = 100 * 1024;
                int messageBytesRead = 0;

                while (messageBytesRead < bodySize) {
                    if (requestDisconnect)
                        return;

                    byte [] bytes = new byte[messageChunkSize];

                    int bytesRec = handler.Receive(bytes);
                    if (bytesRec <= 0) {
                        handler.Disconnect(true);
                        break;
                    }
                    Array.Copy(bytes, 0, bodyBytes, messageBytesRead, bytesRec);

                    messageBytesRead += bytesRec;

                    System.Threading.Thread.Sleep(1);
                }
                handler.Close();

                stopWatch.Stop();
                Debug.Log("Received " + messageBytesRead + " bytes in " + stopWatch.Elapsed.Milliseconds + " ms");
                stopWatch.Reset();

                //create message
                Message message = Message.ParseFromBytes(headerBytes, bodyBytes);

                ProtobufMessage protobufMessage = MessageDeserializer.DeserializeMessage(message);
                messageQueue.Enqueue(protobufMessage);

                System.Threading.Thread.Sleep(1);
            }
        } catch (Exception e) {
            Debug.Log("ERROR: " + localEndPoint + ": Exeception caught: " + e.ToString());

        }
    }
}
