using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Учебные курсы
/// </summary>
public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int? DurationHours { get; set; }

    public decimal? Price { get; set; }

    public string? CoverImageUrl { get; set; }

    public decimal? Rating { get; set; }

    public int? TotalReviews { get; set; }

    public int? TotalStudents { get; set; }

    public bool? IsFeatured { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual CourseStatistic? CourseStatistic { get; set; }

    public virtual ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();

    public virtual ICollection<Webinar> Webinars { get; set; } = new List<Webinar>();
}
