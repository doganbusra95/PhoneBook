namespace PhoneBookApi.Settings.Mail
{
    public class MailInfo
    {
        public string MailAddress { get; set; }

        public string MailServer { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string DisplayName { get; set; }

        public MailInfo()
        {

        }

        public MailInfo(string mailAddress, string mailServer, string userName, string password, int port, string displayName)
        {
            MailAddress = mailAddress;
            MailServer = mailServer;
            UserName = userName;
            Password = password;
            Port = port;
            DisplayName = displayName;
        }
    }
}
