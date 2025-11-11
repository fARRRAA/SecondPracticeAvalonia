using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Пользователи системы (студенты, преподаватели, администраторы)
/// </summary>
public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsBlocked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissionCheckedByNavigations { get; set; } = new List<AssignmentSubmission>();

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissionStudents { get; set; } = new List<AssignmentSubmission>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<DiscussionReply> DiscussionReplies { get; set; } = new List<DiscussionReply>();

    public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual NotificationSetting? NotificationSetting { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual StudentPoint? StudentPoint { get; set; }

    public virtual ICollection<UserActivityLog> UserActivityLogs { get; set; } = new List<UserActivityLog>();

    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

    public virtual ICollection<WebinarParticipant> WebinarParticipants { get; set; } = new List<WebinarParticipant>();

    public virtual ICollection<Webinar> Webinars { get; set; } = new List<Webinar>();
}
