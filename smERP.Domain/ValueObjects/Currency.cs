using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smERP.Domain.ValueObjects;

public class Currency : ValueObject
{
    public string Code { get; }
    public string Symbol { get; }
    public string Name { get; }
    public int DecimalPlaces { get; }

    private Currency(string code, string symbol, string name, int decimalPlaces)
    {
        Code = code;
        Symbol = symbol;
        Name = name;
        DecimalPlaces = decimalPlaces;
    }

    public static Currency Create(string code, string symbol, string name, int decimalPlaces)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code cannot be empty.", nameof(code));

        if (code.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters long.", nameof(code));

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Currency symbol cannot be empty.", nameof(symbol));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Currency name cannot be empty.", nameof(name));

        if (decimalPlaces < 0)
            throw new ArgumentException("Decimal places cannot be negative.", nameof(decimalPlaces));

        return new Currency(code.ToUpperInvariant(), symbol, name, decimalPlaces);
    }

    // Predefined currencies
    public static Currency EGP => new Currency("EGP", "جم", "Egyptian Pound", 2);
    public static Currency USD => new Currency("USD", "$", "US Dollar", 2);
    public static Currency EUR => new Currency("EUR", "€", "Euro", 2);
    public static Currency GBP => new Currency("GBP", "£", "British Pound", 2);
    public static Currency JPY => new Currency("JPY", "¥", "Japanese Yen", 0);
    // Add more currencies as needed

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString()
    {
        return $"{Code} ({Symbol})";
    }

    // Method to format an amount in this currency
    public string FormatAmount(decimal amount)
    {
        return $"{Symbol}{amount.ToString($"F{DecimalPlaces}")}";
    }
}
