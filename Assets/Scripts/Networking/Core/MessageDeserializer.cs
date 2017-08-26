/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MessageDeserializer {

    public static ProtobufMessage DeserializeMessage(Message message) {
        MessageHeader header = message.Header;
        MessageBody body = message.Body;

        int type = header.MessageType;
        byte[] bodyData = body.Data;

        MemoryStream memStream = new MemoryStream(bodyData);

        return DeserializeByType(type, memStream);
    }

    private static ProtobufMessage DeserializeByType(int type, MemoryStream memStream) {
        object protobuf = null;
        switch (type) {
            case ProtobufMessageTypes.SAMPLE_OBJECT:
                protobuf = Protobuf.SampleObject.Parser.ParseFrom(memStream);
                break;
            default:
                return null;
        }
        return new ProtobufMessage(protobuf, type);
    }
}