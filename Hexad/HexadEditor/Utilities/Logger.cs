using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexadEditor.Utilities
{
    enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04,
    }

    class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; } 
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}: {Caller} ({Line})";
    }

    class Logger
    {
    }
}
