using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecondPracticeAvalonia.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AnswerOption> AnswerOptions { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<AssignmentStatus> AssignmentStatuses { get; set; }

    public virtual DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }

    public virtual DbSet<AssignmentType> AssignmentTypes { get; set; }

    public virtual DbSet<Badge> Badges { get; set; }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<CourseFormat> CourseFormats { get; set; }

    public virtual DbSet<CourseLevel> CourseLevels { get; set; }

    public virtual DbSet<CourseModule> CourseModules { get; set; }

    public virtual DbSet<CourseReview> CourseReviews { get; set; }

    public virtual DbSet<CourseStatistic> CourseStatistics { get; set; }

    public virtual DbSet<CourseStatus> CourseStatuses { get; set; }

    public virtual DbSet<CourseTeacher> CourseTeachers { get; set; }

    public virtual DbSet<Discussion> Discussions { get; set; }

    public virtual DbSet<DiscussionReply> DiscussionReplies { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonMaterial> LessonMaterials { get; set; }

    public virtual DbSet<LessonProgress> LessonProgresses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<PromotionUsage> PromotionUsages { get; set; }

    public virtual DbSet<StudentPoint> StudentPoints { get; set; }

    public virtual DbSet<TestAnswer> TestAnswers { get; set; }

    public virtual DbSet<TestQuestion> TestQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

    public virtual DbSet<UserBadge> UserBadges { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Webinar> Webinars { get; set; }

    public virtual DbSet<WebinarParticipant> WebinarParticipants { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=micialware.ru;Port=5432;Database=secondprdb;Username=trieco_admin;Password=trieco");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("announcements_pkey");

            entity.ToTable("announcements");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("is_published");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("announcements_course_id_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("announcements_teacher_id_fkey");
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("answer_options_pkey");

            entity.ToTable("answer_options");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false)
                .HasColumnName("is_correct");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.AnswerOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("answer_options_question_id_fkey");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("assignments_pkey");

            entity.ToTable("assignments", tb => tb.HasComment("Задания и тесты"));

            entity.HasIndex(e => e.LessonId, "idx_assignments_lesson");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Deadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deadline");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsRequired)
                .HasDefaultValue(true)
                .HasColumnName("is_required");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.MaxScore)
                .HasDefaultValue(100)
                .HasColumnName("max_score");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("assignments_lesson_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("assignments_type_id_fkey");
        });

        modelBuilder.Entity<AssignmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("assignment_statuses_pkey");

            entity.ToTable("assignment_statuses", tb => tb.HasComment("Справочник статусов заданий"));

            entity.HasIndex(e => e.Name, "assignment_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<AssignmentSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("assignment_submissions_pkey");

            entity.ToTable("assignment_submissions", tb => tb.HasComment("Выполненные задания студентов"));

            entity.HasIndex(e => e.StudentId, "idx_assignment_submissions_student");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.CheckedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("checked_at");
            entity.Property(e => e.CheckedBy).HasColumnName("checked_by");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("file_url");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("status_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("submitted_at");
            entity.Property(e => e.TeacherComment).HasColumnName("teacher_comment");

            entity.HasOne(d => d.Assignment).WithMany(p => p.AssignmentSubmissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("assignment_submissions_assignment_id_fkey");

            entity.HasOne(d => d.CheckedByNavigation).WithMany(p => p.AssignmentSubmissionCheckedByNavigations)
                .HasForeignKey(d => d.CheckedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("assignment_submissions_checked_by_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.AssignmentSubmissions)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("assignment_submissions_status_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.AssignmentSubmissionStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("assignment_submissions_student_id_fkey");
        });

        modelBuilder.Entity<AssignmentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("assignment_types_pkey");

            entity.ToTable("assignment_types", tb => tb.HasComment("Справочник типов заданий"));

            entity.HasIndex(e => e.Name, "assignment_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("badges_pkey");

            entity.ToTable("badges", tb => tb.HasComment("Достижения и бейджи"));

            entity.HasIndex(e => e.Name, "badges_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(500)
                .HasColumnName("icon_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PointsRequired).HasColumnName("points_required");
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookmarks_pkey");

            entity.ToTable("bookmarks");

            entity.HasIndex(e => new { e.StudentId, e.LessonId, e.PositionSeconds }, "bookmarks_student_id_lesson_id_position_seconds_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PositionSeconds)
                .HasDefaultValue(0)
                .HasColumnName("position_seconds");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookmarks_lesson_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookmarks_student_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("courses_pkey");

            entity.ToTable("courses", tb => tb.HasComment("Учебные курсы"));

            entity.HasIndex(e => e.CategoryId, "idx_courses_category");

            entity.HasIndex(e => e.StatusId, "idx_courses_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(500)
                .HasColumnName("cover_image_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationHours).HasColumnName("duration_hours");
            entity.Property(e => e.FormatId).HasColumnName("format_id");
            entity.Property(e => e.IsFeatured)
                .HasDefaultValue(false)
                .HasColumnName("is_featured");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("price");
            entity.Property(e => e.Rating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("rating");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("status_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TotalReviews)
                .HasDefaultValue(0)
                .HasColumnName("total_reviews");
            entity.Property(e => e.TotalStudents)
                .HasDefaultValue(0)
                .HasColumnName("total_students");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("courses_category_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("courses_created_by_fkey");

            entity.HasOne(d => d.Format).WithMany(p => p.Courses)
                .HasForeignKey(d => d.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("courses_format_id_fkey");

            entity.HasOne(d => d.Level).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("courses_level_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Courses)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("courses_status_id_fkey");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_categories_pkey");

            entity.ToTable("course_categories");

            entity.HasIndex(e => e.Name, "course_categories_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(500)
                .HasColumnName("icon_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_enrollments_pkey");

            entity.ToTable("course_enrollments", tb => tb.HasComment("Записи студентов на курсы"));

            entity.HasIndex(e => new { e.StudentId, e.CourseId }, "course_enrollments_student_id_course_id_key").IsUnique();

            entity.HasIndex(e => e.CourseId, "idx_course_enrollments_course");

            entity.HasIndex(e => e.StudentId, "idx_course_enrollments_student");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CertificateUrl)
                .HasMaxLength(500)
                .HasColumnName("certificate_url");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_enrollments_course_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_enrollments_student_id_fkey");
        });

        modelBuilder.Entity<CourseFormat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_formats_pkey");

            entity.ToTable("course_formats", tb => tb.HasComment("Справочник форматов курсов"));

            entity.HasIndex(e => e.Name, "course_formats_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CourseLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_levels_pkey");

            entity.ToTable("course_levels", tb => tb.HasComment("Справочник уровней сложности курсов"));

            entity.HasIndex(e => e.Name, "course_levels_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CourseModule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_modules_pkey");

            entity.ToTable("course_modules", tb => tb.HasComment("Модули курсов"));

            entity.HasIndex(e => new { e.CourseId, e.OrderIndex }, "course_modules_course_id_order_index_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("is_published");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseModules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_modules_course_id_fkey");
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_reviews_pkey");

            entity.ToTable("course_reviews");

            entity.HasIndex(e => new { e.CourseId, e.StudentId }, "course_reviews_course_id_student_id_key").IsUnique();

            entity.HasIndex(e => e.CourseId, "idx_course_reviews_course");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_reviews_course_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_reviews_student_id_fkey");
        });

        modelBuilder.Entity<CourseStatistic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_statistics_pkey");

            entity.ToTable("course_statistics");

            entity.HasIndex(e => e.CourseId, "course_statistics_course_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("average_rating");
            entity.Property(e => e.CompletionRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("completion_rate");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated");
            entity.Property(e => e.TotalCompletions)
                .HasDefaultValue(0)
                .HasColumnName("total_completions");
            entity.Property(e => e.TotalEnrollments)
                .HasDefaultValue(0)
                .HasColumnName("total_enrollments");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("total_revenue");

            entity.HasOne(d => d.Course).WithOne(p => p.CourseStatistic)
                .HasForeignKey<CourseStatistic>(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_statistics_course_id_fkey");
        });

        modelBuilder.Entity<CourseStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_statuses_pkey");

            entity.ToTable("course_statuses", tb => tb.HasComment("Справочник статусов курсов"));

            entity.HasIndex(e => e.Name, "course_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CourseTeacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_teachers_pkey");

            entity.ToTable("course_teachers");

            entity.HasIndex(e => new { e.CourseId, e.TeacherId }, "course_teachers_course_id_teacher_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseTeachers)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_teachers_course_id_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.CourseTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("course_teachers_teacher_id_fkey");
        });

        modelBuilder.Entity<Discussion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discussions_pkey");

            entity.ToTable("discussions", tb => tb.HasComment("Обсуждения и вопросы по урокам"));

            entity.HasIndex(e => e.LessonId, "idx_discussions_lesson");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsAnswered)
                .HasDefaultValue(false)
                .HasColumnName("is_answered");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Discussions)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("discussions_lesson_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Discussions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("discussions_student_id_fkey");
        });

        modelBuilder.Entity<DiscussionReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discussion_replies_pkey");

            entity.ToTable("discussion_replies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscussionId).HasColumnName("discussion_id");
            entity.Property(e => e.IsTeacher)
                .HasDefaultValue(false)
                .HasColumnName("is_teacher");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Discussion).WithMany(p => p.DiscussionReplies)
                .HasForeignKey(d => d.DiscussionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("discussion_replies_discussion_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.DiscussionReplies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("discussion_replies_user_id_fkey");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lessons_pkey");

            entity.ToTable("lessons", tb => tb.HasComment("Уроки внутри модулей"));

            entity.HasIndex(e => new { e.ModuleId, e.OrderIndex }, "lessons_module_id_order_index_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("is_published");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lessons_module_id_fkey");
        });

        modelBuilder.Entity<LessonMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_materials_pkey");

            entity.ToTable("lesson_materials");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FileSizeKb).HasColumnName("file_size_kb");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("file_type");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("file_url");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Lesson).WithMany(p => p.LessonMaterials)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lesson_materials_lesson_id_fkey");
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_progress_pkey");

            entity.ToTable("lesson_progress", tb => tb.HasComment("Прогресс прохождения уроков"));

            entity.HasIndex(e => e.StudentId, "idx_lesson_progress_student");

            entity.HasIndex(e => new { e.StudentId, e.LessonId }, "lesson_progress_student_id_lesson_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("is_completed");
            entity.Property(e => e.LastPositionSeconds)
                .HasDefaultValue(0)
                .HasColumnName("last_position_seconds");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lesson_progress_lesson_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lesson_progress_student_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications", tb => tb.HasComment("Уведомления пользователей"));

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(500)
                .HasColumnName("link_url");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notifications_type_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_settings_pkey");

            entity.ToTable("notification_settings");

            entity.HasIndex(e => e.UserId, "notification_settings_user_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Announcements)
                .HasDefaultValue(true)
                .HasColumnName("announcements");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeadlineReminders)
                .HasDefaultValue(true)
                .HasColumnName("deadline_reminders");
            entity.Property(e => e.EmailNotifications)
                .HasDefaultValue(true)
                .HasColumnName("email_notifications");
            entity.Property(e => e.GradeNotifications)
                .HasDefaultValue(true)
                .HasColumnName("grade_notifications");
            entity.Property(e => e.NewMaterials)
                .HasDefaultValue(true)
                .HasColumnName("new_materials");
            entity.Property(e => e.PushNotifications)
                .HasDefaultValue(true)
                .HasColumnName("push_notifications");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.NotificationSetting)
                .HasForeignKey<NotificationSetting>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_settings_user_id_fkey");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_types_pkey");

            entity.ToTable("notification_types", tb => tb.HasComment("Справочник типов уведомлений"));

            entity.HasIndex(e => e.Name, "notification_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("promotions_pkey");

            entity.ToTable("promotions", tb => tb.HasComment("Акции и промокоды"));

            entity.HasIndex(e => e.Code, "promotions_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CurrentUses)
                .HasDefaultValue(0)
                .HasColumnName("current_uses");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("discount_amount");
            entity.Property(e => e.DiscountPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("discount_percentage");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxUses).HasColumnName("max_uses");
            entity.Property(e => e.ValidFrom)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("valid_from");
            entity.Property(e => e.ValidUntil)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("valid_until");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("promotions_created_by_fkey");
        });

        modelBuilder.Entity<PromotionUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("promotion_usage_pkey");

            entity.ToTable("promotion_usage");

            entity.HasIndex(e => new { e.PromotionId, e.UserId, e.CourseId }, "promotion_usage_promotion_id_user_id_course_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.UsedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.PromotionUsages)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("promotion_usage_course_id_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionUsages)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("promotion_usage_promotion_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PromotionUsages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("promotion_usage_user_id_fkey");
        });

        modelBuilder.Entity<StudentPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_points_pkey");

            entity.ToTable("student_points");

            entity.HasIndex(e => e.StudentId, "student_points_student_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level)
                .HasDefaultValue(1)
                .HasColumnName("level");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TotalPoints)
                .HasDefaultValue(0)
                .HasColumnName("total_points");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Student).WithOne(p => p.StudentPoint)
                .HasForeignKey<StudentPoint>(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("student_points_student_id_fkey");
        });

        modelBuilder.Entity<TestAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("test_answers_pkey");

            entity.ToTable("test_answers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.PointsEarned)
                .HasDefaultValue(0)
                .HasColumnName("points_earned");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SelectedOptionId).HasColumnName("selected_option_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.TextAnswer).HasColumnName("text_answer");

            entity.HasOne(d => d.Question).WithMany(p => p.TestAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("test_answers_question_id_fkey");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.TestAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("test_answers_selected_option_id_fkey");

            entity.HasOne(d => d.Submission).WithMany(p => p.TestAnswers)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("test_answers_submission_id_fkey");
        });

        modelBuilder.Entity<TestQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("test_questions_pkey");

            entity.ToTable("test_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Points)
                .HasDefaultValue(1)
                .HasColumnName("points");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .HasColumnName("question_type");

            entity.HasOne(d => d.Assignment).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("test_questions_assignment_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Пользователи системы"));

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.RoleId, "idx_users_role");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsBlocked)
                .HasDefaultValue(false)
                .HasColumnName("is_blocked");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_activity_log_pkey");

            entity.ToTable("user_activity_log");

            entity.HasIndex(e => e.UserId, "idx_user_activity_log_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(100)
                .HasColumnName("activity_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Metadata).HasColumnName("metadata");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivityLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_activity_log_user_id_fkey");
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_badges_pkey");

            entity.ToTable("user_badges");

            entity.HasIndex(e => new { e.UserId, e.BadgeId }, "user_badges_user_id_badge_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.EarnedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("earned_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Badge).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.BadgeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_badges_badge_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_badges_user_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_roles_pkey");

            entity.ToTable("user_roles", tb => tb.HasComment("Справочник ролей пользователей"));

            entity.HasIndex(e => e.Name, "user_roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Webinar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("webinars_pkey");

            entity.ToTable("webinars");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.IsLive)
                .HasDefaultValue(false)
                .HasColumnName("is_live");
            entity.Property(e => e.MeetingUrl)
                .HasMaxLength(500)
                .HasColumnName("meeting_url");
            entity.Property(e => e.RecordingUrl)
                .HasMaxLength(500)
                .HasColumnName("recording_url");
            entity.Property(e => e.ScheduledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scheduled_at");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Webinars)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("webinars_course_id_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Webinars)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("webinars_teacher_id_fkey");
        });

        modelBuilder.Entity<WebinarParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("webinar_participants_pkey");

            entity.ToTable("webinar_participants");

            entity.HasIndex(e => new { e.WebinarId, e.StudentId }, "webinar_participants_webinar_id_student_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JoinedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("joined_at");
            entity.Property(e => e.LeftAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("left_at");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.WebinarId).HasColumnName("webinar_id");

            entity.HasOne(d => d.Student).WithMany(p => p.WebinarParticipants)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("webinar_participants_student_id_fkey");

            entity.HasOne(d => d.Webinar).WithMany(p => p.WebinarParticipants)
                .HasForeignKey(d => d.WebinarId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("webinar_participants_webinar_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
