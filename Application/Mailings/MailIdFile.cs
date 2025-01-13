using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Application.Mailings
{
    public class MailIdFile
    {
        public string Content { get; }
        public string FileName { get; }
        public string FileType { get; }
        public Encoding Encoding { get; }

        public MailIdFile(string content, string fileName, string fileType, Encoding encoding)
        {
            Content = content;
            FileName = fileName;
            FileType = fileType;
            Encoding = encoding;
        }
    }
}
