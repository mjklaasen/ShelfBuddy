namespace ShelfBuddy.Contracts;

public record ProductDto(Guid? Id, string Name, ProductCategoryDto ProductCategory);