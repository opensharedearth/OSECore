using System;
using System.IO;
using System.Text;

namespace OSECore.IO
{

    /// <summary>   A string stream.  A facility for reading and writing text as if to a file.</summary>
    public class StringStream : Stream
    {
        private readonly MemoryStream _memory;
        /// <summary>   Constructor that takes a string has an argument.  The stream becomes an input stream that cannot be written to. </summary>
        ///
        /// <param name="text"> The sequence of changes that becomes the entire contents of the stream.</param>
        public StringStream(string text)
        {
            _memory = new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>   Default constructor. This constructs a read/write stream that is initialially of zero length. </summary>
        public StringStream()
        {
            _memory = new MemoryStream();
        }
        /// <summary>
        /// Constructor that takes an integer capacity as an argument.  The stream is read/write with an initial length of zero.
        /// </summary>
        ///
        /// <param name="capacity"> The initial capacity of the stream. The stream will be enlarged if the length of the stream becomes bigger
        ///                         than the initial capacity.</param>
        public StringStream(int capacity)
        {
            _memory = new MemoryStream(capacity);
        }
        /// <summary>
        /// Has no effect on string stream.
        /// </summary>
        public override void Flush()
        {
            _memory.Flush();
        }
        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and
        /// advances the position within the stream by the number of bytes read.
        /// </summary>
        ///
        /// <param name="buffer">   An array of bytes. When this method returns, the buffer contains the
        ///                         specified byte array with the values between offset and (offset +
        ///                         count - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">   The zero-based byte offset in buffer at which to begin storing the
        ///                         data read from the current stream. </param>
        /// <param name="count">    The maximum number of bytes to be read from the current stream. </param>
        ///
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes
        /// requested if that many bytes are not currently available, or zero (0) if the end of the
        /// stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return  _memory.Read(buffer, offset, count);
        }
        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        ///
        /// <param name="offset">   A byte offset relative to the origin parameter. </param>
        /// <param name="origin">   A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating
        ///                         the reference point used to obtain the new position. </param>
        ///
        /// <returns>   The new position within the current stream. </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _memory.Seek(offset, origin);
        }
        /// <summary>   When overridden in a derived class, sets the length of the current stream. </summary>
        ///
        /// <param name="value">    The desired length of the current stream in bytes. </param>
        public override void SetLength(long value)
        {
            _memory.SetLength(value);
        }
        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and
        /// advances the current position within this stream by the number of bytes written.
        /// </summary>
        ///
        /// <param name="buffer">   An array of bytes. This method copies count bytes from buffer to the
        ///                         current stream. </param>
        /// <param name="offset">   The zero-based byte offset in buffer at which to begin copying bytes
        ///                         to the current stream. </param>
        /// <param name="count">    The number of bytes to be written to the current stream. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _memory.Write(buffer, offset, count);
            return;
        }
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream
        /// supports reading.
        /// </summary>
        ///
        /// <value> true if the stream supports reading; otherwise, false. </value>
        public override bool CanRead => _memory.CanRead;
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream
        /// supports seeking.
        /// </summary>
        ///
        /// <value> true if the stream supports seeking; otherwise, false. </value>
        public override bool CanSeek => _memory.CanSeek;
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream
        /// supports writing.
        /// </summary>
        ///
        /// <value> true if the stream supports writing; otherwise, false. </value>
        public override bool CanWrite => _memory.CanWrite;
        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        ///
        /// <throwses cref="T:System.NotSupportedException">    A class derived from Stream does not
        ///                                                     support seeking. </throwses>
        /// <throwses cref="T:System.ObjectDisposedException">  Methods were called after the stream was
        ///                                                     closed. </throwses>
        ///
        /// <value> A long value representing the length of the stream in bytes. </value>
        public override long Length =>  _memory.Length;
        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        ///
        /// <throwses cref="T:System.IO.IOException">           An I/O error occurs. </throwses>
        /// <throwses cref="T:System.NotSupportedException">    The stream does not support seeking. </throwses>
        /// <throwses cref="T:System.ObjectDisposedException">  Methods were called after the stream was
        ///                                                     closed. </throwses>
        ///
        /// <value> The current position within the stream. </value>
        public override long Position
        {
            get => _memory.Position;
            set => _memory.Position = value;
        }
        /// <summary>   Returns the entire contents of the string stream. </summary>
        ///
        /// <returns>   The entire contents of the string stream. </returns>
        /// <remarks>The contents can be extracted even after the string stream has been closed.  Thus this method will work outside
        ///          of a using block.</remarks>
        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(_memory.GetBuffer(), 0, (int) _memory.Length);
        }
        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or
        /// returns -1 if at the end of the stream.
        /// </summary>
        ///
        /// <returns>   The unsigned byte cast to an Int32, or -1 if at the end of the stream. </returns>
        public override int ReadByte()
        {
            return _memory.ReadByte();
        }
        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the
        /// stream by one byte.
        /// </summary>
        ///
        /// <param name="value">    The byte to write to the stream. </param>
        public override void WriteByte(byte value)
        {
            _memory.WriteByte(value);
        }
    }
}