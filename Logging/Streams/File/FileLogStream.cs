using System;

namespace Compendium.Logging.Streams.File
{
    public class FileLogStream : LogStream
    {
        public FileLogStreamBuffer Buffer { get; private set; }
        public FileLogStreamMode Mode { get; }

        public string Path
        {
            get => File.Info.FullName;
            set => File = new IO.File(value);
        }

        public IO.File File { get; private set; }

        public FileLogStream(FileLogStreamMode mode, string path, int interval = -1)
        {
            if (mode != FileLogStreamMode.Append && interval <= 0)
                throw new InvalidOperationException($"Cannot start a file log stream buffer with an interval lower than 0.");

            if (mode != FileLogStreamMode.Append)
            {
                Buffer = new FileLogStreamBuffer(interval);
                Buffer.OnFlushing += OnBufferFlushing;
            }

            Mode = mode;
            Path = path;
        }

        public override void Write(LogMessage message)
        {
            if (Mode is FileLogStreamMode.Append)
            {
                File.Append($"{message.Time} [{message.Source} / {message.Type}] {message.Message}");
            }
            else
            {
                if (message.Type is LogType.Raw)
                    Buffer.Append(message.Message);
                else
                    Buffer.Append($"{message.Time} [{message.Source} / {message.Type}] {message.Message}");
            }
        }

        public override void DisposeInternal()
        {
            Buffer.OnFlushing -= OnBufferFlushing;
            Buffer.Dispose();
            Buffer = null;

            Path = null;

            base.DisposeInternal();
        }

        private void OnBufferFlushing(string[] lines)
        {
            File.Append(lines);
        }
    }
}
