using Opportunity.AssLoader.Serializer;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Opportunity.AssLoader
{
    internal ref struct UUEncoder
    {
        private ReadOnlySpan<byte> data;
        private readonly TextWriter writer;
        private int currentline;

        public static void Encode(ReadOnlySpan<byte> data, TextWriter writer)
        {
            var encoder = new UUEncoder(data, writer);
            encoder.encode();
        }

        private UUEncoder(ReadOnlySpan<byte> data, TextWriter writer)
        {
            this.data = data;
            this.writer = writer;
            this.currentline = 0;
        }

        private void encode()
        {
            if (this.data.IsEmpty)
                return;

            switch (this.data.Length % 3)
            {
            case 0:
                writeHeading(this.data);
                break;
            case 1:
            {
                writeHeading(this.data.Slice(0, this.data.Length - 1));
                var remdata = this.data[this.data.Length - 1];
                var hi = remdata >> 2;
                var lo = (remdata & 0b11) << 4;
                writeChar(hi);
                writeChar(lo);
                break;
            }
            case 2:
            {
                writeHeading(this.data.Slice(0, this.data.Length - 2));
                var remdata0 = this.data[this.data.Length - 2];
                var remdata1 = this.data[this.data.Length - 1];

                var hi = remdata0 >> 2;
                var mi = ((remdata0 & 0b11) << 4) | (remdata1 >> 4);
                var lo = (remdata1 & 0b1111) << 2;
                writeChar(hi);
                writeChar(mi);
                writeChar(lo);
                break;
            }
            }

            if (this.currentline != 0)
                this.writer.WriteLine();
        }

        /// <summary>
        /// data.Length must be time of 3.
        /// </summary>
        private void writeHeading(ReadOnlySpan<byte> data)
        {
            for (var i = 0; i < data.Length; i += 3)
            {
                writePack(data.Slice(i, 3));
            }
        }

        /// <summary>
        /// data.Length must be 3.
        /// </summary>
        private void writePack(ReadOnlySpan<byte> data)
        {
            var num1 = data[0] >> 2;
            var num2 = ((data[0] & 0b11) << 4) | (data[1] >> 4);
            var num3 = ((data[1] & 0b1111) << 2) | (data[2] >> 6);
            var num4 = data[2] & 0b111111;

            writeChar(num1);
            writeChar(num2);
            writeChar(num3);
            writeChar(num4);
        }

        private void writeChar(int uuebyte)
        {
            var ch = (char)(uuebyte + 33);
            this.writer.Write(ch);
            this.currentline++;
            if (this.currentline == 80)
            {
                this.writer.WriteLine();
                this.currentline = 0;
            }
        }
    }

    internal class UUDecoder
    {
        private readonly MemoryStream buf = new MemoryStream();

        public void ReadLine(ReadOnlySpan<char> line)
        {
            if (line.Length % 4 == 0)
            {
                readHeading(line);
                return;
            }
            else
            {
                var len = line.Length / 4 * 4;
                readHeading(line.Slice(0, len));
                line = line.Slice(len);
            }
            var loss = 4 - line.Length;
            Span<char> lastpac = stackalloc char[4];
            line.CopyTo(lastpac);
            readPack(lastpac);
            this.buf.Position -= loss;
        }

        /// <summary>
        /// data.Length must be time of 4.
        /// </summary>
        private void readHeading(ReadOnlySpan<char> data)
        {
            for (var i = 0; i < data.Length; i += 4)
            {
                readPack(data.Slice(i, 4));
            }
        }

        /// <summary>
        /// data.Length must be 4.
        /// </summary>
        private void readPack(ReadOnlySpan<char> data)
        {
            var num1 = readChar(data[0]);
            var num2 = readChar(data[1]);
            var num3 = readChar(data[2]);
            var num4 = readChar(data[3]);

            this.buf.WriteByte((byte)((num1 << 2) | (num2 >> 4)));
            this.buf.WriteByte((byte)((num2 << 4) | (num3 >> 2)));
            this.buf.WriteByte((byte)((num3 << 6) | (num4 >> 0)));
        }

        private static int readChar(char uuechar)
        {
            return uuechar - 33;
        }

        public byte[] ToArray()
        {
            var arr = new byte[this.buf.Position];
            this.buf.Position = 0;
            this.buf.Read(arr, 0, arr.Length);
            return arr;
        }
    }

    /// <summary>
    /// An embedded file in the subtitle.
    /// </summary>
    /// <remarks>
    /// Serialize and deserialize of this type is spcialized,
    /// <see cref="EntryFieldAttribute"/> do not work on this type and it children's fields.
    /// </remarks>
    [DebuggerDisplay(@"{EntryName,nq}: {FileName,nq}")]
    public class EmbeddedFile : Entry
    {
        /// <summary>
        /// Create new instance of <see cref="EmbeddedFile"/>.
        /// </summary>
        /// <param name="filename">Name of the file, with extension.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> conatins invalid char.</exception>
        public EmbeddedFile(string filename)
        {
            if (filename.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(filename));
            this.FileName = filename;
        }

        /// <summary>
        /// "filename", overrided value should not contain captials.
        /// </summary>
        protected override string EntryName => "filename";

        private string filename;
        /// <summary>
        /// Name of the file, with extension.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> conatins invalid char.</exception>
        public string FileName
        {
            get => this.filename;
            set
            {
                if (value.IsNullOrWhiteSpace())
                    throw new ArgumentNullException(nameof(value));
                var ici = value.IndexOfAny(Path.GetInvalidFileNameChars());
                if (ici >= 0)
                    throw new ArgumentException($"Invalid char {value[ici]}.");
                this.filename = value;
            }
        }

        /// <summary>
        /// Content of file.
        /// </summary>
        public byte[] Data { get; set; }

        internal void Serialize(TextWriter writer, ISerializeInfo serializeInfo)
        {
            writer.Write(this.EntryName);
            writer.Write(": ");
            writer.WriteLine(this.filename);
            var data = Data;
            if (data is null || data.Length == 0)
                return;
            UUEncoder.Encode(data, writer);
        }
    }

    /// <summary>
    /// An embedded font in the subtitle.
    /// </summary>
    public sealed class EmbeddedFont : EmbeddedFile
    {
        /// <summary>
        /// Create new instance of <see cref="EmbeddedFont"/>.
        /// </summary>
        /// <param name="filename">Name of the file, with extension.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> conatins invalid char.</exception>
        public EmbeddedFont(string filename) : base(filename) { }

        /// <summary>
        /// "fontname"
        /// </summary>
        protected override string EntryName => "fontname";
    }

    /// <summary>
    /// An embedded picture in the subtitle.
    /// </summary>
    public sealed class EmbeddedGraphic : EmbeddedFile
    {
        /// <summary>
        /// Create new instance of <see cref="EmbeddedGraphic"/>.
        /// </summary>
        /// <param name="filename">Name of the file, with extension.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> conatins invalid char.</exception>
        public EmbeddedGraphic(string filename) : base(filename) { }
    }
}
