using System.Buffers.Binary;
using System.Collections;

var memory = new byte[1];


BitPacker.PackIntoBuffer(new[]
{
    from(new [] { true, false, true }),
}, memory);

var bits = new BitArray(memory);

var s = 1;

BitDescriptor from(bool[] value)
{
    var bitArray = new BitArray(value); 

    var internalBuffer = new byte[1];

    bitArray.CopyTo(internalBuffer, 0);

    return new BitDescriptor(internalBuffer[0],value.Length);
}

record BitDescriptor(int Value, int Length);


class BitPacker
{
    public static void PackIntoBuffer(BitDescriptor[] bitDescriptors, byte[] buffer)
    {
        int index = 0;

        int bitIndex = 7;

        foreach(var bitDescriptor in bitDescriptors)
        {
            var value = bitDescriptor.Value;

            var vLength = bitDescriptor.Length;

            while(vLength > 0)
            {
                if (index >= buffer.Length)
                    return;

                int bitsToPack = bitIndex + 1;

                if (vLength <= bitsToPack)
                {
                    var mask = (1 << vLength) - 1;

                    buffer[index] |= (byte)((value & mask) << (bitsToPack - vLength));

                    bitIndex -= vLength;

                    if (bitIndex == -1)
                    {
                        bitIndex = 7;
                        index++;
                    }

                    vLength = 0;
                }
                else
                {
                    int mask = ((1 << bitsToPack) - 1) << (vLength - bitsToPack);

                    buffer[index] |= (byte)((value & mask) >>> (vLength - bitsToPack));

                    bitIndex = 7;
                    index++;
                    vLength -= bitsToPack;
                }
            }                
        }
    }
}