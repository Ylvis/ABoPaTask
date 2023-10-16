using System.ComponentModel.DataAnnotations;

namespace ABoPaTask.API.Classes
{
    public class Result
    {
        [Key] public int Id { get; set; }
        [Required] public string deviceToken { get; set; }
        [Required] public string X_Name { get; set; }
        [Required] public string value { get; set; }
    }
}
