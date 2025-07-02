using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Library.Specflow
{
    public class ScenarioLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenarioLogId { get; set; }

        public DateTime LogDateTime
        {
            get { return DateTime.Now; }
            set { }
        }

        public string Message { get; set; }
    }
}