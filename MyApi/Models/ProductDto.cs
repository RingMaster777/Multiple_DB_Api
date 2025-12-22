namespace MyApi.Models;

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}

/// <summary>
/// DTO for updating an existing product
/// </summary>
public class UpdateProductDto
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal? Price { get; set; }
    
    public int? Stock { get; set; }
}
