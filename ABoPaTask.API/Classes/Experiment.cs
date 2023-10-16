using System.ComponentModel.DataAnnotations;

namespace ABoPaTask.API.Classes
{
    public class Experiment
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
    }
}
