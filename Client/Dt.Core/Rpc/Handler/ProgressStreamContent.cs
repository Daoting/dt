﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_bytesStep">本次发送字节数</param>
    /// <param name="p_bytesSent">共发送字节数</param>
    /// <param name="p_totalBytesToSend">总字节数</param>
    public delegate void ProgressDelegate(long p_bytesStep, long p_bytesSent, long p_totalBytesToSend);

    public partial class ProgressStreamContent : StreamContent
    {
        public ProgressStreamContent(Stream stream, CancellationToken token)
            : this(new ProgressStream(stream, token))
        {
        }

        public ProgressStreamContent(Stream stream, int bufferSize)
            : this(new ProgressStream(stream, CancellationToken.None), bufferSize)
        {
        }

        ProgressStreamContent(ProgressStream stream)
            : base(stream)
        {
            init(stream);
        }

        ProgressStreamContent(ProgressStream stream, int bufferSize)
            : base(stream, bufferSize)
        {
            init(stream);
        }

        void init(ProgressStream stream)
        {
            stream.ReadCallback = readBytes;

            Progress = delegate { };
        }

        void reset()
        {
            _totalBytes = 0L;
        }

        long _totalBytes;
        long _totalBytesExpected = -1;

        void readBytes(long bytes)
        {
            if (_totalBytesExpected == -1)
                _totalBytesExpected = Headers.ContentLength ?? -1;

            long computedLength;
            if (_totalBytesExpected == -1 && TryComputeLength(out computedLength))
                _totalBytesExpected = computedLength == 0 ? -1 : computedLength;

            // If less than zero still then change to -1
            _totalBytesExpected = Math.Max(-1, _totalBytesExpected);
            _totalBytes += bytes;

            Progress(bytes, _totalBytes, _totalBytesExpected);
        }

        ProgressDelegate _progress;
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                if (value == null) _progress = delegate { };
                else _progress = value;
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            reset();
            return base.SerializeToStreamAsync(stream, context);
        }

        protected override bool TryComputeLength(out long length)
        {
            var result = base.TryComputeLength(out length);
            _totalBytesExpected = length;
            return result;
        }

        partial class ProgressStream : Stream
        {
            CancellationToken token;

            public ProgressStream(Stream stream, CancellationToken token)
            {
                ParentStream = stream;
                this.token = token;

                ReadCallback = delegate { };
                WriteCallback = delegate { };
            }

            public Action<long> ReadCallback { get; set; }

            public Action<long> WriteCallback { get; set; }

            public Stream ParentStream { get; private set; }

            public override bool CanRead { get { return ParentStream.CanRead; } }

            public override bool CanSeek { get { return ParentStream.CanSeek; } }

            public override bool CanWrite { get { return ParentStream.CanWrite; } }

            public override bool CanTimeout { get { return ParentStream.CanTimeout; } }

            public override long Length { get { return ParentStream.Length; } }

            public override void Flush()
            {
                ParentStream.Flush();
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return ParentStream.FlushAsync(cancellationToken);
            }

            public override long Position
            {
                get { return ParentStream.Position; }
                set { ParentStream.Position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                token.ThrowIfCancellationRequested();

                var readCount = ParentStream.Read(buffer, offset, count);
                ReadCallback(readCount);
                return readCount;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                token.ThrowIfCancellationRequested();
                return ParentStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                token.ThrowIfCancellationRequested();
                ParentStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                token.ThrowIfCancellationRequested();
                ParentStream.Write(buffer, offset, count);
                WriteCallback(count);
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                token.ThrowIfCancellationRequested();
                var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);

                var readCount = await ParentStream.ReadAsync(buffer, offset, count, linked.Token);

                ReadCallback(readCount);
                return readCount;
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                token.ThrowIfCancellationRequested();

                var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
                var task = ParentStream.WriteAsync(buffer, offset, count, linked.Token);

                WriteCallback(count);
                return task;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    ParentStream.Dispose();
                }
            }
        }
    }
}