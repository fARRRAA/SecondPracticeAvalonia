//using NpgsqlTypes;
//using System.Text.Json.Serialization;

//namespace SecondPracticeAvalonia.Models.Enums
//{
//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum UserRole
//    {
//        [PgName("admin")] Admin,
//        [PgName("teacher")] Teacher,
//        [PgName("student")] Student
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum CourseLevel
//    {
//        [PgName("beginner")] Beginner,
//        [PgName("advanced")] Advanced,
//        [PgName("expert")] Expert
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum CourseFormat
//    {
//        [PgName("video")] Video,
//        [PgName("text")] Text,
//        [PgName("webinar")] Webinar,
//        [PgName("mixed")] Mixed
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum CourseStatus
//    {
//        [PgName("draft")] Draft,
//        [PgName("published")] Published,
//        [PgName("archived")] Archived
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum AssignmentType
//    {
//        [PgName("homework")] Homework,
//        [PgName("test")] Test,
//        [PgName("project")] Project,
//        [PgName("quiz")] Quiz
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum AssignmentStatus
//    {
//        [PgName("pending")] Pending,
//        [PgName("submitted")] Submitted,
//        [PgName("checked")] Checked,
//        [PgName("rejected")] Rejected
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum NotificationType
//    {
//        [PgName("deadline")] Deadline,
//        [PgName("new_material")] NewMaterial,
//        [PgName("grade")] Grade,
//        [PgName("announcement")] Announcement,
//        [PgName("message")] Message
//    }
//}