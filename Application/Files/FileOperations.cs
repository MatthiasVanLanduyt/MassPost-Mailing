using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Application.Mailings;

namespace Validator.Application.Files
{
    public class FileOperations
    {
        public FileInfo SaveFile(MailIdFile file, string directory)
        {
            string fullPath = Path.Combine(directory, file.FileName);
            File.WriteAllText(fullPath, file.Content, file.Encoding);
            return new FileInfo(fullPath);
        }

        public void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                };
                process.Start();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not open file: {filePath}", ex);
            }
        }
    }
}
