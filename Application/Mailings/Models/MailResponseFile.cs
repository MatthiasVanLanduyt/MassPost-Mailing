using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Application.Mailings.Models
{
    public class MailResponseFile
    {
      
            public byte[] Content { get; }
            public string FileName { get; }
            public string FileType { get; }

            public MailResponseFile(byte[] content, string fileName, string fileType)
            {
                Content = content;
                FileName = fileName;
                FileType = fileType;
            }
        
    }
}
