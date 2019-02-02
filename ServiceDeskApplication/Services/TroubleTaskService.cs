using ServiceDeskApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ServiceDeskApplication.Services
{
    public class TroubleTaskService
    {
        public TroubleTaskIndexViewModel GetIndexViewModel(TroubleTask task)
        {
            return new TroubleTaskIndexViewModel
            {
                Id = task.Id,
                IsFileAttached = task.IsFileAttached,
                AttachedFileName = task.IsFileAttached ? task.AttachedFileName : null,
                CreatorFullName = task.User?.FullName,
                GeneratedDate = task.GeneratedDate,
                Comment = task.Comment,
                Status = task.Status,
                Text = task.Text,
                AssignedFullName = task.Assigned?.FullName
            };
        }

        public Task<List<TroubleTaskIndexViewModel>> GetIndexViewModelListAsync(ApplicationDbContext db,
                                                                        string currentUserId = null)
        {
            IEnumerable<TroubleTask> tasks;

            if (currentUserId != null)
                tasks = db.TroubleTasks.Where(task => task.User.Id == currentUserId)
                                       .ToList();
            else
                tasks = db.TroubleTasks.ToList();

            var taskModels = tasks.Select(obj => new TroubleTaskIndexViewModel
            {
                Id = obj.Id,
                IsFileAttached = obj.IsFileAttached,
                AttachedFileName = obj.IsFileAttached ? obj.AttachedFileName : null,
                CreatorFullName = obj.User?.FullName,
                GeneratedDate = obj.GeneratedDate,
                Comment = obj.Comment,
                Status = obj.Status,
                Text = obj.Text,
                AssignedFullName = obj.Assigned?.FullName
            }).ToList();

            return Task.FromResult(taskModels);
        }

        public TroubleTaskAssignViewModel GetAssignViewModel(TroubleTask task)
        {
            return new TroubleTaskAssignViewModel
            {
                IsFileAttached = task.IsFileAttached,
                AttachedFileName = task.IsFileAttached ? task.AttachedFileName : null,
                CreatorFullName = task.User?.FullName,
                GeneratedDate = task.GeneratedDate,
                Text = task.Text
            };
        }

        public TroubleTaskEditViewModel GetEditViewModel(TroubleTask task)
        {
            return new TroubleTaskEditViewModel
            {
                CreatorFullName = task.User?.FullName,
                GeneratedDate = task.GeneratedDate,
                Comment = task.Comment,
                Status = task.Status,
                Text = task.Text,
                AssignedFullName = task.Assigned?.FullName
            };
        }

        public void ModifyTroubleTask(TroubleTaskEditViewModel viewModel, ref TroubleTask taskToModify)
        {
            taskToModify.Comment = viewModel.Comment;
            taskToModify.Status = viewModel.Status;
        }
    }
}