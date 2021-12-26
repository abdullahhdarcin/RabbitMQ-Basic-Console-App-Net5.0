using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.Publisher
{
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
                channel.QueueDeclare("hello-queue", true, false, false);

                //uygulama her çaıştırdığımda 50 mesaj gidecek.

                Enumerable.Range(0, 50).ToList().ForEach(x =>
                {
                    string message = $"Message = {x}";

                    var messageBody = Encoding.UTF8.GetBytes(message);


                    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                    Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
                    

                });

                Console.ReadLine();


            }
        }
    }
}
