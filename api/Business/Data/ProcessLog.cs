using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

/*
    Logging Entity - defines the database table structure for logs
    Properties: Id, Timestamp, Level, Message, StackTrace, PersonName, Action
    EF core configuration: Table name, primary key, constraints
*/

namespace StargateAPI.Business.Data
{
    [Table("ProcessLog")]
    public class ProcessLog
    {
        // Primary key - auto-incrementing ID
        public int Id { get; set; }
        // When did this event happen?
        public DateTime Timestamp { get; set; }
        // What type of log is this? e.g., "Info", "Error", "Exception"
        public string Level { get; set; } = string.Empty;
        // What was the operation? e.g., "CreatePerson", "CreateAstronautDuty"
        public string Action { get; set; } = string.Empty;
        // Human-readable description of what happened
        public string Message { get; set; } = string.Empty;
        // Stack trace (only for exceptions) - nullable because not always present
        public string? StackTrace { get; set; }
        // Which person was this operation about? - nullable because not always present
        public string? PersonName { get; set; }
    }

    // Entity framework core configuration - defines how this maps to the database
    // Separate entity definition from database configuration
    public class ProcessLogConfiguration : IEntityTypeConfiguration<ProcessLog>
    {
        public void Configure(EntityTypeBuilder<ProcessLog> builder)
        {
            // Set Id as primary key
            builder.HasKey(x => x.Id);
            // Id auto-increments (1, 2, 3, ...)
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            // Timestamp is required (can't be null)
            builder.Property(x => x.Timestamp).IsRequired();
            // Level is required - examples: "Info", "Error", "Exception"
            builder.Property(x => x.Level)
                .IsRequired()
                .HasMaxLength(50);
            // Action is required - examples: "CreatePerson", "CreateAstronautDuty", "UpdatePerson"
            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);
            // Message is required, no max length (may be long)
            builder.Property(x => x.Message).IsRequired();
            // StackTrace is optional (nullable) - only populated for exceptions
            builder.Property(x => x.StackTrace).IsRequired(false);
            // PersonName is optional (nullable)
            builder.Property(x => x.PersonName)
                .IsRequired(false)
                .HasMaxLength(100);
            // Add index on Timestamp for faster queries - e.g. "Show me logs from last week"
            builder.HasIndex(x => x.Timestamp);
            // Add index on Level for faster filtering - e.g. "Show me all exceptions"
            builder.HasIndex(x => x.Level);
        }
    }
}