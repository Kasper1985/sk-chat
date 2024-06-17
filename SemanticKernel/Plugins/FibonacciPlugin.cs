using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins;

public class FibonacciPlugin
{
    [KernelFunction("CalculateFibonacci")]
    [Description("Calculates Fibonacci numbers up to a given maximum number")]
    public List<int> CalculateFibonacci(int maxNumber)
    {
        List<int> fibonacciNumbers = new List<int>();

        if (maxNumber < 1)
        {
            return fibonacciNumbers; // Return empty list if maxNumber is less than 1
        }

        int a = 0, b = 1;
        fibonacciNumbers.Add(a);

        while (b <= maxNumber)
        {
            fibonacciNumbers.Add(b);
            int temp = a;
            a = b;
            b = temp + b;
        }

        return fibonacciNumbers;
    }
}
