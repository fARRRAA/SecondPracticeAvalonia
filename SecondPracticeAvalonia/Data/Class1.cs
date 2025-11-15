//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SecondPracticeAvalonia.Data
//{
//    internal class Class1
//    {
//    }
//    public partial class AnswerOption
//    {
//        public int Id { get; set; }

//        public int? QuestionId { get; set; }

//        public string OptionText { get; set; } = null!;

//        public bool? IsCorrect { get; set; }

//        public int OrderIndex { get; set; }

//        public virtual TestQuestion? Question { get; set; }

//        public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
//    }
//    public partial class Assignment
//    {
//        public int Id { get; set; }

//        public int? LessonId { get; set; }

//        public string Title { get; set; } = null!;

//        public string Description { get; set; } = null!;

//        public int TypeId { get; set; }

//        public int? MaxScore { get; set; }

//        public DateTime? Deadline { get; set; }

//        public bool? IsRequired { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();

//        public virtual Lesson? Lesson { get; set; }

//        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

//        public virtual AssignmentType Type { get; set; } = null!;
//    }
//    public partial class AssignmentSubmission
//    {
//        public int Id { get; set; }

//        public int? AssignmentId { get; set; }

//        public int? StudentId { get; set; }

//        public int? StatusId { get; set; }

//        public string? Content { get; set; }

//        public string? FileUrl { get; set; }

//        public int? Score { get; set; }

//        public string? TeacherComment { get; set; }

//        public DateTime? SubmittedAt { get; set; }

//        public DateTime? CheckedAt { get; set; }

//        public int? CheckedBy { get; set; }

//        public virtual Assignment? Assignment { get; set; }

//        public virtual User? CheckedByNavigation { get; set; }

//        public virtual AssignmentStatus? Status { get; set; }

//        public virtual User? Student { get; set; }

//        public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
//    }
//    public partial class TestQuestion
//    {
//        public int Id { get; set; }

//        public int? AssignmentId { get; set; }

//        public string QuestionText { get; set; } = null!;

//        public string QuestionType { get; set; } = null!;

//        public int OrderIndex { get; set; }

//        public int? Points { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();

//        public virtual Assignment? Assignment { get; set; }

//        public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
//    }
//    public partial class TestAnswer
//    {
//        public int Id { get; set; }

//        public int? SubmissionId { get; set; }

//        public int? QuestionId { get; set; }

//        public int? SelectedOptionId { get; set; }

//        public string? TextAnswer { get; set; }

//        public bool? IsCorrect { get; set; }

//        public int? PointsEarned { get; set; }

//        public virtual TestQuestion? Question { get; set; }

//        public virtual AnswerOption? SelectedOption { get; set; }

//        public virtual AssignmentSubmission? Submission { get; set; }
//    }
//    public partial class User
//    {
//        public int Id { get; set; }

//        public string Email { get; set; } = null!;

//        public string PasswordHash { get; set; } = null!;

//        public string FirstName { get; set; } = null!;

//        public string LastName { get; set; } = null!;

//        public int RoleId { get; set; }

//        public string? AvatarUrl { get; set; }

//        public string? Phone { get; set; }

//        public bool? IsActive { get; set; }

//        public bool? IsBlocked { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public DateTime? UpdatedAt { get; set; }

//        public DateTime? LastLoginAt { get; set; }

//        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

//        public virtual ICollection<AssignmentSubmission> AssignmentSubmissionCheckedByNavigations { get; set; } = new List<AssignmentSubmission>();

//        public virtual ICollection<AssignmentSubmission> AssignmentSubmissionStudents { get; set; } = new List<AssignmentSubmission>();

//        public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

//        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

//        public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

//        public virtual ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

//        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

//        public virtual ICollection<DiscussionReply> DiscussionReplies { get; set; } = new List<DiscussionReply>();

//        public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

//        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

//        public virtual NotificationSetting? NotificationSetting { get; set; }

//        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

//        public virtual ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();

//        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

//        public virtual UserRole Role { get; set; } = null!;

//        public virtual StudentPoint? StudentPoint { get; set; }

//        public virtual ICollection<UserActivityLog> UserActivityLogs { get; set; } = new List<UserActivityLog>();

//        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

//        public virtual ICollection<WebinarParticipant> WebinarParticipants { get; set; } = new List<WebinarParticipant>();

//        public virtual ICollection<Webinar> Webinars { get; set; } = new List<Webinar>();
//    }

//    public partial class Course
//    {
//        public int Id { get; set; }

//        public string Title { get; set; } = null!;

//        public string Description { get; set; } = null!;

//        public int? CategoryId { get; set; }

//        public int LevelId { get; set; }

//        public int FormatId { get; set; }

//        public int? StatusId { get; set; }

//        public int? DurationHours { get; set; }

//        public decimal? Price { get; set; }

//        public string? CoverImageUrl { get; set; }

//        public decimal? Rating { get; set; }

//        public int? TotalReviews { get; set; }

//        public int? TotalStudents { get; set; }

//        public bool? IsFeatured { get; set; }

//        public int? CreatedBy { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

//        public virtual CourseCategory? Category { get; set; }

//        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

//        public virtual ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();

//        public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

//        public virtual CourseStatistic? CourseStatistic { get; set; }

//        public virtual ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

//        public virtual User? CreatedByNavigation { get; set; }

//        public virtual CourseFormat Format { get; set; } = null!;

//        public virtual CourseLevel Level { get; set; } = null!;

//        public virtual ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();

//        public virtual CourseStatus? Status { get; set; }

//        public virtual ICollection<Webinar> Webinars { get; set; } = new List<Webinar>();
//    }
//    public partial class CourseModule
//    {
//        public int Id { get; set; }

//        public int? CourseId { get; set; }

//        public string Title { get; set; } = null!;

//        public string? Description { get; set; }

//        public int OrderIndex { get; set; }

//        public bool? IsPublished { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public DateTime? UpdatedAt { get; set; }

//        public virtual Course? Course { get; set; }

//        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
//    }
//    public partial class Lesson
//    {
//        public int Id { get; set; }

//        public int? ModuleId { get; set; }

//        public string Title { get; set; } = null!;

//        public string? Content { get; set; }

//        public string? VideoUrl { get; set; }

//        public int? DurationMinutes { get; set; }

//        public int OrderIndex { get; set; }

//        public bool? IsPublished { get; set; }

//        public DateTime? CreatedAt { get; set; }

//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

//        public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

//        public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

//        public virtual ICollection<LessonMaterial> LessonMaterials { get; set; } = new List<LessonMaterial>();

//        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

//        public virtual CourseModule? Module { get; set; }
//    }

//}
