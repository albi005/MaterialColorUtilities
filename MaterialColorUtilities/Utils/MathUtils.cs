namespace MaterialColorUtilities.Utils;

public class MathUtils
{
    /**
     * The linear interpolation function.
     *
     * @return start if amount = 0 and stop if amount = 1
     */
    public static double Lerp(double start, double stop, double amount)
    {
        return (1.0 - amount) * start + amount * stop;
    }

    /**
     * Clamps an integer between two integers.
     *
     * @return input when min <= input <= max, and either min or max otherwise.
     */
    public static int ClampInt(int min, int max, int input)
    {
        if (input < min)
            return min;
        if (input > max)
            return max;
        return input;
    }

    /**
     * Clamps an integer between two floating-point numbers.
     *
     * @return input when min <= input <= max, and either min or max otherwise.
     */
    public static double ClampDouble(double min, double max, double input)
    {
        if (input < min)
            return min;
        if (input > max)
            return max;
        return input;
    }

    /**
     * Sanitizes a degree measure as an integer.
     *
     * @return a degree measure between 0 (inclusive) and 360 (exclusive).
     */
    public static int SanitizeDegreesInt(int degrees)
    {
        degrees %= 360;
        if (degrees < 0)
        {
            degrees += 360;
        }
        return degrees;
    }

    /**
     * Sanitizes a degree measure as a floating-point number.
     *
     * @return a degree measure between 0.0 (inclusive) and 360.0 (exclusive).
     */
    public static double SanitizeDegreesDouble(double degrees)
    {
        degrees %= 360.0;
        if (degrees < 0)
        {
            degrees += 360.0;
        }
        return degrees;
    }

    /** Distance of two points on a circle, represented using degrees. */
    public static double DifferenceDegrees(double a, double b)
    {
        return 180.0 - Math.Abs(Math.Abs(a - b) - 180.0);
    }

    public static double[] MatrixMultiply(double[] row, double[][] matrix)
    {
        double a = row[0] * matrix[0][0] + row[1] * matrix[0][1] + row[2] * matrix[0][2];
        double b = row[0] * matrix[1][0] + row[1] * matrix[1][1] + row[2] * matrix[1][2];
        double c = row[0] * matrix[2][0] + row[1] * matrix[2][1] + row[2] * matrix[2][2];
        return new double[] { a, b, c };
    }

    public static double ToRadians(double angdeg) => Math.PI / 180 * angdeg;
    public static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);
    public static double Log1p(double x) => Math.Log(1 + x);
    public static double Expm1(double x) => Math.Exp(x) - 1;
}
