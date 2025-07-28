using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaPractice01;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public List<string> FunctionHandler(string input, ILambdaContext context)
    {
        // Create list and add multiple collections
        var fruits = new List<string> { "apple", "banana" };
        fruits.AddRange(new[] { "cherry", "grape", "apple", "pear" });
        // Filter: starts with 'a'
        var filtered = fruits.Where(f => f.StartsWith("a")).ToList();

        return filtered;
    }
}
