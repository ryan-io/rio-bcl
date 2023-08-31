// See https://aka.ms/new-console-template for more information

using BCL.Serialization;
using Test.Benchmark;
using Test.Code;
using Test.Environment;

var tests = new IntegrationTests_Serializer();
var output = tests.Test_SerializeString();

Console.WriteLine("SaveLocation: " + output.Path);
Console.WriteLine("SaveStatus: " + output.WasSuccessful);


var serializer = new Serializer();
var deserializedString = serializer.DeserializeJson<string>(output.Path);

Console.WriteLine("Lets see if this is empty: " + deserializedString);
Console.ReadLine();

