using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Data.TaskManager.Data;
using TaskManager.Presentation;

namespace TaskManager.Data
{
    public class TaskRepository
    {
        private readonly GenericRepository<ToDoTask> _genericRepository;
        private readonly GenericRepository<Comment> _commentRepository;

        public TaskRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Task_Manager_DB"].ConnectionString;
            _genericRepository = new GenericRepository<ToDoTask>(connectionString);
            _commentRepository = new GenericRepository<Comment>(connectionString);
        }

        public Task ShareTaskAsync(int taskId, int userId)
        {
            string query = "INSERT INTO TASK_SHARES (TASK_ID, SHARED_WITH_USER_ID) VALUES (@TaskID, @UserID)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskID", taskId),
                new SqlParameter("@UserID", userId)
            };
            return _genericRepository.AddAsync(query, parameters);
        }

        public Task AddCommentAsync(int taskId, int userId, string commentText)
        {
            string query = "INSERT INTO COMMENTS (TASK_ID, USER_ID, COMMENT_TEXT, COMMENT_DATE) VALUES (@TaskID, @UserID, @CommentText, @CommentDate)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskID", taskId),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@CommentText", commentText),
                new SqlParameter("@CommentDate", DateTime.Now)
            };
            return _genericRepository.AddAsync(query, parameters);
        }

        public Task<List<Comment>> GetCommentsAsync(int taskId)
        { 
          string query = @"
          SELECT COMMENTS.*, U.USER_NAME, U.USER_SURNAME 
          FROM COMMENTS
          LEFT JOIN USERS AS U ON COMMENTS.USER_ID = U.USER_ID
          WHERE TASK_ID = @TaskID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskID", taskId)
            };
            return _commentRepository.GetAllAsync(query, parameters);
        }

        public Task<List<ToDoTask>> GetAllTasksAsync()
        {
            if (GlobalVariables.IS_ADMIN)
            {
                string query = "SELECT * FROM TASKS";
                return _genericRepository.GetAllAsync(query);
            }
            else
            {
                string query = "SELECT * FROM TASKS WHERE LOGIN_ID = @UserID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", GlobalVariables.LOGIN_ID)
                };
                return _genericRepository.GetAllAsync(query, parameters);

            }



        }

        public Task AddTaskAsync(ToDoTask task)
        {
            string query = "INSERT INTO TASKS (TITLE, DESCRIPTION, IS_COMPLETED, DUE_DATE, REMINDER_TIME,PRIORITY,TAGS) VALUES (@Title, @Description, @IsCompleted,@DueDate, @ReminderTime,@Priority,@Tags)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Title", task.TITLE),
                new SqlParameter("@Description", task.DESCRIPTION),
                new SqlParameter("@IsCompleted", task.IS_COMPLETED),
                new SqlParameter("@DueDate", task.DUE_DATE),
                new SqlParameter("@ReminderTime", task.REMINDER_TIME),
                new SqlParameter("@Priority", task.PRIORITY),
                new SqlParameter("@Tags", task.TAGS)


            };
            return _genericRepository.AddAsync(query, parameters);
        }

        public Task UpdateTaskAsync(ToDoTask task)
        {
            string query = "UPDATE TASKS SET TITLE = @Title, DESCRIPTION = @Description, DUE_DATE = @DueDate, REMINDER_TIME = @ReminderTime, IS_COMPLETED = @IsCompleted, PRIORITY=@Priority, TAGS=@Tags WHERE TASK_ID = @TaskID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Title", task.TITLE),
                new SqlParameter("@Description", task.DESCRIPTION),
                new SqlParameter("@IsCompleted", task.IS_COMPLETED),
                new SqlParameter("@DueDate", task.DUE_DATE),
                new SqlParameter("@ReminderTime", task.REMINDER_TIME),
                new SqlParameter("@Priority", task.PRIORITY),
                new SqlParameter("@Tags", task.TAGS),
                new SqlParameter("@TaskID", task.TASK_ID)
            };
            return _genericRepository.UpdateAsync(query, parameters);
        }

        public Task DeleteTaskAsync(int taskId)
        {
            string query = "DELETE FROM TASKS WHERE TASK_ID = @TaskID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskID", taskId)
            };
            return _genericRepository.DeleteAsync(query, parameters);
        }
        public Task<List<ToDoTask>> GetTasksByUserAsync(int userId)
        {
            string query = "SELECT * FROM TASKS WHERE LOGIN_ID = @UserId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return _genericRepository.GetAllAsync(query, parameters);
        }
    }
}
