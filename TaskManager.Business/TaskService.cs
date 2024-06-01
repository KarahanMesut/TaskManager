using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Data;
using TaskManager.Data.TaskManager.Data;

namespace TaskManager.Business
{
    public class TaskService
    {
        private readonly TaskRepository taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public Task<List<ToDoTask>> GetTasksAsync()
        {
            return taskRepository.GetAllTasksAsync();
        }

        public Task AddTaskAsync(ToDoTask task)
        {
            return taskRepository.AddTaskAsync(task);
        }

        public Task UpdateTaskAsync(ToDoTask task)
        {
            return taskRepository.UpdateTaskAsync(task);
        }

        public Task DeleteTaskAsync(int taskId)
        {
            return taskRepository.DeleteTaskAsync(taskId);
        }

        public async Task<List<ToDoTask>> GetTasksByUserAsync(int userId)
        {
            return await taskRepository.GetTasksByUserAsync(userId);
        }

        public Task ShareTaskAsync(int taskId, int userId)
        {
            return taskRepository.ShareTaskAsync(taskId, userId);
        }

        public Task AddCommentAsync(int taskId, int userId, string commentText)
        {
            return taskRepository.AddCommentAsync(taskId, userId, commentText);
        }

        public Task<List<Comment>> GetCommentsAsync(int taskId)
        {
            return taskRepository.GetCommentsAsync(taskId);
        }
    }

}