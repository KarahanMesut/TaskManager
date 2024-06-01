using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data
{
    namespace TaskManager.Data
    {
        public class Comment
        {
            public int COMMENT_ID { get; set; }
            public int TASK_ID { get; set; }
            public int USER_ID { get; set; }
            public string COMMENT_TEXT { get; set; }
            public string USER_NAME { get; set; }

            public string USER_SURNAME { get; set; }

            public DateTime COMMENT_DATE { get; set; }
        }
    }
}
