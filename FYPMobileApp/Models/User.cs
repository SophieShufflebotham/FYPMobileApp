using System;
using System.Collections.Generic;
using System.Text;

namespace FYPMobileApp.Models
{
    class User
    {
        public string userId { get; set; }
    }

    class UserLogin
    {
        public string username { get; set; }
        private string _password;

        public string password
        {
            get
            {
                return _password;
            }

            set
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                _password = System.Convert.ToBase64String(data);
            }
        }
    }
}
