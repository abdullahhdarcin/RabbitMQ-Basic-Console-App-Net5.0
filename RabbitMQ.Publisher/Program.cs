using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Information = 4,
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            //rabbit mq bağlantısı 1
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lrlnofne:N9OpFwWPajvM8k5rEtIXN99ZxixfUQS-@roedeer.rmq.cloudamqp.com/lrlnofne");

            //bağlantı açma- kurma 2
            using (var connection = factory.CreateConnection())
            {
                //kanal oluşturma 3
                var channel = connection.CreateModel();

                //mesajlar boşa gitmemesi için bir kuyruk oluşturulması gerekiyor. 4
                //durable false olursa memory de tutulur ve rabbitmqya restart attığın için kaybolur. 5 (true)
                //autoDelete eğer consumer kapatırsa yanlışlıkla giderse queue kalsın istediğim için false veriyorum. kuyruk düşmesin istiyorum.
                //channel.QueueDeclare("hello-queue", true, false, false);

                //Fanout
                //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

                //Direct
                channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);


                Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
                {
                    var routeKey = $"route-{x}";
                    var queueName = $"direct-queue-{x}";
                    channel.QueueDeclare(queueName, true, false, false);
                    channel.QueueBind(queueName, "logs-direct", routeKey,null);
                });


                //uygulama her çaıştırdığımda 50 mesaj gidecek.
                Enumerable.Range(0, 50).ToList().ForEach(x =>
                {
                    LogNames log = (LogNames)new Random().Next(1,5);


                    string message = $"log-type :  {log}";
                    var messageBody = Encoding.UTF8.GetBytes(message);

;                    var routeKey = $"route-{log}";

                    channel.BasicPublish("logs-direct",routeKey, null, messageBody);
                    Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
                    

                });

                Console.ReadLine();


            }
        }
    }
}
