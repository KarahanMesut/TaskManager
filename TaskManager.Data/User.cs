using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data
{
    public class User
    {
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string USER_SURNAME { get; set; }
        public string USER_ADRESS { get; set; }
        public string TC_IDENTITY { get; set; }
        public bool IS_STATUS { get; set; }
        public bool IS_ADMIN { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE_NUMBER { get; set; }
    }
}
