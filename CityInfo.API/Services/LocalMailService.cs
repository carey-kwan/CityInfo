using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly IConfiguration _configuration;
        //this is an example of creating a custom service that will be interjected into the application
        //private string _mailTo = "admin@mycompany.com";
        //private string _mailFrom = "noreply@mycompany.com";


        public LocalMailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
                
        public void Send(string subject, string message)
        {
            //send mail - output to debug window
            //Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo} with LocalMailService.");
            Debug.WriteLine($"Mail from {_configuration["mailSettings:mailFromAddress"]} to {_configuration["mailSettings:mailToAddress"]} with LocalMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
