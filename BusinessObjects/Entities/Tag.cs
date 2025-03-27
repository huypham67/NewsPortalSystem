using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Tag
{
    public int TagId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
