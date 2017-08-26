using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MessageBody {

    private byte[] data;
    public byte[] Data { get { return data; } }

    public int Size { get { return (data == null) ? 0 : data.Length; }}
    public MessageBody(byte[] _data) {
        data = _data;
    }
}

