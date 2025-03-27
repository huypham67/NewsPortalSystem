using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Entities;

public partial class Article
{
    [Key] // Định nghĩa khóa chính
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ArticleId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Headline { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Source { get; set; }

    public int? CategoryId { get; set; }

    public string Status { get; set; } = "Draft";

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}