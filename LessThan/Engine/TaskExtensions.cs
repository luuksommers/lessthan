using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LessThan.Models;

namespace LessThan.Engine
{
    public static class TaskExtensions
    {
        public static object ToJson(this Task task)
        {
            return
                new
                    {
                        task.Id,
                        task.EstimatedTime,
                        task.Project,
                        task.IsDone,
                        task.TaskDescription,
                        task.AssignedTo,
                        task.DueDate,
                        task.TimeSpent
                    };
        }
    }
}