using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Consumer
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

                //true olursa kaç tane consumer varsa tek bir sefer de 5 olacak şekilde ayarlar. 3 ona 2 ona diye gidebilir. yani toplam sayıyı belirtir. false yaparsam
                // consumer için kaçar tane gideceğini belirtir.
                //channel.BasicQos(0, 1, false);
                channel.BasicQos(0, 2, true);

                //mesajlar boşa gitmemesi için bir kuyruk oluşturulması gerekiyor. 4
                //durable false olursa memory de tutulur ve rabbitmqya restart attığın için kaybolur. 5 (true)
                //autoDelete eğer consumer kapatırsa yanlışlıkla giderse queue kalsın istediğim için false veriyorum. kuyruk düşmesin istiyorum.
                //channel.QueueDeclare("hello-queue", true, false, false);

                var consumer = new EventingBasicConsumer(channel);

                //faşse değeri mesajı silme benim komutumu bekle diyorum.
                channel.BasicConsume("hello-queue", false, consumer);

                consumer.Received += (object sender, BasicDeliverEventArgs e) =>
                {
                    //mesajı alıyoruz şuan
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());

                    Thread.Sleep(1500);

                    Console.WriteLine("Gelen Mesaj : " + message);

                    channel.BasicAck(e.DeliveryTag, false);
                };

                //mesajları consumer a kaçar tane alacağımı belirtiyorum.

                Console.ReadLine();


            }
        }

    }
}
