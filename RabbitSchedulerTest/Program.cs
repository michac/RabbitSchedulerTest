// See https://aka.ms/new-console-template for more information

using Akka.Actor;
using Akka.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

Console.WriteLine("Hello, World!");

RabbitHelper.TestRabbitConnection();

var builder = new HostApplicationBuilder();

builder.Services.AddAkka("sample-system",
    builder => { builder.WithActors((system, _) => { system.ActorOf<ConnectionTestActor>(); }); });

var host = builder.Build();

await host.RunAsync();

class TestMessage
{
}

class ConnectionTestActor : ReceiveActor
{
    public ConnectionTestActor()
    {
        ReceiveAsync<TestMessage>(OnTestMessageAsync);
    }

    protected override void PreStart()
    {
        Self.Tell(new TestMessage());
    }

    private Task OnTestMessageAsync(TestMessage arg)
    {
        RabbitHelper.TestRabbitConnection();
        
        return Task.CompletedTask;
        
        // Running the CreateConnection method with Task.Run
        //   sets the current scheduler back to the default
        //   so the MainLoop can start.
        // await RabbitHelper.TestRabbitConnectionAsync();
    }
}

public static class RabbitHelper
{
    public static async Task TestRabbitConnectionAsync()
    {
        try
        {
            Console.WriteLine("Invoking: TestRabbitConnection");

            var factory = new ConnectionFactory()
            {
                Password = "password",
                UserName = "tester",
                Port = 5672,
                HostName = "localhost"
            };

            var connection = await Task.Run(() => factory.CreateConnection());

            Console.WriteLine("Connected!");

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void TestRabbitConnection()
    {
        try
        {
            Console.WriteLine("Invoking: TestRabbitConnection");

            var factory = new ConnectionFactory()
            {
                Password = "password",
                UserName = "tester",
                Port = 5672,
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();

            Console.WriteLine("Connected!");

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}