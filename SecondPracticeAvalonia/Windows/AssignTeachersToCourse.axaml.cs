using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AssignTeachersToCourse : Window
    {
        private Course currentCourse;
        private AppDbContext dbContext;

        public AssignTeachersToCourse(Course course)
        {
            InitializeComponent();
            currentCourse = course;
            dbContext = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CourseTitleText.Text = currentCourse.Title;

                // Load all teachers (users with teacher role)
                var teacherRole = dbContext.UserRoles.FirstOrDefault(r => r.Name.ToLower() == "teacher");
                if (teacherRole != null)
                {
                    var teachers = dbContext.Users
                        .Where(u => u.RoleId == teacherRole.Id && (u.IsActive ?? true) && !(u.IsBlocked ?? false))
                        .OrderBy(u => u.Email)
                        .ToList();
                    TeachersListBox.ItemsSource = teachers;
                }

                // Load assigned teachers for this course
                var assignedTeachers = dbContext.CourseTeachers
                    .Include(ct => ct.Teacher)
                    .Where(ct => ct.CourseId == currentCourse.Id)
                    .ToList();
                AssignedTeachersListBox.ItemsSource = assignedTeachers;

                // Pre-select assigned teachers in the available teachers list
                var assignedTeacherIds = assignedTeachers.Select(ct => ct.TeacherId).ToList();
                TeachersListBox.SelectedItems.Clear();
                foreach (var teacher in TeachersListBox.ItemsSource.Cast<User>())
                {
                    if (assignedTeacherIds.Contains(teacher.Id))
                    {
                        TeachersListBox.SelectedItems.Add(teacher);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading data:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnAssign_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTeachers = TeachersListBox.SelectedItems?.Cast<User>().ToList();
                if (selectedTeachers == null || selectedTeachers.Count == 0)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select at least one teacher.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                foreach (var teacher in selectedTeachers)
                {
                    // Check if already assigned
                    var existing = dbContext.CourseTeachers
                        .FirstOrDefault(ct => ct.CourseId == currentCourse.Id && ct.TeacherId == teacher.Id);

                    if (existing == null)
                    {
                        var courseTeacher = new CourseTeacher
                        {
                            CourseId = currentCourse.Id,
                            TeacherId = teacher.Id,
                            AssignedAt = DateTime.Now
                        };
                        dbContext.CourseTeachers.Add(courseTeacher);
                    }
                }

                await dbContext.SaveChangesAsync();
                LoadData();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Teachers assigned successfully.", ButtonEnum.Ok).ShowAsync();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error assigning teachers:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnRemove_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAssignment = AssignedTeachersListBox.SelectedItem as CourseTeacher;
                if (selectedAssignment == null)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a teacher to remove.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                // Load from database to ensure it's tracked
                var assignmentToRemove = dbContext.CourseTeachers
                    .FirstOrDefault(ct => ct.Id == selectedAssignment.Id);
                
                if (assignmentToRemove != null)
                {
                    dbContext.CourseTeachers.Remove(assignmentToRemove);
                    await dbContext.SaveChangesAsync();
                    LoadData();
                    await MessageBoxManager.GetMessageBoxStandard("Success", "Teacher removed successfully.", ButtonEnum.Ok).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error removing teacher:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}

