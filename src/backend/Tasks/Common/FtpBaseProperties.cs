namespace Tasks
{
    public class FtpBaseProperties : PropertiesBase
    {
        public string ConnectionString { get; set; }

        public string Folder { get; set; }

        public string FileNamePattern { get; set; }

        public string ViewHours { get; set; }
    }
}
