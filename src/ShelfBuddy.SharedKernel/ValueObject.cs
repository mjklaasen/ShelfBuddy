﻿namespace ShelfBuddy.SharedKernel;

public abstract class ValueObject
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return ((ValueObject)other)
            .GetEqualityComponents()
            .SequenceEqual(GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }
}