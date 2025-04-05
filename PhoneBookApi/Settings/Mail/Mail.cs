namespace PhoneBookApi.Settings.Mail
{
    public class Mail
    {
        public string PostAddress { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string FileName { get; set; }

        public byte[] File { get; set; }

        public Mail()
        {
            File = new byte[0];

            FileName = string.Empty;
        }
    }
}
