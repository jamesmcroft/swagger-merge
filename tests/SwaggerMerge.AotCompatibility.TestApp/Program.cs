using SwaggerMerge.AotCompatibility.TestApp;

try
{
    SwaggerMergeHandlerAotTest.Test();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return -1;
}

Console.WriteLine("Passed.");
return 0;
