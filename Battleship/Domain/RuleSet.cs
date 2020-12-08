using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class MenuResult
    {
        public ExitResult ExitCode { get; set; }
    }
    public class RuleSet : MenuResult
    {
        // Value types, such as decimal, int, float, DateTime, are inherently required and don't need the [Required] attribute.
        [Required]
        [Range(10, 100)]
        public int BoardWidth { get; set; }
        [Required]
        [Range(10, 100)]
        public int BoardHeight { get; set; }
        [Required]
        public int AllowedPlacementType { get; set; }
        [Required]
        public string Ships { get; set; } = null!;

        public override string ToString()
        {
            return $"ExitCode:{ExitCode}," +
                   $"BoardHeight:{BoardHeight}," +
                   $"BoardWidth:{BoardWidth}," +
                   $"AllowedPlacementType:{AllowedPlacementType}," +
                   $"Ships:{Ships}";
        }
    }

    public enum ExitResult
    {
        Start = 1,
        Continue = 2,
        Exit = 4,
    }
}