using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Assignment4.Entities
{
    public class Task
    { 
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [AllowNull]
        public User AssignedTo { get; set; }
        
        [MaxLength(50)]
        [AllowNull]
        public string Description { get; set; }
        
        [Required]
        public State State { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
