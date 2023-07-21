using System;
using System.IO;

namespace NEP.MonoDirector.Data
{
    public interface IBinaryData
    {
        /// <summary>
        /// Converts the object into a binary form for serialization
        /// GetBinaryID() is called before and placed before this object is wrote
        /// </summary>
        /// <returns>A byte array representing the object</returns>
        byte[] ToBinary();
        
        /// <summary>
        /// Converts the provided byte array into an object
        /// </summary>
        /// <param name="stream">The stream in which bytes are pulled from</param>
        void FromBinary(Stream stream);

        /// <summary>
        /// Provides a unique binary ID to the serializer
        /// </summary>
        /// <returns>A unique binary ID</returns>
        uint GetBinaryID();
    }
}