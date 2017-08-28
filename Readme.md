# Protobuffers over network for Unity3D
This project lets you quickly and easily send protobuffer objects over the
network between 2 apps. The protobuffers will get automatically compressed if
they are larger than 1mb in order to save space when sending them through the
network.

# How it works
The way the project works is with a messaging system. Messages get sent
through sockets. On one side one application has a listening socket and on
the other side the application sends messages to it.

*Messages* are the basic way how protobuffers are sent, a message always
contains a Protobuffer and a *Type* that identifies the protobuffer object you
are sending.

# Project Structure
In the unity project, There are 2 scenes. **SimpleClient** and
**SimpleServer**.

All the information you need to run the project is contained in these 2
scenes, specifically in 2 files called *ClientTest.cs* (for sending messages)
and *ServerTest.cs* for receiving messages.

## SimpleClient scene.
*SimpleClient* is an empty scene that contains a *ClientTest.cs* Behaviour
added to the main camera.

This scene acts as a client and sends messages to the other application
called *SimpleServer* which acts as the listening socket.

### ClientTest.cs
In order to send a Protobuf object to another application, you need to open a
socket and send a Protobuf object encapsulated in a Message object. convert it
to an array of bytes and send it.

The *ClientTest.cs* class contains precissely that function already for you to
send messages and its called:

*void SendMessage(Message m);*

In ClientTest.cs you will find the UI for setting up values for the
SampleObject protobuf object and the address and port of the app you want to
send the values to.

## SimpleServer Scene.
*SimpleServer* is an empty scene that contains a *ServerTest.cs* Behaviour
attached to the main camera.

This scene acts as a server that is continuously listening on a port for
incoming messages from any client. It processes the messages, rebuilds them
and print them.

### ServerTest.cs
In order to receive a protobuf object, a *SocketProtobufListener* object
needs to be created. This object receives an IP, a port where the socket
is going to be listening for incoming messages and a *ConcurrentQueue* for
*ProtobufMessages* that are stored in the Queue as they come.

The *ServerTest.cs* creates a SocketProtobufListener object, binds a socket
automatically to the current IP used and on a separate thread a socket starts
to listen for incoming messages, these messages are stored in the
*messaQueue* declared in the class.

When a message gets added to the queue, in the Update function as soon as a
message arrives, the protobuf message gets read and depending on the type it
has, it gets casted and then printed in the UI with the
*PrintSampleObjectFields(SampleObject s)* function.


# How to add a Protobuf Object and transmit it over the network
Here I will walk you through on how to send an object and receive it on the
other end. This app doesnt check for errors in connections nor anything
else. This is left for you, the reader.

First things first. We need to create a Protobuf object. I will not cover the
specific syntax on how protobuffer objects work, for more reference just
check it out [here](https://developers.google.com/protocol-buffers/docs/overview).

In order to compile our SampleObject you need to have **protoc** installed
and compile the protobuffer for C#. After that you are pretty much done.

Our *SampleObject.proto* looks like this:

>package Protobuf;
>
>message SampleObject {
>    int32  type         = 1;
>    string objectName   = 2;
>    string sampleString = 3;
>    int32 sampleInt     = 4;
>    float sampleFloat   = 5;
>}

It contains a *type* which is **necessary** for all your objects you want to
transmit. 2 sample strings, a float and an integer.

After compiling the protocol buffer to C# we need to tell the system with a
unique identifier the type of the object. This is added in  *ProtobufMessagesTypes.cs*

>//ProtobufMessageTypes.cs
>public static class ProtobufMessageTypes {
>    public const int SAMPLE_OBJECT = 1;//can have any value, just make sure is unique
>}

Then, we create a function for creating this object message in
*MessageCreator.cs*. Here we define a function that creates an object with
the parameters and returns a message (See MessageCreator.cs for the
implementation details).

>//MessageCreator.cs
>public static Message CreateSampleObjectMessage(string objName, string sampleStr, int sampleInt, float sampleFloat)

In order to receive and undertand the message, we have to convert the message
to a ProtobufMessage, this is done in *DeserializeByType(int type, MemoryStream memStream)*

>//MessageDeserializer.cs
>private static ProtobufMessage DeserializeByType(int type, MemoryStream memStream) {
>    object protobuf = null;
>    switch (type) {
>        case ProtobufMessageTypes.SAMPLE_OBJECT:
>            protobuf = Protobuf.SampleObject.Parser.ParseFrom(memStream);
>            break;
>        default:
>            return null;
>    }
>    return new ProtobufMessage(protobuf, type);
>}

Finally, in the *Update()* function in *ServerTest.cs* we can cast our
ProtobufMessage.cs with the specific type to our object.

>// ServerTest.cs
>void Update() {
>    if(messageQueue.Count > 0) {//when there's a protobuf message in the queue, we print it
>        ProtobufMessage pm;
>        messageQueue.TryDequeue(out pm);
>
>        switch(pm.MessageType) {
>            case ProtobufMessageTypes.SAMPLE_OBJECT:
>                SampleObject sampleObject = (SampleObject) pm.Protobuf;
>                //Do something with our object
>                break;
>            }
>        }
>    }
>}
