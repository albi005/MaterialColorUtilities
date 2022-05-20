namespace MaterialColorUtilities.Samples.Console
{
    public class ConsoleHelper
    {
        /// <summary>
        /// Prints every property's name and value in <paramref name="x"/> to the console.
        /// </summary>
        public static void PrintProperties(string title, object x)
        {
            System.Console.WriteLine($"\n - {title} -");
            foreach (var prop in x.GetType().GetProperties())
            {
                object value = prop.GetValue(x);
                System.Console.WriteLine($"{prop.Name}: {value}");
            }
        }
    }
}
