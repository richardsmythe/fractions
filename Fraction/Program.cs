namespace Fraction
{

    public class Program
    {
        static void Main(string[] args)
        {   
            Console.WriteLine("Using fraction objects:");
            Fraction frac1 = new Fraction(-1, 2);
            Fraction frac2 = new Fraction(1, 2);
            Fraction frac3 = new Fraction(1, 3);

            Fraction additionResult = Fraction.Add(frac1, frac2, frac3);
            Console.WriteLine($"{frac1} + {frac2} + {frac3} = {additionResult}");

            Fraction subtractionResult = Fraction.Subtraction(frac1, frac2);
            Console.WriteLine($"{frac1} - {frac2} = {subtractionResult}");

            Fraction multiplicationResult = Fraction.Multiplication(frac1, frac2);
            Console.WriteLine($"{frac1} * {frac2} = {multiplicationResult}");

            Fraction divisionResult = Fraction.Division(frac1, frac2);
            Console.WriteLine($"{frac1} / {frac2} = {divisionResult}");

            Console.WriteLine("\nUsing strings:");
            
            string addExpr = "-1/2+1/2+1/3";
            Fraction addResult = Fraction.Add(addExpr);
            Console.WriteLine($"{addExpr} = {addResult}");

            string subExpr = "1/2-1/3-1/6";
            Fraction subResult = Fraction.Subtract(subExpr);
            Console.WriteLine($"{subExpr} = {subResult}");


            string multExpr = "2/3*3/4*4/5";
            Fraction multResult = Fraction.Multiply(multExpr);
            Console.WriteLine($"{multExpr} = {multResult}");

            string divExpr = "1/2/3/4";
            Fraction divResult = Fraction.Divide(divExpr);
            Console.WriteLine($"{divExpr} = {divResult}");

            Console.WriteLine("\nNegative nums:");
            string negExpr = "-7/10+2/15";
            Fraction negResult = Fraction.Add(negExpr);
            Console.WriteLine($"{negExpr} = {negResult}");

            string negSubExpr = "-3/4-1/4";
            Fraction negSubResult = Fraction.Subtract(negSubExpr);
            Console.WriteLine($"{negSubExpr} = {negSubResult}");
           
            string mixedExpr = "1/2+1/3-1/6";
            Fraction mixedResult = Fraction.Add(mixedExpr);
            Console.WriteLine($"{mixedExpr} = {mixedResult}");
        }
    }
}
