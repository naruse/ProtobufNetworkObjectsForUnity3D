using Google.Protobuf;
using Protobuf;

public static class MessageCreator {

    public static Message CreateSampleObjectMessage(string objName, string sampleStr, int sampleInt, float sampleFloat) {
        SampleObject sampleObj = new SampleObject();
        sampleObj.Type = ProtobufMessageTypes.SAMPLE_OBJECT;
        sampleObj.ObjectName = objName;
        sampleObj.SampleString = sampleStr;
        sampleObj.SampleInt = sampleInt;
        sampleObj.SampleFloat = sampleFloat;

        byte[] messageBody = sampleObj.ToByteArray();

        return Message.Create(sampleObj.Type, messageBody);
    }
}