namespace Efac.Domain.Services;

public static class DianModulo11Calculator
{
    private static readonly int[] PrimeFactors =
    [
        3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71
    ];

    public static int CalculateVerificationDigit(string nit)
    {
        if (string.IsNullOrWhiteSpace(nit))
        {
            throw new ArgumentException("El NIT es obligatorio", nameof(nit));
        }

        var normalizedNit = NormalizeNit(nit);
        if (normalizedNit.Length == 0)
        {
            throw new ArgumentException("El NIT debe contener solo digitos", nameof(nit));
        }

        var sum = 0;
        var factorIndex = 0;

        for (var index = normalizedNit.Length - 1; index >= 0; index--)
        {
            if (factorIndex >= PrimeFactors.Length)
            {
                throw new ArgumentException("El NIT supera la longitud soportada por el algoritmo DIAN", nameof(nit));
            }

            var digit = normalizedNit[index] - '0';
            sum += digit * PrimeFactors[factorIndex];
            factorIndex++;
        }

        var remainder = sum % 11;
        return remainder > 1 ? 11 - remainder : remainder;
    }

    public static string NormalizeNit(string nit)
    {
        return new string(nit.Where(char.IsDigit).ToArray());
    }
}
