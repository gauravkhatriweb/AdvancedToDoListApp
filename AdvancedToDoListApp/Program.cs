using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvancedToDoListApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // List to store tasks
            List<Task> tasks = new List<Task>();
            // Load tasks from file if exists
            LoadTasksFromFile(tasks);

            bool continueApp = true;

            while (continueApp)
            {
                Console.Clear();
                Console.WriteLine("To-Do List Application");
                Console.WriteLine("======================");
                Console.WriteLine("1. View Tasks");
                Console.WriteLine("2. Add Task");
                Console.WriteLine("3. Edit Task");
                Console.WriteLine("4. Complete Task");
                Console.WriteLine("5. Delete Task");
                Console.WriteLine("6. Save Tasks");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewTasks(tasks);
                        break;
                    case "2":
                        AddTask(tasks);
                        break;
                    case "3":
                        EditTask(tasks);
                        break;
                    case "4":
                        CompleteTask(tasks);
                        break;
                    case "5":
                        DeleteTask(tasks);
                        break;
                    case "6":
                        SaveTasksToFile(tasks);
                        break;
                    case "7":
                        continueApp = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Please try again.");
                        break;
                }
            }
        }

        static void ViewTasks(List<Task> tasks)
        {
            Console.Clear();
            Console.WriteLine("To-Do List");
            Console.WriteLine("==========");

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available.");
            }
            else
            {
                // Sort tasks by completion status and due date
                var sortedTasks = tasks.OrderBy(t => t.IsComplete).ThenBy(t => t.DueDate);

                foreach (var task in sortedTasks)
                {
                    Console.WriteLine($"{task.Id}. [{(task.IsComplete ? "X" : " ")}] {task.Description} {(task.DueDate.HasValue ? $"(Due: {task.DueDate.Value.ToShortDateString()})" : "")}");
                }
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void AddTask(List<Task> tasks)
        {
            Console.Clear();
            Console.WriteLine("Add a New Task");
            Console.WriteLine("==============");
            Console.Write("Enter task description: ");
            string description = Console.ReadLine();

            DateTime? dueDate = null;
            Console.Write("Enter due date (optional, format: MM/dd/yyyy): ");
            string dueDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dueDateInput) && DateTime.TryParse(dueDateInput, out DateTime parsedDueDate))
            {
                dueDate = parsedDueDate;
            }

            tasks.Add(new Task { Id = GenerateTaskId(tasks), Description = description, IsComplete = false, DueDate = dueDate });
            Console.WriteLine("Task added successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void EditTask(List<Task> tasks)
        {
            Console.Clear();
            Console.WriteLine("Edit a Task");
            Console.WriteLine("===========");
            ViewTasks(tasks);
            Console.Write("Enter the ID of the task to edit: ");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                Task taskToEdit = tasks.FirstOrDefault(t => t.Id == taskId);
                if (taskToEdit != null)
                {
                    Console.WriteLine($"Editing Task: {taskToEdit.Description}");
                    Console.Write("Enter new description (press enter to keep current): ");
                    string newDescription = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newDescription))
                    {
                        taskToEdit.Description = newDescription;
                    }

                    Console.Write("Enter new due date (press enter to keep current, format: MM/dd/yyyy): ");
                    string newDueDateInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newDueDateInput) && DateTime.TryParse(newDueDateInput, out DateTime parsedDueDate))
                    {
                        taskToEdit.DueDate = parsedDueDate;
                    }

                    Console.WriteLine("Task updated successfully!");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid task ID.");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void CompleteTask(List<Task> tasks)
        {
            Console.Clear();
            Console.WriteLine("Complete a Task");
            Console.WriteLine("===============");
            ViewTasks(tasks);
            Console.Write("Enter the ID of the task to complete: ");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                Task taskToComplete = tasks.FirstOrDefault(t => t.Id == taskId);
                if (taskToComplete != null)
                {
                    taskToComplete.IsComplete = true;
                    Console.WriteLine("Task marked as complete!");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid task ID.");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void DeleteTask(List<Task> tasks)
        {
            Console.Clear();
            Console.WriteLine("Delete a Task");
            Console.WriteLine("==============");
            ViewTasks(tasks);
            Console.Write("Enter the ID of the task to delete: ");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                Task taskToDelete = tasks.FirstOrDefault(t => t.Id == taskId);
                if (taskToDelete != null)
                {
                    tasks.Remove(taskToDelete);
                    Console.WriteLine("Task deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid task ID.");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void SaveTasksToFile(List<Task> tasks)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("tasks.txt"))
                {
                    foreach (var task in tasks)
                    {
                        writer.WriteLine($"{task.Id},{task.Description},{task.IsComplete},{(task.DueDate.HasValue ? task.DueDate.Value.ToString("MM/dd/yyyy") : "")}");
                    }
                }

                Console.WriteLine("Tasks saved to file successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void LoadTasksFromFile(List<Task> tasks)
        {
            try
            {
                if (File.Exists("tasks.txt"))
                {
                    using (StreamReader reader = new StreamReader("tasks.txt"))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length >= 3)
                            {
                                int id = int.Parse(parts[0]);
                                string description = parts[1];
                                bool isComplete = bool.Parse(parts[2]);
                                DateTime? dueDate = null;
                                if (parts.Length >= 4 && !string.IsNullOrWhiteSpace(parts[3]))
                                {
                                    dueDate = DateTime.ParseExact(parts[3], "MM/dd/yyyy", null);
                                }
                                tasks.Add(new Task { Id = id, Description = description, IsComplete = isComplete, DueDate = dueDate });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }

        static int GenerateTaskId(List<Task> tasks)
        {
            return tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
        }

        class Task
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public bool IsComplete { get; set; }
            public DateTime? DueDate { get; set; }
        }
    }
}
