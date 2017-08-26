/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
 */

public class ProtobufMessage {
    public object Protobuf { get; set; }
    public int MessageType { get; set; }

    public ProtobufMessage(object _protobuf, int _messageType) {
        Protobuf = _protobuf;
        MessageType = _messageType;
    }
}
