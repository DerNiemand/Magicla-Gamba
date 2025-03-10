using System;

public record PlayerId(long Value)
{
    public bool Equals(long number)
    {
        return (Value == number);
    }
}

public record LoginId(Guid Value);
