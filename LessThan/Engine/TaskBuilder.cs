using System;
using System.Globalization;
using System.Linq;
using LessThan.Models;

namespace LessThan.Engine
{
    public class TaskBuilder
    {
        public static string[] KeyTags = new[]{"#", "^", "@", "!", "="};

        public static Task CreateTask(string input)
        {
            // task = raw task. Try to parse #, ^, =, @, and !
            var splitted = input.Split(' ');
            var project = "";
            DateTime? rawdeadline = null;
            TimeSpan? rawestimate = null;
            var assignedto = "";

            foreach (var word in splitted)
            {
                if (word.StartsWith("#"))
                {
                    project = word.Substring(1);
                }
                else if (word.StartsWith("^"))
                {
                    DateTime tmpDate;
                    if(DateTime.TryParseExact(word.Substring(1), "yyyy-MM-dd", null, DateTimeStyles.None, out tmpDate))
                    {
                        rawdeadline = tmpDate;
                    }
                }
                else if (word.StartsWith("="))
                {
                    rawestimate = GetEstimate(word.Substring(1));
                }
                else if (word.StartsWith("@"))
                {
                    assignedto = word.Substring(1);
                }
            }

            var subject = string.Join(" ", splitted.Where(x => !KeyTags.Any(x.StartsWith)));

            return new Task { TaskDescription = subject, EstimatedTime = rawestimate, Project = project, AssignedTo = assignedto, DueDate = rawdeadline };
        }

        private static TimeSpan GetEstimate(string word)
        {
            var estimatedMinutes = 0;
            var estimatedHours = 0;
            if (word.EndsWith("m"))
            {
                estimatedMinutes = Convert.ToInt32(word.Substring(0, word.Length - 1));
            }
            else if (word.EndsWith("h"))
            {
                if (word.Contains("."))
                {
                    estimatedHours = Convert.ToInt32(word.Substring(0, word.IndexOf('.')));
                    word = word.Remove(word.Length - 1);

                    estimatedMinutes = Convert.ToInt32(Math.Truncate(60 * (Convert.ToDecimal("0." + word.Substring(word.IndexOf('.') + 1), new CultureInfo("en-US")))));
                }
                else
                {
                    estimatedHours = Convert.ToInt32(word.Substring(0, word.Length - 1));
                }
            }
            else
            {
                estimatedHours = Convert.ToInt32(word);
            }
            return new TimeSpan(0, estimatedHours, estimatedMinutes);
        }
    }
}