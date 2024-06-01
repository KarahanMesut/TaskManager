using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data
{
    public class ToDoTask
    {
        public int TASK_ID { get; set; }
        public string TITLE { get; set; }
        public string DESCRIPTION { get; set; }
        public bool IS_COMPLETED { get; set; }
        public DateTime DUE_DATE { get; set; }
        public int REMINDER_TIME { get; set; }
        public int PRIORITY { get; set; }
        public string TAGS { get; set; }

        public string FormattedDueDate
        {
            get { return DUE_DATE.ToString("dd.MM.yyyy"); }
        }

    }
}
