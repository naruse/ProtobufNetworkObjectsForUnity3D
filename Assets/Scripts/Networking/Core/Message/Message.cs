using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Message {

    public const int MESSAGE_COMPRESS_THRESHOLD_KB = 1 * 1024 * 1024;//1mb

    private MessageHeader _header;
    public MessageHeader Header { get { return _header; } }
    private MessageBody _body;
    public MessageBody Body { get { return _body; } }

    public static Message ParseFromBytes(byte[] messageData) {
        //split header and body bytes bytes
        MessageHeader header = MessageHeader.ParseFromBytes(messageData);
        int headerLength = MessageHeader.HEADER_SIZE;
        byte[] headerBytes = new byte[headerLength];
        Array.Copy(messageData, 0, headerBytes, 0, headerLength);

        int bodySize = header.BodySize;
        byte[] bodyBytes = new byte[bodySize];
        Array.Copy(messageData, headerLength, bodyBytes, 0, bodySize);

        return ParseFromBytes(headerBytes, bodyBytes);
    }

    public static Message ParseFromBytes(byte[] headerData, byte[] bodyData) {
        MessageHeader header = MessageHeader.ParseFromBytes(headerData);
        //Debug.Log("parseFromBytes. MessageType: " + header.getMessageType() + " compressed: " + header.isCompressed());
        MessageBody body = null;
        if (header.IsCompressed) {
            byte[] dataUncompressed = GZipUtils.Decompress(bodyData);
            body = new MessageBody(dataUncompressed);
        } else {
            body = new MessageBody(bodyData);
        }
        return new Message(header, body);
    }


    /// Create a message.
    /// Automatic compression will be performed for large messages
    public static Message Create(int type, byte[] bodyData) {
        int bodySize = bodyData.Length;

        MessageHeader header = null;
        MessageBody body = null;

        if (bodySize > MESSAGE_COMPRESS_THRESHOLD_KB) {//check if body should be compressed
            byte[] compressedBodyData = GZipUtils.Compress(bodyData);
            int compressedBodySize = compressedBodyData.Length;
            header = new MessageHeader(type, compressedBodySize, true);
            body = new MessageBody(compressedBodyData);
        } else { //uncompressed
            header = new MessageHeader(type, bodySize, false);
            body = new MessageBody(bodyData);
        }
        return new Message(header, body);
    }

    private Message(MessageHeader header, MessageBody body) {
        _header = header;
        _body = body;
    }

    public byte[] ToByteArr() {
        byte[] headerBytes = _header.ToByteArr();
        int bodySize = _body.Size;
        byte[] bodyData = _body.Data;
        byte[] messageData = new byte[MessageHeader.HEADER_SIZE + bodySize];

        Array.Copy(headerBytes, 0, messageData, 0, MessageHeader.HEADER_SIZE);//add header
        Array.Copy(bodyData, 0, messageData, MessageHeader.HEADER_SIZE, bodySize);//add body

        return messageData;
    }
}

