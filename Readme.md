# Protobuffers for Unity3D
This project lets you quickly and easily send [protobuf](https://github.com/google/protobuf) objects between applications. The protobuffers will get automatically compressed if they are larger than 1MB in order to save space when sending them.
network.

# How it works
A socket will be opened between the applications to serve as a messaging system for the object transmission. On one side an application has a listening socket, on the other one the application sends messages to it.

**Messages** are the basic way how protobuffers are sent, a message always
contains a Protobuffer and a **Type** that identifies the protobuffer object you
are sending.

# Project Structure
There are two scenes in the Unity project: **SimpleClient** and **SimpleServer**. All the information you need to run the project is contained in these 2 scenes, specifically in 2 files called `ClientTest.cs` (for sending messages)
and `ServerTest.cs` for receiving messages.

## SimpleClient scene.
SimpleClient is an empty scene that contains a `ClientTest.cs` which is added to the main camera.

This scene acts as a client and sends messages to the other application (SimpleServer) which acts as the listening socket.

### ClientTest.cs
In order to send Protobuf objects you need to encapsulate them inside a **Message** object that will be sent through the communications socket. The messages will be converted into byte arrays that will be send over the network.

The `ClientTest.cs` class already contains a function that does this for you:

`void SendMessage(Message m);`

In `ClientTest.cs` you will find the UI for setting up values for the
SampleObject protobuf object and the address and port of the application that will be receiving the data.

## SimpleServer Scene.
*SimpleServer* is an empty scene that contains a *ServerTest.cs* Behaviour
attached to the main camera.

This scene acts as a server that is continuously listening on a port for
incoming messages from any client. It processes the messages, rebuilds them
and print them.

### ServerTest.cs
In order to receive a protobuf object, a `SocketProtobufListener` object
needs to be created. This object receives a host and a port where the socket
is going to be listening for incoming messages. A *ConcurrentQueue* is used for storing incoming *ProtobufMessages*.

`ServerTest.cs` creates a `SocketProtobufListener` object, binds a socket
and starts listening for incoming messages on a on a separate thread. These messages are stored in the
*messageQueue* declared in the class.

When a message gets added to the queue (in the `Update` function as soon as a
message arrives), the protobuf message gets read and depending on the type it
has, it gets casted and then printed in the UI with the
`PrintSampleObjectFields(SampleObject s)` function.


# How to add a Protobuf Object and transmit it over the network
Here I will walk you through on how to send an object and receive it on the
other end. This app doesn't check for errors in connections nor anything
else. This is left as an exercise for the reader :)

I won't be convering how protobufs work or their syntax, for more information you can find and overview [here](https://developers.google.com/protocol-buffers/docs/overview).

In order to compile our SampleObject you need to have **protoc** installed
and compile the protobuffer for C#. After that you are pretty much done.

Our *SampleObject.proto* looks like this:

```csharp
package Protobuf;

message SampleObject {
    int32  type         = 1;
    string objectName   = 2;
    string sampleString = 3;
    int32 sampleInt     = 4;
    float sampleFloat   = 5;
}
```

It contains a *type* (**required** for all your objects you want to
transmit), two sample strings, a float and an integer.

After compiling the protocol buffer to C# we need to add a unique identifier to the type of the object. This is added in  `ProtobufMessagesTypes.cs`

```csharp
//ProtobufMessageTypes.cs
public static class ProtobufMessageTypes {
    public const int SAMPLE_OBJECT = 1;      //  can have any value, just make sure is unique
}
```

Then, we create a function for creating this object message in
`MessageCreator.cs`. Here we define a function that creates an object with
the parameters and returns a message (See `MessageCreator.cs` for the
implementation details).

```csharp
//MessageCreator.cs
public static Message CreateSampleObjectMessage(string objName, string sampleStr, int sampleInt, float sampleFloat)
```

In order to receive and understand the message we have to convert it to a `ProtobufMessage`. This is done by `DeserializeByType(int type, MemoryStream memStream)`:

```csharp
//MessageDeserializer.cs
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
```

Finally, in the `Update()` function in *ServerTest.cs* we can cast our
ProtobufMessage with the specific type to our object:

```csharp
// ServerTest.cs
void Update() {
    if(messageQueue.Count > 0) {  //  when there's a protobuf message in the queue, we print it
        ProtobufMessage pm;
        messageQueue.TryDequeue(out pm);

        switch(pm.MessageType) {
            case ProtobufMessageTypes.SAMPLE_OBJECT:
                SampleObject sampleObject = (SampleObject) pm.Protobuf;
                //Do something with our object
                PrintSampleObjectFields(sampleObject);
                break;
            }
        }
    }
}
```
