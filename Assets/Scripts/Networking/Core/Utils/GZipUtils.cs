using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public static class GZipUtils {

    public static byte[] Compress(byte[] data) {
        using (MemoryStream outStream = new MemoryStream()) {
            using (GZipStream gzipStream = new GZipStream(outStream, CompressionMode.Compress))
            using (MemoryStream srcStream = new MemoryStream(data))
                srcStream.CopyTo(gzipStream);
            return outStream.ToArray();
        }
    }

    public static byte[] Decompress(byte[] compressed) {
        using (MemoryStream inStream = new MemoryStream(compressed))
        using (GZipStream gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
        using (MemoryStream outStream = new MemoryStream()) {//uncompressed stream
            gzipStream.CopyTo(outStream);
            return outStream.ToArray();
        }
    }

    public static void CopyTo(this Stream input, Stream output) {
        byte[] buffer = new byte[4 * 1024];
        int bytesRead;

        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0) {
            output.Write(buffer, 0, bytesRead);
        }
    }
    /*
    public static byte[] DecompressNew(byte[] compressed) {
        byte[] buffer = new byte[4096];
        using (MemoryStream ms = new MemoryStream(compressed))
        using (GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
        using (MemoryStream uncompressed = new MemoryStream())
        {
            for (int r = -1; r != 0; r = gzs.Read(buffer, 0, buffer.Length))
                if (r > 0) uncompressed.Write(buffer, 0, r);
            return uncompressed.ToArray();
        }
    }

    //before .Net4
    public static byte[] ReadFully(Stream input) {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }*/
}
