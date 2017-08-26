
using System;

public class MessageHeader {
    public const int HEADER_SIZE = 32;//header size in bytes

    private int messageType;
    public int MessageType { get { return messageType; } }

    private int bodySize;
    public int BodySize { get { return bodySize; } }

    private bool compressed;
    public bool IsCompressed { get { return compressed; } }

    public static MessageHeader ParseFromBytes(byte[] messageData) {
        byte[] headerBytes = new byte[HEADER_SIZE];
        Array.Copy(messageData, 0, headerBytes, 0, headerBytes.Length);

        //messageType (int) 4 bytes
        int messageType = Utils.ByteArrToInt(headerBytes, 0);

        //bodySize (int) 4 bytes
        int bodySize = Utils.ByteArrToInt(headerBytes, 4);

        //compressed (int) 4 bytes (0=no, 1=yes)
        bool compressed = (Utils.ByteArrToInt(headerBytes, 8) == 0) ? false : true;

        return new MessageHeader(messageType, bodySize, compressed);
    }

    public MessageHeader(int _messageType, int _bodySize, bool _compressed) {
        messageType = _messageType;
        bodySize = _bodySize;
        compressed = _compressed;
    }

    public byte[] ToByteArr() {
        //header 32 bytes
        //  message type (int) 4 bytes
        //  body length (int) 4 bytes
        //  compressed (int) 4 bytes (0=no, 1=yes)

        byte[] headerData = new byte[HEADER_SIZE];

        byte[] messageTypeBytes = Utils.IntToBytes(messageType);
        byte[] messageBodyLengthBytes = Utils.IntToBytes(bodySize);
        byte[] messageCompressed = Utils.IntToBytes((compressed) ? 1 : 0);

        Array.Copy(messageTypeBytes,        0, headerData, 0, 4);
        Array.Copy(messageBodyLengthBytes,  0, headerData, 4, 4);
        Array.Copy(messageCompressed,       0, headerData, 8, 4);

        return headerData;
    }
}

