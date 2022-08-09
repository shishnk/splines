namespace Splines.Models.Spline;

public static class HermiteBasis
{
    public static double Psi1(double x, double h)
    => 1 - 3 * x * x + 2 * x * x * x; // $1 - 3\xi^2 + 2\xi^3$

    public static double Psi2(double x, double h)
    => h * (x - 2 * x * x + x * x * x); // $h_i \cdot (\xi - 2\xi^2 + \xi^3)$

    public static double Psi3(double x, double h)
     => 3 * x * x - 2 * x * x * x; // $3\xi^2 - 2\xi^3$

    public static double Psi4(double x, double h)
    => h * (-x * x + x * x * x); // $h_i \cdot (-\xi^2 + \xi^3)$

    public static double dPsi1(double x, double h)
    => -6 * (x - x * x) / h; // $\dfrac{-6 \cdot (\xi - \xi^2)}{h_i}$

    public static double dPsi2(double x, double h)
    => 1 - 4 * x + 3 * x * x; // $1 - 4\xi + 3\xi^2$

    public static double dPsi3(double x, double h)
    => 6 * (x - x * x) / h; // $\dfrac{6 \cdot (\xi - \xi^2)}{h_i}$

    public static double dPsi4(double x, double h)
    => -2 * x + 3 * x * x; // $-2\xi + 3\xi^2$

    public static double ddPsi1(double x, double h)
    => -6 * (1 - 2 * x) / (h * h); // $\dfrac{-6 \cdot (1 - 2\xi)}{h_i^2}$

    public static double ddPsi2(double x, double h)
    => (-4 + 6 * x) / h; // $\dfrac{-4 + 6\xi}{h_i}$

    public static double ddPsi3(double x, double h)
    => 6 * (1 - 2 * x) / (h * h); // $\dfrac{6 \cdot (1 - 2\xi)}{h_i^2}$

    public static double ddPsi4(double x, double h)
    => (-2 + 6 * x) / h; // $\dfrac{-2 + 6\xi}{h_i}$
}