namespace DataAccess.Input
{
    using System;
    using System.IO;

    public class FileInput : InputBase
    {
        private FileInfo fileInfo;

        public FileInput(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public FileInput(string fileName)
        {
            this.fileInfo = new FileInfo(fileName);
        }

        public override void LoadTasks()
        {
            throw new NotImplementedException("Figure out how TeamDashboard stores files and read them into memory!");
        }
    }
}